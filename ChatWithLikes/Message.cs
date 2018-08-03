using System;
using System.Collections.Generic;
using System.Text;

namespace ChatWithLikes
{
    class Message
    {
        public int MessageId { get; set; }
        public string Text { get; set; }
        public int SenderId { get; set; }
        public int ReplyMessageId { get; set; }
        public DateTime Date { get; set; }
        public int Mark { get; set; }
    }
}
