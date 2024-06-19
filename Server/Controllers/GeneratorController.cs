using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TriangleDbRepository;
using NewBlazorProjecct.Shared.DTOs;

namespace NewBlazorProjecct.Server.Controllers {
    [Route("api/[controller]/{UserID}")]
    [ApiController]
    public class GeneratorController : ControllerBase {

        private readonly DbRepository _db;
        public GeneratorController(DbRepository db) {
            _db = db;
        }

        [HttpGet("GetGames")]
        public async Task<IActionResult> GetAllGamesByUserID(int UserID) {
            object param = new {
                UserID
            };
            string userQuery = "SELECT * from Users WHERE UserID = @UserID";
            var userRecords = await _db.GetRecordsAsync<User>(userQuery, param);
            User myUser = userRecords.FirstOrDefault();
            if (myUser != null) {
                //string gameQuery = "SELECT * from Games WHERE UsersID = @UserID";
                string gameQuery = "SELECT g.GameID, g.GameCode, g.GameName, g.IsPublish, g.ScoreFormat, g.UserID, COUNT(l.LawID) AS LawCount FROM  Games as g LEFT JOIN  Laws l ON g.GameID = l.GameID WHERE  g.UserID = UserID GROUP BY  g.GameID, g.GameCode, g.GameName, g.IsPublish, g.ScoreFormat, g.UserID";
                var gameRecords = await _db.GetRecordsAsync<GameLawCount>(gameQuery, param);
                myUser.GameList = gameRecords.ToList();
                if(myUser.GameList.Count > 0) {
                    return Ok(myUser);
                }
                // אולי להחזיר רת המשתמש פשוט
                return BadRequest("No Games");
            }
            return BadRequest("No User");
        }



    }
}
