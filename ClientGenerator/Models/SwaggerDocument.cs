namespace ClientGenerator.Models;

public class SwaggerDocument
{
    public Server[] Servers;
    public Dictionary<string, PathItem> Paths;
    public Components Components;
}