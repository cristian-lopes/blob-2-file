using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ICE.Blob2File
{
	public partial class Main : Form
	{
		public Main()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Form1 f = new Form1();
			f.Show();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Form2 f = new Form2();
			f.Show();
		}
	}
}
