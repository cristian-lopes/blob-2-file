using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace ICE.Blob2File
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

		public string conteudo = string.Empty;
		public string caminho = string.Empty;

		public DataTable Deserialize(byte[] result)
		{
			BinaryFormatter bformatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			DataTable dt;

			bformatter = new BinaryFormatter();
			byte[] d;
			stream = new MemoryStream(result);
			dt = (DataTable)bformatter.Deserialize(stream);
			stream.Close();

			return dt;
		}

		public byte[] Serialize(DataTable result)
		{
			byte[] binaryDataResult = null;
			using (MemoryStream memStream = new MemoryStream())
			{
				BinaryFormatter brFormatter = new BinaryFormatter();
				result.RemotingFormat = SerializationFormat.Binary;
				brFormatter.Serialize(memStream, result);
				binaryDataResult = memStream.ToArray();
			}
			return binaryDataResult;
		}

		

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (conteudo != null && conteudo.Length > 0)
                {
					using (SqlConnection conn = new SqlConnection("Data Source=" + txtServidor.Text + "; Database=" + txtBanco.Text + "; User ID=" + txtUsuario.Text + "; Password=" + txtSenha.Text + ";"))
					{
						using (SqlCommand cmd = new SqlCommand(txtQuery.Text, conn))
						{
							SqlParameter p = new SqlParameter("@BLOB", SqlDbType.Xml);
							p.Value = conteudo;
							cmd.Parameters.Add(p);
							cmd.Connection.Open();
							int resultado = cmd.ExecuteNonQuery();
						}
					}
				}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Está dando erro para salvar o arquivo!\n\n" + ex.Message);
            }
        }

		private void txtExtensao_MouseClick(object sender, MouseEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				caminho = ofd.FileName;
				txtExtensao.Text = ofd.FileName;
			}			
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			conteudo = File.ReadAllText(this.caminho);
			lblTamanhoArquivo.Text = $"Tamanho: {conteudo.Length} kb";
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{

		}
	}
}
