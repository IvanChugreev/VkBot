using System;
using System.Collections.Generic;
using System.Text;

namespace VkBot
{
    interface ICommunicationProtocol
    {
        // TODO: добавить event для нового дня и недели
        DateTime NextLesson(long? chatId);

        bool NewTimetable(long? chatId, string[] timetable);
    }
}
