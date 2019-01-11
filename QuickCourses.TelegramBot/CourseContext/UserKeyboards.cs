using System.Collections.Generic;

namespace QuickCourses.TelegramBot.CourseContext
{
    public static class UserKeyboards
    {
        public const string NextButton = "Далее";
        public const string BackButton = "Назад";
        public const string MenuButton = "Меню";
        public const string СontentButton = "Содержание";
        public const string StatisticsButton = "Статистика";
        public const string CoursesButton = "Курсы";
        public const string MyCourseButton = "Мои курсы";

        private const int LineWidth = 2;

        public static string[][] ChoseKeyboard(IEnumerable<string> items = null)
        {
            var result = new List<string[]>();
            result.Add(new[] { MenuButton });
            PushLines(items, result);
            return result.ToArray();
        }

        public static string[][] MenuKeyboard()
        {
            return new[]
            {
                new []{CoursesButton, MyCourseButton, StatisticsButton }
            };
        }

        public static string[][] DefaultKeyboard(IEnumerable<string> items = null)
        {
            var result = new List<string[]>();
            result.Add(new[] { MenuButton, СontentButton });
            result.Add(new[] { BackButton, NextButton });
            PushLines(items, result);

            return result.ToArray();
        }

        private static void PushLines(IEnumerable<string> items, List<string[]> container)
        {
            if (items == null)
            {
                return;
            }

            var enumerator = items.GetEnumerator();
            var line = new List<string>();

            while (enumerator.MoveNext())
            {
                if (line.Count == LineWidth)
                {
                    container.Add(line.ToArray());
                    line.Clear();
                }

                line.Add(enumerator.Current);
            }

            container.Add(line.ToArray());
        }
    }
}
