using System.Diagnostics;
using ClientGenerator.Models;
using ClientGenerator.Options;
using ClientGenerator.Templates;
using Newtonsoft.Json;

namespace ClientGenerator;

public static class Generator
{
    private static ClientGeneratorOptions options;

    public static void GenerateProject(ClientGeneratorOptions generatorOptions)
    {
        options = generatorOptions;
        
        var swaggerDocument = JsonConvert.DeserializeObject<SwaggerDocument>(File.ReadAllText(Path.Combine(options.DocumentPath)));
        if (swaggerDocument is null)
            throw new NullReferenceException("Не распарсили документ");

        var cmd = RunConsole();
        cmd.StandardInput.WriteLine($"cd \"{options.OutputDirectory}\"");
        cmd.StandardInput.WriteLine("dotnet new classlib");
        cmd.StandardInput.WriteLine("dotnet add package RestSharp");
        CloseConsole(cmd);

        GenerateApiClient(options.OutputDirectory, swaggerDocument);
        GenerateModels(swaggerDocument);
        
        
    }

    private static void GenerateModels(SwaggerDocument swaggerDocument)
    {
        var modelsPath = Path.Combine(options.OutputDirectory, "Models");
        Directory.CreateDirectory(modelsPath);

        foreach (var (name, schema) in swaggerDocument.Components.Schemas)
        {
            var schemaPath = Path.Combine(modelsPath, $"{name}.cs");

            var modelText = TemplatesStore.Model;
            modelText = modelText.Replace("{modelName}", $"{name}")
                                 .Replace("{properties}", $"{GetPropertiesText(schema.Properties)}");
            
            File.WriteAllText(schemaPath, modelText);
        }
    }

    private static string GetPropertiesText(Dictionary<string, Property?> properties)
    {
        var propertiesText = string.Empty;

        foreach (var (name, property) in properties.Where(x => x.Value is not null))
        {
            var propertyText = TemplatesStore.Property;
            propertyText = propertyText.Replace("{name}", $"{name.ToUpperFirst()}")
                                       .Replace("{type}", $"{property!.GetTypeName()}");

            propertiesText += propertyText + "\n";
        }

        return propertiesText;
    }

    private static void GenerateApiClient(string projectPath, SwaggerDocument swaggerDocument)
    {
        var apiClientPath = Path.Combine(projectPath, "ApiClient");
        Directory.CreateDirectory(apiClientPath);

        GenerateApiClientClass(apiClientPath, swaggerDocument);
    }

    private static void GenerateApiClientClass(string apiClientPath, SwaggerDocument swaggerDocument)
    {
        var apiClientTemplatePath = options.CustomTemplates
                                           .SingleOrDefault(x => x.TemplateType == TemplateType.HttpClient)?.TemplatePath 
                                    ?? Path.Combine(".", "Templates", "ApiClient.txt");
        
        using var writer = File.CreateText(Path.Combine(apiClientPath, "ApiClient.cs"));
        var apiClientText = File.ReadAllText(apiClientTemplatePath);
        
        writer.Write(apiClientText.Replace("{basePath}", $"\"{swaggerDocument.Servers.First().Url}\""));
        writer.Flush();
    }

    private static Process RunConsole()
    {
        var cmd = new Process();
        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = true;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;
        cmd.Start();

        return cmd;
    }

    private static void CloseConsole(Process cmd)
    {
        cmd.StandardInput.Flush();
        cmd.StandardInput.Close();
        cmd.WaitForExit();
        Console.WriteLine(cmd.StandardOutput.ReadToEnd());
    }
}