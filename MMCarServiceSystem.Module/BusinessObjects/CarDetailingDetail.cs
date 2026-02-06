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

namespace MMCarServiceSystem.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("Services")]
[ImageName("Travel_Car")]
public class CarDetailingDetail : BaseObject
{
    public CarDetailingDetail()
    {

    }
    public virtual ServiceOrder ServiceOrder { get; set; }
    public virtual Vehicle Vehicle { get; set; }
    public virtual bool InteriorDetailing { get; set; }
    public virtual bool ExteriorDetailing { get; set; }

    [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy hh:mm tt}")]
    [ModelDefault("EditMask", "dd/MM/yyyy hh:mm tt")]
    public virtual DateTime StartDate { get; set; } = DateTime.Now;


    [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy hh:mm tt}")]
    [ModelDefault("EditMask", "dd/MM/yyyy hh:mm tt")]
    public virtual DateTime? CompletedDate { get; set; }
    public virtual Employee AssignedEmployee { get; set; }
    public virtual DetailingStatus? DetailingStatus { get; set; }
}

public enum DetailingStatus
{
    Pending,
    InProgress,
    Completed
}
