namespace TaskManagerBackend.Application.Services.Email;

public interface IEmailService
{
    bool ValidateEmailAddressFormat(string emailAddress);
}