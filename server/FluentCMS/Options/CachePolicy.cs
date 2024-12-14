namespace FluentCMS.Options;

public class ImageCompressionOptions
{
    public int MaxWidth { get; set; } = 1200;
    public int Quality { get; set; } = 75;
}

public sealed class RouteOptions
{
    public string ApiBaseUrl { get; set; } = "/api";
    public string PageBaseUrl { get; set; } = "";
}

public sealed class CmsOptions
{
    public const string DefaultPageCachePolicyName = "CmsPageCachePolicy";
    public const string DefaultQueryCachePolicyName = "CmsQueryCachePolicy";
    
    public bool EnableClient = true;
    public bool MapCmsHomePage { get; set; } = true;
    public string GraphQlPath { get; set; } = "/graphql";
    public TimeSpan EntitySchemaExpiration { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan QuerySchemaExpiration { get; set; } = TimeSpan.FromMinutes(1);

    public string PageCachePolicy { get; set; } = DefaultPageCachePolicyName;
    public string QueryCachePolicy { get; set; } = DefaultQueryCachePolicyName;
    public int DatabaseQueryTimeout { get; set; } = 30;
    public ImageCompressionOptions ImageCompression { get; set; } = new ();
    public RouteOptions RouteOptions { get; set; } = new ();
}