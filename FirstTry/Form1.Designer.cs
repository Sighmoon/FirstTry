namespace WindowsFormsApplication1
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
            this.outTextBox = new System.Windows.Forms.TextBox();
            this.resultButton = new System.Windows.Forms.Button();
            this.requestTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // outTextBox
            // 
            this.outTextBox.Location = new System.Drawing.Point(65, 103);
            this.outTextBox.Multiline = true;
            this.outTextBox.Name = "outTextBox";
            this.outTextBox.Size = new System.Drawing.Size(514, 148);
            this.outTextBox.TabIndex = 0;
            // 
            // resultButton
            // 
            this.resultButton.Location = new System.Drawing.Point(634, 22);
            this.resultButton.Name = "resultButton";
            this.resultButton.Size = new System.Drawing.Size(75, 23);
            this.resultButton.TabIndex = 2;
            this.resultButton.Text = "Запрос";
            this.resultButton.UseVisualStyleBackColor = true;
            this.resultButton.Click += new System.EventHandler(this.resultButton_Click);
            // 
            // requestTextBox
            // 
            this.requestTextBox.Location = new System.Drawing.Point(65, 22);
            this.requestTextBox.Name = "requestTextBox";
            this.requestTextBox.Size = new System.Drawing.Size(514, 20);
            this.requestTextBox.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 283);
            this.Controls.Add(this.requestTextBox);
            this.Controls.Add(this.resultButton);
            this.Controls.Add(this.outTextBox);
            this.Name = "Form1";
            this.Text = "Парсер";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox outTextBox;
        private System.Windows.Forms.Button resultButton;
        private System.Windows.Forms.TextBox requestTextBox;
    }
}

