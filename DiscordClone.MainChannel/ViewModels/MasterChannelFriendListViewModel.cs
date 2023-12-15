using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.MainChannel.ViewModels
{
    public partial class MasterChannelFriendListViewModel :ObservableObject
    {
        private readonly IEventAggregator _ea;

        public MasterChannelFriendListViewModel(IEventAggregator ea)
        {
            _ea = ea;
        }
    }
}
