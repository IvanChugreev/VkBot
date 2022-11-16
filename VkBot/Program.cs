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
            using (StreamWriter sw = new StreamWriter("asds.txt"))
            {
                string str = PdfTextExtractor1.pdfText("2022-09-26_po_gruppam.pdf");
                sw.WriteLine(str);
            }
            string text1 = File.ReadAllText("asds.txt");
            try { LaunchingBot(); }
            catch { } // TODO: Запись в log
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
