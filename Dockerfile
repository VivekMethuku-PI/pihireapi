FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["PiHire.API/PiHire.API.csproj", "PiHire.API/"]
RUN dotnet restore "PiHire.API/PiHire.API.csproj"
COPY . .
WORKDIR "/src/PiHire.API"
RUN dotnet build "PiHire.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "PiHire.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ["PiHire.API/TemplateGallery", "/app/TemplateGallery"]
COPY ["PiHire.API/Candidate", "/app/Candidate"]
COPY ["PiHire.API/Employee", "/app/Employee"]
COPY ["PiHire.API/Testimonals", "/app/Testimonals"]
COPY ["PiHire.API/Blogs", "/app/Blogs"]
COPY ["PiHire.API/EmailTemplates", "/app/EmailTemplates"]



ENTRYPOINT ["dotnet", "PiHire.API.dll"]
