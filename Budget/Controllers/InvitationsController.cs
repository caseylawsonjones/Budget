using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budget.Models;
using Microsoft.AspNet.Identity;
using Budget.Models.ViewModels;
using Budget.Models.Helpers;
using System.Threading.Tasks;

namespace Budget.Controllers
{
    [Authorize]
    public class InvitationsController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invitations
        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            List<Invitation> invitations = new List<Invitation>();
            InvitationsIndexViewModel InvIndVM = new InvitationsIndexViewModel();
            if (user.HouseholdId == null) {
                invitations = db.Invitations.Where(i => (i.InviteeEmail == user.Email && i.HasAccepted == false && i.HasDeclined == false)).ToList();
                InvIndVM.UserIsInHousehold = false;
            }
            else {
                invitations = db.Invitations.Where(i => i.HouseholdId == user.HouseholdId && i.HasAccepted == false && i.HasDeclined == false).ToList();
                InvIndVM.UserIsInHousehold = true;
            }
            InvIndVM.Invitations = invitations;
            return View(InvIndVM);
        }

        // GET: Invitations/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.Email != invitation.InviteeEmail && invitation.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            return View(invitation);
        }

        // GET: Invitations/Create
        [Authorize]
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,InvitationMessage,InviteeEmail")] Invitation invitation)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                invitation.HasAccepted = false;
                invitation.HasDeclined = false;
                invitation.InviterId = user.Id;
                invitation.HouseholdId = user.HouseholdId.Value;
                invitation.Created = DateTime.UtcNow;
                db.Invitations.Add(invitation);
                db.SaveChanges();
                NotificationsController notification = new NotificationsController();
                Task<int> sendNotification = notification.CreateInvitationNotification(invitation.Id);
                int result = await sendNotification;
                result++;
                return RedirectToAction("Index");
            }

            ViewBag.InviteeId = new SelectList(db.Users.Where(u => u.Id != user.Id), "Id", "FirstName");
            return View(invitation);
        }

        // GET: Invitations/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || invitation.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            ViewBag.InviteeId = new SelectList(db.Users.Where(u => u.Id != user.Id), "Id", "FirstName");
            ViewBag.InviterId = new SelectList(db.Users, "Id", "FirstName", invitation.InviterId);
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,HasAccepted,HasDeclined,InvitationMessage,ResponseMessage,InviterId,InviteeEmail,HouseholdId")] Invitation invitation)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || invitation.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            if (ModelState.IsValid)
            {
                db.Entry(invitation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InviteeId = new SelectList(db.Users.Where(u => u.Id != user.Id), "Id", "FirstName");
            ViewBag.InviterId = new SelectList(db.Users, "Id", "FirstName", invitation.InviterId);
            return View(invitation);
        }

        [Authorize]
        public async Task<ActionResult> InvitationAccept(int invitationId) {
            Invitation invitation = db.Invitations.AsNoTracking().First(i => i.Id == invitationId);
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            // Verify that the Invitation has not already been Accepted or Declined - This is a second layer of security as it was already handled in the View
            if (invitation.HasAccepted == true || invitation.HasDeclined == true) {
                return RedirectToAction("NotAuthorized", "Account");
            }

            // Doublechecking that the logged in user is also the Invitee
            if (user.Email != invitation.InviteeEmail) {
                return RedirectToAction("NotAuthorized", "Account");
            }

            // If Invitee is in another HH, they have to leave that HH before joining this one.
            if (user.HouseholdId != null) {
                LeaveHouseholdHelper leaveHelper = new LeaveHouseholdHelper();
                leaveHelper.LeaveHousehold(user);
            }

            // Add User to the Household
            user.HouseholdId = invitation.HouseholdId;
            user.DateJoined = DateTime.UtcNow;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            // Mark Invitation as Accepted so it does not show up in the list of Unresponded Invitations
            invitation.HasAccepted = true;
            db.Entry(invitation).State = EntityState.Modified;
            db.SaveChanges();

            // Mark Notification as Acknowledged so it does not show up on the list of Unread/Unacknowledged Notifications
            Notification notification = db.Notifications.AsNoTracking().First(n => n.Id == invitation.NotificationId);
            notification.IsAcknowledged = true;
            db.Entry(notification).State = EntityState.Modified;
            db.SaveChanges();

            // Send a notification to the Members of the Household indicating that there is a new Member
            NotificationsController notificationController = new NotificationsController();
            notificationController.CreateInvitationResponseNotification(invitation.Id, true);

            await HttpContext.RefreshAuthentication(db.Users.Find(User.Identity.GetUserId()));
            return RedirectToAction("Index", "Households");
        }

        [Authorize]
        public ActionResult InvitationDecline(int invitationId) {
            Invitation invitation = db.Invitations.AsNoTracking().First(i => i.Id == invitationId);

            // Verify that the Invitation has not already been Accepted or Declined - This is a second layer of security as it was already handled in the View
            if (invitation.HasAccepted == true || invitation.HasDeclined == true) {
                return RedirectToAction("NotAuthorized", "Account");
            }

            // Mark Invitation as Declined so it does not show up in the list of Unresponded Invitations
            invitation.HasDeclined = true;
            db.Entry(invitation).State = EntityState.Modified;
            db.SaveChanges();

            // Mark Invitation Notification as Acknowledged so it does not show up on the list of Unread/Unacknowledged Notifications
            Notification notification = db.Notifications.AsNoTracking().First(n => n.Id == invitation.NotificationId);
            notification.IsAcknowledged = true;
            db.Entry(notification).State = EntityState.Modified;
            db.SaveChanges();

            // Create a Notification to the Household Members indicating that the Invitation was Declined
            var household = db.Households.Find(invitation.HouseholdId);
            NotificationsController notificationController = new NotificationsController();
            notificationController.CreateInvitationResponseNotification(invitation.Id, false);

            return RedirectToAction("Index", "Home");
        }

        // GET: Invitations/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || invitation.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || invitation.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            db.Invitations.Remove(invitation);
            db.SaveChanges();
            return RedirectToAction("Index");
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
