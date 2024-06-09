using DemocracyGame.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TriangleDbRepository;

namespace DemocracyGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LawsController : ControllerBase
    {
        //private readonly HttpClient _client;
        private readonly DbRepository _db;
        public LawsController(DbRepository db/*, HttpClient client*/)
        {
            _db = db;
            //_client = client;
        }
        [HttpGet]
        public async Task<IActionResult> GetLawsContent()
        {
            object param = new
            {
                id = 2
            };

            //string query = "SELECT FullName, Experience, StudyField FROM Lecturers WHERE Email = @email";
            string query = "SELECT Content FROM Laws WHERE LawID = @id";
            var records = await _db.GetRecordsAsync<LawsDTO>(query, param);
            LawsDTO lect = records.FirstOrDefault();
            return Ok(lect);
        }

    }
}
