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



        //// Get the game and all his laws.
        [HttpGet("Game/{gameCode}")]
        public async Task<IActionResult> GetGame(int gameCode) {
            var param = new { GameCode = gameCode };
            const string query = "SELECT * FROM Games WHERE GameCode = @GameCode";

            var records = await _db.GetRecordsAsync<Game>(query, param);
            var game = records.FirstOrDefault();
            if (game == null)
                return BadRequest("No Game Code");

            var lawParam = new { gameID = game.GameID };
            const string lawQuery = "SELECT * FROM Laws WHERE GameID = @gameID";
            var laws = await _db.GetRecordsAsync<LawsDTO>(lawQuery, lawParam);
            game.LawList = laws.ToList();

            if (!game.LawList.Any())
                return BadRequest("No Laws");

            if (game.LawList.Count <= 2)
                return BadRequest("Not Enough Laws");

            if (!game.IsPublish)
                return BadRequest("Didnt Publish Game");

            return Ok(game);
        }




        // Start the game so we can play.
        [HttpGet("ChangeGameStarted/{gameID}/{gameStarted}")]
        public async Task<IActionResult> ChangeGameStarted(int gameID, bool gameStarted) {
            var param = new { gameID, gameStarted };
            const string updateQuery = "UPDATE Games SET GameStarted = @gameStarted WHERE GameID = @gameID";
            var isUpdate = await _db.SaveDataAsync(updateQuery, param);
            if (isUpdate)
                return Ok();
            return BadRequest("Did Not Update GameStarted");
        }



        [HttpGet("GetAllGroups/{gameID}")]
        public async Task<IActionResult> GetAllGroups(int gameID) {
            var param = new { GameID = gameID };
            const string query = "SELECT * FROM Groups WHERE GameID = @GameID";
            var groups = await _db.GetRecordsAsync<Group>(query, param);
            if (groups != null && groups.Any())
                return Ok(groups);
            return BadRequest("No Groups In Game or invalid GameID");
        }

        [HttpPost("InsertGroup")]
        public async Task<IActionResult> InsertGroup(Group group)
        {
            //var param = new { group.GroupName, group.GameID, points = 0, group.Character };
            object param = new { 
                groupName = group.GroupName,  gameID = group.GameID,  points = 0, character = group.Character, passportCharacter = group.PassportCharacter
            };
            const string insertQuery = "INSERT INTO Groups (GroupName, GameID, Points, Character, PassportCharacter) VALUES (@groupName, @gameID, @points, @character, @passportCharacter)";
            group.GroupID = await _db.InsertReturnId(insertQuery, param);
            if (group.GroupID > 0)
            {
                //await _hub.Clients.All.SendAsync("GroupLogin", group);
                return Ok(group.GroupID);
            }
            return BadRequest("didn't insert");
        }

        [HttpGet("DistributePoints/{gameID}/{scoreFormat}")]
        public async Task<IActionResult> DistributePoints(int gameID, bool scoreFormat) {
            var allGroupsResult = await GetAllGroups(gameID); // Call GetAllGroups at the start
            if (allGroupsResult is OkObjectResult okResult) {
                var groupsList = okResult.Value as List<Group>;
                if (groupsList == null || !groupsList.Any())
                    return BadRequest("No groups found");

                var param = new { GameID = gameID };// reset the points.
                const string updateQuery = "UPDATE Groups SET Points = 0 WHERE GameID = @GameID";
                var isUpdate = await _db.SaveDataAsync(updateQuery, param);
                if (!isUpdate)
                    return BadRequest("Failed to reset points");

                return scoreFormat ? await EvenDistribution(groupsList) : await RandomDistribution(groupsList);
            }
            return BadRequest("No groups found");
        }


        private async Task<IActionResult> EvenDistribution(List<Group> groupsList) {
            const int totalPoints = 120;
            var pointsPerGroup = totalPoints / groupsList.Count;
            foreach (var group in groupsList) {
                var updateParam = new { group.GroupID, Points = pointsPerGroup };
                const string updateQuery = "UPDATE Groups SET Points = @Points WHERE GroupID = @GroupID";
                var isUpdate = await _db.SaveDataAsync(updateQuery, updateParam);
                if (!isUpdate)
                    return BadRequest($"Failed to update points for group: {group.GroupName}");
            }
            //await _hub.Clients.All.SendAsync("PointsDistributed");

            string groupname = groupsList[0].GameID.ToString();
            //שליחת הודעה לקבוצה
            await _hub.Clients.Group(groupname).SendAsync("PointsDistributed");

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
                        int maxPoints = Math.Min(15, remainingPoints);
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
            //await _hub.Clients.All.SendAsync("PointsDistributed");

            string groupname = groupsList[0].GameID.ToString();
            //שליחת הודעה לקבוצה
            await _hub.Clients.Group(groupname).SendAsync("PointsDistributed");

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
                string gameName = vote.GameID.ToString();
                await _hub.Clients.Group(gameName).SendAsync("VoteUpdated", vote);
                return Ok("Update");
            }
            else {
                return BadRequest("Failed to update the vote");
            }


        }


        [HttpDelete("DeleteAllGroups/{gameID}")]
        public async Task<IActionResult> DeleteAllGroups(int gameID) {
            var allGroupsResult = await GetAllGroups(gameID); // Call GetAllGroups at the start
            if (allGroupsResult is OkObjectResult okResult) {
                var groupsList = okResult.Value as List<Group>;
                if (groupsList == null || !groupsList.Any()) {
                    return Ok("No Groups");
                }

                var param = new { GameID = gameID };
                const string deleteQuery = "DELETE FROM Groups WHERE GameID = @GameID";
                bool isDelete = await _db.SaveDataAsync(deleteQuery, param);
                if (isDelete) {
                    return Ok("Groups Deleted");
                }
                return BadRequest("Didn't delete groups!");
            }
            return Ok("Failed to retrieve groups");
        }


        // Close the game so we can not play.
        [HttpGet("CloseAllGames/{userID}")]
        public async Task<IActionResult> CloseAllGames(int userID) {
            var param = new { userID, gameStarted = false };
            const string updateQuery = "UPDATE Games SET GameStarted = @gameStarted WHERE UserID = @userID";
            var isUpdate = await _db.SaveDataAsync(updateQuery, param);
            if (isUpdate)
                return Ok();
            return BadRequest("Did Not Update All Games");
        }



        [HttpGet("ResetVotes/{GameID}")]
        public async Task<IActionResult> ResetVotes(int GameID) {
            object param = new { GameID };
            string query = "UPDATE Laws SET For = 0, Avoid = 0, Against = 0, IsPass = false WHERE GameID = @GameID";
            bool isUpdate = await _db.SaveDataAsync(query, param);
            if (isUpdate) {
                return Ok();
            }
            return BadRequest("Not Update");
        }










    }
}
