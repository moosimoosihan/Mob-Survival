// Copyright 2017-2021 Elringus (Artyom Sovetnikov). All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace olimsko
{
    public class ConfigurationSettingsProvider : SettingsProvider
    {
        protected Type ConfigurationType { get; }
        protected Configuration Configuration { get; private set; }
        protected SerializedObject SerializedObject { get; private set; }
        protected virtual string EditorTitle { get; }


        private const string settingsPathPrefix = "Project/olimsko/";
        private static readonly GUIContent helpIcon;
        protected static readonly Type settingsScopeType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SettingsWindow+GUIScope");
        private static readonly Dictionary<Type, Type> settingsTypeMap = BuildSettingsTypeMap();

        private Dictionary<string, Action<SerializedProperty>> overrideDrawers;
        private UnityEngine.Object[] assetTargets;

        static ConfigurationSettingsProvider()
        {
            // helpIcon = new GUIContent(GUIContents.HelpIcon);
            // helpIcon.tooltip = "Open guide in web browser.";
        }

        protected ConfigurationSettingsProvider(Type configType)
            : base(TypeToSettingsPath(configType), SettingsScope.Project)
        {
            Debug.Assert(typeof(Configuration).IsAssignableFrom(configType));
            ConfigurationType = configType;
            EditorTitle = ConfigurationType.Name.Replace("Configuration", string.Empty).InsertCamel();
        }

        public static TConfig LoadOrDefaultAndSave<TConfig>()
            where TConfig : Configuration, new()
        {
            var configuration = ConfigurationProvider.LoadOrDefault<TConfig>();
            if (!AssetDatabase.Contains(configuration))
                SaveConfigurationObject(configuration);

            return configuration;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            Configuration = ConfigurationProvider.LoadOrDefault(ConfigurationType);
            SerializedObject = new SerializedObject(Configuration);
            keywords = GetSearchKeywordsFromSerializedObject(SerializedObject);
            overrideDrawers = OverrideConfigurationDrawers();

            // Save the asset in case it was just generated.
            if (!AssetDatabase.Contains(Configuration))
                SaveConfigurationObject(Configuration);

            assetTargets = new UnityEngine.Object[] { Configuration };
        }

        public override void OnGUI(string searchContext)
        {
            if (SerializedObject is null || !BaseUtil.IsValid(SerializedObject.targetObject))
            {
                EditorGUILayout.HelpBox($"{EditorTitle} configuration asset has been deleted or moved. Try re-opening the settings window or restarting the Unity editor.", MessageType.Error);
                return;
            }

            using (Activator.CreateInstance(settingsScopeType) as IDisposable)
            {
                SerializedObject.Update();

                DrawConfigurationEditor();
                SerializedObject.ApplyModifiedProperties();
            }
        }

        protected virtual Dictionary<string, Action<SerializedProperty>> OverrideConfigurationDrawers() => new Dictionary<string, Action<SerializedProperty>>();

        protected virtual void DrawConfigurationEditor()
        {
            DrawDefaultEditor();
        }

        protected void DrawDefaultEditor()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fixedHeight = 25f,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 20,
            };
            var style2 = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 13,
            };

            EditorGUILayout.BeginVertical("GroupBox");
            EditorGUILayout.BeginVertical("textArea", GUILayout.Height(30));
            EditorGUILayout.LabelField($"{EditorTitle} Configuration", style, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical("", "GroupBox");

            EditorGUI.indentLevel++;
            GUILayout.Space(10);

            var property = SerializedObject.GetIterator();
            var enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (property.propertyPath == "m_Script") continue;
                if (overrideDrawers != null && overrideDrawers.ContainsKey(property.propertyPath))
                {
                    overrideDrawers[property.propertyPath]?.Invoke(property);
                    continue;
                }

                EditorGUILayout.PropertyField(property, true);
            }

            GUILayout.Space(10);
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }

        protected static string TypeToSettingsPath(Type type)
        {
            return settingsPathPrefix + type.Name.Replace("Configuration", string.Empty).InsertCamel();
        }

        protected static void InitializeImplementationOptions<TImplementation>(ref string[] values, ref string[] labels)
        {
            values = OSManager.Types
                .Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(TImplementation)))
                .Select(t => t.AssemblyQualifiedName).ToArray();
            labels = values.Select(s => s.GetBefore(",")).ToArray();
        }

        protected static void DrawImplementations(string[] values, string[] labels, SerializedProperty property)
        {
            var label = EditorGUI.BeginProperty(Rect.zero, null, property);
            var curIndex = ArrayUtility.IndexOf(values, property.stringValue ?? string.Empty);
            var newIndex = EditorGUILayout.Popup(label, curIndex, labels);
            property.stringValue = values.IsIndexValid(newIndex) ? values[newIndex] : string.Empty;
            EditorGUI.EndProperty();
        }

        protected virtual void OnChanged(Action onChanged, Action drawer)
        {
            EditorGUI.BeginChangeCheck();
            drawer?.Invoke();
            if (EditorGUI.EndChangeCheck())
                EditorApplication.delayCall += () => onChanged?.Invoke();
        }

        protected virtual void OnChanged(Action onChanged, SerializedProperty property)
        {
            OnChanged(onChanged, () => EditorGUILayout.PropertyField(property));
        }

        protected virtual void DrawWhen(bool condition, Action drawer)
        {
            if (condition) drawer();
        }

        protected virtual void DrawWhen(bool condition, SerializedProperty property)
        {
            DrawWhen(condition, () => EditorGUILayout.PropertyField(property));
        }

        private static void SaveConfigurationObject(Configuration configuration)
        {
            var dirPath = BaseUtil.CombinePath(BasePath.GeneratedDataPath, $"Resources/{ConfigurationProvider.DefaultResourcesPath}");
            var fullPath = BaseUtil.CombinePath(dirPath, $"{configuration.GetType().Name}.asset");
            var assetPath = BaseUtil.AbsoluteToAssetPath(fullPath);
            if (File.Exists(fullPath)) throw new UnityException("Unity failed to load an existing asset. Try restarting the editor.");
            Directory.CreateDirectory(dirPath);
            AssetDatabase.CreateAsset(configuration, assetPath);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static Dictionary<Type, Type> BuildSettingsTypeMap()
        {
            bool IsEditorFor(Type editorType, Type configType)
            {
                var type = editorType.BaseType;
                while (type != null)
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ConfigurationSettingsProvider<>) && type.GetGenericArguments()[0] == configType)
                        return true;
                    type = type.BaseType;
                }
                return false;
            }

            var configTypes = new List<Type>();
            var editorTypes = new List<Type>();

            foreach (var type in OSManager.Types)
            {
                if (type.IsAbstract) continue;
                if (type.IsSubclassOf(typeof(Configuration)) && type.IsDefined(typeof(Configuration.EditInProjectSettingsAttribute)))
                    configTypes.Add(type);
                else if (type.IsSubclassOf(typeof(ConfigurationSettingsProvider)))
                    editorTypes.Add(type);
            }

            var typeMap = new Dictionary<Type, Type>();
            foreach (var configType in configTypes)
            {
                var compatibleEditors = editorTypes.Where(t => IsEditorFor(t, configType)).ToList();
                if (compatibleEditors.Count == 0) // No specialized editors are found; use the default one.
                    typeMap.Add(configType, typeof(ConfigurationSettingsProvider));
                else if (compatibleEditors.Count == 1) // Single specialized editor is found; use it.
                    typeMap.Add(configType, compatibleEditors.First());
                else // Multiple specialized editors for the config are found.
                {
                    if (compatibleEditors.Count > 2)
                        Debug.LogWarning($"Multiple editors for `{configType}` configuration are found. That is not supported. First overridden one will be used.");
                    var overriddenEditor = compatibleEditors.Find(t => t.IsDefined(typeof(OverrideSettingsAttribute)));
                    if (overriddenEditor is null)
                    {
                        Debug.LogWarning($"Multiple editors for `{configType}` configuration are found, while none has `{nameof(OverrideSettingsAttribute)}` applied. First found one will be used.");
                        typeMap.Add(configType, compatibleEditors.First());
                    }
                    else typeMap.Add(configType, overriddenEditor);
                }
            }

            return typeMap;
        }

        [SettingsProviderGroup]
        private static SettingsProvider[] CreateProviders()
        {
            return settingsTypeMap
                .Select(kv => kv.Value == typeof(ConfigurationSettingsProvider) ? new ConfigurationSettingsProvider(kv.Key) : Activator.CreateInstance(kv.Value) as SettingsProvider).ToArray();
        }

        [MenuItem("olimsko/Configuration", priority = 1)]
        private static void OpenWindow()
        {
            var engineSettingsPath = TypeToSettingsPath(typeof(UIConfiguration));
            SettingsService.OpenProjectSettings(engineSettingsPath);
        }
    }

    public abstract class ConfigurationSettingsProvider<TConfig> : ConfigurationSettingsProvider where TConfig : Configuration
    {
        protected new TConfig Configuration => base.Configuration as TConfig;
        protected static string SettingsPath => TypeToSettingsPath(typeof(TConfig));

        protected ConfigurationSettingsProvider()
            : base(typeof(TConfig)) { }
    }
}
