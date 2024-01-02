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

using System;
using System.Collections.Generic;
using Seq.Apps;
using Seq.Apps.Testing.Hosting;
using Seq.Apps.Testing.Sink;
using Serilog.Configuration;
// ReSharper disable MemberCanBePrivate.Global, CheckNamespace

// Don't require additional using statements to make the `SeqApp()` extension methods visible.
namespace Serilog;

/// <summary>
/// Extends <see cref="LoggerAuditSinkConfiguration"/> with methods for configuring a Seq App sink.
/// </summary>
public static class LoggerAuditSinkConfigurationSeqAppExtensions
{
    /// <summary>
    /// Write log events to an instance of <typeparamref name="TApp"/>.
    /// </summary>
    /// <param name="loggerSinkConfiguration">The Serilog `AuditTo` property receiver.</param>
    /// <param name="settings">Values for the app's exposed <see cref="SeqAppSettingAttribute"/> properties.</param>
    /// <param name="host">A custom/configured <see cref="IAppHost"/>.</param>
    /// <typeparam name="TApp">A type deriving from <see cref="Seq.Apps.SeqApp"/>.</typeparam>
    /// <returns>Configuration object allowing method chaining.</returns>
    public static LoggerConfiguration SeqApp<TApp>(this LoggerAuditSinkConfiguration loggerSinkConfiguration, IReadOnlyDictionary<string, string>? settings = null, IAppHost? host = null)
        where TApp: SeqApp
    {
        return loggerSinkConfiguration.SeqApp(typeof(TApp), settings);
    }

    /// <summary>
    /// Write log events to an instance of <paramref name="appType"/>.
    /// </summary>
    /// <param name="loggerSinkConfiguration">The Serilog `AuditTo` property receiver.</param>
    /// <param name="appType">A type deriving from <see cref="Seq.Apps.SeqApp"/>.</param>
    /// <param name="settings">Values for the app's exposed <see cref="SeqAppSettingAttribute"/> properties.</param>
    /// <param name="host">A custom/configured <see cref="IAppHost"/>.</param>
    /// <returns>Configuration object allowing method chaining.</returns>
    /// <exception cref="ArgumentException"><paramref name="appType"/> does not derive from <see cref="Seq.Apps.SeqApp"/>.</exception>
    public static LoggerConfiguration SeqApp(this LoggerAuditSinkConfiguration loggerSinkConfiguration, Type appType, IReadOnlyDictionary<string, string>? settings = null, IAppHost? host = null)
    {
        if (!typeof(SeqApp).IsAssignableFrom(appType))
            throw new ArgumentException($"The type `{appType}` does not derive from `SeqApp`.");

        var testHost = host ?? new TestAppHost();
        var app = SeqAppActivator.CreateInstance(appType, testHost.App.Title, settings ?? new Dictionary<string, string>());
        return loggerSinkConfiguration.SeqApp(app, testHost);
    }

    /// <summary>
    /// Write log events to <paramref name="app" />
    /// </summary>
    /// <param name="loggerSinkConfiguration">The Serilog `AuditTo` property receiver.</param>
    /// <param name="app">An app instance.</param>
    /// <param name="host">A custom/configured <see cref="IAppHost"/>.</param>
    /// <returns>Configuration object allowing method chaining.</returns>
    public static LoggerConfiguration SeqApp(this LoggerAuditSinkConfiguration loggerSinkConfiguration, SeqApp app, IAppHost? host = null)
    {
        return loggerSinkConfiguration.Sink(new SeqAppSink(app, host ?? new TestAppHost()));
    }
}
