using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.Core.Models
{
    public class Message
    {
        public DateTime Time { get; set; }
        public List<string> messages  { get; set; }
        public Guid sender { get; set; }
        public string senderName { get; set; }
        public Message(DateTime Time , string senderName , Guid sender)
        {
            this.Time = Time;
            this.senderName = senderName;
            this.messages = new List<string>();
            this.sender = sender;
        }
    }
}
