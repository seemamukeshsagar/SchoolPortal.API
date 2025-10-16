using System.Text.Json;
using System.Text.Json.Serialization;

namespace SchoolPortal.Web.Common;

public static class JsonSerializerOptionsConfig
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = true
    };
}