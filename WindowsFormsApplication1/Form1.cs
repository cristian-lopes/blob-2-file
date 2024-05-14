using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace ICE.Blob2File
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
	

		public List<ResultadoQuery> resultado;
		public string diretorio = string.Empty;

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

		public static byte[] StringToByteArray(String hex)
		{
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}

		public bool HasColumn(IDataRecord dr, string columnName)
		{
			for (int i = 0; i < dr.FieldCount; i++)
			{
				if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
					return true;
			}
			return false;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				if (txtServidor.Text != "" && txtBanco.Text != "" && txtUsuario.Text != "" && txtSenha.Text != "")
				{
					if (txtExtensao.Text != "")
					{
						if (txtQuery.Text != "")
						{
							if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
							{
								resultado = new List<ResultadoQuery>();
								diretorio = folderBrowserDialog1.SelectedPath;

								using (SqlConnection conn = new SqlConnection("Data Source=" + txtServidor.Text + "; Database=" + txtBanco.Text + "; User ID=" + txtUsuario.Text + "; Password=" + txtSenha.Text + ";"))
								{
									using (SqlCommand cmd = new SqlCommand(txtQuery.Text, conn))
									{
										cmd.Connection.Open();

										
										string guid = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8);
										int index = 1;

										using (SqlDataReader dr = cmd.ExecuteReader())
										{
											while (dr.Read())
											{
												var res = new ResultadoQuery();

												if (HasColumn(dr, "imagem"))
													res.Imagem = (byte[])dr["imagem"];
												else
													res.Imagem = (byte[])dr[0];

												if(HasColumn(dr, "chave"))
													res.Chave = Convert.ToString(dr["chave"]).Replace(":","");
												else
													res.Chave = $"{guid}_{index}";
												
												resultado.Add(res);
												index++;
											}
										}
									}
								}

								if (resultado.Count > 0)
								{
									lblResultado.Text = "- ok -";
									btnSalvar.Enabled = true;
								}
								else
								{
									lblResultado.Text = "- vazio -";
									btnSalvar.Enabled = false;
								}
							}
						}
						else
						{
							MessageBox.Show("E a query ??");
						}
					}
					else
					{
						MessageBox.Show("Vai salvar em qual extensão ??");
					}
				}
				else
				{
					MessageBox.Show("Por favor amigo, preencha todos os campos!");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Você está fazendo algum cag#$% !!\n\n" + ex.Message);
			}            
		}

		private void btnSalvar_Click(object sender, EventArgs e)
		{
			try
			{
				if (resultado != null && resultado.Count > 0)
				{

					foreach (var item in resultado)
					{
						if(!File.Exists(Path.Combine(diretorio, item.Chave + "." + txtExtensao.Text)))
							File.WriteAllBytes(Path.Combine(diretorio, item.Chave + "." + txtExtensao.Text), item.Imagem);
						else
							File.WriteAllBytes(Path.Combine(diretorio, item.Chave +"_"+ Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 2) + "." + txtExtensao.Text), item.Imagem);
					}

					resultado = new List<ResultadoQuery>();
					MessageBox.Show("Pronto, deu certo!!");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Está dando erro para salvar o(s) arquivo(s)!\n\n" + ex.Message);
			}
		}
	}

	public class ResultadoQuery
	{
		public byte[] Imagem { get; set; }
		public string Chave { get; set; }
	}
}
