using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Prism.Validation;

namespace FunkyGrep.UI.Services
{
    class AppSettingsService : IAppSettingsService
    {
        public static readonly string SettingsRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            nameof(FunkyGrep));

        static readonly JsonSerializerOptions s_DefaultJsonSerializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonValueConverterValidatableBindable() }
        };

        public void Save<TSettings>(TSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (!Directory.Exists(SettingsRoot))
            {
                Directory.CreateDirectory(SettingsRoot);
            }

            var settingsFilePath = GetSettingsFilePath(typeof(TSettings));
            using var stream = File.Open(settingsFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var jsonWriter = new Utf8JsonWriter(
                stream,
                new JsonWriterOptions { Indented = true, Encoder = s_DefaultJsonSerializerOptions.Encoder });
            JsonSerializer.Serialize(
                jsonWriter,
                settings,
                typeof(TSettings),
                s_DefaultJsonSerializerOptions);
        }

        public TSettings LoadOrCreate<TSettings>() where TSettings : new()
        {
            var settingsFilePath = GetSettingsFilePath(typeof(TSettings));

            if (!File.Exists(settingsFilePath))
            {
                var settings = new TSettings();
                this.Save(settings);
                return settings;
            }

            var settingsFileInfo = new FileInfo(settingsFilePath);
            if (settingsFileInfo.Length > 1 * 1024 * 1024)
            {
                throw new InvalidOperationException("Refusing to load settings files larger than 1 MB in size.");
            }

            try
            {
                var fileBytes = File.ReadAllBytes(settingsFilePath);
                return JsonSerializer.Deserialize<TSettings>(fileBytes);
            }
            catch
            {
                var settings = new TSettings();
                this.Save(settings);
                return settings;
            }
        }

        static string GetSettingsFilePath(MemberInfo type)
        {
            var name = type.Name;
            name = name[..1].ToLower() + name[1..];

            if (name.EndsWith("ViewModel"))
            {
                name = name[..^"ViewModel".Length];
            }

            if (name.EndsWith("Model"))
            {
                name = name[..^"Model".Length];
            }

            var fileName = name + ".json";
            return Path.Combine(SettingsRoot, fileName);
        }

        class JsonValueConverterValidatableBindable : JsonConverter<ValidatableBindableBase>
        {
            static readonly JsonSerializerOptions s_DefaultOptions = new JsonSerializerOptions();

            static readonly IReadOnlyList<string> s_FilteredPropertyNames = typeof(ValidatableBindableBase)
                .GetProperties()
                .Select(x => x.Name)
                .ToArray();

            public override bool CanConvert(Type typeToConvert)
            {
                return typeof(ValidatableBindableBase).IsAssignableFrom(typeToConvert);
            }

            public override ValidatableBindableBase Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                return (ValidatableBindableBase)JsonSerializer.Deserialize(ref reader, typeToConvert, options);
            }

            public override void Write(
                Utf8JsonWriter writer,
                ValidatableBindableBase value,
                JsonSerializerOptions options)
            {
                var stream = new MemoryStream();
                using (var tempWriter = new Utf8JsonWriter(stream))
                {
                    JsonSerializer.Serialize(tempWriter, value, value.GetType(), s_DefaultOptions);
                }

                stream.Seek(0, SeekOrigin.Begin);

                var document = JsonDocument.Parse(stream);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    writer.WriteStartObject();
                }
                else
                {
                    return;
                }

                foreach (var property in root.EnumerateObject())
                {
                    var skipProperty = false;

                    foreach (var filteredPropertyName in s_FilteredPropertyNames)
                    {
                        if (property.Name == filteredPropertyName)
                        {
                            skipProperty = true;
                            break;
                        }
                    }

                    if (skipProperty)
                    {
                        continue;
                    }

                    property.WriteTo(writer);
                }

                writer.WriteEndObject();
                writer.Flush();
            }
        }
    }
}
