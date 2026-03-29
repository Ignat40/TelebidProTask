namespace App.Config;

public class AppSet
{
    public string DbHost { get; set; } = "localhost";
    public int DbPort { get; init; } = 5432;
    public string DbName { get; init; } = "registrationdb";
    public string DbUser { get; init; } = "appuser";
    public string DbPassword { get; init; } = "apppassword";
    public int AppPort { get; init; } = 8000;

    public string GetConnectionString()
    {
        return $"Host={DbHost}; Port={DbPort}; Database={DbName}; Username={DbUser}; Password={DbPassword}";
    }

    public static AppSet LoadFromEnv()
    {
        return new AppSet
        {
            DbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost",
            DbPort = int.TryParse(Environment.GetEnvironmentVariable("DB_PORT"), out var dbPort) ? dbPort : 5432,
            DbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "registrationdb",
            DbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "appuser",
            DbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "apppassword",
            AppPort = int.TryParse(Environment.GetEnvironmentVariable("APP_PORT"), out var appPort) ? appPort : 8080

        };
    }
}



