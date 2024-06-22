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


        // Get one game and all his laws.
        [HttpGet("GetOneGame/{gameCode}")]
        public async Task<IActionResult> GetGame(int gameCode) {
            object param = new {
                GameCode = gameCode
            };
            string query = "SELECT * FROM Games WHERE GameCode = @GameCode";

            var recordes = await _db.GetRecordsAsync<Game>(query, param);
            Game game = recordes.FirstOrDefault();
            if (game != null) {
                // לשלוף את כל החוקים
                object lawParam = new {
                    gameID = game.GameID
                };
                string lawQuery = "SELECT * FROM Laws WHERE GameID = @gameID";
                var Laws = await _db.GetRecordsAsync<LawsDTO>(lawQuery, lawParam);
                game.LawList = Laws.ToList();
                return Ok(game);
            }
            else {
                return BadRequest("No Game Code");
            }

        }

        [HttpGet("SaveLaw/{GameID}/{LawContent}")]
        public async Task<IActionResult> SaveLaw(int GameID, string LawContent) {
            object param = new {
                GameID,
                LawContent,
                For = 0,
                Againt = 0,
                Avoid = 0,
                IsPass = false
            };
            string query = "INSERT INTO Laws (GameID, Content, For, Against, Avoid, IsPass) VALUES (@GameID, @LawContent, @For, @Againt, @Avoid, @Avoid)";
            int newLawID = await _db.InsertReturnId(query, param);
            if (newLawID > 0) {
                object IdParam = new { newLawID };
                string getLawQuery = "SELECT * FROM Laws WHERE LawID = @newLawID";
                var lawRecords = await _db.GetRecordsAsync<LawsDTO>(getLawQuery, IdParam);
                LawsDTO newLaw = lawRecords.FirstOrDefault();
                if (newLaw != null) {
                    return Ok(newLaw);
                }
                return BadRequest("Not Get Law");
            }
            return BadRequest("Not Save Law");


        }



    }
}
