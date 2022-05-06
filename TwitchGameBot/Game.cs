using System;
namespace TwitchGameBot
{
    class Game 
    {
        Labirint lab;
        public Person user1;
        public Person user2;
        public string answer;

        public Game() { }
        

        public string Request(Person user1, Person user2)
        {
            string message = $"{user2.name} appointed you a duel {user1.name}  \n Send '!duel accept' to accept";        
            this.user1 = user1;
            this.user2 = user2;
            return message;   
            
        }     

        public string LabPathCalculation()
        {
            Random rand = new Random();
            int target = rand.Next(10, 40);
            answer = target.ToString();
            lab = new Labirint(11,13,target);
            lab.PathInitialized();            
            return lab.PathToImg();

        }

        public bool LabPathResult(string arg)
        {
            bool correct = false;
            
            if (Convert.ToInt32(arg) == Convert.ToInt32(answer) + 2 ) correct = true;
            return  correct ;
        }

    }
}
