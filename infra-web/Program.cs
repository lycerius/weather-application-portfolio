using System.Collections.Generic;
using Pulumi;
using Pulumi.Aws.S3;
using Pulumi.Aws.S3.Inputs;

return await Deployment.RunAsync(() =>
{
    var apiStack = new StackReference("weather-api-infra/dev");
    var apiUrl = apiStack.GetOutput("url");

    var bucket = new Bucket("weather-web-bucket", new BucketArgs
    {
        Acl = "public-read",
        ForceDestroy = true,
        Website = new BucketWebsiteArgs
        {
            IndexDocument = "index.html",
            ErrorDocument = "index.html",
        },
    });

    return new Dictionary<string, object?>
    {
        ["apiUrl"] = apiUrl,
        ["bucketName"] = bucket.Id,
        ["websiteUrl"] = bucket.WebsiteEndpoint.Apply(endpoint => $"http://{endpoint}"),
        ["bucketArn"] = bucket.Arn,
    };
});
