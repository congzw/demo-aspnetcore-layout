using System.Text.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace Common
{
    public class JsonHelper
    {
        public static JsonHelper Instance => new JsonHelper();

        public string ToJson(object model, bool indented = false)
        {
            if (model == null)
            {
                return null;
            }

            if (model is string)
            {
                return model.ToString();
            }

            var jsonOptions = CreateJsonOptions(indented);
            return JsonSerializer.Serialize(model, jsonOptions);
        }

        public T FromJson<T>(string json, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultValue;
            }
            var jsonOptions = CreateJsonOptions(null);
            return JsonSerializer.Deserialize<T>(json, jsonOptions);
        }

        public object FromJson(Type returnType, string json, object defaultValue)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultValue;
            }

            var jsonOptions = CreateJsonOptions(null);
            return JsonSerializer.Deserialize(json, returnType, jsonOptions);
        }

        public Task SaveJsonFileAsync(string filePath, string json, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "文件路径不能为空");
            }
            var dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            return File.WriteAllTextAsync(filePath, json, Encoding.UTF8, cancellationToken);
        }

        public Task<string> ReadJsonFileAsync(string filePath, string defaultValue = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "文件路径不能为空");
            }
            if (!File.Exists(filePath))
            {
                return Task.FromResult(defaultValue);
            }
            return File.ReadAllTextAsync(filePath, cancellationToken);
        }

        public void SaveJsonFile(string filePath, string json)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "文件路径不能为空");
            }
            var dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        public string ReadJsonFile(string filePath, string defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "文件路径不能为空");
            }
            if (!File.Exists(filePath))
            {
                return defaultValue;
            }
            return File.ReadAllText(filePath);
        }

        public JsonSerializerOptions CreateJsonOptions(bool? indented)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                //中文编程Unicode的问题
                //Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                //https://github.com/dotnet/runtime/issues/50998
                //https://learn.microsoft.com/zh-cn/dotnet/standard/serialization/system-text-json/character-encoding
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            if (indented != null)
            {
                jsonOptions.WriteIndented = indented.Value;
            }

            jsonOptions.Converters.Add(new JsonStringEnumConverter());
            jsonOptions.Converters.Add(new DateTimeConverterUsingDateTimeParse());

            return jsonOptions;
        }

        public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.Parse(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
        }
    }

    public class JsonFileHelper
    {
        public static JsonFileHelper Instance = new JsonFileHelper();

        /// <summary>
        /// 保存Json文件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task SaveJsonFileAsync(object model, string filePath, bool indented = false, CancellationToken cancellationToken = default)
        {
            var json = model.ToJson(indented);
            await JsonHelper.Instance.SaveJsonFileAsync(filePath, json, cancellationToken);
        }

        /// <summary>
        /// 读取Json文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<T> ReadJsonFileAsync<T>(string filePath, T defaultValue, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "文件路径不能为空");
            }
            var json = await JsonHelper.Instance.ReadJsonFileAsync(filePath, null, cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultValue;
            }
            return json.FromJson(defaultValue);
        }

        /// <summary>
        /// 保存Json文件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public void SaveJsonFile(object model, string filePath, bool indented = false)
        {
            var json = model.ToJson(indented);
            JsonHelper.Instance.SaveJsonFile(filePath, json);
        }

        /// <summary>
        /// 读取Json文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public T ReadJsonFile<T>(string filePath, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "文件路径不能为空");
            }
            var json = JsonHelper.Instance.ReadJsonFile(filePath, null);
            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultValue;
            }
            return json.FromJson(defaultValue);
        }
    }

    public static class JsonExtensions
    {
        /// <summary>
        /// object as json
        /// </summary>
        /// <param name="model"></param>
        /// <param name="indented"></param>
        /// <returns></returns>
        public static string ToJson(this object model, bool indented = false)
        {
            return JsonHelper.Instance.ToJson(model, indented);
        }

        /// <summary>
        /// json to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json, T defaultValue)
        {
            return JsonHelper.Instance.FromJson(json, defaultValue);
        }

        public static object FromJson(this string json, Type theType, object defaultValue)
        {
            return JsonHelper.Instance.FromJson(theType, json, defaultValue);
        }

        public static T JsonConvertToObject<T>(this IDictionary<string, object> dict, T defaultValue = default)
        {
            if (dict == null)
            {
                return defaultValue;
            }
            return dict.ToJson().FromJson(defaultValue);
        }

        public static IDictionary<string, object> JsonConvertToDictionary<T>(this T model, IDictionary<string, object> defaultValue = default)
        {
            if (model == null)
            {
                return defaultValue;
            }
            return model.ToJson().FromJson(defaultValue);
        }

        /// <summary>
        /// 文件操作相关
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static JsonFileHelper ForJsonFile(this JsonHelper instance)
        {
            return JsonFileHelper.Instance;
        }
    }

    public class JsonFileData
    {
        public string FilePath { get; set; }
        public T LoadFromFile<T>(T defaultValue)
        {
            if (!File.Exists(FilePath))
            {
                return defaultValue;
            }
            var json = ReadFile(FilePath);
            if (string.IsNullOrEmpty(json))
            {
                return defaultValue;
            }
            var loadOne = json.FromJson(defaultValue);
            return loadOne;
        }
        public void SaveToFile<T>(T theValue)
        {
            var json = theValue.ToJson(true);
            SaveFile(FilePath, json);
        }

        public static JsonFileData Create(string filePath)
        {
            return new JsonFileData() { FilePath = filePath };
        }
        private static string ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            return File.ReadAllText(filePath);
        }
        private static void SaveFile(string filePath, string content)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }
    }
}