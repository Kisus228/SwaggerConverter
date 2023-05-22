using Newtonsoft.Json;

namespace ClientGenerator.Models;

public class Parameter
{
    public string Name;

    public string In;

    public string Description;

    public bool Required;

    public bool Deprecated;

    public bool AllowEmptyValue;
}