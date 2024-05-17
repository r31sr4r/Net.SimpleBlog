using MediatR;
using Net.SimpleBlog.Application.UseCases.User.Common;

namespace Net.SimpleBlog.Application.UseCases.User.CreateUser;
public class CreateUserInput : IRequest<UserModelOutput>
{
    public CreateUserInput(
        string name,
        string email,
        string phone,
        string cpf,
        DateTime dateOfBirth,
        string? rg = null,
        string? password = null,
        bool isActive = true
    )
    {
        Name = name;
        Email = email;
        Phone = phone;
        CPF = cpf;
        RG = rg;
        Password = password ?? string.Empty;
        DateOfBirth = dateOfBirth;
        IsActive = isActive;
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string CPF { get; set; }
    public string? RG { get; set; }
    public string? Password { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsActive { get; set; }
}
