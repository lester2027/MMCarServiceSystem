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
[DefaultProperty(nameof(InvoiceNumber))]
public class Invoice : BaseObject
{
    private ServiceOrder serviceOrder;
    private decimal subTotal;
    private decimal taxAmount;
    private decimal totalAmount;

    public Invoice()
    {
        InvoiceDate = DateTime.Now;
        InvoiceNumber = GenerateInvoiceNumber();
    }

    [Appearance("Invoice_InvoiceNumber_ReadOnly", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView")]
    public virtual string InvoiceNumber { get; set; }

    public virtual ServiceOrder ServiceOrder
    {
        get => serviceOrder;
        set
        {
            if (serviceOrder != value)
            {
                serviceOrder = value;
                if (serviceOrder != null)
                {
                    // Automatically set SubTotal from ServiceOrder's TotalFee
                    SubTotal = serviceOrder.TotalFee;
                    CalculateTotals();
                }
                else
                {
                    // Reset all values when ServiceOrder is cleared
                    SubTotal = 0m;
                    TaxAmount = 0m;
                    TotalAmount = 0m;
                }
            }
        }
    }

    public virtual Customer Customer { get; set; }
    public virtual DateTime InvoiceDate { get; set; }

    [ModelDefault("AllowEdit", "False")]
    [Appearance("Invoice_SubTotal_ReadOnly", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView")]
    [ModelDefault("DisplayFormat", "{0:C2}")]
    public virtual decimal SubTotal
    {
        get => subTotal;
        set => subTotal = value;
    }

    [ModelDefault("AllowEdit", "False")]
    [Appearance("Invoice_TaxAmount_ReadOnly", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView")]
    [ModelDefault("DisplayFormat", "{0:C2}")]
    public virtual decimal TaxAmount
    {
        get => taxAmount;
        set => taxAmount = value;
    }

    [ModelDefault("AllowEdit", "False")]
    [Appearance("Invoice_TotalAmount_ReadOnly", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView")]
    [ModelDefault("DisplayFormat", "{0:C2}")]
    public virtual decimal TotalAmount
    {
        get => totalAmount;
        set => totalAmount = value;
    }

    public virtual ObservableCollection<ServiceOrder> ServiceOrders { get; set; } = new ObservableCollection<ServiceOrder>();
    public virtual ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();

    private string GenerateInvoiceNumber()
    {
        return $"INV-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
    }

    private void CalculateTotals()
    {
        const decimal taxRate = 0.12m; // 12% tax
        TaxAmount = Math.Round(SubTotal * taxRate, 2);
        TotalAmount = SubTotal + TaxAmount;
    }

    // Override ToString to display InvoiceNumber instead of object ID
    public override string ToString()
    {
        return InvoiceNumber ?? base.ToString();
    }
}