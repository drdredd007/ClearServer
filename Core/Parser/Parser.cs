using System;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace ClearServer.Core.Parser
{
    class Parser
    {
        readonly DatabaseWorker database = new DatabaseWorker();
        public static User user;
        enum AuthForm
        {
            Full,
            Input,
            Login,
            Password
        }

        enum RegForm
        {
            Full,
            Input,
            Name,
            Birthday,
            City,
            Skills,
            Login,
            Password
        }

        readonly string[] patterns = { @"(login)=(.*?)&password=(.*?)$",
            @"(name)=(.*?)&date=(.*?)&place=(.*?)&skills=(.*?)&regLogin=(.*?)&regPass=(.*?)$"};
        public Parser(string message, NetworkStream ClientStream)
        {
            string unescaped = Uri.UnescapeDataString(message);
            unescaped = unescaped.Replace('+', ' ');
            Match httpParse;
            foreach (var pattern in patterns)
            {

                httpParse = Regex.Match(unescaped, pattern);
                if (httpParse != Match.Empty)
                {
                    switch (httpParse.Groups[1].Value)
                    {
                        case "login":
                            Console.WriteLine(httpParse.Value);
                            var authUser = new User
                            {
                                login = httpParse.Groups[(int)AuthForm.Login].Value.ToLower(),
                                password = httpParse.Groups[(int)AuthForm.Password].Value
                            };
                            user = database.UserAuth(authUser);
                            if (user != null)
                                OnAuth(ClientStream, message);
                            else
                                ErrorWorker.SendError(ClientStream, 403);
                            break;
                        case "name":
                            var regUser = new User
                            {
                                name = httpParse.Groups[(int)RegForm.Name].Value,
                                date = httpParse.Groups[(int)RegForm.Birthday].Value,
                                city = httpParse.Groups[(int)RegForm.City].Value,
                                skills = httpParse.Groups[(int)RegForm.Skills].Value,
                                login = httpParse.Groups[(int)RegForm.Login].Value.ToLower(),
                                password = httpParse.Groups[(int)RegForm.Password].Value
                            };
                            if (database.LoginValidate(regUser.login))
                            {
                                database.UserRegister(regUser);
                                OnRegister(ClientStream, message);
                            }
                            break;
                    }
                }

            }
        }

        private void OnRegister(NetworkStream clientStream, string message)
        {
            Client.Response(clientStream, message);
        }

        public void OnAuth(NetworkStream ClientStream, string Message)
        {
            Console.WriteLine("User authorized");
            Client.Response(ClientStream, Message);
        }
    }
}
