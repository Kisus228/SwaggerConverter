namespace ClientGenerator.Options;

public class ClientGeneratorOptions
{
    internal string DocumentPath { get; private set; }
    internal string OutputDirectory { get; private set; }
    internal List<CustomTemplate> CustomTemplates { get; } = new();

    public ClientGeneratorOptions SetDocumentPath(string path)
    {
        DocumentPath = path;
        return this;
    }

    public ClientGeneratorOptions SetOutputDirectory(string directoryPath)
    {
        OutputDirectory = directoryPath;
        return this;
    }

    public ClientGeneratorOptions AddCustomTemplates(params CustomTemplate[] customTemplates)
    {
        CustomTemplates.AddRange(customTemplates);
        return this;
    }
}