using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Config;

namespace WindowsFormsApp1.Services
{
    public class AuthService : BaseService
    {
        public async Task<bool> LoginAsync(string identifier, string password)
        {
            try
            {
                var loginData = new
                {
                    usernameOrEmail = identifier,
                    password = password,
                    rememberMe = false
                };

                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("auth/login", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<LoginResponse>(responseString);

                    if (result != null && result.Success && result.Data != null)
                    {
                        // 1. تخزين التوكن في الإعدادات (للاستخدام في الـ Headers)
                        AppConfig.AuthToken = result.Data.Token;

                        // 2. أهم خطوة: تخزين بيانات المستخدم بالكامل في الخزنة العالمية
                        // بنساوي كائن اليوزر اللي جاي من السيرفر بالخزنة بتاعتنا
                        GlobalUser.CurrentUser = result.Data;

                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
}