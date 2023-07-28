using System;

namespace PokemonSystem.Core.Moves;

/// <summary>
/// 通过等级提升学习的招式
/// </summary>
[Serializable]
public class MoveByLevel
{
    /// <summary>
    /// 招式
    /// </summary>
    public Move Move;

    /// <summary>
    /// 需要等级
    /// </summary>
    public byte NeedLevel;

    /// <summary>
    /// 是否为进化时学习
    /// </summary>
    public bool IsEvolving;
}