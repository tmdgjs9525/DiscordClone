using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscordClone.Core.Events;
using DiscordClone.Core.Icons;
using DiscordClone.SideBars.Models;
using DiscordClone.SideBars.Styles;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DiscordClone.SideBars.ViewModels
{
    public partial class SideChannelViewModel : ObservableObject
    {
        [ObservableProperty]
        private Channel masterChannel;
        [ObservableProperty]
        private List<Channel> channels;
        private readonly IEventAggregator _ea;

        public SideChannelViewModel(IEventAggregator ea)
        {
            _ea = ea;
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                // 디자인 타임에만 실행되는 코드
                MasterChannel = new Channel("", Icons.D, (Brush)new BrushConverter().ConvertFrom("#dbdee1"));
                Channels = new List<Channel> {
                new Channel("", Icons.D , (Brush)new BrushConverter().ConvertFrom("#dbdee1")) ,
                new Channel("", Icons.PetFoot , Brushes.Red),new Channel("", Icons.PetFoot , Brushes.Red)};
            }
            init();
        }
        [RelayCommand]
        private void navigationMasterChannelClick()
        {
            _ea.GetEvent<NavigationChannel>().Publish("MainContentMasterChannel");
        }

        [RelayCommand]
        private void navigationChannelClick(string channelName)
        {
            _ea.GetEvent<NavigationChannel>().Publish(channelName);
        }
        private void init()
        {
            MasterChannel = new Channel("", Icons.D, (Brush)new BrushConverter().ConvertFrom("#dbdee1"));
            List<Channel> initChannel = new List<Channel> { 
                new Channel("", Icons.D , (Brush)new BrushConverter().ConvertFrom("#dbdee1")) ,
                new Channel("", Icons.PetFoot , Brushes.Red),new Channel("", Icons.PetFoot , Brushes.Red)};
            initChannel[0].setAllContentsViewd(true);
            Channels = initChannel;
        }
    }
}
