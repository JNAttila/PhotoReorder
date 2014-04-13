namespace PhotoReorder
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnFrom = new System.Windows.Forms.Button();
            this.tbFrom = new System.Windows.Forms.TextBox();
            this.btnTo = new System.Windows.Forms.Button();
            this.tbTo = new System.Windows.Forms.TextBox();
            this.btnReorder = new System.Windows.Forms.Button();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnFrom
            // 
            this.btnFrom.Location = new System.Drawing.Point(12, 12);
            this.btnFrom.Name = "btnFrom";
            this.btnFrom.Size = new System.Drawing.Size(75, 38);
            this.btnFrom.TabIndex = 0;
            this.btnFrom.Text = "Honnan";
            this.btnFrom.UseVisualStyleBackColor = true;
            this.btnFrom.Click += new System.EventHandler(this.btnFrom_Click);
            // 
            // tbFrom
            // 
            this.tbFrom.Location = new System.Drawing.Point(93, 22);
            this.tbFrom.Name = "tbFrom";
            this.tbFrom.ReadOnly = true;
            this.tbFrom.Size = new System.Drawing.Size(425, 20);
            this.tbFrom.TabIndex = 1;
            // 
            // btnTo
            // 
            this.btnTo.Location = new System.Drawing.Point(443, 48);
            this.btnTo.Name = "btnTo";
            this.btnTo.Size = new System.Drawing.Size(75, 38);
            this.btnTo.TabIndex = 2;
            this.btnTo.Text = "Hová";
            this.btnTo.UseVisualStyleBackColor = true;
            this.btnTo.Click += new System.EventHandler(this.btnTo_Click);
            // 
            // tbTo
            // 
            this.tbTo.Location = new System.Drawing.Point(12, 58);
            this.tbTo.Name = "tbTo";
            this.tbTo.ReadOnly = true;
            this.tbTo.Size = new System.Drawing.Size(425, 20);
            this.tbTo.TabIndex = 3;
            // 
            // btnReorder
            // 
            this.btnReorder.Location = new System.Drawing.Point(144, 84);
            this.btnReorder.Name = "btnReorder";
            this.btnReorder.Size = new System.Drawing.Size(244, 29);
            this.btnReorder.TabIndex = 4;
            this.btnReorder.Text = "Rendez";
            this.btnReorder.UseVisualStyleBackColor = true;
            this.btnReorder.Click += new System.EventHandler(this.btnReorder_Click);
            // 
            // tbResult
            // 
            this.tbResult.Location = new System.Drawing.Point(12, 119);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbResult.Size = new System.Drawing.Size(506, 304);
            this.tbResult.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 435);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.btnReorder);
            this.Controls.Add(this.tbTo);
            this.Controls.Add(this.btnTo);
            this.Controls.Add(this.tbFrom);
            this.Controls.Add(this.btnFrom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFrom;
        private System.Windows.Forms.TextBox tbFrom;
        private System.Windows.Forms.Button btnTo;
        private System.Windows.Forms.TextBox tbTo;
        private System.Windows.Forms.Button btnReorder;
        private System.Windows.Forms.TextBox tbResult;
    }
}

