
using Microsoft.AspNetCore.SignalR;

namespace SignalRMVCApp.SignalRHub;

public class ChatHub : Hub<IChatClient>
{
    private static Dictionary<string, string> _connections = new();

    // key -connectionID  value- username
    public override async Task OnConnectedAsync()
    {
        string userId = Context.ConnectionId; // we are setting key as userId for _connections Dictionary
        _connections[userId] = "";
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string userId = Context.ConnectionId;
        _connections.Remove(userId);
        await SendConnectedUsers();
        await base.OnDisconnectedAsync(exception);
    }

    // these method client can call on server
    public async Task JoinChat(string userName)
    {
        string userId = Context.ConnectionId;
        _connections[userId] = userName;
        await SendConnectedUsers();
        await Clients.All.NewUserJoined(userName);
    }

    //client call SendMessageToUser  on Server and in response we call ReceiveMessage on Client
    public async Task SendMessageToUser(string toUser, string message)
    {
        var fromUserId = Context.ConnectionId;
        var fromUser = _connections[fromUserId];

        var userId = _connections.FirstOrDefault(x => x.Value == toUser).Key;
        if (userId != null)
            await Clients.Client(userId).ReceiveMessage(fromUser, message);
    }

     public async Task BroadCastMessage( string message)
    {
        var fromUserId = Context.ConnectionId;
        var fromUser = _connections[fromUserId];
        await Clients.All.ReceiveMessage(fromUser, message);
    }

    private async Task SendConnectedUsers()
    {
        var connectedUsers = _connections.Values.ToList();
        await Clients.All.GetConnectedUsers(connectedUsers);
    }
}
