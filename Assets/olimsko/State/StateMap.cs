using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace olimsko
{
    [Serializable]
    public class StateMap : ISerializationCallbackReceiver
    {
        [JsonProperty] private Dictionary<string, object> ObjectMap = new Dictionary<string, object>(StringComparer.Ordinal);

        [SerializeField] private SerializableDictionary<string, string> objectJsonMap = new SerializableDictionary<string, string>();

        public virtual void OnBeforeSerialize()
        {
            objectJsonMap.Clear();
            foreach (var kv in ObjectMap)
            {
                string jsonData = OSManager.GetConfiguration<StateConfiguration>().UseNewtonsoftJson ? JsonConvert.SerializeObject(kv.Value) : JsonUtility.ToJson(kv.Value);
                objectJsonMap.Add(kv.Key, jsonData);
            }
        }

        public virtual void OnAfterDeserialize()
        {
            ObjectMap.Clear();
            foreach (var kv in objectJsonMap)
            {
                var type = Type.GetType(kv.Key);
                if (type is null) continue;
                ObjectMap[kv.Key] = OSManager.GetConfiguration<StateConfiguration>().UseNewtonsoftJson ? JsonConvert.DeserializeObject(kv.Value, type) : JsonUtility.FromJson(kv.Value, type);
            }
        }

        public void SetState<TState>(TState state, string instanceId = default) where TState : class, new()
        {
            var key = typeof(TState).AssemblyQualifiedName;
            if (!string.IsNullOrEmpty(instanceId))
                key += $", InstanceID={instanceId}";
            ObjectMap[key] = state;
        }

        public TState GetState<TState>(string instanceId = default) where TState : class, new()
        {
            var key = typeof(TState).AssemblyQualifiedName;
            if (!string.IsNullOrEmpty(instanceId))
                key += $", InstanceID={instanceId}";

            if (OSManager.GetConfiguration<StateConfiguration>().UseNewtonsoftJson)
            {
                if (ObjectMap.TryGetValue(key, out var jState))
                {
                    if (jState is JObject jObject)
                    {
                        return jObject.ToObject<TState>();
                    }
                    else
                    {
                        return jState as TState;
                    }
                }
                else
                {
                    var newState = new TState();
                    ObjectMap[key] = newState;
                    return newState;
                }
            }
            return ObjectMap.TryGetValue(key, out var state) ? state as TState : new TState();
        }
    }
}
