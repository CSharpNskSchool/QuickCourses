using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.TelegramBot.Models;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace QuickCourses.TelegramBot.CourseContext
{
    public class UserCourseContextManager
    {
        private readonly AuthenticatedQuickCoursesClient quickCoursesClient;
        private readonly ITelegramBotClient telegramBotClient;
        private readonly IRepository<UserInfo> userRepository;

        public UserCourseContextManager(
            AuthenticatedQuickCoursesClient quickCoursesClient,
            ITelegramBotClient telegramBotClient,
            IRepository<UserInfo> userRepository)
        {
            this.quickCoursesClient = quickCoursesClient;
            this.telegramBotClient = telegramBotClient;
            this.userRepository = userRepository;
        }

        public async Task ProcessUserMessage(Message message)
        {
            var telegramId = message.From.Id;
            var userId = await GetUserId(telegramId);
            if (!await userRepository.ContainsAsync(userId))
            {
                await userRepository.InsertAsync(new UserInfo
                {
                    TelegramId = telegramId,
                    Id = userId, CurrentAction = UserAction.OnTheMenu,
                    CourseState = new CourseState()
                });
            }
            var userInfo = await userRepository.GetAsync(userId);
            var userChat = new TelegramChat(telegramBotClient, userInfo, message.Chat.Id);
            var userContext = new UserCourseContext(quickCoursesClient, userChat, message);
            await userContext.ProcessInput();
            await userRepository.ReplaceAsync(userId, userInfo);
        }

        private async Task<string> GetUserId(int telegramId)
        {
            try
            {
                return await quickCoursesClient.GetIdByLoginAsync(telegramId.ToString());
            }
            catch
            {
                await quickCoursesClient.RegisterAsync(new Api.Models.Authentication.RegistrationInfo { Login = telegramId.ToString() });
                return await quickCoursesClient.GetIdByLoginAsync(telegramId.ToString());
            }
        }
    }
}
