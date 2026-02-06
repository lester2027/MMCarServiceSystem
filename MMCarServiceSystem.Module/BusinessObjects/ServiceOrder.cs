using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
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
[NavigationItem("Management")]
[DefaultProperty(nameof(OrderNumber))]
[ImageName("BO_Report")]
public class ServiceOrder : BaseObject
{
    #region -- Fields --
    private Part part;
    private int quantityOfPartsUsed;
    private decimal subTotal;
    private ServiceType? serviceType;
    private decimal serviceFee;
    private decimal totalFee;
    #endregion
    public ServiceOrder()
    {
        if (string.IsNullOrEmpty(OrderNumber))
        {
            OrderNumber = GenerateOrderNumber();
        }
        StartDate = DateTime.Now;
    }

    [ModelDefault("AllowEdit", "False")]
    public virtual string OrderNumber { get; set; }
    public virtual Customer CustomerName { get; set; }
    public virtual Vehicle PlateNumber { get; set; }
    public virtual DateTime StartDate { get; set; }
    public virtual DateTime CompletionDate { get; set; }
    public virtual Status? Status { get; set; }
    public virtual ServiceType? ServiceType
    {
        get => serviceType;
        set
        {
            if (serviceType != value)
            {
                serviceType = value;
                UpdateServiceFee();
                UpdateTotalFee();
            }
        }
    }
    public virtual Employee AssignedEmployee { get; set; }
    public virtual Part Part
    {
        get => part;
        set
        {
            if (part != value)
            {
                if (part != null && quantityOfPartsUsed > 0)
                {
                    part.Stock += quantityOfPartsUsed;
                }
                part = value;
                if (part != null && quantityOfPartsUsed > 0)
                {
                    part.Stock -= quantityOfPartsUsed;
                }
                if (part != null)
                {
                    PriceOfParts = part.ItemPrice;
                    UpdateSubTotal();
                }
            }
        }
    }

    [ModelDefault("AllowEdit", "False")]
    [Appearance("ServiceOrder_PriceOfParts_ReadOnly", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView")]
    public virtual decimal PriceOfParts { get; set; }
    public virtual int QuantityOfPartsUsed
    {
        get => quantityOfPartsUsed;
        set
        {
            if (quantityOfPartsUsed != value)
            {
                int difference = value - quantityOfPartsUsed;

                if (part != null)
                {
                    part.Stock -= difference;
                }
                quantityOfPartsUsed = value;
                UpdateSubTotal();
            }
        }
    }

    [ModelDefault("AllowEdit", "False")]
    [Appearance("ServiceOrder_SubTotal_ReadOnly", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView")]
    public virtual decimal SubTotal
    {
        get => subTotal;
        set => subTotal = value;
    }

    [ModelDefault("AllowEdit", "False")]
    [Appearance("ServiceOrder_ServiceFee_ReadOnly", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView")]
    public virtual decimal ServiceFee
    {
        get => serviceFee;
        set => serviceFee = value;
    }

    [ModelDefault("AllowEdit", "False")]
    [Appearance("ServiceOrder_TotalFee_ReadOnly", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView")]
    public virtual decimal TotalFee
    {
        get => totalFee;
        set => totalFee = value;
    }
    private void UpdateSubTotal()
    {
        SubTotal = PriceOfParts * QuantityOfPartsUsed;
        UpdateTotalFee();
    }
    private void UpdateServiceFee()
    {
        ServiceFee = ServiceType switch
        {
            BusinessObjects.ServiceType.CarWash => 50m,
            BusinessObjects.ServiceType.CarPaint => 100m,
            BusinessObjects.ServiceType.CarDetailing => 100m,
            BusinessObjects.ServiceType.CarRepair => 150m,
            _ => 0m
        };
    }
    private void UpdateTotalFee()
    {
        TotalFee = SubTotal + ServiceFee;
    }


    public virtual ObservableCollection<CarWashDetail> CarWashDetails { get; set; } = new ObservableCollection<CarWashDetail>();
    public virtual ObservableCollection<CarPaintDetail> CarPaintDetails { get; set; } = new ObservableCollection<CarPaintDetail>();
    public virtual ObservableCollection<CarDetailingDetail> CarDetailingDetails { get; set; } = new ObservableCollection<CarDetailingDetail>();
    public virtual ObservableCollection<CarRepairDetail> CarRepairDetails { get; set; } = new ObservableCollection<CarRepairDetail>();

    private string GenerateOrderNumber()
    {
        var prefix = "SO";
        var timestamp = DateTime.Now.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        return $"{prefix}-{timestamp}-{random}";
    }
}


public enum ServiceType
{
    [Description("Car Wash")]
    CarWash,

    [Description("Car Paint")]
    CarPaint,

    [Description("Car Detailing")]
    CarDetailing,

    [Description("Car Repair")]
    CarRepair
}

public enum Status
{
    [Description("Pending")]
    Pending,

    [Description("In Progress")]
    InProgress,

    [Description("Completed")]
    Completed,

    [Description("Cancelled")]
    Cancelled
}