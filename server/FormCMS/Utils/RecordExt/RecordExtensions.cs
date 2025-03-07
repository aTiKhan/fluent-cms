using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentResults;
using Humanizer;

namespace FormCMS.Utils.RecordExt;

public static class RecordExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public static TValue CamelKeyObject<TValue>(this Record r, string fieldName)
    {
        var s = (string)r[fieldName.Camelize()];
        return JsonSerializer.Deserialize<TValue>(s, JsonSerializerOptions)!;
    }

    public static int CamelKeyInt(this Record r, string field)
    {
        var v = r[field.Camelize()];
        return v switch
        {
            int val => val,
            long val => (int)val,
            _ => 0
        };
    }

    public static string CamelKeyStr(this Record r, string field)
        => (string)r[field.Camelize()];

    public static bool CamelKeyDateTime(
        this Record record,
        Enum field,
        out DateTime e)
    {
        e = default;
        if (!record.TryGetValue(field.ToString().Camelize(), out var o) || o is not string s)
        {
            return false;
        }

        return DateTime.TryParse(s, out e);
    }

    public static bool CamelKeyEnum<TEnum>(
        this Record record,
        Enum field,
        out TEnum e)
        where TEnum : struct
    {
        e = default;
        if (!record.TryGetValue(field.ToString().Camelize(), out var o) || o is not string s)
        {
            return false;
        }

        return Enum.TryParse(s, true, out e);
    }

    public static bool CamelKeyEnum<TEnum>(
        this Record record,
        string field,
        out TEnum e)
        where TEnum : struct
    {
        e = default;
        if (!record.TryGetValue(field.Camelize(), out var o) || o is not string s)
        {
            return false;
        }

        return Enum.TryParse(s, true, out e);
    }

    public static bool RemoveCamelKey(this Record r, Enum field, out object? val)
        => r.Remove(field.ToString().Camelize(), out val);
    
    public static void AddCamelKeyCamelValue(this Record record, Enum field, Enum value)
    {
        record.Add(field.ToString().Camelize(), value.ToString().Camelize());
    }

    public static void AddCamelKey(this Record record, Enum field, object value)
    {
        record.Add(field.ToString().Camelize(), value);
    }

    public static Result<T> ToObject<T>(this Record r) 
    {
        var constructor = typeof(T).GetConstructors().FirstOrDefault();
        if (constructor == null)
            return Result.Fail($"Type {typeof(T).Name} does not have a suitable constructor.");

        var parameters = constructor.GetParameters();
        var args = new object?[parameters.Length];
        foreach (var parameter in parameters)
        {
            var propertyName = parameter.Name?.Camelize();
            if (propertyName == null || !r.TryGetValue(propertyName, out var value)) continue;
            args[parameter.Position] = value switch
            {
                string enumStr when parameter.ParameterType.IsEnum =>
                    Enum.Parse(parameter.ParameterType, enumStr, ignoreCase: true),

                string jsonStr when typeof(Record).IsAssignableFrom(parameter.ParameterType) ||
                                    parameter.ParameterType.IsClass &&
                                    parameter.ParameterType != typeof(string) =>
                    JsonSerializer.Deserialize(jsonStr, parameter.ParameterType, JsonSerializerOptions),

                null when Nullable.GetUnderlyingType(parameter.ParameterType) != null => null,
                _ => Convert.ChangeType(value, parameter.ParameterType)
            };
        }
        return (T)constructor.Invoke(args);
    }
    
    public static Record FormObject(object input, HashSet<string>? whiteList = null, HashSet<string>? blackList = null)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));
        var dict = new Dictionary<string, object>();
        
        foreach (var property in input.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (whiteList != null && !whiteList.Contains(property.Name)) continue;
            if (blackList != null && blackList.Contains(property.Name)) continue;
            var value = property.GetValue(input);
            dict[property.Name.Camelize()] = value switch
            {
                null => null!,
                Enum valueEnum => valueEnum.ToString().Camelize(),
                _ when typeof(Record).IsAssignableFrom(property.PropertyType) || 
                       property.PropertyType.IsClass && property.PropertyType != typeof(string)
                    => JsonSerializer.Serialize(value, JsonSerializerOptions),
                
                _ => value
            };
        }
        return dict;
    }

    public static bool ByJsonPath<T>(this Record dictionary, string key, out T? val)
    {
        val = default;
        var parts = key.Split('.');
        object current = dictionary;

        foreach (var part in parts)
        {
            if (current is Record dict && dict.TryGetValue(part, out var tmp))
            {
                current = tmp;
            }
            else
            {
                return false;
            }
        }

        if (current is T t)
        {
            val = t;
        }

        return true;
    }

    public static Record[] ToTree(this Record[] records, string idField, string parentField)
    {
        var parentIdField = parentField;
        var lookup = records.ToDictionary(r => r[idField]);
        var roots = new List<IDictionary<string, object>>();

        foreach (var record in records)
        {
            if (record.TryGetValue(parentIdField, out var parentId) && parentId != null &&
                lookup.TryGetValue(parentId, out var parentRecord))
            {
                if (!parentRecord.ContainsKey("children"))
                {
                    parentRecord["children"] = new List<Record>();
                }

                ((List<Record>)parentRecord["children"]).Add(record);
            }
            else
            {
                roots.Add(record);
            }
        }

        return roots.ToArray();
    }

}