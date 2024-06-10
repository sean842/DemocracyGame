using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NewBlazorProjecct.Server.Hubs;
using TriangleDbRepository;

namespace NewBlazorProjecct.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly DbRepository _db;
        private readonly IHubContext<ChatHub> _hub;

        public ChatController(DbRepository db, IHubContext<ChatHub> hubContext)
        {
            _db = db;
            _hub = hubContext;
        }




    }
}
