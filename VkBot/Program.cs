using System;
using System.IO;
using VkNet.Model;
using VkNet.Model.GroupUpdate;

namespace VkBot
{
    class Program
    {
        static void Main(string[] args)
        {
            int h = 0;
            try { LaunchingBot(); }
            catch { } // TODO: Запись в log
            Week wek = new Week() { Parity = false };
            foreach (var item in wek.Days)
            {
                foreach (var item2 in item.Lessons)
                {
                    Console.WriteLine(item2.Name + "123");
                    h++;
                    
                }
            }
            Console.WriteLine(h);
            Console.WriteLine();
        }

        static void LaunchingBot()
        {            
            while (true)
            {
                BotsLongPollHistoryResponse history = VkApiFacade.HistoryResponse;

                if (history?.Updates != null)
                    foreach (GroupUpdate update in history.Updates)
                        BotCommands.ReactToUpdate(update);
            }
        }
    }
}
