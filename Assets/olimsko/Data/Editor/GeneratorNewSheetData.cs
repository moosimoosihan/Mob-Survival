using UnityEngine;
using UnityEditor;
using olimsko;
using System.IO;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class GenerateNewSheetData : EditorWindow
{
    private string m_GoogleSheetKey = "";
    private string m_SheetID = "";
    private string m_ClassName = "";
    private int m_SelectedOption = 0;
    private string m_KeyType = "int";
    private static string m_SelectionFolderPath;

    private GUIStyle m_ErrorLabelStyle = new GUIStyle();

    [MenuItem("Assets/Create/olimsko/GenerateNewGoogleSheetData", false, 0)]
    public static void ShowWindow()
    {
        m_SelectionFolderPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (Path.GetExtension(m_SelectionFolderPath) != "")
        {
            m_SelectionFolderPath = m_SelectionFolderPath.Replace(Path.GetFileName(m_SelectionFolderPath), "");
        }
        else
        {
            m_SelectionFolderPath += "/";
        }

        GetWindow<GenerateNewSheetData>("Generate New Sheet Data");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Google Sheet Key", EditorStyles.boldLabel);
        GUILayout.Label("    [ProjectSettings -> olimsko -> Data -> GoogleSheetKey]");
        GUILayout.Space(10);
        m_GoogleSheetKey = EditorGUILayout.TextField("Google Sheet Key : ", string.IsNullOrEmpty(ConfigurationProvider.LoadOrDefault<DataConfiguration>().GoogleSheetKey) ? m_GoogleSheetKey : ConfigurationProvider.LoadOrDefault<DataConfiguration>().GoogleSheetKey);

        GUILayout.Space(10);
        GUILayout.Label("Google Sheet ID", EditorStyles.boldLabel);
        GUILayout.Label("    [Only number]");
        GUILayout.Space(10);
        m_SheetID = EditorGUILayout.TextField("Google Sheet ID : ", m_SheetID);

        GUILayout.Space(10);
        GUILayout.Label("Class Name", EditorStyles.boldLabel);
        GUILayout.Label("    [DO NOT INCLUDE EXTENSION (.cs))]");
        GUILayout.Space(10);
        m_ClassName = EditorGUILayout.TextField("Class Name : ", m_ClassName);

        GUILayout.Space(10);
        GUILayout.Label("Collections Type:", EditorStyles.boldLabel);
        GUILayout.Space(10);

        using (new EditorGUILayout.ToggleGroupScope("Type", true))
        {
            if (EditorGUILayout.Toggle("List", m_SelectedOption == 0))
            {
                m_SelectedOption = 0;
            }
            if (EditorGUILayout.Toggle("Dictionary", m_SelectedOption == 1))
            {
                m_SelectedOption = 1;
            }
            if (EditorGUILayout.Toggle("SerializableDictionary", m_SelectedOption == 2))
            {
                m_SelectedOption = 2;
            }
        }

        if (m_SelectedOption == 1 || m_SelectedOption == 2)
        {
            GUILayout.Space(10);
            GUILayout.Label("    [Please Write Key Type (ex. int, string)]");
            m_KeyType = EditorGUILayout.TextField("Key Type : ", m_KeyType);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Generate Object"))
        {
            m_ErrorLabelStyle.normal.textColor = Color.red;
            if (string.IsNullOrEmpty(m_GoogleSheetKey))
            {
                GUILayout.Label("Google Sheet Key is empty", m_ErrorLabelStyle);
                return;
            }

            if (string.IsNullOrEmpty(m_SheetID))
            {
                GUILayout.Label("Google Sheet ID is empty", m_ErrorLabelStyle);
                return;
            }

            if (string.IsNullOrEmpty(m_ClassName))
            {
                GUILayout.Label("Class Name is empty", m_ErrorLabelStyle);
                return;
            }

            if (m_ClassName.IndexOf("SO") == -1)
            {
                m_ClassName += "SO";
            }
            GenerateDataClass().Forget();

        }
    }

    private void GenerateDataFiles()
    {
        string className = m_ClassName.Replace("SO", "");
        string scriptName = $"{m_ClassName}.cs";
        string assetPath = m_SelectionFolderPath;
        string folderPath = Path.GetDirectoryName(assetPath);
        string scriptPath = Path.Combine(folderPath, scriptName);
        string scriptableObjectPath = Path.Combine(folderPath, $"{m_ClassName}.asset");

        using (StreamWriter outputFile = new StreamWriter(scriptPath, false))
        {
            outputFile.WriteLine("using System;");
            outputFile.WriteLine("using System.Collections.Generic;");
            outputFile.WriteLine("using UnityEngine;");
            outputFile.WriteLine("using olimsko;");
            outputFile.WriteLine();
            outputFile.WriteLine($"[CreateAssetMenu(fileName = \"{m_ClassName}\", menuName = \"olimsko/Data/{m_ClassName}\", order = 1)]");
            outputFile.WriteLine($"public class {m_ClassName} : GoogleSheetData");
            outputFile.WriteLine("{");

            switch (m_SelectedOption)
            {
                case 0: outputFile.WriteLine($"    [SerializeField] public List<{className}> {className} = new List<{className}>();"); break;
                case 1: outputFile.WriteLine($"    [SerializeField] public Dictionary<{m_KeyType},{className}> {className} = new Dictionary<{m_KeyType},{className}>();"); break;
                case 2: outputFile.WriteLine($"    [SerializeField] public SerializableDictionary<{m_KeyType},{className}> {className} = new SerializableDictionary<{m_KeyType},{className}>();"); break;
            }

            outputFile.WriteLine("    public override void SetData(string[,] data)");
            outputFile.WriteLine("    {");
            outputFile.WriteLine($"        SetDataToList(data, {className});");
            outputFile.WriteLine("    }");

            outputFile.WriteLine("}");

            outputFile.Flush();
            outputFile.Close();
        }

        EditorPrefs.SetBool("olimsko_GoogleSheetSO_AddScript", true);
        EditorPrefs.SetString("olimsko_GoogleSheetSO_ClassName", m_ClassName);
        EditorPrefs.SetString("olimsko_GoogleSheetSO_TargetPath", scriptableObjectPath);
        EditorPrefs.SetString("olimsko_GoogleSheetSO_SheetID", m_SheetID);

        AssetDatabase.Refresh();
    }

    #region Generate Data Class
    private async UniTask GenerateDataClass()
    {
        string TsvUrl = ConfigurationProvider.LoadOrDefault<DataConfiguration>().TsvUrl;
        var url = TsvUrl.Replace("{Key}", m_GoogleSheetKey).Replace("{SheetId}", m_SheetID);
        var data = await GetGoogleSpreadSheetDataAsync(url);
        CreateScriptFile(data);

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

    private void CreateScriptFile(string[,] datas)
    {
        string[] properties = new string[datas.GetLength(1)];

        for (int i = 0; i < datas.GetLength(1); i++)
        {
            properties[i] = datas[0, i];
        }

        string className = m_ClassName.Replace("SO", "");
        string scriptName = $"{className}.cs";
        string assetPath = m_SelectionFolderPath;
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

        GenerateDataFiles();
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
    #endregion
}