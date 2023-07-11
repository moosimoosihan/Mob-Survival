using UnityEngine;
using UnityEditor;
using olimsko;
using System.IO;

public class GenerateNewSheetData : EditorWindow
{
    private string m_GoogleSheetKey = "";
    private string m_SheetID = "";
    private string m_ClassName = "";

    [MenuItem("Olimsko/GenerateNewSheetData")]
    public static void ShowWindow()
    {
        GetWindow<GenerateNewSheetData>("Generate New Sheet Data");
    }

    private void OnGUI()
    {
        GUILayout.Label("Google Sheet Key", EditorStyles.boldLabel);
        GUILayout.Label("ProjectSettings -> olimsko -> Data -> GoogleSheetKey");

        m_GoogleSheetKey = EditorGUILayout.TextField("Google Sheet Key", string.IsNullOrEmpty(ConfigurationProvider.LoadOrDefault<DataConfiguration>().GoogleSheetKey) ? m_GoogleSheetKey : ConfigurationProvider.LoadOrDefault<DataConfiguration>().GoogleSheetKey);

        GUILayout.Label("Google Sheet ID", EditorStyles.boldLabel);
        GUILayout.Label("Only number");
        m_SheetID = EditorGUILayout.TextField("Google Sheet ID", m_SheetID);

        GUILayout.Label("Class Name", EditorStyles.boldLabel);
        GUILayout.Label("~TableSO");
        m_ClassName = EditorGUILayout.TextField("Class Name", m_ClassName);

        if (GUILayout.Button("Generate Object"))
        {
            // CreateScriptFile();
        }
    }

    // private string CreateScriptFile(GoogleSheetData googleSheetData, string[] properties)
    // {
    //     string className = googleSheetData.GetType().Name.Replace("SO", "");
    //     string scriptName = $"{className}.cs";
    //     string assetPath = AssetDatabase.GetAssetPath(googleSheetData);
    //     string folderPath = Path.GetDirectoryName(assetPath);
    //     string scriptPath = Path.Combine(folderPath, scriptName);

    //     string firstPropertyName = properties[0].Split(':')[0].Trim();
    //     string firstPropertyType = ParsePropertyType(properties[0]);

    //     using (StreamWriter outputFile = new StreamWriter(scriptPath, false))
    //     {
    //         outputFile.WriteLine("using System;");
    //         outputFile.WriteLine("using UnityEngine;");
    //         outputFile.WriteLine("using olimsko;");
    //         outputFile.WriteLine();
    //         outputFile.WriteLine("[Serializable]");
    //         outputFile.WriteLine($"public class {className} : ITableData<{firstPropertyType}>");
    //         outputFile.WriteLine("{");

    //         List<string> constructorArguments = new List<string>();

    //         for (int i = 0; i < properties.Length; i++)
    //         {
    //             string propertyName = properties[i].Split(':')[0].Trim();
    //             string propertyType = ParsePropertyType(properties[i]);
    //             string defaultValue = GetDefaultValue(properties[i]);

    //             outputFile.WriteLine($"    [SerializeField] private {propertyType} m_{propertyName}{defaultValue};");
    //             constructorArguments.Add($"{propertyType} {propertyName.ToLower()}");
    //         }

    //         WriteConstructors(outputFile, className, constructorArguments, properties);
    //         WriteProperties(outputFile, properties);
    //         WriteGetKeyMethod(outputFile, firstPropertyType, firstPropertyName);
    //         WriteSetDataFromRowMethod(outputFile, properties);

    //         outputFile.WriteLine("}");

    //         outputFile.Flush();
    //         outputFile.Close();
    //     }

    //     AssetDatabase.Refresh();

    //     return scriptPath;
    // }

    // private string ParsePropertyType(string property)
    // {
    //     string propertyType = property.Split(':')[1].Trim();
    //     propertyType = RemoveParenthesesAndContent(propertyType);
    //     if (propertyType.Contains("=")) propertyType = propertyType.Split('=')[0].Trim();

    //     return propertyType;
    // }

    // private string RemoveParenthesesAndContent(string input)
    // {
    //     int openParenIndex = input.IndexOf('(');
    //     int closeParenIndex = input.IndexOf(')');

    //     if (openParenIndex >= 0 && closeParenIndex >= 0)
    //     {
    //         return input.Remove(openParenIndex, closeParenIndex - openParenIndex + 1).Trim();
    //     }

    //     return input.Trim();
    // }

    // private string GetDefaultValue(string property)
    // {
    //     string defaultValue = "";
    //     string propertyType = property.Split(':')[1].Trim();
    //     propertyType = RemoveParenthesesAndContent(propertyType);

    //     if (propertyType.Contains("="))
    //     {
    //         defaultValue = $" = {propertyType.Split('=')[1].Trim()}";
    //     }

    //     return defaultValue;
    // }

    // private void WriteConstructors(StreamWriter outputFile, string className, List<string> constructorArguments, string[] properties)
    // {
    //     string constructorArgs = string.Join(", ", constructorArguments);
    //     outputFile.WriteLine();
    //     outputFile.WriteLine($"    public {className}() {{ }}");
    //     outputFile.WriteLine();
    //     outputFile.WriteLine($"    public {className}({constructorArgs})");
    //     outputFile.WriteLine("    {");

    //     for (int i = 0; i < constructorArguments.Count; i++)
    //     {
    //         string constructorArgsName = constructorArguments[i].Split(' ')[1];
    //         string propertyName = properties[i].Split(':')[0].Trim();

    //         outputFile.WriteLine($"        m_{propertyName} = {constructorArgsName};");
    //     }

    //     outputFile.WriteLine("    }");
    // }

    // private void WriteProperties(StreamWriter outputFile, string[] properties)
    // {
    //     outputFile.WriteLine();
    //     foreach (string property in properties)
    //     {
    //         string propertyName = property.Split(':')[0].Trim();
    //         string propertyType = ParsePropertyType(property);

    //         outputFile.WriteLine($"    public {propertyType} {propertyName} {{ get => m_{propertyName}; set => m_{propertyName} = value; }}");
    //     }
    // }

    // private void WriteGetKeyMethod(StreamWriter outputFile, string firstPropertyType, string firstPropertyName)
    // {
    //     outputFile.WriteLine();
    //     outputFile.WriteLine($"    public {firstPropertyType} GetKey()");
    //     outputFile.WriteLine("    {");
    //     outputFile.WriteLine($"        return {firstPropertyName};");
    //     outputFile.WriteLine("    }");
    // }

    // private void WriteSetDataFromRowMethod(StreamWriter outputFile, string[] properties)
    // {
    //     outputFile.WriteLine();
    //     outputFile.WriteLine("    public void SetDataFromRow(string[,] data, int row)");
    //     outputFile.WriteLine("    {");

    //     int columnIndex = 0;
    //     foreach (string property in properties)
    //     {
    //         string propertyName = property.Split(':')[0].Trim();
    //         string propertyType = ParsePropertyType(property);

    //         if (propertyType != "bool" && propertyType != "int" && propertyType != "float" && propertyType != "string" && propertyType != "byte" && propertyType != "sbyte" && propertyType != "char" && propertyType != "decimal" && propertyType != "double" && propertyType != "long" && propertyType != "short" && propertyType != "uint" && propertyType != "ulong" && propertyType != "ushort")
    //         {
    //             outputFile.WriteLine($"        m_{propertyName} = ({propertyType})Enum.Parse(typeof({propertyType}), data[row, {columnIndex}]);");
    //         }
    //         else if (propertyType == "string")
    //         {
    //             outputFile.WriteLine($"        m_{propertyName} = data[row, {columnIndex}];");
    //         }
    //         else
    //         {
    //             outputFile.WriteLine($"        m_{propertyName} = {propertyType}.Parse(data[row, {columnIndex}]);");
    //         }

    //         columnIndex++;
    //     }

    //     outputFile.WriteLine("    }");
    // }
}