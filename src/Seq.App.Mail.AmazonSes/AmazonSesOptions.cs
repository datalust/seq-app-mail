namespace Seq.App.Mail.AmazonSes;

class AmazonSesOptions
{
    public string AccessKeyId { get; }
    public string SecretKey { get; }

    public AmazonSesOptions(string accessKeyId, string secretKey)
    {
        AccessKeyId = accessKeyId;
        SecretKey = secretKey;
    }
}
