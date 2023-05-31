using System.IO;
using UnityEngine;

namespace olimsko
{
    public class GameStateHandler : SaveStateManager<GameState>
    {
        protected override string SaveDataPath => $"{GameDataPath}/{savesFolderPath}";
        protected sealed override string Extension => "json";

        private readonly string savePattern, quickSavePattern;
        private readonly string savesFolderPath;
        private readonly StateConfiguration config;

        public GameStateHandler(StateConfiguration config, string savesFolderPath)
        {
            this.savesFolderPath = savesFolderPath;
            this.config = config;
            savePattern = string.Format(config.SaveSlotFormat, "*") + $".{Extension}";
            quickSavePattern = string.Format(config.QuickSaveSlotFormat, "*") + $".{Extension}";
        }

        public override bool AnySaveExists()
        {
            if (!Directory.Exists(SaveDataPath)) return false;
            var saveExists = Directory.GetFiles(SaveDataPath, savePattern, SearchOption.TopDirectoryOnly).Length > 0;
            var qSaveExists = Directory.GetFiles(SaveDataPath, quickSavePattern, SearchOption.TopDirectoryOnly).Length > 0;
            return saveExists || qSaveExists;
        }
    }
}