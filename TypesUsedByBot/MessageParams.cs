namespace TypesUsedByBot
{
    public class MessageParams<T>
    {
        public T ChatId { get; set; }

        public string Text { get; set; }

        public DocumentParams[] ArrayOfLinksToAttachedFiles { get; set; }

        public MessageParams(T chatId, string text, DocumentParams[] arrayOfLinksToAttachedFiles)
        {
            ChatId = chatId;
            Text = text;
            ArrayOfLinksToAttachedFiles = arrayOfLinksToAttachedFiles;
        }
    }

    public class DocumentParams
    {
        public string Title { get; set; }

        public string Ext { get; set; }

        public string Uri { get; set; }

        public DocumentParams(string title, string ext, string uri)
        {
            Title = title;
            Ext = ext;
            Uri = uri;
        }
    }
}
