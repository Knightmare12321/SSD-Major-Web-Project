using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SSD_Major_Web_Project.Repositories
{
    public class UtilityRepo
    {
        //public T FilterHarmfulInput<T>(T value)
        //{
        //    if (value == null)
        //        return value;

        //    Type valueType = value.GetType();

        //    if (IsSimpleType(valueType))
        //    {
        //        // For simple types, convert to string, filter, and then convert back to the original type
        //        string stringValue = value.ToString();
        //        string filteredString = FilterHtmlTags(stringValue);
        //        filteredString = FilterSqlStatements(filteredString);
        //        return (T)Convert.ChangeType(filteredString, valueType);
        //    }
        //    else if (valueType.IsInterface)
        //    {
        //        // For interface types, return value as it is
        //        return value;
        //    }
        //    else
        //    {
        //        // For complex types, recursively filter each property
        //        PropertyInfo[] properties = valueType.GetProperties();
        //        foreach (var property in properties)
        //        {
        //            // Check if the property is a collection property
        //            if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
        //                property.PropertyType != typeof(string))
        //            {
        //                // Skip filtering collection properties
        //                continue;
        //            }

        //            // Get the value of the property
        //            object propertyValue = property.GetValue(value);

        //            // Recursively call FilterHarmfulInput for complex properties
        //            if (propertyValue != null)
        //            {
        //                // Filter the property value recursively
        //                object filteredPropertyValue = FilterHarmfulInput(propertyValue);

        //                // Set the filtered property value back to the result object
        //                property.SetValue(value, filteredPropertyValue);
        //            }
        //        }

        //        return value;
        //    }
        //}

        public T FilterHarmfulInput<T>(T obj)
        {
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            if (obj == null || objType == typeof(IFormFile))
                return obj;



            foreach (var property in properties)
            {
                // Check if the property is indexed before trying to get its value
                if (!property.GetIndexParameters().Any())
                {
                    Type propertyType = property.PropertyType;
                    object propertyValue = property.GetValue(obj);

                    if (propertyValue != null)
                    {
                        string stringValue = propertyValue.ToString();
                        string filteredString = FilterHtmlTags(stringValue);
                        filteredString = FilterSqlStatements(filteredString);

                        // Convert filtered string back to original type
                        object convertedValue = Convert.ChangeType(filteredString, propertyType);
                        property.SetValue(obj, convertedValue);

                        if (IsListType(property.PropertyType))
                        {
                        }
                    }
                }
            }

            return obj;
        }

        static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);
        }

        static bool IsListType(Type type)
        {
            // Check if the type is a generic List<T>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return true;
            }

            // Check if the type implements IEnumerable<T> (includes arrays)
            return typeof(IEnumerable<>).IsAssignableFrom(type);
        }

        public string FilterHtmlTags(string input)
        {
            // Use regular expressions to remove HTML tags
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        public string FilterSqlStatements(string input)
        {
            // Regular expression pattern to match common SQL keywords and syntax
            string pattern = @"\b(SELECT|UPDATE|DELETE|INSERT INTO|ALTER|CREATE|DROP|TRUNCATE|TABLE|FROM|WHERE|JOIN|AND|OR)\b";

            // Do a case-insensitive match and replace potential SQL statements with an empty string
            return Regex.Replace(input, pattern, string.Empty, RegexOptions.IgnoreCase);
        }
    }
}
