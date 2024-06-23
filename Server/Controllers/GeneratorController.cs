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
            // GET the user.
            object param = new {
                UserID
            };
            string userQuery = "SELECT * from Users WHERE UserID = @UserID";
            var userRecords = await _db.GetRecordsAsync<User>(userQuery, param);
            User myUser = userRecords.FirstOrDefault();
            if (myUser != null) {
                // GET all his games.
                string gameQuery = "SELECT g.GameID, g.GameCode, g.GameName, g.IsPublish, g.ScoreFormat, g.UserID, COUNT(l.LawID) AS LawCount, g.CanPublish FROM  Games as g LEFT JOIN  Laws l ON g.GameID = l.GameID WHERE  g.UserID = UserID GROUP BY  g.GameID, g.GameCode, g.GameName, g.IsPublish, g.ScoreFormat, g.UserID, g.CanPublish";
                var gameRecords = await _db.GetRecordsAsync<GameLawCount>(gameQuery, param);
                myUser.GameList = gameRecords.ToList();
                if(myUser.GameList.Count > 0) {
                    // CHECK if we can publish.
                    foreach (GameLawCount game in myUser.GameList) {
                        bool gameValidation = await canPublish(game.GameID);
                        game.CanPublish = gameValidation;
                        if (gameValidation == false) {
                            game.IsPublish = false;
                        }

                    }
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

        [HttpGet("AddNewLaw/{GameID}/{LawContent}")]
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

        [HttpDelete("DeleteGame/{gameID}")]
        public async Task<IActionResult> DeleteGame(int gameID) {
            object param = new { gameID };
            string query = "DELETE from Games WHERE GameID = @gameID";
            bool isDelete = await _db.SaveDataAsync(query, param);
            if (isDelete) {
                return Ok();
            }
            return BadRequest("Not Delete");
        }


        [HttpPost("PublishGame")]
        public async Task <IActionResult> PublishGame(PublishGame gameToPublish) {
            // Check if we can publish.
            bool gameValidation = await canPublish(gameToPublish.GameID);
            // if we can NOT publish we set IsPublish = false.
            if (gameValidation == false) {
                gameToPublish.IsPublish = false;
            }
            // UPDATE the game.
            object param = new { 
                gameID = gameToPublish.GameID,
                isPublish = gameToPublish.IsPublish,
                canPublish = gameValidation
            };
            string updateQuery = "UPDATE Games SET IsPublish=@isPublish, CanPublish = @canPublish WHERE GameID=@gameID";
            bool isUpdate = await _db.SaveDataAsync(updateQuery, param);
            if (isUpdate == true) {
                // GET the Game.
                object param2 = new {
                    ID = gameToPublish.GameID
                };
                string gameQuery = "sELECT  g.GameID, g.GameCode, g.GameName, g.IsPublish, g.ScoreFormat, g.UserID, COUNT(l.LawID) AS lawsNumber, g.CanPublish FROM Games AS g LEFT JOIN Laws AS l ON g.GameID = l.GameID WHERE g.GameID = @ID GROUP BY g.GameID, g.GameCode, g.GameName, g.IsPublish, g.ScoreFormat, g.UserID, g.CanPublish;";
                var gameRecord2 = await _db.GetRecordsAsync<GameLawCount>(gameQuery, param2);
                GameLawCount gameToReturn = gameRecord2.FirstOrDefault();
                if (gameToReturn != null) {
                    return Ok(gameToReturn);
                }
                return BadRequest("not get game");
            }
            return BadRequest("Update Failed");
        }


        private async Task<bool> canPublish(int gameId) {
            int validAmountOfLaws = 2;
            object param = new { ID = gameId };
            string gameQuery = "SELECT count(*) from Laws where GameID = @ID";
            var gameRecord = await _db.GetRecordsAsync<int>(gameQuery, param);
            int numOfLegitQuestions = gameRecord.FirstOrDefault();
            if (numOfLegitQuestions > validAmountOfLaws) {
                return true; 
            }
            else { return false; }
        }

        [HttpPost("UpdateLaw")]
        public async Task<IActionResult> UpdateLaw(LawsDTO lawToUpdate) {
            string updateQuery = "UPDATE Laws SET Content = @Content WHERE LawID = @LawID";
            bool isUpdate = await _db.SaveDataAsync(updateQuery, lawToUpdate);
            if(isUpdate) {
                string getQuery = "SELECT * FROM Laws WHERE LawID = @LawID";
                var lawRecords = await _db.GetRecordsAsync<LawsDTO>(getQuery, lawToUpdate);
                LawsDTO newLaw = lawRecords.FirstOrDefault();
                if (newLaw != null) {
                    return Ok(newLaw);
                }
                return BadRequest("Didnt Get Law");

            }
            return BadRequest("Didnt Update");
        }








    }
}
