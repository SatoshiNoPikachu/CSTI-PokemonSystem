using System;
using UnityEngine;

namespace PokemonSystem.Core;

/// <summary>
/// 蛋群
/// </summary>
[Serializable]
public class EggGroup : ScriptableObject
{
    /// <summary>
    /// 蛋群名称
    /// </summary>
    public LocalizedString EggGroupName;
}