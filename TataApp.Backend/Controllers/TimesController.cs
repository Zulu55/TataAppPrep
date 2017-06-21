using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using TataApp.Backend.Models;
using TataApp.Domain;
using TataApp.Backend.Helpers;

namespace TataApp.Backend.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TimesController : Controller
    {
        private DataContextLocal db = new DataContextLocal();

        // GET: Times
        public async Task<ActionResult> Index()
        {
            var times = db.Times.Include(t => t.Activity).Include(t => t.Employee).Include(t => t.Project);
            return View(await times.ToListAsync());
        }

        // GET: Times/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var time = await db.Times.FindAsync(id);

            if (time == null)
            {
                return HttpNotFound();
            }

            return View(time);
        }

        // GET: Times/Create
        public ActionResult Create()
        {
            ViewBag.ActivityId = new SelectList(db.Activities, "ActivityId", "Description");
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "FullName");
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "Description");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Time time)
        {
            if (ModelState.IsValid)
            {
                db.Times.Add(time);
                var response = await DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            ViewBag.ActivityId = new SelectList(db.Activities, "ActivityId", "Description", time.ActivityId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "FullName", time.EmployeeId);
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "Description", time.ProjectId);
            return View(time);
        }

        // GET: Times/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var time = await db.Times.FindAsync(id);

            if (time == null)
            {
                return HttpNotFound();
            }

            ViewBag.ActivityId = new SelectList(db.Activities, "ActivityId", "Description", time.ActivityId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "FullName", time.EmployeeId);
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "Description", time.ProjectId);
            return View(time);
        }

        // POST: Times/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Time time)
        {
            if (ModelState.IsValid)
            {
                db.Entry(time).State = EntityState.Modified;
                var response = await DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            ViewBag.ActivityId = new SelectList(db.Activities, "ActivityId", "Description", time.ActivityId);
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "FullName", time.EmployeeId);
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectId", "Description", time.ProjectId);
            return View(time);
        }

        // GET: Times/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var time = await db.Times.FindAsync(id);

            if (time == null)
            {
                return HttpNotFound();
            }

            return View(time);
        }

        // POST: Times/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var time = await db.Times.FindAsync(id);
            db.Times.Remove(time);
            var response = await DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(time);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}