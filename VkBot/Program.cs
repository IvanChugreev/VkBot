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
            try { LaunchingBot(); }
            catch { } // TODO: Запись в log
           
            Week Chisl = WeekLoader.GetFromFile("Novy_textovy_dokument_4.txt");
            
            Chisl.Output();
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
