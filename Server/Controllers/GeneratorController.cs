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



        [HttpGet("AddGame/{GameName}")]
        public async Task<IActionResult> AddNewGame(int UserID, string GameName) {
            object param = new {
                GameCode = 0,
                GameName,
                IsPublish = false,
                ScoreFormat = false,
                UserID
            };
            string addGameQuery = "INSERT INTO Games (GameCode, GameName, IsPublish, ScoreFormat, UserID) VALUES (@GameCode, @GameName, @IsPublish, @ScoreFormat, @UserID)";
            int gameID = await _db.InsertReturnId(addGameQuery, param);
            if (gameID > 0) {
                // We Update The GameCode.
                int newGameCode = gameID + 100;
                object param2 = new {
                    GameID = gameID,
                    GameCode = newGameCode
                };
                string query2 = "UPDATE Games SET GameCode = @GameCode WHERE GameID=@GameID";
                bool isUpdate = await _db.SaveDataAsync(query2, param2);
                if (isUpdate) {
                    // Get the game and send it back.
                    string getGameQuery = "SELECT GameID , GameName, GameCode, IsPublish, ScoreFormat, UserID FROM Games WHERE GameID = @GameID";
                    var gameRecors = await _db.GetRecordsAsync<GameLawCount>(getGameQuery, param2);
                    GameLawCount newGame = gameRecors.FirstOrDefault();
                    if(newGame!= null) {
                        return Ok(newGame);
                    }
                    return BadRequest("Not Get");
                }
                return BadRequest("Not Update");

            }
            return BadRequest("Not Insert");



        }




    }
}
