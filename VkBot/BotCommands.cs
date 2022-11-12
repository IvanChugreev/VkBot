using System;
using System.Collections.Generic;
using VkNet.Model.GroupUpdate;

namespace VkBot
{
    static class BotCommands
    {
        public static readonly Dictionary<string, Func<GroupUpdate, bool>> CommandByMsgDict = new Dictionary<string, Func<GroupUpdate, bool>>()
        {
            [".help"] = Help,
            [".start"] = Start,
            [".stop"] = Stop,
            [".new"] = NewTimetable,
            [".change"] = ChangeDayTimetable,
        };

        public static void ReactToUpdate(GroupUpdate update)
        {
            // TODO: Дописать реакцию при возникновении ошибки
            // TODO: fix Если в сообщении будет несколько слов
            if (update.Instance is MessageNew newMessage && CommandByMsgDict.ContainsKey(newMessage.Message.Text))
                CommandByMsgDict[newMessage.Message.Text](update);
        }

        private static bool Help(GroupUpdate update)
        {
            // TODO: Написать реализацию метода
            return false;
        }

        private static bool Start(GroupUpdate update)
        {
            // TODO: Написать реализацию метода
            return false;
        }

        private static bool Stop(GroupUpdate update)
        {
            // TODO: Написать реализацию метода
            return false;
        }

        private static bool NewTimetable(GroupUpdate update)
        {
            // TODO: Написать реализацию метода
            return false;
        }

        private static bool ChangeDayTimetable(GroupUpdate update)
        {
            // TODO: Написать реализацию метода
            return false;
        }
    }
}
