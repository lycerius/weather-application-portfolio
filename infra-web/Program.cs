using System.Collections.Generic;
using System.Text.Json;
using Pulumi;
using Pulumi.Aws.S3;
using Pulumi.Aws.S3.Inputs;

return await Deployment.RunAsync(() =>
{
    // DIY backends always use the literal "organization"; the API stack must share this backend
    var apiStack = new StackReference("organization/weather-api-infra/dev");
    var apiUrl = apiStack.GetOutput("url");

    var bucket = new Bucket("weather-web-bucket", new BucketArgs
    {
        ForceDestroy = true,
    });

    var website = new BucketWebsiteConfiguration("weather-web-website", new BucketWebsiteConfigurationArgs
    {
        Bucket = bucket.Id,
        IndexDocument = new BucketWebsiteConfigurationIndexDocumentArgs { Suffix = "index.html" },
        // Serve the SPA for unknown paths so client-side routes resolve
        ErrorDocument = new BucketWebsiteConfigurationErrorDocumentArgs { Key = "index.html" },
    });

    // New buckets block public access and disable ACLs, so allow a public bucket policy instead
    var publicAccess = new BucketPublicAccessBlock("weather-web-public-access", new BucketPublicAccessBlockArgs
    {
        Bucket = bucket.Id,
        BlockPublicAcls = true,
        IgnorePublicAcls = true,
        BlockPublicPolicy = false,
        RestrictPublicBuckets = false,
    });

    new BucketPolicy("weather-web-public-read", new BucketPolicyArgs
    {
        Bucket = bucket.Id,
        Policy = bucket.Arn.Apply(arn => JsonSerializer.Serialize(new
        {
            Version = "2012-10-17",
            Statement = new[]
            {
                new
                {
                    Effect = "Allow",
                    Principal = "*",
                    Action = "s3:GetObject",
                    Resource = $"{arn}/*",
                },
            },
        })),
    }, new CustomResourceOptions { DependsOn = { publicAccess } });

    return new Dictionary<string, object?>
    {
        ["apiUrl"] = apiUrl,
        ["bucketName"] = bucket.Id,
        ["websiteUrl"] = website.WebsiteEndpoint.Apply(endpoint => $"http://{endpoint}"),
        ["bucketArn"] = bucket.Arn,
    };
});
