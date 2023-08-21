using System.ComponentModel;

namespace PokemonSystem.Core;

/// <summary>
/// 性别比例
/// </summary>
public enum GenderRatio
{
    /// <summary>
    /// 无性别
    /// </summary>
    None,
    
    /// <summary>
    /// 只有雄性
    /// </summary>
    OnlyMale,
    
    /// <summary>
    /// 只有雌性
    /// </summary>
    OnlyFemale,
    
    /// <summary>
    /// 7 : 1
    /// </summary>
    [Description("7:1")]
    _71,
    
    /// <summary>
    /// 3 : 1
    /// </summary>
    [Description("3:1")]
    _31,
    
    /// <summary>
    /// 1 : 1
    /// </summary>
    [Description("1:1")]
    _11,
    
    /// <summary>
    /// 1 : 3
    /// </summary>
    [Description("1:3")]
    _13,
    
    /// <summary>
    /// 1 : 7
    /// </summary>
    [Description("1:7")]
    _17
}