namespace Support
{
	partial class MainForm
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
			this.components = new System.ComponentModel.Container();
			this.btnCapture = new System.Windows.Forms.Button();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.btnStart1 = new System.Windows.Forms.Button();
			this.lblStatus = new System.Windows.Forms.Label();
			this.btnStart2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnCapture
			// 
			this.btnCapture.Location = new System.Drawing.Point(52, 22);
			this.btnCapture.Name = "btnCapture";
			this.btnCapture.Size = new System.Drawing.Size(75, 23);
			this.btnCapture.TabIndex = 0;
			this.btnCapture.Text = "Скриншот";
			this.btnCapture.UseVisualStyleBackColor = true;
			this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
			// 
			// timer
			// 
			this.timer.Interval = 1;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// btnStart1
			// 
			this.btnStart1.Location = new System.Drawing.Point(52, 51);
			this.btnStart1.Name = "btnStart1";
			this.btnStart1.Size = new System.Drawing.Size(75, 23);
			this.btnStart1.TabIndex = 1;
			this.btnStart1.Text = "Старт 1";
			this.btnStart1.UseVisualStyleBackColor = true;
			this.btnStart1.Click += new System.EventHandler(this.btnStart1_Click);
			// 
			// lblStatus
			// 
			this.lblStatus.AutoSize = true;
			this.lblStatus.Location = new System.Drawing.Point(151, 73);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(41, 13);
			this.lblStatus.TabIndex = 2;
			this.lblStatus.Text = "Статус";
			// 
			// btnStart2
			// 
			this.btnStart2.Location = new System.Drawing.Point(52, 80);
			this.btnStart2.Name = "btnStart2";
			this.btnStart2.Size = new System.Drawing.Size(75, 23);
			this.btnStart2.TabIndex = 3;
			this.btnStart2.Text = "Старт 2";
			this.btnStart2.UseVisualStyleBackColor = true;
			this.btnStart2.Click += new System.EventHandler(this.btnStart2_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(619, 338);
			this.Controls.Add(this.btnStart2);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.btnStart1);
			this.Controls.Add(this.btnCapture);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCapture;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.Button btnStart1;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Button btnStart2;
	}
}

