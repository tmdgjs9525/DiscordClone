using DiscordClone.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DiscordClone.MasterChannel.Models
{
    public class Friend
    {
        public string userId { get; private set; }

        public string Name { get; private set; }
        public Guid Guid { get; private set; }
        public FriendState State { get; set; }
        public bool isFriend { get; set; }
        public Brush Color { get; set; }
        public int alarm { get; set; }
        public Friend(string id,string Name, Guid Id, FriendState State , bool isFriend)
        {
            this.userId = id;
            this.Name = Name;
            this.Guid = Id;
            this.State = State;
            this.isFriend = isFriend;
            this.Color = Brushes.Purple;
        }

        public static Friend ConvertUserToFriend(User user)
        {
            Friend friend = new Friend(user.userId,user.userName, user.guid,FriendState.ONLINE,true);
            friend.Color = user.color;
            return friend;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Friend other = (Friend)obj;
            return userId == other.userId &&
                   Name == other.Name &&
                   Guid == other.Guid &&
                   State == other.State &&
                   isFriend == other.isFriend &&
                   Color.Equals(other.Color) &&  // Brush의 동등성 비교
                   alarm == other.alarm;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(userId, Name, Guid, State, isFriend, Color, alarm);
        }
    }
}
