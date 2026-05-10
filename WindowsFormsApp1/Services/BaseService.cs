using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; // تأكد من تثبيت مكتبة Newtonsoft.Json من NuGet
using WindowsFormsApp1.Config;

namespace WindowsFormsApp1.Services
{
    public class BaseService
    {
        protected readonly HttpClient _client;

        public BaseService()
        {
            _client = new HttpClient();

            // العنوان الأساسي من الـ Config
            _client.BaseAddress = new Uri(AppConfig.BaseUrl);

            // إعدادات الـ Headers الأساسية
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // إضافة التوكن تلقائياً إذا كان متوفراً (AuthToken)
            if (!string.IsNullOrEmpty(AppConfig.AuthToken))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AppConfig.AuthToken);
            }
        }

        // دالة لجلب البيانات (GET)
        protected async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
                return default;
            }
            catch (Exception)
            {
                // يمكن إضافة Log للخطأ هنا
                return default;
            }
        }

        // دالة لتحديث البيانات (PATCH) - مطلوبة لعمليات التوثيق والموافقة
        protected async Task<TResponse> PatchAsync<TResponse, TRequest>(string endpoint, TRequest data)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // إنشاء طلب Patch يدوي لضمان التوافق
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
                {
                    Content = content
                };

                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TResponse>(jsonResponse);
                }
                return default;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}