using System;
using System.Collections.Generic;
using System.Text;

namespace ChatWithLikes
{
    class Mark
    {
        public Mark(int messageId, int userId, int value)
        {
            MessageId = messageId;
            UserId = userId;
            Value = value;
        }

        public int MessageId { get; set; }
        public Message Message { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int Value { get; set; }
    }
}
