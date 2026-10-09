namespace Domain;

using System;
using System.ComponentModel;
public static class EnumExtensions
{
    // public static string GetDescription(this Enum value)
    // {
    //     var fieldInfo = value.GetType().GetField(value.ToString());

    //     if (fieldInfo == null)
    //         return value.ToString();

    //     var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

    //     return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    // }
    public static Guid GetId(this Enum value)
    {
        var description = value.ToString();
        var fieldInfo = value.GetType().GetField(value.ToString());
        if (fieldInfo != null)
        {
            var attributes = (IdAttribute[])fieldInfo.GetCustomAttributes(typeof(IdAttribute), false);
            if (attributes.Length > 0)
            {
                description = attributes[0].Description;
            }
        }
        if (Guid.TryParse(description, out var result))
        {
            return result;
        }
        else
        {
            return Guid.Empty;
        }
    }
    // public static string GetDetail(this Enum value)
    // {
    //     var fieldInfo = value.GetType().GetField(value.ToString());

    //     if (fieldInfo != null)
    //     {
    //         var attributes = (DetailAttribute[])fieldInfo.GetCustomAttributes(typeof(DetailAttribute), false);

    //         if (attributes != null)
    //         {
    //             return attributes[0].Description;
    //         }
    //     }

    //     return value.ToString();
    // }
    public static IEnumerable<T> GetEnumValues<T>() where T : Enum
    {
        var enumType = typeof(T);
        if (!enumType.IsEnum)
            throw new InvalidElementException($"{nameof(T)} debe ser de tipo Enum.");

        return (IEnumerable<T>)Enum.GetValues(enumType);
    }
    public static T GetEnumValueFromId<T>(Guid id) where T : Enum
    {
        var enumType = typeof(T);
        if (!enumType.IsEnum)
            throw new InvalidElementException($"{nameof(T)} debe ser de tipo Enum.");

        foreach (T enumValue in Enum.GetValues(enumType))
        {
            var enumDescription = GetId(enumValue);
            if (enumDescription == id)
                return enumValue;
        }

        throw new NotFoundException($"No se encontró un valor Enum para '{id}'.");
    }
}
