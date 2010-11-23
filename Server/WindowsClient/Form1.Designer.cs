namespace WindowsClient
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
            this.lst_Types1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lst_Types2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lst_Types3 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lst_Types4 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lst_Rounds = new System.Windows.Forms.ComboBox();
            this.btn_Start = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lst_Types1
            // 
            this.lst_Types1.FormattingEnabled = true;
            this.lst_Types1.Location = new System.Drawing.Point(88, 10);
            this.lst_Types1.Name = "lst_Types1";
            this.lst_Types1.Size = new System.Drawing.Size(121, 21);
            this.lst_Types1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Player 1:";
            // 
            // lst_Types2
            // 
            this.lst_Types2.FormattingEnabled = true;
            this.lst_Types2.Location = new System.Drawing.Point(87, 41);
            this.lst_Types2.Name = "lst_Types2";
            this.lst_Types2.Size = new System.Drawing.Size(121, 21);
            this.lst_Types2.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Player 2:";
            // 
            // lst_Types3
            // 
            this.lst_Types3.FormattingEnabled = true;
            this.lst_Types3.Location = new System.Drawing.Point(87, 72);
            this.lst_Types3.Name = "lst_Types3";
            this.lst_Types3.Size = new System.Drawing.Size(121, 21);
            this.lst_Types3.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Player 3:";
            // 
            // lst_Types4
            // 
            this.lst_Types4.FormattingEnabled = true;
            this.lst_Types4.Location = new System.Drawing.Point(87, 102);
            this.lst_Types4.Name = "lst_Types4";
            this.lst_Types4.Size = new System.Drawing.Size(121, 21);
            this.lst_Types4.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Player 4:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(252, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Rounds";
            // 
            // lst_Rounds
            // 
            this.lst_Rounds.FormattingEnabled = true;
            this.lst_Rounds.Items.AddRange(new object[] {
            "1",
            "2",
            "5",
            "10",
            "15",
            "20",
            "50",
            "100",
            "200",
            "1000"});
            this.lst_Rounds.Location = new System.Drawing.Point(302, 12);
            this.lst_Rounds.Name = "lst_Rounds";
            this.lst_Rounds.Size = new System.Drawing.Size(121, 21);
            this.lst_Rounds.TabIndex = 0;
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(224, 72);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(199, 51);
            this.btn_Start.TabIndex = 3;
            this.btn_Start.Text = "Start";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 147);
            this.Controls.Add(this.btn_Start);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lst_Types4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lst_Types3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lst_Types2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lst_Rounds);
            this.Controls.Add(this.lst_Types1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox lst_Types1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox lst_Types2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox lst_Types3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox lst_Types4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox lst_Rounds;
        private System.Windows.Forms.Button btn_Start;
    }
}

