using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using PokemonSystem.Core;
using UnityEngine;
using Type = System.Type;

namespace PokemonSystem;

public static class DataManager
{
    private static readonly Dictionary<Type, IDictionary> AllData = new();

    private static readonly Dictionary<string, Core.Type> TypeDict = new();
    private static readonly Dictionary<string, Nature> NatureDict = new();

    static DataManager()
    {
        AllData.Add(typeof(Core.Type), TypeDict);
        AllData.Add(typeof(Nature), NatureDict);
    }

    private static Dictionary<string, T> GetDataDict<T>() where T : class
    {
        return (from data in AllData where data.Key == typeof(T) select data.Value as Dictionary<string, T>)
            .FirstOrDefault();
    }

    public static T GetDataFromKey<T>(string key) where T : class
    {
        var dict = GetDataDict<T>();
        if (dict == null) return null;
        return dict.TryGetValue(key, out var data) ? data : null;
    }

    public static List<T> GetDataFromKey<T>(IEnumerable<string> keys) where T : class
    {
        var dict = GetDataDict<T>();
        if (dict == null) return null;
        var list = new List<T>();
        foreach (var key in keys)
        {
            if (!dict.TryGetValue(key, out var data)) continue;
            if (data != null) list.Add(data);
            else Debug.LogWarning($"[PokemonSystem]: Not find key {key} in {typeof(T).Name} sequence.");
        }
        return list;
    }

    public static void LoadData()
    {
        _LoadDataScriptObj(TypeDict, "PokemonSystem_Type");

        _LoadDataScriptObj(NatureDict, "PokemonSystem_Nature", false);
    }

    // private static void _LoadData<T>(Dictionary<string, T> dict, string data_name)
    // {
    //     dict.Clear();
    //
    //     var path = Path.Combine(Plugin.GetSelfDllPath(), data_name);
    //     if (!Directory.Exists(path)) return;
    //     var files = new DirectoryInfo(path).GetFiles("*.json");
    //     foreach (var file in files)
    //     {
    //         var json = File.ReadAllText(file.FullName, Encoding.UTF8);
    //         var name = Path.GetFileNameWithoutExtension(file.Name);
    //         var obj = JsonMapper.ToObject<T>(json);
    //         dict[name] = obj;
    //     }
    // }

    /// <summary>
    /// 加载 ScriptableObject 数据
    /// </summary>
    /// <param name="dict">存储字典</param>
    /// <param name="data_name">目录名称</param>
    /// <param name="is_warp"></param>
    /// <typeparam name="T">派生自 ScriptableObject 的类型</typeparam>
    private static void _LoadDataScriptObj<T>(Dictionary<string, T> dict, string data_name, bool is_warp = true)
        where T : ScriptableObject
    {
        dict.Clear();

        var type = typeof(T);
        if (ModLoader.ModLoader.AllScriptableObjectWithoutGuidTypeDict.TryGetValue(type, out var value))
            value.Clear();
        else
            ModLoader.ModLoader.AllScriptableObjectWithoutGuidTypeDict.Add(type,
                new Dictionary<string, ScriptableObject>());

        var path_root = Path.Combine(BepInEx.Paths.BepInExRootPath, "plugins");
        var mod_dirs = new DirectoryInfo(path_root).GetDirectories();
        var self_name = Path.GetDirectoryName(Plugin.GetSelfDllPath());
        var json_dict = new Dictionary<string, string>();

        foreach (var mod_dir in mod_dirs)
        {
            var path = Path.Combine(mod_dir.FullName, "ScriptableObject", data_name);
            if (!Directory.Exists(path)) continue;

            var files = new DirectoryInfo(path).GetFiles("*.json");
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                if (dict.ContainsKey(name))
                {
                    if (mod_dir.Name == self_name)
                        Debug.LogWarning($"[PokemonSystem]: PokemonSystem override {data_name} same key {name}.");
                    else
                    {
                        Debug.LogWarning($"[PokemonSystem]: {mod_dir.Name} not load {data_name} same key {name}.");
                        continue;
                    }
                }

                var json = File.ReadAllText(file.FullName, Encoding.UTF8);
                if (is_warp) json_dict[name] = json;

                var obj = ScriptableObject.CreateInstance<T>();
                JsonUtility.FromJsonOverwrite(json, obj);
                obj.name = name;
                dict[name] = obj;

                ModLoader.ModLoader.AllScriptableObjectWithoutGuidTypeDict[type][name] = obj;
            }
        }

        if (!is_warp) return;
        foreach (var data in dict)
            ModLoader.WarpperFunction.JsonCommonWarpper(data.Value, JsonMapper.ToObject(json_dict[data.Key]));
    }
}