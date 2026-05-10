using Newtonsoft.Json;

namespace WindowsFormsApp1.Models
{
    // الطبقة الخارجية للرد
    public class LoginResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public UserData Data { get; set; }
    }

    // البيانات اللي جوه الـ data
    public class UserData
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("role")]
        public int Role { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}