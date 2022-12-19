using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TypesUsedByBot;

namespace VkBot
{
    // T - тип данных для Id чата
    class BotCommands<T>
    {
        private readonly Bot<T> bot;
        private readonly Dictionary<T, UserTimers<T>> timersByChatIdDict;

        public Dictionary<string, Func<MessageParams<T>, Task>> CommandByMsgDict { get; }

        public BotCommands(Bot<T> bot)
        {
            this.bot = bot;

            timersByChatIdDict = new Dictionary<T, UserTimers<T>>();

            CommandByMsgDict = new Dictionary<string, Func<MessageParams<T>, Task>>()
            {
                [".help"] = HelpCommandAsync,
                [".start"] = StartCommandAsync,
                [".stop"] = StopCommandAsync,
                [".new"] = NewTimetableCommandAsync,
                [".chg"] = ChangeDayTimetableCommandAsync
            };
        }

        private void HelpCommand(MessageParams<T> message)
            => bot.MessangerApi.SendTextMessage(
                message.ChatId,
                string.Join("\r\n", new string[] {
                        "Команды бота:",
                        ".help - рассказать про все команды бота",
                        ".new - сохранить ваше расписание (вместе с этой командой нужно передать .txt файл вашего расписания)",
                        ".start - подписаться на рассылку уведомлений",
                        ".stop - отписаться от рассылки уведомлений",
                        ".chg - изменить что-то в рсаписании"
                }));

        private async Task HelpCommandAsync(MessageParams<T> message) => await Task.Run(() => HelpCommand(message));

        private void StartCommand(MessageParams<T> message)
        {
            if (timersByChatIdDict.ContainsKey(message.ChatId))
                bot.MessangerApi.SendTextMessage(message.ChatId, "Вы уже подписаны на рассылку расписания");
            else
            {
                UserTimers<T> userTimers = new UserTimers<T>(bot, message.ChatId);

                userTimers.StartTimerWorkday();

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

                bot.RepositoryApi.NewTimetable(message.ChatId, ParserTxt.ParseIntoTimetable(File.ReadAllLines(documents[0].Title)));

                File.Delete(documents[0].Title);

                bot.MessangerApi.SendTextMessage(message.ChatId, "Добавлено новое рассписание");
            }
        }

        private async Task NewTimetableCommandAsync(MessageParams<T> message) => await Task.Run(() => NewTimetableCommand(message));

        private void DownloadDocument(DocumentParams document)
        {
            using WebClient client = new WebClient();

            client.DownloadFile(document.Uri, document.Title);
        }

        private void ChangeDayTimetableCommand(MessageParams<T> message)
        {
            // TODO: Написать реализацию метода
        }

        private async Task ChangeDayTimetableCommandAsync(MessageParams<T> message) => await Task.Run(() => ChangeDayTimetableCommand(message));
    }
}
