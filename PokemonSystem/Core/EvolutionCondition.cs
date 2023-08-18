using System;
using PokemonSystem.Core.Items;
using PokemonSystem.Core.Poke;

namespace PokemonSystem.Core;

/// <summary>
/// 进化条件
/// </summary>
[Serializable]
public class EvolutionCondition
{
    /// <summary>
    /// 进化后的宝可梦
    /// </summary>
    public PokemonData ToPokemon;

    /// <summary>
    /// 额外产生的宝可梦
    /// </summary>
    public PokemonData ExtraPokemon;

    /// <summary>
    /// 需要等级
    /// </summary>
    public byte NeedLevel;

    /// <summary>
    /// 需要友好度
    /// </summary>
    public byte NeedAffection;

    /// <summary>
    /// 需要道具
    /// </summary>
    public Item NeedItem;

    /// <summary>
    /// 需要时间
    /// </summary>
    public TimeSlot NeedTime;

    /// <summary>
    /// 需要性别
    /// </summary>
    public GenderEnum NeedGender;
}