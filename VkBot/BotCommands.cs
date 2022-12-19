using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using TypesUsedByBot;

namespace VkBot
{
    // T - тип данных для Id чата
    class BotCommands<T>
    {
        private readonly IMessengerApi<T> messengerApi;
        private readonly IRepositoryApi<T> repositoryApi;
        private readonly Dictionary<T, Timer> timerWokdayByIdDict;
        private readonly Dictionary<T, Timer> timerLessonByIdDict;
        // TODO: переделать это в свойство
        public readonly Dictionary<string, Action<MessageParams<T>>> CommandByMsgDict;

        public BotCommands(IMessengerApi<T> messengerApi, IRepositoryApi<T> repositoryApi)
        {
            this.messengerApi = messengerApi;

            this.repositoryApi = repositoryApi;

            timerWokdayByIdDict = new Dictionary<T, Timer>();

            timerLessonByIdDict = new Dictionary<T, Timer>();

            CommandByMsgDict = new Dictionary<string, Action<MessageParams<T>>>()
            {
                [".help"] = HelpCommand,
                [".start"] = StartCommand,
                [".stop"] = StopCommand,
                [".new"] = NewTimetableCommand,
                [".chg"] = ChangeDayTimetableCommand,
            };
        }

        private void HelpCommand(MessageParams<T> message)
            => messengerApi.SendTextMessage(
                message.ChatId,
                string.Join("\r\n", new string[] {
                        "Команды бота:",
                        ".help - рассказать про все команды бота",
                        ".new - сохранить ваше расписание (вместе с этой командой нужно передать .txt файл вашего расписания)",
                        ".start - подписаться на рассылку уведомлений",
                        ".stop - отписаться от рассылки уведомлений",
                        ".chg - изменить что-то в рсаписании"
                }));

        private void StartCommand(MessageParams<T> message)
        {
            if (timerLessonByIdDict.ContainsKey(message.ChatId))
                messengerApi.SendTextMessage(message.ChatId, "Вы уже подписаны на рассылку расписания");
            else
            {
                (Workday workday, DateTime startTimeOfWorkday) = repositoryApi.NextWokrday(message.ChatId);

                Timer timer = new Timer((startTimeOfWorkday - DateTime.Now).TotalMilliseconds);



                (Lesson lesson, DateTime startTimeOfLesson) = repositoryApi.NextLesson(message.ChatId);

                //Timer timer = new Timer((repositoryApi.NextLesson(message.ChatId) - DateTime.Now).TotalMilliseconds);

                //timer.Start();

                //timerByIdDict.Add(message.PeerId, timer);

                //VkApiFacade.SendTextMessege(message.PeerId, "Теперь вы подписаны на рассылку расписания");
            }
        }

        private void StopCommand(MessageParams<T> message)
        {
            //Message message = ParseGroupUpdateIntoMessage(update);

            //if (timerByIdDict.ContainsKey(message.PeerId))
            //{
            //    timerByIdDict[message.PeerId].Stop();

            //    timerByIdDict.Remove(message.PeerId);

            //    VkApiFacade.SendTextMessege(message.PeerId, "Вы отказались от рассылки расписания");
            //}
            //else
            //    VkApiFacade.SendTextMessege(message.PeerId, "Вы не подписаны на рассылку расписания");
        }

        private void NewTimetableCommand(MessageParams<T> message)
        {
            //Message message = ParseGroupUpdateIntoMessage(update);
            //// TODO: Можно ещё и pdf считывать
            //if (message.Attachments.Count == 1 && message.Attachments[0].Instance is Document doc && doc.Ext == "txt")
            //{
            //    DownloadDocumentFromVk(doc);

            //    dataStore.NewTimetable(File.ReadAllLines(doc.Title));

            //    File.Delete(doc.Title);
            //}
        }

        private void ChangeDayTimetableCommand(MessageParams<T> message)
        {
            // TODO: Написать реализацию метода
        }
    }
}
