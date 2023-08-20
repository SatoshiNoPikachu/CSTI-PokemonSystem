using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;

namespace PokemonSystem;

[BepInPlugin("Pikachu.PokemonSystem", "Pokemon System", "0.0.0.1")]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance;

    private void Awake()
    {
        if (AccessTools.TypeByName("ModLoader.ModPack") != null && IsDisable("PokemonSystem")) return;
        Instance = this;

        Harmony.CreateAndPatchAll(typeof(Plugin));
        Logger.LogInfo("Plugin [Pokemon System] is loaded!");
    }

    private static bool IsDisable(string mod_name)
    {
        return !ModLoader.ModLoader.ModPacks.TryGetValue(mod_name, out var pack) || pack == null ||
               !pack.EnableEntry.Value;
    }

    public static string GetSelfDllPath()
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }

    [HarmonyPostfix, HarmonyPatch(typeof(GameLoad), "LoadMainGameData")]
    public static void GameLoad_LoadMainGameData_Postfix()
    {
        Manager.DataManager.LoadData();
    }
}