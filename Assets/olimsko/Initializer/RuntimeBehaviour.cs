using System;
using UnityEngine;

namespace olimsko
{

    public class RuntimeBehaviour : MonoBehaviour, IOSManagerBehaviour
    {
        public event Action OnBehaviourUpdate;
        public event Action OnBehaviourLateUpdate;
        public event Action OnBehaviourDestroy;

        private GameObject rootObject;
        private MonoBehaviour monoBehaviour;

        public static RuntimeBehaviour Create(bool dontDestroyOnLoad = true)
        {
            var go = new GameObject("[Runtime] OSManager");
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(go);
            var behaviourComp = go.AddComponent<RuntimeBehaviour>();
            behaviourComp.rootObject = go;
            behaviourComp.monoBehaviour = behaviourComp;
            return behaviourComp;
        }

        public GameObject GetRootObject() => rootObject;

        public void AddChildObject(GameObject obj)
        {
            if (BaseUtil.IsValid(obj))
                obj.transform.SetParent(transform);
        }

        public GameObject AddNewChildObject(string name = null)
        {
            GameObject go = new GameObject();
            go.name = name ?? "EmptySystem";
            go.transform.SetParent(this.transform);
            return go;
        }

        public void Destroy()
        {
            if (monoBehaviour && monoBehaviour.gameObject)
                Destroy(monoBehaviour.gameObject);
        }

        private void Update()
        {
            OnBehaviourUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnBehaviourLateUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            OnBehaviourDestroy?.Invoke();
        }
    }
}
