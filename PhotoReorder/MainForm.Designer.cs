namespace PhotoReorder
{
    partial class PhotoReorder
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
            this.btnReorder = new System.Windows.Forms.Button();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.chbMove = new System.Windows.Forms.CheckBox();
            this.chbMachine = new System.Windows.Forms.CheckBox();
            this.chbDebug = new System.Windows.Forms.CheckBox();
            this.pgBarMain = new System.Windows.Forms.ProgressBar();
            this.flowLayoutPanel1.SuspendLayout();
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
            this.tbFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFrom.Location = new System.Drawing.Point(93, 22);
            this.tbFrom.Name = "tbFrom";
            this.tbFrom.ReadOnly = true;
            this.tbFrom.Size = new System.Drawing.Size(792, 20);
            this.tbFrom.TabIndex = 1;
            // 
            // btnReorder
            // 
            this.btnReorder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReorder.Location = new System.Drawing.Point(668, 56);
            this.btnReorder.Name = "btnReorder";
            this.btnReorder.Size = new System.Drawing.Size(136, 29);
            this.btnReorder.TabIndex = 4;
            this.btnReorder.Text = "Rendez";
            this.btnReorder.UseVisualStyleBackColor = true;
            this.btnReorder.Click += new System.EventHandler(this.btnReorder_Click);
            // 
            // tbResult
            // 
            this.tbResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResult.Location = new System.Drawing.Point(12, 91);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbResult.Size = new System.Drawing.Size(873, 392);
            this.tbResult.TabIndex = 5;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.chbMove);
            this.flowLayoutPanel1.Controls.Add(this.chbMachine);
            this.flowLayoutPanel1.Controls.Add(this.chbDebug);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 56);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(301, 29);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // chbMove
            // 
            this.chbMove.AutoSize = true;
            this.chbMove.Checked = true;
            this.chbMove.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbMove.Location = new System.Drawing.Point(3, 3);
            this.chbMove.Name = "chbMove";
            this.chbMove.Size = new System.Drawing.Size(101, 17);
            this.chbMove.TabIndex = 0;
            this.chbMove.Text = "Fájlok másolása";
            this.chbMove.UseVisualStyleBackColor = true;
            // 
            // chbMachine
            // 
            this.chbMachine.AutoSize = true;
            this.chbMachine.Checked = true;
            this.chbMachine.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbMachine.Location = new System.Drawing.Point(110, 3);
            this.chbMachine.Name = "chbMachine";
            this.chbMachine.Size = new System.Drawing.Size(108, 17);
            this.chbMachine.TabIndex = 1;
            this.chbMachine.Text = "Gépenként külön";
            this.chbMachine.UseVisualStyleBackColor = true;
            // 
            // chbDebug
            // 
            this.chbDebug.AutoSize = true;
            this.chbDebug.Checked = true;
            this.chbDebug.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbDebug.Location = new System.Drawing.Point(224, 3);
            this.chbDebug.Name = "chbDebug";
            this.chbDebug.Size = new System.Drawing.Size(73, 17);
            this.chbDebug.TabIndex = 2;
            this.chbDebug.Text = "Részletek";
            this.chbDebug.UseVisualStyleBackColor = true;
            // 
            // pgBarMain
            // 
            this.pgBarMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgBarMain.Location = new System.Drawing.Point(319, 59);
            this.pgBarMain.Name = "pgBarMain";
            this.pgBarMain.Size = new System.Drawing.Size(343, 20);
            this.pgBarMain.TabIndex = 2;
            // 
            // PhotoReorder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 495);
            this.Controls.Add(this.pgBarMain);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.btnReorder);
            this.Controls.Add(this.tbFrom);
            this.Controls.Add(this.btnFrom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PhotoReorder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fotó rendező";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFrom;
        private System.Windows.Forms.TextBox tbFrom;
        private System.Windows.Forms.Button btnReorder;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox chbMove;
        private System.Windows.Forms.CheckBox chbMachine;
        private System.Windows.Forms.ProgressBar pgBarMain;
        private System.Windows.Forms.CheckBox chbDebug;
    }
}

