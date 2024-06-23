using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TriangleDbRepository;
using NewBlazorProjecct.Shared.DTOs;

namespace NewBlazorProjecct.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly DbRepository _db;
        public AuthController(DbRepository db) {
            _db = db;
        }

        [HttpPost("login")]
        public async Task<IActionResult> UserLogin(Login userLoginDto) {
            object param = new {
                UserId = 1,
                UserName = userLoginDto.Name,
                Password = userLoginDto.Password
            };
            string Namequery = "SELECT * from Users where Name =@UserName";
            var userRecords = await _db.GetRecordsAsync<User>(Namequery, param);
            User userToLog = userRecords.FirstOrDefault();
            if (userToLog != null) {
                if (userToLog.Password == userLoginDto.Password) {
                    string Passwordquery = "SELECT Password from Users where Name =@UserName";
                    var passwordRecord = await _db.GetRecordsAsync<User>(Passwordquery, param);
                    return Ok(passwordRecord);
                }
                else {
                    return BadRequest("אחד מהפרטים שהוזנו שגוי");
                }
            }
            else {
                return BadRequest("NotGood");
            }
        }
    }
}