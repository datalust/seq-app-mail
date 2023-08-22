# Seq output apps for SMTP and Microsoft365

A suite of updated HTML email apps for Seq, supporting various authentication methods.

## Escaping of text inserted into HTML documents

The templating system employed by the app does HTML escaping automatically. Untrusted values from log events can be safely
inserted into HTML attributes (single and double quoted), and all HTML element bodies excluding `<script>` and `<style>`.

## Development

To run the simple SMTP version of the app locally during development, build with `Build.ps1` then pipe `seqcli tail` output from your Seq instance
to `RunLocalSmtp.ps1`:

```
./Build.ps1
seqcli tail --json | ./RunLocalSmtp.ps1
```

This assumes you're running an SMTP server such as Papercut on your local machine at port 25.
