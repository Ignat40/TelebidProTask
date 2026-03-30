using App.Config;

namespace Tests;

public class AppSetTests
{
    [Fact]
    public void LoadFromEnvironment_UsesDefaults_WhenVarsMissing()
    {
        Environment.SetEnvironmentVariable("DB_HOST", null);
        Environment.SetEnvironmentVariable("DB_PORT", null);
        Environment.SetEnvironmentVariable("DB_NAME", null);
        Environment.SetEnvironmentVariable("DB_USER", null);
        Environment.SetEnvironmentVariable("DB_PASSWORD", null);
        Environment.SetEnvironmentVariable("APP_PORT", null);

        var settings = AppSet.LoadFromEnv();

        Assert.Equal("localhost", settings.DbHost);
        Assert.Equal(5432, settings.DbPort);
        Assert.Equal("registrationdb", settings.DbName);
        Assert.Equal("appuser", settings.DbUser);
        Assert.Equal("apppassword", settings.DbPassword);
        Assert.Equal(8080, settings.AppPort);
    }

    [Fact]
    public void LoadFromEnvironment_UsesEnvironmentValues()
    {
        Environment.SetEnvironmentVariable("DB_HOST", "db");
        Environment.SetEnvironmentVariable("DB_PORT", "5433");
        Environment.SetEnvironmentVariable("DB_NAME", "mydb");
        Environment.SetEnvironmentVariable("DB_USER", "myuser");
        Environment.SetEnvironmentVariable("DB_PASSWORD", "mypass");
        Environment.SetEnvironmentVariable("APP_PORT", "9090");

        var settings = AppSet.LoadFromEnv();

        Assert.Equal("db", settings.DbHost);
        Assert.Equal(5433, settings.DbPort);
        Assert.Equal("mydb", settings.DbName);
        Assert.Equal("myuser", settings.DbUser);
        Assert.Equal("mypass", settings.DbPassword);
        Assert.Equal(9090, settings.AppPort);
    }

    [Fact]
    public void GetConnectionString_ContainsExpectedParts()
    {
        var settings = new AppSet
        {
            DbHost = "db",
            DbPort = 5432,
            DbName = "registrationdb",
            DbUser = "appuser",
            DbPassword = "apppassword"
        };

        var connectionString = settings.GetConnectionString();

        Assert.Contains("Host=db", connectionString);
        Assert.Contains("Port=5432", connectionString);
        Assert.Contains("Database=registrationdb", connectionString);
        Assert.Contains("Username=appuser", connectionString);
    }
}