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

        [HttpPut("{id}")]
        /*OBS Song song och ImageFilePath är vad POST request förväntas tas emot. Dvs två olika inputs. Därför måste song.ImageFilePath = imagePath.
         Ytterligare, så måste [FromForm] användas för att kombinera olika typer av data. Egentligen borde imagefilepath borde vara imagefile.*/
        public async Task<IActionResult> PutSong(int id, [FromForm] Song song, [FromForm] IFormFile imageFile = null)
        {


            if (id != song.Id)
            {
                return BadRequest("The ID in the URL does not match the ID of the song.");
            }

            //Kontrollerar att kategorin som skickas med song finns i kateogrin tabellen (id), retunerar true/false
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == song.CategoryId);

            //Kontrollerar först om song validerar korrekt, senare om kategorin inte finns
            if (song == null || !categoryExists)
            {
                return BadRequest("Invalid categoryId");
            }

            //Hämta låten som har valts, spara data i en var.
            var selectedSongById = await _context.Songs.FindAsync(id);

            //kontrollerar att det finns en fil som har blivit uppladdad
            if (imageFile != null && imageFile.Length > 0)
            {

                // Först kontrollerar att selectedSongById.imagefilepath (Directory/GUID.png) är inte tom. Sen kontrollerar (Directory/GUID.png) finns redan i vår server.
                if (!string.IsNullOrEmpty(selectedSongById.ImageFilePath) && System.IO.File.Exists(selectedSongById.ImageFilePath))
                {
                    //Om true, raderas bilden enlign nedan.

                    System.IO.File.Delete(selectedSongById.ImageFilePath);
                }


                //Genererar unik filpath för den nya uppladdad bilden. Lik den för laravel för att sedan spara den i wwwrootes/images. Unika koden genereras mha GUID, och ersätter original bildens namn.
                //Path.Combine tar 3 parametrar (1. Directory /var/www/myapp , 2.wwwroot/images , 3. GUID + (jpg,png,svg...)
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName));

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                    selectedSongById.ImageFilePath = imagePath; // Set the file path on the song object
                }
            }

            else
            {
                // If no new image file is uploaded, keep the path of the existing image.
                song.ImageFilePath = selectedSongById.ImageFilePath;
            }

            //Då vi baserar hela nya datan på den selectedSongById.song. så fyller vi den med Song från input
            selectedSongById.Name = song.Name;
            selectedSongById.LengthInSeconds = song.LengthInSeconds;
            selectedSongById.CategoryId = song.CategoryId;


            _context.Entry(selectedSongById).State = EntityState.Modified;

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

            //Return ok retunerar kod 200. mer avnändbar. 
            return Ok(new
            {
                Message = $"The song with the name of {selectedSongById.Name} was updated successfullys.",
                Song = selectedSongById
            });


        }



        // POST: api/Songs

        /*OBS Song song och ImageFilePath är vad POST request förväntas tas emot. Dvs två olika inputs. Därför måste song.ImageFilePath = imagePath.
            Ytterligare, så måste [FromForm] användas för att kombinera olika typer av data*/

        [HttpPost]
        public async Task<ActionResult<Song>> PostSong([FromForm] Song song, [FromForm] IFormFile imageFile = null)
        {

            //Kontrollerar att kategorin som skickas med song finns i kateogrin tabellen (id), retunerar true/false

            //var categoryExists = await _context.Categories.AnyAsync(c => c.Id == song.CategoryId);

            //Kontrollerar först om song validerar korrekt, senare om kategorin inte finns

            if (song == null)
            {
                return BadRequest("Invalid categoryId");
            }


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

            else
            {
                song.ImageFilePath = null;
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

            // Först kontrollerar att selectedSongById.imagefilepath (Directory/GUID.png) är inte tom. Sen kontrollerar (Directory/GUID.png) finns redan i vår server.
            if (!string.IsNullOrEmpty(song.ImageFilePath) && System.IO.File.Exists(song.ImageFilePath))
            {
                //Om true, raderas bilden enlign nedan.

                System.IO.File.Delete(song.ImageFilePath);
            }


            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return Ok($"song with the name of {song.Name} has been deleted!");
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }
    }
}
