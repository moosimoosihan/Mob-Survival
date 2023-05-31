using System.Collections.Generic;
using UnityEngine;


namespace olimsko
{
    [EditInProjectSettings]
    public class DataConfiguration : Configuration
    {
        public bool UseGoogleSheet;
        public bool IsInitializeAtRuntime;
        public string GoogleSheetKey;
        public List<GoogleSheetDataList> ListGoogleSheetData;
    }

    [System.Serializable]
    public class GoogleSheetDataList
    {
        [SerializeField] private string m_Key;
        [SerializeField] private GoogleSheetData m_Data;

        public GoogleSheetDataList(string key, GoogleSheetData data)
        {
            m_Key = key;
            m_Data = data;
            m_Data.SheetId = key;
        }

        public string Key
        {
            get => m_Key;
            set
            {
                m_Key = value;
                if (m_Data != null)
                {
                    m_Data.SheetId = value;
                }
            }
        }
        public GoogleSheetData Data { get => m_Data; set => m_Data = value; }
    }
}