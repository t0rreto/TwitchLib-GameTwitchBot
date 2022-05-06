using System;
using System.Collections.Generic;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using System.Threading;

namespace TwitchGameBot
{


    public class Bot
    {
        List<Person> players;
        private static TwitchAPI api;
        public TwitchClient client;
        ConnectionCredentials credentials = new ConnectionCredentials(Setting.twitchClient_ClientName, Setting.twitchClient_OauthToken);
        string channel = Setting.channelName;
        enum StageState {Waiting, ExpectationAnswer, During, Сooldown }
        StageState stage;
        Game game = new Game();
        static object locker = new object();

        public Bot()
        {
            
            client = new TwitchClient();      
            client.Initialize(credentials, channel);
            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
            client.OnMessageReceived += Client_OnMessageReceived;

            api = new TwitchAPI();
            api.Settings.ClientId = Setting.twitchApi_ClientId;
            api.Settings.AccessToken = Setting.twitchApi_AccessToken;


            client.Connect();
            stage = StageState.Waiting;

        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            //If the bot does not have moderator/vip user rights, the delay is necessary to avoid spam restrictions. Recommended 5000 and above
            if (e.ChatMessage.Username == Setting.twitchClient_ClientName)
            {
                Thread.Sleep(1000);
            }

        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            Thread myThread = new Thread(delegate() { СommandHandler(e); });
            myThread.Start();
            
        }

        private void СommandHandler(OnChatCommandReceivedArgs e)
        {
           
            Console.WriteLine(stage);
            
            string command = e.Command.CommandText;
            if (command.ToLower() != "duel") return;
            List<string> args = e.Command.ArgumentsAsList;
            if (args.Count == 0) return;

           
            

            switch (stage)
            {
                case StageState.Waiting:
                    if (args[0][0] != '@') return;
                    if (e.Command.ChatMessage.DisplayName.ToLower()==args[0].Substring(1))
                    {
                        tSendMessage($"You can't play with yourself {e.Command.ChatMessage.DisplayName}");
                       // return;
                    }
                    User[] users = LoginsCheaker(new List<string> { e.Command.ChatMessage.DisplayName, args[0].Substring(1) });
                    if (users == null)
                    {
                        tSendMessage($"There is no user with this name {args[0].Substring(1)}");
                        return;
                    }
                        
                                 
                    players = Person.onPersonList(users);
                    string response = game.Request(players[0], players[1]);
                    stage = StageState.ExpectationAnswer;
                    tSendMessage(response);

                    Thread.Sleep(25000);
                    if(stage==StageState.ExpectationAnswer)
                    {
                        stage = StageState.Waiting;
                        tSendMessage($"{players[1].name} no one came :( ");
                    }

                    break;

                case StageState.ExpectationAnswer:
                    if (args[0] != "accept" || e.Command.ChatMessage.DisplayName.ToLower() != game.user1.name.ToLower()) break;
                    stage = StageState.During;
                    string message = game.LabPathCalculation();
                    tSendMessage("Your task is to calculate the distance in cells from the entrance to the apple. Get ready");
                    tSendMessage(message);                    

                    Thread.Sleep(60000);
                    if (stage == StageState.During)
                    {
                        stage = StageState.Waiting;
                        tSendMessage($"{players[1].name} {players[0].name}  Time is up, no one gave the right answer");
                    }


                    break;

                case StageState.During:
                    if (e.Command.ChatMessage.DisplayName != game.user2.name && e.Command.ChatMessage.DisplayName != game.user1.name) return;
                    stage = StageState.Сooldown;

                    if (game.LabPathResult(args[0]))
                    {                              
                        tSendMessage($"You won {e.Command.ChatMessage.DisplayName}");
                    }
                    else
                    {
                        tSendMessage($"You made a mistake and lost {e.Command.ChatMessage.DisplayName}");
                    }
                    stage = StageState.Waiting;

                    break;

       
            }
        }

        public void tSendMessage(string message)
        {
            
            lock (locker)
            {
                client.SendMessage(channel, message);
                Thread.Sleep(5000);
            }
            
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            
            client.SendMessage(channel, "Bot has entered");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {           
            Console.WriteLine(e.Data);

        }

        private User[] LoginsCheaker(List<string> logins)
        {
            try {
                var a = api.V5.Users;
                GetUsersResponse getUsers = api.Helix.Users.GetUsersAsync(null, logins).Result;
                if (getUsers.Users.Length != logins.Count) return null;
                User[] users = getUsers.Users;
                if (users[0].DisplayName != logins[0])
                {
                    User temp = users[1];
                    users[1] = users[0];
                    users[0] = temp;
                }
                return users;
            }
            catch (Exception e) {
                Console.WriteLine(e.InnerException);
                return null;
            }
                       
        }

        
    }
}