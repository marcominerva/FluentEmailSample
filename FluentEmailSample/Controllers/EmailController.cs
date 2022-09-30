using System.Diagnostics;
using FluentEmail.Core;
using FluentEmail.Core.Models;
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
    public async Task<IActionResult> Send()
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

        return NoContent();
    }
}
