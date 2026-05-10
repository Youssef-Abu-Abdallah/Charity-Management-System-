using System;

namespace WindowsFormsApp1.Models
{
    public class CharityNeedRequest
    {
        // التصنيف (Food=0, Clothing=1, Medical=2, Education=3, Other=4)
        public int Category { get; set; }

        // اسم المنتج (مطلوب - بحد أقصى 200 حرف)
        public string ProductName { get; set; }

        // الكمية (مطلوب - أقل قيمة 0.01)
        public double Quantity { get; set; }

        // الوحدة (Ton=0, Kg=1, Gram=2, Liter=3, Ml=4, Pack=5, Box=6, Can=7, Piece=8)
        public int Unit { get; set; }

        // الأولوية (Urgent=0, High=1, Normal=2, Low=3)
        public int Priority { get; set; }

        // وصف إضافي (اختياري - بحد أقصى 1000 حرف)
        public string Description { get; set; }

        // مسار الصورة على جهاز الكمبيوتر (مش هنبعت المسار للـ API، بس هنستخدمه عشان نفتح ملف الصورة)
        public string LocalImagePath { get; set; }
    }
}