using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace olimsko
{
    [InitializeAtRuntime]
    public class DataManager : IOSMEntity<DataConfiguration>
    {
        public DataConfiguration Configuration { get; }

        private Dictionary<Type, IGoogleSheetData> m_DictionaryGoogleSheetData = new Dictionary<Type, IGoogleSheetData>();

        public DataManager(DataConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async UniTask InitializeAsync()
        {
            if (Configuration.UseGoogleSheet)
            {
                for (int i = 0; i < Configuration.ListGoogleSheetData.Count; i++)
                {
                    var data = Configuration.ListGoogleSheetData[i];

                    if (Configuration.IsInitializeAtRuntime)
                    {
                        var tsvUrl = ConfigurationProvider.LoadOrDefault<DataConfiguration>().TsvUrl.Replace("{Key}", Configuration.GoogleSheetKey).Replace("{SheetId}", data.Key);

                        Configuration.ListGoogleSheetData[i].Data.SetData(await GetGoogleSpreadSheetDataAsync(tsvUrl));
                    }

                    m_DictionaryGoogleSheetData.Add(data.Data.GetType(), data.Data);
                }


                Debug.Log("GoogleSheetData Initialized Done.");
            }
        }

        public virtual T GetData<T>() where T : class, IGoogleSheetData => GetData(typeof(T)) as T;

        public virtual IGoogleSheetData GetData(Type type)
        {
            if (m_DictionaryGoogleSheetData.TryGetValue(type, out var cachedResult))
                return cachedResult;

            return null;
        }

        public async UniTask SetGoogleSpreadSheetDataToSO(GoogleSheetData data)
        {
            if (!Configuration.UseGoogleSheet)
                return;

            foreach (var item in Configuration.ListGoogleSheetData)
            {
                if (item.Data == data)
                {
                    var tsvUrl = data.TsvUrl.Replace("{Key}", Configuration.GoogleSheetKey).Replace("{SheetId}", item.Key);

                    data.SetData(await GetGoogleSpreadSheetDataAsync(tsvUrl));
                    return;
                }
            }
        }

        private async UniTask<string[,]> GetGoogleSpreadSheetDataAsync(string url)
        {

            UnityWebRequest request = UnityWebRequest.Get(url);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tsvData = request.downloadHandler.text;
                string[,] data = ParsingSpreadSheetData(tsvData);
                return data;
            }
            else
            {
                Debug.LogError("Error getting TSV data: " + request.error);
                return null;
            }
        }

        private string[,] ParsingSpreadSheetData(string data)
        {
            string[] row = data.Replace("\r", "").Split('\n');
            int rowSize = row.Length;
            int columnSize = row[0].Split('\t').Length;
            string[,] Sentence = new string[rowSize, columnSize];

            for (int i = 0; i < rowSize; i++)
            {
                string[] column = row[i].Split('\t');
                for (int j = 0; j < columnSize; j++)
                {
                    Sentence[i, j] = column[j];
                }
            }

            return Sentence;
        }

        public void Reset()
        {

        }

        public void DestroyThis()
        {

        }
    }
}