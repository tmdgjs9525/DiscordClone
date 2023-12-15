using DiscordClone.MainChannel.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DiscordClone.MainChannel
{
    public class MainChannelModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<MainChannelContent>();

        }
    }
}