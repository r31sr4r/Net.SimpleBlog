using MediatR;
using Net.SimpleBlog.Application.UseCases.User.Common;

namespace Net.SimpleBlog.Application.UseCases.User.AuthUser;
public class AuthUserInput : IRequest<UserModelOutput>
{
    public string Email { get; set; }
    public string Password { get; set; }

    public AuthUserInput(string email, string password)
    {
        Email = email;
        Password = password;
    }
}