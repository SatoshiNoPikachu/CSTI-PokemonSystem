using HarmonyLib;
using System.Collections;
using System.Reflection;

namespace PokemonSystem;

internal class BaseMod
{
    private static readonly MethodInfo Gm_ChangeStatValue = AccessTools.Method(typeof(GameManager), "ChangeStatValue");
    private static readonly MethodInfo Gm_RemoveCard = AccessTools.Method(typeof(GameManager), "RemoveCard");

    public static void ChangeStatValue(InGameStat stat, float value)
    {
        GameManager.Instance.StartCoroutine((IEnumerator)Gm_ChangeStatValue.Invoke(GameManager.Instance,
            new object[] { stat, value, StatModification.Permanent }));
    }

    public static void RemoveCard(InGameCardBase card)
    {
        GameManager.Instance.StartCoroutine((IEnumerator)Gm_RemoveCard.Invoke(GameManager.Instance,
            new object[] { card, false, false, 0, false }));
    }
}