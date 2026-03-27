using TaskManagerBackend.Domain.Workflow;

namespace TaskManagerBackend.Domain.Data;

public record StringAttribute
{
    private StringAttribute(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            // TODO: Should consider processing possible case of invalid data in database
            throw new InvariantException(ActionResultType.UserError,
                                         "String attribute cannot be null or empty");
        }
        
        Value = value.Trim();
    }

    public static StringAttribute CreateRequired(string value)
    {
        return new StringAttribute(value);
    }
    
    public static StringAttribute? CreateOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) 
                   ? null
                   : new StringAttribute(value);
    }

    public string Value { get; set; }
}