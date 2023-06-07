using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.Security.Cryptography;

namespace WKR
{
    public partial class Form2 : Form
    {
        public static string login;
        public Form2()
        {
            InitializeComponent();
            textBox1.PasswordChar = '\u25CF';

        }

        private string Hesh_Pass(string input)
        {
            MD5 Md = MD5.Create();
            byte[] getbyte = Encoding.ASCII.GetBytes(input);
            byte[] hash = Md.ComputeHash(getbyte);

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=database123;User ID=postgres");
            conn.Open();
            NpgsqlCommand auth = new NpgsqlCommand("select login from users where id = 1", conn);
            string adm = ((string)auth.ExecuteScalar());
            login = textBox2.Text;
            try { 
                NpgsqlCommand com = new NpgsqlCommand("select * from users where login= '" + textBox2.Text + "' and password = '" + Hesh_Pass(textBox1.Text) + "'", conn);
                NpgsqlDataReader dr = com.ExecuteReader();

                if (dr.Read())
                {
                    if (textBox2.Text == adm)
                    {
                        Form3 newForm3 = new Form3();
                        newForm3.Show();
                        this.Hide();
                    }
                    else
                    {
                        Form1 newForm1 = new Form1(login);
                        newForm1.Show();
                        this.Hide();
                    }
                }
                else
                {
                    label4.Show();
                    label4.Text = "Проверьте корректность введенных данных";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте подключение к базе данных");
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            label4.Hide();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            label4.Hide();
        }
    }
}
