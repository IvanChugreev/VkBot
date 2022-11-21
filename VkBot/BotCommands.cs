﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Timers;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.GroupUpdate;

namespace VkBot
{
    static class BotCommands
    {
        private static ICommunicationProtocol dataStore;
        private static readonly Dictionary<long?, Timer> timerByIdDict;
        public static readonly Dictionary<string, Action<GroupUpdate>> CommandByMsgDict;

        static BotCommands()
        {
            // TODO: дописать инициализацию dataStore 

            timerByIdDict = new Dictionary<long?, Timer>();

            CommandByMsgDict = new Dictionary<string, Action<GroupUpdate>>()
            {
                [".help"] = Help,
                [".start"] = Start,
                [".stop"] = Stop,
                [".new"] = NewTimetable,
                [".chg"] = ChangeDayTimetable,
            };
        }

        public static void ReactToUpdate(GroupUpdate update)
        {
            // TODO: Дописать реакцию при возникновении ошибки
            if (update.Instance is MessageNew newMessage && 
                CommandByMsgDict.ContainsKey(newMessage.Message.Text.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0]))
                CommandByMsgDict[newMessage.Message.Text](update);
        }

        private static Message ParseGroupUpdateIntoMessage(GroupUpdate update) 
            => ((MessageNew)update.Instance).Message;

        private static void DownloadDocumentFromVk(Document document)
        {
            using WebClient client = new WebClient();

            client.DownloadFile(document.Uri, document.Title);
        }

        private static void Help(GroupUpdate update) =>
            VkApiFacade.SendTextMessege(ParseGroupUpdateIntoMessage(update).ChatId, 
                string.Join("\r\n", new string[] {
                "Команды бота:",
                ".help - рассказать про все команды бота",
                ".new - сохранить ваше расписание (вместе с этой командой нужно передать .txt файл вашего расписания)",
                ".start - подписаться на рассылку уведомлений",
                ".stop - отписаться от рассылки уведомлений",
                ".chg - изменить что-то в рсаписании"}));

        private static void Start(GroupUpdate update)
        {
            Message message = ParseGroupUpdateIntoMessage(update);

            if (timerByIdDict.ContainsKey(message.ChatId))
                VkApiFacade.SendTextMessege(message.ChatId, "Вы уже подписаны на рассылку расписания");
            else
            {
                Timer timer = new Timer((dataStore.NextLesson(message.ChatId) - DateTime.Now).TotalMilliseconds);

                timer.Start();

                timerByIdDict.Add(message.ChatId, timer);

                VkApiFacade.SendTextMessege(message.ChatId, "Вы подписаны на рассылку расписания");
            }
        }

        private static void Stop(GroupUpdate update)
        {
            Message message = ParseGroupUpdateIntoMessage(update);

            if (timerByIdDict.ContainsKey(message.ChatId))
            {
                timerByIdDict[message.ChatId].Stop();

                timerByIdDict.Remove(message.ChatId);

                VkApiFacade.SendTextMessege(message.ChatId, "Вы отказались от рассылки расписания");
            }
            else
                VkApiFacade.SendTextMessege(message.ChatId, "Вы не подписаны на рассылку расписания");
        }

        private static void NewTimetable(GroupUpdate update)
        {
            Message message = ParseGroupUpdateIntoMessage(update);
            // TODO: Можно ещё и pdf считывать
            if (message.Attachments.Count == 1 && message.Attachments[0].Instance is Document doc && doc.Ext == "txt")
            {
                DownloadDocumentFromVk(doc);

                dataStore.NewTimetable(File.ReadAllLines(doc.Title));

                File.Delete(doc.Title);
            }
        }

        private static void ChangeDayTimetable(GroupUpdate update)
        {
            // TODO: Написать реализацию метода
        }
    }
}
