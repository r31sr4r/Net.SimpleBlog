using System.Text.RegularExpressions;

namespace Net.SimpleBlog.Domain.Validation;
public class ValidationHelper
{
    public static bool IsCpfValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = cpf.Trim().Replace(".", "").Replace("-", "");

        if (cpf.Length != 11)
            return false;

        for (int j = 0; j < 10; j++)
            if (j.ToString().PadLeft(11, char.Parse(j.ToString())) == cpf)
                return false;

        int[] multiplier1 = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplier2 = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int sum = 0;

        for (int i = 0; i < 9; i++)
            sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];

        int remainder = sum % 11;

        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        string digit = remainder.ToString();
        tempCpf = tempCpf + digit;
        sum = 0;

        for (int i = 0; i < 10; i++)
            sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];

        remainder = sum % 11;

        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        digit = digit + remainder.ToString();

        return cpf.EndsWith(digit);
    }

    public static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidPhone(string phone)
    {
        var regex = new Regex(@"^\(\d{2}\)\s?\d{4,5}-\d{4}$");
        return regex.IsMatch(phone.Trim());
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // Definindo os critérios para uma senha forte
        var hasMinimumLength = new Regex(@".{8,}"); // Pelo menos 8 caracteres
        var hasNumber = new Regex(@"[0-9]+"); // Pelo menos um número
        var hasUpperChar = new Regex(@"[A-Z]+"); // Pelo menos uma letra maiúscula
        var hasLowerChar = new Regex(@"[a-z]+"); // Pelo menos uma letra minúscula
        var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]"); // Pelo menos um símbolo especial

        // Validando a senha de acordo com os critérios definidos
        return hasMinimumLength.IsMatch(password) &&
               hasNumber.IsMatch(password) &&
               hasUpperChar.IsMatch(password) &&
               hasLowerChar.IsMatch(password) &&
               hasSymbols.IsMatch(password);
    }

}

