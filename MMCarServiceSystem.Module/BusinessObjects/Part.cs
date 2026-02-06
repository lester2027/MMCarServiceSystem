using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;

namespace MMCarServiceSystem.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("Management")]
[ModelDefault("DefaultListViewSorting", "Service;ItemName")]
[ImageName("ViewSettings")]
public class Part : BaseObject
{
    private int stock;

    public Part()
    {
    }

    public virtual string ItemName { get; set; }
    public virtual string ItemDescription { get; set; }
    public virtual decimal ItemPrice { get; set; }
    public virtual ServiceType? Service { get; set; }

    [RuleRange(0, int.MaxValue, CustomMessageTemplate = "Stock cannot be negative!")]
    public virtual int Stock
    {
        get => stock;
        set
        {
            if (value < 0)
            {
                throw new InvalidOperationException($"Insufficient stock for {ItemName}. Available: {stock}, Requested: {stock - value}");
            }
            stock = value;
        }
    }
    public enum ServiceType
    {
        CarWash,
        CarPaint,
        CarDetailing,
        CarRepair,
    }
}