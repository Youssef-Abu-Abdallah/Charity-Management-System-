using System;

namespace WindowsFormsApp1.Models
{
    public class CharityNeedDTO
    {
        public string id { get; set; }
        public string productName { get; set; }
        public double quantity { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
        public int category { get; set; }
        public int unit { get; set; }
        public int priority { get; set; }
        public int status { get; set; }

        // --- الخصائص المحسوبة بالعربي (باستخدام Get التقليدي) ---

        public string حالة_الطلب
        {
            get
            {
                switch (status)
                {
                    case 0: return "قيد الانتظار";
                    case 1: return "مقبول";
                    case 2: return "مرفوض";
                    case 3: return "مكتمل";
                    default: return "غير معروف";
                }
            }
        }

        public string التصنيف
        {
            get
            {
                switch (category)
                {
                    case 0: return "طعام";
                    case 1: return "ملابس";
                    case 2: return "طبي";
                    case 3: return "تعليم";
                    case 4: return "أخرى";
                    default: return "غير مصنف";
                }
            }
        }

        public string الأولوية
        {
            get
            {
                switch (priority)
                {
                    case 0: return "عاجل جداً";
                    case 1: return "مرتفعة";
                    case 2: return "عادية";
                    case 3: return "منخفضة";
                    default: return "غير محدد";
                }
            }
        }

        public string الوحدة
        {
            get
            {
                switch (unit)
                {
                    case 0: return "طن";
                    case 1: return "كيلو جرام";
                    case 2: return "جرام";
                    case 3: return "لتر";
                    case 4: return "مللي";
                    case 5: return "عبوة";
                    case 6: return "صندوق";
                    case 7: return "علبة";
                    case 8: return "قطعة";
                    default: return "وحدة";
                }
            }
        }
    }
}