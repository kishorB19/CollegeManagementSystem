FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CollegeManagementSystem.csproj", "."]
RUN dotnet restore "CollegeManagementSystem.csproj"
COPY . .
RUN dotnet build "CollegeManagementSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CollegeManagementSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
CMD ["sh", "-c", "dotnet CollegeManagementSystem.dll --urls http://0.0.0.0:${PORT:-80}"]
