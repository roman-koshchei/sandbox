using Microsoft.AspNetCore.SignalR;

namespace Sandbox.Hubs;

public class ChatHub : Hub
{
    private const string receiveMessage = "receive-message";

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync(receiveMessage, user, message);
    }
}