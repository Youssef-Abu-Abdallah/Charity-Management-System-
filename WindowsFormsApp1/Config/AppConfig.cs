namespace WindowsFormsApp1.Config
{
    public static class AppConfig
    {
        public static string BaseUrl = "https://waffer.runasp.net/api/v1/";
        public static string AuthToken { get; set; }
        public static string CurrentUserName { get; set; }
        public static int CurrentUserRoleID { get; set; } // لازم يكون static int
    }
}


