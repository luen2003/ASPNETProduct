using System.Reflection;

namespace IdentityServer.Extensions;

public static class DictionaryExtensions
{
    public static T ToObject<T>(this IDictionary<string, object?> source)
        where T : class, new()
    {
        var someObject = new T();
        var someObjectType = someObject.GetType();

        foreach (var item in source)
        {
            var property = someObjectType
                .GetProperty(item.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            
            if (property is null) continue;
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                property.SetValue(someObject,
                    item.Value is null || string.IsNullOrEmpty(item.Value.ToString())
                        ? null
                        : Convert.ChangeType(ConvertObjectToDataType(property.PropertyType.GetGenericArguments()[0], item.Value),
                            property.PropertyType.GetGenericArguments()[0]), null);
            }
            else
            {
                property.SetValue(someObject, ConvertObjectToDataType(property.PropertyType, item.Value!), null);
            }
        }
        return someObject;
    }

    private static object ConvertObjectToDataType(Type type, object value)
    {
        if (type == typeof(DateTime))
        {
            return DateTimeParse.ConvertLongToDateTime((long)value);
        }

        if (type == typeof(DateOnly))
        {
            return DateTimeParse.ConvertLongToDate((long)value);
        }

        return type == typeof(bool) ? Convert.ToBoolean(value) : value;
    }
}