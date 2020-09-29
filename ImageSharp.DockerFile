FROM mcr.microsoft.com/dotnet/sdk:5.0.100-rc.1
# FROM mcr.microsoft.com/dotnet/sdk:5.0
MAINTAINER Brian Popow <popow@gmx.de>

ENV TERM xterm

COPY . /app/
WORKDIR /app/tests/Issue1356/
RUN dotnet restore
CMD ["dotnet", "run"]