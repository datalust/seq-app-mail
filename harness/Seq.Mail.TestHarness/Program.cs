using Seq.App.Mail.Smtp;
using Serilog;

using var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .AuditTo.SeqApp<SmtpMailApp>(new Dictionary<string, string>
    {
        [nameof(SmtpMailApp.From)] = "from@localhost",
        [nameof(SmtpMailApp.To)] = "to@localhost",
        [nameof(SmtpMailApp.Host)] = "localhost",
        [nameof(SmtpMailApp.ProtocolSecurity)] = "None"
    })
    .CreateLogger();

logger.Information("Hello, {Name}!", Environment.UserName);
