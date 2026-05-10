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

            if (GlobalUser.CurrentUser == null) return;

            role = GlobalUser.CurrentUser.Role;

            // 1. تخصيص الأزرار بناءً على الـ Role
            SetupRoleUI();

            // 2. ربط الأحداث يدوياً للتأكد أنها تعمل
            btnNeeds.Click += btnNeeds_GeneralClick;

            // ربط زرار البروفايل (تأكد أن الاسم btnProfile مطابق لاسم الزرار في الـ Designer)
            btnProfile.Click += btnProfile_Click;

            btnUsers.Click += btnBrowseNeeds_Click;
        }


        private void btnBrowseNeeds_Click(object sender, EventArgs e)
        {
            // هنكريت UC_BrowseNeeds في الخطوة الجاية
            ShowControl(new UC_MyNeeds());
        }
        private void SetupRoleUI()
        {
            // تغيير النصوص بناءً على الصلاحية
            if (role == 2) // Admin
            {
                btnNeeds.Text = "   ✔️  توثيق طلبات الحسابات";
                lblUserRole.Text = "الصلاحية: مسؤول النظام";
                lblUserRole.ForeColor = Color.Gold;
            }
            else if (role == 0) // Charity
            {
                btnNeeds.Text = "   ➕  إنشاء احتياج جديد";
                // هنا التعديل اللي طلبته:
                btnUsers.Text = "   🔍  تصفح احتياجاتي";

                lblUserRole.Text = "الصلاحية: جمعية خيرية";
                lblUserRole.ForeColor = Color.White;
            }
            else if (role == 1) // Donor
            {
                btnNeeds.Text = "   🎁  تصفح الاحتياجات";
                lblUserRole.Text = "الصلاحية: جهة متبرعة";
                lblUserRole.ForeColor = Color.Cyan;
            }

            // التأكد من نص زر المعلومات
            btnProfile.Text = "   👤  معلوماتي";
        }

        // حدث زر معلوماتي
        private void btnProfile_Click(object sender, EventArgs e)
        {
            ShowControl(new UC_Profile());
        }

        // حدث زر المهام (إضافة احتياج أو توثيق)
        private void btnNeeds_GeneralClick(object sender, EventArgs e)
        {
            if (GlobalUser.CurrentUser == null) return;

            if (role == 2) ShowControl(new UC_VerifyUsers());
            else if (role == 0) ShowControl(new UC_AddNeed());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (GlobalUser.CurrentUser == null)
            {
                MessageBox.Show("بيانات المستخدم غير مكتملة، يرجى تسجيل الدخول.");
                this.Close();
                return;
            }

            lblUserName.Text = "مرحباً: " + GlobalUser.CurrentUser.UserName;

            // عند التحميل، اظهر صفحة البروفايل تلقائياً كصفحة رئيسية
            ShowControl(new UC_Profile());
        }

        private void ShowControl(UserControl control)
        {
            if (control == null) return;

            // pnlContent هو البانل اللي بيتعرض جواه الـ UserControls
            pnlContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(control);
            control.BringToFront();
        }

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