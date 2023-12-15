using System;

namespace DiscordClone.Core.Models
{
    public class User
    {
        public string userId { get; private set; }
        public string userName { get; private set; }
        public string password { get; private set; }
        public Guid guid { get; private set; }

        private User()
        {
           
        }
        private static User instance;
        public static User Instance()
        {
            if (instance == null)
            {
                instance = new User();
            }
            return instance;
        }
        public void setUser(string userId, string userName, string password, Guid guid)
        {
            this.userId = userId;
            this.userName = userName;
            this.password = password;
            this.guid = guid;
        }

    }
}
