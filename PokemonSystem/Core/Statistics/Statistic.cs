using System;
using PokemonSystem.Core.Poke;
using UnityEngine;

namespace PokemonSystem.Core.Statistics;

/// <summary>
/// 能力
/// </summary>
public sealed class Statistic : StatisticValue<int>
{
    /// <summary>
    /// 计算能力值
    /// <br/>
    /// 若参数 strength 为 0，则返回值为 1
    /// </summary>
    /// <param name="type">能力类型</param>
    /// <param name="nature">性格</param>
    /// <param name="strength">种族值</param>
    /// <param name="iv">个体值</param>
    /// <param name="ev">基础点数</param>
    /// <param name="level">等级</param>
    /// <returns>能力值</returns>
    private static int CalculateValue(StatisticType type, Nature nature, byte strength, byte iv, byte ev, byte level)
    {
        if (strength == 0) return 1;

        if (type == StatisticType.HP) return (strength * 2 + iv + ev / 4) * level / 100 + 10 + level;

        var value = (strength * 2 + iv + ev / 4) * level / 100 + 5;

        var fix = 1.0;
        if (type == nature.StatisticUp) fix += 0.1;
        if (type == nature.StatisticDown) fix -= 0.1;

        return (int)(value * fix);
    }

    /// <summary>
    /// 创建指定宝可梦的能力对象
    /// </summary>
    /// <param name="pokemon">宝可梦</param>
    /// <returns>能力对象</returns>
    public static Statistic Create(Pokemon pokemon)
    {
        return pokemon?.PokemonModel is null ? null : new Statistic(pokemon);
    }

    /// <summary>
    /// 实例化并计算宝可梦能力
    /// </summary>
    /// <param name="pokemon">宝可梦</param>
    private Statistic(Pokemon pokemon)
    {
        var strength = pokemon.PokemonModel.SpeciesStrength;

        foreach (var type in GetTypes())
        {
            var sp = strength[type];
            var iv = pokemon.IV[type];
            var ev = pokemon.EV[type];
            var lv = pokemon.Exp.Level;

            if (sp is null || iv is null || ev is null)
            {
                this[type] = 1;
                Debug.LogWarning($"Not found {Enum.GetName(typeof(StatisticType), type)} value.");
                continue;
            }

            this[type] = CalculateValue(type, pokemon.Nature, (byte)sp, (byte)iv, (byte)ev, lv);
        }
    }
}