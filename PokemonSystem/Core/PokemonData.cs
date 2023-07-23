using System;
using UnityEngine;

namespace PokemonSystem.Core;

/// <summary>
/// 宝可梦数据
/// </summary>
[Serializable]
public class PokemonData : ScriptableObject
{
    /// <summary>
    /// 宝可梦名称
    /// </summary>
    public LocalizedString PokemonName;

    /// <summary>
    /// 宝可梦立绘
    /// </summary>
    public Sprite PokemonImage;

    /// <summary>
    /// 第一属性
    /// </summary>
    public Type Type1;

    /// <summary>
    /// 第二属性
    /// </summary>
    public Type Type2;

    /// <summary>
    /// 种族值
    /// </summary>
    public SpeciesStrength SpeciesStrength;
}