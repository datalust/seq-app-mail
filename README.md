# Seq Mail Apps [![Build status](https://ci.appveyor.com/api/projects/status/6jo5xhyfans07msl/branch/dev?svg=true)](https://ci.appveyor.com/project/datalust/seq-app-mail/branch/dev)

This repository contains the Seq output apps for various email services, built on a shared email templating system.

| Package id                                                                          | Description                                                     |
|-------------------------------------------------------------------------------------|-----------------------------------------------------------------|
| [`Seq.App.Mail.Microsoft365`](https://nuget.org/packages/seq.app.mail.microsoft365) | Send email through Microsoft 365 using the Microsoft Graph API. |
| [`Seq.App.Mail.Smtp`](https://nuget.org/packages/seq.app.mail.smtp)                 | Send email using the SMTP protocol.                             |

> Need to send email using a protocol or API not listed here? Let us know!


## Getting started

Install the app under _Settings > Apps_, using one of the package ids from the table above.

Visit the Seq documentation for detailed information about [installing and configuring Seq Apps](https://docs.datalust.co/docs/installing-seq-apps).

## Settings

When starting an instance of the app in Seq, the following parameters can be supplied.

### All apps

| Property               | Description                                                                        | Template? | Default                      |
|------------------------|------------------------------------------------------------------------------------|-----------|------------------------------|
| **From**               | The email address of the sending identity.                                         |           |                              |
| **To**                 | One or more email addresses that email will be sent to, separated by commas.       | Yes       |                              |
| **Subject**            | The email subject.                                                                 | Yes       | `{@Message}`                 |
| **Body**               | The email body.                                                                    | Yes       | See `src/Seq.Mail/Resources` |
| **Body is plain text** | If checked, the body template will be interpreted as plain text, rather than HTML. |           |                              |

### `Seq.App.Mail.Microsoft365`

To send mail using the Microsoft 365 app, first create an app registration in Azure. The app must have the `Mail.Send` permission 
for the Microsoft Graph API.

| Property               | Description                                                                                  | Template? | Default |
|------------------------|----------------------------------------------------------------------------------------------|---|---|
| **Tenant id**          | The id of the Azure tenant (directory) in which the app is registered.                       | | |
| **Client id**          | The app's client (application) id.                                                           | | |
| **Client secret**      | A client secret belonging to the app registration.                                           | | |
| **Save to sent items** | If checked, email sent by the app will be saved to the sending identity's sent items folder. | | |

### `Seq.App.Mail.Smtp`

| Property              | Description                                                                             | Template? | Default |
|-----------------------|-----------------------------------------------------------------------------------------|-----------|---------|
| **Host**              | The DNS name of the SMTP server.                                                        |           |         |
| **Port**              | The port to connect to on the SMTP server.                                              |           | 25      |
| **Protocol security** | Options controlling TLS/SSL security.                                                   |           |         |
| **Username**          | Name used when authenticating to the SMTP server, if required.                          |           |         |
| **Password**          | Password used when authenticating to the SMTP server; ignored unless `Username` is set. |           |         |

## Templates

Event and notification properties can be inserted dynamically into many of the settings listed above, by surrounding them
with braces:

```
Error in {Environment}!
```

Here, `Environment` is an event property, producing a message subject like `Error in Production!`.

### Basic syntax

Templates support:

 * Most built-in Seq event properties, including `@Level`, `@Message`, and `@Exception`,
 * First-class properties of events and alerts, like `Environment` in the example above,
 * Most Seq scalar functions, such as `ToIsoString()`, `Coalesce()`, `Substring()`, `IndexOf()`, and so on,
 * Seq operators such as `=`, `<>`, `<`, `>`, `like`, `in`, `is null`,
 * Constant numbers `123.4`, strings `'abc'`, Boolean `true` and `false`, and `null`,
 * Arrays delimited with brackets `[]` and zero-based indexing,
 * Object literals using braces `{}` that support string-based indexing,
 * Most other Seq expression language features.

Literal braces in templated text fields can be escaped by doubling, `{{` and `}}`.

Formatting of dates and numbers can be achieved using .NET format strings following a colon, e.g.:

```
Completed in {Elapsed:0.00} ms
```

### Conditionals and repetition

To conditionally include text, use `{#if expr}`:

```
{#if Count = 0}
  Nothing here
{#else if Count = 1}
  Only one
{#else}
  Found {Count} items
{#end}
```

The `else`/`else if` blocks are optional.

To iterate over array elements or object properties use `{#each e in expr}` or `{#each k, v in expr}`:

```
{#each name, value in @Properties}
  {name} is {value}
{#delimit}
  ---
{#else}
  No properties
{#end}
```

The `delimit` and `else` blocks are optional.

## Escaping text inserted into HTML message bodies

The templating system employed by the app does HTML escaping automatically for HTML email bodies. Untrusted 
values from log events can be safely inserted into HTML attributes and element bodies, excluding script and 
style contexts, in which no safe escaping is possible.

## Development

To run the simple SMTP version of the app locally, without installing it into Seq, build with `Build.ps1` then 
pipe `seqcli tail` output from your Seq instance to `RunLocalSmtp.ps1`:

```
./Build.ps1
seqcli tail --json | ./RunLocalSmtp.ps1
```

This assumes that you have `seqcli` configured with the address of your Seq server (`http://localhost:5341` by default),
and you're running an SMTP server such as [Papercut SMTP](https://github.com/ChangemakerStudios/Papercut-SMTP) on your
local machine at port 25.

## Acknowledgements

Templating support is based on code from [_Serilog.Expressions_](https://github.com/serilog/serilog-expressions).