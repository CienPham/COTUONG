using Microsoft.AspNetCore.SignalR;

namespace BanCov2.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendChessMove(string message)
        {
            await Clients.All.SendAsync("ReceiveChessMove", message);

        }
        public override async Task<Task> OnConnectedAsync()
        {
            string roomId = Context.GetHttpContext().Request.Query["roomId"];
            if (!string.IsNullOrEmpty(roomId))
            {
                roomId = roomId.ToLower().Trim();
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            }

            return base.OnConnectedAsync();
        }
        public override async Task<Task> OnDisconnectedAsync(Exception exception)
        {
            string roomId = Context.GetHttpContext().Request.Query["roomId"];
            if (!string.IsNullOrEmpty(roomId))
            {
                roomId = roomId.ToLower().Trim();
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
