using WSClient.Models;
using WSClient.Services;
using WSClient.ViewModels;

namespace WSClient
{
    public partial class MainPage : ContentPage
    {
        readonly MessageViewModel messageViewModel;
        readonly WebSocketService webSocketService;
        public MainPage()
        {
            InitializeComponent();
            webSocketService = new WebSocketService();
            messageViewModel = new MessageViewModel(webSocketService);
            this.Disappearing += async (s, e) => { await webSocketService.CloseConnection(); };
            this.BindingContext = messageViewModel;
            //messageViewModel.Messages.Add(new Message { Sender = "Youssef", Text = "Hello\nIt's me from your future.\nPlease don't west your time.", Date = DateTime.Now });
            //messageViewModel.Messages.Add(new Message { Sender = "Youssef", Text = "Hello\nIt's me from your future.\nPlease don't west your time.", Date = DateTime.Now, Alignment = LayoutOptions.End, FlowDirection = FlowDirection.RightToLeft });
        }

        private async void btnConnect_Clicked(object sender, EventArgs e)
        {
            if (await webSocketService.Connect(txtSender.Text))
            {
                btnConnect.IsEnabled = false;
                btnDisconnect.IsEnabled = btnSend.IsEnabled = true;
                txtSender.IsEnabled = false;
                txtMessage.IsEnabled = true;
            }
        }

        private async void btnDisconnect_Clicked(object sender, EventArgs e)
        {
            await webSocketService.CloseConnection();

            btnConnect.IsEnabled = true;
            btnDisconnect.IsEnabled = btnSend.IsEnabled = false;
            txtSender.IsEnabled = true;
            txtMessage.IsEnabled = false;
        }

        private async void btnSend_Clicked(object sender, EventArgs e)
        {
            btnSend.IsEnabled = false;
            await webSocketService.SendMessage(new Message { Sender = txtSender.Text, Text = txtMessage.Text,Date = DateTime.Now });
            btnSend.IsEnabled = true;
            txtMessage.Text = "";
        }
    }

}
