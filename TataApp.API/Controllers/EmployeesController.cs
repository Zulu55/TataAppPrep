namespace TataApp.API.Controllers
{
    using Domain;
    using Helpers;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    [RoutePrefix("api/Employees")]
    public class EmployeesController : ApiController
    {
        private DataContext db = new DataContext();

        [HttpPost]
        [Route("LoginFacebook")]
        public async Task<IHttpActionResult> LoginFacebook(FacebookResponse profile)
        {
            try
            {
                var employee = await db.Employees.Where(e => e.Email == profile.Id).FirstOrDefaultAsync();
                if (employee == null)
                {
                    employee = new Employee
                    {
                        Email = profile.Id,
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        Picture = profile.Picture.Data.Url,
                        DocumentTypeId = 1,
                        LoginTypeId = 2,
                    };

                    db.Employees.Add(employee);
                    CreateUserASP(profile.Id, "User", profile.Id);
                }
                else
                {
                    employee.FirstName = profile.FirstName;
                    employee.LastName = profile.LastName;
                    employee.Picture = profile.Picture.Data.Url;
                    db.Entry(employee).State = EntityState.Modified;
                }

                await db.SaveChangesAsync();
                return Ok(true);
            }
            catch (DbEntityValidationException e)
            {
                var message = string.Empty;
                foreach (var eve in e.EntityValidationErrors)
                {
                    message = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        message += string.Format("\n- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                return BadRequest(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PasswordRecovery")]
        public async Task<IHttpActionResult> PasswordRecovery(JObject form)
        {
            try
            {
                var email = string.Empty;
                dynamic jsonObject = form;

                try
                {
                    email = jsonObject.Email.Value;
                }
                catch
                {
                    return BadRequest("Incorrect call.");
                }

                var employee = await db.Employees
                    .Where(e => e.Email.ToLower() == email.ToLower())
                    .FirstOrDefaultAsync();
                if (employee == null)
                {
                    return NotFound();
                }

                var userContext = new ApplicationDbContext();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
                var userASP = userManager.FindByEmail(email);
                if (userASP == null)
                {
                    return NotFound();
                }

                var random = new Random();
                var newPassword = string.Format("{0}", random.Next(100000, 999999));
                var response1 = userManager.RemovePassword(userASP.Id);
                var response2 = await userManager.AddPasswordAsync(userASP.Id, newPassword);
                if (response2.Succeeded)
                {
                    var subject = "TATA App - Password Recovery";
                    var body = string.Format(@"
                        <h1>TATA App - Password Recovery</h1>
                        <p>Your new password is: <strong>{0}</strong></p>
                        <p>Please, don't forget change it for one easy remember for you.",
                        newPassword);

                    await MailHelper.SendMail(email, subject, body);
                    return Ok(true);
                }

                return BadRequest("The password can't be changed.");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(JObject form)
        {
            var email = string.Empty;
            var currentPassword = string.Empty;
            var newPassword = string.Empty;
            dynamic jsonObject = form;

            try
            {
                email = jsonObject.Email.Value;
                currentPassword = jsonObject.CurrentPassword.Value;
                newPassword = jsonObject.NewPassword.Value;
            }
            catch
            {
                return BadRequest("Incorrect call");
            }

            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(email);

            if (userASP == null)
            {
                return BadRequest("Incorrect call");
            }

            var response = await userManager.ChangePasswordAsync(userASP.Id, currentPassword, newPassword);
            if (!response.Succeeded)
            {
                return BadRequest(response.Errors.FirstOrDefault());
            }

            return Ok("ok");
        }

        [Authorize]
        [ResponseType(typeof(Employee))]
        public async Task<IHttpActionResult> GetEmployee(int id)
        {
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // GET: api/Employees
        [Authorize]
        public IQueryable<Employee> GetEmployees()
        {
            return db.Employees;
        }

        // POST: api/GetGetEmployeeByEmailOrCode
        [Authorize]
        [HttpPost]
        [Route("GetGetEmployeeByEmailOrCode")]
        public async Task<IHttpActionResult> GetGetEmployeeByEmailOrCode(JObject form)
        {
            var emailOrCode = string.Empty;
            dynamic jsonObject = form;

            try
            {
                emailOrCode = jsonObject.EmailOrCode.Value;
            }
            catch
            {
                return BadRequest("Incorrect call");
            }

            Employee employee = null;
            if (emailOrCode.IndexOf('@') != -1)
            {
                employee = await db.Employees.Where(e => e.Email.ToLower() == emailOrCode.ToLower()).FirstOrDefaultAsync();
            }
            else
            {
                int code;
                int.TryParse(emailOrCode, out code);
                employee = await db.Employees.Where(e => e.EmployeeCode == code).FirstOrDefaultAsync();
            }

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // PUT: api/Employees/5
        [ResponseType(typeof(void))]
        [Authorize]
        public async Task<IHttpActionResult> PutEmployee(int id, EmployeeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.EmployeeId)
            {
                return BadRequest();
            }

            var olEmployee = await db.Employees.FindAsync(id);
            if (olEmployee == null)
            {
                return NotFound();
            }

            var oldEmail = olEmployee.Email;
            var isEmailChanged = olEmployee.Email.ToLower() != request.Email.ToLower();

            if (request.ImageArray != null && request.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(request.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var folder = "~/Content/Employees";
                var fullPath = string.Format("{0}/{1}", folder, file);
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    request.Picture = fullPath;
                }
            }
            else
            {
                request.Picture = olEmployee.Picture;
            }

            olEmployee.FirstName = request.FirstName;
            olEmployee.LastName = request.LastName;
            olEmployee.EmployeeCode = request.EmployeeCode;
            olEmployee.DocumentTypeId = request.DocumentTypeId;
            olEmployee.LoginTypeId = request.LoginTypeId;
            olEmployee.Document = request.Document;
            olEmployee.Picture = request.Picture;
            olEmployee.Email = request.Email;
            olEmployee.Phone = request.Phone;
            olEmployee.Address = request.Address;
            db.Entry(olEmployee).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
                if (isEmailChanged)
                {
                    UpdateUserName(oldEmail, request.Email);
                }

                return Ok(olEmployee);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool UpdateUserName(string currentUserName, string newUserName)
        {
            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(currentUserName);
            if (userASP == null)
            {
                return false;
            }

            userASP.UserName = newUserName;
            userASP.Email = newUserName;
            var response = userManager.Update(userASP);
            return response.Succeeded;
        }

        // POST: api/Employees
        [ResponseType(typeof(Employee))]
        public async Task<IHttpActionResult> PostEmployee(EmployeeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.ImageArray != null && request.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(request.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var folder = "~/Content/Employees";
                var fullPath = string.Format("{0}/{1}", folder, file);
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    request.Picture = fullPath;
                }
            }

            var employee = ToEmployee(request);
            db.Employees.Add(employee);
            await db.SaveChangesAsync();
            CreateUserASP(request.Email, "Employee", request.Password);

            return CreatedAtRoute("DefaultApi", new { id = employee.EmployeeId }, employee);
        }

        private void CreateUserASP(string email, string roleName, string password)
        {
            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var userASP = new ApplicationUser
            {
                Email = email,
                UserName = email,
            };

            var result = userManager.Create(userASP, password);
            if (result.Succeeded)
            {
                userManager.AddToRole(userASP.Id, roleName);
            }
        }

        private Employee ToEmployee(EmployeeRequest request)
        {
            return new Employee
            {
                Address = request.Address,
                Document = request.Document,
                DocumentType = request.DocumentType,
                DocumentTypeId = request.DocumentTypeId,
                Email = request.Email,
                EmployeeCode = request.EmployeeCode,
                EmployeeId = request.EmployeeId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                LoginType = request.LoginType,
                LoginTypeId = request.LoginTypeId,
                Phone = request.Phone,
                Picture = request.Picture,
                Times = request.Times,
            };
        }

        // DELETE: api/Employees/5
        [Authorize]
        [ResponseType(typeof(Employee))]
        public async Task<IHttpActionResult> DeleteEmployee(int id)
        {
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            db.Employees.Remove(employee);
            await db.SaveChangesAsync();

            return Ok(employee);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmployeeExists(int id)
        {
            return db.Employees.Count(e => e.EmployeeId == id) > 0;
        }
    }
}