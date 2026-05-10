using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1.Config;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    public partial class MainForm : Form
    {
        int role;
        public MainForm()
        {
            InitializeComponent();
            role = GlobalUser.CurrentUser.Role;
            if (role == 2) // Admin
            {
                btnNeeds.Text = "   ✔️  توثيق طلبات الحسابات"; // النص المخصص للآدمن
                lblUserRole.Text = "الصلاحية: مسؤول النظام";
                lblUserRole.ForeColor = Color.Gold;
            }
            else if (role == 0) // Charity
            {
                btnNeeds.Text = "   ➕  إنشاء احتياج جديد"; // النص المخصص للجمعية
                lblUserRole.Text = "الصلاحية: جمعية خيرية";
                lblUserRole.ForeColor = Color.White;
            }
            btnNeeds.Click += btnNeeds_GeneralClick;
        }

        // هذه الميثود هي التي ستنفذ عند الضغط على الزر
        private void btnNeeds_GeneralClick(object sender, EventArgs e)
        {
            if (GlobalUser.CurrentUser == null) return;


            if (role == 2) // Admin
            {
                ShowControl(new UC_VerifyUsers());
            }
            else if (role == 0) // Charity
            {
                ShowControl(new UC_AddNeed());
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (GlobalUser.CurrentUser == null)
            {
                MessageBox.Show("بيانات المستخدم غير مكتملة، يرجى تسجيل الدخول.");
                this.Close();
                return;
            }

            // تحديث البيانات في الواجهة
            lblUserName.Text = "مرحباً: " + GlobalUser.CurrentUser.UserName.ToString();

            // استدعاء توزيع الصلاحيات
            ApplyUserPermissions();
        }
        private void ApplyUserPermissions()
        {
            if (GlobalUser.CurrentUser == null) return;

            int userRole = GlobalUser.CurrentUser.Role;

            // حالة مسؤول النظام (Admin)
            if (userRole == 2)
            {
                btnNeeds.Text = "   ✔️  توثيق طلبات الحسابات"; // النص المخصص للآدمن
                lblUserRole.Text = "الصلاحية: مسؤول النظام";
                lblUserRole.ForeColor = Color.Gold;

                // فتح شاشة التوثيق فوراً
                ShowControl(new UC_VerifyUsers());
            }
            // حالة الجمعية الخيرية (Charity)
            else if (userRole == 0)
            {
                btnNeeds.Text = "   ➕  إنشاء احتياج جديد"; // النص المخصص للجمعية
                lblUserRole.Text = "الصلاحية: جمعية خيرية";
                lblUserRole.ForeColor = Color.White;

                // فتح شاشة إضافة احتياج فوراً
                ShowControl(new UC_AddNeed());
            }
            // حالة المتبرع (Donor) - لو الـ Role بتاعها 1
            else if (userRole == 1)
            {
                btnNeeds.Text = "   🎁  تصفح الاحتياجات";
                lblUserRole.Text = "الصلاحية: جهة متبرعة";
                lblUserRole.ForeColor = Color.Cyan;
            }
        }

        // أضف هاتين الميثودين داخل كلاس MainForm لضمان سهولة الاستدعاء
        private void AdminClick(object sender, EventArgs e) => ShowControl(new UC_VerifyUsers());
        private void CharityClick(object sender, EventArgs e) => ShowControl(new UC_AddNeed());


        private void ShowControl(UserControl control)
        {
            if (control == null) return;

            pnlContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(control);
            control.BringToFront();
        }

        // --- حذفنا ميثود btnNeeds_Click القديمة تماماً لأنها كانت تفتح التوثيق إجبارياً ---

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }


    }
}