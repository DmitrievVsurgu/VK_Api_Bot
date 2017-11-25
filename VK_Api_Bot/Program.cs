using System;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace VK_Api_Bot
{
    class Program
    {
        static VkApi vkapi = new VkApi();
        static string[] Commands = { "hello", "hi" };

        static void Main(string[] args)
        {
            string Login = System.IO.File.ReadAllText(@"D:/Login.txt");
            string Password = System.IO.File.ReadAllText(@"D:/Password.txt");
            ulong ID = ulong.Parse(System.IO.File.ReadAllText(@"D:/ID.txt"));

            Console.WriteLine("Auth started...");
            if(Auth(Login, Password, ID))
            {
                Console.WriteLine("Auth completed.");
                var Friends = GetFriends();
                User I = vkapi.Users.Get(vkapi.UserId.Value);
                Console.WriteLine("-1: " + I.FirstName + " " + I.LastName);
                for (int i = 0; i < Friends.Count; i++)
                {
                    Console.WriteLine(i + " " + Friends[i].FirstName + " " + Friends[i].LastName);
                }
                Console.Write("Введите номер друга: ");
                int number = int.Parse(Console.ReadLine());
                while (number > Friends.Count && number != -1)
                {
                    Console.WriteLine("Неверный номер!");
                    Console.Write("Введите номер друга: ");
                    number = int.Parse(Console.ReadLine());
                }
                if(number == -1)
                {
                    Console.WriteLine("Выбраны вы: " + I.FirstName + " " + I.LastName);
                    CheckMessages(I.Id);
                }
                else
                {
                    Console.WriteLine("Выбран друг: " + Friends[number].FirstName + " " + Friends[number].LastName);
                    CheckMessages(Friends[number].Id);
                }

                Console.WriteLine("Нажмите ENTER чтобы продолжить...");
                Console.ReadLine();
            }
        }

        static bool Auth(string Login, string Password, ulong ID)
        {
            try
            {
                vkapi.Authorize(new ApiAuthParams
                {
                    Login = Login,
                    Password = Password,
                    Settings = Settings.All,
                    ApplicationId = ID
                });
                return true;
            }
            catch
            {
                return false;
            }
        }
        static void CheckMessages(long CheckedUserID)
        {
            bool IsFirst = true;
            long ThisUserID = 0;
            Message LastMessage = null;
            Message CurrentMessage = null;
            bool Enable = true;

            while (Enable)
            {
                try
                {
                    if (IsFirst)
                    {
                        ThisUserID = vkapi.UserId.Value;
                        LastMessage = vkapi.Messages.Get(new MessagesGetParams
                        {
                            Count = 1
                        }).Messages[0];
                        IsFirst = false;
                    }
                    else
                    {
                        CurrentMessage = vkapi.Messages.Get(new MessagesGetParams
                        {
                            Count = 1
                        }).Messages[0];
                        if(CurrentMessage.Date != LastMessage.Date)
                        {
                            Command(CheckedUserID, CurrentMessage.Body);
                        }
                        LastMessage = CurrentMessage;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Enable = false;
                }
            }
        }

        static void Command(long CheckedUserID, string Body)
        {
            int CommandID = -1;
            for(int i = 0; i < Commands.Length; i++)
            {
                if (Body.ToLower() == Commands[i])
                {
                    CommandID = i;
                    break;
                }
            }
            if (CommandID != -1)
            {
                switch (CommandID)
                {
                    case 0: SendMessage(CheckedUserID, "Helloo!!!!");
                        break;
                    case 1: SendMessage(CheckedUserID, "Hello!!!");
                        break;
                }
            }
        }
        static VkCollection<User>GetFriends()
        {
            VkCollection<User> Friends = vkapi.Friends.Get(new FriendsGetParams
            {
                UserId = vkapi.UserId,
                Fields = ProfileFields.FirstName | ProfileFields.LastName,
                Order = FriendsOrder.Name
            });
            return Friends;
        }
        static void SendMessage(long ID, string Body)
        {
            vkapi.Messages.Send(new MessagesSendParams
            {
                UserId = ID,
                Message = Body
            });
        }
    }
}
