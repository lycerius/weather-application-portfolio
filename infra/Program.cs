using System.Collections.Generic;
using Pulumi;
using Pulumi.Aws.Ecs;
using Pulumi.Awsx.Ecs;
using Pulumi.Awsx.Ecs.Inputs;
using Pulumi.Awsx.Lb;

return await Deployment.RunAsync(() => 
{
    var lb = new ApplicationLoadBalancer("nginx-lb");
    var cluster = new Cluster("nginx-cluster");


    // Define the Fargate Service using the official nginx image
    var service = new FargateService("nginx-service", new FargateServiceArgs
    {
        AssignPublicIp = true,
        Cluster = cluster.Arn,
        TaskDefinitionArgs = new Pulumi.Awsx.Ecs.Inputs.FargateServiceTaskDefinitionArgs
        {
            Container = new Pulumi.Awsx.Ecs.Inputs.TaskDefinitionContainerDefinitionArgs
            {
                Name = "nginx",
                Image = "nginx:latest", // Pulls directly from Docker Hub
                Memory = 512,
                Cpu = 256,
                PortMappings = new List<TaskDefinitionPortMappingArgs>
                {
                    new TaskDefinitionPortMappingArgs
                    {
                        ContainerPort = 80,
                        TargetGroup = lb.DefaultTargetGroup
                    }
                }
            }
        }
    });
});

