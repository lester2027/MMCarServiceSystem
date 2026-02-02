using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;
using Microsoft.CodeAnalysis;
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
[DefaultProperty(nameof(PlateNumber))]
public class Vehicle : BaseObject
{
    public Vehicle()
    {

    }

    public virtual string PlateNumber { get; set; }
    public virtual string Color { get; set; }
    public virtual string Model { get; set; }

    [ModelDefault("DisplayFormat", "{0:N0} km")]
    [ModelDefault("EditMask", "d")]
    public virtual int Mileage { get; set; } = 0;

    [RuleRange(1886, 2100, CustomMessageTemplate = "Please enter a valid car year.")]
    [ModelDefault("DisplayFormat", "####")]
    [ModelDefault("EditMask", "####")]
    [ModelDefault("AllowSpin", "False")]
    public virtual int? YearModel { get; set; } = DateTime.Now.Year;

    public virtual ServiceType? DesiredService { get; set; }

    [VisibleInListView(false)]
    [ModelDefault("RowCount", "3")]
    [Appearance("Vehicle_DiagnosticNotes_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView",
        Criteria = "DesiredService != 'CarRepair'")]
    [RuleRequiredField("Vehicle_DiagnosticNotes_Required", DefaultContexts.Save,
        TargetCriteria = "DesiredService = 'CarRepair'",
        CustomMessageTemplate = "Diagnostic Notes is required when Desired Service is Car Repair")]
    public virtual string DiagnosticNotes { get; set; }
    public virtual Customer Customer { get; set; }
}

public enum DesiredService
{
    CarWash,
    CarPaint,
    CarDetailing,
    CarRepair,
}