using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Config; // التأكد من استدعاء المكان المخزن فيه التوكن
using WindowsFormsApp1.Models;


using Newtonsoft.Json; // تأكد من وجود هذا السطر فوق
using System.Text;     // تأكد من وجود هذا السطر فوق


namespace WindowsFormsApp1.Services
{
    public class CharityService : BaseService
    {
        /// <summary>
        /// إرسال احتياج جديد مع صورة للـ API
        /// </summary>
        /// <param name="need">كائن يحتوي على بيانات الاحتياج ومسار الصورة</param>
       

        public async Task<List<CharityNeedDTO>> GetAllNeedsAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(AppConfig.AuthToken))
                {
                    _client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppConfig.AuthToken);
                }

                var response = await _client.GetAsync("charity/charity-needs");

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();

                    // هنا الحل: بنحول الـ JSON لكائن ديناميكي الأول
                    var apiResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

                    // حسب رسالة الخطأ، الـ API بيبعت البيانات جوه خاصية (غالباً اسمها data أو items)
                    // بناءً على الـ JSON اللي بعته قبل كدة، هنحاول نوصل للـ data
                    var data = apiResponse.data;

                    // تحويل الجزء الخاص بالبيانات فقط إلى القائمة بتاعتنا
                    string itemsJson = JsonConvert.SerializeObject(data);
                    return JsonConvert.DeserializeObject<List<CharityNeedDTO>>(itemsJson);
                }

                return new List<CharityNeedDTO>();
            }
            catch (Exception ex)
            {
                // عرض الخطأ بشكل أوضح للمساعدة في التشخيص
                Console.WriteLine($"Serialization Error: {ex.Message}");
                return new List<CharityNeedDTO>();
            }
        }


        public async Task<bool> CreateNeedAsync(CreateCharityNeedDTO need)
        {
            try
            {
                // 1. تجهيز التوكن
                if (!string.IsNullOrEmpty(AppConfig.AuthToken))
                {
                    _client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppConfig.AuthToken);
                }

                // 2. استخدام MultipartFormDataContent لأننا نرسل صورة وبيانات
                using (var content = new MultipartFormDataContent())
                {
                    // إضافة الحقول النصية والعددية
                    content.Add(new StringContent(need.ProductName), "ProductName");
                    content.Add(new StringContent(need.Quantity.ToString()), "Quantity");
                    content.Add(new StringContent(need.Category.ToString()), "Category");
                    content.Add(new StringContent(need.Unit.ToString()), "Unit");
                    content.Add(new StringContent(need.Priority.ToString()), "Priority");
                    content.Add(new StringContent(need.Description ?? ""), "Description");

                    // 3. معالجة الصورة (إذا وُجدت)
                    if (!string.IsNullOrEmpty(need.ProductImagePath) && File.Exists(need.ProductImagePath))
                    {
                        var fileStream = new FileStream(need.ProductImagePath, FileMode.Open, FileAccess.Read);
                        var fileContent = new StreamContent(fileStream);

                        // تحديد نوع الملف (اختياري لكن يفضل)
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                        content.Add(fileContent, "ProductImage", Path.GetFileName(need.ProductImagePath));
                    }

                    // 4. الإرسال للسيرفر
                    // الرابط بناءً على ملف الـ JSON هو: api/v1/charity/charity-needs
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





        public async Task<List<OfferDTO>> GetAllOffersAsync()
        {
            try
            {
                // استبدل الرابط أدناه بالرابط الكامل للسيرفر الخاص بك
                // جربنا هنا نضع المسار كاملاً لضمان عدم وجود تكرار في api/v1
                var response = await _client.GetAsync("https://waffer.runasp.net/api/v1/public/offers");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    // فك التشفير مع تجاهل حالة الأحرف تلقائياً
                    var apiResult = JsonConvert.DeserializeObject<ApiResponse>(json);

                    if (apiResult != null && apiResult.Data != null && apiResult.Data.Count > 0)
                    {
                        return apiResult.Data;
                    }
                }
                return new List<OfferDTO>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في الاتصال: " + ex.Message);
                return new List<OfferDTO>();
            }
        }


    }
}