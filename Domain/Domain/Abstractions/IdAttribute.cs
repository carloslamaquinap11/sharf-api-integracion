namespace Domain;

[AttributeUsage(AttributeTargets.Field)]
public class IdAttribute : Attribute
{
    public string Description { get; } = string.Empty;

    public IdAttribute(string description)
    {
        Description = description;
    }
}
