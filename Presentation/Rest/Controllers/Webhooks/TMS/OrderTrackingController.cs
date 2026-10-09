namespace Rest;

using Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

public class OrderTrackingController : BaseApiController
{
    public OrderTrackingController(IMediator mediator) : base(mediator)
    {
    }

    [ServiceFilter(typeof(SecurityAuthorize))]
    [HttpPost]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SaveWebhookPayload([FromBody] JsonElement payload)
    {
        var result = await _mediator.Send(new SaveWebhookPayloadCommand(JsonSerializer.Serialize(payload)));
        return Ok(result);
    }
}