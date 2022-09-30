using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace FluentEmailSample.Sendinblue;

public class SendinblueSender : ISender
{
    public SendinblueSender(string apiKey)
    {
        Configuration.Default.ApiKey.TryAdd("api-key", apiKey);
    }

    public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        => SendAsync(email, token).GetAwaiter().GetResult();

    public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
    {
        var sendResponse = new SendResponse();

        var message = await CreateSmtpEmailAsync(email);
        var emailSender = new TransactionalEmailsApi();

        try
        {
            var result = await emailSender.SendTransacEmailAsyncWithHttpInfo(message);

            if (result.StatusCode is >= StatusCodes.Status200OK and <= 299)
            {
                sendResponse.MessageId = result.Data.MessageId;
            }
            else
            {
                sendResponse.ErrorMessages.Add(result.StatusCode.ToString());
            }
        }
        catch (Exception ex)
        {
            sendResponse.ErrorMessages.Add(ex.Message);
        }

        return sendResponse;
    }

    private static async Task<SendSmtpEmail> CreateSmtpEmailAsync(IFluentEmail message)
    {
        var content = message.Data;

        var sender = new SendSmtpEmailSender(!string.IsNullOrWhiteSpace(content.FromAddress.Name) ? content.FromAddress.Name : null, content.FromAddress.EmailAddress);
        var toAddressList = content.ToAddresses.Any() ? content.ToAddresses.Select(a => new SendSmtpEmailTo(a.EmailAddress, a.Name)).ToList() : null;
        var ccAddressList = content.CcAddresses.Any() ? content.CcAddresses.Select(a => new SendSmtpEmailCc(a.EmailAddress, a.Name)).ToList() : null;
        var bccAddressList = content.BccAddresses.Any() ? content.BccAddresses.Select(a => new SendSmtpEmailBcc(a.EmailAddress, a.Name)).ToList() : null;
        var replyToAddress = content.ReplyToAddresses.Any() ? new SendSmtpEmailReplyTo(content.ReplyToAddresses.First().EmailAddress, content.ReplyToAddresses.First().Name) : null;

        var email = new SendSmtpEmail(sender, toAddressList, cc: ccAddressList, bcc: bccAddressList, replyTo: replyToAddress)
        {
            Subject = content.Subject
        };

        if (content.IsHtml)
        {
            email.HtmlContent = content.Body;
            email.TextContent = content.PlaintextAlternativeBody;
        }
        else
        {
            email.TextContent = content.Body;
        }

        if (content.Attachments.Any())
        {
            email.Attachment = new List<SendSmtpEmailAttachment>();
            foreach (var attachment in content.Attachments)
            {
                var emailAttachment = await ConvertAttachmentAsync(attachment);
                email.Attachment.Add(emailAttachment);
            }
        }

        return email;
    }

    private static async Task<SendSmtpEmailAttachment> ConvertAttachmentAsync(Attachment attachment)
    {
        var sendinblueAttachment = new SendSmtpEmailAttachment
        {
            Content = await GetAttachmentByteArrayAsync(attachment.Data),
            Name = attachment.Filename
        };

        return sendinblueAttachment;

        static async Task<byte[]> GetAttachmentByteArrayAsync(Stream stream)
        {
            using var ms = new MemoryStream();

            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
