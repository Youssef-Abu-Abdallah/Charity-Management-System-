using WindowsFormsApp1.Models;

namespace WindowsFormsApp1
{
    public static class GlobalUser
    {
        // هذا المتغير سيخزن كائن المستخدم بالكامل (بما فيه الـ Role والاسم)
        public static UserData CurrentUser { get; set; }
    }
}