using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using LitJson;
using ModLoader;
using PokemonSystem.Core;
using PokemonSystem.Core.Experiences;
using PokemonSystem.Core.Gender;
using PokemonSystem.Core.Moves;
using PokemonSystem.Core.Poke;
using PokemonSystem.Core.Statistics;
using UnityEngine;
using Type = System.Type;

namespace PokemonSystem.Tools;

/// <summary>
/// ModEditor JsonData
/// </summary>
public class EditorJsonData
{
    /// <summary>
    /// 输出宝可梦系统JsonData
    /// </summary>
    public static void WritePokeJsonData()
    {
        var data = new EditorJsonData(Plugin.GetSelfDllPath(), "Poke-");

        data.AddType<PokemonData>("Pokemon");
        data.AddType<Core.Type>();
        data.AddType<Nature>();
        data.AddType<SpeciesStrength>();
        data.AddType<ExperienceGroup>();
        data.AddType<GenderRatio>();
        data.AddType<MoveByLevel>();
        data.AddType<Move>();
        data.AddType<MoveByEgg>();
        data.AddType<EggGroup>();
        data.AddType<EvolutionCondition>();
        data.AddType<TimeSlot>();
        data.AddType<GenderEnum>();
        data.AddType<MoveCategory>();
        data.AddType<MoveEffect>();

        data.CreateJsonData();
    }

    /// <summary>
    /// 类型字典
    /// </summary>
    private readonly Dictionary<Type, string> _types = new();

    /// <summary>
    /// 输出路径
    /// </summary>
    private readonly string _output;

    /// <summary>
    /// 前缀
    /// </summary>
    private readonly string _prefix;

    /// <summary>
    /// 实例化 EditorJsonData 对象
    /// </summary>
    /// <param name="output">输出路径</param>
    /// <param name="prefix">前缀</param>
    public EditorJsonData(string output, string prefix = "")
    {
        _output = Path.Combine(output, "CSTI-JsonData");
        _prefix = prefix;
    }

    /// <summary>
    /// 添加类型
    /// </summary>
    /// <param name="type">类型</param>
    public void AddType(Type type)
    {
        AddType(type, type.Name);
    }

    /// <summary>
    /// 添加类型
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="name">别名</param>
    public void AddType(Type type, string name)
    {
        _types[type] = _prefix + name;
    }

    /// <summary>
    /// 添加类型
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public void AddType<T>()
    {
        AddType(typeof(T));
    }

    /// <summary>
    /// 添加类型
    /// </summary>
    /// <param name="name">别名</param>
    /// <typeparam name="T">类型</typeparam>
    public void AddType<T>(string name)
    {
        AddType(typeof(T), name);
    }

    /// <summary>
    /// 创建 JsonData
    /// </summary>
    public void CreateJsonData()
    {
        Debug.Log("----- Start Create JsonData -----");

        CreateTypeJsonData();
        CreateNotes();
        CreateTemplate();

        Debug.Log("----- End Create JsonData -----");
    }

    /// <summary>
    /// 创建 TypeJsonData
    /// </summary>
    private void CreateTypeJsonData()
    {
        var dir = Directory.CreateDirectory(Path.Combine(_output, "ScriptableObjectTypeJsonData"));

        foreach (var (type, name) in _types)
        {
            if (type.IsEnum)
            {
                CreateEnumTypeJsonData(type, name);
                continue;
            }

            Debug.Log($"Type: {type.Name}");
            var fields = new Dictionary<string, string>();
            foreach (var field in type.GetFields())
            {
                if (IsFieldNotSerialized(field)) continue;

                var field_type = ResolveType(field.FieldType);
                fields[field.Name] = _types.TryGetValue(field_type, out var n) ? n : field_type.Name;
            }

            OutputJson(dir.FullName, name, JsonMapper.ToJson(fields));
        }
    }

    /// <summary>
    /// 创建枚举类型 TypeJsonData
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="name">名称</param>
    private void CreateEnumTypeJsonData(Type type, string name)
    {
        Debug.Log($"Enum: {type.Name}");

        var fields = new Dictionary<string, int>();
        foreach (var field in type.GetFields())
        {
            if (field.Name == "value__") continue;
            var desc = field.GetCustomAttribute<DescriptionAttribute>();
            fields[desc is null ? field.Name : desc.Description] = (int)Enum.Parse(type, field.Name);
        }

        var path = Path.Combine(_output, "ScriptableObjectTypeJsonData", "EnumType");
        var dir = Directory.CreateDirectory(path);
        OutputJson(dir.FullName, name, JsonMapper.ToJson(fields));
    }

    /// <summary>
    /// 创建注释
    /// </summary>
    private void CreateNotes()
    {
        var dir_zh = Directory.CreateDirectory(Path.Combine(_output, "Notes"));
        var dir_en = Directory.CreateDirectory(Path.Combine(_output, "Notes-En"));

        foreach (var (type, name) in _types)
        {
            if (type.IsEnum) continue;

            Debug.Log($"Notes: {type.Name}");

            var zh = new FileInfo(Path.Combine(dir_zh.FullName, $"{name}.txt"));
            var en = new FileInfo(Path.Combine(dir_en.FullName, $"{name}.txt"));

            var dict_zh = LoadNotes(zh.FullName);
            var dict_en = LoadNotes(en.FullName);

            var fields = (from field in type.GetFields() where !IsFieldNotSerialized(field) select field.Name).ToList();

            var f_zh = zh.CreateText();
            var f_en = en.CreateText();
            foreach (var field in fields)
            {
                var n_zh = dict_zh.TryGetValue(field, out var v1) ? v1 : "";
                var n_en = dict_en.TryGetValue(field, out var v2) ? v2 : "";
                f_zh.WriteLine($"{field}\t{n_zh}");
                f_en.WriteLine($"{field}\t{n_en}");
            }

            f_zh.Flush();
            f_zh.Close();
            f_en.Flush();
            f_en.Close();
        }
    }

    /// <summary>
    /// 加载注释
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>注释字典</returns>
    private Dictionary<string, string> LoadNotes(string path)
    {
        var dict = new Dictionary<string, string>();
        if (!File.Exists(path)) return dict;

        var lines = File.ReadAllLines(path, Encoding.UTF8);
        foreach (var line in lines)
        {
            if (line == "") continue;

            var data = line.Split(new char[] { '\t' }, 2);
            dict[data[0]] = data.Length == 1 ? "" : data[1];
        }

        return dict;
    }

    /// <summary>
    /// 创建模板
    /// </summary>
    private void CreateTemplate()
    {
        var dir_tmp = Directory.CreateDirectory(Path.Combine(_output, "ScriptableObjectJsonDataWithWarpLitAllInOne"));
        var dir_name = Directory.CreateDirectory(Path.Combine(_output, "ScriptableObjectObjectName"));
        var dir_base = Directory.CreateDirectory(Path.Combine(_output, "UniqueIDScriptableBaseJsonData"));

        foreach (var (type, name) in _types)
        {
            if (!type.IsSubclassOf(typeof(ScriptableObject))) continue;

            Debug.Log($"Template: {type.Name}");

            var data = TypeToJsonData(type, name);
            var dir = Directory.CreateDirectory(Path.Combine(dir_tmp.FullName, name));

            var json = data.ToJson();
            OutputJson(dir.FullName, "", json);
            OutputJson(dir.FullName, "BaseTemplate(模板)", json);
            OutputJson(dir_base.FullName, name, json);
            OutputTxt(dir_name.FullName, name, "BaseTemplate(模板)");
        }
    }

    /// <summary>
    /// 创建 BaseJson (UniqueIDScriptableBaseJsonData)
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="base_name">顶层类型名称</param>
    private void CreateBaseJson(Type type, string base_name)
    {
        if (type.IsEnum) return;
        if (type.IsArray) type = type.GetElementType();

        if (type is null) return;
        if (!_types.ContainsKey(type)) return;
        if (type.IsSubclassOf(typeof(UnityEngine.Object))) return;

        Debug.Log($"-- BaseJson: {type.Name}");
        var data = TypeToJsonData(type, base_name);
        var dir = Directory.CreateDirectory(Path.Combine(_output, "UniqueIDScriptableBaseJsonData", base_name));
        OutputJson(dir.FullName, _types[type], data.ToJson());
    }

    /// <summary>
    /// 类型转 JsonData
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="base_name">顶层类型名称</param>
    /// <returns>JsonData</returns>
    private JsonData TypeToJsonData(Type type, string base_name)
    {
        var data = new JsonData();

        var no_field = true;
        foreach (var field in type.GetFields())
        {
            if (IsFieldNotSerialized(field)) continue;

            no_field = false;
            CreateBaseJson(field.FieldType, base_name);
            data[field.Name] = FieldToJsonData(field, base_name);
        }

        if (no_field) data.SetJsonType(JsonType.Object);
        return data;
    }

    /// <summary>
    /// 字段转 JsonData
    /// </summary>
    /// <param name="field">字段信息</param>
    /// <param name="base_name">顶层类型名称</param>
    /// <returns>JsonData</returns>
    private JsonData FieldToJsonData(FieldInfo field, string base_name)
    {
        JsonData data;
        var type = field.FieldType;

        if (type.IsPrimitive)
        {
            if (type == typeof(bool)) data = new JsonData(false);
            else if (type == typeof(char)) data = new JsonData("");
            else if (type == typeof(float) || type == typeof(double)) data = new JsonData(0.0);
            else data = new JsonData(0);
        }
        else if (type == typeof(string))
        {
            data = new JsonData("");
        }
        else if (type.IsEnum)
        {
            data = new JsonData(0);
        }
        else if (type.IsArray || ResolveGenericType(type) == typeof(List<>))
        {
            data = new JsonData();
            data.SetJsonType(JsonType.Array);
        }
        else
        {
            data = ClassFieldToJsonData(type, base_name);
        }

        return data;
    }

    /// <summary>
    /// 类类型转 JsonData
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="base_name">顶层类型名称</param>
    /// <returns>JsonData</returns>
    private JsonData ClassFieldToJsonData(Type type, string base_name)
    {
        if (!type.IsSubclassOf(typeof(UnityEngine.Object))) return TypeToJsonData(type, base_name);

        var data = new JsonData
        {
            ["m_FileID"] = 0,
            ["m_PathID"] = 0
        };
        return data;
    }

    /// <summary>
    /// 输出 Json 格式文件
    /// </summary>
    /// <param name="path">输出路径</param>
    /// <param name="name">文件名称</param>
    /// <param name="json">Json字符串</param>
    private static void OutputJson(string path, string name, string json)
    {
        try
        {
            json = Regex.Unescape(json);
        }
        catch (ArgumentException)
        {
        }

        var file = File.CreateText(Path.Combine(path, $"{name}.json"));
        file.Write(json);
        file.Flush();
        file.Close();
    }

    /// <summary>
    /// 输出 txt 格式文件
    /// </summary>
    /// <param name="path">输出路径</param>
    /// <param name="name">文件名称</param>
    /// <param name="text">文件内容</param>
    private static void OutputTxt(string path, string name, string text)
    {
        var file = File.CreateText(Path.Combine(path, $"{name}.txt"));
        file.Write(text);
        file.Flush();
        file.Close();
    }

    /// <summary>
    /// 解析类型 <br/>
    /// 若参数 type 不满足以下任一情况，则直接返回该参数 <br/>
    /// 为整数类型时，返回 int 类型 <br/>
    /// 为字符类型时，返回 string 类型 <br/>
    /// 为数组类型时，返回数组元素的类型 <br/>
    /// 为列表类型时，返回列表元素的类型
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>解析后的类型</returns>
    private static Type ResolveType(Type type)
    {
        var t = type;

        // 解析数组类型
        if (type.IsArray) t = type.GetElementType();
        if (t is null) return type;

        // 解析列表类型
        if (ResolveGenericType(type) == typeof(List<>)) t = type.GetGenericArguments()[0];

        // 解析基元类型
        if (!t.IsPrimitive) return t;
        if (t == typeof(char)) return typeof(string);
        if (t != typeof(bool) && t != typeof(float) && t != typeof(double)) return typeof(int);

        return t;
    }


    /// <summary>
    /// 解析泛型类型
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>解析后的类型</returns>
    private static Type ResolveGenericType(Type type)
    {
        return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
    }

    /// <summary>
    /// 字段是否不可序列化 <br/>
    /// 不可序列化的字段：字典类型，或 static | const | readonly 修饰的
    /// </summary>
    /// <param name="field">字段</param>
    /// <returns>是否不可序列化</returns>
    private static bool IsFieldNotSerialized(FieldInfo field)
    {
        if (field.IsStatic) return true;
        if (field.IsLiteral) return true;
        if (field.IsInitOnly) return true;
        if (ResolveGenericType(field.FieldType) == typeof(Dictionary<,>)) return true;

        return field.GetCustomAttribute<NonSerializedAttribute>() is not null;
    }
}