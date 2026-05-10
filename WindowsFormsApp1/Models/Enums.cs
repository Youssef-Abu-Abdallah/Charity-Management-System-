using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    // أنواع الحسابات
    public enum UserRole
    {
        Charity = 0,
        DonorOrganization = 1,
        Admin = 2
    }

    // حالة طلب الانضمام أو الاحتياجات
    public enum ApplicationStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
        Fulfilled = 3 // مكتمل
    }

    // تصنيفات المنتجات
    public enum ProductCategory
    {
        Food = 0,
        Clothing = 1,
        Medical = 2,
        Education = 3,
        Other = 4
    }

    // وحدات القياس
    public enum MeasurementUnit
    {
        Ton = 0, Kg = 1, Gram = 2, Liter = 3, Ml = 4,
        Pack = 5, Box = 6, Can = 7, Piece = 8
    }

    public enum CharityNeedPriority
    {
        Urgent = 0,
        High = 1,
        Normal = 2,
        Low = 3
    }

}
