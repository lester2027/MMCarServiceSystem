using DevExpress.CodeParser;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.EFCore.Utils;
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
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace MMCarServiceSystem.Module.BusinessObjects;

[DefaultClassOptions]
public class CarPaintDetail : BaseObject
{
    private bool isFullBodyPaint;

    public CarPaintDetail()
    {

    }
    public virtual ServiceOrder ServiceOrder { get; set; }
    public virtual Vehicle Vehicle { get; set; }
    public virtual string PaintColor { get; set; }
    public virtual PaintType? PainType { get; set; }

    public virtual bool IsFullBodyPaint
    {
        get => isFullBodyPaint;
        set
        {
            if (isFullBodyPaint != value)
            {
                isFullBodyPaint = value;

                if (isFullBodyPaint)
                {
                    DesiredArea = null;
                }
            }
        }
    }

    [Appearance("CarPaintDetail_DesiredArea_Disabled", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView",
        Criteria = "IsFullBodyPaint = true")]
    [ModelDefault("RowCount", "2")]
    public virtual string DesiredArea { get; set; }

    [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy hh:mm tt}")]
    [ModelDefault("EditMask", "dd/MM/yyyy hh:mm tt")]
    public virtual DateTime StartDate { get; set; } = DateTime.Now;


    [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy hh:mm tt}")]
    [ModelDefault("EditMask", "dd/MM/yyyy hh:mm tt")]
    public virtual DateTime? CompletedDate { get; set; }
    public virtual Employee AssignedEmployee { get; set; }
    public virtual PaintStatus? PaintStatus { get; set; }

}


public enum PaintType
{
    Metallic,
    Matte,
    Glossy
}

public enum PaintStatus
{
    Pending,
    InProgress,
    Completed
}