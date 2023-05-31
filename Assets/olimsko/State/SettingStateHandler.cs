using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace olimsko
{
    public class SettingStateHandler : SaveStateManager<SettingState>
    {
        protected override string SaveDataPath => $"{GameDataPath}/{savesFolderPath}";
        protected override string Extension => "json";

        private readonly string defaultSlotId;
        private readonly string savesFolderPath;

        public SettingStateHandler(StateConfiguration config, string savesFolderPath)
        {
            this.savesFolderPath = savesFolderPath;
            defaultSlotId = config.SettingSlotId;
        }

        public override bool AnySaveExists()
        {
            if (!Directory.Exists(SaveDataPath)) return false;
            return Directory.GetFiles(SaveDataPath, $"{defaultSlotId}.{Extension}", SearchOption.TopDirectoryOnly).Length > 0;
        }
    }
}