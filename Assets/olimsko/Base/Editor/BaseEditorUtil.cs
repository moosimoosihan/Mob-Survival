using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

public static class BaseEditorUtil
{
    public static Object LoadAssetByGuid(string guid, Type type)
    {
        var path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path)) return null;
        return AssetDatabase.LoadAssetAtPath(path, type);
    }
}