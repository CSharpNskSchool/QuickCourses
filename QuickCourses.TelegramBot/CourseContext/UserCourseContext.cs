using QuickCourses.TelegramBot.Models;
using Telegram.Bot.Types;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using QuickCourses.Api.Models.Interaction;

namespace QuickCourses.TelegramBot.CourseContext
{
    using Extensions;
    using QuickCourses.Api.Models.Primitives;

    public class UserCourseContext
    {
        private readonly TelegramChat telegramChat;
        private readonly AuthenticatedQuickCoursesClient quickCoursesClient;
        private readonly UserInfo owner;
        private readonly Message message;

        public UserCourseContext(AuthenticatedQuickCoursesClient quickCoursesClient, TelegramChat telegramChat, Message message)
        {
            this.telegramChat = telegramChat;
            this.message = message;
            this.quickCoursesClient = quickCoursesClient;
            owner = telegramChat.Owner;
        }

        public async Task ProcessInput()
        {
            var input = message.Text;
            
            if (IsCommand(input))
            {
                await ProcessCommand(input);
                return;
            }
            //Голобальные состояния
            switch (input)
            {
                case "Меню":
                    await ShowMenu();
                    return;
                case "Содержание" when owner.CurrentAction != UserAction.ChosingCourse:
                    await ShowContent();
                    return;
                default:
                    break;
            }

            switch (owner.CurrentAction)
            {
                case UserAction.ChosingMyCourse:
                case UserAction.ChosingCourse:
                    await ChosingCourse(input);
                    return;
                case UserAction.Answers:
                    await Answers(input);
                    return;
                case UserAction.ReadingLesson:
                    await ReadingLesson(input);
                    return;
                case UserAction.ReadingStep:
                    await ReadingStep(input);
                    return;
                case UserAction.OnTheMenu:
                    await Menu(input);
                    return;
                case UserAction.ChosingContent:
                    await ChosingContent(input);
                    return;
                case UserAction.ChosingLessonContent:
                    await ChosingLessonContent(input);
                    return;
            }

        }

        #region commands
        private async Task ProcessCommand(string command)
        {
            switch (command)
            {
                case "/start":
                    await telegramChat.SendMessage("Добро пожаловать в QuickCourses!\n");
                    await ShowMenu();
                    break;

                default:
                    await telegramChat.SendMessage("Bad command");
                    break;
            }
        }
        #endregion
        #region  States
        private async Task ChosingContent(string input)
        {
            if(!int.TryParse(input.Split('.')[0], out var number) || number == 0)
            {
                await telegramChat.SendMessage("Что ты сказал?");
                return;
            }

            number--;

            owner.CourseState.LessonId = number;
            owner.CurrentAction = UserAction.ChosingLessonContent;
            await ShowLessonContent();
        }

        private async Task ChosingLessonContent(string input)
        {
            if (!int.TryParse(input.Split('.')[0], out var number) || number == 0)
            {
                await telegramChat.SendMessage("Что ты сказал?");
                return;
            }

            number--;

            owner.CourseState.StepId = number;
            var course = await quickCoursesClient.GetCourseAsync(owner.CourseState.CourseId);

            if (number == 0)
            {
                owner.CurrentAction = UserAction.ReadingStep;
                await ShowCurrentStep(course);
                return;
            }
            
            owner.CurrentAction = UserAction.ReadingLesson;
            await ShowCurrentLesson(course);
        }

        private async Task ChosingCourse(string input)
        {
            if (input == UserKeyboards.NextButton)
            {
                return;
            }
            if (input == UserKeyboards.BackButton)
            {
                return;
            }

            var courses = await quickCoursesClient.GetCoursesAsync();
            var course = courses.FirstOrDefault(x => x.Description.Name == input);

            if (course == null)
            {
                await telegramChat.SendMessage("Не верное название курса!");
                return;
            };

            owner.CourseState = new CourseState
            {
                CourseId = course.Id,
                LessonId = 0,
                QuestionId = 0,
                StepId = 0
            };
            owner.CurrentAction = UserAction.ReadingLesson;

            var progress = await quickCoursesClient.GetProgressAsync(owner.Id);
            var courseProgress = progress.FirstOrDefault(x => x.CourceId == course.Id);

            if (courseProgress == null)
            {
                owner.CourseState.ProgressId = (await quickCoursesClient.StartCourseAsync(course.Id, owner.Id)).Id;
                await telegramChat.SendMessage("Курс начат!");
            }
            else
            {
                owner.CourseState.ProgressId = courseProgress.Id;
                await telegramChat.SendMessage("Курс возобнавлен!");
            }

            await ShowCurrentLesson(course);
        }

        private async Task ReadingLesson(string input)
        {
            if (input != UserKeyboards.NextButton && input != UserKeyboards.BackButton)
            {
                return;
            }

            var course = await quickCoursesClient.GetCourseAsync(owner.CourseState.CourseId);

            if (input == UserKeyboards.BackButton)
            {
                await Backward(course);
                return;
            }

            owner.CurrentAction = UserAction.ReadingStep;
            await ShowCurrentStep(course);
        }
        private async Task ReadingStep(string input)
        {
            if (input != UserKeyboards.BackButton && input != UserKeyboards.NextButton)
            {
                return;
            }

            var course = await quickCoursesClient.GetCourseAsync(owner.CourseState.CourseId);

            if (input == UserKeyboards.BackButton)
            {
                await Backward(course);
                return;
            }

            owner.CurrentAction = UserAction.Answers;
            await ShowCurrentQuestion(course);
        }

        private async Task Answers(string input)
        {
            var course = await quickCoursesClient.GetCourseAsync(owner.CourseState.CourseId);

            if (input == UserKeyboards.BackButton)
            {
                await Backward(course);
                return;
            }

            if (input == UserKeyboards.NextButton)
            {
                await ForwardAfterQuestion(course);
                return;
            }

            if (!await GiveAnswer(course, input))
            {
                await telegramChat.SendMessage("Что ты сказал?");
                return;
            }

            await ForwardAfterQuestion(course);
        }
        private async Task Menu(string input)
        {
            switch (input)
            {
                case UserKeyboards.CoursesButton:
                    await ShowActualCourses();
                    return;
                case UserKeyboards.MyCourseButton:
                    await ShowMyCourses();
                    return;
                case UserKeyboards.StatisticsButton:
                    await ShowStats();
                    return;

            }

            await telegramChat.SendMessage("Повтори, что ты сказал?");
        }
        #endregion
        #region Good code
        private async Task ShowStats()
        {
            await telegramChat.SendMessage("*Статистика:*");
            var allProgress = await quickCoursesClient.GetProgressAsync(owner.Id);
            
            foreach(var progress in allProgress)
            {
                var description = (await quickCoursesClient.GetCourseAsync(progress.CourceId)).Description;
                await telegramChat.SendMessage($"Курс _{description.Name}_");
                await telegramChat.SendMessage(progress.Statistics.ConvertToString());
            }
        }
        private async Task Backward(Course course)
        {
            var userState = owner.CourseState;

            if (owner.CurrentAction == UserAction.Answers)
            {
                if (userState.QuestionId - 1 >= 0)
                {
                    userState.QuestionId--;
                    await ShowCurrentQuestion(course);
                    return;
                }

                owner.CurrentAction = UserAction.ReadingStep;
                await ShowCurrentStep(course);
            }
            else if (owner.CurrentAction == UserAction.ReadingStep)
            {
                if (userState.StepId - 1 >= 0)
                {
                    userState.StepId--;
                    userState.QuestionId = course.Lessons[userState.LessonId]
                        .Steps[userState.StepId].Questions.Count() - 1;

                    if (userState.QuestionId == -1)
                    {
                        userState.QuestionId = 0;
                        owner.CurrentAction = UserAction.ReadingStep;
                        await ShowCurrentStep(course);
                    }
                    else
                    {
                        owner.CurrentAction = UserAction.Answers;
                        await ShowCurrentQuestion(course);
                    }

                    return;
                }

                owner.CurrentAction = UserAction.ReadingLesson;
                await ShowCurrentLesson(course);
            }
            else if (owner.CurrentAction == UserAction.ReadingLesson)
            {
                if (userState.LessonId == 0)
                {
                    return;
                }

                userState.LessonId--;
                var stepCount = course.Lessons[userState.LessonId].Steps.Count();

                if (stepCount == 0)
                {
                    userState.QuestionId = 0;
                    userState.StepId = 0;
                    await ShowCurrentLesson(course);
                    return;
                }

                var stepId = stepCount - 1;
                var questionCount = course.Lessons[userState.LessonId].Steps[stepId].Questions.Count;

                if (questionCount == 0)
                {
                    userState.QuestionId = 0;
                    userState.StepId = stepId;
                    owner.CurrentAction = UserAction.ReadingStep;
                    await ShowCurrentStep(course);
                    return;
                }

                var questionId = questionCount - 1;

                userState.StepId = stepId;
                userState.QuestionId = questionId;
                owner.CurrentAction = UserAction.Answers;
                await ShowCurrentQuestion(course);
            }
        }
        private async Task ForwardAfterQuestion(Course course)
        {
            var userState = owner.CourseState;

            if (course.Lessons[userState.LessonId].Steps[userState.StepId].Questions.Count > userState.QuestionId + 1)
            {
                userState.QuestionId++;
                owner.CurrentAction = UserAction.Answers;
                await ShowCurrentQuestion(course);
            }
            else if (course.Lessons[userState.LessonId].Steps.Count > userState.StepId + 1)
            {
                userState.QuestionId = 0;
                userState.StepId++;
                owner.CurrentAction = UserAction.ReadingStep;
                await ShowCurrentStep(course);
            }
            else if (course.Lessons.Count > userState.LessonId + 1)
            {
                owner.CourseState.QuestionId = 0;
                owner.CourseState.StepId = 0;
                owner.CourseState.LessonId++;
                owner.CurrentAction = UserAction.ReadingLesson;
                await ShowCurrentLesson(course);
            }
            else
            {
                owner.CourseState.QuestionId = 0;
                owner.CourseState.StepId = 0;
                owner.CourseState.LessonId = 0;

                var progress = quickCoursesClient.GetCourseProgressAsync(owner.CourseState.ProgressId);

                await telegramChat.SendMessage("Курс пройден!");
                await ShowMenu();
            }
        }
        private async Task ShowMenu()
        {
            owner.CurrentAction = UserAction.OnTheMenu;
            await telegramChat.SendMessage("Что вам нужно?\n", UserKeyboards.MenuKeyboard());
        }
        private async Task ShowActualCourses()
        {
            var courses = await quickCoursesClient.GetCoursesAsync();
            var progress = await quickCoursesClient.GetProgressAsync(owner.Id);
            
            if (progress.Any())
            {
                courses = courses.Where(x => progress.All(y => y.CourceId != x.Id));
            }

            await telegramChat.SendMessage("Выберите курс", UserKeyboards.ChoseKeyboard(courses.Select(x => x.Description.Name)));
            owner.CurrentAction = UserAction.ChosingCourse;
        }
        private async Task ShowMyCourses()
        {
            var courses = await quickCoursesClient.GetCoursesAsync();
            var progress = await quickCoursesClient.GetProgressAsync(owner.Id);
            courses = courses.Where(x => progress.Any(y => y.CourceId == x.Id));

            await telegramChat.SendMessage("Выберите курс", UserKeyboards.ChoseKeyboard(courses.Select(x => x.Description.Name)));
            owner.CurrentAction = UserAction.ChosingMyCourse;
        }
        private async Task ShowCurrentLesson(Course course)
        {
            var currentLesson = course.Lessons[owner.CourseState.LessonId];
            await telegramChat.SendMessage($"*{currentLesson.Description.Name}*\n{currentLesson.Description.Overview}", UserKeyboards.DefaultKeyboard());
        }
        private async Task ShowCurrentStep(Course course)
        {
            if (course.Lessons[owner.CourseState.LessonId].Steps.Count == 0)
            {
                return;
            }

            var currentStep = course.Lessons[owner.CourseState.LessonId].Steps[owner.CourseState.StepId];
            await telegramChat.SendMessage($"*{currentStep.EducationalMaterial.Description.Name}*\n_{currentStep.EducationalMaterial.Description.Overview}_\n{currentStep.EducationalMaterial.Article}", UserKeyboards.DefaultKeyboard());
        }
        private async Task ShowCurrentQuestion(Course course)
        {
            var progress = await quickCoursesClient.GetCourseProgressAsync(owner.CourseState.ProgressId);
            var qustionState = progress
                .LessonProgresses[owner.CourseState.LessonId]
                .StepProgresses[owner.CourseState.StepId]
                .QuestionStates[owner.CourseState.QuestionId];

            if (course.Lessons[owner.CourseState.LessonId].Steps[owner.CourseState.StepId].Questions.Count == 0)
            {
                return;
            }
            
            var question = course
                .Lessons[owner.CourseState.LessonId]
                .Steps[owner.CourseState.StepId]
                .Questions[owner.CourseState.QuestionId];

            if (qustionState.Passed || qustionState.CurrentAttemptsCount == 2)
            {
                await telegramChat.SendMessage(
                    $"{question.Text}\nПравильный ответ: *{question.AnswerVariants[question.CorrectAnswers.First()].Text}*",
                    UserKeyboards.DefaultKeyboard());
                return;
            }

            owner.CurrentAction = UserAction.Answers;
            await telegramChat.SendMessage($"*{question.Text}*", UserKeyboards.DefaultKeyboard(question.AnswerVariants.Select((x, i) => $"{(char)(x.Id + 'a')}")));
            var questionsText = string.Join('\n', question.AnswerVariants.Select(x => $"*{(char)(x.Id + 'a')})* {x.Text}"));

            await telegramChat.SendMessage(questionsText);
        }
        private async Task<bool> GiveAnswer(Course course, string input)
        {
            var question = course
                .Lessons[owner.CourseState.LessonId]
                .Steps[owner.CourseState.StepId]
                .Questions[owner.CourseState.QuestionId];

            if (string.IsNullOrEmpty(input) || !char.IsLetter(input[0]))
            {
                return false;
            }

            var index = char.ToLower(input[0]) - 'a';

            if (index < 0 || index >= question.AnswerVariants.Count)
            {
                return false;
            }

            var result = await quickCoursesClient.SendAnswerAsync(
                owner.CourseState.ProgressId,
                owner.CourseState.LessonId,
                owner.CourseState.StepId,
                new Answer
                {
                    SelectedAnswers = new List<int> { index }, 
                    QuestionId = owner.CourseState.QuestionId
                });

            var message = string.Empty;

            if (result.Passed)
            {
                message = "Вы ответили верно!";
            }
            else
            {
                message = "Вы ошиблись!";
            }

            await telegramChat.SendMessage(message, UserKeyboards.DefaultKeyboard());

            return true;
        }
        private async Task ShowContent()
        {
            if (owner.CourseState.CourseId == string.Empty)
            {
                return;
            }

            var course = await quickCoursesClient.GetCourseAsync(owner.CourseState.CourseId);
            var content = course.Lessons.Select((x, i) => $"{x.Id + 1}. {x.Description.Name}");
        
            owner.CurrentAction = UserAction.ChosingContent;

            await telegramChat.SendMessage("Выберите урок", UserKeyboards.ChoseKeyboard(content));
        }
        private async Task ShowLessonContent()
        {
            var course = await quickCoursesClient.GetCourseAsync(owner.CourseState.CourseId);
            var content = course.Lessons[owner.CourseState.LessonId].Steps.Select((x, i) => $"{x.Id + 1}. {x.EducationalMaterial.Description.Name}");

            owner.CurrentAction = UserAction.ChosingLessonContent;

            await telegramChat.SendMessage("Выберите шаг", UserKeyboards.ChoseKeyboard(content));
        }
        #endregion

        private bool IsCommand(string input)
        {
            return !string.IsNullOrEmpty(input) && input[0] == '/';
        }
    }
}
