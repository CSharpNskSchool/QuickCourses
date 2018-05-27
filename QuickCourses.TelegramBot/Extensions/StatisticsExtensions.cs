using QuickCourses.Api.Models.Progress;
using System;

namespace QuickCourses.TelegramBot.Extensions
{
    public static class StatisticsExtensions
    {
        public static string ConvertToString(this Statistics statistics)
        {
            if (statistics == null)
            {
                throw new ArgumentNullException(nameof(statistics));
            }

            return $"Общее число вопросов: *{statistics.TotalQuestionsCount}*\nПравильных ответов: *{statistics.PassedQuestionsCount}*\nКурс пройден: *{ConvertToString(statistics.Passed)}*";
        }

        private static string ConvertToString(bool flag)
        {
            return flag ? "Да" : "Нет";
        }
    }
}
