using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    public partial class UC_BrowseOffers : UserControl
    {
        private DataGridView dgvOffers;
        private CharityService _charityService;
        private Label lblTitle;

        public UC_BrowseOffers()
        {
            _charityService = new CharityService();
            InitializeCustomComponents();

            // تحميل البيانات عند بدء التشغيل
            this.Load += async (s, e) => await LoadOffers();
        }

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(248, 249, 250); // خلفية رمادية ناعمة وموحدة
            this.RightToLeft = RightToLeft.Yes;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // عنوان الصفحة
            lblTitle = new Label
            {
                Text = "🍎 التبرعات المتاحة من المؤسسات الشريكة",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(43, 54, 116), // أزرق نيلي احترافي
                Location = new Point(20, 20),
                AutoSize = true
            };

            // ضبط محاذاة العنوان ديناميكياً مع اليمين
            this.SizeChanged += (s, e) => {
                lblTitle.Location = new Point(this.Width - lblTitle.Width - 25, 20);
            };

            // إعداد الجدول بتصميم Flat حديث
            dgvOffers = new DataGridView
            {
                Location = new Point(25, 75),
                Size = new Size(this.Width - 50, this.Height - 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                GridColor = Color.FromArgb(235, 238, 242), // لون شبكة ناعم
                EnableHeadersVisualStyles = false,
                RowTemplate = { Height = 45 } // ارتفاع السطر
            };

            // تنسيق رأس الجدول (Header)
            dgvOffers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(43, 54, 116);
            dgvOffers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvOffers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            dgvOffers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvOffers.ColumnHeadersHeight = 48;

            // تنسيق الخلايا العادية
            dgvOffers.DefaultCellStyle.BackColor = Color.White;
            dgvOffers.DefaultCellStyle.ForeColor = Color.FromArgb(45, 55, 72);
            dgvOffers.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvOffers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 244, 253);
            dgvOffers.DefaultCellStyle.SelectionForeColor = Color.FromArgb(43, 54, 116);
            dgvOffers.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // أسطر متبادلة الألوان لراحة أكبر للعين
            dgvOffers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 253, 254);

            // ربط الأحداث (تم إضافة CellFormatting للتأكيد القاطع على رسم الزر)
            dgvOffers.CellContentClick += dgvOffers_CellContentClick;
            dgvOffers.DataBindingComplete += dgvOffers_DataBindingComplete;
            dgvOffers.CellFormatting += dgvOffers_CellFormatting;

            this.Controls.Add(lblTitle);
            this.Controls.Add(dgvOffers);
        }

        private async Task LoadOffers()
        {
            try
            {
                var offers = await _charityService.GetAllOffersAsync();

                if (offers != null)
                {
                    var displayList = offers.Select(o => new {
                        o.OfferId,
                        o.ProductName,
                        o.DonorOrganizationName,
                        o.Quantity,
                        UnitName = GetUnitName(o.Unit),
                        CategoryName = GetCategoryName(o.Category),
                        StatusName = GetStatusName(o.Status)
                    }).ToList();

                    dgvOffers.Invoke((MethodInvoker)delegate {
                        // تنظيف الجدول تماماً لمنع أي تداخل للأعمدة القديمة
                        dgvOffers.DataSource = null;
                        dgvOffers.Columns.Clear();

                        // 1. ربط البيانات أولاً
                        dgvOffers.DataSource = displayList;

                        // 2. إضافة عمود الأزرار ثانياً ليركب فوق البيانات بشكل مستقر
                        AddApplyButton();

                        // 3. عمل التنسيقات النهائية للمسميات والأعمدة
                        FormatGrid();
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ غير متوقع أثناء تحميل البيانات: {ex.Message}", "خطأ نظام", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatGrid()
        {
            // إخفاء المعرف الخاص بالداتابيز بشكل آمن
            if (dgvOffers.Columns.Contains("OfferId")) dgvOffers.Columns["OfferId"].Visible = false;

            // مسميات رؤوس الأعمدة
            if (dgvOffers.Columns.Contains("ProductName")) dgvOffers.Columns["ProductName"].HeaderText = "اسم المنتج المتاح";
            if (dgvOffers.Columns.Contains("DonorOrganizationName")) dgvOffers.Columns["DonorOrganizationName"].HeaderText = "الجهة المتبرعة";
            if (dgvOffers.Columns.Contains("Quantity")) dgvOffers.Columns["Quantity"].HeaderText = "الكمية المتاحة";
            if (dgvOffers.Columns.Contains("UnitName")) dgvOffers.Columns["UnitName"].HeaderText = "الوحدة الكلية";
            if (dgvOffers.Columns.Contains("CategoryName")) dgvOffers.Columns["CategoryName"].HeaderText = "التصنيف الرئيسي";
            if (dgvOffers.Columns.Contains("StatusName")) dgvOffers.Columns["StatusName"].HeaderText = "حالة التبرع الحالي";

            // ترتيب ظهور عمود الإجراء "تقديم الطلب الآن" في أقصى اليسار
            if (dgvOffers.Columns.Contains("ApplyButton"))
            {
                dgvOffers.Columns["ApplyButton"].HeaderText = "العمليات المتاحة";
                dgvOffers.Columns["ApplyButton"].DisplayIndex = dgvOffers.Columns.Count - 1;
            }

            // حماية البيانات من التعديل اليدوي ما عدا عمود الأزرار
            foreach (DataGridViewColumn col in dgvOffers.Columns)
            {
                if (col.Name != "ApplyButton") col.ReadOnly = true;
            }
        }

        private void AddApplyButton()
        {
            if (dgvOffers.Columns["ApplyButton"] == null)
            {
                DataGridViewButtonColumn applyButton = new DataGridViewButtonColumn();
                applyButton.Name = "ApplyButton";
                applyButton.Text = "تقديم الطلب الآن";
                applyButton.UseColumnTextForButtonValue = true; // تعيين النص الافتراضي للعمود
                applyButton.FlatStyle = FlatStyle.Flat;

                // تنسيق التصميم العام للزر داخل العمود
                applyButton.DefaultCellStyle.BackColor = Color.FromArgb(46, 204, 113); // أخضر حيوي
                applyButton.DefaultCellStyle.ForeColor = Color.White;
                applyButton.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                applyButton.DefaultCellStyle.SelectionBackColor = Color.FromArgb(39, 174, 96);
                applyButton.DefaultCellStyle.SelectionForeColor = Color.White;

                dgvOffers.Columns.Add(applyButton);
            }
        }

        // إجبار التلوين والنص على الظهور في كل الخلايا بلا استثناء أثناء تنسيقها برمجياً
        private void dgvOffers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // التحقق من أننا نقوم بتهيئة عمود الأزرار حالياً
            if (e.RowIndex >= 0 && dgvOffers.Columns[e.ColumnIndex].Name == "ApplyButton")
            {
                // إجبار وضع النص داخل الخلية للسطر الحالي حتى لو اختفى بسبب ألوان الأسطر البديلة
                e.Value = "تقديم الطلب الآن";

                // إعادة تثبيت الألوان بشكل صارم للخلية الحالية
                e.CellStyle.BackColor = Color.FromArgb(46, 204, 113);
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.SelectionBackColor = Color.FromArgb(39, 174, 96);
                e.CellStyle.SelectionForeColor = Color.White;
            }
        }

        // تلوين نصوص حالات التبرع تلقائياً بناءً على القيمة (مقبول، مرفوض، إلخ)
        private void dgvOffers_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvOffers.Rows)
            {
                if (dgvOffers.Columns.Contains("StatusName"))
                {
                    var statusCell = row.Cells["StatusName"];
                    if (statusCell.Value != null)
                    {
                        string status = statusCell.Value.ToString();

                        if (status == "مقبول" || status == "تم التنفيذ")
                        {
                            statusCell.Style.ForeColor = Color.FromArgb(39, 174, 96);
                            statusCell.Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                        }
                        else if (status == "قيد الانتظار")
                        {
                            statusCell.Style.ForeColor = Color.FromArgb(230, 126, 34);
                            statusCell.Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                        }
                        else if (status == "مرفوض")
                        {
                            statusCell.Style.ForeColor = Color.FromArgb(192, 57, 43);
                            statusCell.Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                        }
                    }
                }
            }
        }

        private async void dgvOffers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // التأكد أن الضغط تم على عمود الزر المخصص بشكل صحيح
            if (e.RowIndex >= 0 && dgvOffers.Columns[e.ColumnIndex].Name == "ApplyButton")
            {
                var offerIdValue = dgvOffers.Rows[e.RowIndex].Cells["OfferId"].Value;
                if (offerIdValue == null) return;

                string offerId = offerIdValue.ToString();

                var confirm = MessageBox.Show("هل أنت متأكد من الرغبة في تقديم طلب الاستفادة من هذا التبرع؟",
                                            "تأكيد إرسال الطلب", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        dgvOffers.Enabled = false;

                        bool success = await _charityService.ApplyForOfferAsync(offerId);

                        if (success)
                        {
                            MessageBox.Show("تم إرسال طلبك بنجاح وهو الآن قيد المراجعة!", "تمت العملية", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await LoadOffers(); // إعادة تحميل وتحديث الجدول بالكامل تلقائياً
                        }
                        else
                        {
                            MessageBox.Show("فشل إرسال الطلب، يرجى مراجعة اتصال الشبكة والمحاولة لاحقاً او التأكد من انك لم تقم بالتقديم من قبل.", "خطأ في الإرسال", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"حدث خطأ أثناء معالجة الطلب: {ex.Message}", "خطأ في النظام", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        dgvOffers.Enabled = true;
                    }
                }
            }
        }

        // --- دوال التحويل المساعدة المتوافقة مع مشروعك بالكامل ---
        private string GetUnitName(int unit)
        {
            switch (unit)
            {
                case 0: return "طن";
                case 1: return "كيلو جرام";
                case 2: return "جرام";
                case 3: return "لتر";
                case 4: return "ملي لتر";
                case 5: return "عبوة";
                case 6: return "صندوق";
                case 7: return "علبة";
                case 8: return "قطعة";
                default: return "وحدة";
            }
        }

        private string GetStatusName(int status)
        {
            switch (status)
            {
                case 0: return "قيد الانتظار";
                case 1: return "مقبول";
                case 2: return "مرفوض";
                case 3: return "تم التنفيذ";
                default: return "غير محدد";
            }
        }

        private string GetCategoryName(int category)
        {
            switch (category)
            {
                case 0: return "طعام غذائي";
                case 1: return "ملابس وكساء";
                case 2: return "مستلزمات طبية";
                case 3: return "أدوات تعليمية";
                default: return "أقسام أخرى";
            }
        }
    }
}