using QuickCourses.TelegramBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCourses.TelegramBot.CourseContext
{
    public class TelegramChat
    {
        private readonly ITelegramBotClient telegramBotClient;

        public TelegramChat(ITelegramBotClient telegramBotClient, UserInfo owner, long chatId)
        {
            Owner = owner;
            ChatId = chatId;
            this.telegramBotClient = telegramBotClient;
        }

        public long ChatId { get; }
        public UserInfo Owner  {get; }

        public async Task SendMessage(string message, string[][] buttons = null)
        {
            var keyboard = buttons == null ? null : CreateKeyboard(buttons);
            await telegramBotClient.SendTextMessageAsync(ChatId, message, ParseMode.Markdown, replyMarkup: keyboard);
        }
        
        private ReplyKeyboardMarkup CreateKeyboard(string[][] buttons)
        {
            var keyboardButtons = buttons
                .Where(x=>x!= null)
                .Select(x => x.Select(y => new KeyboardButton(y)).ToArray()).ToArray();
            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }
    }
}
