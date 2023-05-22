namespace ClientGenerator.Templates;

public static class TemplatesStore
{
    public static string ApiClient = GetTemplateText(nameof(ApiClient));
    public static string Model = File.ReadAllText(Path.Combine("Templates", $"{nameof(Model)}.txt"));
    public static string Property = File.ReadAllText(Path.Combine("Templates", $"{nameof(Property)}.txt"));
    
    private static string GetTemplateText(string name) => File.ReadAllText(Path.Combine("Templates", $"{name}.txt"));
}