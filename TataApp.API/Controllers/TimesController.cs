using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TataApp.API.Models;
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
            var timesResponse = new List<TimeResponse>();
            foreach (var time in times)
            {
                timesResponse.Add(new TimeResponse
                {
                    Activity = time.Activity,
                    ActivityId = time.ActivityId,
                    DateRegistered = time.DateRegistered,
                    DateReported = time.DateReported,
                    EmployeeId = time.EmployeeId,
                    From = time.From,
                    Project = time.Project,
                    ProjectId = time.ProjectId,
                    Remarks = time.Remarks,
                    TimeId = time.TimeId,
                    To = time.To,
                });
            }

            return Ok(timesResponse);
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
        public async Task<IHttpActionResult> PostTime(NewTimeRequest request)
        {
            Time time = null;

            if (!request.IsRepeated)
            {
                time = ToTime(request);
                db.Times.Add(time);
            }
            else
            {
                var date = request.DateReported;
                while (date <= request.Until)
                {
                    var sw = false;

                    if (date.DayOfWeek == DayOfWeek.Monday && request.IsRepeatMonday)
                    {
                        sw = true;
                    }

                    if (date.DayOfWeek == DayOfWeek.Tuesday && request.IsRepeatTuesday)
                    {
                        sw = true;
                    }

                    if (date.DayOfWeek == DayOfWeek.Wednesday && request.IsRepeatWednesday)
                    {
                        sw = true;
                    }

                    if (date.DayOfWeek == DayOfWeek.Thursday && request.IsRepeatThursday)
                    {
                        sw = true;
                    }

                    if (date.DayOfWeek == DayOfWeek.Friday && request.IsRepeatFriday)
                    {
                        sw = true;
                    }

                    if (date.DayOfWeek == DayOfWeek.Saturday && request.IsRepeatSaturday)
                    {
                        sw = true;
                    }

                    if (date.DayOfWeek == DayOfWeek.Sunday && request.IsRepeatSunday)
                    {
                        sw = true;
                    }

                    if (sw)
                    {
                        time = new Time
                        {
                            ActivityId = request.ActivityId,
                            DateRegistered = DateTime.Now,
                            DateReported = date,
                            EmployeeId = request.EmployeeId,
                            From = request.From,
                            Latitude = request.Latitude,
                            Longitude = request.Longitude,
                            ProjectId = request.ProjectId,
                            Remarks = request.Remarks,
                            To = request.To,
                        };

                        db.Times.Add(time);
                    }

                    date = date.AddDays(1);
                }
            }

            await db.SaveChangesAsync();
            return Ok(time);
        }

        private Time ToTime(NewTimeRequest request)
        {
            return new Time
            {
                ActivityId = request.ActivityId,
                DateRegistered = DateTime.Now,
                DateReported = request.DateReported,
                EmployeeId = request.EmployeeId,
                From = request.From,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                ProjectId = request.ProjectId,
                Remarks = request.Remarks,
                To = request.To,
            };
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