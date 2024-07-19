namespace XConnectTest
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
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.txtTrID = new System.Windows.Forms.TextBox();
            this.ComboCurrency = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.btnPrintTotal = new System.Windows.Forms.Button();
            this.btnVoid = new System.Windows.Forms.Button();
            this.btnRefund = new System.Windows.Forms.Button();
            this.btnCloseDay = new System.Windows.Forms.Button();
            this.btnPay = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtAmount
            // 
            this.txtAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAmount.Location = new System.Drawing.Point(35, 77);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(226, 21);
            this.txtAmount.TabIndex = 16;
            this.txtAmount.Text = "100";
            this.txtAmount.TextChanged += new System.EventHandler(this.txtAmount_TextChanged);
            // 
            // txtTrID
            // 
            this.txtTrID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTrID.Location = new System.Drawing.Point(150, 151);
            this.txtTrID.Name = "txtTrID";
            this.txtTrID.Size = new System.Drawing.Size(193, 21);
            this.txtTrID.TabIndex = 15;
            this.txtTrID.TextChanged += new System.EventHandler(this.txtTrID_TextChanged);
            // 
            // ComboCurrency
            // 
            this.ComboCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboCurrency.FormattingEnabled = true;
            this.ComboCurrency.Items.AddRange(new object[] {
            "GEL",
            "EUR",
            "USD",
            "ERTGULI",
            "PLUS"});
            this.ComboCurrency.Location = new System.Drawing.Point(281, 37);
            this.ComboCurrency.Name = "ComboCurrency";
            this.ComboCurrency.Size = new System.Drawing.Size(226, 21);
            this.ComboCurrency.TabIndex = 14;
            this.ComboCurrency.SelectedIndexChanged += new System.EventHandler(this.ComboCurrency_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(279, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Currency";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Alias";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtAlias
            // 
            this.txtAlias.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAlias.Location = new System.Drawing.Point(33, 37);
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(228, 21);
            this.txtAlias.TabIndex = 11;
            this.txtAlias.Text = "UFC-TBC";
            this.txtAlias.TextChanged += new System.EventHandler(this.txtAlias_TextChanged);
            // 
            // btnPrintTotal
            // 
            this.btnPrintTotal.Location = new System.Drawing.Point(399, 104);
            this.btnPrintTotal.Name = "btnPrintTotal";
            this.btnPrintTotal.Size = new System.Drawing.Size(111, 30);
            this.btnPrintTotal.TabIndex = 6;
            this.btnPrintTotal.Text = "Print Total";
            this.btnPrintTotal.UseVisualStyleBackColor = true;
            this.btnPrintTotal.Click += new System.EventHandler(this.btnPrintTotal_Click);
            // 
            // btnVoid
            // 
            this.btnVoid.Location = new System.Drawing.Point(33, 147);
            this.btnVoid.Name = "btnVoid";
            this.btnVoid.Size = new System.Drawing.Size(111, 30);
            this.btnVoid.TabIndex = 7;
            this.btnVoid.Text = "Void";
            this.btnVoid.UseVisualStyleBackColor = true;
            this.btnVoid.Click += new System.EventHandler(this.btnVoid_Click);
            // 
            // btnRefund
            // 
            this.btnRefund.Location = new System.Drawing.Point(150, 104);
            this.btnRefund.Name = "btnRefund";
            this.btnRefund.Size = new System.Drawing.Size(111, 30);
            this.btnRefund.TabIndex = 8;
            this.btnRefund.Text = "Refund";
            this.btnRefund.UseVisualStyleBackColor = true;
            this.btnRefund.Click += new System.EventHandler(this.btnRefund_Click);
            // 
            // btnCloseDay
            // 
            this.btnCloseDay.Location = new System.Drawing.Point(282, 104);
            this.btnCloseDay.Name = "btnCloseDay";
            this.btnCloseDay.Size = new System.Drawing.Size(111, 30);
            this.btnCloseDay.TabIndex = 9;
            this.btnCloseDay.Text = "Close Day";
            this.btnCloseDay.UseVisualStyleBackColor = true;
            this.btnCloseDay.Click += new System.EventHandler(this.btnCloseDay_Click);
            // 
            // btnPay
            // 
            this.btnPay.Location = new System.Drawing.Point(33, 104);
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(111, 30);
            this.btnPay.TabIndex = 10;
            this.btnPay.Text = "Payment";
            this.btnPay.UseVisualStyleBackColor = true;
            this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 199);
            this.Controls.Add(this.txtAmount);
            this.Controls.Add(this.txtTrID);
            this.Controls.Add(this.ComboCurrency);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtAlias);
            this.Controls.Add(this.btnPrintTotal);
            this.Controls.Add(this.btnVoid);
            this.Controls.Add(this.btnRefund);
            this.Controls.Add(this.btnCloseDay);
            this.Controls.Add(this.btnPay);
            this.Name = "Form1";
            this.Text = "XConnect";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.TextBox txtTrID;
        private System.Windows.Forms.ComboBox ComboCurrency;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAlias;
        private System.Windows.Forms.Button btnPrintTotal;
        private System.Windows.Forms.Button btnVoid;
        private System.Windows.Forms.Button btnRefund;
        private System.Windows.Forms.Button btnCloseDay;
        private System.Windows.Forms.Button btnPay;
    }
}

