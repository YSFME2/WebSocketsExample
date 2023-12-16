using WSClient.Models;
using WSClient.Services;
using WSClient.ViewModels;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;

namespace WSClient
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var messageViewModel = new MessageViewModel();
            this.Disappearing += async (s, e) => { messageViewModel.DisconnectCommand.Execute(null); };
            this.BindingContext = messageViewModel;
            //messageViewModel.Messages.Add(new Message { Sender = "Youssef", Text = "Hello\nIt's me from your future.\nPlease don't west your time.", Date = DateTime.Now });
            //messageViewModel.Messages.Add(new Message { Sender = "Youssef", Text = "Hello\nIt's me from your future.\nPlease don't west your time.", Date = DateTime.Now, Alignment = LayoutOptions.End, FlowDirection = FlowDirection.RightToLeft });
        }
    }

}
