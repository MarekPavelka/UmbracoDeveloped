using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using UmbracoDeveloped.Core.ViewModels;
using Umbraco.Core.Logging;
using System.Net.Mail;

namespace UmbracoDeveloped.Core.Controllers
{
    public class ContactController : SurfaceController
    {
        public ActionResult RenderContactForm()
        {
            var vm = new ContactFormViewModel();
            return PartialView("~/Views/Partials/ContactForm.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleContactForm(ContactFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "Please check the form");
                //return PartialView("~/Views/Partials/ContactForm.cshtml", vm);
                return CurrentUmbracoPage();
            }

            try
            {
                var contactForms = Umbraco.ContentAtRoot().DescendantsOrSelfOfType("contactForms").FirstOrDefault();
                if (contactForms != null)
                {
                    //create content of type contactForm, in the parent by id
                    var newContact = Services.ContentService.Create("Contact", contactForms.Id, "contactForm");
                    newContact.SetValue("contactName", vm.Name);
                    newContact.SetValue("contactEmail", vm.EmailAddress);
                    newContact.SetValue("contactSubject", vm.Subject);
                    newContact.SetValue("contactComments", vm.Comment);
                    Services.ContentService.SaveAndPublish(newContact);
                }

                SendContactFormReceivedEmail(vm);
                TempData["status"] = "OK";

                return RedirectToCurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                Logger.Error<ContactController>("Error in contact form submission", ex.Message);
                ModelState.AddModelError("Error", "Sorry there was a problem noting your details. Would you please try again later?");
            }

            return CurrentUmbracoPage();
        }

        private void SendContactFormReceivedEmail(ContactFormViewModel vm)
        {
            var siteSettings = Umbraco.ContentAtRoot().DescendantsOrSelfOfType("siteSettings").FirstOrDefault();
            if (siteSettings == null)
                throw new Exception("There are no site settings");

            var fromAddress = siteSettings.Value<string>("emailFromAddress");
            if (string.IsNullOrEmpty(fromAddress))
                throw new Exception("Missing from address for email in site settings");

            var toAddresses = siteSettings.Value<string>("emailAdminAccounts").Trim();
            if (string.IsNullOrEmpty(toAddresses))
                throw new Exception("Missing to addresses for email in site settings");

            var smtpMessage = CreateSmtpMessage(fromAddress, toAddresses, vm);
            //smtp client is picking up configuration from webConfig;
            using (var smtp = new SmtpClient())
            {
                smtp.Send(smtpMessage);
            }
        }

        private MailMessage CreateSmtpMessage(string fromAddress, string toAddresses, ContactFormViewModel vm)
        {
            var emailSubject = "There has been a contact for submitted";
            var emailBody = $"A new contact form has been received from {vm.Name}. Their comments were: {vm.Comment}";

            var smtpMessage = new MailMessage();
            smtpMessage.Subject = emailSubject;
            smtpMessage.Body = emailBody;
            smtpMessage.From = new MailAddress(fromAddress);
            smtpMessage.To.Add(toAddresses);

            return smtpMessage;
        }
    }
}
