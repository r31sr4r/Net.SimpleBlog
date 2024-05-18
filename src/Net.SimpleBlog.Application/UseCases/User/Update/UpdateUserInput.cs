using MediatR;
using Net.SimpleBlog.Application.UseCases.User.Common;

namespace Net.SimpleBlog.Application.UseCases.User.Update;
public class UpdateUserInput : IRequest<UserModelOutput>
{
    public UpdateUserInput(
        Guid id,
        string name,
        string email,
        string phone,
        string cpf,
        DateTime dateOfBirth,
        string? rg = null,
        bool isActive = true
    )
    {
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
        CPF = cpf;
        RG = rg;
        DateOfBirth = dateOfBirth;
        IsActive = isActive;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string CPF { get; set; }
    public string? RG { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsActive { get; set; }

}

