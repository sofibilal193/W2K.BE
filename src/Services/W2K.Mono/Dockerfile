# syntax=docker/dockerfile:1
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
EXPOSE 8080
WORKDIR /app

# Update, install security updates, and configure unattended-upgrades
RUN apt-get update && apt-get upgrade -y \
    && apt-get install -y --no-install-recommends unattended-upgrades \
    && apt-get clean \
    && apt-get autoremove -y \
    && rm -rf /var/lib/apt/lists/* \
    && echo 'Unattended-Upgrade::Allowed-Origins {"${distro_id}:${distro_codename}-security";}' > /etc/apt/apt.conf.d/20auto-upgrades \
    && echo 'APT::Periodic::Update-Package-Lists "1";' >> /etc/apt/apt.conf.d/20auto-upgrades \
    && echo 'APT::Periodic::Download-Upgradeable-Packages "1";' >> /etc/apt/apt.conf.d/20auto-upgrades \
    && echo 'APT::Periodic::AutocleanInterval "7";' >> /etc/apt/apt.conf.d/20auto-upgrades \
    && echo 'APT::Periodic::Unattended-Upgrade "1";' >> /etc/apt/apt.conf.d/20auto-upgrades

# Copy application files
COPY "bin/Release/net10.0/publish/" .

# create non-root user and switch to it
# RUN addgroup --system decisionfi \
#     && adduser --system --ingroup decisionfi decisionfi
RUN groupadd -r decisionfi && useradd -r -g decisionfi decisionfi

USER decisionfi

ENTRYPOINT ["dotnet", "W2K.WebApp.dll"]