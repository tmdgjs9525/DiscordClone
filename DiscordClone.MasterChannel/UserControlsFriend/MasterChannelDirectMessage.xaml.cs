using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscordClone.MasterChannel.UserControlsFriend
{
    /// <summary>
    /// MasterChannelDirectMessage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MasterChannelDirectMessage : UserControl
    {
        public MasterChannelDirectMessage()
        {
            InitializeComponent();
        }

        private void ItemsControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            scroll.ScrollToVerticalOffset(double.MaxValue);
        }
    }
}
