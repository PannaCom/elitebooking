using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading;
using System.Threading.Tasks;
namespace HotelGolfBooking
{
    public class ChatHub : Hub
    {
        //public void Hello()
        //{
        //    Clients.All.hello();
        //}
        //public void Send(string name, string message)
        //{
        //    // Call the broadcastMessage method to update clients.
        //    Clients.All.broadcastMessage(name, message);
        //}
        private readonly TimeSpan BroadcastInterval =
            TimeSpan.FromMilliseconds(2000);
        private Timer _broadcastLoop;
        private bool isRunning = false;

        public async Task JoinRoom(string roomName,string contents)
        {
            await Groups.Add(Context.ConnectionId, roomName);
            Clients.Group(roomName).addChatMessage(roomName,contents);
        }
        //public async Task broadCast(string roomName, string contents)
        //{
        //    await Groups.Add(Context.ConnectionId, roomName);
        //    Clients.Group(roomName).broadCast(roomName, contents);
        //}
        //public async Task noticeNews(string roomName)
        //{
        //    await Groups.Add(Context.ConnectionId, roomName);
        //    Clients.Group(roomName).noticeNews(roomName);
        //}
        //public async Task LeaveRoom(string roomName)
        //{
        //    await Groups.Remove(Context.ConnectionId, roomName);
        //}
        //public async Task LeaveRoomComment(string token)
        //{
        //    await Groups.Remove(Context.ConnectionId, token);
        //}
    }
}