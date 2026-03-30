using App.Config;
using App.Controllers;
using App.Data;
using App.Models;
using App.Security;
using App.Server;
using App.Services;
using App.Views;

namespace App;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var settings = AppSet.LoadFromEnv();

            var connectionFactory = new DbConnection(settings);
            var dbInit = new DbInit(connectionFactory);
            await dbInit.InitAsync();

            Console.WriteLine("DB init es perfecto");
            Console.WriteLine("Start HTTP server:");

            var userRepo = new UserRepo(connectionFactory);
            var sessionRepository = new SessionRepository(connectionFactory);

            var validationService = new ValidationService();
            var passwordHasher = new PasswordHasher();
            var captchaService = new CaptchaService();
            var htmlRenderer = new HtmlRenderer();

            var sessionServices = new SessionServices(sessionRepository, userRepo);
            var authService = new AuthService(
                userRepo,
                validationService,
                passwordHasher,
                captchaService,
                sessionServices);

            var authController = new AuthController(authService, captchaService, htmlRenderer);
            var profileController = new ProfileController(
                sessionServices,
                userRepo,
                validationService,
                passwordHasher,
                htmlRenderer);

            var router = new Router(authController, profileController, sessionServices, htmlRenderer);
            var server = new SimpleHttpServer(settings.AppPort, router);

            await server.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Startup failed:");
            Console.WriteLine(ex);
        }
    }
}