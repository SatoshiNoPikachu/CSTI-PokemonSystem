using System;
using PokemonSystem.Core.Experiences;
using PokemonSystem.Core.Moves;
using PokemonSystem.Core.Statistics;
using UnityEngine;

namespace PokemonSystem.Core.Poke;

/// <summary>
/// 宝可梦数据
/// </summary>
[Serializable]
public class PokemonData : ScriptableObject, IScriptableObject
{
    /// <summary>
    /// 宝可梦名称
    /// </summary>
    public LocalizedString PokemonName;

    /// <summary>
    /// 图鉴介绍
    /// </summary>
    public LocalizedString PokedexDesc;

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

    /// <summary>
    /// 经验组
    /// </summary>
    public ExperienceGroup ExpGroup;
    
    /// <summary>
    /// 性别比例
    /// </summary>
    public GenderRatio GenderRatio;

    /// <summary>
    /// 身高
    /// </summary>
    public float Height;

    /// <summary>
    /// 体重
    /// </summary>
    public float Weight;

    /// <summary>
    /// 基础经验值
    /// </summary>
    public short BaseExp;

    /// <summary>
    /// 捕获率
    /// </summary>
    public byte CatchRate;

    /// <summary>
    /// 孵化周期
    /// </summary>
    public byte EggCycle;

    /// <summary>
    /// 通过等级提升学习的招式
    /// </summary>
    public MoveByLevel[] MoveByLevel;

    /// <summary>
    /// 通过招式学习器学习的招式
    /// </summary>
    public Move[] MoveByTM;

    /// <summary>
    /// 可学会的蛋招式
    /// </summary>
    public MoveByEgg[] MoveByEgg;

    /// <summary>
    /// 蛋群
    /// </summary>
    public EggGroup[] EggGroup;

    /// <summary>
    /// 进化条件
    /// </summary>
    public EvolutionCondition[] EvolutionConditions;
}