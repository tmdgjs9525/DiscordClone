using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscordClone.Core.Util;
using DiscordClone.MasterChannel.Util;
using DiscordClone.MasterChannel.ViewModels;
using Prism.Regions;
using System;
using System.Windows;

namespace DiscordClone.ViewModels
{
    public partial class LoginViewModel :ObservableObject
    {
        public Window thisWindow;
        [ObservableProperty]
        private string userId;
        [ObservableProperty]
        private string nickName;
        [ObservableProperty]
        private string password;

        SignalR signalr = SignalR.Instance();
        DatabaseManager databaseManager = DatabaseManager.Instance();
        private readonly IRegionManager _regionManager;
        public LoginViewModel()
        {
            signalr.StartSignalR();

        }
        public LoginViewModel(IRegionManager regionManager)
        {
             signalr.StartSignalR();
             _regionManager = regionManager;

        }

        [RelayCommand]
        private void Join()
        {
            if (!databaseManager.createAccount(UserId, NickName, Password, Guid.NewGuid()))
            {
                MessageBox.Show("회원가입 실패");
            }
        }
        [RelayCommand]
        private async void Login()
        {
            MainContentMasterChannelViewModel.currentUser = await signalr.loginAsync(UserId, Password);
            
            if (MainContentMasterChannelViewModel.currentUser == null)
            {
                MessageBox.Show("아이디 및 비밀번호 확인");
            }
            else
            {
                signalr.MatchGuidWithUserName(MainContentMasterChannelViewModel.currentUser.guid);
                thisWindow.Close();
            }
        }
    }
}
