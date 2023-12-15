using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.MasterChannel.Models
{
    public class Friend
    {
        public string userId { get; private set; }

        public string Name { get; private set; }
        public Guid Guid { get; private set; }
        public FriendState State { get; set; }
        public bool isFriend { get; set; }
        public Friend(string id,string Name, Guid Id, FriendState State , bool isFriend)
        {
            this.userId = id;
            this.Name = Name;
            this.Guid = Id;
            this.State = State;
            this.isFriend = isFriend;
        }
    }
}
