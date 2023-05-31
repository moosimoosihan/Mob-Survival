using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace olimsko
{
    [EditInProjectSettings]
    public class ResourceConfiguration : Configuration
    {
        public bool UseGoogleSpreadSheet = false;
        public List<string> ListSpreadSheetPath = new List<string>();

        public bool UseAddressableAsset = false;
        public string ListAddressablePath = "olimsko";

        public bool UseLocalResource = true;
        public string UseLocalResourcePath = "olimsko/Resources/";
    }
}