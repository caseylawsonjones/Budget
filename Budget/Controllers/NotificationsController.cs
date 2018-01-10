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
using System.Net.Mail;
using System.Configuration;
using System.Threading.Tasks;

namespace Budget.Controllers
{
    [Authorize]
    public class NotificationsController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Notifications
        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var notifications = db.Notifications.Where(n => n.RecipientId == user.Id).ToList();
            return View(notifications);
        }

        // GET: Notifications/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            return View(notification);
        }

        // GET: Notifications/Create
        [Authorize]
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.BudgetId = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.TransactionId = new SelectList(db.Transactions.Where(x => x.HouseholdId == user.HouseholdId && x.IsVoided == false), "Id", "Title");
            ViewBag.CategoryId = new SelectList(db.NotificationCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == 0), "Id", "Category");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,OriginalDeliveryDate,CurrentDeliveryDate,DateAcknowledged,IsAcknowledged,HouseholdId,CategoryId,OwnerId,AccountId,BudgetId,TransactionId,IncomeId,ExpenseId")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Notifications.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.BudgetId = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.TransactionId = new SelectList(db.Transactions.Where(x => x.HouseholdId == user.HouseholdId && x.IsVoided == false), "Id", "Title");
            ViewBag.CategoryId = new SelectList(db.NotificationCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == 0), "Id", "Category");
            return View(notification);
        }

        [Authorize]
        public async Task<int> CreateInvitationNotification(int id) {

            Notification notification = new Notification();
            Invitation invitation = db.Invitations.Find(id);
            ApplicationUser inviter = db.Users.Find(invitation.InviterId);

            // FIRST TO SEND THE EMAIL/EXTERNAL PORTION OF THE INVITATION
            // Prepare the email
            string mainBody = "You have received an invitation to join " + inviter.FullName + "'s Household on the Financial Fusion website <br>The <a href='https://cjones-budgeter.azurewebsites.net'>Financial Fusion</a> site is a household budgeting web application.<hr>Here is the invitation message that " + inviter.FirstName + " sent you:<br>" + invitation.InvitationMessage + "<br><hr>Click <a href='https://cjones-budgeter.azurewebsites.net'>here</a> to to go the site where you can register your free account and join " + inviter.FullName + "'s Household!";
            string subject = "Request to Join a Household on the Financial Fusion site!";
            // If the user already has an account on the site, the email will be slightly modified
            if (db.Users.Any(u => u.Email == invitation.InviteeEmail)){
                mainBody = "You have received an invitation to join " + inviter.FullName + "'s Household on the Financial Fusion website <br>The <a href='https://cjones-budgeter.azurewebsites.net'>Financial Fusion</a> site is a household budgeting web application.<hr>Here is the invitation message that " + inviter.FirstName + " sent you:<br>" + invitation.InvitationMessage + "<br><hr>You already have an account, so just click <a href='https://cjones-budgeter.azurewebsites.net'>here</a> to log into the site!" + inviter.FullName + "'s Household!";
            }
            // Send the email
            await SendNotification(invitation.InviteeEmail, mainBody, subject);

            // NOW TO SEND THE INTERNAL PORTION OF THE INVITATION
            // Check to see if the invited person is already registered and active on the site
            if (db.Users.Any(u => u.Email == invitation.InviteeEmail && u.EmailConfirmed == true)) {
                notification.RecipientId = db.Users.First(u => u.Email == invitation.InviteeEmail).Id;
            }
            notification.Title = ("Invitation to Join " + inviter.FullName + "'s Household");
            notification.Description = "This is an invitation to join a new household unit for this application.";
            notification.Body = "To accept or decline this invitation, click <a href='/Invitations/Details/" + invitation.Id + "'>here</a>";
            notification.Created = DateTime.UtcNow;
            notification.OriginalDeliveryDate = DateTime.UtcNow;
            notification.CurrentDeliveryDate = DateTime.UtcNow;
            notification.IsAcknowledged = false;
            notification.HouseholdId = inviter.HouseholdId.Value;
            notification.CategoryId = db.NotificationCategories.First(n => n.Category == "Invitation").Id;
            notification.OwnerId = inviter.Id;
            db.Notifications.Add(notification);
            db.SaveChanges();

            // Assign the NotificationId to the Invitation so the Notification can be marked as IsAcknowledged when the Invitation is responded to.
            invitation.NotificationId = notification.Id;
            db.Entry(invitation).State = EntityState.Modified;
            db.SaveChanges();

            return 1;
        }

        private async Task SendNotification(string toEmailAddress, string emailBody, string emailSubject) {
            try {
                var fromEmailAddress = "Financial Fusion<webappmessages@gmail.com>";
                //var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p>";
                //model.Body = "This is a message from your BugTracker web application.  The name and the email of the contacting person is above.";
                var email = new MailMessage(fromEmailAddress, toEmailAddress) {
                    Subject = emailSubject,
                    Body = emailBody,
                    IsBodyHtml = true
                };
                var svc = new PersonalEmail();
                await svc.SendAsync(email);
                return;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
        }


        public void CreateInvitationResponseNotification(int id, bool hasAccepted) {
            Notification notification = new Notification();
            Invitation invitation = db.Invitations.Find(id);
            ApplicationUser invitee = db.Users.FirstOrDefault(u => u.Email == invitation.InviteeEmail);
            ApplicationUser inviter = db.Users.Find(invitation.Inviter.Id);
            var household = db.Households.Find(invitation.HouseholdId);
            string response = "";
            if (hasAccepted) {
                response = "accepted";
            }
            else {
                response = "declined";
            }
            foreach(var member in household.Members) {
                var recipient = db.Users.Find(member.Id);
                notification.Title = (invitee.FullName + " has " + response + " their invitation to the household.");
                notification.Created = DateTime.UtcNow;
                notification.OriginalDeliveryDate = DateTime.UtcNow;
                notification.CurrentDeliveryDate = DateTime.UtcNow;
                notification.IsAcknowledged = false;
                notification.HouseholdId = inviter.HouseholdId.Value;
                notification.CategoryId = db.NotificationCategories.First(n => n.Category == "Invitation Response").Id;
                notification.OwnerId = invitee.Id;
                notification.RecipientId = recipient.Id;
                db.Notifications.Add(notification);
                db.SaveChanges();
            }
        }

        public void CreateLeaveHouseholdNotification(int userId) {
            Notification notification = new Notification();
            ApplicationUser user = db.Users.Find(userId);
            var household = db.Households.Find(user.HouseholdId);
            foreach (var member in household.Members) {
                var recipient = db.Users.Find(member.Id);
                notification.Title = (user.FullName + " has left the household.");
                notification.Created = DateTime.UtcNow;
                notification.OriginalDeliveryDate = DateTime.UtcNow;
                notification.CurrentDeliveryDate = DateTime.UtcNow;
                notification.IsAcknowledged = false;
                notification.HouseholdId = household.Id;
                notification.CategoryId = db.NotificationCategories.First(n => n.Category == "Member Left Household").Id;
                notification.OwnerId = user.Id;
                notification.RecipientId = recipient.Id;
                db.Notifications.Add(notification);
                db.SaveChanges();
            }
        }

        // GET: Notifications/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || notification.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.BudgetId = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.TransactionId = new SelectList(db.Transactions.Where(x => x.HouseholdId == user.HouseholdId && x.IsVoided == false), "Id", "Title");
            ViewBag.CategoryId = new SelectList(db.NotificationCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == 0), "Id", "Category");
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,OriginalDeliveryDate,CurrentDeliveryDate,DateAcknowledged,IsAcknowledged,HouseholdId,CategoryId,OwnerId,AccountId,BudgetId,TransactionId,IncomeId,ExpenseId")] Notification notification)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || notification.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IncomeId = new SelectList(db.Incomes.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users.Where(x => x.HouseholdId == user.HouseholdId), "Id", "FirstName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.ExpenseId = new SelectList(db.Expenses.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.BudgetId = new SelectList(db.BudgetPlans.Where(x => x.HouseholdId == user.HouseholdId && x.IsActive == true), "Id", "Name");
            ViewBag.TransactionId = new SelectList(db.Transactions.Where(x => x.HouseholdId == user.HouseholdId && x.IsVoided == false), "Id", "Title");
            ViewBag.CategoryId = new SelectList(db.NotificationCategories.Where(x => x.HouseholdId == user.HouseholdId || x.HouseholdId == 0), "Id", "Category");
            return View(notification);
        }

        // GET: Notifications/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || notification.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notifications.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseholdId == null || notification.HouseholdId != user.HouseholdId) {
                return RedirectToAction("NotAuthorized", "Account");
            }
            db.Notifications.Remove(notification);
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
