using Net.SimpleBlog.Application.UseCases.User.Common;

namespace Net.SimpleBlog.Api.ApiModels.User;

public class AuthResponse
{
    public string Email { get; set; }
    public string Token { get; set; }
}
