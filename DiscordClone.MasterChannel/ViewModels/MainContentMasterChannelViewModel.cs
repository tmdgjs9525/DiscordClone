using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscordClone.Core.Events;
using DiscordClone.Core.Models;
using DiscordClone.Core.Util;
using DiscordClone.Core.ViewTypeEnum;
using DiscordClone.MasterChannel.Models;
using DiscordClone.MasterChannel.Nitro.Views;
using DiscordClone.MasterChannel.Store.Views;
using DiscordClone.MasterChannel.UserControlsFriend;
using DiscordClone.MasterChannel.Util;
using DiscordClone.MasterChannel.Views;
using Microsoft.AspNet.SignalR.Client;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
namespace DiscordClone.MasterChannel.ViewModels
{
    public partial class MainContentMasterChannelViewModel :ObservableObject
    {
        private IEventAggregator _ea;
        private readonly IRegionManager _regionManager;
        DatabaseManager databaseManager = DatabaseManager.Instance();

        private List<Friend> _friends; //친구목록 다 들고있는 리스트

        [ObservableProperty]
        private List<Friend> friends;  //state에 따라서 구분되어 넣어져있는 리스트

        [ObservableProperty]
        private List<Friend> directMessageFriends; // 왼쪽 친구리스트

        [ObservableProperty]
        private Friend currentDMFriend; //현재 선택된 친구

        [ObservableProperty]
        private string currentText = string.Empty;

        [ObservableProperty]
        private string currentFriendNumber; //현재 친구 몇명인지


        [ObservableProperty]
        private List<Message> messages; //DM 메세지들

        [ObservableProperty]
        private List<Friend> callingUsers = new List<Friend>();
        [ObservableProperty]
        private Visibility callingPageVisible;

        public static User currentUser;
        SignalR signalr = SignalR.Instance();
        bool calling = false;
        public MainContentMasterChannelViewModel(IEventAggregator ea,IRegionManager regionManager)
        {
            _ea = ea;
            _ea.GetEvent<NavigationViewType>().Subscribe(NavigationEvent);
             _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion("MasterChannelContentRegion", typeof(MasterChannelFriendList));
            _regionManager.RegisterViewWithRegion("MasterChannelContentRegion", typeof(MainContentNitro));
            _regionManager.RegisterViewWithRegion("MasterChannelContentRegion", typeof(MainContentStore));
            _regionManager.RegisterViewWithRegion("MasterChannelContentRegion", typeof(MasterChannelDirectMessage));
            CallingPageVisible = Visibility.Collapsed;
            DirectMessageFriends = genFriends();
            _friends = genFriends();
            getFriendsInStatus("ONLINE");
           
            //_regionManager.RegisterViewWithRegion("MasterChannelContentRegion", typeof(NitroMain));
            CurrentFriendNumber = "온라인 - 0명";
            signalr.setRecieveMessageDelegate(ReceiveMessage);
            signalr.setRecevieJoinChatRoom(ReceiveJoinChatRoom);
        }
        [RelayCommand]
        private void callFriend()
        {
            signalr.ChatRoomOnLive(currentUser.guid,CurrentDMFriend.Guid);
            CurrentDMFriend.Color = Brushes.Blue;
            currentUser.color = Brushes.Green;
            CallingUsers.Add(Friend.ConvertUserToFriend(currentUser));
            CallingPageVisible = Visibility.Visible;
            //signalr.StartAudioCapture(CurrentDMFriend.Guid);
        }
        private void ReceiveJoinChatRoom(Friend friend)
        {
            List<Friend> temp = CallingUsers.ToList();
            temp.Add(friend);
            CallingUsers = temp;
        }
        private async void ReceiveMessage(Message message)
        {
            Messages = await signalr.getDirectMessages(currentUser.guid, message.sender);

            List<Message> temp = new List<Message>();
            temp = Messages.ToList();
            if (temp.Last().sender.Equals(message.sender))
            {
                temp.Last().messages.Add(message.messages.Last());
            }
            else
            {
                temp.Add(new Message(DateTime.Now, message.senderName, message.sender));
                temp.Last().messages.Add(message.messages.Last());
            }
            Messages = temp;

            foreach (var f in Friends)
            {
                if (f.Guid.Equals(message.sender))
                {
                    f.alarm = 1;
                    //Task.Delay(2000);
                    //f.alarm = 0;
                }
            }
            
        }

        [RelayCommand]
        private void SendMessage()
        {
            //SignalR signal = SignalR.Instance();
            //signal.SendMessageToFriend(CurrentDMFriend.Guid,CurrentText);

            List<Message> temp = new List<Message>();
            temp = Messages.ToList();
            databaseManager.AddMessage(currentUser.guid,CurrentDMFriend.Guid, CurrentText);
            if (temp.Count == 0)
            {
                temp.Add(new Message(DateTime.Now, currentUser.userName, currentUser.guid));
                temp.Last().messages.Add(CurrentText);
            }
            else if (temp.Last().sender.Equals(currentUser.guid))
            {
                temp.Last().messages.Add(CurrentText);
            }
            else
            {
                temp.Add(new Message(DateTime.Now, currentUser.userName, currentUser.guid));
                temp.Last().messages.Add(CurrentText);
            }
            Message sendMessage = new Message(temp.Last().Time,temp.Last().senderName,temp.Last().sender);
            sendMessage.messages.Add(CurrentText);
            signalr.sendMessage(CurrentDMFriend.Guid, sendMessage);
            Messages = temp;
            CurrentText = "";
        }
        [RelayCommand]
        private void AddFriend(object friendParam)
        {
            Friend friend = friendParam as Friend;
            List<Friend> friends = databaseManager.loadFriends(currentUser.guid);
            foreach (Friend f in friends)
            {
                if (f.userId.Equals(friend.userId))
                {
                    friend.isFriend = true;
                    MessageBox.Show("이미 친구입니다");
                    return;
                }
            }
            databaseManager.AddFriendAndCreateChatRoom(currentUser.guid, friend.Guid,"default");
            Friends = genFriends();
            CurrentFriendNumber = Friends.Count.ToString() + " 명";

        }
        [RelayCommand]
        private async Task NaviDMPageAsync(object friendParam)
        {
            CurrentDMFriend = friendParam as Friend;
            bool isLive = await signalr.readChatRoomOnLive(currentUser.guid, CurrentDMFriend.Guid);
            if (isLive)
            {
                CallingPageVisible = Visibility.Visible;
                CallingUsers.Add(CurrentDMFriend);
            }
            else
            {
                CallingPageVisible = Visibility.Collapsed;
            }
            _ea.GetEvent<NavigationViewType>().Publish(ViewType.MasterChannelDirectMessage);
        }
        [RelayCommand]
        private void SearchFriend(string UserId)
        {
            List<Friend> temp = new List<Friend>();
            Friend friend = databaseManager.SearchFriend(currentUser.guid,UserId);
            if (friend !=null)
            {
                temp.Add(friend);
                Friends = temp;

            }
        }
        private List<Friend> genFriends()
        {
            List<Friend> test = databaseManager.loadFriends(currentUser.guid);
            return test;  
        }
        [RelayCommand]
        private void JoinChatRoom()
        {
            signalr.joinDirectChatRoom(Friend.ConvertUserToFriend(currentUser),CurrentDMFriend.Guid);
        }
        [RelayCommand]
        private void getFriendsInStatus(string state)
        {
            List<Friend> list = new List<Friend>();
            if (state.Equals("ALL"))
            {
                Friends = _friends;
                CurrentFriendNumber = "모두 - " + Friends.Count.ToString() +" 명";
                return;
            }
            foreach (Friend f in _friends)
            {
                if (state.Equals(f.State.ToString()))
                {
                    list.Add(f);
                }
            }
            Friends = list;
            CurrentFriendNumber = Friends.Count.ToString() +" 명";

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
                    _ea.GetEvent<NavigationViewType>().Publish(ViewType.MainContentNitro);
                    break;
                case "store":
                    _ea.GetEvent<NavigationViewType>().Publish(ViewType.MainContentStore);
                    break;
                default: break;

            }
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
