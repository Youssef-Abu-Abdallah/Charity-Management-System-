using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Splash());



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. عرض الـ Splash Screen أولاً
            // استبدل SplashScreen باسم الفورم اللي فيها اللوجو عندك
            Splash splash = new Splash();
            Application.Run(splash);

            // 2. بعد ما الـ Splash تقفل (this.Close)، البرنامج هيكمل للسطر اللي بعده
            // هنا بنفتح شاشة الـ Login اللي عملناها
            Application.Run(new WindowsFormsApp1.Forms.LoginForm());


        }
    }
}
