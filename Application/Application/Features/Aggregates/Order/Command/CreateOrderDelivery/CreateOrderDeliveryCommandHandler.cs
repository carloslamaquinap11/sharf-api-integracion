namespace Application;

using Domain;
using MediatR;
public class CreateOrderDeliveryCommandHandler(IClientRepository clientRepository,
                                                IVehicleRepository vehicleRepository, 
                                                ICourierRepository courierRepository, 
                                                IUnitOfWork unitOfWork, 
                                                IMediator mediator) : IRequestHandler<CreateOrderDeliveryCommand, bool>
{
    public async Task<bool> Handle(CreateOrderDeliveryCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var detail = payload.Details!;
        var evidences = GetValidEvidences(detail.Evidences ?? []);

        var idClient = await GetIdClientByCode(detail.ClientCode!, detail.ClientName!);

        var idServiceType = (await mediator.Send(new GetServiceTypeQuery())).Where(x => x.IdProvider == payload.ServiceType).FirstOrDefault()?.Id ?? throw new NotFoundException("El serviceType enviado no es válido");
        var idDispatchType = (await mediator.Send(new GetDispatchTypeQuery())).Where(x => x.IdProvider == payload.DispatchType).FirstOrDefault()?.Id ?? throw new NotFoundException("El dispatchType enviado no es válido");
        var idOrderStatus = (await mediator.Send(new GetOrderStatusQuery())).Where(x => x.IdProvider == payload.Status).FirstOrDefault()?.Id ?? throw new NotFoundException("El status enviado no es válido");
        var idOrderSubStatus = (await mediator.Send(new GetOrderSubStatusQuery())).Where(x => x.IdProvider == payload.SubStatus).FirstOrDefault()?.Id ?? throw new NotFoundException("El subStatus enviado no es válido");
        var maxVisitsToBeReturned = await mediator.Send(new GetMaxVisitsToBeReturnedQuery());

        var order = Order.Create(idClient, idServiceType, idDispatchType, detail.OrderNumber!, idOrderStatus, idOrderSubStatus, detail.ReceivedBy!, detail.Comments, maxVisitsToBeReturned, detail.TrackingNumber!, evidences);

        var idVehicle = await GetIdVehicleByCode(payload.VehicleCode!);
        var idCourier = await GetIdCourierByName(payload.CourierName!);

        var tracking = Tracking.Create(detail.TrackingNumber!, idVehicle, idCourier, order.Id);

        unitOfWork.Repository<Order>().AddEntity(order);
        unitOfWork.Repository<Tracking>().AddEntity(tracking);

        await unitOfWork.Complete();

        return true;
    }
    private async Task<Guid> GetIdVehicleByCode(string code)
    {
        var vehicle = await vehicleRepository.GetVehicleByCode(code, true);
        if (vehicle is null)
        {
            var newVehicle = Vehicle.Create(code);
            unitOfWork.Repository<Vehicle>().AddEntity(newVehicle);
            return newVehicle.Id;
        }

        return vehicle.Id;
    }
    private async Task<Guid> GetIdCourierByName(string name)
    {
        var courier = await courierRepository.GetCourierByName(name, true);
        if (courier is null)
        {
            var newCourier = Courier.Create(name);
            unitOfWork.Repository<Courier>().AddEntity(newCourier);
            return newCourier.Id;
        }

        return courier.Id;
    }
    private async Task<Guid> GetIdClientByCode(string code, string name)
    {
        var client = await clientRepository.GetClientByCode(code, true);
        if (client is null)
        {
            var newClient = Client.Create(code, name, NotificationProfileEnum.Default.GetId(), "https://tiendasperuanas.com/api/notification");
            unitOfWork.Repository<Client>().AddEntity(newClient);
            return newClient.Id;
        }

        return client.Id;
    }
    private List<TrackingEvidenceViewModel> GetValidEvidences(IEnumerable<EvidenceRequest?> evidencesRequest)
    {
        var evidences = evidencesRequest.Select(x =>
        {
            if (x is null)
                throw new InvalidElementException("La evidencia no puede ser nula");

            return new TrackingEvidenceViewModel
            (
                x.Label ?? string.Empty,
                x.FileType ?? throw new InvalidElementException("La extensión del archivo es requerida"),
                x.FileName ?? throw new InvalidElementException("El nombre del archivo es requerido"),
                x.Url ?? throw new InvalidElementException("El url del archivo es requerido")
            );
        }).ToList();

        return evidences;
    }
}