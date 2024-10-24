using Microsoft.AspNetCore.SignalR;
using NewBlazorProjecct.Shared.DTOs;

namespace NewBlazorProjecct.Server.Hubs {
    public class ChatHub : Hub {


        // פונקציה המעדכנת את מזהה המשתמש לאחר התחברות מחדש
        public async Task Reconnected(Group group) {
            //שליחת המשתמש
            await Clients.All.SendAsync("GroupReconnected", group);
        }

        public async Task SendLawCounter(ReconnectingData Data, string TargetConnectionId) {
            
            await Clients.Client(TargetConnectionId).SendAsync("GetData", Data);
        }


        // פונקציה המוסיפה משתמש לקבוצה- מקבלת משתמש ומזהה קבוצה
        public async Task Join(Group user, string GroupName) {
            //הוספה לקבוצה
            await Groups.AddToGroupAsync(user.ConnectionID, GroupName);

            //שליחת הודעה לקבוצה
            await Clients.Group(GroupName).SendAsync("JoinGroup", GroupName, user);
        }

        // מוסיף את המרצה לקבוצה
        public async Task JoinLecturer(string ConnectionID, string GroupName) {
            //הוספה לקבוצה
            await Groups.AddToGroupAsync(ConnectionID, GroupName);

            string lecturemsg = $" lecturer is looged in to group name: {GroupName}, connectionID: {ConnectionID}";

            //שליחת הודעה לקבוצה
            await Clients.Group(GroupName).SendAsync("LecturJoin", lecturemsg);

        }


        public async Task StartVote(string GroupName) {
            await Clients.Group(GroupName).SendAsync("StartVote");
        }

        public async Task ShowChart(string GroupName) {
            await Clients.Group(GroupName).SendAsync("showPieChart");
        }

        public async Task ShowTheNextLaw(string GroupName) {
            await Clients.Group(GroupName).SendAsync("ShowNextLaw");
        }

        public async Task StartAgain(string GroupName) {
            await Clients.Group(GroupName).SendAsync("StartAgain");
        }


    }
}
