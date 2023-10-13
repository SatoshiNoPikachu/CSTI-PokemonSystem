using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PokemonSystem.Module;

/// <summary>
/// 附加数据
/// </summary>
public class ExtraData
{
    /// <summary>
    /// 前缀
    /// </summary>
    private const string Prefix = "PokeSysExtraData|";
    
    /// <summary>
    /// 源
    /// </summary>
    private readonly object _source;
    
    /// <summary>
    /// 数据
    /// </summary>
    private readonly Dictionary<string, string> _data = new();

    /// <summary>
    /// 创建卡牌的附加数据对象
    /// </summary>
    /// <param name="card">卡牌对象</param>
    public ExtraData(InGameCardBase card)
    {
        _source = card;
        LoadData(card);
    }

    /// <summary>
    /// 创建状态的附加数据对象
    /// </summary>
    /// <param name="stat">状态对象</param>
    public ExtraData(InGameStat stat)
    {
        _source = stat;
        LoadData(stat);
    }

    /// <summary>
    /// 重新加载
    /// </summary>
    public void Reload()
    {
        if (_source.GetType() == typeof(InGameStat)) LoadData((InGameStat)_source);
        else LoadData((InGameCardBase)_source);
    }

    /// <summary>
    /// 加载卡牌的附加数据
    /// </summary>
    /// <param name="card">卡牌对象</param>
    private void LoadData(InGameCardBase card)
    {
        _data.Clear();
        foreach (var raw in card.DroppedCollections.Keys)
        {
            LoadData(raw);
        }
    }

    /// <summary>
    /// 加载状态的附加数据对象
    /// </summary>
    /// <param name="stat">状态对象</param>
    private void LoadData(InGameStat stat)
    {
        _data.Clear();
        foreach (var raw in stat.StalenessValues.Select(values => values.ModifierSource))
        {
            LoadData(raw);
        }
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="raw">原始数据</param>
    private void LoadData(string raw)
    {
        if (!TryGetData(raw, out var s)) return;
        var data = ResolveData(s);
        if (_data.ContainsKey(data.Item1)) return;
        _data[data.Item1] = data.Item2;
    }

    /// <summary>
    /// 尝试获取数据
    /// </summary>
    /// <param name="value">数据</param>
    /// <param name="data">当获取成功，则包含去除前缀后的数据，否则为空字符串</param>
    /// <returns>是否成功获取</returns>
    private static bool TryGetData(string value, out string data)
    {
        if (value.StartsWith(Prefix))
        {
            data = value.Substring(Prefix.Length);
            return true;
        }

        data = "";
        return false;
    }

    /// <summary>
    /// 解析数据
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>通过解析数据获得的键值元组</returns>
    private static (string, string) ResolveData(string data)
    {
        var content = data.Split(new[] { ':' }, 2);
        if (content.Length == 1) content = new[] { content[0], "" };
        return (content[0], content[1]);
    }

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="def">未找到对应键时返回的默认值</param>
    /// <param name="setIfNotFound">若未找到对应键时是否使用默认值设置</param>
    /// <returns>对应键的值</returns>
    public string Get(string key, string def = "", bool setIfNotFound = true)
    {
        if (_data.TryGetValue(key, out var value)) return value;
        value = def;
        if (setIfNotFound) Set(key, value);
        return value;
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public void Set(string key, string value)
    {
        if (_source is InGameCardBase card) Set(key, value, card);
        else Set(key, value, (InGameStat)_source);
    }

    /// <summary>
    /// 设置卡牌数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="card">卡牌对象</param>
    private void Set(string key, string value, InGameCardBase card)
    {
        _data[key] = value;
        var data = $"{Prefix}{key}:{value}";
        var values = card.DroppedCollections;
        var delete = new List<string>();
        foreach (var d in values.Keys)
        {
            if (!TryGetData(d, out var s)) continue;
            if (ResolveData(s).Item1 != key) continue;
            delete.Add(d);
        }

        foreach (var k in delete)
        {
            values.Remove(k);
        }

        card.DroppedCollections[data] = Vector2Int.zero;
    }

    /// <summary>
    /// 设置状态数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="stat">状态对象</param>
    private void Set(string key, string value, InGameStat stat)
    {
        _data[key] = value;
        var data = new StalenessData($"{Prefix}{key}:{value}", 0);
        var values = stat.StalenessValues;
        for (var i = 0; i < values.Count; i++)
        {
            if (!TryGetData(values[i].ModifierSource, out var s)) continue;
            if (ResolveData(s).Item1 != key) continue;
            values[i] = data;
            return;
        }

        values.Add(data);
    }
}