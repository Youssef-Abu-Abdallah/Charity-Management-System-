namespace WindowsFormsApp1.Forms // تأكد ان الـ namespace مطابق للملف الأول
{
    partial class UC_Profile
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UC_Profile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "UC_Profile";
            this.Size = new System.Drawing.Size(800, 600);
            this.ResumeLayout(false);
        }
    }
}