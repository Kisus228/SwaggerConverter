using Newtonsoft.Json;

namespace ClientGenerator.Models;

public class Property
{
    public string? Description;

    public string? Type;

    [JsonProperty(PropertyName = "$ref")]
    public string? Ref;

    public string GetTypeName()
    {
        return Ref is not null 
            ? Ref.Split('/').Last() 
            : ToSharpType(Type!);
    }

    private static string ToSharpType(string type)
    {
        return type switch
        {
            "integer" => "int",
            "array" => "int[]",
            "boolean" => "bool",
            _ => type
        };
    }
}

