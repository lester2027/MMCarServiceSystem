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
public class CarRepairDetail : BaseObject
{
    private Vehicle vehicle;

    public CarRepairDetail()
    {

    }
    public virtual ServiceOrder ServiceOrder { get; set; }
    public virtual Vehicle Vehicle
    {
        get => vehicle;
        set
        {
            if (vehicle != value)
            {
                vehicle = value;

                if (vehicle != null &&
                    vehicle.DesiredService == ServiceType.CarRepair &&
                    !string.IsNullOrEmpty(vehicle.DiagnosticNotes))
                {
                    DiagnosticNotes = vehicle.DiagnosticNotes;
                }
            }
        }
    }

    [ModelDefault("RowCount", "3")]
    public virtual string DiagnosticNotes { get; set; }

    [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy / hh:mm tt}")]
    [ModelDefault("EditMask", "dd/MM/yyyy / hh:mm tt")]
    public virtual DateTime StartDate { get; set; } = DateTime.Now;

    [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy / hh:mm tt}")]
    [ModelDefault("EditMask", "dd/MM/yyyy / hh:mm tt")]
    public virtual DateTime? CompletedDate { get; set; }
    public virtual Employee AssignedEmployee { get; set; }
    public virtual RepairStatus? RepairStatus { get; set; }
}

public enum RepairStatus
{
    Pending,
    InProgress,
    Completed
}