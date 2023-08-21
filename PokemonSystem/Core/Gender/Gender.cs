using System;

namespace PokemonSystem.Core.Gender;

/// <summary>
/// 性别
/// </summary>
public class Gender
{
    /// <summary>
    /// 获取随机性别
    /// </summary>
    /// <param name="ratio">性别比例</param>
    /// <returns>性别枚举</returns>
    public static GenderEnum GetRandomGender(GenderRatio ratio)
    {
        switch (ratio)
        {
            case GenderRatio.None:
                return GenderEnum.Unknown;
            case GenderRatio.OnlyMale:
                return GenderEnum.Male;
            case GenderRatio.OnlyFemale:
                return GenderEnum.Female;
            case GenderRatio._71:
            case GenderRatio._31:
            case GenderRatio._11:
            case GenderRatio._13:
            case GenderRatio._17:
            default:
                var num = new Random(Guid.NewGuid().GetHashCode()).Next(1, 254);
                return ratio switch
                {
                    GenderRatio._71 => num < 31 ? GenderEnum.Female : GenderEnum.Male,
                    GenderRatio._31 => num < 63 ? GenderEnum.Female : GenderEnum.Male,
                    GenderRatio._11 => num < 127 ? GenderEnum.Female : GenderEnum.Male,
                    GenderRatio._13 => num < 191 ? GenderEnum.Female : GenderEnum.Male,
                    GenderRatio._17 => num < 223 ? GenderEnum.Female : GenderEnum.Male,
                    _ => GenderEnum.Unknown
                };
        }
    }
}