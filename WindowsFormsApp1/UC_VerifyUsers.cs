using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using WindowsFormsApp1.Services;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Forms
{
    public partial class UC_VerifyUsers : UserControl
    {
        private DataGridView dgvPendingUsers;
        private DataGridView dgvPendingDonors;
        private Button btnVerify;
        private Button btnRefresh;
        private Label lblTitle;
        private Panel pnlHeader;
        private TabControl tabUsers;
        private TabPage tabCharities;
        private TabPage tabDonors;

        private readonly AdminService _adminService;

        public UC_VerifyUsers()
        {
            _adminService = new AdminService();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // إعدادات الـ UserControl الرئيسية (Modern Light Theme)
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(240, 242, 245); // خلفية رمادية ناعمة ومريحة للعين (مثل فيسبوك)
            this.RightToLeft = RightToLeft.Yes;          // دعم كامل للغة العربية
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // 1. لوحة الرأس (Header Panel)
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = Color.White // هيدر أبيض نظيف ومميز عن الخلفية
            };

            lblTitle = new Label
            {
                Text = "🛡️ توثيق الحسابات المعلقة", // تم التعديل حسب طلبك
                ForeColor = Color.FromArgb(43, 54, 116), // أزرق نيلي فخم ومتناسق مع باقي الواجهات
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 22)
            };
            lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnRefresh = new Button
            {
                Text = "تحديث البيانات 🔄",
                Size = new Size(150, 38),
                Location = new Point(20, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.FromArgb(52, 152, 219), // أزرق مودرن مريح
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => await LoadPendingUsers();

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnRefresh);

            // 2. نظام التبويبات المطور (Tabs)
            tabUsers = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                SizeMode = TabSizeMode.Fixed,
                ItemSize = new Size(180, 40)
            };

            // التبويبات تأخذ خلفية بيضاء لتظهر كبطاقة فوق الخلفية الرمادية الأساسية
            tabCharities = new TabPage { Text = "🏢 الجمعيات الخيرية", BackColor = Color.White };
            tabDonors = new TabPage { Text = "🤝 المؤسسات المانحة", BackColor = Color.White };

            // إعداد جدول الجمعيات
            dgvPendingUsers = CreateStyledGrid();
            ConfigureGridColumns(dgvPendingUsers, "charityName", "اسم الجمعية الخيرية");
            tabCharities.Controls.Add(dgvPendingUsers);

            // إعداد جدول المتبرعين
            dgvPendingDonors = CreateStyledGrid();
            ConfigureGridColumns(dgvPendingDonors, "donorOrganizationName", "اسم المؤسسة المانحة");
            tabDonors.Controls.Add(dgvPendingDonors);

            tabUsers.TabPages.Add(tabCharities);
            tabUsers.TabPages.Add(tabDonors);

            // 3. لوحة التحكم السفلية (Footer Panel)
            Panel pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 75,
                BackColor = Color.White
            };

            btnVerify = new Button
            {
                Text = "✔️ توثيق الحساب المختار وتفعيله فوراً",
                Size = new Size(300, 45),
                Location = new Point(20, 15),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                BackColor = Color.FromArgb(46, 204, 113), // أخضر احترافي ومبهج للعمليات الناجحة
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVerify.FlatAppearance.BorderSize = 0;
            btnVerify.Click += async (s, e) => await VerifySelectedUser();
            pnlFooter.Controls.Add(btnVerify);

            // إضافة العناصر بالترتيب الصحيح
            this.Controls.Add(tabUsers);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlFooter);

            // تحميل البيانات عند أول ظهور للكونترول
            this.Load += async (s, e) => await LoadPendingUsers();
        }

        // ميثود مساعدة لتصميم الجداول بشكل فخم ومناسب للمظهر الفاتح (Light Web/Dashboard Style)
        private DataGridView CreateStyledGrid()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                ForeColor = Color.FromArgb(45, 55, 72), // لون نصوص رمادي داكن أنيق جداً بدلاً من الأسود الحاد
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(230, 235, 240), // خطوط شبكية خفيفة جداً ومودرن

                RowTemplate = { Height = 42 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,

                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 45,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                RowHeadersVisible = false
            };

            // تصميم هيدر الجدول (رأس الأعمدة الفاتح)
            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 250), // رمادي فاتح جداً هادئ ورائع للهيدر
                ForeColor = Color.FromArgb(43, 54, 116),    // أزرق نيلي متناسق مع العناوين الرئيسية
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                SelectionBackColor = Color.FromArgb(248, 249, 250)
            };

            // تصميم خلايا البيانات والصفوف
            dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(45, 55, 72),
                Font = new Font("Segoe UI", 10F),
                SelectionBackColor = Color.FromArgb(235, 243, 250), // لون تحديد لبني هادئ جداً ومريح للعين
                SelectionForeColor = Color.FromArgb(0, 102, 204),   // نص أزرق مميز عند الاختيار
                Padding = new Padding(10, 0, 10, 0)
            };

            // تفعيل الـ Double Buffering لمنع الرعشة والبطء أثناء التمرير في الجدول
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, dgv, new object[] { true });

            return dgv;
        }

        private void ConfigureGridColumns(DataGridView dgv, string nameDataProperty, string nameHeader)
        {
            dgv.Columns.Clear();

            // عمود الـ ID (مخفي)
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "userId", DataPropertyName = "userId", Visible = false });

            // عمود الاسم المخصص
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = nameDataProperty,
                DataPropertyName = nameDataProperty,
                HeaderText = nameHeader,
                AutoSizeMode = (DataGridViewAutoSizeColumnMode)DataGridViewAutoSizeColumnsMode.Fill
            });

            // عمود الإيميل
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "email",
                DataPropertyName = "email",
                HeaderText = "البريد الإلكتروني",
                AutoSizeMode = (DataGridViewAutoSizeColumnMode)DataGridViewAutoSizeColumnsMode.Fill
            });
        }

        private async Task LoadPendingUsers()
        {
            try
            {
                btnRefresh.Enabled = false;
                btnRefresh.Text = "⏳ جاري التحديث...";

                var result = await _adminService.GetPendingVerificationsAsync();

                if (result != null && result.success && result.data != null)
                {
                    dgvPendingUsers.DataSource = null;
                    dgvPendingUsers.DataSource = result.data.pendingCharities;

                    dgvPendingDonors.DataSource = null;
                    dgvPendingDonors.DataSource = result.data.pendingDonors;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"فشل في جلب البيانات المحدثة: {ex.Message}", "تنبيه النظام", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnRefresh.Enabled = true;
                btnRefresh.Text = "تحديث البيانات 🔄";
            }
        }

        private async Task VerifySelectedUser()
        {
            DataGridView activeGrid = (tabUsers.SelectedTab == tabCharities) ? dgvPendingUsers : dgvPendingDonors;

            if (activeGrid.SelectedRows.Count == 0 || activeGrid.CurrentRow == null)
            {
                MessageBox.Show("يرجى تحديد حساب من الجدول أولاً للقيام بتوثيقه.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var currentRow = activeGrid.SelectedRows[0];
                if (currentRow.Cells["userId"].Value == null) return;

                Guid userId = (Guid)currentRow.Cells["userId"].Value;
                string targetColumnKey = (tabUsers.SelectedTab == tabCharities) ? "charityName" : "donorOrganizationName";
                string accountName = currentRow.Cells[targetColumnKey].Value?.ToString() ?? "حساب غير مسمى";

                var confirm = MessageBox.Show($"هل أنت متأكد من منح شهادة التوثيق وتفعيل حساب:\n👉 {accountName}؟",
                    "تأكيد طلب التوثيق الرسمي", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    btnVerify.Enabled = false;
                    btnVerify.Text = "⏳ جاري التوثيق...";

                    bool success = await _adminService.VerifyUserAsync(userId);

                    if (success)
                    {
                        MessageBox.Show("✅ تم توثيق وتفعيل الحساب بنجاح وإرسال إشعار للمستخدم!", "عملية ناجحة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadPendingUsers();
                    }
                    else
                    {
                        MessageBox.Show("فشل توثيق الحساب. يرجى التحقق من صلاحيات النظام أو اتصال الإنترنت.", "خطأ في العملية", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ غير متوقع أثناء معالجة التوثيق: {ex.Message}", "خطأ تقني داخلي", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnVerify.Enabled = true;
                btnVerify.Text = "✔️ توثيق الحساب المختار وتفعيله فوراً";
            }
        }
    }
}