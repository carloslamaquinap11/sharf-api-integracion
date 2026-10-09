namespace Domain;

public class Courier : BaseDomainModel
{
    public string Name { get; private set; } = string.Empty;
    public List<Tracking> Trackings { get; private set; } = [];
    private Courier() { }
    private Courier(Guid id) : base(id) { }
    private Courier(Guid id, string name) : base(id)
    {
        Name = name;
    }
    public static Courier Create(string name)
    {
        return new Courier(Guid.NewGuid(), name);
    }
}