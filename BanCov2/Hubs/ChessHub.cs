// ChessHub.cs
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChessHub : Hub
{
    // Dictionary để lưu trữ thông tin về các phòng chơi
    private static Dictionary<string, RoomInfo> _rooms = new Dictionary<string, RoomInfo>();

    public class RoomInfo
    {
        public string RoomId { get; set; }
        public string Player1ConnectionId { get; set; }
        public string Player2ConnectionId { get; set; }
        public bool IsGameStarted { get; set; }
    }

    // Phương thức xử lý khi người chơi kết nối
    public override async Task OnConnectedAsync()
    {
        var roomId = Context.GetHttpContext().Request.Query["roomId"].ToString();
        var connectionId = Context.ConnectionId;

        if (!_rooms.ContainsKey(roomId))
        {
            // Tạo phòng mới nếu chưa tồn tại
            _rooms[roomId] = new RoomInfo
            {
                RoomId = roomId,
                Player1ConnectionId = connectionId,
                IsGameStarted = false
            };
            await Groups.AddToGroupAsync(connectionId, roomId);
            await Clients.Caller.SendAsync("PlayerJoined", "1"); // Player 1 = Quân đỏ
        }
        else if (_rooms[roomId].Player2ConnectionId == null)
        {
            // Thêm người chơi thứ 2 vào phòng
            _rooms[roomId].Player2ConnectionId = connectionId;
            _rooms[roomId].IsGameStarted = true;
            await Groups.AddToGroupAsync(connectionId, roomId);
            await Clients.Caller.SendAsync("PlayerJoined", "2"); // Player 2 = Quân đen
            await Clients.Group(roomId).SendAsync("GameStarted");
        }
        else
        {
            // Phòng đã đầy
            await Clients.Caller.SendAsync("RoomFull");
            Context.Abort();
        }

        await base.OnConnectedAsync();
    }

    // Phương thức xử lý khi người chơi ngắt kết nối
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var roomId = _rooms.FirstOrDefault(r =>
            r.Value.Player1ConnectionId == Context.ConnectionId ||
            r.Value.Player2ConnectionId == Context.ConnectionId).Key;

        if (roomId != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("PlayerDisconnected");
            _rooms.Remove(roomId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Phương thức xử lý nước đi
    public async Task MakeMove(string roomId, string moveData)
    {
        if (_rooms.ContainsKey(roomId) && _rooms[roomId].IsGameStarted)
        {
            // Gửi nước đi đến tất cả người chơi trong phòng, trừ người gửi
            await Clients.OthersInGroup(roomId).SendAsync("ReceiveChessMove", moveData);
        }
    }

    // Phương thức thông báo kết thúc game
    public async Task EndGame(string roomId, string winner)
    {
        if (_rooms.ContainsKey(roomId))
        {
            await Clients.Group(roomId).SendAsync("GameEnded", winner);
            _rooms.Remove(roomId);
        }
    }
}