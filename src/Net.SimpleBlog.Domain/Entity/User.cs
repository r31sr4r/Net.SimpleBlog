using Net.SimpleBlog.Domain.Common.Security;
using Net.SimpleBlog.Domain.Exceptions;
using Net.SimpleBlog.Domain.SeedWork;
using Net.SimpleBlog.Domain.Validation;

namespace Net.SimpleBlog.Domain.Entity;
public class User : AggregateRoot
{
    public User()
    {
    }

    public User(
        string name,
        string email,
        string phone,
        string cpf,
        DateTime dateOfBirth,
        string rg,
        string? password,
        bool isActive = true
    ) : base()
    {
        Name = name;
        Email = email;
        Phone = phone;
        CPF = cpf;
        RG = rg;
        DateOfBirth = dateOfBirth;
        Password = password != string.Empty
            ? PasswordHasher.HashPassword(password!)
            : null;
        IsActive = isActive;

        CreatedAt = DateTime.Now;
        Validate();

    }

    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string CPF { get; private set; }
    public string RG { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string? Password { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? InvestmentPreferences { get; private set; }
    public DateTime? AnalysisDate { get; private set; }

    public void Update(string name, string email, string phone, string cpf, DateTime dateOfBirth, string? rg = null)
    {
        Name = name;
        Email = email;
        Phone = phone;
        CPF = cpf;
        RG = rg;
        DateOfBirth = dateOfBirth;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new EntityValidationException($"{nameof(Name)} should not be empty or null");
        if (Name.Length <= 3)
            throw new EntityValidationException($"{nameof(Name)} should be greater than 3 characters");
        if (Name.Length >= 255)
            throw new EntityValidationException($"{nameof(Name)} should be less than 255 characters");
        if (string.IsNullOrWhiteSpace(Email))
            throw new EntityValidationException($"{nameof(Email)} should not be empty or null");
        if (!ValidationHelper.IsValidEmail(Email))
            throw new EntityValidationException($"{nameof(Email)} is not in a valid format");
        if (string.IsNullOrWhiteSpace(CPF))
            throw new EntityValidationException($"{nameof(CPF)} should not be empty or null");
        if (!ValidationHelper.IsCpfValid(CPF))
            throw new EntityValidationException($"{nameof(CPF)} is not valid");
        if (string.IsNullOrWhiteSpace(Phone))
            throw new EntityValidationException($"{nameof(Phone)} should not be empty or null");
        if (!ValidationHelper.IsValidPhone(Phone))
            throw new EntityValidationException($"{nameof(Phone)} is not in a valid format");
        if (!string.IsNullOrEmpty(Password))
            ValidatePassword();
    }

    private void ValidatePassword()
    {
        if (!ValidationHelper.IsValidPassword(Password!))
            throw new EntityValidationException($"{nameof(Password)} does not meet complexity requirements");
    }

    public void Activate()
    {
        IsActive = true;
        Validate();
    }

    public void Deactivate()
    {
        IsActive = false;
        Validate();
    }

    public void UpdatePassword(string currentPassword, string newPassword)
    {
        if (!PasswordHasher.VerifyPasswordHash(currentPassword, this.Password))
        {
            throw new EntityValidationException("Current password is not valid");
        }

        if (!ValidationHelper.IsValidPassword(newPassword))
        {
            throw new EntityValidationException("New password does not meet complexity requirements");
        }

        this.Password = PasswordHasher.HashPassword(newPassword);
    }
}

