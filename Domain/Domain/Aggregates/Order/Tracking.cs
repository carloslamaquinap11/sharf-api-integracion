namespace Domain;

public class Tracking : BaseDomainModel
{
    public string Number { get; private set; } = string.Empty;
    public Guid IdVehicle { get; private set; }
    public Guid IdCourier { get; private set; }
    public Guid IdOrder { get; private set; }
    public Vehicle? Vehicle { get; private set; } = null;
    public Courier? Courier { get; private set; } = null;
    public Order? Order { get; private set; } = null;
    public List<File> Files { get; private set; } = [];
    private Tracking() { }
    private Tracking(Guid id) : base(id) { }
    private Tracking(Guid id, string number, Guid idVehicle, Guid idCourier, Guid idOrder) : base(id)
    {
        Number = number;
        IdVehicle = idVehicle;
        IdCourier = idCourier;
        IdOrder = idOrder;
    }
    public static Tracking Create(string number, Guid idVehicle, Guid idCourier, Guid idOrder)
    {
        return new Tracking(Guid.NewGuid(), number, idVehicle, idCourier, idOrder);
    }
}