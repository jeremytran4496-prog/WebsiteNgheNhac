using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebNhac.Data;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace WebNhac.Controllers
{
    [Route("api/tracks")]
    [ApiController]
    public class TracksApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public TracksApiController(ApplicationDbContext db) => _db = db;

        [HttpPost("{id:int}/played")]
        public async Task<IActionResult> Played(int id)
        {
            var song = await _db.Songs.FirstOrDefaultAsync(s => s.Id == id);
            if (song == null) return NotFound();
            song.PlayCount += 1;
            await _db.SaveChangesAsync();
            return Ok(new { song.Id, song.PlayCount });
        }
    }
}
