using HarmonyLib;

namespace PokemonSystem.Patcher;

[Harmony]
public static class CardGraphicsPatch
{
    [HarmonyPostfix, HarmonyPatch(typeof(CardGraphics), "Setup")]
    public static void Setup_Postfix(CardGraphics __instance , InGameCardBase _From)
    {
        
    }
}