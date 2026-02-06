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
[NavigationItem("Management")]
[ImageName("HybridDemo_All Employees")]
public class Employee : BaseObject
{
    private Department? department;

    public Employee()
    {

    }

    public virtual string EmployeeName { get; set; }
    public virtual int Age { get; set; }
    public virtual string ContactNumber { get; set; }
    public virtual string Email { get; set; }
    public virtual DateTime HiredDate { get; set; }
    public virtual DateTime? ResignationDate { get; set; }
    public virtual EmploymentStatus? EmploymentStatus { get; set; }
    public virtual string Position { get; set; }

    public virtual Department? Department
    {
        get => department;
        set
        {
            if (department != value)
            {
                department = value;
                UpdateHourlyRateBasedOnDepartment();
            }
        }
    }

    [ModelDefault("AllowEdit", "False")]
    public virtual decimal HourlyRate { get; set; }

    public virtual ObservableCollection<ServiceOrder> ServiceOrders { get; set; } = new ObservableCollection<ServiceOrder>();
    public virtual ObservableCollection<Appointment> Appointments { get; set; } = new ObservableCollection<Appointment>();

    #region --Methods for Calculating Hourly Rate Based on Department--
    private void UpdateHourlyRateBasedOnDepartment()
    {
        if (Department.HasValue)
        {
            HourlyRate = Department.Value switch
            {
                BusinessObjects.Department.Manager => 30m,
                BusinessObjects.Department.Admin => 10m,
                BusinessObjects.Department.CarWash => 10m,
                BusinessObjects.Department.CarPaint => 15m,
                BusinessObjects.Department.CarDetailing => 15m,
                BusinessObjects.Department.CarRepair => 20m,
                _ => 0m
            };
        }
        else
        {
            HourlyRate = 0m;
        }
    }
}
#endregion
public enum EmploymentStatus
{
    Active,
    Inactive,
    OnLeave,
    Resigned
}

public enum Department
{
    Manager,
    Admin,
    CarWash,
    CarPaint,
    CarDetailing,
    CarRepair
}