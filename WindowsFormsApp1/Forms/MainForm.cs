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

            this.StartPosition = FormStartPosition.CenterScreen;

            if (GlobalUser.CurrentUser == null) return;

            role = GlobalUser.CurrentUser.Role;

            // 1. تخصيص النصوص والألوان بناءً على الرتبة
            SetupRoleUI();

            // 2. تحسين مظهر الأزرار الحالية مع الحفاظ على ألوانك
            ApplyStyleToExistingButtons();

            // 3. إضافة الزر الجديد برمجياً وتنسيق مكانه
            SetupCustomButtons();

            // ربط الأحداث
            btnNeeds.Click += btnNeeds_GeneralClick;
            btnProfile.Click += btnProfile_Click;
            btnUsers.Click += btnBrowseNeeds_Click;
        }

        private void ApplyStyleToExistingButtons()
        {
            Button[] sideButtons = { btnNeeds, btnUsers, btnProfile, btnLogout };
            foreach (var btn in sideButtons)
            {
                if (btn == null) continue;
                btn.FlatStyle = FlatStyle.Flat;

                // السطر اللي تحت ده هو السر في إلغاء الـ Border
                btn.FlatAppearance.BorderSize = 0;

                btn.Cursor = Cursors.Hand;
                btn.TextAlign = ContentAlignment.MiddleRight;
            }
        }

        private void SetupCustomButtons()
        {
            // إنشاء زر "تصفح التبرعات" بدون أي Border
            Button btnBrowseOffers = new Button
            {
                Text = "   🍎  تصفح التبرعات",
                Size = new Size(btnUsers.Width, btnUsers.Height),
                Location = new Point(btnUsers.Location.X, btnUsers.Location.Y + btnUsers.Height + 10),
                BackColor = btnUsers.BackColor,
                ForeColor = btnUsers.ForeColor,
                FlatStyle = FlatStyle.Flat, // ضروري لإخفاء التأثيرات الكلاسيكية
                Font = btnUsers.Font,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleRight
            };

            // إلغاء الـ Border نهائياً
            btnBrowseOffers.FlatAppearance.BorderSize = 0;

            // ربط الزرار بميثود الضغط
            btnBrowseOffers.Click += (s, e) => {
                ShowControl(new UC_BrowseOffers());
            };

            // إضافته للـ Parent
            if (btnUsers.Parent != null)
            {
                btnUsers.Parent.Controls.Add(btnBrowseOffers);
                btnBrowseOffers.BringToFront();
            }
            else
            {
                this.Controls.Add(btnBrowseOffers);
                btnBrowseOffers.BringToFront();
            }

            // تحريك زر الخروج للأسفل
            if (btnLogout != null)
            {
                btnLogout.Location = new Point(btnLogout.Location.X, btnBrowseOffers.Location.Y + btnBrowseOffers.Height + 20);
            }
        }

        private void SetupRoleUI()
        {
            if (role == 2) // Admin
            {
                btnNeeds.Text = "   ✔️  توثيق الطلبات";
                lblUserRole.Text = "الصلاحية: مسؤول النظام";
            }
            else if (role == 0) // Charity
            {
                btnNeeds.Text = "   ➕  إنشاء احتياج";
                btnUsers.Text = "   🔍  احتياجاتي المرفوعة";
                lblUserRole.Text = "الصلاحية: جمعية خيرية";
            }
            else if (role == 1) // Donor
            {
                btnNeeds.Text = "   🎁  تصفح الاحتياجات";
                lblUserRole.Text = "الصلاحية: جهة متبرعة";
            }
            btnProfile.Text = "   👤  معلوماتي الشخصية";
        }

        private void btnBrowseNeeds_Click(object sender, EventArgs e)
        {
            ShowControl(new UC_MyNeeds());
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            ShowControl(new UC_Profile());
        }

        private void btnNeeds_GeneralClick(object sender, EventArgs e)
        {
            if (role == 2) ShowControl(new UC_VerifyUsers());
            else if (role == 0) ShowControl(new UC_AddNeed());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (GlobalUser.CurrentUser == null)
            {
                MessageBox.Show("يرجى تسجيل الدخول أولاً.");
                this.Close();
                return;
            }

            lblUserName.Text = "مرحباً: " + GlobalUser.CurrentUser.UserName;
            ShowControl(new UC_Profile());
        }

        private void ShowControl(UserControl control)
        {
            if (control == null || pnlContent == null) return;

            pnlContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(control);
            control.BringToFront();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Environment.Exit(0);
        }

        
    }
}