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


       




    



    }
}