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

            // 3. إضافة الأزرار الجديدة برمجياً (تصفح التبرعات + طلباتي المرسلة)
            SetupCustomButtons();

            // ربط الأحداث للأزرار الأساسية
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

                // إلغاء الـ Border
                btn.FlatAppearance.BorderSize = 0;

                btn.Cursor = Cursors.Hand;
                btn.TextAlign = ContentAlignment.MiddleRight;
            }
        }

        private void SetupCustomButtons()
        {
            // --- 1. إنشاء زر "تصفح التبرعات" ---
            Button btnBrowseOffers = new Button
            {
                Text = "   🍎  تصفح التبرعات",
                Size = new Size(btnUsers.Width, btnUsers.Height),
                Location = new Point(btnUsers.Location.X, btnUsers.Location.Y + btnUsers.Height + 10),
                BackColor = btnUsers.BackColor,
                ForeColor = btnUsers.ForeColor,
                FlatStyle = FlatStyle.Flat,
                Font = btnUsers.Font,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleRight
            };
            btnBrowseOffers.FlatAppearance.BorderSize = 0;

            // ربط الضغط بفتح صفحة تصفح التبرعات
            btnBrowseOffers.Click += (s, e) =>
            {
                ShowControl(new UC_BrowseOffers());
            };

            // إضافته للـ Parent
            if (btnUsers.Parent != null) btnUsers.Parent.Controls.Add(btnBrowseOffers);
            else this.Controls.Add(btnBrowseOffers);

            btnBrowseOffers.BringToFront();

            // --- 2. إنشاء زر "طلباتي المرسلة" (يظهر فقط إذا كان المستخدم جمعية خيرية) ---
            if (role == 0) // Charity
            {
                Button btnSentApps = new Button
                {
                    Text = "   📤  طلباتي المرسلة",
                    Size = new Size(btnBrowseOffers.Width, btnBrowseOffers.Height),
                    Location = new Point(btnBrowseOffers.Location.X, btnBrowseOffers.Location.Y + btnBrowseOffers.Height + 10),
                    BackColor = btnBrowseOffers.BackColor,
                    ForeColor = btnBrowseOffers.ForeColor,
                    FlatStyle = FlatStyle.Flat,
                    Font = btnBrowseOffers.Font,
                    Cursor = Cursors.Hand,
                    TextAlign = ContentAlignment.MiddleRight
                };
                btnSentApps.FlatAppearance.BorderSize = 0;

                // الربط المباشر
                btnSentApps.Click += (s, e) =>
                {
                    ShowControl(new UC_SentApplications());
                };

                if (btnBrowseOffers.Parent != null) btnBrowseOffers.Parent.Controls.Add(btnSentApps);
                btnSentApps.BringToFront();
            }
            else
            {
                // إذا لم يكن جمعية، اجعل زر الخروج أسفل زر "تصفح التبرعات"
                if (btnLogout != null)
                {
                    btnLogout.Location = new Point(btnLogout.Location.X, btnBrowseOffers.Location.Y + btnBrowseOffers.Height + 20);
                }
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
            if (MessageBox.Show("هل انت متأكد من الخروج من التطبيق", "تنبيه", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Application.Restart();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Environment.Exit(0);
        }
    }
}