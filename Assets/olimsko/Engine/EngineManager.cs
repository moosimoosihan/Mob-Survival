using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace olimsko
{
    [InitializeAtRuntime]
    public class EngineManager : IOSMEntity<EngineConfiguration>
    {
        public EngineConfiguration Configuration { get; }

        private Dictionary<Type, IGoogleSheetData> m_DictionaryGoogleSheetData = new Dictionary<Type, IGoogleSheetData>();

        public EngineManager(EngineConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async UniTask InitializeAsync()
        {

        }


        public void Reset()
        {

        }

        public void DestroyThis()
        {

        }
    }
}