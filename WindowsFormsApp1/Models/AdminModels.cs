using System;
using System.Collections.Generic;

namespace WindowsFormsApp1.Models
{
    // 1. بيانات الجمعية المعلقة (بناءً على الـ JSON الفعلي)
    public class PendingCharityDTO
    {
        public Guid charityId { get; set; }
        public Guid userId { get; set; }
        public string charityName { get; set; }
        public string email { get; set; }
        public string city { get; set; }
        public string governorate { get; set; }
        public string imageUrl { get; set; }
        public DateTime createdAt { get; set; }
    }

    // 2. بيانات جهة التبرع المعلقة
    public class PendingDonorDTO
    {
        public Guid donorOrganizationId { get; set; }
        public Guid userId { get; set; }
        public string donorOrganizationName { get; set; }
        public string email { get; set; }
        public string city { get; set; }
        public string governorate { get; set; }
        public string imageUrl { get; set; }
        public DateTime createdAt { get; set; }
    }

    // 3. الكائن الذي يحتوي على القائمتين داخل حقل data
    public class PendingVerificationData
    {
        public List<PendingCharityDTO> pendingCharities { get; set; }
        public List<PendingDonorDTO> pendingDonors { get; set; }
    }

    // 4. الرد النهائي من السيرفر (Root Object)
    public class PendingUsersResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public PendingVerificationData data { get; set; }
        public object pagination { get; set; }
        public object error { get; set; }
        public DateTime timestamp { get; set; }
    }

    // 5. الكلاس المطلوب لإرسال أمر التوثيق (لحل مشكلة AdminService)
    public class ActionUserRequestDTO
    {
        public Guid userId { get; set; }
    }
}