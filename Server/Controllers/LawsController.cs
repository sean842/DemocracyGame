using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TriangleDbRepository;
using NewBlazorProjecct.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;
using NewBlazorProjecct.Server.Hubs;
using SQLitePCL;

namespace NewBlazorProjecct.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class LawsController : ControllerBase {
        private readonly DbRepository _db;
        private readonly IHubContext<ChatHub> _hub;
        public LawsController(DbRepository db, IHubContext<ChatHub> hubContext) {
            _db = db;
            _hub = hubContext;

        }


        // Get the game and all his laws.
        [HttpGet("Game/{gameCode}")]
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
                if (game.LawList.Count > 0) {
                    if (game.LawList.Count > 2) {
                        if(game.IsPublish == true) {

                            return Ok(game);
                        }
                        else { return BadRequest("Didnt Publish Game"); }
                    }
                    else {
                        return BadRequest("Not Enough Laws");
                    }
                }
                else {
                    return BadRequest("No Laws");
                }
            }
            else {
                return BadRequest("No Game Code");
            }

        }

        [HttpGet("GetAllGroups/{gameID}")]
        public async Task<IActionResult> GetAllGroups(int gameID) {
            object param = new {
                GameID = gameID
            };
            string query = "SELECT * FROM Groups WHERE GameID = @GameID";
            var groups = await _db.GetRecordsAsync<Group>(query, param);
            List<Group> groupsList = groups.ToList();
            if (groupsList != null && groupsList.Any()) {
                return Ok(groupsList);
            }
            else {
                return NotFound("No groups found");
            }
        }







        [HttpPost("InsertGroup")]
        public async Task<IActionResult> InsertGroup(Group group) {
            object param = new {
                groupName = group.GroupName,
                gameID = group.GameID,
                points = 0,
                character = group.Character,
            };
            string insertQuery = "INSERT INTO Groups (GroupName, GameID, Points, Character) VALUES (@groupName, @gameID, @points, @character)";
            group.GroupID = await _db.InsertReturnId(insertQuery, param);
            if (group.GroupID > 0) {
                await _hub.Clients.All.SendAsync("GroupLogin", group);
                return Ok(group.GroupID);
            }
            else {
                return BadRequest("didnt insert");
            }
        }


        [HttpGet("DistributePoints/{gameID}/{scoreFormat}")]
        public async Task<IActionResult> DistributePoints2(int gameID, bool scoreFormat) {
            // Call GetAllGroups at the start
            var allGroupsResult = await GetAllGroups(gameID);
            if (allGroupsResult is OkObjectResult okResult) {
                List<Group> groupsList = okResult.Value as List<Group>;
                // reset the points.
                object param = new { GameID = gameID };
                string updateQuery = "UPDATE Groups SET Points = 0 WHERE GameID = @GameID";
                bool isUpdate = await _db.SaveDataAsync(updateQuery, param);

                if (groupsList.Count > 0) {
                    if (scoreFormat) {
                        return await EvenDistribution(groupsList);                        
                    }
                    else {
                        return await RandomDistribution(groupsList);
                    }
                }
            }
            return BadRequest("No groups found");
        }

        private async Task<IActionResult> EvenDistribution(List<Group> groupsList) {
            int totalPoints = 120;
            int pointsPerGroup = totalPoints / groupsList.Count;
            // ... your existing even distribution logic ...
            foreach (Group group in groupsList) {
                object updateParam = new {
                    GroupID = group.GroupID,
                    Points = pointsPerGroup
                };
                string updateQuery = "UPDATE Groups SET Points = @Points WHERE GroupID = @GroupID";
                bool isUpdate = await _db.SaveDataAsync(updateQuery, updateParam);
                if (!isUpdate) {
                    return BadRequest("Failed to update points for group: " + group.GroupName);
                }
            }
            // Notify clients about the points distribution
            await _hub.Clients.All.SendAsync("PointsDistributed");
            return Ok(true);
        }

        private async Task<IActionResult> RandomDistribution(List<Group> groupsList) {
            int totalPoints = 120;
            Random rnd = new Random();
            int remainingPoints = totalPoints;

            while (remainingPoints > 0) {// if i have points to give:
                foreach (Group group in groupsList) {
                    int pointsToDistribute;
                    if (remainingPoints < 5) {
                        pointsToDistribute = remainingPoints;
                        remainingPoints = 0;
                    }
                    else {
                        int maxPoints = Math.Min(21, remainingPoints);
                        pointsToDistribute = rnd.Next(5, maxPoints + 1);
                        remainingPoints -= pointsToDistribute;
                    }

                    object updateParam = new {
                        GroupID = group.GroupID,
                        Points = pointsToDistribute
                    };
                    string updateQuery = "UPDATE Groups SET Points = Points + @Points WHERE GroupID = @GroupID";
                    bool isUpdate = await _db.SaveDataAsync(updateQuery, updateParam);
                    if (!isUpdate) {
                        return BadRequest("Failed to update points for group: " + group.GroupName);
                    }

                    if (remainingPoints == 0) break; // Break out of the loop if all points have been distributed
                }
            }

            // Notify clients about the points distribution
            await _hub.Clients.All.SendAsync("PointsDistributed");
            return Ok(true);
        }

        [HttpPost("Vote")]
        public async Task<IActionResult> Vote(VoteDTO vote) {
            // Prepare the SQL update query based on the vote type
            string updateQuery;
            switch (vote.VoteType) {
                case "For":
                    updateQuery = "UPDATE Laws SET For = For + @Points WHERE LawID = @LawID AND GameID = @GameID";
                    break;
                case "Against":
                    updateQuery = "UPDATE Laws SET Against = Against + @Points WHERE LawID = @LawID AND GameID = @GameID";
                    break;
                case "Avoid":
                    updateQuery = "UPDATE Laws SET Avoid = Avoid + @Points WHERE LawID = @LawID AND GameID = @GameID";
                    break;
                default:
                    return BadRequest("Invalid vote type");
            }
            bool isUpdate = await _db.SaveDataAsync(updateQuery, vote);

            if (isUpdate) {
                // Notify clients about the vote update
                await _hub.Clients.All.SendAsync("VoteUpdated", vote);
                return Ok("Update");
            }
            else {
                return BadRequest("Failed to update the vote");
            }


        }


        [HttpDelete("DeleteAllGroups")]
        public async Task<IActionResult> DeleteAllGroups() {
            object param = new { };
            string deleteQuery = "DELETE FROM Groups";
            bool isDelete = await _db.SaveDataAsync(deleteQuery, param);
            if(isDelete) {
                return Ok();
            }
            return BadRequest("didnt delete!");
        }

        [HttpGet("ResetVotes/{GameID}")]
        public async Task<IActionResult> ResetVotes(int GameID) {
            object param = new { GameID };
            string query = "UPDATE Laws SET For = 0, Avoid = 0, Against = 0 where GameID = @GameID";
            bool isUpdate = await _db.SaveDataAsync(query, param);
            if (isUpdate) {
                return Ok();
            }
            return BadRequest("Not Update");
        }




    }
}
