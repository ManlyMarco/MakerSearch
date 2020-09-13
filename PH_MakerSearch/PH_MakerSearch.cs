﻿using System.IO;

using HarmonyLib;

using BepInEx;
using BepInEx.Configuration;

using UnityEngine.UI;

namespace PH_MakerSearch
{
    [BepInProcess("PlayHome64bit")]
    [BepInProcess("PlayHome32bit")] // *tumbleweed*
    [BepInPlugin(nameof(PH_MakerSearch), nameof(PH_MakerSearch), VERSION)]
    public class PH_MakerSearch : BaseUnityPlugin
    {
        public const string VERSION = "1.3.1";

        public static PH_MakerSearch instance;
        
        public static string searchString;
        public static string TranslationCachePath;
        
        public static InputField input;
        public static ThumbnailSelectUI selectUI;

        public static ConfigEntry<bool> caseSensitive { get; private set; }
        public static ConfigEntry<bool> useTranslatedCache { get; private set; }

        private void Awake()
        {
            instance = this;
            
            caseSensitive = Config.Bind(new ConfigDefinition("General", "Case sensitive"), false);
            useTranslatedCache = Config.Bind(new ConfigDefinition("General", "Search translated cache"), true, new ConfigDescription("Search in translated cache, if nonexistant then translate. Only works when search includes name"));

            var harmony = new Harmony(nameof(PH_MakerSearch));
            harmony.PatchAll(typeof(Hooks));
            
            TranslationCachePath = Path.Combine(Paths.CachePath, "PH_MakerSearch.cache");
        }
        
        public static void Search()
        {
            Tools.ResetDisables();

            if (searchString == "")
                return;

            var trav = Traverse.Create(selectUI);
            var datas = trav.Field("cells").GetValue<ThumbnailSelectCell[]>();

            for (var i = 0; i < datas.Length; i++)
                if(!Tools.ItemMatchesSearch(searchString, i))
                    datas[i].gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Cacher.WriteCache();
        }
    }
}