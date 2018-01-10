using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budget.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Budget.Models.Helpers;
using Budget.Models.ViewModels;

namespace Budget.Controllers
{
    [Authorize]
    public class HouseholdsController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Households - Index is replacing Details, since each user will only have one Household
        [Authorize]
        public ActionResult Index()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId != null) {
                Household household = db.Households.First(h => h.Id == user.HouseholdId);
                return View(household);
            }

            return RedirectToAction("Index", "Manage");
        }

        // GET: Households/Details/5
        //[Authorize]
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Household household = db.Households.Find(id);
        //    if (household == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(household);
        //}

        // GET: Households/Create

        [Authorize]
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            HouseholdCreationViewModel hhCVM = new HouseholdCreationViewModel();
            // Check to see if user is already in a Household and are they the last Member
            if (user.HouseholdId != null) {
                hhCVM.UserAlreadyInHousehold = true;
                Household existingHousehold = db.Households.Find(user.HouseholdId);
                if (existingHousehold.Members.Count() < 2)
                    hhCVM.UserIsLastHouseholdMember = true;
            }
            return View(hhCVM);
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description")] Household household)
        {
            if (ModelState.IsValid) {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

                // First verify that no other HH have the same name
                // We will also check this at the input field via an Ajax button
                if(db.Households.Any(h => h.Name == household.Name)) {
                    ViewBag.ErrorMessage = "A Household already exists with that name.";
                    return View(household);
                }

                // If User is in another HH, they have to leave that HH before Creating this one.
                if (user.HouseholdId != null) {
                    LeaveHouseholdHelper leaveHelper = new LeaveHouseholdHelper();
                    leaveHelper.LeaveHousehold(user);
                }

                // Now, for our regularly scheduled broadcast:
                household.OwnerId = user.Id;
                household.Created = DateTime.UtcNow;
                household.IsActive = true;
                //household.LogoUrl = "~/Content/Images/Household Logos/HouseholdDefaultLogo.png";
                db.Households.Add(household);
                db.SaveChanges();

                //Now create a folder for any Logos to be added
                //var attPath = "~/Content/Images/Household Logos/" + household.Id;
                //var newPath = Server.MapPath(attPath);
                //Directory.CreateDirectory(newPath);

                // Creating user must belong to the household
                user.HouseholdId = household.Id;
                user.DateJoined = DateTime.UtcNow;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                // If a Household Logo was uploaded, we have to add that
                //if (logo != null && logo.ContentLength > 0) {
                //    var ext = Path.GetExtension(logo.FileName).ToLower(); // Gets image's extension and then sets it to lower case
                //    var filePath = "Content/Images/Household Logos/" + household.Id;
                //    var absPath = Server.MapPath("~/" + filePath);
                //    string newFileName = logo.FileName;
                //    var num = 0;
                //    while (System.IO.File.Exists(Path.Combine(absPath, newFileName))) {
                //        //Sets "filename" back to the default value
                //        newFileName = Path.GetFileNameWithoutExtension(logo.FileName);
                //        //Add's parentheses after the name with a number ex. filename(4)
                //        newFileName = string.Format(newFileName + "(" + ++num + ")" + ext);
                //        //Makes sure pPic gets updated with the new filename so it could check
                //        //attach = fileName + Path.GetExtension(doc.FileName);
                //    }
                //    logo.SaveAs(Path.Combine(absPath, newFileName));

                //    // Update attachment info and add to database
                //    household.LogoUrl = "/" + filePath + "/" + newFileName;
                //    db.Entry(household).State = EntityState.Modified;
                //    db.SaveChanges();
                //}

                await HttpContext.RefreshAuthentication(db.Users.Find(User.Identity.GetUserId()));
                return RedirectToAction("Index", "Home");
            }
            else {
                return View(household);
            }
        }

        // GET: Households/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            // First verify that the User is a Member of the Household
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || id != user.HouseholdId) {
                ViewBag.ErrorMessage = "Either you belong to no Household, or the resource you are trying to view belongs to another Household.  Either condition prevents you from viewing the resource.";
                return RedirectToAction("NotAuthorized", "Account");
            }
            if (id == null)
            {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return RedirectToAction("NotAuthorized", "Account");
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                ViewBag.ErrorMessage = "The resource that you are trying to view is either corrupt or does not exist.  Either condition prevents you from viewing this resource.";
                return RedirectToAction("NotAuthorized", "Account");
            }
            //ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", household.OwnerId);
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Created,IsActive,OwnerId")] Household household)
        {
            // First verify that the User is a Member of the Household
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || household.Id != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }

            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", household.OwnerId);
            return View(household);
        }

        [Authorize]
        public async Task<ActionResult> LeaveHousehold() {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            LeaveHouseholdHelper leaveHelper = new LeaveHouseholdHelper();
            leaveHelper.LeaveHousehold(user);

            await HttpContext.RefreshAuthentication(db.Users.Find(User.Identity.GetUserId()));
            return RedirectToAction("Index", "Home");
        }

        //// GET: Households/Delete/5
        //[Authorize]
        //public ActionResult Delete(int? id)
        //{
        //    // First verify that the User is a Member of the Household
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    if (user.HouseholdId == null || id != user.HouseholdId) {
        //        return RedirectToAction("NotAuthorized", "Account");
        //    }

        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Household household = db.Households.Find(id);
        //    if (household == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(household);
        //}

        //// POST: Households/Delete/5
        //[Authorize]
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    // First verify that the User is a Member of the Household
        //    var user = db.Users.Find(User.Identity.GetUserId());
        //    if (user.HouseholdId == null || id != user.HouseholdId) {
        //        return RedirectToAction("NotAuthorized", "Account");
        //    }

        //    Household household = db.Households.Find(id);
        //    db.Households.Remove(household);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


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
