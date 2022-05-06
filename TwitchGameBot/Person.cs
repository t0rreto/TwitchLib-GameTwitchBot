using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
namespace TwitchGameBot
{
    class Person
    {
        public string id;
        public string name;

        Person()
        {

        }
        Person(User user)
        {
            id = user.Id;
            name = user.DisplayName;
        }

         public static List<Person> onPersonList(User[] users)
        {
            List<Person> persons = new List<Person>();
            foreach (var user in users)
            {
                persons.Add(new Person(user));
            }
            return persons;
        }

    }
}
