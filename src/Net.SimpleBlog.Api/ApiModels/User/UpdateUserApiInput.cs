namespace Net.SimpleBlog.Api.ApiModels.User;

public class UpdateUserApiInput
{
    public UpdateUserApiInput(
    string name,
    string email,
    string phone,
    string cpf,
    DateTime dateOfBirth,
    string rg,
    bool isActive = true
    ) 
    {
        Name = name;
        Email = email;
        Phone = phone;
        CPF = cpf;
        RG = rg;
        DateOfBirth = dateOfBirth;
        IsActive = isActive;
        CreatedAt = DateTime.Now;
    }

    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string CPF { get; private set; }
    public string RG { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

}
