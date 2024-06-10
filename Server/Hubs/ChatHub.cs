using Microsoft.AspNetCore.SignalR;
using NewBlazorProjecct.Shared.DTOs;

namespace NewBlazorProjecct.Server.Hubs {
    public class ChatHub : Hub {

        public async Task StartGame() {
            await Clients.All.SendAsync("StartTheGame");
        }

        public async Task StartVote() {
            await Clients.All.SendAsync("StartVote");
        }

        // לא עובד





    }
}
