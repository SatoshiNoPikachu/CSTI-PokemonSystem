using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HarmonyLib;
using LitJson;
using PokemonSystem.Core;
using PokemonSystem.Core.Moves;
using PokemonSystem.Core.Poke;
using UnityEngine;
using Object = UnityEngine.Object;
using Type = System.Type;

namespace PokemonSystem.Manager;

public static class DataManager
{
    private static readonly Dictionary<Type, IDictionary> AllData = new();
    private static readonly Dictionary<string, Core.Type> TypeDict = new();
    private static readonly Dictionary<string, Nature> NatureDict = new();
    private static readonly Dictionary<string, EggGroup> EggGroupDict = new();
    private static readonly Dictionary<string, Move> MoveDict = new();
    private static readonly Dictionary<string, PokemonData> PokemonDataDict = new();

    static DataManager()
    {
        AllData.Add(typeof(Core.Type), TypeDict);

        AllData.Add(typeof(Nature), NatureDict);

        AllData.Add(typeof(EggGroup), EggGroupDict);

        AllData.Add(typeof(Move), MoveDict);

        AllData.Add(typeof(PokemonData), PokemonDataDict);
    }

    public static void LoadData()
    {
        var warp_data = new Dictionary<ScriptableObject, JsonData>();

        LoadDataScriptObj(TypeDict, "Poke-Type", warp_data);

        LoadDataScriptObj(NatureDict, "Poke-Nature");

        LoadDataScriptObj(EggGroupDict, "Poke-EggGroup");

        LoadDataScriptObj(MoveDict, "Poke-Move", warp_data);

        LoadDataScriptObj(PokemonDataDict, "Poke-Pokemon", warp_data);

        WarpData(warp_data);
    }

    /// <summary>
    /// 获取数据字典
    /// </summary>
    /// <typeparam name="T">引用数据类型</typeparam>
    /// <returns>数据字典</returns>
    private static Dictionary<string, T> GetDataDict<T>() where T : class
    {
        return (from data in AllData where data.Key == typeof(T) select data.Value as Dictionary<string, T>)
            .FirstOrDefault();
    }

    /// <summary>
    /// 根据键获取数据
    /// </summary>
    /// <param name="key">数据键</param>
    /// <typeparam name="T">引用数据类型</typeparam>
    /// <returns>数据</returns>
    public static T GetDataFromKey<T>(string key) where T : class
    {
        var dict = GetDataDict<T>();
        if (dict == null) return null;
        return dict.TryGetValue(key, out var data) ? data : null;
    }

    /// <summary>
    /// 根据多个键获取数据
    /// </summary>
    /// <param name="keys">可迭代的数据键序列</param>
    /// <typeparam name="T">引用数据类型</typeparam>
    /// <returns>数据列表</returns>
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

    /// <summary>
    /// 加载 ScriptableObject 数据
    /// </summary>
    /// <param name="dict">存储字典</param>
    /// <param name="data_name">目录名称</param>
    /// <param name="out_dict">输出字典</param>
    /// <typeparam name="T">派生自 ScriptableObject 的类型</typeparam>
    private static void LoadDataScriptObj<T>(IDictionary<string, T> dict, string data_name,
        IDictionary<ScriptableObject, JsonData> out_dict = null)
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
        var cache = new Dictionary<string, KeyValuePair<ScriptableObject, JsonData>>();

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

                var obj = ScriptableObject.CreateInstance<T>();
                JsonUtility.FromJsonOverwrite(json, obj);
                obj.name = name;
                dict[name] = obj;

                if (out_dict != null || obj is IScriptableObject)
                {
                    var json_obj = JsonMapper.ToObject(json);

                    if (out_dict != null) cache[name] = new KeyValuePair<ScriptableObject, JsonData>(obj, json_obj);

                    // 修补数据
                    if (obj is IScriptableObject) FixData(obj, json_obj);
                }

                ModLoader.ModLoader.AllScriptableObjectWithoutGuidTypeDict[type][name] = obj;
            }

            if (out_dict == null) continue;
            foreach (var data in cache)
            {
                out_dict.Add(data.Value.Key, data.Value.Value);
            }
        }
    }

    /// <summary>
    /// 修补数据
    /// </summary>
    /// <param name="obj">数据对象</param>
    /// <param name="json_data">Json数据</param>
    private static void FixData(object obj, JsonData json_data)
    {
        if (json_data.IsObject)
        {
            foreach (var field_name in json_data.Keys)
            {
                // 跳过 EditorWarp 字段
                if (field_name.EndsWith("WarpData") || field_name.EndsWith("WarpType")) continue;

                var field = AccessTools.Field(obj.GetType(), field_name);
                if (field.GetValue(obj) is not null || field.FieldType.IsSubclassOf(typeof(Object))) continue;

                var json_field = json_data[field_name];

                // 修补数组类型字段
                if (json_data[field_name].IsArray)
                {
                    var elementType = field.FieldType.GetElementType();

                    if (elementType is null)
                    {
                        Debug.LogWarning($"Unable get element type for {obj.GetType()}");
                        continue;
                    }

                    // 创建数组
                    var arr = Array.CreateInstance(elementType, json_field.Count);
                    if (!elementType.IsSubclassOf(typeof(Object))) FixData(arr, json_field);
                    field.SetValue(obj, arr);
                    continue;
                }

                var field_value = FromJson(json_field, field.FieldType);
                field.SetValue(obj, field_value);
            }
        }
        else if (json_data.IsArray)
        {
            if (!obj.GetType().IsArray)
            {
                Debug.LogWarning("JsonData is Array, but object is not.");
                return;
            }

            var element_type = obj.GetType().GetElementType();

            if (element_type is null)
            {
                Debug.LogWarning($"Unable get element type for {obj.GetType()}");
                return;
            }

            var arr = (Array)obj;
            for (var i = 0; i < json_data.Count; i++)
            {
                var element = FromJson(json_data[i], element_type);
                arr.SetValue(element, i);
            }
        }
        else Debug.LogWarning("JsonData type is not supported.");
    }

    /// <summary>
    /// 反序列化对象
    /// </summary>
    /// <param name="data">JsonData</param>
    /// <param name="type">对象类型</param>
    /// <returns>对象</returns>
    private static object FromJson(JsonData data, Type type)
    {
        var obj = JsonUtility.FromJson(data.ToJson(), type);
        if (obj is IScriptableObject) FixData(obj, data);
        return obj;
    }

    /// <summary>
    /// 数据映射
    /// </summary>
    /// <param name="data_dict">数据字典</param>
    private static void WarpData(Dictionary<ScriptableObject, JsonData> data_dict)
    {
        foreach (var data in data_dict)
        {
            ModLoader.WarpperFunction.JsonCommonWarpper(data.Key, data.Value);
        }
    }
}