using System.Collections.Generic;
using Pulumi;
using Pulumi.Aws.Ecs;
using Pulumi.Awsx.Ecs;
using Pulumi.Awsx.Ecs.Inputs;
using Pulumi.Awsx.Lb;
using Pulumi.Awsx.Lb.Inputs;
using AwsLbInputs = Pulumi.Aws.LB.Inputs;

// ASP.NET Core 8+ container images listen on 8080 by default
const int ContainerPort = 8080;

return await Deployment.RunAsync(() =>
{
    var config = new Config();

    // Fully-qualified image reference pushed by CI, e.g. <registry>/weather-api@sha256:...
    var image = config.Require("image");

    var lb = new ApplicationLoadBalancer("weather-api-lb", new ApplicationLoadBalancerArgs
    {
        DefaultTargetGroup = new TargetGroupArgs
        {
            Port = ContainerPort,
            Protocol = "HTTP",
            TargetType = "ip",
            HealthCheck = new AwsLbInputs.TargetGroupHealthCheckArgs
            {
                Path = "/health",
                Matcher = "200",
            },
        },
    });

    var cluster = new Cluster("weather-api-cluster");

    var service = new FargateService("weather-api-service", new FargateServiceArgs
    {
        AssignPublicIp = true,
        Cluster = cluster.Arn,
        DesiredCount = 1,
        TaskDefinitionArgs = new FargateServiceTaskDefinitionArgs
        {
            Container = new TaskDefinitionContainerDefinitionArgs
            {
                Name = "weather-api",
                Image = image,
                Memory = 512,
                Cpu = 256,
                Essential = true,
                PortMappings = new List<TaskDefinitionPortMappingArgs>
                {
                    new TaskDefinitionPortMappingArgs
                    {
                        ContainerPort = ContainerPort,
                        TargetGroup = lb.DefaultTargetGroup,
                    },
                },
            },
        },
    });

    return new Dictionary<string, object?>
    {
        // DnsName is itself an Output, so it must be unwrapped rather than interpolated inside Apply
        ["url"] = Output.Format($"http://{lb.LoadBalancer.Apply(lb => lb.DnsName)}"),
        ["image"] = image,
    };
});
