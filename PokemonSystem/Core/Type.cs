using System;
using System.Linq;
using UnityEngine;

namespace PokemonSystem.Core;

/// <summary>
/// 属性
/// </summary>
[Serializable]
public class Type : ScriptableObject
{
    /// <summary>
    /// 属性名称
    /// </summary>
    public LocalizedString TypeName;

    /// <summary>
    /// 属性图标
    /// </summary>
    public Sprite TypeIcon;

    /// <summary>
    /// 效果绝佳
    /// </summary>
    public Type[] EffectiveSuper = { };

    /// <summary>
    /// 效果不好
    /// </summary>
    public Type[] EffectiveNotVery = { };

    /// <summary>
    /// 没有效果
    /// </summary>
    public Type[] EffectiveNot = { };

    /// <summary>
    /// 计算属性相性
    /// </summary>
    /// <param name="type">目标属性</param>
    /// <returns>伤害倍数</returns>
    public double CalculateMultiple(Type type)
    {
        double multiple = 1;

        if (EffectiveSuper.Contains(type)) multiple *= 2;
        else if (EffectiveNotVery.Contains(type)) multiple *= 0.5;
        else if (EffectiveNot.Contains(type)) multiple = 0;

        return multiple;
    }

    /// <summary>
    /// 计算属性相性
    /// </summary>
    /// <param name="type1">目标第一属性</param>
    /// <param name="type2">目标第二属性</param>
    /// <returns>伤害倍数</returns>
    public double CalculateMultiple(Type type1, Type type2)
    {
        double multiple = 1;

        multiple *= CalculateMultiple(type1) * CalculateMultiple(type2);

        return multiple;
    }
}