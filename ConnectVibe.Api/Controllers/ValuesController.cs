using ConnectVibe.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectVibe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ConnectVibeApiContext _context;

        public ValuesController(ConnectVibeApiContext context)
        {
            _context = context;
        }

        // GET: api/Values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Values>>> GetValues()
        {
            return await _context.Values.ToListAsync();
        }

        // GET: api/Values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Values>> GetValues(int id)
        {
            var values = await _context.Values.FindAsync(id);

            if (values == null)
            {
                return NotFound();
            }

            return values;
        }

        // PUT: api/Values/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutValues(int id, Values values)
        {
            if (id != values.Id)
            {
                return BadRequest();
            }

            _context.Entry(values).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ValuesExists(id))
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

        // POST: api/Values
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Values>> PostValues(Values values)
        {
            _context.Values.Add(values);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetValues", new { id = values.Id }, values);
        }

        // DELETE: api/Values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValues(int id)
        {
            var values = await _context.Values.FindAsync(id);
            if (values == null)
            {
                return NotFound();
            }

            _context.Values.Remove(values);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ValuesExists(int id)
        {
            return _context.Values.Any(e => e.Id == id);
        }
    }
}
