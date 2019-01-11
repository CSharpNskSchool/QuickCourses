using Microsoft.AspNetCore.Mvc;
using QuickCourses.TelegramBot.CourseContext;
using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace QuickCourses.TelegramBot.Controllers
{
    [Produces("application/json")]
    [Route("api/v0/bot")]
    public class TelegramBotController : Controller
    {
        private static readonly StringBuilder awesomeLog = new StringBuilder();
        private readonly UserCourseContextManager userCourseContextManager;

        public TelegramBotController(UserCourseContextManager userCourseContextManager)
        {
            this.userCourseContextManager = userCourseContextManager;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            return awesomeLog.ToString();
        }

        [HttpPost]
        [RequireHttps]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            if (update == null)
            {
                return BadRequest();
            }

            if (update.CallbackQuery != null)
            {
            }

            if (update.Message != null)
            {
                try
                {
                   await userCourseContextManager.ProcessUserMessage(update.Message);
                }
                catch(Exception ex)
                {
                    awesomeLog
                        .AppendLine(ex.Message)
                        .AppendLine()
                        .AppendLine(ex.StackTrace);
                }
            }

            return Ok(); 
        }
    }
}
