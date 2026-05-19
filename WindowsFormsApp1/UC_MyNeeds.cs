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
            _ = LoadMyNeeds();
        }

        private void InitializeCustomComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.RightToLeft = RightToLeft.Yes;

            Label lblTitle = new Label
            {
                Text = "📋 قائمة احتياجاتي المرفوعة",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(1020, 20),
                AutoSize = true
            };

            lblLoading = new Label
            {
                Text = "جاري جلب البيانات...",
                Location = new Point(625, 60),
                AutoSize = true,
                Visible = false
            };

            dgvMyNeeds = new DataGridView
            {
                Location = new Point(680, 90),
                Size = new Size(850, 480), // رجعنا الحجم كبير لأن مفيش أزرار
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowTemplate = { Height = 35 },
                RightToLeft = RightToLeft.Yes
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblLoading);
            this.Controls.Add(dgvMyNeeds);
        }

        private async Task LoadMyNeeds()
        {
            try
            {
                lblLoading.Visible = true;
                var needsList = await _charityService.GetAllNeedsAsync();
                if (needsList != null)
                {
                    dgvMyNeeds.DataSource = needsList;
                    FormatGrid();
                }
            }
            catch (Exception ex) { MessageBox.Show("خطأ: " + ex.Message); }
            finally { lblLoading.Visible = false; }
        }

        private void FormatGrid()
        {
            if (dgvMyNeeds.Columns.Count == 0) return;
            string[] toHide = { "id", "category", "unit", "priority", "status", "imageUrl", "description" };
            foreach (var col in toHide)
                if (dgvMyNeeds.Columns.Contains(col)) dgvMyNeeds.Columns[col].Visible = false;

            if (dgvMyNeeds.Columns.Contains("productName")) dgvMyNeeds.Columns["productName"].HeaderText = "اسم المنتج";
            if (dgvMyNeeds.Columns.Contains("quantity")) dgvMyNeeds.Columns["quantity"].HeaderText = "الكمية";
            if (dgvMyNeeds.Columns.Contains("الوحدة")) dgvMyNeeds.Columns["الوحدة"].HeaderText = "الوحدة";
            if (dgvMyNeeds.Columns.Contains("حالة_الطلب")) dgvMyNeeds.Columns["حالة_الطلب"].HeaderText = "الحالة";
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UC_MyNeeds
            // 
            this.Name = "UC_MyNeeds";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ResumeLayout(false);

        }
    }
}