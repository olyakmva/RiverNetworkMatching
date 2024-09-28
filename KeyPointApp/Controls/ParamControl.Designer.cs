namespace KeyPointApp.Controls
{
    partial class ParamControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            distanceNumUpDown = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)distanceNumUpDown).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(2, 6);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(130, 28);
            label1.TabIndex = 0;
            label1.Text = "BendDistance";
            // 
            // distanceNumUpDown
            // 
            distanceNumUpDown.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            distanceNumUpDown.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            distanceNumUpDown.Location = new Point(136, 7);
            distanceNumUpDown.Margin = new Padding(2);
            distanceNumUpDown.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            distanceNumUpDown.Name = "distanceNumUpDown";
            distanceNumUpDown.Size = new Size(93, 32);
            distanceNumUpDown.TabIndex = 3;
            distanceNumUpDown.Value = new decimal(new int[] { 700, 0, 0, 0 });
            // 
            // ParamControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLight;
            Controls.Add(distanceNumUpDown);
            Controls.Add(label1);
            Margin = new Padding(2);
            Name = "ParamControl";
            Size = new Size(235, 54);
            ((System.ComponentModel.ISupportInitialize)distanceNumUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private NumericUpDown distanceNumUpDown;
    }
}
