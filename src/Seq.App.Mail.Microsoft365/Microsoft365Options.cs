namespace Seq.App.Mail.Microsoft365;

class Microsoft365Options
{
    public string TenantId { get; }
    public string ClientId { get; }
    public string ClientSecret { get; }
    public bool SaveToSentItems { get; }

    public Microsoft365Options(string tenantId, string clientId, string clientSecret, bool saveToSentItems)
    {
        TenantId = tenantId;
        ClientId = clientId;
        ClientSecret = clientSecret;
        SaveToSentItems = saveToSentItems;
    }
}