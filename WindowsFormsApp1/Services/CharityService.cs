using System;
using WindowsFormsApp1.Models;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Services
{
    public class CharityService : BaseService
    {
        // إرسال احتياج جديد مع صورة
        public async Task<bool> CreateNeedAsync(CreateCharityNeedDTO need)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    // 1. إضافة البيانات النصية والـ Enums
                    content.Add(new StringContent(((int)need.Category).ToString()), "Category");
                    content.Add(new StringContent(need.ProductName), "ProductName");
                    content.Add(new StringContent(need.Quantity.ToString()), "Quantity");
                    content.Add(new StringContent(((int)need.Unit).ToString()), "Unit");
                    content.Add(new StringContent(((int)need.Priority).ToString()), "Priority");
                    content.Add(new StringContent(need.Description), "Description");

                    // 2. معالجة ورفع الصورة
                    if (!string.IsNullOrEmpty(need.ProductImagePath) && File.Exists(need.ProductImagePath))
                    {
                        var fileStream = new FileStream(need.ProductImagePath, FileMode.Open, FileAccess.Read);
                        var fileContent = new StreamContent(fileStream);
                        // تحديد نوع الملف (Image)
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                        content.Add(fileContent, "ProductImage", Path.GetFileName(need.ProductImagePath));
                    }

                    // 3. الإرسال للمسار المحدد في الـ API
                    var response = await _client.PostAsync("charity/charity-needs", content);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}