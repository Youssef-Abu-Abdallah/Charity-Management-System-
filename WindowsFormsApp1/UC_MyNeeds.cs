using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    public partial class UC_MyNeeds : UserControl
    {
        private DataGridView dgvMyNeeds;
        private CharityService _charityService;
        private Label lblLoading;

        public UC_MyNeeds()
        {
            _charityService = new CharityService();
            InitializeCustomComponents();

            // بدء تحميل البيانات فور فتح الكنترول
            _ = LoadMyNeeds();
        }

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.RightToLeft = RightToLeft.Yes;

            // العنوان
            Label lblTitle = new Label
            {
                Text = "📋 قائمة احتياجاتي المرفوعة",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(45, 45, 48)
            };

            // رسالة جاري التحميل
            lblLoading = new Label
            {
                Text = "جاري جلب البيانات من السيرفر...",
                Font = new Font("Segoe UI", 10),
                Location = new Point(25, 60),
                AutoSize = true,
                Visible = false,
                ForeColor = Color.DarkBlue
            };

            // إعداد الجدول
            dgvMyNeeds = new DataGridView
            {
                Location = new Point(20, 90),
                Size = new Size(850, 480),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                GridColor = Color.LightGray,
                RowTemplate = { Height = 35 } // ارتفاع مناسب للنصوص
            };

            // تحسين شكل الهيدر
            dgvMyNeeds.EnableHeadersVisualStyles = false;
            dgvMyNeeds.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvMyNeeds.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvMyNeeds.ColumnHeadersHeight = 40;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblLoading);
            this.Controls.Add(dgvMyNeeds);
        }

        private async Task LoadMyNeeds()
        {
            try
            {
                lblLoading.Visible = true;

                // جلب البيانات من السيرفر باستخدام الميثود المحدثة في السيرفيس
                var needsList = await _charityService.GetAllNeedsAsync();

                if (needsList != null)
                {
                    dgvMyNeeds.DataSource = needsList;
                    FormatGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل البيانات: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lblLoading.Visible = false;
            }
        }

        private void FormatGrid()
        {
            if (dgvMyNeeds.Columns.Count == 0) return;

            // 1. إخفاء الأعمدة التي لا نريد عرضها (البيانات الخام والروابط)
            string[] toHide = { "id", "category", "unit", "priority", "status", "imageUrl", "description" };
            foreach (var colName in toHide)
            {
                if (dgvMyNeeds.Columns.Contains(colName))
                    dgvMyNeeds.Columns[colName].Visible = false;
            }

            // 2. تسمية الأعمدة العربية (التي برمجناها في الـ DTO)
            SetHeader("productName", "اسم المنتج");
            SetHeader("quantity", "الكمية");
            SetHeader("الوحدة", "الوحدة");
            SetHeader("حالة_الطلب", "حالة الطلب");
            SetHeader("التصنيف", "التصنيف");
            SetHeader("الأولوية", "الأولوية");

            // 3. تنسيق إضافي لعمود الحالة (اختياري: تلوين النص حسب الحالة)
            dgvMyNeeds.CellFormatting += (s, e) =>
            {
                if (dgvMyNeeds.Columns[e.ColumnIndex].Name == "حالة_الطلب" && e.Value != null)
                {
                    string val = e.Value.ToString();
                    if (val == "مقبول") e.CellStyle.ForeColor = Color.Green;
                    else if (val == "مرفوض") e.CellStyle.ForeColor = Color.Red;
                    else if (val == "قيد الانتظار") e.CellStyle.ForeColor = Color.Gold;
                }
            };
        }

        private void SetHeader(string colName, string headerText)
        {
            if (dgvMyNeeds.Columns.Contains(colName))
            {
                dgvMyNeeds.Columns[colName].HeaderText = headerText;
            }
        }
    }
}