﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TypesUsedByBot;
using System.Text;

namespace VkBot
{
    // T - тип данных для Id чата
    class BotCommands<T>
    {
        private readonly Bot<T> bot;
        private readonly Dictionary<T, UserTimers<T>> timersByChatIdDict;
        private readonly Dictionary<T, bool> haveTimetableByChatIdDict;

        public Dictionary<string, Func<MessageParams<T>, Task>> CommandByMsgDict { get; }

        public BotCommands(Bot<T> bot)
        {
            this.bot = bot;

            timersByChatIdDict = new Dictionary<T, UserTimers<T>>();

            haveTimetableByChatIdDict = new Dictionary<T, bool>();

            CommandByMsgDict = new Dictionary<string, Func<MessageParams<T>, Task>>()
            {
                [".help"] = HelpCommandAsync,
                [".start"] = StartCommandAsync,
                [".stop"] = StopCommandAsync,
                [".new"] = NewTimetableCommandAsync,
                [".add"] = AddLessonCommandAsync,
                [".del"] = DeleteLessonCommandAsync,
                [".today"] = TodayCommandAsync,
                [".example"] = ExampleCommandAsync
            };
        }

        private void HelpCommand(MessageParams<T> message)
            => bot.MessangerApi.SendTextMessage(
                message.ChatId,
                string.Join("\r\n", new string[] {
                        "Команды бота:",
                        ".help - рассказать про все команды бота",
                        ".new - сохранить ваше расписание (вместе с этой командой нужно передать .txt файл вашего расписания, как в .example)",
                        ".start - подписаться на рассылку уведомлений",
                        ".stop - отписаться от рассылки уведомлений",
                        ".add - добавить занятие в расписание",
                        ".del - удаляет занятие в расписании",
                        ".today - вывести расписание на сегодня",
                        ".example - пример расписания"
                }));

        private async Task HelpCommandAsync(MessageParams<T> message) => await Task.Run(() => HelpCommand(message));

        private void ExampleCommand(MessageParams<T> message)
            => bot.MessangerApi.SendTextMessage(
                message.ChatId,
                string.Join("\r\n", new string[] {
                        "- Пример расписания:",
                        "ИБ-3",
                        "Числитель",
                        "Пн",
                        "9:00-Матстат-Гринев-414",
                        "Ср",
                        "13:00-Схематехника-Гвоздарев-Физфак 14каб",
                        "14:30-Схематехника-Гвоздарев-Физфак 14каб",
                        "Знаменатель",
                        "Сб",
                        "9:00-ОС-Савинов-412",
                        "10:45-ОС-Савинов-412",
                        "- Пример добавления занятия:",
                        ".add числитель Пн 15:00-Матстат-Гринев-414",
                        "- Пример удаления занятия:",
                        ".del числитель Пн 15:00"
                }));

        private async Task ExampleCommandAsync(MessageParams<T> message) => await Task.Run(() => ExampleCommand(message));

        private void StartCommand(MessageParams<T> message)
        {
            if (!haveTimetableByChatIdDict.ContainsKey(message.ChatId))
            {
                bot.MessangerApi.SendTextMessage(message.ChatId, "Сначала создайте расписание .new");

                return;
            }

            if (timersByChatIdDict.ContainsKey(message.ChatId))
                bot.MessangerApi.SendTextMessage(message.ChatId, "Вы уже подписаны на рассылку расписания");
            else
            {
                UserTimers<T> userTimers = new UserTimers<T>(bot, message.ChatId);

                userTimers.Start();

                timersByChatIdDict[message.ChatId] = userTimers;

                bot.MessangerApi.SendTextMessage(message.ChatId, "Теперь вы подписаны на рассылку расписания");
            }
        }

        private async Task StartCommandAsync(MessageParams<T> message) => await Task.Run(() => StartCommand(message));

        private void StopCommand(MessageParams<T> message)
        {
            if (timersByChatIdDict.ContainsKey(message.ChatId))
            {
                timersByChatIdDict[message.ChatId].Stop();

                timersByChatIdDict.Remove(message.ChatId);

                bot.MessangerApi.SendTextMessage(message.ChatId, "Вы отказались от рассылки расписания");
            }
            else
                bot.MessangerApi.SendTextMessage(message.ChatId, "Вы не подписаны на рассылку расписания");
        }

        private async Task StopCommandAsync(MessageParams<T> message) => await Task.Run(() => StopCommand(message));

        private void NewTimetableCommand(MessageParams<T> message)
        {
            DocumentParams[] documents = message.ArrayOfLinksToAttachedFiles;

            if (documents.Length > 0 && documents[0].Ext == "txt")
            {
                DownloadDocument(documents[0]);

                try { bot.RepositoryApi.NewTimetable(message.ChatId, ParserTxt.ParseIntoTimetable(EncodingFile(documents[0].Title))); }
                catch (ArgumentException) { bot.MessangerApi.SendTextMessage(message.ChatId, "Расписание не распознано, проверьте правильно ли вы записали расписание (сравните с примером .example)"); }

                File.Delete(documents[0].Title);

                bot.MessangerApi.SendTextMessage(message.ChatId, "Добавлено новое расписание");

                haveTimetableByChatIdDict[message.ChatId] = true;
            }
        }

        private static string[] EncodingFile(string fileName)
        {
            byte[] asciiBytes = File.ReadAllBytes(fileName);

            string text = Encoding.GetEncoding(1251).GetString(asciiBytes);

            return text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private async Task NewTimetableCommandAsync(MessageParams<T> message) => await Task.Run(() => NewTimetableCommand(message));

        private void DownloadDocument(DocumentParams document)
        {
            using WebClient client = new WebClient();

            client.DownloadFile(document.Uri, document.Title);
        }

        private void AddLessonCommand(MessageParams<T> message)
        {
            string[] words = message.Text.Split(' ');

            bool result = bot.RepositoryApi.AddLesson(
                message.ChatId,
                words[1] == "числитель", 
                ParserTxt.ParseIntoDayOfWeek(words[2]) ?? throw new ArgumentException("Некорректные данные команды"), 
                ParserTxt.ParseIntoLesson(words[3]));

            bot.MessangerApi.SendTextMessage(message.ChatId, result ? "Занятие добавлено" : "Данное время занято (или вы ещё не создали расписание)");
        }

        private async Task AddLessonCommandAsync(MessageParams<T> message) => await Task.Run(() => AddLessonCommand(message));

        private void DeleteLessonCommand(MessageParams<T> message)
        {
            string[] words = message.Text.Split(' ');

            bool result = bot.RepositoryApi.DeleteLesson(
                message.ChatId,
                words[1] == "числитель",
                ParserTxt.ParseIntoDayOfWeek(words[2]) ?? throw new ArgumentException("Некорректные данные команды"),
                TimeSpan.Parse(words[3]));

            bot.MessangerApi.SendTextMessage(message.ChatId, result ? "Занятие удалено" : "Занятие не найдено");
        }

        private async Task DeleteLessonCommandAsync(MessageParams<T> message) => await Task.Run(() => DeleteLessonCommand(message));

        private void TodayCommand(MessageParams<T> message)
        {
            Workday workday = bot.RepositoryApi.GetWorkdayForDate(message.ChatId, DateTime.Today);

            bot.MessangerApi.SendTextMessage(message.ChatId, workday == null ? "Сегодня нет занятий" : workday.ToString());
        }

        private async Task TodayCommandAsync(MessageParams<T> message) => await Task.Run(() => TodayCommand(message));
    }
}
