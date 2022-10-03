namespace FluentEmailSample.Controllers.Common;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
{

}