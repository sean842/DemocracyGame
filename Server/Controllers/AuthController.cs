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
                UserName = userLoginDto.Name,
                password = userLoginDto.Password
            };
            string Namequery = "SELECT * FROM Users WHERE Name = @UserName And Password = @password";
            var userRecords = await _db.GetRecordsAsync<User>(Namequery, param);
            User userToLog = userRecords.FirstOrDefault();
            if (userToLog != null) {
                if (userToLog.Password == userLoginDto.Password) {
                    if (userToLog.Name == userLoginDto.Name) {

                        return Ok(userToLog);
                    }
                    else { return BadRequest("השם משתמש לא תואם"); }
                }
                else { return BadRequest("הסיסמה לא תואמת"); }
            }
            else { return BadRequest("אין משתמש כזה"); }
        }




        [HttpPost("checkUserExists")]
        public async Task<IActionResult> CheckUserExists(Login userLoginDto) {
            object param = new {
                UserName = userLoginDto.Name,
                password = userLoginDto.Password
            };
            string query = "SELECT * FROM Users WHERE Name = @UserName AND Password = @password";
            var userRecords = await _db.GetRecordsAsync<User>(query, param);
            User existingUser = userRecords.FirstOrDefault();

            if (existingUser != null) {
                return BadRequest("User already exists");
            }

            return Ok();
        }



        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(Login newUserDto) {
            // Check if user already exists using the CheckUserExists method
            var result = await CheckUserExists(newUserDto);

            if (result is BadRequestObjectResult) {
                return BadRequest("User already exists");
            }

            // Insert new user into the database if it does not exist
            object param = new {
                UserName = newUserDto.Name,
                password = newUserDto.Password
            };
            string insertQuery = "INSERT INTO Users (Name, Password) VALUES (@UserName, @password)";
            int NewUserID = await _db.InsertReturnId(insertQuery, param);

            if (NewUserID > 0) {
                return Ok();
            }
            else { return BadRequest("didnt insert new user"); }
        }



    }
}