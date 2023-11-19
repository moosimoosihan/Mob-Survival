using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace olimsko
{
    public static class OSManager
    {
        public static event Action OnInitializeStarted;
        public static event Action OnInitializeComplete;

        public static event Action<float> OnInitializationProgress;

        private static UniTaskCompletionSource m_InitializeOSM;

        public static IReadOnlyCollection<Type> Types => typesCache ?? (typesCache = GetEngineTypes());
        private static readonly List<UnityEngine.Object> m_ListObjects = new List<UnityEngine.Object>();
        private static readonly List<IOSMEntity> m_ListEntity = new List<IOSMEntity>();
        private static readonly Dictionary<Type, IOSMEntity> m_DicEntity = new Dictionary<Type, IOSMEntity>();
        public static IOSManagerBehaviour Behaviour { get; private set; }
        private static IReadOnlyCollection<Type> typesCache;
        private static IConfigurationProvider configurationProvider;
        public static bool IsInitialized => m_InitializeOSM != null && m_InitializeOSM.Task.Status == UniTaskStatus.Succeeded;
        public static bool IsInitializing => m_InitializeOSM != null && !(m_InitializeOSM.Task.Status == UniTaskStatus.Succeeded);

        private static readonly List<Func<UniTask>> prepostInitializationTasks = new List<Func<UniTask>>();
        private static readonly List<Func<UniTask>> postInitializationTasks = new List<Func<UniTask>>();

        public static void AddPrePostInitializationTask(Func<UniTask> task) => prepostInitializationTasks.Insert(0, task);
        public static void RemovePrePostInitializationTask(Func<UniTask> task) => prepostInitializationTasks.Remove(task);

        public static void AddPostInitializationTask(Func<UniTask> task) => postInitializationTasks.Insert(0, task);
        public static void RemovePostInitializationTask(Func<UniTask> task) => postInitializationTasks.Remove(task);

        public static async UniTask InitializeAsync(IConfigurationProvider configurationProvider, IOSManagerBehaviour behaviour, IList<IOSMEntity> entities)
        {
            if (IsInitialized)
            {
                return;
            }
            if (IsInitializing)
            {
                await m_InitializeOSM.Task;
                return;
            }

            Behaviour = behaviour;
            m_InitializeOSM = new UniTaskCompletionSource();
            OnInitializeStarted?.Invoke();
            OSManager.configurationProvider = configurationProvider;

            OSManager.m_ListEntity.Clear();
            OSManager.m_ListEntity.AddRange(entities);

            for (int i = prepostInitializationTasks.Count - 1; i >= 0; i--)
            {
                OnInitializationProgress?.Invoke(.75f + .25f * (1 - i / (float)prepostInitializationTasks.Count));
                await prepostInitializationTasks[i]();
                if (!IsInitializing) return;
            }

            for (int i = postInitializationTasks.Count - 1; i >= 0; i--)
            {
                OnInitializationProgress?.Invoke(.75f + .25f * (1 - i / (float)postInitializationTasks.Count));
                await postInitializationTasks[i]();
                if (!IsInitializing) return;
            }
        }

        public static void SetInitialized()
        {
            m_InitializeOSM?.TrySetResult();
            Debug.Log("OSManager Initialized Done.");
            OnInitializeComplete?.Invoke();
        }

        public static T GetConfiguration<T>() where T : Configuration => GetConfiguration(typeof(T)) as T;

        public static Configuration GetConfiguration(Type type)
        {
            if (configurationProvider is null)
                throw new Exception($"Failed to provide `{type.Name}` configuration object: Configuration provider is not available or the engine is not initialized.");

            return configurationProvider.GetConfiguration(type);
        }


        public static TEntity GetService<TEntity>()
            where TEntity : class, IOSMEntity
        {
            return GetService(typeof(TEntity)) as TEntity;
        }

        public static bool TryGetService<TEntity>(out TEntity result)
            where TEntity : class, IOSMEntity
        {
            result = GetService<TEntity>();
            return result != null;
        }

        public static IOSMEntity GetService(Type serviceType)
        {
            if (m_DicEntity.TryGetValue(serviceType, out var cachedResult))
                return cachedResult;
            var result = m_ListEntity.FirstOrDefault(serviceType.IsInstanceOfType);
            if (result is null) return null;
            m_DicEntity[serviceType] = result;
            return result;
        }

        private static IReadOnlyCollection<Type> GetEngineTypes()
        {
            var engineTypes = new List<Type>(1000);
            var assemblyNames = new string[] { "Assembly-CSharp", "Assembly-CSharp-Editor" };
            var domainAssemblies = BaseUtil.GetDomainAssemblies(true, true, true);
            foreach (var assemblyName in assemblyNames)
            {
                var assembly = domainAssemblies.FirstOrDefault(a => a.FullName.StartsWithFast($"{assemblyName},"));
                if (assembly is null) continue;
                engineTypes.AddRange(assembly.GetExportedTypes());
            }
            return engineTypes;
        }

        public static IReadOnlyCollection<TService> FindAllServices<TService>(Predicate<TService> predicate = null)
            where TService : class, IOSMEntity
        {
            var requestedType = typeof(TService);
            var servicesOfType = m_ListEntity.FindAll(requestedType.IsInstanceOfType);
            if (servicesOfType.Count > 0)
                return servicesOfType.FindAll(s => predicate is null || predicate(s as TService)).Cast<TService>().ToArray();
            return Array.Empty<TService>();
        }

        public static T Instantiate<T>(T prototype, string name = default, int? layer = default, Transform parent = default) where T : UnityEngine.Object
        {
            if (Behaviour is null)
                throw new Exception($"Failed to instantiate `{name ?? prototype.name}`: engine is not ready. ");

            var newObj = parent ? UnityEngine.Object.Instantiate(prototype, parent) : UnityEngine.Object.Instantiate(prototype);
            var gameObj = newObj is GameObject newGObj ? newGObj : (newObj as Component)?.gameObject;
            if (!parent) Behaviour.AddChildObject(gameObj);

            if (!string.IsNullOrEmpty(name)) newObj.name = name;

            if (layer.HasValue) gameObj.ForEachDescendant(obj => obj.layer = layer.Value);

            m_ListObjects.Add(newObj);

            return newObj;
        }

    }


}
