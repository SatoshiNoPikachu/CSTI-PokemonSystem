using System;
using PokemonSystem.Core.Statistics;
using UnityEngine;

namespace PokemonSystem.Core;

/// <summary>
/// 性格
/// </summary>
[Serializable]
public class Nature : ScriptableObject
{
    /// <summary>
    /// 性格名称
    /// </summary>
    public LocalizedString NatureName;

    /// <summary>
    /// 容易成长的能力
    /// </summary>
    public StatisticType StatisticUp;

    /// <summary>
    /// 不容易成长的能力
    /// </summary>
    public StatisticType StatisticDown;
}