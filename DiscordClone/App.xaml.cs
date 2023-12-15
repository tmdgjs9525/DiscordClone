using DiscordClone.SideBars.Views;
using DiscordClone.Views;
using Prism.Ioc;
using System.Windows;

namespace DiscordClone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<MainWindow>();
            containerRegistry.RegisterSingleton<SideChannelRadioButton>();

            //containerRegistry.RegisterSingleton<MainContentMasterChannel>();
            //containerRegistry.RegisterSingleton<MasterChannelStateViewModel>();
            //containerRegistry.RegisterForNavigation<MainContentMasterChannel, MasterChannelStateViewModel>();

            //containerRegistry.RegisterSingleton<MasterChannelFriendList>();
            //containerRegistry.RegisterSingleton<MasterChannelFriendListViewModel>();
            //containerRegistry.RegisterForNavigation<MasterChannelFriendList, MasterChannelFriendListViewModel>();



            //containerRegistry.RegisterSingleton<NitroMain>();
            //containerRegistry.RegisterSingleton<NitroMainViewModel>();
            //containerRegistry.RegisterForNavigation<NitroMain, NitroMainViewModel>();
        }
        //protected override void ConfigureViewModelLocator()
        //{
        //    base.ConfigureViewModelLocator();
        //    Prism.Mvvm.ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
        //    {
        //        string viewName = viewType.FullName;
        //        if(viewName == null)
        //        {
        //            return null;
        //        }
        //        if(viewName.EndsWith("View"))
        //        {
        //            viewName = viewName.Substring(0, viewName.Length - 4);
        //        }
        //        viewName = viewName.Replace(".Views.",".ViewModels.");
        //        //viewName = viewName.Replace(".Controls.","ControlViewModels.);
        //        string viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
        //        string viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";
        //        return Type.GetType(viewModelName);
        //    });
        //}
    }
}
