using TypesUsedByBot;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

namespace VkApiAdapterForBot
{
    public class VkApiAdapter : IMessengerApi<long>
    {
        private readonly Random randomId;
        private readonly VkApi Api;
        private readonly ulong GroupId;

        private BotsLongPollHistoryResponse HistoryResponse
        {
            get
            {
                LongPollServerResponse serverResponse = Api.Groups.GetLongPollServer(GroupId);

                return Api.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams
                {
                    Server = serverResponse.Server,
                    Ts = serverResponse.Ts,
                    Key = serverResponse.Key,
                    Wait = 25
                });
            }
        }

        public VkApiAdapter(string accessToken, ulong groupId)
        {
            randomId = new Random();

            Api = new VkApi();

            Api.Authorize(new ApiAuthParams() { AccessToken = accessToken });

            GroupId = groupId;
        }

        public void SendTextMessege(long recipientId, string text) 
            => Api.Messages.Send(new MessagesSendParams 
            { 
                RandomId = randomId.Next(), 
                Message = text, 
                PeerId = recipientId 
            });

        public List<MessageParams<long>> GetNewMesseges()
        {
            List<MessageParams<long>> messages = new List<MessageParams<long>>();

            BotsLongPollHistoryResponse historyResponse = HistoryResponse;

            if (historyResponse?.Updates != null)
                foreach (GroupUpdate update in historyResponse.Updates)
                    messages.Add(GetMessageParamsFromMessage(((MessageNew)update.Instance).Message));

            return messages;
        }

        private static MessageParams<long> GetMessageParamsFromMessage(Message message) 
            => new MessageParams<long>(
                message.PeerId ?? throw new ArgumentNullException("PeerId cannot be null"),
                message.Text,
                GetArrayOfDocumentParamsFromMessage(message)
                );

        private static DocumentParams[] GetArrayOfDocumentParamsFromMessage(Message message)
        {
            List<DocumentParams> documentParams = new List<DocumentParams>();

            foreach (Attachment attachment in message.Attachments)
                if (attachment.Instance is Document document)
                    documentParams.Add(new DocumentParams(document.Title, document.Ext, document.Uri));

            return documentParams.ToArray();
        }
    }
}