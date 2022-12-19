using System;
using System.Threading.Tasks;
using TypesUsedByBot;

namespace VkBot
{
    // T - тип данных для Id чата получаемого из мессенджера
    class Bot<T>
    {
        private object _lock;
        private bool enabled;
        private readonly BotCommands<T> commands;

        public IMessengerApi<T> MessangerApi { get; private set; }

        public IRepositoryApi<T> RepositoryApi { get; private set; }

        public Bot(IMessengerApi<T> messangerApi, IRepositoryApi<T> repositoryApi)
        {
            _lock = new object();

            enabled = false;

            MessangerApi = messangerApi;

            RepositoryApi = repositoryApi;

            commands = new BotCommands<T>(this);
        }

        public async Task StartAsync()
        {
            enabled = true;

            while(true)
            {
                lock(_lock)
                {
                    if (!enabled)
                        break;
                }

                await ReactToNewMessagesAsync();
            }
        }

        public void Stop()
        {
            lock (_lock)
                enabled = false;
        }

        private async Task ReactToNewMessagesAsync()
        {
            foreach (MessageParams<T> message in MessangerApi.GetNewMessages())
            {
                string firstWord = message.Text.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0];

                if (commands.CommandByMsgDict.ContainsKey(firstWord))
                    try { await commands.CommandByMsgDict[firstWord](message); }
                    catch (ArgumentException e) { MessangerApi.SendTextMessage(message.ChatId, e.Message); }
                    catch { MessangerApi.SendTextMessage(message.ChatId, "Какие-то неполадки, но мы уже чиним"); }
            }
        }
    }
}
