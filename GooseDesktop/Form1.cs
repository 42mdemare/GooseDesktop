using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GooseDesktop
{
	public class Form1 : Form
	{
		private IContainer components;

		public Form1()
		{
			InitializeComponent();
		}

        private void Form1_Load(object sender, EventArgs e)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

        private void InitializeComponent()
		{
			SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 16f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(282, 253);
			base.Name = "Form1";
			Text = "Form1";
			base.Load += new System.EventHandler(Form1_Load);
			ResumeLayout(false);
		}
	}
}
