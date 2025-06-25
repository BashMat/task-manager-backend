namespace TaskManagerBackend.Domain.Validation;

public interface IEmailValidator
{
    bool Validate(string emailAddress);
}