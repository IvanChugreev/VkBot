using System;
using System.IO;
using System.Threading.Tasks;
using TypesUsedByBot;
using VkApiAdapterForBot;
using DataBase;

namespace VkBot
{
    class Program
    {
        async static Task Main(string[] args)
        {
            IMessengerApi<long> vkApi = null;

            try
            {
                // Чтобы подключиться к VkApi в файле setting.txt должно быть: в первой строке токен, во второй строке id группы
                string[] setting = File.ReadAllLines("setting.txt");

                // Инициализация API мессенджера
                vkApi = new VkApiAdapter(setting[0], ulong.Parse(setting[1]));
            }
            catch (FileNotFoundException e) { Console.WriteLine(e.Message); }
            catch { Console.WriteLine("Некорректные данные в файле setting.txt (в файле setting.txt должно быть: в первой строке токен, во второй строке id группы)"); }

            if (vkApi == null)
                return;

            // Инициализация системы хранения
            IRepositoryApi<long> repositoryApi = new Protocol();

            Bot<long> bot = new Bot<long>(vkApi, repositoryApi);

            Task task = bot.StartAsync();

            Console.ReadKey();

            bot.Stop();

            await task;
        }
    }
}
