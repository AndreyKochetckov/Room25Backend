using Microsoft.AspNetCore.Mvc;
using Room25Backend.Application.Interfaces;
using Room25Backend.Domain.Entities;
using System.Net;

namespace Room25Backend.API.Controllers;

[Route("api/session")]
[ApiController]
public class SessionController : ControllerBase
{
    ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ConflictObjectResult), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ConflictObjectResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ConflictObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ConflictObjectResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post()
    {
        try
        {
            return Ok(await _sessionService.CreateSession());
        }
        catch (HttpRequestException e)
        {
            return e.StatusCode switch
            {
                HttpStatusCode.Conflict => Conflict(new { message = e.Message }),
                HttpStatusCode.BadRequest => BadRequest(new { message = e.Message }),
                HttpStatusCode.NotFound => NotFound(new { message = e.Message }),
                HttpStatusCode.Forbidden => Forbid(e.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, e.Message)
            };
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpPut("start-session")]
    [ProducesResponseType(typeof(GameInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ConflictObjectResult), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ConflictObjectResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ConflictObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ConflictObjectResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StartSession([FromBody] Guid sessionId)
    {
        try
        {
            return Ok(await _sessionService.StartSession(sessionId));
        }
        catch (HttpRequestException e)
        {
            return e.StatusCode switch
            {
                HttpStatusCode.Conflict => Conflict(new { message = e.Message }),
                HttpStatusCode.BadRequest => BadRequest(new { message = e.Message }),
                HttpStatusCode.NotFound => NotFound(new { message = e.Message }),
                HttpStatusCode.Forbidden => Forbid(e.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, e.Message)
            };
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
