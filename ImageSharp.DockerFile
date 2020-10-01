FROM mcr.microsoft.com/dotnet/sdk:5.0.100-rc.1
MAINTAINER Brian Popow <popow@gmx.de>

ENV TERM xterm

RUN apt-get update
RUN apt-get -y install libgdiplus libgif-dev autoconf libtool automake build-essential gettext libglib2.0-dev libcairo2-dev libtiff-dev libexif-dev

COPY . /app/

WORKDIR /app/tests/Issue1356/
RUN dotnet restore
CMD ["dotnet", "run"]

# uncomment this to execute the tests
# WORKDIR /app/tests/ImageSharp.Tests/
# CMD ["dotnet", "test", "-c", "Release", "-f", "netcoreapp5.0"]