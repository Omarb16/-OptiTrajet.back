using Microsoft.AspNetCore.SignalR;

namespace OptiTrajet.Hubs
{
    public class SocketHub : Hub, ISocketHub
    {
        private readonly IHubContext<SocketHub> _hubContext;

        public SocketHub(IHubContext<SocketHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendProgress(string connectionId, int progress)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("progress", progress);
        }

        public override async Task OnConnectedAsync()
        {
            await _hubContext.Clients.Client(Context.ConnectionId).SendAsync("connected", Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _hubContext.Clients.Client(Context.ConnectionId).SendAsync("disconnected");
        }
    }
}
