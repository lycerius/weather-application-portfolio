# Weather API Infrastructure

Pulumi (C#) program that runs the `weather-api` container on AWS:

- An Application Load Balancer that health-checks `/health` on port 8080
- An ECS cluster and Fargate service (1 task, 256 CPU / 512 MB) running the API image

Normally this is deployed by the [Deploy Weather Application](../.github/workflows/deploy.yml) workflow on every push to `main`. Use the steps below only when you need to run `pulumi up` by hand, for example to debug a failed deploy.

## Prerequisites

The dev container already includes all of these:

- .NET 10 SDK
- Pulumi CLI
- AWS CLI v2

## Required secrets and access

| What | Why | Where to get it |
| ---- | --- | --------------- |
| `PULUMI_CONFIG_PASSPHRASE` | Decrypts the `dev` stack's secrets. It must match the passphrase the stack was created with. | Same value as the `PULUMI_CONFIG_PASSPHRASE` GitHub Actions secret |
| AWS credentials for account `595266861432` | Reading and writing Pulumi state, and creating the resources | Your own IAM user or SSO profile (see below) |

### AWS permissions

CI assumes `arn:aws:iam::595266861432:role/GithubWorkerRole` through GitHub OIDC. You can assume that role locally only if its trust policy allows your IAM principal. Otherwise, use an identity with equivalent permissions:

- **S3**: read and write `s3://pulimi-state-595266861432-us-east-1-an` (the Pulumi state backend)
- **ECS**: manage clusters, services, and task definitions
- **Elastic Load Balancing**: manage load balancers, target groups, and listeners
- **EC2**: describe the default VPC and subnets, and manage security groups
- **IAM**: create the task execution role, and `iam:PassRole` on it
- **CloudWatch Logs**: create the service's log group
- **ECR**: read the `weather-api` repository (to look up image digests)
- **S3** (frontend stack): create the site bucket, and manage its website configuration, public access block, and bucket policy

Before you start, check which identity you're using:

```bash
aws sts get-caller-identity
```

## Running `pulumi up` locally

> This is the same `dev` stack that CI deploys. Anything you change here is overwritten by the next deploy from `main`. Don't run it while a workflow deploy is in progress.

```bash
cd infra

export AWS_REGION=us-east-1
export PULUMI_CONFIG_PASSPHRASE='<value of the GitHub secret>'

pulumi login s3://pulimi-state-595266861432-us-east-1-an/weather-infra/
pulumi stack select dev
```

The program requires an `image` config value: a fully qualified image reference that is already in ECR. To redeploy the image that is currently running:

```bash
pulumi config set image "$(pulumi stack output image)"
```

To deploy a specific version instead:

```bash
VERSION=1.0.1   # without the leading "v"
REGISTRY=595266861432.dkr.ecr.us-east-1.amazonaws.com
DIGEST=$(aws ecr describe-images --repository-name weather-api \
  --image-ids imageTag=$VERSION --query 'imageDetails[0].imageDigest' --output text)

pulumi config set image "$REGISTRY/weather-api:$VERSION@$DIGEST"
```

Then preview and apply:

```bash
pulumi preview --diff
pulumi up
```

`pulumi config set` writes the image into `Pulumi.dev.yaml`. Don't commit that change, because CI sets the image on every deploy. Discard it with:

```bash
git checkout Pulumi.dev.yaml
```

## Outputs

| Output | Description |
| ------ | ----------- |
| `url` | Public URL of the load balancer, e.g. `http://weather-api-lb-….elb.amazonaws.com` |
| `image` | Image reference currently deployed |

```bash
pulumi stack output url
```

## Frontend stack

The static site lives in [`../infra-web`](../infra-web). It uses the same passphrase, AWS access, and state backend. The shared backend is required, because the web stack reads this stack's `url` output through a `StackReference`. Deploy the API stack first.

```bash
cd infra-web
pulumi login s3://pulimi-state-595266861432-us-east-1-an/weather-infra/
pulumi stack select dev --create
pulumi up

# Upload the built frontend
(cd ../weather-web && npm ci && npm run build)
aws s3 sync ../weather-web/dist "s3://$(pulumi stack output bucketName)" --delete
```

The bucket is public through a bucket policy, so the account-level S3 Block Public Access setting must allow public bucket policies.
