using System.Collections.Generic;
using System.Linq;
using PokemonSystem.Core.Experiences;
using PokemonSystem.Core.Items;
using PokemonSystem.Core.Moves;
using PokemonSystem.Core.Statistics;

namespace PokemonSystem.Core;

/// <summary>
/// 宝可梦
/// </summary>
public class Pokemon
{
    /// <summary>
    /// 经验值
    /// </summary>
    public Experience Exp;

    /// <summary>
    /// 友好度
    /// </summary>
    public byte Affection;

    /// <summary>
    /// 宝可梦数据
    /// </summary>
    public readonly PokemonData PokemonModel;

    /// <summary>
    /// 性格
    /// </summary>
    public Nature Nature;

    /// <summary>
    /// 个体值
    /// </summary>
    public IndividualValues IV;

    /// <summary>
    /// 基础点数
    /// </summary>
    public BasePoints EV;

    /// <summary>
    /// 携带道具
    /// </summary>
    public HeldItem HeldItem;

    /// <summary>
    /// 学会的招式
    /// </summary>
    public List<Move> LearnedMoves;

    /// <summary>
    /// 当前的招式
    /// </summary>
    public readonly Move[] CurrentMoves = new Move[4];

    /// <summary>
    /// 将宝可梦保存数据转换成宝可梦对象
    /// </summary>
    /// <param name="data">宝可梦保存数据</param>
    public Pokemon(PokemonSaveData data)
    {
        PokemonModel = DataManager.GetDataFromKey<PokemonData>(data.PokemonKey);

        Exp = new Experience(data.Exp, PokemonModel.ExpGroup);
        Affection = data.Affection;
        Nature = DataManager.GetDataFromKey<Nature>(data.NatureKey);
        IV = data.IV;
        EV = data.EV;
        HeldItem = DataManager.GetDataFromKey<HeldItem>(data.HeldItemKey);
        LearnedMoves = DataManager.GetDataFromKey<Move>(data.LearnedMoves);
        DataManager.GetDataFromKey<Move>(data.CurrentMoves.Take(data.CurrentMoves.Length)).CopyTo(CurrentMoves);
    }

    /*
    public Statistic GetStatistic()
    {
        var stat = new Statistic();

        return stat;
    }
    */
}