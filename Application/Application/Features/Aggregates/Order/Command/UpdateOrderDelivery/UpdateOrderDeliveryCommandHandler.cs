namespace Application;

using Domain;
using MediatR;
public class UpdateOrderDeliveryCommandHandler(IOrderRepository orderRepository,
                                                IUnitOfWork unitOfWork,
                                                IMediator mediator) : IRequestHandler<UpdateOrderDeliveryCommand, bool>
{
    public async Task<bool> Handle(UpdateOrderDeliveryCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var detail = payload.Details!;
        var evidences = GetValidEvidences(detail.Evidences ?? []);

        var maxVisitsToBeReturned = await mediator.Send(new GetMaxVisitsToBeReturnedQuery());

        var idOrderStatus = (await mediator.Send(new GetOrderStatusQuery())).Where(x => x.IdProvider == payload.Status).FirstOrDefault()?.Id ?? throw new NotFoundException("El status enviado no es válido");
        var idOrderSubStatus = (await mediator.Send(new GetOrderSubStatusQuery())).Where(x => x.IdProvider == payload.SubStatus).FirstOrDefault()?.Id ?? throw new NotFoundException("El subStatus enviado no es válido");

        var order = await orderRepository.GetOrderByNumber(detail.OrderNumber!) ?? throw new NotFoundException("No se encontró el pedido");
        
        order.ActualizarEstado(idOrderStatus, idOrderSubStatus, maxVisitsToBeReturned, detail.TrackingNumber!, evidences);

        unitOfWork.Repository<Order>().UpdateEntity(order);

        await unitOfWork.Complete();

        return true;
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