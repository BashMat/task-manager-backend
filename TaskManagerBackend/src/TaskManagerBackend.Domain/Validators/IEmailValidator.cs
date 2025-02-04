namespace TaskManagerBackend.Domain.Validators;

public interface IEmailValidator
{
    bool ValidateEmailAddressFormat(string emailAddress);
}