using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace olimsko
{
    [EditInProjectSettings]
    public class UIConfiguration : Configuration
    {
        public RenderMode UIRenderMode = RenderMode.ScreenSpaceOverlay;
        public GameObject CustomUICamera;

        public Vector2 DefaultScreenSize = new Vector2(3840, 2160);
        [Range(0f, 1f)] public float Match = 1;

        public bool AutoSpawnInputSystem = true;
        public GameObject CustomInputSystem;

        public bool UseHideHotKey = true;
        public KeyCode HideHotKeyCode = KeyCode.Escape;
        public string UINameWhenNoHotKeyList = "";

        public bool OverrideObjectLayer = true;
        public int ObjectLayer = 5;

        public Sprite TransparentImage;
        public bool MakeScriptWhenAddPrefab = true;
        public List<PrefabResources> ListPrefabs = new List<PrefabResources>();
    }
}