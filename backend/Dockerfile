FROM mcr.microsoft.com/dotnet/sdk:9.0.301 AS development

WORKDIR /app

COPY . .

EXPOSE 5126

CMD ["dotnet", "watch", "run", "--project", "SocialMediaApp.Api/SocialMediaApp.Api.csproj", "--urls", "http://0.0.0.0:5126"]