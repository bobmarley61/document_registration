using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using Npgsql;

namespace WKR
{
    public partial class Form3 : Form
    {
        NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=database123;User ID=postgres");
        NpgsqlCommand com = new NpgsqlCommand();
        DataTable dt;
        NpgsqlDataAdapter data_ad;
        public bool open = false;
        public int SelectedRow;
        public int ID;
        public bool EmptyCell = false;
        public bool EmptyRow = false;
        public Form3()
        {
            InitializeComponent();
            conn.Open();
            com.Connection = conn;
            com.CommandType = CommandType.Text;
            com.CommandText = " ";
            com.CommandText = "SELECT table_name FROM information_schema.tables WHERE table_schema='public' ORDER BY table_name;";
            data_ad = new NpgsqlDataAdapter(com);
            dt = new DataTable();
            data_ad.Fill(dt);
            dataGridView1.DataSource = dt;
            List<string> spisk = dt.AsEnumerable().Select(x => x[0].ToString()).ToList();

            foreach (string item in spisk)
            {
                comboBox1.Items.Add(item);
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = "users";
            com.CommandText = "select * from users";
            Db_Fill(com);
            open = true;

        }
        private string Hesh_Pass(string input)
        {
            MD5 Md = MD5.Create();
            byte[] getbyte = Encoding.ASCII.GetBytes(input);
            byte[] hash = Md.ComputeHash(getbyte);

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
        private void Db_Fill(NpgsqlCommand com)
        {
            try
            {
                data_ad = new NpgsqlDataAdapter(com);
                dt = new DataTable();
                data_ad.Fill(dt);
                dataGridView1.DataSource = dt;

            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось подключиться к базе данных!");
                dataGridView1.Hide();
            }
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 newForm2 = new Form2();
            newForm2.Show();
            this.Hide();
        }

        public void AddRow()
        {
            //Приказ
            if (comboBox1.Text == "prikaz")
            {
                var Cell = dataGridView1.Rows[SelectedRow].Cells[dataGridView1.CurrentCell.ColumnIndex].Value.ToString();
                string Column = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;
                try
                {
                    if (Column == "date_pr")
                    {
                        string query = $"INSERT INTO prikaz (date_pr, index_pr, text_pr, after_text_pr, date_insert, fio) VALUES (:Cell, null, null, null, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "index_pr")
                    {
                        string query = $"INSERT INTO prikaz (date_pr, index_pr, text_pr, after_text_pr, date_insert, fio) VALUES (null, :Cell, null, null, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "text_pr")
                    {
                        string query = $"INSERT INTO prikaz (date_pr, index_pr, text_pr, after_text_pr, date_insert, fio) VALUES (null, null, :Cell, null, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "after_text_pr")
                    {
                        string query = $"INSERT INTO prikaz (date_pr, index_pr, text_pr, after_text_pr, date_insert, fio) VALUES (null, null, null, :Cell, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "date_insert")
                    {
                        string query = $"INSERT INTO prikaz (date_pr, index_pr, text_pr, after_text_pr, date_insert, fio) VALUES (null, null, null, null, :Cell, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "fio")
                    {
                        string query = $"INSERT INTO prikaz (date_pr, index_pr, text_pr, after_text_pr, date_insert, fio) VALUES (null, null, null, null, null, :Cell);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    SelectTable();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка заполнения");
                }
            }
            //Служебка
            if (comboBox1.Text == "sluzebka")
            {
                var Cell = dataGridView1.Rows[SelectedRow].Cells[dataGridView1.CurrentCell.ColumnIndex].Value.ToString();
                string Column = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;
                try
                {
                    if (Column == "date_sl")
                    {
                        string query = $"INSERT INTO sluzebka (date_sl, index_sl, whom_sl, text_sl, after_text_sl, date_insert, fio) VALUES (:Cell, null, null, null, null, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "index_sl")
                    {
                        string query = $"INSERT INTO sluzebka (date_sl, index_sl, whom_sl, text_sl, after_text_sl, date_insert, fio) VALUES (null, :Cell, null, null, null, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "whom_sl")
                    {
                        string query = $"INSERT INTO sluzebka (date_sl, index_sl, whom_sl, text_sl, after_text_sl, date_insert, fio) VALUES (null, null, :Cell, null, null, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "text_sl")
                    {
                        string query = $"INSERT INTO sluzebka (date_sl, index_sl, whom_sl, text_sl, after_text_sl, date_insert, fio) VALUES (null, null, null, :Cell, null, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "after_text_sl")
                    {
                        string query = $"INSERT INTO sluzebka (date_sl, index_sl, whom_sl, text_sl, after_text_sl, date_insert, fio) VALUES (null, null, null, null, :Cell, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "date_insert")
                    {
                        string query = $"INSERT INTO sluzebka (date_sl, index_sl, whom_sl, text_sl, after_text_sl, date_insert, fio) VALUES (null, null, null, null, null, :Cell, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "fio")
                    {
                        string query = $"INSERT INTO sluzebka (date_sl, index_sl, whom_sl, text_sl, after_text_sl, date_insert, fio) VALUES (null, null, null, null, null, null, :Cell);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                SelectTable();
            }
                catch (Exception)
            {
                MessageBox.Show("Ошибка заполнения");
            }
        }
            //Информационное письмо
            if (comboBox1.Text == "inform")
            {
                var Cell = dataGridView1.Rows[SelectedRow].Cells[dataGridView1.CurrentCell.ColumnIndex].Value.ToString();
                string Column = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;
                try
                {
                    if (Column == "date_inf")
                    {
                        string query = $"INSERT INTO inform (date_inf, index_inf, text_inf, after_text_inf, date_insert, fio) VALUES (:Cell, null, null, null, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "index_inf")
                    {
                        string query = $"INSERT INTO inform (date_inf, index_inf, text_inf, after_text_inf, date_insert, fio) VALUES (null, :Cell, null, null, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "text_inf")
                    {
                        string query = $"INSERT INTO inform (date_inf, index_inf, text_inf, after_text_inf, date_insert, fio) VALUES (null, null, :Cell, null, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "after_text_inf")
                    {
                        string query = $"INSERT INTO inform (date_inf, index_inf, text_inf, after_text_inf, date_insert, fio) VALUES (null, null, null, :Cell, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "date_insert")
                    {
                        string query = $"INSERT INTO inform (date_inf, index_inf, text_inf, after_text_inf, date_insert, fio) VALUES (null, null, null, null, :Cell, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "fio")
                    {
                        string query = $"INSERT INTO inform (date_inf, index_inf, text_inf, after_text_inf, date_insert, fio) VALUES (null, null, null, null, null, :Cell);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    SelectTable();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка заполнения");
                }
            }
            //Пользователи
            if (comboBox1.Text == "users")
            {
                var Cell = dataGridView1.Rows[SelectedRow].Cells[dataGridView1.CurrentCell.ColumnIndex].Value.ToString();
                string Column = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;
                try
                {
                    if (Column == "login")
                    {
                        string query = $"INSERT INTO users (login, password, fio) VALUES (:Cell, null, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "password")
                    {
                        string query = $"INSERT INTO users (login, password, fio) VALUES (null, :Cell, null);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Hesh_Pass(Cell));
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "fio")
                    {
                        string query = $"INSERT INTO users (login, password, fio) VALUES (null, null, :Cell);";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    SelectTable();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка заполнения");
                }
            }
        }
        private void Updtbl()
        {
            //Приказ
            if (comboBox1.Text == "prikaz")
            {
                var Cell = dataGridView1.Rows[SelectedRow].Cells[dataGridView1.CurrentCell.ColumnIndex].Value.ToString();
                string Column = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;
                ID = Convert.ToInt32(dataGridView1.Rows[SelectedRow].Cells[0].Value.ToString());
                try
                {
                    if (Column == "date_pr")
                    {
                        string query = $"UPDATE prikaz SET date_pr = :Cell WHERE id_pr = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                        SelectTable();
                    }
                    if (Column == "index_pr")
                    {
                        string query = $"UPDATE prikaz SET index_pr = :Cell WHERE id_pr = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "text_pr")
                    {
                        string query = $"UPDATE prikaz SET text_pr = :Cell WHERE id_pr = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "after_text_pr")
                    {
                        string query = $"UPDATE prikaz SET after_text_pr = :Cell WHERE id_pr = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "date_insert")
                    {
                        string query = $"UPDATE prikaz SET date_insert = :Cell WHERE id_pr = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "fio")
                    {
                        string query = $"UPDATE prikaz SET fio = :Cell WHERE id_pr = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка заполнения");
                }
            }
            //Служебка
            if (comboBox1.Text == "sluzebka")
            {
                var Cell = dataGridView1.Rows[SelectedRow].Cells[dataGridView1.CurrentCell.ColumnIndex].Value.ToString();
                string Column = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;
                ID = Convert.ToInt32(dataGridView1.Rows[SelectedRow].Cells[0].Value.ToString());
                try
                {
                    if (Column == "date_sl")
                    {
                        string query = $"UPDATE sluzebka SET date_sl = :Cell WHERE id_sl = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "index_sl")
                    {
                        string query = $"UPDATE sluzebka SET index_sl = :Cell WHERE id_sl = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "whom_sl")
                    {
                        string query = $"UPDATE sluzebka SET whom_sl = :Cell WHERE id_sl = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "text_sl")
                    {
                        string query = $"UPDATE sluzebka SET text_sl = :Cell WHERE id_sl = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "after_text_sl")
                    {
                        string query = $"UPDATE sluzebka SET after_text_sl = :Cell WHERE id_sl = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "date_insert")
                    {
                        string query = $"UPDATE sluzebka SET date_insert = :Cell WHERE id_sl = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "fio")
                    {
                        string query = $"UPDATE sluzebka SET fio = :Cell WHERE id_sl = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка заполнения");
                }
            }
            //Информационное письмо
            if (comboBox1.Text == "inform")
            {
                var Cell = dataGridView1.Rows[SelectedRow].Cells[dataGridView1.CurrentCell.ColumnIndex].Value.ToString();
                string Column = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;
                ID = Convert.ToInt32(dataGridView1.Rows[SelectedRow].Cells[0].Value.ToString());
                try
                {
                    if (Column == "date_inf")
                    {
                        string query = $"UPDATE inform SET date_inf = :Cell WHERE id_inf = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "index_inf")
                    {
                        string query = $"UPDATE inform SET index_inf = :Cell WHERE id_inf = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "text_inf")
                    {
                        string query = $"UPDATE inform SET text_inf = :Cell WHERE id_inf = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "after_text_inf")
                    {
                        string query = $"UPDATE inform SET after_text_inf = :Cell WHERE id_inf = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "date_insert")
                    {
                        string query = $"UPDATE inform SET date_insert = :Cell WHERE id_inf = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "fio")
                    {
                        string query = $"UPDATE inform SET fio = :Cell WHERE id_inf = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка заполнения");
                }
            }
            //Пользователи
            if (comboBox1.Text == "users")
            {
                var Cell = dataGridView1.Rows[SelectedRow].Cells[dataGridView1.CurrentCell.ColumnIndex].Value.ToString();
                string Column = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;
                ID = Convert.ToInt32(dataGridView1.Rows[SelectedRow].Cells[0].Value.ToString());
                try
                {
                    if (Column == "login")
                    {
                        string query = $"UPDATE users SET login = :Cell WHERE id = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "password")
                    {
                        string query = $"UPDATE users SET password = :Cell WHERE id = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Hesh_Pass(Cell));
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                    if (Column == "fio")
                    {
                        string query = $"UPDATE users SET fio = :Cell WHERE id = :id;";
                        NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                        cmd.Parameters.AddWithValue(":Cell", Cell);
                        cmd.Parameters.AddWithValue(":id", ID);
                        var rowsAffected = cmd.ExecuteScalar();
                        query = "";
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка заполнения");
                }
            }
        }
        public void DeleteRow()
        {
            if (comboBox1.Text == "prikaz")
            {
                try
                {
                    ID = Convert.ToInt32(dataGridView1.Rows[SelectedRow].Cells[0].Value.ToString());
                    string query = "DELETE FROM prikaz WHERE id_pr = :id;";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue(":id", ID);
                    var rowsAffected = cmd.ExecuteScalar();
                    query = "";
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка удаления записи");

                }
            }
            if (comboBox1.Text == "sluzebka")
            {
                try
                {
                    ID = Convert.ToInt32(dataGridView1.Rows[SelectedRow].Cells[0].Value.ToString());
                    string query = "DELETE FROM sluzebka WHERE id_sl = :id;";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue(":id", ID);
                    var rowsAffected = cmd.ExecuteScalar();
                    query = "";
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка удаления записи");

                }
            }
            if (comboBox1.Text == "inform")
            {
                try
                {
                    ID = Convert.ToInt32(dataGridView1.Rows[SelectedRow].Cells[0].Value.ToString());
                    string query = "DELETE FROM inform WHERE id_inf = :id;";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue(":id", ID);
                    var rowsAffected = cmd.ExecuteScalar();
                    query = "";
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка удаления записи");

                }
            }
            if (comboBox1.Text == "users")
            {
                try
                {
                    ID = Convert.ToInt32(dataGridView1.Rows[SelectedRow].Cells[0].Value.ToString());
                    string query = "DELETE FROM users WHERE id = :id;";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue(":id", ID);
                    var rowsAffected = cmd.ExecuteScalar();
                    query = "";
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка удаления записи");

                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectTable();
        }
        private void SelectTable()
        {
            if (comboBox1.Text == "users")
            {
                com.CommandText = "select * from users";
                Db_Fill(com);
            }
            if (comboBox1.Text == "prikaz")
            {
                com.CommandText = "select * from prikaz";
                Db_Fill(com);
            }
            if (comboBox1.Text == "sluzebka")
            {
                com.CommandText = "select * from sluzebka";
                Db_Fill(com);
            }
            if (comboBox1.Text == "inform")
            {
                com.CommandText = "select * from inform";
                Db_Fill(com);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                NpgsqlCommand ins_id1 = new NpgsqlCommand("select setval('prikaz_id_pr_seq', (select max(id_pr) from prikaz));", conn);
                NpgsqlCommand ins_id2 = new NpgsqlCommand("select setval('sluzebka_id_sl_seq', (select max(id_sl) from sluzebka));", conn);
                NpgsqlCommand ins_id3 = new NpgsqlCommand("select setval('inform_id_inf_seq', (select max(id_inf) from inform));", conn);
                NpgsqlCommand ins_id4 = new NpgsqlCommand("select setval('users_id_seq', (select max(id) from users));", conn);

                ins_id1.ExecuteScalar();
                ins_id2.ExecuteScalar();
                ins_id3.ExecuteScalar();
                ins_id4.ExecuteScalar();
                DataTable DT = (DataTable)dataGridView1.DataSource;
                if (DT != null)
                    DT.Clear();
                SelectTable();

                dataGridView1.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectedRow = dataGridView1.CurrentCell.RowIndex;


            if (dataGridView1.Rows[SelectedRow].Cells[dataGridView1.CurrentCell.ColumnIndex].Value == System.DBNull.Value)
            {
                EmptyCell = true;
            }
            else
            {
                EmptyCell = false;
            }

            if (dataGridView1.Rows[SelectedRow].Cells[0].Value.ToString() == string.Empty)
            {
                EmptyRow = true;
            }
            else
            {
                EmptyRow = false;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((EmptyCell == true) & (EmptyRow == true))
            {
                AddRow();
            }
            else if ((EmptyCell == true) & (EmptyRow == false))
            {
                Updtbl();
            }
            else if ((EmptyCell == false) & (EmptyRow == false))
            {
                Updtbl();
            }
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult res = MessageBox.Show("Вы уверены, что хотите удалить строку?", "Подтверждение на удаление строки", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                DeleteRow();
            }
            if (res == DialogResult.No)
            {

            }
            dataGridView1.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime datetime = DateTime.Now;
                int del_number = Convert.ToInt32(textBox1.Text);

                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    string table_d = dataGridView1[5, i].Value.ToString();
                    DateTime table_date = Convert.ToDateTime(table_d);
                    int total = (datetime - table_date).Days;
                    if (del_number < Convert.ToInt32(total))
                    {
                        if (comboBox1.Text == "prikaz")
                        {
                            NpgsqlCommand del_date = new NpgsqlCommand("delete from prikaz where date_insert='" + table_date.ToString("dd.MM.yyyy") + "'", conn);
                            del_date.ExecuteScalar();
                        }
                        if (comboBox1.Text == "sluzebka")
                        {
                            NpgsqlCommand del_date = new NpgsqlCommand("delete from sluzebka where date_insert='" + table_date.ToString("dd.MM.yyyy") + "'", conn);
                            del_date.ExecuteScalar();
                        }
                        if (comboBox1.Text == "inform")
                        {
                            NpgsqlCommand del_date = new NpgsqlCommand("delete from inform where date_insert='" + table_date.ToString("dd.MM.yyyy") + "'", conn);
                            del_date.ExecuteScalar();
                        }
                    }
                }
                MessageBox.Show("Успешно удалено");
            }
            catch (Exception)
            {
                MessageBox.Show("Таких записей нет");
            }
        }
    }
}
