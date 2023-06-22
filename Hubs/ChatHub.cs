using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Social_Media_Website.Hubs
{
    public class ChatHub:Hub    
    {
        public async Task SendMessage(string user ,/* string ProfileImageUrl,*/ string message)
        {
            await Clients.All.SendAsync("Recive Message", /*ProfileImageUrl,*/user ,message);
        }
    }
}
