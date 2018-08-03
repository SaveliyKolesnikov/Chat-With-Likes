using System;
using System.Text;


namespace ChatWithLikes
{
    class Message
    {
        public Message(int messageId, string text, int senderId, int? replyMessageId, DateTime date, int mark)
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
        public User Sender { get; set; }
        public int? ReplyMessageId { get; set; }
        public Message ReplyMessage { get; set; }
        public DateTime Date { get; set; }
        public int Mark { get; set; }

        public override string ToString()
        {
            var result = new StringBuilder();
            var currentSender = Sender is null
                ? $"SenderId: {SenderId.ToString()}"
                : Sender.ToString();
            result.AppendLine($"{currentSender}:");
            if (!(ReplyMessage is null))
            {
                var sender = ReplyMessage.Sender is null
                    ? $"SenderId: {ReplyMessage.SenderId.ToString()}"
                    : ReplyMessage.Sender.ToString();
                result.AppendLine($"| {sender}:");
                foreach (var row in ReplyMessage.Text.Split('\n'))
                    result.AppendLine($"| {row}");
                result.AppendLine($"| {ReplyMessage.Date}  Mark: {ReplyMessage.Mark}");
            }

            result.AppendLine(Text);
            result.AppendLine($"{Date}  Mark: {Mark}");
            return result.ToString();
        }
    }
}
