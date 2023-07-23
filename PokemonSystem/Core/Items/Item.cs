using UnityEngine;

namespace PokemonSystem.Core.Items;

/// <summary>
/// 道具
/// </summary>
public abstract class Item : ScriptableObject
{
    public LocalizedString ItemName;
}