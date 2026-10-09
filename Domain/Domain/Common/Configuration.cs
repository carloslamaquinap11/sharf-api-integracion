namespace Domain;

public sealed class Configuration : BaseDomainModel
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    private Configuration(Guid id) : base(id) { }
    private Configuration(Guid id, string key, string value, string description) : base(id)
    {
        Key = key;
        Value = value;
        Description = description;
    }
    public static Configuration Create(string key, string value, string description)
    {
        return new Configuration(Guid.NewGuid(), key, value, description);
    }
}