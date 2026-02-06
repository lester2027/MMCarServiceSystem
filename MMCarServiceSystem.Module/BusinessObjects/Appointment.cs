using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;
using MMCarServiceSystem.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace MMCarServiceSystem.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("CustomerManagement")]
[ImageName("BO_Appointment")]
public class Appointment : BaseObject
{
    private bool isNewObject = true;

    public Appointment()
    {

    }

    public virtual string CustomerName { get; set; }
    public virtual string ContactNumber { get; set; }
    public virtual string Email { get; set; }
    public virtual string VehicleModel { get; set; }
    public virtual string VehicleLicensePlate { get; set; }
    public virtual DateTime ScheduleDate { get; set; } = DateTime.Now;
    public virtual ServiceType? Service { get; set; }
    public virtual string Notes { get; set; }

    public override void OnSaving()
    {
        base.OnSaving();

        
        if (isNewObject && !string.IsNullOrEmpty(Email))
        {
            SendConfirmationEmail();
        }
    }

    public override void OnLoaded()
    {
        base.OnLoaded();
        isNewObject = false;
    }

    private void SendConfirmationEmail()
    {
        try
        {
            
            string fromEmail = "lesterviloria1527@gmail.com"; 
            string fromPassword = "fpmh utsj ecok nydf"; 
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587;

            
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail, "MM Car Service System"); 
            mail.To.Add(Email); 
            mail.Subject = "Appointment Confirmation - MM Car Service System";

            #region -- Appointment Email Body --
            mail.Body = $@"


Dear {CustomerName},

Thank you for scheduling an appointment with MM Car Service System.

Appointment Details:
-------------------
Customer Name: {CustomerName}
Contact Number: {ContactNumber}
Email: {Email}
Vehicle Model: {VehicleModel}
License Plate: {VehicleLicensePlate}
Service Type: {Service}
Scheduled Date: {ScheduleDate:dddd, MMMM dd, yyyy 'at' hh:mm tt}
Notes: {Notes}

We look forward to serving you!

Best regards,
MM Car Service System Team

---
This is an automated message. Please do not reply to this email.
";
            #endregion

            mail.IsBodyHtml = false;

            
            SmtpClient smtp = new SmtpClient(smtpHost, smtpPort);
            smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;

            
            System.Diagnostics.Debug.WriteLine($"Sending confirmation email to: {Email}");
            smtp.Send(mail);
            System.Diagnostics.Debug.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to send confirmation email: {ex.Message}");
        }
    }
}