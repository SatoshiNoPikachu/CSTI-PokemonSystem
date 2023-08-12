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
    /// 获取能力值
    /// <br/>
    /// 若参数 type 为非基本能力类型，则返回 null
    /// </summary>
    /// <param name="type">能力类型</param>
    /// <returns>能力值</returns>
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

    /// <summary>
    /// 设置能力值
    /// </summary>
    /// <param name="type">能力类型</param>
    /// <param name="value">值</param>
    /// <returns>是否进行了赋值</returns>
    public virtual bool SetValue(StatisticType type, T value)
    {
        switch (type)
        {
            case StatisticType.HP:
                HP = value;
                return true;
            case StatisticType.Attack:
                Attack = value;
                return true;
            case StatisticType.Defense:
                Defense = value;
                return true;
            case StatisticType.SpecialAttack:
                SpecialAttack = value;
                return true;
            case StatisticType.SpecialDefense:
                SpecialDefense = value;
                return true;
            case StatisticType.Speed:
                Speed = value;
                return true;
            case StatisticType.Null:
            default:
                return false;
        }
    }
}