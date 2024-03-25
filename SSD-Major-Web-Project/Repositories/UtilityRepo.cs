using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SSD_Major_Web_Project.Repositories
{
    public class UtilityRepo
    {
        //public T FilterHarmfulInput<T>(T value)
        //{
        //    // Convert value to json string
        //    string stringValue = JsonSerializer.Serialize(value); ;

        //    // Perform HTML tag filtering
        //    string filteredString = FilterHtmlTags(stringValue);

        //    //Perform SQL statement filtering
        //    filteredString = FilterSqlStatements(filteredString);

        //    // Convert back to original type
        //    T result = JsonSerializer.Deserialize<T>(filteredString);

        //    return result;
        //}

        public T FilterHarmfulInput<T>(T value)
        {
            if (value == null)
                return value;

            Type valueType = value.GetType();

            if (IsSimpleType(valueType))
            {
                // For simple types, convert to string, filter, and then convert back to the original type
                string stringValue = value.ToString();
                string filteredString = FilterHtmlTags(stringValue);
                filteredString = FilterSqlStatements(filteredString);
                return (T)Convert.ChangeType(filteredString, valueType);
            }
            else if (valueType.IsInterface)
            {
                // For interface types, return value as it is
                return value;
            }
            else
            {
                // For complex types, recursively filter each property
                PropertyInfo[] properties = valueType.GetProperties();
                foreach (var property in properties)
                {
                    // Get the value of the property
                    object propertyValue = property.GetValue(value);

                    // Recursively call FilterHarmfulInput for complex properties
                    if (propertyValue != null)
                    {
                        // Filter the property value recursively
                        object filteredPropertyValue = FilterHarmfulInput(propertyValue);

                        // Set the filtered property value back to the result object
                        property.SetValue(value, filteredPropertyValue);
                    }
                }

                return value;
            }
        }

        static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);
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
