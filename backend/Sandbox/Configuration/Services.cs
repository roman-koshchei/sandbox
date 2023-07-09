using Sandbox.Lib;
using Unator.Email.Senders;
using Unator.Email;
using Unator;

namespace Sandbox.Configuration;

public static class Services
{
    public static void AddJwt(this IServiceCollection services)
    {
        JwtSecrets jwtSecrets = new(
            Env.Get("JWT_ISSUER"),
            Env.Get("JWT_AUDIENCE"),
            Env.Get("JWT_SECRET")
        );
        services.AddScoped<Jwt>(_ => new(jwtSecrets));
    }

    public static void AddEmail(this IServiceCollection services)
    {
        var resendApiKey = Env.Get("RESEND_API_KEY");
        services.AddSingleton<UEmailSender>(_ => new EmailGod(
            new EmailService(new Resend(resendApiKey), new DayLimiter(100)) // 100/day = 3000/month
        ));
    }
}
