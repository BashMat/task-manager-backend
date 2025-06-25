using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Domain.Validation;

public class PasswordAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return false;
        }
        
        if (value is not string)
        {
            return false;
        }
        
        string password = (string) value;

        return password.Length >= 8;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The field {name} must be a string with a minimum length of 8.";
    }
}