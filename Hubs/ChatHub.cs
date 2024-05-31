using Microsoft.AspNetCore.SignalR;

namespace DemocracyGame.Hubs
{
    public class ChatHub : Hub
    {
        ////פונקציה שפועלת בהתחברות
        //public async Task Login(string user)
        //{
        //    await Clients.All.SendAsync("UserLogin", user);
        //}

        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}

        public async Task SendVote(string voteType)
        {
            // Process the received vote (e.g., store it, broadcast it to other clients)
            await Clients.All.SendAsync("ReceiveVote", voteType);
        }

        public async Task CreateTheChart()
        {
            await Clients.All.SendAsync("CreateThePieChart");
        }
        //public async Task NextLaw(int nextLawIndex)
        //{
        //    await Clients.All.SendAsync("GetNextLaw", nextLawIndex);
        //}

    }
}
