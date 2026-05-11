using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Linq;
using WindowsFormsApp1.Config;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    public class CharityService : BaseService
    {
        // 1. جلب الاحتياجات الخاصة بالجمعية
        public async Task<List<CharityNeedDTO>> GetAllNeedsAsync()
        {
            try
            {
                SetAuthHeader();
                var response = await _client.GetAsync("charity/charity-needs");

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<CharityNeedDTO>>>(jsonString);
                    return apiResponse?.Data ?? new List<CharityNeedDTO>();
                }
                return new List<CharityNeedDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<CharityNeedDTO>();
            }
        }

        // 2. إنشاء احتياج جديد (مع صورة)
        public async Task<bool> CreateNeedAsync(CreateCharityNeedDTO need)
        {
            try
            {
                SetAuthHeader();
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(need.ProductName), "ProductName");
                    content.Add(new StringContent(need.Quantity.ToString()), "Quantity");
                    content.Add(new StringContent(need.Category.ToString()), "Category");
                    content.Add(new StringContent(need.Unit.ToString()), "Unit");
                    content.Add(new StringContent(need.Priority.ToString()), "Priority");
                    content.Add(new StringContent(need.Description ?? ""), "Description");

                    if (!string.IsNullOrEmpty(need.ProductImagePath) && File.Exists(need.ProductImagePath))
                    {
                        var fileStream = new FileStream(need.ProductImagePath, FileMode.Open, FileAccess.Read);
                        var fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                        content.Add(fileContent, "ProductImage", Path.GetFileName(need.ProductImagePath));
                    }

                    var response = await _client.PostAsync("charity/charity-needs", content);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating need: {ex.Message}");
                return false;
            }
        }

        // 3. تصفح التبرعات المتاحة للجميع
        public async Task<List<OfferDTO>> GetAllOffersAsync()
        {
            try
            {
                var response = await _client.GetAsync("public/offers");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<ApiResponse<List<OfferDTO>>>(json);
                    return apiResult?.Data ?? new List<OfferDTO>();
                }
                return new List<OfferDTO>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في الاتصال: " + ex.Message);
                return new List<OfferDTO>();
            }
        }

        // 4. التقديم على تبرع
        public async Task<bool> ApplyForOfferAsync(string offerId)
        {
            try
            {
                SetAuthHeader();
                var response = await _client.PostAsync($"charity/offers/{offerId}/apply", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying for offer: {ex.Message}");
                return false;
            }
        }

        // 5. الميثود المطلوبة: جلب طلباتي التي قدمت عليها (الحل النهائي)
        public async Task<List<CharityApplicationDTO>> GetMySentApplicationsAsync()
        {
            try
            {
                SetAuthHeader();

                // الرابط مأخوذ من ملف الـ OpenAPI الذي أرسلته
                var response = await _client.GetAsync("charity/applications/sent");

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();

                    // تحويل الـ JSON لكائن مرن (JObject)
                    var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(jsonString);

                    // استخراج المصفوفة الموجودة داخل حقل "data"
                    var dataToken = jsonObject["data"];

                    if (dataToken != null && dataToken.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                    {
                        // تحويل المصفوفة مباشرة لقائمة من الـ DTO
                        return dataToken.ToObject<List<CharityApplicationDTO>>();
                    }
                }
                return new List<CharityApplicationDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Critical Error: {ex.Message}");
                return new List<CharityApplicationDTO>();
            }
        }


        // ميثود مساعدة لضبط الهيدر ومنع التكرار
        private void SetAuthHeader()
        {
            if (!string.IsNullOrEmpty(AppConfig.AuthToken))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AppConfig.AuthToken);
            }
        }
    }
}