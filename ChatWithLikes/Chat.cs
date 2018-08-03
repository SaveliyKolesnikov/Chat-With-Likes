using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ChatWithLikes
{
    class Chat : IDisposable
    {
        private User CurrentUser { get; set; }
        public UserRepository UserRepository { get; }
        public MessageRepository MessageRepository { get; }
        public MarksRepository MarksRepository { get; }
        public IReadOnlyList<User> UsersReadOnlyList => Users;
        public IReadOnlyList<Message> MessagesReadOnlyList => Messages;
        public IReadOnlyList<Mark> MarksReadOnlyList => Marks;
        private List<User> Users { get; set; }
        private List<Message> Messages { get; set; }
        private List<Mark> Marks { get; set; }
        private readonly SqlConnection _connection;

        public Chat(string conStr) : this(new SqlConnection(conStr ?? throw new ArgumentNullException(nameof(conStr))))
        {
        }

        public Chat(SqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            UserRepository = new UserRepository(connection);
            MessageRepository = new MessageRepository(connection);
            MarksRepository = new MarksRepository(connection);
            Users = UserRepository.GetUsersAsync().GetAwaiter().GetResult();
            Marks = MarksRepository.GetMarksAsync().GetAwaiter().GetResult();
            UpdateMessages().GetAwaiter().GetResult();
        }

        public void ShowUi()
        {
            if (CurrentUser is null)
                LogInUser();

            do
            {
                Console.Clear();
                Console.WriteLine($"Hello, {CurrentUser.Username}!");
                Console.WriteLine("Select an action: ");
                Console.WriteLine(
                    "1. Print the chat history" + Environment.NewLine +
                    "2. Like a message" + Environment.NewLine +
                    "3. Write a message" + Environment.NewLine +
                    "4. Log out" + Environment.NewLine +
                    "0. Exit"
                    );
                var choice = Convert.ToInt32(Console.ReadLine());

                if (choice < 0 || choice > 4) continue;
                Console.Clear();
                switch (choice)
                {
                    case 0:
                        return;
                    case 1:
                        PrintChatHistory();
                        break;
                    case 2:
                        LikeAMessageAsync().GetAwaiter().GetResult();
                        break;
                    case 3:
                        WriteAMessageAsync().GetAwaiter().GetResult();
                        break;
                    case 4:
                        LogInUser();
                        break;
                }
                Console.WriteLine("Press enter to continue.");
                Console.ReadLine();
            } while (true);


        }

        private async Task WriteAMessageAsync()
        {
            int choice;
            do
            {
                var num = 1;
                foreach (var message in Messages.OrderBy(message => message.Date))
                    Console.WriteLine($"{num++}. {message}");
                Console.WriteLine("Select the message you want to reply or -1 if you don't want.");
                choice = Convert.ToInt32(Console.ReadLine());
            } while ((choice < 1 || choice > Messages.Count) && choice != -1);

            var replyedMessage = choice == -1 ? null : Messages[choice - 1];
            Console.WriteLine("Write a body of the message.");
            var text = Console.ReadLine();
            var newMessage = new Message(
                messageId: Messages.LastOrDefault()?.MessageId ?? 0 + 1,
                text: text,
                senderId: CurrentUser.UserId,
                replyMessageId: replyedMessage?.MessageId,
                date: DateTime.Now,
                mark: default(int)
            )
            {
                ReplyMessage = replyedMessage,
                Sender = CurrentUser
            };

            await MessageRepository.CreateMessageAsync(newMessage);
            await UpdateMessages();

            Console.WriteLine("Operation has been done successfully.");
        }

        private async Task LikeAMessageAsync()
        {
            int choice;
            do
            {
                var num = 1;
                foreach (var message in Messages.OrderBy(message => message.Date))
                    Console.WriteLine($"{num++}. {message}");
                Console.WriteLine("Select a message that you want to mark.");
                choice = Convert.ToInt32(Console.ReadLine());
            } while (choice < 1 || choice > Messages.Count);

            var selectedMessage = Messages[choice - 1];
            var markValue = default(int);
            do
            {
                Console.Clear();
                Console.WriteLine("Selected message: ");
                Console.WriteLine(selectedMessage);
                Console.WriteLine("Rate the message:");
                Console.WriteLine("Enter a mark (-1, 0, 1) or -10 to exit.");
                markValue = Convert.ToInt32(Console.ReadLine());
            } while (markValue == -10 && markValue == -1 && markValue == 0 && markValue == 1);

            var userMark = Marks.Find(mark =>
                mark.UserId == CurrentUser.UserId && mark.MessageId == selectedMessage.MessageId);
            if (userMark is null)
            {
                var newMark = new Mark(selectedMessage.MessageId, CurrentUser.UserId, markValue);
                await MarksRepository.CreateMarkAsync(newMark);
                Marks.Add(newMark);
                selectedMessage.Mark += newMark.Value;
            }
            else
            {
                if (userMark.Value != markValue)
                {
                    var markDiff = markValue - userMark.Value;
                    userMark.Value = markValue;
                    await MarksRepository.UpdateMarkAsync(userMark);
                    selectedMessage.Mark += markDiff;
                }
            }

            Console.WriteLine("Message has been marked successfully.");
        }

        private async Task UpdateMessages()
        {
            Messages = await MessageRepository.GetMessagesAsync();
            foreach (var message in Messages)
            {
                message.Sender = Users.Find(user => user.UserId == message.SenderId);
                message.ReplyMessage = Messages.Find(m => m.MessageId == message.ReplyMessageId);
            }
        }

        private void PrintChatHistory()
        {
            foreach (var message in Messages.OrderBy(message => message.Date))
                Console.WriteLine(message);
        }

        private void LogInUser()
        {
            int choice;
            var isFirstEnter = true;

            do
            {
                Console.Clear();
                if (!isFirstEnter)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error! Please, enter a correct value.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine("Please, select your username: ");
                for (var i = 0; i < Users.Count; i++)
                    Console.WriteLine($"{i + 1}. {Users[i]}");
                choice = Convert.ToInt32(Console.ReadLine());
                isFirstEnter = false;
            } while (choice < 1 || choice > Users.Count);

            CurrentUser = Users[choice - 1];
        }

        public void Dispose()
        {
            UserRepository.Dispose();
            MessageRepository.Dispose();
            MarksRepository.Dispose();
        }
    }
}
