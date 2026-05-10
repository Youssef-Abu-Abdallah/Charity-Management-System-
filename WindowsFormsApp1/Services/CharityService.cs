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


namespace WindowsFormsApp1.Services
{
    public class CharityService : BaseService
    {
        /// <summary>
        /// إرسال احتياج جديد مع صورة للـ API
        /// </summary>
        /// <param name="need">كائن يحتوي على بيانات الاحتياج ومسار الصورة</param>
        public async Task<bool> CreateNeedAsync(CreateCharityNeedDTO need)
        {
            try
            {
                // 1. استخدام التوكن المخزن في AppConfig لتفويض العملية (Authorization)
                // نفترض أن التوكن مخزن في AppConfig.Token أو AppConfig.AccessToken
                if (!string.IsNullOrEmpty(AppConfig.AuthToken))
                {
                    _client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", AppConfig.AuthToken);
                }

                using (var content = new MultipartFormDataContent())
                {
                    // 2. إضافة البيانات النصية والـ Enums كما يتوقعها الـ API (Key-Value)
                    content.Add(new StringContent(((int)need.Category).ToString()), "Category");
                    content.Add(new StringContent(need.ProductName), "ProductName");
                    content.Add(new StringContent(need.Quantity.ToString()), "Quantity");
                    content.Add(new StringContent(((int)need.Unit).ToString()), "Unit");
                    content.Add(new StringContent(((int)need.Priority).ToString()), "Priority");

                    if (!string.IsNullOrEmpty(need.Description))
                    {
                        content.Add(new StringContent(need.Description), "Description");
                    }

                    // 3. تحويل الصورة المختارة إلى Stream لإرسالها
                    if (!string.IsNullOrEmpty(need.ProductImagePath) && File.Exists(need.ProductImagePath))
                    {
                        var fileStream = new FileStream(need.ProductImagePath, FileMode.Open, FileAccess.Read);
                        var fileContent = new StreamContent(fileStream);

                        // تحديد نوع الملف بناءً على الامتداد لضمان القبول من السيرفر
                        string extension = Path.GetExtension(need.ProductImagePath).ToLower();
                        string mimeType = (extension == ".png") ? "image/png" : "image/jpeg";
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

                        // أضفنا "ProductImage" كاسم للحقل بناءً على ملف الـ JSON
                        content.Add(fileContent, "ProductImage", Path.GetFileName(need.ProductImagePath));
                    }

                    // 4. الإرسال للمسار المحدد في الـ API
                    // ملاحظة: BaseService عادة بتعرف الـ BaseAddress، لذا نكتفي بالمسار النسبي
                    var response = await _client.PostAsync("charity/charity-needs", content);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                // يمكنك تسجيل الخطأ هنا (Logging) إذا لزم الأمر
                Console.WriteLine($"Error creating charity need: {ex.Message}");
                return false;
            }
        }

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

    }
}