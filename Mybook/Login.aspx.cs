﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mybook
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_entrar_Click(object sender, EventArgs e)
        {
            SqlConnection myConn = new SqlConnection(ConfigurationManager.ConnectionStrings["Mybook"].ConnectionString);

            SqlCommand myCommand = new SqlCommand();

            myCommand.Parameters.AddWithValue("@email", tb_email.Text);
            myCommand.Parameters.AddWithValue("@pw",EncryptString (tb_password.Text));

            SqlParameter retorno = new SqlParameter();
            retorno.ParameterName = "@retorno";
            retorno.Direction = ParameterDirection.Output;
            retorno.SqlDbType = SqlDbType.Int;
            myCommand.Parameters.Add(retorno);
            
            SqlParameter retorno_perfil = new SqlParameter();
            retorno_perfil.ParameterName = "@retorno_perfil";
            retorno_perfil.Direction = ParameterDirection.Output;
            retorno_perfil.SqlDbType = SqlDbType.Int;
            myCommand.Parameters.Add(retorno_perfil);

            SqlParameter retorno_nome = new SqlParameter("@retorno_nome", System.Data.SqlDbType.VarChar, 50);
            //retorno_nome.ParameterName = "@retorno_nome";
            retorno_nome.Direction = ParameterDirection.Output;
           // retorno_nome.SqlDbType = SqlDbType.VarChar;
            myCommand.Parameters.Add(retorno_nome);

            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.CommandText = "login";

            myCommand.Connection = myConn;
            myConn.Open();
            myCommand.ExecuteNonQuery();
            int respostaRetorno = Convert.ToInt32(myCommand.Parameters["@retorno"].Value);
            string resposta = Convert.ToString (myCommand.Parameters["@retorno_nome"].Value);
            int respostaperfil = Convert.ToInt32(myCommand.Parameters["@retorno_perfil"].Value);
            myConn.Close();

            if (respostaRetorno == 1)
            {
                Session["id_perfil"] = respostaperfil;
                Session["email"] = tb_email.Text;
                Session["nome"] = resposta;
                Response.Redirect("Index.aspx");
            }
            else
            {
                lbl_mensagem.Text = "*Credenciais Incorretas*";
            }
            if(tb_email.Text == null || tb_password.Text == null)
            {
                lbl_mensagem.Text = null;
            }
        }

        protected void vtn_voltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("Index.aspx");
        }

        protected void btn_register_Click(object sender, EventArgs e)
        {
            Response.Redirect("Register.aspx");
        }

        public static string EncryptString(string Message)
        {
            string Passphrase = "MyBook";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();



            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below



            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));



            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();



            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;



            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);



            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }



            // Step 6. Return the encrypted string as a base64 encoded string



            string enc = Convert.ToBase64String(Results);
            enc = enc.Replace("+", "KkKkK");
            enc = enc.Replace("/", "JjJjJ");
            enc = enc.Replace("\\", "IiIiI");
            return enc;
        }
    }
}