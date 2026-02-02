using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
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
public class Payment : BaseObject
{
    private Invoice invoice;
    private decimal? amount;
    private decimal? amountTendered;

    public Payment()
    {
        DateOfPayment = DateTime.Now;
        PaymentStatus = BusinessObjects.PaymentStatus.Pending;
        ReceiptNumber = GenerateReceiptNumber();
    }

    [RuleUniqueValue("Payment_ReceiptNumber_Unique", DefaultContexts.Save)]
    [Appearance("Payment_ReceiptNumber_ReadOnly", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView")]
    public virtual string ReceiptNumber { get; set; }

    [RuleRequiredField("Payment_DateOfPayment_Required", DefaultContexts.Save)]
    public virtual DateTime DateOfPayment { get; set; }

    public virtual Invoice Invoice
    {
        get => invoice;
        set
        {
            if (invoice != value)
            {
                invoice = value;

                if (invoice != null)
                {
                    Amount = invoice.TotalAmount;
                }
                else
                {
                    Amount = null;
                }
            }
        }
    }

    [RuleRequiredField("Payment_Amount_Required", DefaultContexts.Save)]
    [RuleValueComparison("Payment_Amount_Positive", DefaultContexts.Save,
        ValueComparisonType.GreaterThan, 0,
        CustomMessageTemplate = "Amount must be greater than zero")]
    [ModelDefault("AllowEdit", "False")]
    [Appearance("Payment_Amount_ReadOnly", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView")]
    [ModelDefault("DisplayFormat", "{0:C2}")]
    public virtual decimal? Amount
    {
        get => amount;
        set => amount = value;
    }

    [RuleRequiredField("Payment_ModeOfPayment_Required", DefaultContexts.Save)]
    public virtual ModeOfPayment? ModeOfPayment { get; set; }

    public virtual PaymentStatus? PaymentStatus { get; set; }

    // Card/Account Details - Visible only for Credit/Debit Card
    [Appearance("Payment_CardLastFourDigits_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView",
        Criteria = "ModeOfPayment != 'CreditCard' And ModeOfPayment != 'DebitCard'")]
    [RuleRequiredField("Payment_CardNumber_Required", DefaultContexts.Save,
        TargetCriteria = "ModeOfPayment = 'CreditCard' OR ModeOfPayment = 'DebitCard'")]
    [RuleRegularExpression("Payment_CardNumber_Format", DefaultContexts.Save,
        @"^\d{4}$",
        CustomMessageTemplate = "Please enter last 4 digits only",
        TargetCriteria = "ModeOfPayment = 'CreditCard' OR ModeOfPayment = 'DebitCard'")]
    public virtual string CardLastFourDigits { get; set; }

    [Appearance("Payment_CardHolderName_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView",
        Criteria = "ModeOfPayment != 'CreditCard' And ModeOfPayment != 'DebitCard'")]
    [RuleRequiredField("Payment_CardHolderName_Required", DefaultContexts.Save,
        TargetCriteria = "ModeOfPayment = 'CreditCard' OR ModeOfPayment = 'DebitCard'")]
    public virtual string CardHolderName { get; set; }

    // Bank Transfer Details - Visible only for Bank Transfer
    [Appearance("Payment_BankName_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView",
        Criteria = "ModeOfPayment != 'BankTransfer'")]
    [RuleRequiredField("Payment_BankName_Required", DefaultContexts.Save,
        TargetCriteria = "ModeOfPayment = 'BankTransfer'")]
    public virtual string BankName { get; set; }

    [Appearance("Payment_TransactionReference_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView",
        Criteria = "ModeOfPayment != 'BankTransfer' And ModeOfPayment != 'MobilePayment'")]
    [RuleRequiredField("Payment_TransactionReference_Required", DefaultContexts.Save,
        TargetCriteria = "ModeOfPayment = 'BankTransfer' OR ModeOfPayment = 'MobilePayment'")]
    public virtual string TransactionReference { get; set; }

    // Mobile Payment Details - Visible only for Mobile Payment
    [Appearance("Payment_MobileProvider_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView",
        Criteria = "ModeOfPayment != 'MobilePayment'")]
    [RuleRequiredField("Payment_MobileProvider_Required", DefaultContexts.Save,
        TargetCriteria = "ModeOfPayment = 'MobilePayment'")]
    public virtual string MobilePaymentProvider { get; set; }

    [Appearance("Payment_MobileNumber_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView",
        Criteria = "ModeOfPayment != 'MobilePayment'")]
    [RuleRequiredField("Payment_MobileNumber_Required", DefaultContexts.Save,
        TargetCriteria = "ModeOfPayment = 'MobilePayment'")]
    [RuleRegularExpression("Payment_MobileNumber_Format", DefaultContexts.Save,
        @"^\+?[0-9]{10,15}$",
        CustomMessageTemplate = "Invalid mobile number format",
        TargetCriteria = "ModeOfPayment = 'MobilePayment'")]
    public virtual string MobileNumber { get; set; }

    // Authorization Code - Hidden by default, visible for all non-cash payments
    [Appearance("Payment_AuthorizationCode_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView",
        Criteria = "ModeOfPayment = 'Cash' Or ModeOfPayment Is Null")]
    public virtual string AuthorizationCode { get; set; }

    // Cash Payment Details - Visible only for Cash
    [Appearance("Payment_AmountTendered_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView",
        Criteria = "ModeOfPayment != 'Cash'")]
    [ModelDefault("DisplayFormat", "{0:C2}")]
    public virtual decimal? AmountTendered
    {
        get => amountTendered;
        set => amountTendered = value;
    }

    public override void OnSaving()
    {
        base.OnSaving();

        // Validate amount tendered for cash payments
        if (ModeOfPayment == BusinessObjects.ModeOfPayment.Cash)
        {
            if (!AmountTendered.HasValue)
            {
                throw new InvalidOperationException("Amount tendered is required for cash payments");
            }

            if (Amount.HasValue && AmountTendered.Value < Amount.Value)
            {
                throw new InvalidOperationException("Amount tendered must be equal or greater than payment amount");
            }
        }
    }

    [Appearance("Payment_ChangeAmount_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView",
        Criteria = "ModeOfPayment != 'Cash'")]
    [Appearance("Payment_ChangeAmount_ReadOnly", AppearanceItemType = "ViewItem",
        Enabled = false, Context = "DetailView")]
    [ModelDefault("DisplayFormat", "{0:C2}")]
    public virtual decimal? ChangeAmount
    {
        get
        {
            if (ModeOfPayment == BusinessObjects.ModeOfPayment.Cash && AmountTendered.HasValue && Amount.HasValue)
            {
                return AmountTendered.Value - Amount.Value;
            }
            return null;
        }
    }

    public virtual string Notes { get; set; }

    // Payment verification - Hidden by default
    [Appearance("Payment_IsVerified_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
    public virtual bool IsVerified { get; set; }

    [Appearance("Payment_VerifiedDate_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
    public virtual DateTime? VerifiedDate { get; set; }

    [Appearance("Payment_VerifiedBy_Visible", AppearanceItemType = "ViewItem",
        Visibility = ViewItemVisibility.Hide, Context = "DetailView")]
    public virtual string VerifiedBy { get; set; }

    // Business Logic Methods
    public void VerifyPayment(string verifiedBy)
    {
        if (PaymentStatus != BusinessObjects.PaymentStatus.Completed)
        {
            throw new InvalidOperationException("Only completed payments can be verified");
        }

        IsVerified = true;
        VerifiedDate = DateTime.Now;
        VerifiedBy = verifiedBy;
    }

    public void ProcessPayment()
    {
        if (!ValidatePaymentDetails())
        {
            throw new InvalidOperationException("Payment details are incomplete or invalid");
        }

        PaymentStatus = BusinessObjects.PaymentStatus.Completed;
    }

    private bool ValidatePaymentDetails()
    {
        if (!Amount.HasValue || Amount.Value <= 0)
            return false;

        switch (ModeOfPayment)
        {
            case BusinessObjects.ModeOfPayment.Cash:
                return AmountTendered.HasValue && AmountTendered >= Amount;

            case BusinessObjects.ModeOfPayment.CreditCard:
            case BusinessObjects.ModeOfPayment.DebitCard:
                return !string.IsNullOrEmpty(CardLastFourDigits) &&
                       !string.IsNullOrEmpty(CardHolderName);

            case BusinessObjects.ModeOfPayment.BankTransfer:
                return !string.IsNullOrEmpty(BankName) &&
                       !string.IsNullOrEmpty(TransactionReference);

            case BusinessObjects.ModeOfPayment.MobilePayment:
                return !string.IsNullOrEmpty(MobilePaymentProvider) &&
                       !string.IsNullOrEmpty(MobileNumber) &&
                       !string.IsNullOrEmpty(TransactionReference);

            default:
                return false;
        }
    }

    private string GenerateReceiptNumber()
    {
        return $"RCP-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
    }

    public void RefundPayment(string reason)
    {
        if (PaymentStatus != BusinessObjects.PaymentStatus.Completed)
        {
            throw new InvalidOperationException("Only completed payments can be refunded");
        }

        if (ModeOfPayment == BusinessObjects.ModeOfPayment.Cash)
        {
            PaymentStatus = BusinessObjects.PaymentStatus.Refunded;
            Notes = $"Refunded: {reason}. {Notes}";
        }
        else
        {
            PaymentStatus = BusinessObjects.PaymentStatus.Pending;
            Notes = $"Refund initiated: {reason}. Awaiting confirmation. {Notes}";
        }
    }
}

public enum ModeOfPayment
{
    [Description("Cash Payment")]
    Cash,

    [Description("Credit Card")]
    CreditCard,

    [Description("Debit Card")]
    DebitCard,

    [Description("Mobile Payment (GCash, PayMaya, etc.)")]
    MobilePayment,

    [Description("Bank Transfer")]
    BankTransfer
}

public enum PaymentStatus
{
    [Description("Payment is pending")]
    Pending,

    [Description("Payment completed successfully")]
    Completed,

    [Description("Payment failed")]
    Failed,

    [Description("Payment refunded")]
    Refunded
}