namespace Domain;

public class Vehicle : BaseDomainModel
{
    public string Code { get; private set; } = string.Empty;
    public List<Tracking> Trackings { get; private set; } = [];
    private Vehicle() { }
    private Vehicle(Guid id) : base(id) { }
    private Vehicle(Guid id, string code) : base(id)
    {
        Code = code;
    }
    public static Vehicle Create(string code)
    {
        return new Vehicle(Guid.NewGuid(), code);
    }
}