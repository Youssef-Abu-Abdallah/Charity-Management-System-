using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1.Services;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Forms
{ 
    public partial class UC_VerifyUsers : UserControl
    {
        private DataGridView dgvPendingUsers;
        private Button btnVerify;
        private Button btnRefresh;
        private Label lblTitle;
        private Panel pnlHeader;
        private readonly AdminService _adminService;

        public UC_VerifyUsers()
        {
            // InitializeComponent(); // لو السطر ده موجود عطلة (أو سيبه لو عايز تستخدم الـ Designer)
            _adminService = new AdminService();
            InitializeCustomComponents(); // ده اللي فيه الكود بتاعنا
        }

        private TabControl tabUsers;
        private TabPage tabCharities;
        private TabPage tabDonors;
        private DataGridView dgvPendingDonors; // جدول جديد للمتبرعين

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(30, 30, 30);

            // 1. الرأس (Header)
            lblTitle = new Label { Text = "توثيق الحسابات المعلقة", ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(45, 45, 48) };
            btnRefresh = new Button { Text = "تحديث الكل 🔄", Size = new Size(130, 40), Location = new Point(800, 20), Anchor = AnchorStyles.Top | AnchorStyles.Right, BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefresh.Click += async (s, e) => await LoadPendingUsers();
            pnlHeader.Controls.Add(lblTitle); pnlHeader.Controls.Add(btnRefresh);

            // 2. نظام التبويبات (Tabs)
            tabUsers = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            tabCharities = new TabPage { Text = "الجمعيات الخيرية", BackColor = Color.White };
            tabDonors = new TabPage { Text = "المؤسسات المانحة", BackColor = Color.White };

            // إعداد جدول الجمعيات (القديم)
            dgvPendingUsers = CreateStyledGrid();
            tabCharities.Controls.Add(dgvPendingUsers);

            // إعداد جدول المتبرعين (الجديد)
            dgvPendingDonors = CreateStyledGrid();
            tabDonors.Controls.Add(dgvPendingDonors);

            tabUsers.TabPages.Add(tabCharities);
            tabUsers.TabPages.Add(tabDonors);

            // 3. التذييل (Footer)
            Panel pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.FromArgb(45, 45, 48) };
            btnVerify = new Button { Text = "توثيق الحساب المختار ✔️", Size = new Size(250, 45), Location = new Point(20, 12), BackColor = Color.ForestGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnVerify.Click += async (s, e) => await VerifySelectedUser();
            pnlFooter.Controls.Add(btnVerify);

            this.Controls.Add(tabUsers);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlFooter);

            this.Load += async (s, e) => await LoadPendingUsers();
        }

        // ميثود مساعدة لتوحيد شكل الجداول
        private DataGridView CreateStyledGrid()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            return dgv;
        }






        private async System.Threading.Tasks.Task LoadPendingUsers()
        {
            try
            {
                btnRefresh.Enabled = false;
                var result = await _adminService.GetPendingVerificationsAsync();

                if (result != null && result.success && result.data != null)
                {
                    // ربط الجمعيات
                    dgvPendingUsers.DataSource = result.data.pendingCharities;
                    FormatGrid(dgvPendingUsers, "charityName", "اسم الجمعية");

                    // ربط المتبرعين
                    dgvPendingDonors.DataSource = result.data.pendingDonors;
                    FormatGrid(dgvPendingDonors, "donorOrganizationName", "اسم المؤسسة");
                }
            }
            finally { btnRefresh.Enabled = true; }
        }

        private void FormatGrid(DataGridView dgv, string nameColumn, string headerText)
        {
            if (dgv.Columns["userId"] != null) dgv.Columns["userId"].Visible = false;
            if (dgv.Columns[nameColumn] != null) dgv.Columns[nameColumn].HeaderText = headerText;
            if (dgv.Columns["email"] != null) dgv.Columns["email"].HeaderText = "البريد الإلكتروني";
        }

        private async System.Threading.Tasks.Task VerifySelectedUser()
        {
            // 1. تحديد الجدول النشط بناءً على التبويب المختار حالياً
            DataGridView activeGrid = (tabUsers.SelectedTab == tabCharities) ? dgvPendingUsers : dgvPendingDonors;

            // 2. التحقق من أن المستخدم اختار صفاً من الجدول
            if (activeGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("يرجى اختيار حساب من القائمة أولاً لتوثيقه.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 3. استخراج المعرف الفريد (userId) من الصف المختار
                if (activeGrid.SelectedRows[0].Cells["userId"].Value == null) return;

                Guid userId = (Guid)activeGrid.SelectedRows[0].Cells["userId"].Value;
                string accountName = activeGrid.SelectedRows[0].Cells[tabUsers.SelectedTab == tabCharities ? "charityName" : "donorOrganizationName"].Value?.ToString();

                // 4. طلب تأكيد من الآدمن قبل الإرسال للسيرفر
                var confirm = MessageBox.Show($"هل أنت متأكد من توثيق حساب: ({accountName})؟", "تأكيد التوثيق", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    // تعطيل الزر مؤقتاً لمنع النقرات المتكررة
                    btnVerify.Enabled = false;

                    // 5. استدعاء الخدمة لإرسال طلب التوثيق للـ API
                    bool success = await _adminService.VerifyUserAsync(userId);

                    if (success)
                    {
                        MessageBox.Show("تم توثيق الحساب وتفعيله بنجاح!", "عملية ناجحة", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 6. تحديث القائمة فوراً لإزالة الحساب الذي تم توثيقه
                        await LoadPendingUsers();
                    }
                    else
                    {
                        MessageBox.Show("فشل توثيق الحساب. قد يكون التوكن قد انتهى أو هناك مشكلة في السيرفر.", " خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ غير متوقع: {ex.Message}", "خطأ تقني", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // إعادة تفعيل الزر في كل الأحوال
                btnVerify.Enabled = true;
            }
        }
    }
}