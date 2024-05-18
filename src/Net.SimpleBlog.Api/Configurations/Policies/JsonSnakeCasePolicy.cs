using Net.SimpleBlog.Api.Extensions.String;
using System.Text.Json;

namespace Net.SimpleBlog.Api.Configurations.Policies;

public class JsonSnakeCasePolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
        => name.ToSnakeCase();
}
