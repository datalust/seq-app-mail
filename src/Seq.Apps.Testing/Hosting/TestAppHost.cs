// Copyright © Datalust Pty Ltd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using Serilog;

namespace Seq.Apps.Testing.Hosting;

/// <summary>
/// A simple default implementation of <see cref="IAppHost"/>. The purpose of <see cref="IAppHost"/>
/// is to allow a running app instance to inspect its own configuration and environment.
/// </summary>
public class TestAppHost : IAppHost
{
    /// <summary>
    /// Information describing the app being hosted.
    /// </summary>
    public App App { get; } = new("app-1", "Test App", new Dictionary<string, string>(), "//TEST");

    /// <summary>
    /// Information about the Seq instance hosting the app.
    /// </summary>
    public Host Host { get; } = new("https://seq.example.com", null);

    /// <summary>
    /// Logger used by the app to report diagnostic messages.
    /// </summary>
    public ILogger Logger { get; } = new LoggerConfiguration().CreateLogger();
    
    /// <summary>
    /// Obsolete; use <see cref="Seq.Apps.App.StoragePath"/>.
    /// </summary>
    public string StoragePath => App.StoragePath;
}
