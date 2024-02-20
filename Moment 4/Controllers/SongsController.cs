using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moment_4.Data;
using Moment_4.Models;

namespace Moment_4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly MainContext _context;

        public SongsController(MainContext context)
        {
            _context = context;
        }

        // GET: api/Songs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            return await _context.Songs.ToListAsync();
        }

        // GET: api/Songs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            return song;
        }

        // PUT: api/Songs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSong(int id, Song song)
        {
            if (id != song.Id)
            {
                return BadRequest();
            }

            _context.Entry(song).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Songs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Song>> PostSong(Song song, [FromForm] IFormFile imageFile)
        {
            //kontrollerar att det finns en fil som har blivit uppladdad
            if (imageFile != null && imageFile.Length > 0)
            {
                //Genererar unik filpath, lik den för laravel för att sedan spara den i wwwrootes/images. Unika koden genereras mha GUID, och ersätter original bildens namn.


                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName));
                
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                    song.ImageFilePath = imagePath; // Set the file path on the song object
                }
            }


            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSong", new { id = song.Id }, song);
        }

        // DELETE: api/Songs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }
    }
}
