using Microsoft.AspNetCore.SignalR;

namespace PickleBall.Service.SignalR
{
    public class BookingHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task LeaveCourtGroup(string courtId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, courtId);
        }
    }
}
