using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1.Config;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    public partial class MainForm : Form
    {
        private int role;
        private FlowLayoutPanel pnlSidebarFlow;
        private Panel pnlActiveIndicator;

        // ألوان الهوية الرقمية المعتمدة لـ Dashboard
        private readonly Color Color_SidebarBg = Color.FromArgb(33, 43, 96);
        private readonly Color Color_ButtonHover = Color.FromArgb(43, 54, 116);
        private readonly Color Color_ActiveBtn = Color.FromArgb(46, 204, 113);
        private readonly Color Color_ContentBg = Color.FromArgb(248, 249, 250);

        public MainForm()
        {
            InitializeComponent();
            ConfigureFormSpecs();
        }

        private void ConfigureFormSpecs()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color_ContentBg;

            if (GlobalUser.CurrentUser == null) return;
            role = GlobalUser.CurrentUser.Role;

            if (pnlContent != null) pnlContent.BackColor = Color_ContentBg;

            // 1. بناء وإعداد نظام القائمة الجانبية بالكامل
            SetupModernSidebar();

            // 2. تخصيص النصوص والأيقونات بناءً على الرتبة
            SetupRoleUI();

            // ربط الأحداث المباشرة للأزرار الافتراضية بشكل آمن يمنع التكرار
            if (btnNeeds != null) { btnNeeds.Click -= btnNeeds_GeneralClick; btnNeeds.Click += btnNeeds_GeneralClick; }
            if (btnProfile != null) { btnProfile.Click -= btnProfile_Click; btnProfile.Click += btnProfile_Click; }
            if (btnUsers != null) { btnUsers.Click -= btnBrowseNeeds_Click; btnUsers.Click += btnBrowseNeeds_Click; }

            this.Load += MainForm_Load;
        }

        private void SetupModernSidebar()
        {
            // تحديد الحاوية الجانبية المصممة بالـ Designer
            Control sidebarParent = btnNeeds?.Parent ?? this;
            sidebarParent.BackColor = Color_SidebarBg;

            if (lblUserName != null) { lblUserName.ForeColor = Color.White; lblUserName.Font = new Font("Segoe UI", 11F, FontStyle.Bold); }
            if (lblUserRole != null) { lblUserRole.ForeColor = Color.FromArgb(174, 182, 211); lblUserRole.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular); }

            // إنشاء الـ FlowLayoutPanel البرمجي وإعطائه مساحة كاملة مرنة من الأعلى للقاع
            pnlSidebarFlow = new FlowLayoutPanel
            {
                Location = new Point(0, 130), // يبدأ تحت لوحة اسم المستخدم مباشرة
                Size = new Size(sidebarParent.Width, sidebarParent.Height - 210), // ترك مساحة لزر تسجيل الخروج بالأسفل
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true, // تفعيل التمرير التلقائي الذكي لضمان ظهور كافة الأزرار في أي شاشة
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            // تكتيك لإخفاء شريط التمرير الأفقي المزعج وجعل الرأسي ناعماً
            pnlSidebarFlow.HorizontalScroll.Maximum = 0;
            pnlSidebarFlow.HorizontalScroll.Visible = false;

            // إنشاء خط المؤشر العمودي التفاعلي للزر النشط
            pnlActiveIndicator = new Panel
            {
                Size = new Size(6, 48),
                BackColor = Color_ActiveBtn,
                Visible = false
            };
            sidebarParent.Controls.Add(pnlActiveIndicator);

            // إضافة الـ FlowLayoutPanel أولاً في الواجهة الجانبية
            sidebarParent.Controls.Add(pnlSidebarFlow);

            // ----------------------------------------------------
            // 🔥 هنا يتم التحكم في الترتيب الصارم والأكيد للأزرار لتبدو مرئية بالكامل 🔥
            // ----------------------------------------------------
            // 5. عرض العروض والطلبات التي قدمت عليها (يظهر فقط للجمعية الخيرية)
            if (role == 0)
            {
                Button btnSentApps = new Button { Text = "   📤   طلبات التبرع المرسلة" };
                FormatSingleButton(btnSentApps);
                btnSentApps.Click += (s, e) => ShowControl(new UC_SentApplications());
                pnlSidebarFlow.Controls.Add(btnSentApps);
                btnSentApps.BringToFront();
            }

            // 4. تصفح عروض التبرعات المتاحة (زر ديناميكي مشترك لجميع الحسابات)
            Button btnBrowseOffers = new Button { Text = "   🍎   تصفح التبرعات المتاحة" };
            FormatSingleButton(btnBrowseOffers);
            btnBrowseOffers.Click += (s, e) => ShowControl(new UC_BrowseOffers());
            pnlSidebarFlow.Controls.Add(btnBrowseOffers);
            btnBrowseOffers.BringToFront();

            // 3. عرض الاحتياجات المرفوعة التي قمت بعملها (تظهر فقط للجمعية الخيرية)
            if (btnUsers != null)
            {
                FormatSingleButton(btnUsers);
                pnlSidebarFlow.Controls.Add(btnUsers);
                btnUsers.BringToFront();
            }

            // 2. إنشاء احتياج (أو توثيق الطلبات للأدمن / تصفح الاحتياجات للمتبرع)
            if (btnNeeds != null)
            {
                FormatSingleButton(btnNeeds);
                pnlSidebarFlow.Controls.Add(btnNeeds);
                btnNeeds.BringToFront();
            }
            // 1. حسابي الشخصي
            if (btnProfile != null)
            {
                FormatSingleButton(btnProfile);
                pnlSidebarFlow.Controls.Add(btnProfile);
                btnProfile.BringToFront();
            }

            // إعداد وتثبيت زر تسجيل الخروج في قاع الشاشة بشكل مستقل تماماً
            if (btnLogout != null)
            {
                FormatSingleButton(btnLogout);
                btnLogout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                btnLogout.Location = new Point(0, sidebarParent.Height - btnLogout.Height - 15);
                btnLogout.ForeColor = Color.FromArgb(254, 121, 104);
                sidebarParent.Controls.Add(btnLogout);
                btnLogout.BringToFront();
            }
        }

        private void FormatSingleButton(Button btn)
        {
            // تصفير الإعدادات الموروثة من الـ Designer لمنع تشوه الأبعاد والـ Location داخل الـ Flow
            btn.Anchor = AnchorStyles.None;
            btn.Dock = DockStyle.None;

            // إزالة كافة الهوامش والـ Padding الداخلية والخارجية تماماً لملء العرض المتاح
            btn.Margin = new Padding(0, 4, 0, 4);
            btn.Padding = new Padding(0);

            // جعل عرض الزر يتطابق بدقة مع العرض الداخلي الكامل للـ Sidebar المتاح حالياً بدون هوامش
            btn.Size = new Size(pnlSidebarFlow.ClientSize.Width, 48);

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = Color_ButtonHover;
            btn.FlatAppearance.MouseOverBackColor = Color_ButtonHover;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btn.ForeColor = Color.FromArgb(226, 232, 240);

            // ضبط الاتجاه والمحاذاة لجهة اليمين بالتوافق مع اللغة العربية
            btn.RightToLeft = RightToLeft.Yes;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.ImageAlign = ContentAlignment.MiddleLeft;

            btn.MouseEnter += (s, e) => { if (pnlActiveIndicator.Tag as Button != btn) btn.ForeColor = Color.White; };
            btn.MouseLeave += (s, e) => { if (pnlActiveIndicator.Tag as Button != btn) btn.ForeColor = Color.FromArgb(226, 232, 240); };
            btn.Click += (s, e) => HighlightActiveButton(btn);
        }

        private void HighlightActiveButton(Button targetButton)
        {
            if (targetButton == null || targetButton == btnLogout) return;

            // إرجاع اللون الطبيعي لكافة أزرار القائمة
            foreach (Control ctrl in pnlSidebarFlow.Controls)
            {
                if (ctrl is Button b) b.ForeColor = Color.FromArgb(226, 232, 240);
            }

            targetButton.ForeColor = Color_ActiveBtn;

            pnlActiveIndicator.Tag = targetButton;
            pnlActiveIndicator.Size = new Size(6, targetButton.Height);

            // حساب الإحداثيات العمودية بدقة متناهية ليقف المؤشر بجانب الزر المختار تماماً
            pnlActiveIndicator.Location = new Point(
                targetButton.Parent.Width - pnlActiveIndicator.Width,
                targetButton.Parent.Location.Y + targetButton.Location.Y - pnlSidebarFlow.VerticalScroll.Value
            );
            pnlActiveIndicator.Visible = true;
            pnlActiveIndicator.BringToFront();
        }

        private void SetupRoleUI()
        {
            if (role == 2) // Admin
            {
                if (btnNeeds != null) btnNeeds.Text = "   ✔️   توثيق طلبات النظام";
                if (btnUsers != null) btnUsers.Visible = false; // لا يملك احتياجات مرفوعة
                lblUserRole.Text = "الصلاحية: مسؤول النظام 🛡️";
            }
            else if (role == 0) // Charity
            {
                if (btnNeeds != null) btnNeeds.Text = "   ➕   إنشاء طلب احتياج";
                if (btnUsers != null) { btnUsers.Text = "   🔍   احتياجاتي المرفوعة"; btnUsers.Visible = true; }
                lblUserRole.Text = "الصلاحية: جمعية خيرية 🏢";
            }
            else if (role == 1) // Donor
            {
                if (btnNeeds != null) btnNeeds.Text = "   🎁   تصفح كافة الاحتياجات";
                if (btnUsers != null) btnUsers.Visible = false; // لا يملك احتياجات مرفوعة
                lblUserRole.Text = "الصلاحية: جهة متبرعة 🤝";
            }
            if (btnProfile != null) btnProfile.Text = "   👤   معلوماتي الشخصية";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (GlobalUser.CurrentUser == null)
            {
                MessageBox.Show("انتهت جلسة العمل، يرجى تسجيل الدخول أولاً.", "تنبيه حماية", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            lblUserName.Text = "مرحباً بك: " + GlobalUser.CurrentUser.UserName;

            // فتح واجهة الحساب الشخصي افتراضياً عند بداية الدخول
            if (btnProfile != null)
            {
                HighlightActiveButton(btnProfile);
                ShowControl(new UC_Profile());
            }
        }

        private void ShowControl(UserControl control)
        {
            if (control == null || pnlContent == null) return;

            pnlContent.SuspendLayout();
            pnlContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(control);
            control.BringToFront();
            pnlContent.ResumeLayout();
        }

        private void btnBrowseNeeds_Click(object sender, EventArgs e) => ShowControl(new UC_MyNeeds());
        private void btnProfile_Click(object sender, EventArgs e) => ShowControl(new UC_Profile());

        private void btnNeeds_GeneralClick(object sender, EventArgs e)
        {
            if (role == 2) ShowControl(new UC_VerifyUsers());
            else if (role == 0) ShowControl(new UC_AddNeed());
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("هل أنت متأكد من رغبتك في إغلاق التطبيق", "تأكيد الخروج",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
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