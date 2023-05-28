// Copyright 2017-2021 Elringus (Artyom Sovetnikov). All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace olimsko
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public const string DefaultResourcesPath = "olimsko/Config";

        private readonly Dictionary<Type, Configuration> m_DicConfig = new Dictionary<Type, Configuration>();

        public ConfigurationProvider(string resourcesPath = DefaultResourcesPath)
        {
            var configTypes = OSManager.Types
                .Where(type => typeof(Configuration).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);

            foreach (var configType in configTypes)
            {
                var configAsset = LoadOrDefault(configType, resourcesPath);
                var configObject = UnityEngine.Object.Instantiate(configAsset);
                m_DicConfig.Add(configType, configObject);
            }
        }

        public virtual Configuration GetConfiguration(Type type)
        {
            if (m_DicConfig.TryGetValue(type, out var result))
                return result;

            throw new Exception($"Failed to provide `{type.Name}` configuration object: Requested configuration type not found in project resources.");
        }

        public static TConfig LoadOrDefault<TConfig>(string resourcesPath = DefaultResourcesPath)
            where TConfig : Configuration
        {
            return LoadOrDefault(typeof(TConfig), resourcesPath) as TConfig;
        }

        public static Configuration LoadOrDefault(Type type, string resourcesPath = DefaultResourcesPath)
        {
            var resourcePath = $"{resourcesPath}/{type.Name}";
            var configAsset = Resources.Load(resourcePath, type) as Configuration;

            if (!configAsset)
                configAsset = ScriptableObject.CreateInstance(type) as Configuration;

            return configAsset;
        }
    }
}
