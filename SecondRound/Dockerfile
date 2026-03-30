FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . .
RUN dotnet publish App/App.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app

COPY --from=build /app/out .

ENV DB_HOST=db
ENV DB_PORT=5432
ENV DB_NAME=registrationdb
ENV DB_USER=appuser
ENV DB_PASSWORD=apppassword
ENV APP_PORT=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "App.dll"]