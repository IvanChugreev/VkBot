using System;
using System.IO;
using System.Threading.Tasks;
using TypesUsedByBot;
using VkApiAdapterForBot;

namespace VkBot
{
    class Program
    {
        async static Task Main(string[] args)
        {
            // Чтобы подключиться к VkApi в файле setting.txt должно быть: в первой строке токен, во второй строке id группы
            string[] setting = File.ReadAllText("setting.txt").Split(new char[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Инициализация API мессенджера
            IMessengerApi<long> vkApi = new VkApiAdapter(setting[0], ulong.Parse(setting[1]));

            // Инициализация системы хранения

            //BotCommands<long> commands = new BotCommands<long>(vkApi, )
        }
    }
}
