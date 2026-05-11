using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;


namespace WindowsFormsApp1.Models
{

    public class OfferDTO
    {
        [JsonProperty("offerId")]
        public string OfferId { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("donorOrganizationName")]
        public string DonorOrganizationName { get; set; }

        [JsonProperty("quantity")]
        public double Quantity { get; set; }

        // تأكد من إضافة السطرين دول تحديداً
        [JsonProperty("unit")]
        public int Unit { get; set; }

        [JsonProperty("category")]
        public int Category { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }

    public class ApiResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public List<OfferDTO> Data { get; set; }
    }


}
