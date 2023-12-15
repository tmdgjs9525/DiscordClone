using System.Windows.Media;

namespace DiscordClone.SideBars.Models
{
    public class Channel
    {
        public string channelIcon { get; set; }
        public string channelName { get; set; }
        public Brush color { get; set; }
        public bool allContentsViewed { get; set; } = false;
        public Channel(string channelName, string channelIcon , Brush color)
        {
            this.channelName = channelName;
            this.channelIcon = channelIcon;
            this.color = color;
        }
       
     
        public void setAllContentsViewd(bool allContentsViewed) //안의 채팅 등 다 확인했는지
        {
            this.allContentsViewed = allContentsViewed;
        }
    }
}
