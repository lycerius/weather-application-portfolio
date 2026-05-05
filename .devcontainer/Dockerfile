FROM mcr.microsoft.com/dotnet/sdk:10.0

# Install Node.js and Yarn using package managers
RUN apt-get update && apt-get install -y curl gnupg
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
RUN curl -sL https://dl.yarnpkg.com/debian/pubkey.gpg | gpg --dearmor | tee /usr/share/keyrings/yarnkey.gpg >/dev/null
RUN echo "deb [signed-by=/usr/share/keyrings/yarnkey.gpg] https://dl.yarnpkg.com/debian stable main" | tee /etc/apt/sources.list.d/yarn.list
RUN apt-get update && apt-get install -y nodejs yarn curl unzip ssh
RUN curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip" \
    && unzip awscliv2.zip \
    && ./aws/install \
    && rm -rf awscliv2.zip ./aws

# Install Pulumi
RUN curl -fsSL https://get.pulumi.com | sh
ENV PATH=$PATH:/root/.pulumi/bin

# Clean up
RUN apt-get clean && rm -rf /var/lib/apt/lists/*

#Generate Dev Certs
RUN dotnet dev-certs https