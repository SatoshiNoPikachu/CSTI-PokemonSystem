using System;
using PokemonSystem.Core.Moves;

namespace PokemonSystem.Core.Items;

/// <summary>
/// 招式学习器
/// </summary>
[Serializable]
public class TechnicalMachine : Item
{
    /// <summary>
    /// 招式
    /// </summary>
    public Move Move;
}