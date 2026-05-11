using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class SentApplicationDTO
    {
        public string Id { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; }
        public OfferDTO Offer { get; set; } // بيانات العرض المرتبط بالطلب
    }
}
