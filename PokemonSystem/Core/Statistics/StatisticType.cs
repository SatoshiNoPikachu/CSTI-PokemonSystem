using System;

namespace PokemonSystem.Core.Statistics;

/// <summary>
/// 能力类型
/// </summary>
[Serializable]
public enum StatisticType
{
    /// <summary>
    /// 无
    /// </summary>
    Null,
    
    /// <summary>
    /// 体力
    /// </summary>
    HP,
    
    /// <summary>
    /// 攻击
    /// </summary>
    Attack,
    
    /// <summary>
    /// 防御
    /// </summary>
    Defense,
    
    /// <summary>
    /// 特攻
    /// </summary>
    SpecialAttack,
    
    /// <summary>
    /// 特防
    /// </summary>
    SpecialDefense,
    
    /// <summary>
    /// 速度
    /// </summary>
    Speed
}