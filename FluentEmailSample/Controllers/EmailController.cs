using System.Diagnostics;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmailSample.Models;
using Microsoft.AspNetCore.Mvc;

namespace FluentEmailSample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IFluentEmail fluentEmail;

    public EmailController(IFluentEmail fluentEmail)
    {
        this.fluentEmail = fluentEmail;
    }

    [HttpPost]
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
            return Accepted();
        }

        var problem = ProblemDetailsFactory.CreateProblemDetails(HttpContext, StatusCodes.Status400BadRequest, response.ErrorMessages.FirstOrDefault());
        return StatusCode(StatusCodes.Status400BadRequest, problem);
    }

    [HttpPost("template")]
    public async Task<IActionResult> SendTemplate(TemplatedEmailContent message)
    {
        var response = await fluentEmail
            .To("marco.minerva@gmail.com")
            .Subject("Messaggio di prova")
            //.Body("Questa è la prova incredibile di un messaggio che sto inviando con FluentEmail")
            .UsingTemplate("Ciao, @Model.Name", new { Name = "Marco" })
            .Attach(new Attachment
            {
                Data = System.IO.File.OpenRead(@"D:\Taggia.jpg"),
                Filename = "Taggia.jpg"
            })
            .SendAsync();

        Debug.WriteLine(response);

        return Accepted();
    }
}
