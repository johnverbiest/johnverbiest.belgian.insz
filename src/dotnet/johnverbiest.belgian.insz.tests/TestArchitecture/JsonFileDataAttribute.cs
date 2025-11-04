#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Xunit.Sdk;

namespace johnverbiest.belgian.insz.tests.TestArchitecture
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class JsonFileDataAttribute : DataAttribute
    {
        private static readonly ConcurrentDictionary<string, (string[] TestCases, string? Name, string? Description)> Cache = new();

        public JsonFileDataAttribute(string relativePath, string? jsonPath = null)
        {
            RelativePath = relativePath;
            JsonPath = jsonPath ?? "testCases";
            JsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public string RelativePath { get; }
        public string JsonPath { get; }
        public JsonSerializerOptions JsonOptions { get; set; }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod is null) throw new ArgumentNullException(nameof(testMethod));

            var fullPath = Path.Combine(AppContext.BaseDirectory, RelativePath);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException(
                    $"JSON test data file not found: {fullPath}. " +
                    "Set the file to Copy to Output Directory.");

            var key = fullPath + "::" + JsonPath;
            var (jsonStrings, testSuiteName, testSuiteDescription) = Cache.GetOrAdd(key, _ =>
            {
                using var stream = File.OpenRead(fullPath);
                using var doc = JsonDocument.Parse(stream);
                JsonElement root = doc.RootElement;
                
                // Extract name and description from root level
                string? name = root.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : null;
                string? description = root.TryGetProperty("description", out var descEl) ? descEl.GetString() : null;

                JsonElement node = root;
                if (!string.IsNullOrEmpty(JsonPath))
                {
                    foreach (var part in JsonPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!node.TryGetProperty(part, out node))
                            throw new InvalidOperationException($"Path '{JsonPath}' not found in {fullPath}.");
                    }
                }

                if (node.ValueKind != JsonValueKind.Array)
                    throw new InvalidOperationException(
                        $"JSON at {JsonPath} in {fullPath} must be an array.");

                var testCases = node.EnumerateArray().Select(el => el.GetRawText()).ToArray();
                return (testCases, name, description);
            });

            var parameters = testMethod.GetParameters();
            bool singleParam = parameters.Length == 1;

            foreach (var jsonString in jsonStrings)
            {
                using var doc = JsonDocument.Parse(jsonString);
                var el = doc.RootElement;
                
                if (singleParam)
                {
                    var targetType = parameters[0].ParameterType;
                    var deserializedObject = DeserializeToType(jsonString, targetType, JsonOptions);
                    
                    // If it's a TestCase, inject the test suite name and description
                    if (deserializedObject is TestCase testCase)
                    {
                        var enhancedTestCase = testCase with 
                        { 
                            Name = testCase.Name ?? testSuiteName,
                            Description = testCase.Description ?? testSuiteDescription
                        };
                        yield return new object[] { enhancedTestCase };
                    }
                    else
                    {
                        yield return new object[] { deserializedObject! };
                    }
                }
                else
                {
                    if (el.ValueKind != JsonValueKind.Object)
                        throw new InvalidOperationException(
                            $"Each array element must be an object to map to multiple parameters ({testMethod.Name}).");

                    var row = new object?[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var p = parameters[i];
                        if (!el.TryGetProperty(p.Name!, out var prop))
                            throw new InvalidOperationException(
                                $"Parameter '{p.Name}' not found in test vector element for {testMethod.Name}.");

                        var deserializedParam = DeserializeToType(prop.GetRawText(), p.ParameterType, JsonOptions);
                        
                        // If it's a TestCase parameter, inject the test suite name and description
                        if (deserializedParam is TestCase testCase)
                        {
                            row[i] = testCase with 
                            { 
                                Name = testCase.Name ?? testSuiteName,
                                Description = testCase.Description ?? testSuiteDescription
                            };
                        }
                        else
                        {
                            row[i] = deserializedParam;
                        }
                    }
                    yield return row!;
                }
            }
        }

        private static object? DeserializeToType(string jsonString, Type type, JsonSerializerOptions options)
        {
            try
            {
                return JsonSerializer.Deserialize(jsonString, type, options);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize JSON value '{jsonString}' to type '{type.Name}': {ex.Message}");
            }
        }
    }
}
