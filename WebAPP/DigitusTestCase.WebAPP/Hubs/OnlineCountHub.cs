using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DigitusTestCase.WebAPP.Hubs
{
    [Authorize]
    public class OnlineCountHub : Hub
    {
        private static readonly Dictionary<string, int> onlineUsers = new Dictionary<string, int>();
        private static int Count = 0;
        public Task<ConnectionOpenedResult> ConnectionOpened(string userId)
        {
            var joined = false;
            lock (onlineUsers)
            {
                if (onlineUsers.ContainsKey(userId))
                {
                    onlineUsers[userId] += 1;
                }
                else
                {
                    onlineUsers.Add(userId, 1);
                    joined = true;
                }
            }
            return Task.FromResult(new ConnectionOpenedResult { UserJoined = joined });
        }
        public Task<ConnectionClosedResult> ConnectionClosed(string userId)
        {
            var left = false;
            lock (onlineUsers)
            {
                if (onlineUsers.ContainsKey(userId))
                {
                    onlineUsers[userId] -= 1;
                    if (onlineUsers[userId] <= 0)
                    {
                        onlineUsers.Remove(userId);
                        left = true;
                    }
                }
            }

            return Task.FromResult(new ConnectionClosedResult { UserLeft = left });
        }
        public int GetOnlineUsersCount()
        {
            lock (onlineUsers)
            {
                return onlineUsers.Count;
            }
        }
        public override Task OnConnectedAsync()
        {
          
            ConnectionOpened(Context.GetHttpContext().Request.Cookies[".AspNetCore.Identity.Application"]);
            base.OnConnectedAsync();
            Clients.All.SendAsync("updateCount", GetOnlineUsersCount());
            return Task.CompletedTask;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {

            ConnectionClosed(Context.GetHttpContext().Request.Cookies[".AspNetCore.Identity.Application"]);
            base.OnDisconnectedAsync(exception);
            Clients.All.SendAsync("updateCount", GetOnlineUsersCount());
            return Task.CompletedTask;
        }
    }
    public class ConnectionOpenedResult
    {
        public bool UserJoined { get; set; }
    }

    public class ConnectionClosedResult
    {
        public bool UserLeft { get; set; }
    }
}
 
