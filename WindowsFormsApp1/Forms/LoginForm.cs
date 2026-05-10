using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1.Config;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            // تنسيق بسيط للحقول
            txtIdentifier.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            // 1. التحقق من أن الحقول ليست فارغة
            if (string.IsNullOrWhiteSpace(txtIdentifier.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("يرجى إدخال اسم المستخدم وكلمة المرور", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. تجهيز شكل الزرار أثناء التحميل
            btnLogin.Enabled = false;
            btnLogin.Text = "جاري التحقق...";
            Color originalColor = btnLogin.BackColor;
            btnLogin.BackColor = Color.Gray;

            try
            {
                // 3. محاولة تسجيل الدخول عبر الخدمة
                AuthService authService = new AuthService();

                // ملاحظة: الـ AuthService جوه دالة LoginAsync هو اللي بيملى AppConfig بالبيانات
                bool isSuccess = await authService.LoginAsync(txtIdentifier.Text, txtPassword.Text);

                if (isSuccess)
                {
                    // 4. تأكيد نجاح العملية (اختياري للتأكد من الـ Role)
                    // MessageBox.Show($"مرحباً {AppConfig.CurrentUserName}, رقم صلاحيتك هو {AppConfig.CurrentUserRoleID}");

                    // 5. الانتقال للشاشة الرئيسية
                    this.Hide();
                    MainForm mainDashboard = new MainForm();
                    mainDashboard.Show();
                }
                else
                {
                    MessageBox.Show("اسم المستخدم أو كلمة المرور غير صحيحة", "فشل الدخول", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnLogin.Enabled = true;
                    btnLogin.Text = "تسجيل الدخول";
                    btnLogin.BackColor = originalColor;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ غير متوقع: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLogin.Enabled = true;
                btnLogin.Text = "تسجيل الدخول";
                btnLogin.BackColor = originalColor;
            }
        }
    }
}