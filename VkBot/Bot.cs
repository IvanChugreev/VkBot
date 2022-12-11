using TypesUsedByBot;

namespace VkBot
{
    // T - тип данных для Id чата
    class Bot<T>
    {
        //private readonly IMessengerApi<T> messengerApi;
        //private readonly IRepositoryApi repositoryApi;
        private readonly BotCommands<T> commands;

        public Bot(IMessengerApi<T> messengerApi, IRepositoryApi<T> repositoryApi)
        {
            //this.messengerApi = messengerApi;

            //this.repositoryApi = repositoryApi;

            commands = new BotCommands<T>(messengerApi, repositoryApi);
        }


    }
}
