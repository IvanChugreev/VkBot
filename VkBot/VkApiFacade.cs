using System;
using System.IO;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VkBot
{
    static class VkApiFacade
    {
        private static readonly Random randomId;

        public static VkApi Api { get; }

        public static ulong GroupId { get; }

        public static LongPollServerResponse ServerResponse { get; }

        public static BotsLongPollHistoryResponse HistoryResponse
        {
            get => Api.Groups.GetBotsLongPollHistory( 
                new BotsLongPollHistoryParams { Server = ServerResponse.Server, Ts = ServerResponse.Ts, Key = ServerResponse.Key, Wait = 25 });
        }

        static VkApiFacade()
        {
            randomId = new Random();

            // В файле setting.txt должно быть: в первой строке токен, во второй строке id группы
            string[] setting = File.ReadAllText("setting.txt").Split(new char[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Api = new VkApi();

            Api.Authorize(new ApiAuthParams() { AccessToken = setting[0] });

            GroupId = ulong.Parse(setting[1]);

            ServerResponse = Api.Groups.GetLongPollServer(GroupId);
        }

        public static long SendTextMessege(long recipientId, string text)
            => Api.Messages.Send(new MessagesSendParams { RandomId = randomId.Next(), Message = text, PeerId = recipientId });
    }
}
