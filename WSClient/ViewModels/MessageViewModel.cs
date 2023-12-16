using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WSClient.Models;
using WSClient.Services;

namespace WSClient.ViewModels
{
    internal class MessageViewModel : BaseViewModel
    {
        readonly WebSocketService webSocketService;
        private bool isConnected;
        private bool isDisconnected;
        private bool isSendingEnabled;
        private bool isSendingMessage;
        private string message;

        public ObservableCollection<Message> Messages { get; }
        public string Sender { get; set; }


        public bool IsConnected
        {
            get => isConnected;
            private set
            {
                SetProperty(ref isConnected, value);
                IsDisconnected = !value;
                IsSendingEnabled = value && !IsSendingMessage;
            }
        }
        public bool IsDisconnected { get => isDisconnected; private set => SetProperty(ref isDisconnected, value); }
        public bool IsSendingEnabled { get => isSendingEnabled; private set { SetProperty(ref isSendingEnabled, value); } }

        private bool IsSendingMessage { get => isSendingMessage; set { isSendingMessage = value; IsSendingEnabled = IsConnected && !value; } }

        public string Message { get => message; set => SetProperty(ref message, value); }
        public ICommand ConnectCommand { get; private set; }
        public ICommand DisconnectCommand { get; private set; }
        public ICommand SendCommand { get; private set; }
        public MessageViewModel()
        {
            Messages = new ObservableCollection<Message>();
            webSocketService = new WebSocketService();
            webSocketService.MessageReceived += (s, message) =>
            {
                message.Image = "https://i.pravatar.cc/100?u=wsExample" + message.Sender?.Replace(" ", "");
                message.FlowDirection = message.Sender == Sender ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                Messages.Add(message);
            };
            IsConnected = false;

            ConnectCommand = new Command(OnConnectExecuted);
            DisconnectCommand = new Command(OnDisconnectExecuted);
            SendCommand = new Command(OnSendExecuted);
        }

        private async void OnConnectExecuted()
        {
            if (await webSocketService.Connect(Sender))
            {
                IsConnected = true;
            }
        }

        private async void OnDisconnectExecuted()
        {
            await webSocketService.CloseConnection();
            IsConnected = false;
        }

        private async void OnSendExecuted()
        {
            IsSendingMessage = true;
            await webSocketService.SendMessage(new Message { Sender = Sender, Text = Message, Date = DateTime.Now });
            IsSendingMessage = false;
            Message = "";
        }
    }
}
