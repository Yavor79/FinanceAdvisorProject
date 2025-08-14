using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace FinanceAdvisor.Common.Logging
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs all public instance properties of a single object.
        /// </summary>
        ///
        public static void LogObjectProperties<T>(this ILogger logger, T? obj, string context = "")
        {
            if (obj == null)
            {
                logger.LogInformation("{Context}Object is null", context);
                return;
            }

            var type = obj.GetType();
            logger.LogInformation("{Context}Logging object of type {TypeName}", context, type.Name);

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = prop.GetValue(obj) ?? "null";
                logger.LogInformation("{Context}{PropertyName}: {PropertyValue}", context, prop.Name, value);
            }

            logger.LogInformation("{Context}--- End of object log ---", context);
        }

        /// <summary>
        /// Logs all public instance properties of each object in a collection.
        /// </summary>
        public static void LogCollectionProperties<T>(this ILogger logger, IEnumerable<T>? collection, string context = "")
        {
            if (collection == null)
            {
                logger.LogInformation("{Context}Collection is null", context);
                return;
            }

            var count = collection.Count();
            logger.LogInformation("{Context}Collection logging start ({Count} items)", context, count);

            int index = 1;
            foreach (var item in collection)
            {
                logger.LogInformation("{Context}--- Item {Index} ---", context, index);
                logger.LogObjectProperties(item, context);
                index++;
            }

            logger.LogInformation("{Context}--- End of collection log ---", context);
        }

        /// <summary>
        /// Logs a DTO as JSON (compact, good for structured logging systems).
        /// </summary>
        public static void LogAsJson<T>(this ILogger logger, T? obj, string context = "")
        {
            if (obj == null)
            {
                logger.LogInformation("{Context}Object is null", context);
                return;
            }

            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
            logger.LogInformation("{Context}JSON: {Json}", context, json);
        }

        /// <summary>
        /// Logs a collection as JSON.
        /// </summary>
        public static void LogCollectionAsJson<T>(this ILogger logger, IEnumerable<T>? collection, string context = "")
        {
            if (collection == null)
            {
                logger.LogInformation("{Context}Collection is null", context);
                return;
            }

            var json = JsonSerializer.Serialize(collection, new JsonSerializerOptions { WriteIndented = false });
            logger.LogInformation("{Context}JSON: {Json}", context, json);
        }
    }
}
