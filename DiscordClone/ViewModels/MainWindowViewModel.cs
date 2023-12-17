using CommunityToolkit.Mvvm.Input;
using DiscordClone.Core.Events;
using DiscordClone.Core.Util;
using DiscordClone.MasterChannel.Util;
using DiscordClone.MasterChannel.Views;
using DiscordClone.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows;
using System.Windows.Input;

namespace DiscordClone.ViewModels
{
    public partial class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _ea;
        private SignalR signalR = SignalR.Instance();

        public ICommand NavigationCommand { get; set; }

        DatabaseManager databaseManager = DatabaseManager.Instance();

        public MainWindowViewModel(IRegionManager regionManager,IEventAggregator ea)
        {
            Login login = new Login();
            if (login.DataContext is LoginViewModel viewmodel)
            {
                viewmodel.thisWindow = login;
            }
            login.ShowDialog();

            signalR.StartSignalR();

            //databaseManager.getTempData();
            _ea = ea;
           _ea.GetEvent<NavigationChannel>().Subscribe(NavigationEvent);
            NavigationCommand = new DelegateCommand<string>(OnNavigation);
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion("MainContentRegion",typeof(MainContentMasterChannel));
        }


        [RelayCommand]
        private void Exit()
        {
            Application.Current.Shutdown();
        }
        private void NavigationEvent(string eventData)
        {
            OnNavigation(eventData);
        }
        private void OnNavigation(string para)
        {
            switch (para)
            {
                //Back이란 문자열이 들어오면..
                case "Back":
                    {
                        //Back을 구현하기 위해서 ContentRegion의 Journal을 가져오고, 뒤로가기가 가능한지 확인 후 실행
                        IRegionNavigationJournal journal = _regionManager.Regions["MainContentRegion"]
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
                    _regionManager.RequestNavigate("MainContentRegion", para);
                    break;
            }
        }
    }
}
