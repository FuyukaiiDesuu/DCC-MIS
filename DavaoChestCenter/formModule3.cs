﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DavaoChestCenter
{
    public partial class formModule3 : Form
    {
        Dictionary<int, string> staff = new Dictionary<int, string>();

        int selectedStaff = -1;
        int selectedSchedule = -1;

        public formModule3()
        {
            InitializeComponent();

            refreshTable();

            gatherPeople();
        }

        public void refreshTable()
        {
            using (var con = new MySqlConnection(conClass.connectionString))
            {
                con.Open();
                using (var com = new MySqlCommand("SELECT id, firstname, middlename, lastname, schedule_days, working_time_start, working_time_end FROM staff RIGHT JOIN schedules ON staff.id = schedules.staff_id", con))
                {
                    var adp = new MySqlDataAdapter(com);
                    var dt = new DataTable();
                    adp.Fill(dt);
                    dataGridViewSchedule.DataSource = dt;

                    dataGridViewSchedule.Columns["id"].Visible = false;
                }

                using (var com = new MySqlCommand("SELECT attendance.id, firstname, lastname, time_start, time_end FROM attendance LEFT JOIN staff ON attendance.person = staff.id WHERE date > DATE_SUB(NOW(), INTERVAL 1 DAY)", con))
                {
                    var adp = new MySqlDataAdapter(com);
                    var dt = new DataTable();
                    adp.Fill(dt);
                    dataGridViewToday.DataSource = dt;

                    dataGridViewToday.Columns["id"].Visible = false;
                }
                con.Close();
            }
        }

        private void timerCurrent_Tick(object sender, EventArgs e)
        {
            label1.Text = System.DateTime.Now.ToString("dddd, MMMM/dd/yyyy HH:mm:ss");
        }

        private void dataGridViewSchedule_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedStaff = int.Parse(dataGridViewSchedule.Rows[e.RowIndex].Cells["id"].Value.ToString());
        }

        private void buttonCheckIn_Click(object sender, EventArgs e)
        {
            if (selectedStaff != -1)
            {
                using (var con = new MySqlConnection(conClass.connectionString))
                {
                    con.Open();
                    using (var com = new MySqlCommand("INSERT INTO attendance VALUES(null, @person, @date, @time_start, '')", con))
                    {
                        com.Parameters.AddWithValue("@person", selectedStaff);
                        com.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                        com.Parameters.AddWithValue("@time_start", DateTime.Now.ToString("HH:mm:ss"));

                        com.ExecuteNonQuery();

                        refreshTable();
                    }
                    con.Close();
                }
            }
            else
            {
                MessageBox.Show("select user first");
            }
        }

        private void buttonCheckOut_Click(object sender, EventArgs e)
        {
            if (selectedSchedule != -1)
            {
                using (var con = new MySqlConnection(conClass.connectionString))
                {
                    con.Open();
                    using (var com = new MySqlCommand("UPDATE attendance SET time_end = @time_end WHERE id = @id", con))
                    {
                        com.Parameters.AddWithValue("@time_end", DateTime.Now.ToString("HH:mm:ss"));
                        com.Parameters.AddWithValue("@id", selectedSchedule);

                        com.ExecuteNonQuery();

                        refreshTable();

                        sort();
                    }
                    con.Close();
                }
            }
            else
            {
                MessageBox.Show("select schedule first");
            }
        }

        private void gatherPeople()
        {
            using (var con = new MySqlConnection(conClass.connectionString))
            {
                con.Open();
                using (var com = new MySqlCommand("SELECT * FROM staff", con))
                {
                    using (var rdr = com.ExecuteReader())
                    {
                        while (rdr.HasRows)
                        {
                            if (rdr.Read())
                            {
                                staff.Add(rdr.GetInt32(0), rdr.GetString(1) + " " + rdr.GetString(3));

                                comboBox1.DataSource = new BindingSource(staff, null);
                                comboBox1.DisplayMember = "Value";
                                comboBox1.ValueMember = "Key";
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                con.Close();
            }
        }

        private void dataGridViewToday_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedSchedule = int.Parse(dataGridViewToday.Rows[e.RowIndex].Cells["id"].Value.ToString());
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            sort();
        }

        public void sort()
        {
            using (var con = new MySqlConnection(conClass.connectionString))
            {
                con.Open();
                if (checkBox1.Checked == false)
                {
                    using (var com = new MySqlCommand("SELECT attendance.id, date, firstname, lastname, time_start, time_end FROM attendance LEFT JOIN staff ON attendance.person = staff.id WHERE (date BETWEEN @dateTimePicker1 AND @dateTimePicker2) AND staff.id = @staff_id", con))
                    {
                        com.Parameters.AddWithValue("@dateTimePicker1", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                        com.Parameters.AddWithValue("@dateTimePicker2", dateTimePicker2.Value.ToString("yyyy-MM-dd"));
                        com.Parameters.AddWithValue("@staff_id", ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key);

                        var adp = new MySqlDataAdapter(com);
                        var dt = new DataTable();
                        adp.Fill(dt);
                        dataGridViewAttendance.DataSource = dt;

                        dataGridViewAttendance.Columns["id"].Visible = false;
                    }
                }
                else
                {
                    using (var com = new MySqlCommand("SELECT attendance.id, date, firstname, lastname, time_start, time_end FROM attendance LEFT JOIN staff ON attendance.person = staff.id WHERE (date BETWEEN @dateTimePicker1 AND @dateTimePicker2)", con))
                    {
                        com.Parameters.AddWithValue("@dateTimePicker1", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                        com.Parameters.AddWithValue("@dateTimePicker2", dateTimePicker2.Value.ToString("yyyy-MM-dd"));

                        var adp = new MySqlDataAdapter(com);
                        var dt = new DataTable();
                        adp.Fill(dt);
                        dataGridViewAttendance.DataSource = dt;

                        dataGridViewAttendance.Columns["id"].Visible = false;
                    }
                }
                con.Close();
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            sort();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            sort();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            sort();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            sort();
        }
    }
}
