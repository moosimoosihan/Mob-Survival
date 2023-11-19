using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace olimsko
{
    public class RuntimeInitializer : MonoBehaviour
    {
        [SerializeField] private bool m_InitializeOnAwake = true;

        public static bool IsInitialized => InitializeRuntime != null && InitializeRuntime.Task.Status == UniTaskStatus.Succeeded;
        public static UniTaskCompletionSource InitializeRuntime;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnApplicationLoaded()
        {
            InitializeAsync().Forget();
        }

        private void Awake()
        {
            if (!m_InitializeOnAwake) return;
            if (IsInitialized) return;
            InitializeAsync().Forget();
        }

        public static async UniTask InitializeAsync(IConfigurationProvider configurationProvider = null)
        {
            InitializeRuntime = new UniTaskCompletionSource();

            if (configurationProvider is null)
                configurationProvider = new ConfigurationProvider();

            var initData = new List<OSMEntityInitializeData>();
            var overridenTypes = initData.Where(d => d.Override != null).Select(d => d.Override).ToList();
            foreach (var type in OSManager.Types)
            {
                var initAttribute = Attribute.GetCustomAttribute(type, typeof(InitializeAtRuntimeAttribute), false) as InitializeAtRuntimeAttribute;
                if (initAttribute is null) continue;
                initData.Add(new OSMEntityInitializeData(type, initAttribute));
                if (initAttribute.Override != null)
                    overridenTypes.Add(initAttribute.Override);
            }
            initData = initData.Where(d => !overridenTypes.Contains(d.Type)).ToList(); // Exclude services overriden by user.

            bool IsService(Type t) => typeof(IOSMEntity).IsAssignableFrom(t);
            bool IsBehaviour(Type t) => typeof(IOSManagerBehaviour).IsAssignableFrom(t);
            bool IsConfig(Type t) => typeof(Configuration).IsAssignableFrom(t);

            IEnumerable<OSMEntityInitializeData> GetDependencies(OSMEntityInitializeData d) => d.CtorArgs.Where(IsService).SelectMany(argType => initData.Where(dd => d != dd && argType.IsAssignableFrom(dd.Type)));
            initData = initData.OrderBy(d => d.Priority).TopologicalOrder(GetDependencies).ToList();

            var behaviour = RuntimeBehaviour.Create();
            var services = new List<IOSMEntity>();
            var ctorParams = new List<object>();
            foreach (var data in initData)
            {
                foreach (var argType in data.CtorArgs)
                    if (IsService(argType)) ctorParams.Add(services.First(s => argType.IsInstanceOfType(s)));
                    else if (IsBehaviour(argType)) ctorParams.Add(behaviour);
                    else if (IsConfig(argType)) ctorParams.Add(configurationProvider.GetConfiguration(argType));
                    else throw new Exception($"Only `{nameof(Configuration)}`, `{nameof(IOSManagerBehaviour)}` and `{nameof(IOSMEntity)}` with an `{nameof(InitializeAtRuntimeAttribute)}` can be requested in an engine service constructor.");
                var service = Activator.CreateInstance(data.Type, ctorParams.ToArray()) as IOSMEntity;
                services.Add(service);
                ctorParams.Clear();
            }

            await OSManager.InitializeAsync(configurationProvider, behaviour, services);

            foreach (var entity in services)
            {
                await entity.InitializeAsync();
            }

            OSManager.SetInitialized();
            InitializeRuntime.TrySetResult();
        }
    }

}
