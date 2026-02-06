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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MMCarServiceSystem.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("CustomerManagement")]
[ImageName("Customers")]
public class Customer : BaseObject
{
    public Customer()
    {

    }

    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }

    [RuleRegularExpression(@"^(09|\+639)\d{9}$",
   CustomMessageTemplate = "Please enter a valid Philippine phone number (e.g., 09171234567 or +639171234567).")]
    public virtual string PhoneNumber { get; set; }

    [RuleRegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        CustomMessageTemplate = "Please enter a valid email address.")]
    public virtual string Email { get; set; }
    [MaxLength]
    public virtual string Address { get; set; }

}
