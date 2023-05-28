using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace olimsko
{
    [InitializeAtRuntime]
    public class ContextManager : IOSMEntity<ContextConfiguration>, IManagedState<GameState>
    {
        private Dictionary<Type, IContextModel> m_DicGetContextResults = new Dictionary<Type, IContextModel>();

        public ContextConfiguration Configuration { get; }

        public ContextManager(ContextConfiguration configuration)
        {
            Configuration = configuration;
        }

        public UniTask InitializeAsync()
        {
            var go = OSManager.Behaviour.GetRootObject();

            var derivedTypes = OSManager.Types
           .Where(t => t.IsClass && !t.IsAbstract && typeof(IContextModel).IsAssignableFrom(t))
           .ToList();

            var context = new GameObject("Context");
            OSManager.Behaviour.AddChildObject(context);

            foreach (System.Type type in derivedTypes)
            {
                var item = context.AddComponent(type);
                m_DicGetContextResults.Add(type, (IContextModel)item);
            }

            return UniTask.CompletedTask;
        }

        public virtual T GetContext<T>() where T : class, IContextModel => GetContext(typeof(T)) as T;

        public virtual IContextModel GetContext(Type type)
        {
            if (m_DicGetContextResults.TryGetValue(type, out var cachedResult))
                return cachedResult;

            // foreach (var managedUI in m_ListManagedUIView)
            //     if (type.IsAssignableFrom(managedUI.ComponentType))
            //     {
            //         var result = managedUI.UIComponent;
            //         m_DicGetUIResults[type] = result;
            //         return managedUI.UIComponent;
            //     }

            return null;
        }

        public void Reset()
        {

        }

        public void DestroyThis()
        {

        }

        public void SaveDataState(GameState state)
        {
            foreach (var item in m_DicGetContextResults)
            {
                item.Value.SaveDataState(state);
            }
        }

        public async UniTask LoadDataStateAsync(GameState state)
        {
            foreach (var item in m_DicGetContextResults)
            {
                await item.Value.LoadDataStateAsync(state);
            }
        }

        public async UniTask InitializeStateAsync()
        {
            foreach (var item in m_DicGetContextResults)
            {
                await item.Value.InitializeStateAsync();
            }
        }
    }

}
