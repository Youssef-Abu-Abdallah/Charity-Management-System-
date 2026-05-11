using Newtonsoft.Json;
using System;

public class CharityApplicationDTO
{
    [JsonProperty("offerApplicationId")]
    public string OfferApplicationId { get; set; }

    [JsonProperty("productName")]
    public string ProductName { get; set; }

    [JsonProperty("donorOrganizationName")]
    public string DonorOrganizationName { get; set; }

    [JsonProperty("quantity")]
    public double Quantity { get; set; } // الـ JSON بيقول إنه number/double

    [JsonProperty("unit")]
    public int Unit { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
}