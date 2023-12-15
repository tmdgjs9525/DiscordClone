using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscordClone.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        DatabaseManager databaseManager = DatabaseManager.Instance();
        public LoginViewModel()
        {
                
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
        private void Login()
        {
            if(!databaseManager.login(UserId, Password))
            {
                MessageBox.Show("아이디 및 비밀번호 확인");
            }
            else
            {
                thisWindow.Close();
            }
        }
    }
}
