namespace FluentEmailSample.Extensions;

public static class FluentEmailSendinblueBuilderExtensions
{
    public static FluentEmailServicesBuilder AddSendinblueSender(this FluentEmailServicesBuilder builder, string apiKey)
    {
        builder.Services.TryAddSingleton<ISender>(_ => new SendinblueSender(apiKey));
        return builder;
    }
}
