using System;

namespace PokemonSystem.Core.Statistics;

/// <summary>
/// 基础点数
/// </summary>
[Serializable]
public class BasePoints : StatisticValue<byte>
{
    /// <summary>
    /// 单项最大值
    /// </summary>
    public const byte MaxValue = 252;

    /// <summary>
    /// 最大总和
    /// </summary>
    public const short MaxSum = 510;

    /// <summary>
    /// 获取基础点数值
    /// </summary>
    /// <param name="type">能力类型</param>
    /// <returns>基础点数值</returns>
    public override byte? GetValue(StatisticType type)
    {
        CheckValues();
        return base.GetValue(type);
    }

    /// <summary>
    /// 设置基础点数值
    /// </summary>
    /// <param name="type">能力类型</param>
    /// <param name="value">值</param>
    /// <returns>是否进行了赋值</returns>
    public override bool SetValue(StatisticType type, byte value)
    {
        if (value > MaxValue) value = MaxValue;
        
        var current = GetValue(type);
        if (current is null) return false;
        var sum = GetSum();

        var after = sum + value - (byte)current;
        return after > MaxSum ? base.SetValue(type, (byte)(value - after + MaxSum)) : base.SetValue(type, value);
    }
    
    /// <summary>
    /// 校验值
    /// </summary>
    public void CheckValues()
    {
        if (HP > MaxValue) HP = MaxValue;
        if (Attack > MaxValue) Attack = MaxValue;
        if (Defense > MaxValue) Defense = MaxValue;
        if (SpecialAttack > MaxValue) SpecialAttack = MaxValue;
        if (SpecialDefense > MaxValue) SpecialDefense = MaxValue;
        if (Speed > MaxValue) Speed = MaxValue;

        if (GetSum() > MaxSum)
            HP = Attack = Defense = SpecialAttack = SpecialDefense = Speed = 0;
    }

    /// <summary>
    /// 获取总和
    /// </summary>
    /// <returns>总和</returns>
    public int GetSum() => HP + Attack + Defense + SpecialAttack + SpecialDefense + Speed;
}