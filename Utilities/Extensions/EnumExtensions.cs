using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Utilities.Extensions;

public static class EnumExtensions
{

    public static T ToEnum<T>(this string enumString, T defaultEnum)
    {
        T result;
        try
        {
            result = (T)Enum.Parse(typeof(T), enumString, true);
        }
        catch
        {
            result = defaultEnum;
        }
        return result;
    }
    public static List<int> GetEnumIntValues<TEnum>(this TEnum enumValue)
    {
        var enumType = enumValue?.GetType();
        if (enumType != null)
        {
            var enumValues = Enum.GetValues(enumType);
            var enumIntValues = enumValues.Cast<int>().ToList();
            return enumIntValues;
        }
        return new List<int>();
    }
    public static T ParseWithDescriptions<T>(string stringToParse)
            where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum) { throw new ArgumentException("Invalid Enumeration Type"); }

        if (string.IsNullOrWhiteSpace(stringToParse)) return default(T);
        var descriptions = GetEnumDescriptions<T>();
        if (descriptions.ContainsKey(stringToParse)) return StringToEnum<T>(stringToParse);

        if (!descriptions.ContainsValue(stringToParse)) return (T)(object)0;

        var enumString = descriptions.First(d => d.Value == stringToParse).Key;

        return StringToEnum<T>(enumString);
    }

    public static Dictionary<string, string> GetEnumDescriptions<T>()
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum) { throw new ArgumentException("Invalid Enumeration Type"); }
        var descriptions = new Dictionary<string, string>();

        var mis = typeof(T).GetMembers().Where(m => m.DeclaringType == typeof(T) && m.Name != "value__").ToList();
        mis.ForEach(mi =>
        {
            var descriptionAttributes = ((DescriptionAttribute[])mi.GetCustomAttributes(typeof(DescriptionAttribute), false)).ToList();
            if (descriptionAttributes.Count == 0)
            {
                descriptions.Add(mi.Name, mi.Name);
                return;
            }

            descriptions.Add(mi.Name, descriptionAttributes[0].Description);
        });

        return descriptions;
    }

    public static List<T> GetEnumEntries<T>()
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum) { throw new ArgumentException("Invalid Enumeration Type"); }

        return typeof(T).GetMembers().Where(m => m.DeclaringType == typeof(T) && m.Name != "value__").Select(t => StringToEnum<T>(t.Name)).ToList();
    }

    public static List<string?> GetEnumNames<T>()
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum) { throw new ArgumentException("Invalid Enumeration Type"); }

        return typeof(T).GetMembers().Where(m => m.DeclaringType == typeof(T) && m.Name != "value__").Select(v => v.ToString()).ToList();
    }

    public static T StringToEnum<T>(string name)
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum) { throw new ArgumentException("Invalid Enumeration Type"); }

        if (string.IsNullOrWhiteSpace(name)) return default(T);

        return (T)Enum.Parse(typeof(T), name);
    }

    public static string? ToEnumString<T>(this T value)
        where T : struct, IConvertible
    {
        var description = value.ToDescription();
        return !string.IsNullOrWhiteSpace(description) ? description : value.ToString();
    }

    public static string GetDescription(this Enum genericEnum)
    {
        Type genericEnumType = genericEnum.GetType();
        MemberInfo[] memberInfo = genericEnumType.GetMember(genericEnum.ToString());
        if ((memberInfo.Length > 0))
        {
            var attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((attribs.Any()))
            {
                return ((DescriptionAttribute)attribs.ElementAt(0)).Description;
            }
        }
        return genericEnum.ToString();
    }
    public static T GetEnumStartingWith<T>(string enumStartingWith, T defaultValue)
    {
        if (string.IsNullOrWhiteSpace(enumStartingWith))
        {
            return defaultValue;
        }

        var enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToList();
        var matchedEnumIndex = enumValues.FindIndex(enumValue => enumValue.ToString().ToLower().Contains(enumStartingWith.ToLower()));
        return matchedEnumIndex >= 0 ? enumValues[matchedEnumIndex] : defaultValue;
    }
    public static string GetStringFromFlags(this Enum input)
    {
        List<string> descriptions = new();
        foreach (Enum value in Enum.GetValues(input.GetType()))
        {
            if (input.HasFlag(value) &&
            value.ToString() != "Unknown")
                descriptions.Add(value.GetDescription());
        }
        return string.Join(", ", descriptions);
    }

    private static string ToDescription<T>(this T value)
    {
        var field = typeof(T).GetField(value.ToString());
        return field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .Cast<DescriptionAttribute>()
                    .Select(x => x.Description)
                    .FirstOrDefault();
    }
    public static string? GetDisplayName(this Enum enumValue)
    {
        if (enumValue is null)
        {
            throw new ArgumentNullException($"{nameof(enumValue)} : {typeof(Enum)}");
        }

        var displayName = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo is not null)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
            if (attrs is not null && attrs.Length > 0)
            {
                displayName = ((DisplayAttribute)attrs[0]).Name;
            }
        }

        return displayName;
    }
}
