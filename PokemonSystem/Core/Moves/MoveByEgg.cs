using System;
using PokemonSystem.Core.Items;

namespace PokemonSystem.Core.Moves;

/// <summary>
/// 蛋招式
/// </summary>
[Serializable]
public class MoveByEgg
{
    /// <summary>
    /// 招式
    /// </summary>
    public Move Move;

    /// <summary>
    /// 亲代
    /// </summary>
    public PokemonData[] Parents;

    /// <summary>
    /// 亲代需要携带的道具
    /// </summary>
    public HeldItem NeedItem;
}