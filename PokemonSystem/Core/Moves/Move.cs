using System;
using UnityEngine;

namespace PokemonSystem.Core.Moves;

/// <summary>
/// 招式
/// </summary>
[Serializable]
public class Move : ScriptableObject
{
    /// <summary>
    /// 名称
    /// </summary>
    public LocalizedString MoveName;

    /// <summary>
    /// 说明
    /// </summary>
    public LocalizedString MoveDesc;
    
    /// <summary>
    /// 属性
    /// </summary>
    public Type Type;

    /// <summary>
    /// 分类
    /// </summary>
    public MoveCategory Category;

    /// <summary>
    /// 威力
    /// </summary>
    public int Power;

    /// <summary>
    /// 命中
    /// </summary>
    public int Accuracy;

    /// <summary>
    /// 点数
    /// </summary>
    public int PP;
    
    /// <summary>
    /// 效果
    /// </summary>
    public MoveEffect Effect;
}