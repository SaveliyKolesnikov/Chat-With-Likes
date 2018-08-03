using System;


namespace ChatWithLikes
{
    class Message
    {
        public Message(int messageId, string text, int senderId, int replyMessageId, DateTime date, int mark)
        {
            MessageId = messageId;
            Text = text;
            SenderId = senderId;
            ReplyMessageId = replyMessageId;
            Date = date;
            Mark = mark;
        }

        public int MessageId { get; }
        public string Text { get; set; }
        public int SenderId { get; set; }
        public int ReplyMessageId { get; set; }
        public DateTime Date { get; set; }
        public int Mark { get; set; }
    }
}
