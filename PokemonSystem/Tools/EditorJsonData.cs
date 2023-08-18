using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using LitJson;
using ModLoader;
using PokemonSystem.Core;
using PokemonSystem.Core.Experiences;
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

    private readonly Dictionary<Type, string> _types = new();

    private readonly string _output;
    private readonly string _prefix;

    public EditorJsonData(string output, string prefix = "")
    {
        _output = Path.Combine(output, "CSTI-JsonData");
        _prefix = prefix;
    }

    public void AddType(Type type)
    {
        AddType(type, type.Name);
    }

    public void AddType(Type type, string name)
    {
        _types[type] = _prefix + name;
    }

    public void AddType<T>()
    {
        AddType(typeof(T));
    }

    public void AddType<T>(string name)
    {
        AddType(typeof(T), name);
    }

    public void CreateJsonData()
    {
        Debug.Log("----- Start Create JsonData -----");

        CreateTypeJsonData();
        CreateNotes();
        CreateTemplate();

        Debug.Log("----- End Create JsonData -----");
    }

    private void CreateTypeJsonData()
    {
        var dir = Directory.CreateDirectory(Path.Combine(_output, "ScriptableObjectTypeJsonData"));

        foreach (var type in _types)
        {
            if (type.Key.IsEnum)
            {
                CreateTypeJsonDataEnum(type.Key, type.Value);
                continue;
            }

            Debug.Log($"Type: {type.Key.Name}");
            var fields = new Dictionary<string, string>();
            foreach (var field in type.Key.GetFields())
            {
                var field_type = ResolveType(field.FieldType);
                fields[field.Name] = _types.TryGetValue(field_type, out var name) ? name : field_type.Name;
            }

            OutputJson(dir.FullName, type.Value, JsonMapper.ToJson(fields));
        }
    }

    private void CreateTypeJsonDataEnum(Type type, string name)
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

    private void CreateNotes()
    {
        var dir_zh = Directory.CreateDirectory(Path.Combine(_output, "Notes"));
        var dir_en = Directory.CreateDirectory(Path.Combine(_output, "Notes-En"));

        foreach (var (type, name) in _types)
        {
            if (type.IsEnum) continue;

            Debug.Log($"Notes: {type.Name}");

            var fields = type.GetFields().Select(field => $"{field.Name}\t").ToList();
            var f_zh = File.CreateText(Path.Combine(dir_zh.FullName, $"{name}.txt"));
            var f_en = File.CreateText(Path.Combine(dir_en.FullName, $"{name}.txt"));
            foreach (var field in fields)
            {
                f_zh.WriteLine($"{field}\t");
                f_en.WriteLine($"{field}\t");
            }

            f_zh.Flush();
            f_zh.Close();
            f_en.Flush();
            f_en.Close();
        }
    }

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

    private void CreateBaseJson(Type type, string base_name)
    {
        if (type.IsArray) type = type.GetElementType();
        
        if (type is null) return;
        if (!_types.ContainsKey(type)) return;
        if (type.IsSubclassOf(typeof(UnityEngine.Object))) return;

        Debug.Log($"-- BaseJson: {type.Name}");
        var data = TypeToJsonData(type, base_name);
        var dir = Directory.CreateDirectory(Path.Combine(_output, "UniqueIDScriptableBaseJsonData", base_name));
        OutputJson(dir.FullName, _types[type], data.ToJson());
    }

    private JsonData TypeToJsonData(Type type, string base_name)
    {
        var data = new JsonData();

        var no_field = true;
        foreach (var field in type.GetFields())
        {
            if (field.IsStatic || field.IsLiteral) continue;

            no_field = false;
            CreateBaseJson(field.FieldType, base_name);
            data[field.Name] = FieldToJsonData(field, base_name);
        }

        if (no_field) data.SetJsonType(JsonType.Object);
        return data;
    }

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
        else if (type.IsArray)
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

    private static void OutputTxt(string path, string name, string text)
    {
        var file = File.CreateText(Path.Combine(path, $"{name}.txt"));
        file.Write(text);
        file.Flush();
        file.Close();
    }

    private static Type ResolveType(Type type)
    {
        var t = type;
        if (type.IsArray) t = type.GetElementType();

        if (type == typeof(byte)) t = typeof(int);
        else if (type == typeof(short)) t = typeof(int);

        return t ?? type;
    }
}