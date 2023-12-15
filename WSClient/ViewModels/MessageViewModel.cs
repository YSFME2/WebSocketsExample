using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSClient.Models;
using WSClient.Services;

namespace WSClient.ViewModels
{
    internal class MessageViewModel
    {
        public ObservableCollection<Message> Messages { get; }
        public string Sender { get; set; }
        readonly WebSocketService wsService;
        public MessageViewModel(WebSocketService webSocketService)
        {
            Messages = new ObservableCollection<Message>();
            wsService = webSocketService;
            wsService.MessageReceived += (s, message) =>
            {
                message.Image = "https://i.pravatar.cc/100?u=wsExample" + message.Sender?.Replace(" ", "");
                if (message.Sender == Sender)
                {
                    message.Alignment = LayoutOptions.End;
                    message.FlowDirection = FlowDirection.RightToLeft;
                }
                Messages.Add(message);
            };
        }
    }
}
