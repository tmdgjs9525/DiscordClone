using CommunityToolkit.Mvvm.ComponentModel;
using DiscordClone.Core.Events;
using DiscordClone.Core.ViewTypeEnum;
using DiscordClone.MainChannel.Views;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClone.MainChannel.ViewModels
{
    public partial class MainChannelContentViewModel :ObservableObject
    {
        private IEventAggregator _ea;
        private readonly IRegionManager _regionManager;

        public MainChannelContentViewModel(IEventAggregator ea,IRegionManager regionManager)
        {
            _ea = ea;
            _ea.GetEvent<NavigationViewType>().Subscribe(NavigationEvent);
             _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion("MasterChannelContentRegion", typeof(MainChannelFriendList));
            //_regionManager.RegisterViewWithRegion("MasterChannelContentRegion", typeof(NitroMain));
        }

        private void NavigationEvent(ViewType eventData)
        {
            OnNavigation(eventData.ToString());
        }
        private void OnNavigation(string para)
        {
            switch (para)
            {
                //Back이란 문자열이 들어오면..
                case "Back":
                    {
                        //Back을 구현하기 위해서 ContentRegion의 Journal을 가져오고, 뒤로가기가 가능한지 확인 후 실행
                        IRegionNavigationJournal journal = _regionManager.Regions["MasterChannelContentRegion"]
                                                                .NavigationService.Journal;
                        if (journal.CanGoBack)
                        {
                            journal.GoBack();
                        }
                    }
                    break;
                //그외 일반 문자열이 들어오면    
                default:
                    //ContentRegion에 para가 지정하는 화면으로 네비게이트 해라
                    _regionManager.RequestNavigate("MasterChannelContentRegion", para);
                    break;
            }
        }
    }
}
