# Seq output apps for SMTP and Microsoft365

A suite of updated HTML email apps for Seq, supporting various authentication methods.

## Escaping of text inserted into HTML documents

The templating system employed by the app does HTML escaping automatically. Untrusted values from log events can be safely
inserted into HTML attributes (single and double quoted), and all HTML element bodies excluding `<script>` and `<style>`.