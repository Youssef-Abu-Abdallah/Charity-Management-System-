using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Forms
{
    public partial class UC_Profile : UserControl
    {
        private Label lblTitle;
        private Panel pnlCard;

        public UC_Profile()
        {
            // استدعاء الدالة الرسمية القادمة من ملف الـ Designer.cs بدون تداخل
            InitializeComponent();
            SetupProfileUI();
        }

        private void SetupProfileUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.RightToLeft = RightToLeft.Yes;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            var user = GlobalUser.CurrentUser;
            if (user == null) return;

            // 1. عنوان الصفحة
            lblTitle = new Label
            {
                Text = "👤 حسابي الشخصي",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(43, 54, 116),
                Location = new Point(20, 25),
                AutoSize = true
            };

            // 2. بطاقة عرض البيانات
            pnlCard = new Panel
            {
                BackColor = Color.White,
                Location = new Point(25, 85),
                Size = new Size(this.Width - 50, 260),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.None
            };

            // ضبط الأبعاد المرنة عند تغيير حجم الشاشة
            this.SizeChanged += (s, e) => {
                lblTitle.Location = new Point(this.Width - lblTitle.Width - 25, 25);
                pnlCard.Size = new Size(this.Width - 50, pnlCard.Height);
            };

            // رسم حواف خفيفة للبطاقة
            pnlCard.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle,
                    Color.FromArgb(230, 233, 238), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(230, 233, 238), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(230, 233, 238), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(230, 233, 238), 1, ButtonBorderStyle.Solid);
            };

            // 3. توزيع الحقول برمجياً بدقة RTL
            int currentY = 30;

            AddProfileField("اسم الحساب المعتمد :", user.UserName, currentY);
            currentY += 75;

            AddProfileField("نوع الصلاحية والنظام :", GetRoleName(user.Role), currentY);

            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlCard);
        }

        private void AddProfileField(string title, string value, int yPosition)
        {
            Label lblTitleField = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(74, 85, 104),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            Label lblValueField = new Label
            {
                Text = value ?? "غير متوفر بنظام الحساب",
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = Color.FromArgb(43, 54, 116),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            pnlCard.Controls.Add(lblTitleField);
            pnlCard.Controls.Add(lblValueField);

            Action layoutFields = () =>
            {
                lblTitleField.Location = new Point(pnlCard.Width - lblTitleField.Width - 30, yPosition);
                lblValueField.Location = new Point(pnlCard.Width - lblTitleField.Width - lblValueField.Width - 50, yPosition + 2);
            };

            layoutFields();
            pnlCard.SizeChanged += (s, e) => layoutFields();
        }

        private string GetRoleName(int role)
        {
            switch (role)
            {
                case 0: return "🏢 جمعية خيرية ";
                case 1: return "🤝 متبرع (جهة مانحة)";
                case 2: return "🛡️ مسؤول النظام (Admin)";
                default: return "👤 مستخدم عام";
            }
        }
    }
}