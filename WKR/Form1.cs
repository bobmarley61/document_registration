using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Text.RegularExpressions;
using Npgsql;
using Tesseract;
using BitMiracle.Docotic.Pdf;
using DataTable = System.Data.DataTable;


namespace WKR
{
    public partial class Form1 : Form
    {
        NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=database123;User ID=postgres");
        NpgsqlCommand com = new NpgsqlCommand();
        DataTable dt;
        NpgsqlDataAdapter nda;
        public bool open = false;
        public int SelectedRow;
        public int ID;
        public bool EmptyCell = false;
        public bool EmptyRow = false;
        bool ButtonClick = true;
        bool ButtonClickImage = true;
        public Form1(string login)
        {
            InitializeComponent();
            label8.Hide();
            conn.Open();
            com.Connection = conn;
            com.CommandType = CommandType.Text;
            com.CommandText = " ";
            com.CommandText = "SELECT table_name FROM information_schema.tables WHERE table_schema='diplom' ORDER BY table_name;";
            nda = new NpgsqlDataAdapter(com);
            dt = new DataTable();
            nda.Fill(dt);
            dataGridView1.DataSource = dt;

            this.Width = 1100;
            this.Height = 600;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        public void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            string filepath = fileDialog.FileName.ToString();
            if (!string.IsNullOrEmpty(fileDialog.FileName))
            {  
                if (filepath.Substring(filepath.Length - 5) == ".docx" || filepath.Substring(filepath.Length - 4) == ".doc")
                {
                    try
                    {
                        Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
                        Document doc = app.Documents.Open(filepath);
                        string data = doc.Content.Text;
                        richTextBox3.Text = data;
                        app.Quit();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ошибка при выборе документа");
                    }
                }
                else if (filepath.Substring(filepath.Length - 4) == ".pdf")
                {
                    var StrBuildDoc = new StringBuilder();
                    using (var pdf = new BitMiracle.Docotic.Pdf.PdfDocument(filepath))
                    {
                        using (var tesseract = new TesseractEngine(@"tessdata", "rus", EngineMode.LstmOnly))
                        {
                            tesseract.SetVariable("textord_min_linesize", 2.5);
                            tesseract.SetVariable("lstm_choice_mode", 2);
                            for (int i = 0; i < pdf.PageCount; ++i)
                            {
                                if (StrBuildDoc.Length > 0)
                                    StrBuildDoc.Append("\r\n\r\n");

                                BitMiracle.Docotic.Pdf.PdfPage page = pdf.Pages[i];
                                string readingtext = page.GetText();
                                if (!string.IsNullOrEmpty(readingtext.Trim()))
                                {
                                    StrBuildDoc.Append(readingtext);
                                    continue;
                                }

                                foreach (BitMiracle.Docotic.Pdf.PdfImage image in page.GetImages())
                                {
                                    if (image.Height == 512)
                                        image.ReplaceWith("1px.png");
                                }

                                PdfDrawOptions Drawoptions = PdfDrawOptions.Create();
                                Drawoptions.BackgroundColor = new PdfRgbColor(255, 255, 255);
                                Drawoptions.HorizontalResolution = 100;
                                Drawoptions.VerticalResolution = 100;

                                string pageSave = $"C:/Users/jigul/Desktop/Диплом/scaning_page/page_{i}.png";
                                string pageImage = $"page_{i}.png";
                                page.Save(pageImage, Drawoptions);
                                page.Save(pageSave, Drawoptions); 
                                using (Pix img = Pix.LoadFromFile(pageImage))
                                {
                                    using (Tesseract.Page recognizedPage = tesseract.Process(img))
                                    {
                                        string recognizedText = recognizedPage.GetText();
                                        StrBuildDoc.Append(recognizedText);
                                    }
                                }

                                File.Delete(pageImage);
                            }
                        }
                    }

                    using (var writer = new StreamWriter("result.txt"))
                    {
                        writer.Write(StrBuildDoc.ToString());
                        richTextBox3.Text = StrBuildDoc.ToString();
                    }
                }
                else if (filepath.Substring(filepath.Length - 4) == ".png" || filepath.Substring(filepath.Length - 4) == ".jpg")
                {
                    try
                    {
                        var ocrengine = new TesseractEngine(@"tessdata", "rus", EngineMode.LstmOnly);
                        var img = Pix.LoadFromFile(filepath);
                        var res = ocrengine.Process(img);
                        richTextBox3.Text = res.GetText();
                        //pictureBox1.Image = Image.FromFile(filepath);
                        //Bitmap save_img = new Bitmap(filepath);
                        Image img_save = Image.FromFile(filepath);
                        img_save.Save($"C:/Users/jigul/Desktop/Диплом/scaning_page/page_0.png");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ошибка при выборе документа");
                    }
                }
                else
                {
                    MessageBox.Show("Неверный формат файла.");
                }

                string prikaz = @"^\s*[П]\s*[Р]\s*[И]\s*[К]\s*[А]\s*[З]\s*$";
                string inform = @"^\s*[И]\s*[Н]\s*[Ф]\s*[О]\s*[Р]\s*[М]\s*[А]\s*[Ц]\s*[И]\s*[О]\s*[Н]{2}\s*[О]\s*[Е]\s*[П]\s*[И]\s*[С]\s*[Ь]\s*[М]\s*[О]\s*$";
                string sluzebka = @"^\s*[С]\s*[Л]\s*[У]\s*[Ж]\s*[Е]\s*[Б]\s*[Н]\s*[А]\s*[Я]\s*[З]\s*[А]\s*[П]\s*[И]\s*[С]\s*[К]\s*[А]$";

                for (int i = 0; i < richTextBox3.Lines.Count(); i++)
                {
                    if (Regex.IsMatch(richTextBox3.Lines[i], sluzebka))
                    {
                        textBox4.Text = richTextBox3.Lines[i];
                        break;
                    }
                    else if (Regex.IsMatch(richTextBox3.Lines[i], prikaz))
                    {
                        textBox4.Text = richTextBox3.Lines[i];
                        break;
                    }
                    else if (Regex.IsMatch(richTextBox3.Lines[i], inform))
                    {
                        textBox4.Text = richTextBox3.Lines[i];
                        break;
                    }
                }

                if (Regex.IsMatch(textBox4.Text, prikaz))
                {
                    string regul_all_prik = @"^\D[0-9]{2}\D\s[а-я]+\s[0-9]{4}\s[г]?[.]?\s+[№]\s\S+\s*$";
                    string regul_date_prik = @"^\D[0-9]{2}\D\s[а-я]+\s[0-9]{4}$";
                    int k = 0;

                    while (!Regex.IsMatch(richTextBox3.Lines[k], regul_all_prik, RegexOptions.IgnoreCase))
                    {
                        k++;
                        if (Regex.IsMatch(richTextBox3.Lines[k], regul_all_prik, RegexOptions.IgnoreCase))
                        {
                            break;
                        }
                    }
                    string dateDoc_else_line = richTextBox3.Lines[k];
                    if (Regex.IsMatch(dateDoc_else_line, regul_all_prik, RegexOptions.IgnoreCase))
                    {
                        string[] dateDoc_else1 = dateDoc_else_line.Split(' ');
                        string dateDoc_elseall = dateDoc_else1[0] + " " + dateDoc_else1[1] + " " + dateDoc_else1[2];
                        if (Regex.IsMatch(dateDoc_elseall, regul_date_prik, RegexOptions.IgnoreCase))
                        {
                            textBox1.Text = dateDoc_elseall;
                            textBox2.Text = dateDoc_else_line.Split().Last();
                        }
                        else
                        {
                            MessageBox.Show("Поля не соответствуют формату, пожалуйста, введите вручную.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Поля не соответствуют формату, пожалуйста, введите вручную.");
                    }
                }
                else if (Regex.IsMatch(textBox4.Text, sluzebka))
                {
                    string regul_all_sluz = @"^\s*[0-9]{2}[.][0-9]{2}[.][0-9]{4}\s*[г]?[.]?\s+[№]\s\S+\s*$";
                    string regul_date_sluz = @"^\s*[0-9]{2}[.][0-9]{2}[.][0-9]{4}$";
                    string regul_fio_sluz1 = @"^[А-ЯЁ][а-яё]*\s+[А-ЯЁ][.][А-ЯЁ][.]\s*$";
                    string regul_fio_sluz2 = @"^[А-ЯЁ][.][А-ЯЁ][.]\s+[А-ЯЁ][а-яё]*\s*$";
                    int k = 0;

                    while (!Regex.IsMatch(richTextBox3.Lines[k], regul_all_sluz, RegexOptions.IgnoreCase))
                    {
                        k++;
                        if (Regex.IsMatch(richTextBox3.Lines[k], regul_all_sluz, RegexOptions.IgnoreCase))
                        {
                            break;
                        }
                    }

                    string dateDoc_else_line = richTextBox3.Lines[k];

                    if (Regex.IsMatch(dateDoc_else_line, regul_all_sluz, RegexOptions.IgnoreCase))
                    {
                        string[] dateDoc_else1 = dateDoc_else_line.Split(' ');
                        dateDoc_else1 = dateDoc_else1.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                        string dateDoc_elseall = dateDoc_else1[0];

                        if (Regex.IsMatch(dateDoc_elseall, regul_date_sluz, RegexOptions.IgnoreCase))
                        {
                            textBox1.Text = dateDoc_elseall;
                            textBox2.Text = dateDoc_else_line.Split().Last();
                        }
                        else
                        {
                            MessageBox.Show("Поля не соответствуют формату, пожалуйста, введите вручную.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Поля не соответствуют формату, пожалуйста, введите вручную.");
                    }

                    while ((!Regex.IsMatch(richTextBox3.Lines[k], regul_fio_sluz1, RegexOptions.IgnoreCase)) || (!Regex.IsMatch(richTextBox3.Lines[k], regul_fio_sluz2, RegexOptions.IgnoreCase)))
                    {
                        k++;
                        if (Regex.IsMatch(richTextBox3.Lines[k], regul_fio_sluz1, RegexOptions.IgnoreCase) || Regex.IsMatch(richTextBox3.Lines[k], regul_fio_sluz2, RegexOptions.IgnoreCase))
                        {
                            break;
                        }
                    }
                    textBox3.Text = richTextBox3.Lines[k];
                }
                else if (Regex.IsMatch(textBox4.Text, inform))
                {
                    string regul_all_inf = @"^\D[0-9]{2}\D\s[а-я]+\s[0-9]{4}\s[г]?[.]?\s+[№]\s\S+\s*$";
                    string regul_date_inf = @"^\D[0-9]{2}\D\s[а-я]+\s[0-9]{4}$";
                    string regul_date_inf1 = @"^\s*[0-9]{2}[.][0-9]{2}[.][0-9]{4}$";
                    int k = 0;

                    try
                    {
                        while (!Regex.IsMatch(richTextBox3.Lines[k], regul_all_inf, RegexOptions.IgnoreCase))
                        {
                            k++;
                            
                            if (Regex.IsMatch(richTextBox3.Lines[k], regul_all_inf, RegexOptions.IgnoreCase))
                            {
                                break;
                            }

                        }
                    
                    string dateDoc_else_line = richTextBox3.Lines[k];
                    if (Regex.IsMatch(dateDoc_else_line, regul_all_inf, RegexOptions.IgnoreCase))
                    {
                        string[] dateDoc_else1 = dateDoc_else_line.Split(' ');
                        string dateDoc_elseall = dateDoc_else1[0] + " " + dateDoc_else1[1] + " " + dateDoc_else1[2];

                        if (Regex.IsMatch(dateDoc_elseall, regul_date_inf, RegexOptions.IgnoreCase) || Regex.IsMatch(dateDoc_elseall, regul_date_inf1, RegexOptions.IgnoreCase))
                        {
                            textBox1.Text = dateDoc_elseall;
                            textBox2.Text = dateDoc_else_line.Split().Last();
                        }
                        else
                        {
                            MessageBox.Show("Поля не соответствуют формату, пожалуйста, введите вручную.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Поля не соответствуют формату, пожалуйста, введите вручную.");
                    }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Несоотвествие формату строки, пожалуйста, введите вручную");
                    }
                }
                else
                {
                    MessageBox.Show("Документ не определен, пожалуйста, введите поля вручную");
                }

            }
            else
            {
                MessageBox.Show("Выберите файл");
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Text = "Приказ";
            com.CommandText = "select * from prikaz";
            Db_Fill(com);
            open = true;

        }
        private void Db_Fill(NpgsqlCommand com)
        {
            try
            {
                nda = new NpgsqlDataAdapter(com);
                dt = new DataTable();
                nda.Fill(dt);
                dataGridView1.DataSource = dt;

            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось подключиться к базе данных!");
                dataGridView1.Hide();
            }
        }
        private void SelectTable()
        {
            if (comboBox1.Text == "Приказ")
            {
                NpgsqlCommand ins_id1 = new NpgsqlCommand("select setval('prikaz_id_pr_seq', (select max(id_pr) from prikaz));", conn);
                ins_id1.ExecuteScalar();

                com.CommandText = "select * from prikaz";
                Db_Fill(com);
            }
            if (comboBox1.Text == "Служебная записка")
            {
                NpgsqlCommand ins_id2 = new NpgsqlCommand("select setval('sluzebka_id_sl_seq', (select max(id_sl) from sluzebka));", conn);
                ins_id2.ExecuteScalar();

                com.CommandText = "select * from sluzebka";
                Db_Fill(com);
            }
            if (comboBox1.Text == "Информационное письмо")
            {
                NpgsqlCommand ins_id3 = new NpgsqlCommand("select setval('sluzebka_id_sl_seq', (select max(id_sl) from sluzebka));", conn);
                ins_id3.ExecuteScalar();

                com.CommandText = "select * from inform";
                Db_Fill(com);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectTable();
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = (checkBox1.CheckState == CheckState.Checked);
        }

        private void checkBox3_CheckStateChanged(object sender, EventArgs e)
        {
            textBox3.Enabled = (checkBox3.CheckState == CheckState.Checked);
        }

        private void checkBox2_CheckStateChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = (checkBox2.CheckState == CheckState.Checked);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 newForm2 = new Form2();
            newForm2.Show();
            this.Hide();
        }

        public void button4_Click(object sender, EventArgs e)
        {
            if (richTextBox1.TextLength != 0 && richTextBox1.TextLength != 0 && textBox1.TextLength != 0 && textBox2.TextLength != 0 && textBox4.TextLength != 0) {
                if (ButtonClick)
                {
                    label8.Show();
                    ButtonClick = false;
                }
                else
                {
                    try
                    { 
                        DateTime datetime = DateTime.Now;
                        string date = textBox1.Text;
                        string index = textBox2.Text;
                        string whom = textBox3.Text;
                        string text = richTextBox1.Text;
                        string after_text = richTextBox2.Text;
                        string date_insert = datetime.ToString("dd.MM.yyyy");
                        NpgsqlCommand com1 = new NpgsqlCommand("select fio from users where login= '" + Form2.login + "'", conn);
                        string docved = ((string)com1.ExecuteScalar());
                        NpgsqlCommand com_pr_id = new NpgsqlCommand("select setval('prikaz_id_pr_seq', (select max(id_pr) from prikaz));", conn);
                        int id_pr = Convert.ToInt32(com_pr_id.ExecuteScalar()) + 1;
                        NpgsqlCommand com_sl_id = new NpgsqlCommand("select setval('sluzebka_id_sl_seq', (select max(id_sl) from sluzebka));", conn);
                        int id_sl = Convert.ToInt32(com_sl_id.ExecuteScalar()) + 1;
                        NpgsqlCommand com_inf_id = new NpgsqlCommand("select setval('inform_id_inf_seq', (select max(id_inf) from inform));", conn);
                        int id_inf = Convert.ToInt32(com_inf_id.ExecuteScalar()) + 1;


                        string prikaz = @"^\s*[П]\s*[Р]\s*[И]\s*[К]\s*[А]\s*[З]\s*$";
                        string inform = @"^\s*[И]\s*[Н]\s*[Ф]\s*[О]\s*[Р]\s*[М]\s*[А]\s*[Ц]\s*[И]\s*[О]\s*[Н]{2}\s*[О]\s*[Е]\s*[П]\s*[И]\s*[С]\s*[Ь]\s*[М]\s*[О]\s*$";
                        string sluzebka = @"^\s*[С]\s*[Л]\s*[У]\s*[Ж]\s*[Е]\s*[Б]\s*[Н]\s*[А]\s*[Я]\s*[З]\s*[А]\s*[П]\s*[И]\s*[С]\s*[К]\s*[А]$";

                        if (Regex.IsMatch(textBox4.Text, prikaz))
                        {
                            try
                            {
                                var com_pr = new NpgsqlCommand("insert into prikaz (id_pr, date_pr, index_pr, text_pr, after_text_pr, date_insert, fio) values(@id_pr,@date_pr,@index_pr,@text_pr,@after_text_pr,@date_insert,@fio)", conn);
                                com_pr.Parameters.AddWithValue("@id_pr", id_pr);
                                com_pr.Parameters.AddWithValue("@date_pr", date);
                                com_pr.Parameters.AddWithValue("@index_pr", index);
                                com_pr.Parameters.AddWithValue("@text_pr", text);
                                com_pr.Parameters.AddWithValue("@after_text_pr", after_text);
                                com_pr.Parameters.AddWithValue("@date_insert", date_insert);
                                com_pr.Parameters.AddWithValue("@fio", docved);
                                com_pr.ExecuteNonQuery();
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Ошибка заполнения");
                            }
                        }
                        else if (Regex.IsMatch(textBox4.Text, sluzebka))
                        {
                            try
                            {
                                var com_sl = new NpgsqlCommand("insert into sluzebka (id_sl, date_sl, index_sl, whom_sl, text_sl, after_text_sl, date_insert, fio) values(@id_sl,@date_sl,@index_sl,@whom_sl,@text_sl,@after_text_sl,@date_insert,@fio)", conn);
                                com_sl.Parameters.AddWithValue("@id_sl", id_sl);
                                com_sl.Parameters.AddWithValue("@date_sl", date);
                                com_sl.Parameters.AddWithValue("@index_sl", index);
                                com_sl.Parameters.AddWithValue("@whom_sl", whom);
                                com_sl.Parameters.AddWithValue("@text_sl", text);
                                com_sl.Parameters.AddWithValue("@after_text_sl", after_text);
                                com_sl.Parameters.AddWithValue("@date_insert", date_insert);
                                com_sl.Parameters.AddWithValue("@fio", docved);
                                com_sl.ExecuteNonQuery();
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Ошибка заполнения");
                            }
                        }
                        else if (Regex.IsMatch(textBox4.Text, inform))
                        {
                            try
                            {
                                var com_inf = new NpgsqlCommand("insert into inform (id_inf, date_inf, index_inf, text_inf, after_text_inf, date_insert, fio) values(@id_inf,@date_inf,@index_inf,@text_inf,@after_text_inf,@date_insert,@fio)", conn);
                                com_inf.Parameters.AddWithValue("@id_inf", id_inf);
                                com_inf.Parameters.AddWithValue("@date_inf", date);
                                com_inf.Parameters.AddWithValue("@index_inf", index);
                                com_inf.Parameters.AddWithValue("@text_inf", text);
                                com_inf.Parameters.AddWithValue("@after_text_inf", after_text);
                                com_inf.Parameters.AddWithValue("@date_insert", date_insert);
                                com_inf.Parameters.AddWithValue("@fio", docved);
                                com_inf.ExecuteNonQuery();
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Ошибка заполнения");
                            }
                        }
                        label8.Hide();
                        ButtonClick = true;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Неверный формат данных в таблице");
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните поля, обязательные для заполнения!");
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable DT = (DataTable)dataGridView1.DataSource;
                if (DT != null)
                    DT.Clear();
                SelectTable();

                dataGridView1.Show();
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                richTextBox1.Clear();
                richTextBox2.Clear();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (ButtonClickImage)
                {
                    ButtonClickImage = false;
                    int numberPage = 0;
                    numberPage = Convert.ToInt32(textBox5.Text) - 1;
                    Bitmap mem_img = new Bitmap($"C:/Users/jigul/Desktop/Диплом/scaning_page/page_{numberPage}.png");
                    //Image mem_img = Image.FromFile($"C:/Users/jigul/Desktop/Диплом/scaning_page/page_{numberPage}.png");
                    mem_img.Save($"C:/Users/jigul/Desktop/Диплом/scaning_page/mem_{numberPage}.png");
                    mem_img.Dispose();
                    Bitmap mem_img1 = new Bitmap($"C:/Users/jigul/Desktop/Диплом/scaning_page/mem_{numberPage}.png");
                    //Image mem_img1 = Image.FromFile($"C:/Users/jigul/Desktop/Диплом/scaning_page/mem_{numberPage}.png");
                    pictureBox1.Image = mem_img1;
                    panel1.Controls.Add(pictureBox1);
                    panel1.BringToFront();

                }
                else
                {
                    ButtonClickImage = true;
                    panel1.SendToBack();
                    //panel1.Controls.Clear();
                    pictureBox1.Image.Dispose();
                    textBox5.Clear();
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
