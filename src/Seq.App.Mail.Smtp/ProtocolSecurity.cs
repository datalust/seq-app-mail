using System.ComponentModel;

namespace Seq.App.Mail.Smtp;

public enum ProtocolSecurity
{
    [Description("Require TLS using implicit TLS (TLS-on-connect) for port 465, or `STARTTLS` for other ports.")]
    RequireTls = 0,
        
    [Description("Require that the server support TLS via the `STARTTLS` protocol extension.")]
    RequireStartTls,
        
    [Description("Require that the server support TLS implicitly (TLS-on-connect).")]
    RequireImplicitTls,
        
    [Description("Do not require TLS. The connection may use TLS if supported by the server, otherwise an " +
                 "unencrypted connection may be used.")]
    None
}