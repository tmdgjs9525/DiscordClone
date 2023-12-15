using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscordClone.Core.Events;
using DiscordClone.Core.ViewTypeEnum;
using Prism.Events;

namespace DiscordClone.MainChannel.ViewModels
{
    public partial class MasterChannelStateViewModel : ObservableObject
    {
        private readonly IEventAggregator _ea;

        public MasterChannelStateViewModel(IEventAggregator ea)
        {
            _ea = ea;
        }

        [RelayCommand]
        private void navigationBtnClick(string data)
        {
            switch (data)
            {
                case "friend":
                    _ea.GetEvent<NavigationViewType>().Publish(ViewType.MasterChannelFriendList);
                    break;
                case "nitro":
                    _ea.GetEvent<NavigationViewType>().Publish(ViewType.NitroMain);
                    break;
                case "store":
                    _ea.GetEvent<NavigationViewType>().Publish(ViewType.Store);
                    break;
                default: break;

            }
        }
    }
}
