using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickCourses.Client;
using QuickCourses.Api.Models.Authentication;
using QuickCourses.TelegramBot.CourseContext;
using QuickCourses.TelegramBot.Data;
using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using QuickCourses.Api.Data.Infrastructure;

namespace QuickCourses.TelegramBot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserCourseContextManager(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var botAuthData = new AuthData();
            var settings = new Settings();
            var apiUrl = configuration["QuickCoursesApi:Url"];
            configuration.GetSection("TelegramBot:AuthData")
                         .Bind(botAuthData);

            configuration.GetSection("MongoConnection:Settings").Bind(settings);

            var userRepository = new UserRepository(settings);
            var quickCoursesClient = new AuthenticatedQuickCoursesClient(botAuthData, ApiVersion.V1, apiUrl);
            var telegramBotClient = new TelegramBotClient(configuration["TelegramBot:Token"]);
            var telegramBotUrl = configuration["TelegramBot:Url"] + "/api/v" + configuration["TelegramBot:Version"] + "/bot";
            var test = userRepository.GetAsync("5b0a8d36378f763ef857a8a5");
            telegramBotClient.DeleteWebhookAsync().Wait();
            telegramBotClient.SetWebhookAsync(telegramBotUrl, allowedUpdates: new[] { UpdateType.MessageUpdate, UpdateType.CallbackQueryUpdate }).Wait();

            var userCourseContextManager = new UserCourseContextManager(quickCoursesClient, telegramBotClient, userRepository);
            services.AddSingleton(userCourseContextManager);

            return services;
        }
        
    }
}
