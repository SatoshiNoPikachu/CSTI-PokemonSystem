using System;

namespace PokemonSystem.Core.Statistics;

/// <summary>
/// 能力值
/// </summary>
/// <typeparam name="T">值类型</typeparam>
[Serializable]
public abstract class StatisticValue<T> where T : struct
{
    /// <summary>
    /// 体力
    /// </summary>
    public T HP;

    /// <summary>
    /// 攻击
    /// </summary>
    public T Attack;

    /// <summary>
    /// 防御
    /// </summary>
    public T Defense;

    /// <summary>
    /// 特攻
    /// </summary>
    public T SpecialAttack;

    /// <summary>
    /// 特防
    /// </summary>
    public T SpecialDefense;

    /// <summary>
    /// 速度
    /// </summary>
    public T Speed;

    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="type">能力类型</param>
    /// <returns>返回参数 type 所对应能力的值，若不为六种基本的能力类型，则返回 null</returns>
    public virtual T? GetValue(StatisticType type)
    {
        return type switch
        {
            StatisticType.HP => HP,
            StatisticType.Attack => Attack,
            StatisticType.Defense => Defense,
            StatisticType.SpecialAttack => SpecialAttack,
            StatisticType.SpecialDefense => SpecialDefense,
            StatisticType.Speed => Speed,
            _ => null
        };
    }
}