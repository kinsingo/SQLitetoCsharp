using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace MySqlCRUD
{
    public partial class Form1 : Form
    {
        string connectionString = "Data Source=" + Directory.GetCurrentDirectory() + @"\sqlite_bookdb.db;" + " Version=3;";
        int bookID = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Clear();
            DGVFill();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection sqliteCon = new SQLiteConnection(connectionString))
            {
                sqliteCon.Open();
                string cmd = @"INSERT INTO table1(BookName, Author, Description) 
                               VALUES(@_BookName,@_Author,@_Description);";
                //string cmd = $"INSERT INTO table1(BookID, BookName, Author, Description) VALUES(0, 123, 34534,574);";
                SQLiteCommand sqlitecmd = new SQLiteCommand(cmd, sqliteCon);
                sqlitecmd.CommandType = CommandType.Text;
                sqlitecmd.Parameters.AddWithValue("@_BookName", txtBookName.Text.Trim());
                sqlitecmd.Parameters.AddWithValue("@_Author", txtAuthor.Text.Trim());
                sqlitecmd.Parameters.AddWithValue("@_Description", txtDescription.Text.Trim());
                sqlitecmd.ExecuteNonQuery();
                MessageBox.Show("Submitted Successfully");
                Clear();
                DGVFill();
            }
        }

        private void DGVFill()
        {
            using (SQLiteConnection sqliteCon = new SQLiteConnection(connectionString))
            {
                sqliteCon.Open();
                SQLiteDataAdapter sqlDA = new SQLiteDataAdapter("SELECT * FROM table1", sqliteCon);
                sqlDA.SelectCommand.CommandType = CommandType.Text;
                DataTable datatable = new DataTable();
                sqlDA.Fill(datatable);
                dgvBook.DataSource = datatable;
            }
        }

        private void Clear()
        {
            txtBookName.Clear();
            txtAuthor.Clear();
            txtDescription.Clear();
            txtSearch.Clear();
            txt_BookID.Clear();
            bookID = 0;
            btnSave.Text = "Save";
        }

        private void dgvBook_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvBook.CurrentRow.Index != -1)
            {
                bookID = Convert.ToInt32(dgvBook.CurrentRow.Cells[0].Value.ToString());
                txtBookName.Text = dgvBook.CurrentRow.Cells[1].Value.ToString();
                txtAuthor.Text = dgvBook.CurrentRow.Cells[2].Value.ToString();
                txtDescription.Text = dgvBook.CurrentRow.Cells[3].Value.ToString();
                txt_BookID.Text = bookID.ToString();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection sqliteCon = new SQLiteConnection(connectionString))
            {
                sqliteCon.Open();

                SQLiteDataAdapter sqlDA = new SQLiteDataAdapter("SELECT * FROM table1 WHERE BookName LIKE @_BookName OR Author Like @_Author; ", sqliteCon);
                sqlDA.SelectCommand.CommandType = CommandType.Text;
                sqlDA.SelectCommand.Parameters.AddWithValue("@_BookName", $"%{txtSearch.Text}%");
                sqlDA.SelectCommand.Parameters.AddWithValue("@_Author", $"%{txtSearch.Text}%");
                DataTable datatable = new DataTable();
                sqlDA.Fill(datatable);
                dgvBook.DataSource = datatable;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(txt_BookID.Text != "")
            {
                using (SQLiteConnection sqliteCon = new SQLiteConnection(connectionString))
                {
                    sqliteCon.Open();
                    string cmd = @"DELETE FROM 
                                      table1 
                                   WHERE 
                                      BookID = @_BookID;";

                    SQLiteCommand sqlitecmd = new SQLiteCommand(cmd, sqliteCon);
                    sqlitecmd.CommandType = CommandType.Text;
                    sqlitecmd.Parameters.AddWithValue("@_BookName", txtBookName.Text.Trim());
                    sqlitecmd.Parameters.AddWithValue("@_Author", txtAuthor.Text.Trim());
                    sqlitecmd.Parameters.AddWithValue("@_Description", txtDescription.Text.Trim());
                    sqlitecmd.Parameters.AddWithValue("@_BookID", txt_BookID.Text.Trim());
                    sqlitecmd.ExecuteNonQuery();
                    MessageBox.Show("Deleted Successfully");
                    Clear();
                    DGVFill();
                }
            }
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            if (txt_BookID.Text != "")
            {
                using (SQLiteConnection sqliteCon = new SQLiteConnection(connectionString))
                {
                    sqliteCon.Open();
                    string cmd = @"UPDATE 
                                      table1 
                                   SET 
                                      BookName = @_BookName,
                                      Author = @_Author,
                                      Description = @_Description 
                                   WHERE 
                                      BookID = @_BookID;";
                  
                    SQLiteCommand sqlitecmd = new SQLiteCommand(cmd, sqliteCon);
                    sqlitecmd.CommandType = CommandType.Text;
                    sqlitecmd.Parameters.AddWithValue("@_BookName", txtBookName.Text.Trim());
                    sqlitecmd.Parameters.AddWithValue("@_Author", txtAuthor.Text.Trim());
                    sqlitecmd.Parameters.AddWithValue("@_Description", txtDescription.Text.Trim());
                    sqlitecmd.Parameters.AddWithValue("@_BookID", txt_BookID.Text.Trim());
                    sqlitecmd.ExecuteNonQuery();
                    MessageBox.Show("Updated Successfully");
                    Clear();
                    DGVFill();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sql_cmd = "CREATE TABLE " + textBox_TableName.Text +
               @"(BookID INTEGER NOT NULL,
               BookName TEXT,
               Author TEXT,
              Description TEXT,
              PRIMARY KEY(BookID AUTOINCREMENT))";

            using (SQLiteConnection sqliteCon = new SQLiteConnection(connectionString))
            {
                sqliteCon.Open();
                SQLiteCommand sqlitecmd = new SQLiteCommand(sql_cmd.ToString(), sqliteCon);
                sqlitecmd.CommandType = CommandType.Text;
                sqlitecmd.ExecuteNonQuery();
                MessageBox.Show(textBox_TableName.Text + " has been created");
            }
        }

        private void button_Show_Tables_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection sqliteCon = new SQLiteConnection(connectionString))
            {
                sqliteCon.Open();
               
                //List<string> QueryResult = new List<string>();
                string ans = "";

                SQLiteCommand mysqlcmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type = 'table'", sqliteCon);
                SQLiteDataReader reader = mysqlcmd.ExecuteReader();
                while (reader.Read())
                {
                    //QueryResult.Add(reader.GetString(0));
                    ans += (reader.GetString(0) + "\n");

                }
                reader.Close();

                MessageBox.Show(ans);
            }


             
        }

        private void btn_Show_All_Click(object sender, EventArgs e)
        {
            Clear();
            DGVFill();
        }
    }
}
