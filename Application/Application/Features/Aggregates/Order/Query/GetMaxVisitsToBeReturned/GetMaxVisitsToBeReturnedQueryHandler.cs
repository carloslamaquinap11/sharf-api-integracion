namespace Application;

using Domain;
using MediatR;
public class GetMaxVisitsToBeReturnedQueryHandler(IApiSeguridadService apiSeguridadService) : IRequestHandler<GetMaxVisitsToBeReturnedQuery, int>
{
    public async Task<int> Handle(GetMaxVisitsToBeReturnedQuery request, CancellationToken cancellationToken)
    {
        var maxVisitsToBeReturnedString = await apiSeguridadService.GetValueByKey("MAX_VISITS_TO_BE_RETURNED", true);
        if (!int.TryParse(maxVisitsToBeReturnedString, out int maxVisitsToBeReturned))
            throw new InvalidElementException("No se encontró el valor máximo de visitar para ser devuelto");

        return maxVisitsToBeReturned;
    }
}