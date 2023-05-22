namespace SwaggerDocumentationGenerator.Options;

public class DocumentationGeneratorOptions
{
    public string OutputPath { internal get; set; }
    internal List<Type> ApiControllers { get; } = new();
    public string[] ServerUrls { internal get; set; }
    public string ApiVersion { internal get; set; }

    public DocumentationGeneratorOptions SetOutputPath(string path)
    {
        OutputPath = path;
        return this;
    }

    public DocumentationGeneratorOptions AddControllers(params Type[] controllerTypes)
    {
        ApiControllers.AddRange(controllerTypes);
        return this;
    }

    public DocumentationGeneratorOptions SetServerUrls(string[] urls)
    {
        ServerUrls = urls;
        return this;
    }

    public DocumentationGeneratorOptions SetApiVersion(string version)
    {
        ApiVersion = version;
        return this;
    }
}