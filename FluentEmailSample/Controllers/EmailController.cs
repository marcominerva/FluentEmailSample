using System.Net.Mime;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmailSample.Models;
using Microsoft.AspNetCore.Mvc;

namespace FluentEmailSample.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class EmailController : ControllerBase
{
    private readonly IFluentEmail fluentEmail;

    public EmailController(IFluentEmail fluentEmail)
    {
        this.fluentEmail = fluentEmail;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Send(EmailContent message)
    {
        var response = await fluentEmail
            .To(message.Receivers.Select(x => new Address(x)))
            .CC(message.CcReceivers.Select(x => new Address(x)))
            .BCC(message.BccReceivers.Select(x => new Address(x)))
            .Subject(message.Subject)
            .Body(message.Body, message.IsHtml)
            //.Attach(new Attachment
            //{
            //    Data = System.IO.File.OpenRead(@"D:\Taggia.jpg"),
            //    Filename = "Taggia.jpg",
            //    ContentType = "image/jpeg"
            //})
            .SendAsync();

        if (response.Successful)
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        var problem = ProblemDetailsFactory.CreateProblemDetails(HttpContext, StatusCodes.Status400BadRequest, response.ErrorMessages.FirstOrDefault());
        return StatusCode(StatusCodes.Status400BadRequest, problem);
    }

    [HttpPost("template")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> SendTemplate(TemplatedEmailContent message)
    {
        var response = await fluentEmail
            .To(message.Receivers.Select(x => new Address(x)))
            .CC(message.CcReceivers.Select(x => new Address(x)))
            .BCC(message.BccReceivers.Select(x => new Address(x)))
            .Subject(message.Subject)
            .UsingTemplate("Hello, @Model.Name! The current server time is: @System.DateTimeOffset.Now", new { message.Model?.Name })
            //.Attach(new Attachment
            //{
            //    Data = System.IO.File.OpenRead(@"D:\Taggia.jpg"),
            //    Filename = "Taggia.jpg",
            //    ContentType = "image/jpeg"
            //})
            .SendAsync();

        if (response.Successful)
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        var problem = ProblemDetailsFactory.CreateProblemDetails(HttpContext, StatusCodes.Status400BadRequest, response.ErrorMessages.FirstOrDefault());
        return StatusCode(StatusCodes.Status400BadRequest, problem);
    }
}
