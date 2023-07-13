using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

namespace olimsko
{
    [CustomEditor(typeof(GoogleSheetData), true), CanEditMultipleObjects]
    public class GoogleSheetDataEditor : Editor
    {
        Dictionary<string, SerializedProperty> m_DicProperty = new Dictionary<string, SerializedProperty>();

        private DataConfiguration m_DataConfiguration => ConfigurationProvider.LoadOrDefault<DataConfiguration>();

        protected void OnEnable()
        {
            m_DicProperty.Clear();

            var property = serializedObject.GetIterator();
            var enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                SerializedProperty temp = serializedObject.FindProperty(property.propertyPath);
                m_DicProperty.Add(temp.propertyPath, temp);
            }
        }

        public override void OnInspectorGUI()
        {
            GoogleSheetData googleSheetData = target as GoogleSheetData;

            base.OnInspectorGUI();

            if (m_DicProperty["m_Key"].stringValue == string.Empty)
            {
                m_DicProperty["m_Key"].stringValue = m_DataConfiguration.GoogleSheetKey;
            }

            if (m_DicProperty["m_SheetId"].stringValue == string.Empty)
            {
                m_DicProperty["m_SheetId"].stringValue = m_DataConfiguration.ListGoogleSheetData.Where(x => x.Data == googleSheetData).FirstOrDefault().Key;
            }

            if (GUILayout.Button("Load"))
            {
                googleSheetData.Load().Forget();
            }

            if (GUILayout.Button("Generate Class"))
            {
                GenerateClass(googleSheetData).Forget();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private async UniTask GenerateClass(GoogleSheetData googleSheetData)
        {
            var url = googleSheetData.TsvUrl.Replace("{Key}", googleSheetData.Key).Replace("{SheetId}", googleSheetData.SheetId);
            var data = await GetGoogleSpreadSheetDataAsync(url);
            CreateScriptFile(googleSheetData, data);
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

        private string CreateScriptFile(GoogleSheetData googleSheetData, string[,] datas)
        {
            string[] properties = new string[datas.GetLength(1)];

            for (int i = 0; i < datas.GetLength(1); i++)
            {
                properties[i] = datas[0, i];
            }

            string className = googleSheetData.GetType().Name.Replace("SO", "");
            string scriptName = $"{className}.cs";
            string assetPath = AssetDatabase.GetAssetPath(googleSheetData);
            string folderPath = Path.GetDirectoryName(assetPath);
            string scriptPath = Path.Combine(folderPath, scriptName);

            string firstPropertyName = properties[0].Split(':')[0].Trim();
            string firstPropertyType = ParsePropertyType(properties[0]);

            using (StreamWriter outputFile = new StreamWriter(scriptPath, false))
            {
                outputFile.WriteLine("using System;");
                outputFile.WriteLine("using UnityEngine;");
                outputFile.WriteLine("using olimsko;");
                outputFile.WriteLine();

                Dictionary<string, List<string>> enumDictionary = new Dictionary<string, List<string>>();

                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].IndexOf("enum") != -1)
                    {
                        int j = i;
                        ParseEnumClass(outputFile, datas, j, enumDictionary);
                    }
                }

                WriteEnumDictionary(outputFile, enumDictionary);

                outputFile.WriteLine("[Serializable]");
                outputFile.WriteLine($"public class {className} : ITableData<{firstPropertyType}>");
                outputFile.WriteLine("{");

                List<string> constructorArguments = new List<string>();

                for (int i = 0; i < properties.Length; i++)
                {
                    string propertyName = properties[i].Split(':')[0].Trim();
                    string propertyType = ParsePropertyType(properties[i]);
                    string defaultValue = GetDefaultValue(properties[i]);

                    outputFile.WriteLine($"    [SerializeField] private {propertyType} m_{propertyName}{defaultValue};");
                    constructorArguments.Add($"{propertyType} {propertyName.ToLower()}");
                }

                WriteConstructors(outputFile, className, constructorArguments, properties);
                WriteProperties(outputFile, properties);
                WriteGetKeyMethod(outputFile, firstPropertyType, firstPropertyName);
                WriteSetDataFromRowMethod(outputFile, properties);

                outputFile.WriteLine("}");

                outputFile.Flush();
                outputFile.Close();
            }

            AssetDatabase.Refresh();

            return scriptPath;
        }

        private void ParseEnumClass(StreamWriter outputFile, string[,] datas, int idx, Dictionary<string, List<string>> enumDictionary)
        {
            string enumName = ParsePropertyType(datas[0, idx]);

            for (int i = 1; i < datas.GetLength(0); i++)
            {
                string enumValue = datas[i, idx].Trim();

                if (!enumDictionary.ContainsKey(enumName))
                {
                    enumDictionary.Add(enumName, new List<string>());
                }

                if (!enumDictionary[enumName].Contains(enumValue) && !string.IsNullOrEmpty(enumValue))
                {
                    enumDictionary[enumName].Add(enumValue);
                }
            }
        }

        private void WriteEnumDictionary(StreamWriter outputFile, Dictionary<string, List<string>> enumDictionary)
        {
            foreach (var enumData in enumDictionary)
            {
                outputFile.WriteLine($"public enum {enumData.Key} {{");

                foreach (var enumValue in enumData.Value)
                {
                    outputFile.WriteLine($"    {enumValue},");
                }
                outputFile.WriteLine("    Default");
                outputFile.WriteLine("}");
                outputFile.WriteLine();
            }
        }

        private string ParsePropertyType(string property)
        {
            string propertyType = property.Split(':')[1].Trim();
            propertyType = RemoveParenthesesAndContent(propertyType);
            if (propertyType.Contains("=")) propertyType = propertyType.Split('=')[0].Trim();

            return propertyType;
        }

        private string RemoveParenthesesAndContent(string input)
        {
            int openParenIndex = input.IndexOf('(');
            int closeParenIndex = input.IndexOf(')');

            if (openParenIndex >= 0 && closeParenIndex >= 0)
            {
                return input.Remove(openParenIndex, closeParenIndex - openParenIndex + 1).Trim();
            }

            return input.Trim();
        }

        private string GetDefaultValue(string property)
        {
            string defaultValue = "";
            string propertyType = property.Split(':')[1].Trim();
            propertyType = RemoveParenthesesAndContent(propertyType);

            if (propertyType.Contains("="))
            {
                defaultValue = $" = {propertyType.Split('=')[1].Trim()}";
            }

            return defaultValue;
        }

        private void WriteConstructors(StreamWriter outputFile, string className, List<string> constructorArguments, string[] properties)
        {
            string constructorArgs = string.Join(", ", constructorArguments);
            outputFile.WriteLine();
            outputFile.WriteLine($"    public {className}() {{ }}");
            outputFile.WriteLine();
            outputFile.WriteLine($"    public {className}({constructorArgs})");
            outputFile.WriteLine("    {");

            for (int i = 0; i < constructorArguments.Count; i++)
            {
                string constructorArgsName = constructorArguments[i].Split(' ')[1];
                string propertyName = properties[i].Split(':')[0].Trim();

                outputFile.WriteLine($"        m_{propertyName} = {constructorArgsName};");
            }

            outputFile.WriteLine("    }");
        }

        private void WriteProperties(StreamWriter outputFile, string[] properties)
        {
            outputFile.WriteLine();
            foreach (string property in properties)
            {
                string propertyName = property.Split(':')[0].Trim();
                string propertyType = ParsePropertyType(property);

                outputFile.WriteLine($"    public {propertyType} {propertyName} {{ get => m_{propertyName}; set => m_{propertyName} = value; }}");
            }
        }

        private void WriteGetKeyMethod(StreamWriter outputFile, string firstPropertyType, string firstPropertyName)
        {
            outputFile.WriteLine();
            outputFile.WriteLine($"    public {firstPropertyType} GetKey()");
            outputFile.WriteLine("    {");
            outputFile.WriteLine($"        return {firstPropertyName};");
            outputFile.WriteLine("    }");
        }

        private void WriteSetDataFromRowMethod(StreamWriter outputFile, string[] properties)
        {
            outputFile.WriteLine();
            outputFile.WriteLine("    public void SetDataFromRow(string[,] data, int row)");
            outputFile.WriteLine("    {");

            int columnIndex = 0;
            foreach (string property in properties)
            {
                string propertyName = property.Split(':')[0].Trim();
                string propertyType = ParsePropertyType(property);

                if (propertyType != "bool" && propertyType != "int" && propertyType != "float" && propertyType != "string" && propertyType != "byte" && propertyType != "sbyte" && propertyType != "char" && propertyType != "decimal" && propertyType != "double" && propertyType != "long" && propertyType != "short" && propertyType != "uint" && propertyType != "ulong" && propertyType != "ushort")
                {
                    outputFile.WriteLine($"        m_{propertyName} = string.IsNullOrEmpty(data[row, {columnIndex}]) ? {propertyType}.Default : ({propertyType})Enum.Parse(typeof({propertyType}), data[row, {columnIndex}]);");
                }
                else if (propertyType == "string")
                {
                    outputFile.WriteLine($"        m_{propertyName} = data[row, {columnIndex}];");
                }
                else
                {
                    outputFile.WriteLine($"        m_{propertyName} = string.IsNullOrEmpty(data[row, {columnIndex}]) ? default : {propertyType}.Parse(data[row, {columnIndex}]);");
                }

                columnIndex++;
            }

            outputFile.WriteLine("    }");
        }

    }
}
