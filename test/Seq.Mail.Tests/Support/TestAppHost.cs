using System.Collections.Generic;
using Seq.Apps;
using Serilog;

namespace Seq.Mail.Tests.Support;

class TestAppHost : IAppHost
{
    public App App { get; } = new("app-1", "Test App", new Dictionary<string, string>(), "//TEST");
    public Host Host { get; } = new("https://seq.example.com", null);
    public ILogger Logger { get; } = new LoggerConfiguration().CreateLogger();
    public string StoragePath { get; } = "//TEST";
}