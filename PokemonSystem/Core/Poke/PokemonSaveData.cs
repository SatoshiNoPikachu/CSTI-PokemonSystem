using System;
using System.Linq;
using LitJson;
using PokemonSystem.Core.Statistics;

namespace PokemonSystem.Core.Poke;

/// <summary>
/// 宝可梦保存数据
/// </summary>
[Serializable]
public class PokemonSaveData
{
    /// <summary>
    /// 经验值
    /// </summary>
    public int Exp;

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

    public PokemonSaveData()
    {
    }

    /// <summary>
    /// 将宝可梦对象转换成可序列化的数据
    /// </summary>
    /// <param name="pkm">宝可梦对象</param>
    public PokemonSaveData(Pokemon pkm)
    {
        Exp = pkm.Exp.Exp;
        Affection = pkm.Affection;
        PokemonKey = pkm.PokemonModel.name;
        NatureKey = pkm.Nature.name;
        IV = pkm.IV;
        EV = pkm.EV;
        HeldItemKey = pkm.HeldItem.name;
        LearnedMoves = (from move in pkm.LearnedMoves where move != null select move.name).ToArray();
        CurrentMoves = (from move in pkm.CurrentMoves where move != null select move.name).ToArray();
    }

    /// <summary>
    /// 转换成宝可梦对象
    /// </summary>
    /// <returns></returns>
    public Pokemon ToPokemon() => new(this);

    /// <summary>
    /// 转换成 Json 文本
    /// </summary>
    /// <returns></returns>
    public string ToJson() => JsonMapper.ToJson(this);
}