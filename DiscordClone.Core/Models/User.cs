using System;
using System.Windows.Media;


namespace DiscordClone.Core.Models
{
    public class User
    {
        public string userId { get; private set; }
        public string userName { get; private set; }
        public string password { get; private set; }
        public Guid guid { get; private set; }
        public Brush color { get; set; }
        public User(string userId, string userName, string password, Guid guid)
        {
            this.userId = userId;
            this.userName = userName;
            this.password = password;
            this.guid = guid;
        }


    }
}
