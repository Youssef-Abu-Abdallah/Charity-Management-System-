using System;

namespace WindowsFormsApp1.Models // تأكد أن الـ namespace هنا مطابق لباقي المشروع
{
    public class CreateCharityNeedDTO
    {
        public int Category { get; set; } // تم تغيير النوع لـ int لتسهيل التعامل مع الـ API
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public int Unit { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }
        public string ProductImagePath { get; set; }
    }
}