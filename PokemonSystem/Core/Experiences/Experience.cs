namespace PokemonSystem.Core.Experiences;

/// <summary>
/// 经验
/// </summary>
public class Experience
{
    /// <summary>
    /// 最大等级
    /// </summary>
    private const byte MaxLevel = 100;

    /// <summary>
    /// 经验值
    /// </summary>
    public int Exp { get; private set; }

    /// <summary>
    /// 等级
    /// </summary>
    public byte Level { get; private set; }

    /// <summary>
    /// 经验组
    /// </summary>
    private readonly ExperienceGroup _expGroup;
    
    /// <summary>
    /// 实例化经验对象
    /// </summary>
    /// <param name="exp">经验值</param>
    /// <param name="expGroup">经验组</param>
    public Experience(int exp, ExperienceGroup expGroup)
    {
        Exp = exp;
        Level = ExperienceTable.GetLevel(exp, expGroup);
        _expGroup = expGroup;
    }

    /// <summary>
    /// 增加经验
    /// </summary>
    /// <param name="exp">增加的经验值</param>
    /// <returns>等级</returns>
    public byte AddExp(int exp)
    {
        Exp += exp;
        if (Level < MaxLevel && Exp > ExperienceTable.GetExp(Level + 1, _expGroup)) Level += 1;
        return Level;
    }
}