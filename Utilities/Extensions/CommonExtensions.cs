using System.Globalization;
using System.Reflection;
using Utilities.Attributes;

namespace Utilities.Extensions
{
    public static class CommonExtensions
    {
        
        public static DateTime ParseMatrixDateTime(this string date, string time)
        {
            var dateTimeString = date + time;

            return DateTime.ParseExact(dateTimeString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }
        public static List<string> GetProperties<T>(this T source) where T : class
        {
            var properties = source.GetType().GetProperties();

            var propertyList = new List<string>();
            foreach (var property in properties)
            {
                propertyList.Add(property.Name);
            }

            return propertyList;
        }
        public static List<string> GetCustomPropertiesName<T>(this List<string> itemList)
        {
            var resultList = new List<string>();
            var typeT = typeof(T);
            foreach (var item in itemList)
            {
                var property = typeT.GetProperty(item, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (property != null)
                {
                    var columnNameAttribute = property.GetCustomAttribute<ColumnNameAttribute>();
                    resultList.Add(columnNameAttribute != null ? columnNameAttribute.ColumnName : property.Name);
                }
            }

            return resultList;
        }
        public static List<T> CastTo<T>(this IEnumerable<object> anonymousList)
        {
            var resultList = new List<T>();

            foreach (var item in anonymousList)
            {
                var itemType = typeof(T);
                var properties = item.GetType().GetProperties();

                var resultItem = (T)Activator.CreateInstance(itemType)!;

                foreach (var property in properties)
                {
                    var propertyName = property.Name;
                    var propertyValue = property.GetValue(item);

                    var resultProperty = itemType.GetProperty(propertyName);
                    if (resultProperty != null)
                        resultProperty.SetValue(resultItem, propertyValue);
                }

                resultList.Add(resultItem);
            }

            return resultList;
        }

    }
}
