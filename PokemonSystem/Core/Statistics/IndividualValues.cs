using System;

namespace PokemonSystem.Core.Statistics;

/// <summary>
/// 个体值
/// </summary>
[Serializable]
public class IndividualValues : StatisticValue<byte>
{
    /// <summary>
    /// 单项最大值
    /// </summary>
    public const byte MaxValue = 31;
    
    /// <summary>
    /// 获取随机个体值
    /// </summary>
    /// <returns>个体值对象</returns>
    public static IndividualValues GetRandomIV()
    {
        Random random = new(Guid.NewGuid().GetHashCode());
        IndividualValues iv = new()
        {
            HP = (byte)random.Next(32),
            Attack = (byte)random.Next(32),
            Defense = (byte)random.Next(32),
            SpecialAttack = (byte)random.Next(32),
            SpecialDefense = (byte)random.Next(32),
            Speed = (byte)random.Next(32),
        };
        return iv;
    }

    /// <summary>
    /// 获取个体值，返回值不会大于允许的最大值
    /// <br/>
    /// 若参数 type 为非基本能力类型，则返回 null
    /// </summary>
    /// <param name="type">能力类型</param>
    /// <returns>能力值</returns>
    public override byte? GetValue(StatisticType type)
    {
        var value = base.GetValue(type);
        return value is null or > MaxValue ? value : MaxValue;
    }

    
    /// <summary>
    /// 设置个体值，该值不会大于允许的最大值
    /// </summary>
    /// <param name="type">能力类型</param>
    /// <param name="value">值</param>
    /// <returns>是否进行了赋值</returns>
    public override bool SetValue(StatisticType type, byte value)
    {
        return base.SetValue(type, value > MaxValue ? MaxValue : value);
    }
}