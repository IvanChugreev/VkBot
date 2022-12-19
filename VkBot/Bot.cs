using TypesUsedByBot;

namespace VkBot
{
    // T - тип данных для Id чата получаемого из мессенджера
    class Bot<T>
    {
        private readonly BotCommands<T> commands;

        public IMessengerApi<T> MessengerApi { get; private set; }

        public IRepositoryApi<T> RepositoryApi { get; private set; }

        public Bot(IMessengerApi<T> messengerApi, IRepositoryApi<T> repositoryApi)
        {
            //this.messengerApi = messengerApi;

            //this.repositoryApi = repositoryApi;

            //commands = new BotCommands<T>(messengerApi, repositoryApi);
        }

        //public void ReactToUpdate(MessageParams<T> message)
        //{
        //    // TODO: Дописать реакцию при возникновении ошибки

        //    if (update.Instance is MessageNew newMessage &&
        //        CommandByMsgDict.ContainsKey(newMessage.Message.Text.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[0]))
        //        CommandByMsgDict[newMessage.Message.Text](update);
        //}

    }
}
