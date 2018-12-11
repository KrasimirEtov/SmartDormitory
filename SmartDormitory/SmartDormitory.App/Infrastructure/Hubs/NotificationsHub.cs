using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Hubs
{
    public class NotificationsHub : Hub
    {
       

        //public Task SendMessageToCaller(string message)
        //{
        //    return Clients.Caller.SendAsync("ReceiveMessage", message);
        //}

        //public Task SendMessageToGroups(string message)
        //{
        //    List<string> groups = new List<string>() { "SignalR Users" };
        //    return Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        //}

        //public override async Task OnConnectedAsync()
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
        //    await base.OnDisconnectedAsync(exception);
        //}
    }
}
