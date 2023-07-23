using System;
using System.Linq;
using LitJson;

namespace PokemonSystem.Core;

/// <summary>
/// 宝可梦保存数据
/// </summary>
[Serializable]
public class PokemonSaveData
{
    /// <summary>
    /// 友好度
    /// </summary>
    public byte Affection;

    /// <summary>
    /// 宝可梦 Key
    /// </summary>
    public string PokemonKey;

    /// <summary>
    /// 性格 Key
    /// </summary>
    public string NatureKey;

    /// <summary>
    /// 个体值
    /// </summary>
    public IndividualValues IV;

    /// <summary>
    /// 基础点数
    /// </summary>
    public BasePoints EV;

    /// <summary>
    /// 携带道具 Key
    /// </summary>
    public string HeldItemKey;

    /// <summary>
    /// 学会的招式
    /// </summary>
    public string[] LearnedMoves;

    /// <summary>
    /// 当前的招式
    /// </summary>
    public string[] CurrentMoves;

    /// <summary>
    /// 将宝可梦对象转换成可序列化的数据
    /// </summary>
    /// <param name="pkm">宝可梦对象</param>
    public PokemonSaveData(Pokemon pkm)
    {
        Affection = pkm.Affection;
        PokemonKey = pkm.PokemonModel.name;
        NatureKey = pkm.Nature.name;
        IV = pkm.IV;
        EV = pkm.EV;
        HeldItemKey = pkm.HeldItem.name;
        LearnedMoves = (from move in pkm.LearnedMoves where move != null select move.name).ToArray();
        CurrentMoves = (from move in pkm.CurrentMoves where move != null select move.name).ToArray();
    }

    public string ToJson() => JsonMapper.ToJson(this);
}