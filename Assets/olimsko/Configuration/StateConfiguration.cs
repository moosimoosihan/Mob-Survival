using UnityEngine;

namespace olimsko
{
    [EditInProjectSettings]
    public class StateConfiguration : Configuration
    {
        public string SaveFolderName = "Saves";
        public string SettingSlotId = "Settings";
        public string GlobalSlotId = "GlobalSave";
        public string SaveSlotFormat = "Save{0:000}";
        public string QuickSaveSlotFormat = "QuickSave{0:000}";

        public int MaxSaveSlotLimit = 10;
        public int MaxQuickSaveSlotLimit = 5;

        public bool UseNewtonsoftJson = true;

        public bool EncryptSave = true;
        public string EncryptSaveKey = "olimsko";

        public bool UseDebugLog = true;

        public string IndexToSaveSlotId(int index) => string.Format(SaveSlotFormat, index);
        public string IndexToQuickSaveSlotId(int index) => string.Format(QuickSaveSlotFormat, index);
    }
}
