using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
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
            LoadMyNeeds(); // تحميل البيانات عند فتح الصفحة
        }

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.RightToLeft = RightToLeft.Yes;

            Label lblTitle = new Label
            {
                Text = "📋 قائمة احتياجاتي المرفوعة",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(45, 45, 45)
            };

            lblLoading = new Label
            {
                Text = "جاري تحميل البيانات...",
                Location = new Point(20, 55),
                AutoSize = true,
                Visible = false,
                ForeColor = Color.Blue
            };

            dgvMyNeeds = new DataGridView
            {
                Location = new Point(20, 80),
                Size = new Size(1500,1000),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblLoading);
            this.Controls.Add(dgvMyNeeds);
        }

        private async void LoadMyNeeds()
        {
            try
            {
                lblLoading.Visible = true;
                // استدعاء الميثود التي تجلب احتياجات الجمعية فقط (المسار: charity/charity-needs)
                var needsList = await _charityService.GetAllNeedsAsync();

                if (needsList != null)
                {
                    dgvMyNeeds.DataSource = needsList;
                    // تحسين شكل الأعمدة
                    FormatGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل احتياجاتك: " + ex.Message);
            }
            finally
            {
                lblLoading.Visible = false;
            }
        }

        private void FormatGrid()
        {
            if (dgvMyNeeds.Columns.Count == 0) return;

            void HideIfExist(string colName)
            {
                if (dgvMyNeeds.Columns.Contains(colName)) dgvMyNeeds.Columns[colName].Visible = false;
            }
            void SetHeaderIfExist(string colName, string headerText)
            {
                if (dgvMyNeeds.Columns.Contains(colName)) dgvMyNeeds.Columns[colName].HeaderText = headerText;
            }

            // إخفاء أعمدة الأرقام والبيانات التقنية
            HideIfExist("id");
            HideIfExist("category");
            HideIfExist("unit");
            HideIfExist("priority");
            HideIfExist("status");
            HideIfExist("imageUrl");
            HideIfExist("description");

            // إظهار أعمدة النصوص العربية وتسميتها
            SetHeaderIfExist("productName", "اسم المنتج");
            SetHeaderIfExist("quantity", "الكمية");
            SetHeaderIfExist("الوحدة", "الوحدة"); // أضفنا ده
            SetHeaderIfExist("حالة_الطلب", "الحالة");
            SetHeaderIfExist("التصنيف", "التصنيف");
            SetHeaderIfExist("الأولوية", "الأولوية");

            // تلوين الصفوف بناءً على الحالة (إضافة اختيارية لمسة جمالية)
            dgvMyNeeds.CellFormatting += (s, e) => {
                if (dgvMyNeeds.Columns[e.ColumnIndex].Name == "حالة_الطلب" && e.Value != null)
                {
                    if (e.Value.ToString() == "مقبول") e.CellStyle.ForeColor = Color.Green;
                    else if (e.Value.ToString() == "مرفوض") e.CellStyle.ForeColor = Color.Red;
                    else if (e.Value.ToString() == "قيد الانتظار") e.CellStyle.ForeColor = Color.Orange;
                }
            };
        }

    }
}