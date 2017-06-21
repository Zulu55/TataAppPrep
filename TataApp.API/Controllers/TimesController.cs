using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TataApp.Domain;

namespace TataApp.API.Controllers
{
    [Authorize]
    public class TimesController : ApiController
    {
        private DataContext db = new DataContext();

        // GET: api/Times
        public IQueryable<Time> GetTimes()
        {
            return db.Times;
        }

        // GET: api/Times/5
        [ResponseType(typeof(Time))]
        public async Task<IHttpActionResult> GetTime(int id)
        {
            var times = await db.Times.Where(t => t.EmployeeId == id).ToListAsync();
            if (times == null)
            {
                return NotFound();
            }

            return Ok(times);
        }

        // PUT: api/Times/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTime(int id, Time time)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != time.TimeId)
            {
                return BadRequest();
            }

            db.Entry(time).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Times
        [ResponseType(typeof(Time))]
        public async Task<IHttpActionResult> PostTime(Time time)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Times.Add(time);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = time.TimeId }, time);
        }

        // DELETE: api/Times/5
        [ResponseType(typeof(Time))]
        public async Task<IHttpActionResult> DeleteTime(int id)
        {
            Time time = await db.Times.FindAsync(id);
            if (time == null)
            {
                return NotFound();
            }

            db.Times.Remove(time);
            await db.SaveChangesAsync();

            return Ok(time);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TimeExists(int id)
        {
            return db.Times.Count(e => e.TimeId == id) > 0;
        }
    }
}