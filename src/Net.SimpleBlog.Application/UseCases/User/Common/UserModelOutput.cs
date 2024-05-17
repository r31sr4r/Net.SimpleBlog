namespace Net.SimpleBlog.Application.UseCases.User.Common;
public class UserModelOutput
{
    public UserModelOutput(
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
        IsActive = isActive;
        CreatedAt = DateTime.Now;
        CPF = cpf;
        RG = rg;
        DateOfBirth = dateOfBirth;
    }

    public Guid Id { get; set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CPF { get; private set; }
    public string? RG { get; private set; }
    public DateTime DateOfBirth { get; private set; }

    public static UserModelOutput FromUser(Domain.Entity.User user)
    {
        return new UserModelOutput(
            user.Id,
            user.Name,
            user.Email,
            user.Phone,
            user.CPF,
            user.DateOfBirth,
            user.RG,
            user.IsActive
        );
    }
}

