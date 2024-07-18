# 使用 SDK 鏡像進行構建
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製 csproj 並還原相依套件
COPY ["WebData.csproj", "./"]
RUN dotnet restore

# 複製其餘的程式碼並進行構建
COPY . .
RUN dotnet build -c Release -o /app/build

# 發布應用程序
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# 最終階段 - 使用 ASP.NET 執行時鏡像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 設置非 root 用戶
RUN useradd -m myappuser
USER myappuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

ENTRYPOINT ["dotnet", "WebData.dll"]