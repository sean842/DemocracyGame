﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TriangleDbRepository;
using NewBlazorProjecct.Shared.DTOs;
using Microsoft.AspNetCore.SignalR;
using NewBlazorProjecct.Server.Hubs;

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
            // לשלוף את כל החוקים
            if (game != null) {

                object lawParam = new {
                    gameID = game.GameID
                };

                string lawQuery = "SELECT * FROM Laws WHERE GameID = @gameID";
                var Laws = await _db.GetRecordsAsync<LawsDTO>(lawQuery, lawParam);
                game.LawList = Laws.ToList();
                if (game.LawList.Count > 0) {
                    return Ok(game);
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
            };
            string insertQuery = "INSERT INTO Groups (GroupName, GameID, Points) VALUES (@groupName, @gameID, @points)";
            group.GroupID = await _db.InsertReturnId(insertQuery, param);
            if(group.GroupID > 0) {
                await _hub.Clients.All.SendAsync("GroupLogin", group);
                return Ok(group.GroupID);
            }
            else {
                return BadRequest("didnt insert");
            }
        }



        [HttpGet("DistributePoints/{gameID}")]
        public async Task<IActionResult> DistributePoints(int gameID) {
            // Fetch all groups for the game
            object param = new {
                GameID = gameID
            };
            string query = "SELECT * FROM Groups WHERE GameID = @GameID";
            var groups = await _db.GetRecordsAsync<Group>(query, param);

            if (groups == null || !groups.Any()) {
                return NotFound("No groups found");
            }

            // Calculate points to distribute
            int totalPoints = 120;
            int pointsPerGroup = totalPoints / groups.Count();

            // Update points for each group
            foreach (var group in groups) {
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

            return Ok("Points distributed successfully");
        }




        [HttpPost("Vote")]
        public async Task<IActionResult> Vote(VoteDTO vote) {
            //object param = new {
            //    LawID = vote.LawID,
            //    GameID = vote.GameID
            //};
            // Prepare the SQL update query based on the vote type
            string updateQuery;
            switch (vote.VoteType) {
                case "For":
                    updateQuery = "UPDATE Laws SET For = For + 1 WHERE LawID = @LawID AND GameID = @GameID";
                    break;
                case "Against":
                    updateQuery = "UPDATE Laws SET Against = Against + 1 WHERE LawID = @LawID AND GameID = @GameID";
                    break;
                case "Avoid":
                    updateQuery = "UPDATE Laws SET Avoid = Avoid + 1 WHERE LawID = @LawID AND GameID = @GameID";
                    break;
                default:
                    return BadRequest("Invalid vote type");
            }
            bool isUpdate = await _db.SaveDataAsync(updateQuery, vote);

            if(isUpdate) {
                // Notify clients about the vote update
                await _hub.Clients.All.SendAsync("VoteUpdated", vote);
                return Ok("Update");
            }
            else {
                return BadRequest( "Failed to update the vote");
            }


        }








    }
}