using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    // رجعنا partial والوراثة من UserControl عشان الـ Designer يهدأ
    public partial class UC_Profile : UserControl
    {
        public UC_Profile()
        {
            // استدعاء ميثود الـ Designer الأساسية
            //InitializeComponent();

            // استدعاء ميثود التصميم البرمجي الخاصة بنا
            SetupProfileUI();
        }

        private void SetupProfileUI()
        {
            // لا نحذف العناصر هنا لأن InitializeComponent لسه شغالة
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.RightToLeft = RightToLeft.Yes;

            var user = GlobalUser.CurrentUser;
            if (user == null) return;

            Label lblTitle = new Label
            {
                Text = "👤 معلوماتي الشخصية",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.DarkSlateBlue
            };

            int startY = 100;

            AddInfoLabel("اسم المستخدم:", user.UserName, startY);
            AddInfoLabel("نوع الحساب:", GetRoleName(user.Role), startY + 160);

            this.Controls.Add(lblTitle);
        }

        private void AddInfoLabel(string title, string value, int y)
        {
            Label lblT = new Label { Text = title, Location = new Point(20, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
            Label lblV = new Label { Text = value ?? "غير متوفر", Location = new Point(20, y + 30), Font = new Font("Segoe UI", 12), AutoSize = true, ForeColor = Color.FromArgb(64, 64, 64) };

            this.Controls.Add(lblT);
            this.Controls.Add(lblV);
        }

        private string GetRoleName(int role)
        {
            switch (role)
            {
                case 0: return "جمعية خيرية";
                case 1: return "متبرع (جهة مانحة)";
                case 2: return "مسؤول النظام (Admin)";
                default: return "غير محدد";
            }
        }
    }
}