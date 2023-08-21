using HarmonyLib;
using PokemonSystem.Manager;

namespace PokemonSystem.Patcher;

[Harmony]
public static class GameLoadPatch
{
    [HarmonyPostfix, HarmonyPatch(typeof(GameLoad), "LoadMainGameData")]
    public static void LoadMainGameData_Postfix()
    {
        // 加载数据
        DataManager.LoadData();
    }
}