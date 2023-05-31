using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.ComponentModel;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

namespace olimsko
{
    [Serializable]
    public abstract class GoogleSheetData : ScriptableObject, IGoogleSheetData
    {
        [SerializeField] private string m_Key;
        [SerializeField] private string m_SheetId;

        private DataConfiguration m_DataConfiguration => ConfigurationProvider.LoadOrDefault<DataConfiguration>();

        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(m_Key))
                {
                    m_Key = m_DataConfiguration.GoogleSheetKey;
                }
                return m_Key;
            }
            set
            {
                m_Key = value;
            }
        }

        public string SheetId
        {
            get
            {
                if (string.IsNullOrEmpty(m_SheetId))
                {
                    m_SheetId = m_DataConfiguration.ListGoogleSheetData.Where(x => x.Data == this).FirstOrDefault().Key;
                }
                return m_SheetId;
            }
            set
            {
                m_SheetId = value;
            }
        }
        public string TsvUrl { get; } = "https://docs.google.com/spreadsheets/d/{Key}/export?format=tsv&gid={SheetId}";

        public abstract void SetData(string[,] data);

        public async UniTaskVoid Load()
        {
            var url = TsvUrl.Replace("{Key}", Key).Replace("{SheetId}", SheetId);
            var data = await GetGoogleSpreadSheetDataAsync(url);
            SetData(data);
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

        protected void SetDataToSerializableDictionary<T1, T2>(string[,] data, SerializableDictionary<T1, T2> dictionary) where T2 : ITableData<T1>, new()
        {
            dictionary.Clear();

            int numRows = data.GetLength(0);
            int numColumns = data.GetLength(1);

            for (int i = 1; i < numRows; i++)
            {
                T2 rowData = new T2();
                rowData.SetDataFromRow(data, i);
                dictionary.Add(rowData.GetKey(), rowData);
            }
        }

        protected void SetDataToDictionary<T1, T2>(string[,] data, Dictionary<T1, T2> dictionary) where T2 : ITableData<T1>, new()
        {
            dictionary.Clear();

            int numRows = data.GetLength(0);
            int numColumns = data.GetLength(1);

            for (int i = 1; i < numRows; i++)
            {
                T2 rowData = new T2();
                rowData.SetDataFromRow(data, i);
                dictionary.Add(rowData.GetKey(), rowData);
            }
        }

        protected void SetDataToList<T2>(string[,] data, List<T2> list) where T2 : ITableData<int>, new()
        {
            list.Clear();

            int numRows = data.GetLength(0);
            int numColumns = data.GetLength(1);

            for (int i = 1; i < numRows; i++)
            {
                T2 rowData = new T2();
                rowData.SetDataFromRow(data, i);
                list.Add(rowData);
            }
        }


    }
}