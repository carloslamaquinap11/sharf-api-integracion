namespace Rest;

using Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

public class OrderController : BaseApiController
{
    public OrderController(IMediator mediator) : base(mediator)
    {
    }

    [ServiceFilter(typeof(SecurityAuthorize))]
    [HttpGet("EventHistory")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetEventHistory()
    {
        var result = await _mediator.Send(new GetEventHistoryQuery());
        return Ok(result);
    }
    [ServiceFilter(typeof(SecurityAuthorize))]
    [HttpGet("{orderNumber}/EventHistory")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetEventHistoryByOrderNumber([FromRoute] string orderNumber)
    {
        var result = await _mediator.Send(new GetEventHistoryByOrderNumberQuery(orderNumber));
        return Ok(result);
    }
}