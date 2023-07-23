using System;

namespace PokemonSystem.Core;

/// <summary>
/// 个体值
/// </summary>
[Serializable]
public class IndividualValues : StatisticValue<byte>
{
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
    
    /*
    /// <summary>
    /// 体力
    /// </summary>
    public byte HP = 0;
    
    /// <summary>
    /// 攻击
    /// </summary>
    public byte Attack = 0;
    
    /// <summary>
    /// 防御
    /// </summary>
    public byte Defense = 0;
    
    /// <summary>
    /// 特攻
    /// </summary>
    public byte SpecialAttack = 0;
    
    /// <summary>
    /// 特防
    /// </summary>
    public byte SpecialDefense = 0;
    
    /// <summary>
    /// 速度
    /// </summary>
    public byte Speed = 0;
    */
}