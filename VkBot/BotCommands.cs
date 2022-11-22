using System;
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
        private static readonly ICommunicationProtocol dataStore;
        private static readonly Dictionary<long?, Timer> timerByIdDict;
        public static readonly Dictionary<string, Action<GroupUpdate>> CommandByMsgDict;

        static BotCommands()
        {
            // TODO: возможность выбрать тип хранения данных
            dataStore = new FileStorageSystem();

            timerByIdDict = new Dictionary<long?, Timer>();

            CommandByMsgDict = new Dictionary<string, Action<GroupUpdate>>()
            {
                [".help"] = Help,
                [".start"] = Start,
                [".stop"] = Stop,
                [".new"] = NewTimetable,
                //[".chg"] = ChangeDayTimetable,
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
            VkApiFacade.SendTextMessege(ParseGroupUpdateIntoMessage(update).PeerId, 
                string.Join("\r\n", new string[] {
                "Команды бота:",
                ".help - рассказать про все команды бота",
                ".new - сохранить ваше расписание (вместе с этой командой нужно передать .txt файл вашего расписания)",
                ".start - подписаться на рассылку уведомлений",
                ".stop - отписаться от рассылки уведомлений",
                ".chg - изменить что-то в рсаписании(пока не работает)"}));

        private static void Start(GroupUpdate update)
        {
            Message message = ParseGroupUpdateIntoMessage(update);

            if (timerByIdDict.ContainsKey(message.PeerId))
                VkApiFacade.SendTextMessege(message.PeerId, "Вы уже подписаны на рассылку расписания");
            else
            {
                Timer timer = new Timer((dataStore.NextLesson(message.PeerId) - DateTime.Now).TotalMilliseconds);

                timer.Start();

                timerByIdDict.Add(message.PeerId, timer);

                VkApiFacade.SendTextMessege(message.PeerId, "Вы подписаны на рассылку расписания");
            }
        }

        private static void Stop(GroupUpdate update)
        {
            Message message = ParseGroupUpdateIntoMessage(update);

            if (timerByIdDict.ContainsKey(message.PeerId))
            {
                timerByIdDict[message.PeerId].Stop();

                timerByIdDict.Remove(message.PeerId);

                VkApiFacade.SendTextMessege(message.PeerId, "Вы отказались от рассылки расписания");
            }
            else
                VkApiFacade.SendTextMessege(message.PeerId, "Вы не подписаны на рассылку расписания");
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
