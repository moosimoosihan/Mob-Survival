using System.IO;
using UnityEngine;

namespace olimsko
{
    public class GlobalStateHandler : SaveStateManager<GlobalState>
    {
        protected override string SaveDataPath => $"{GameDataPath}/{savesFolderPath}";
        protected override string Extension => "json";

        private readonly string defaultSlotId;
        private readonly string savesFolderPath;
        private readonly StateConfiguration config;

        public GlobalStateHandler(StateConfiguration config, string savesFolderPath)
        {
            this.savesFolderPath = savesFolderPath;
            this.config = config;
            defaultSlotId = config.GlobalSlotId;
        }

        public override bool AnySaveExists()
        {
            if (!Directory.Exists(SaveDataPath)) return false;
            return Directory.GetFiles(SaveDataPath, $"{defaultSlotId}.{Extension}", SearchOption.TopDirectoryOnly).Length > 0;
        }
    }
}