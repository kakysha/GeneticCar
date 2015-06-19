namespace GACar
{
	partial class Form1
	{
		/// <summary>
		/// Требуется переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Обязательный метод для поддержки конструктора - не изменяйте
		/// содержимое данного метода при помощи редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.fpsLabel = new System.Windows.Forms.Label();
			this.distanceLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// fpsLabel
			// 
			this.fpsLabel.AutoSize = true;
			this.fpsLabel.Location = new System.Drawing.Point(747, 2);
			this.fpsLabel.Name = "fpsLabel";
			this.fpsLabel.Size = new System.Drawing.Size(35, 13);
			this.fpsLabel.TabIndex = 0;
			this.fpsLabel.Text = "label1";
			// 
			// distanceLabel
			// 
			this.distanceLabel.AutoSize = true;
			this.distanceLabel.Location = new System.Drawing.Point(747, 15);
			this.distanceLabel.Name = "distanceLabel";
			this.distanceLabel.Size = new System.Drawing.Size(35, 13);
			this.distanceLabel.TabIndex = 1;
			this.distanceLabel.Text = "label1";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 562);
			this.Controls.Add(this.distanceLabel);
			this.Controls.Add(this.fpsLabel);
			this.DoubleBuffered = true;
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(800, 800);
			this.MinimumSize = new System.Drawing.Size(800, 600);
			this.Name = "Form1";
			this.Text = "GA";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label fpsLabel;
		private System.Windows.Forms.Label distanceLabel;


	}
}

