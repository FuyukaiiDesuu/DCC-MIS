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
    public partial class formTransactionEdit : Form
    {
        int selectedProduct = -1;

        public formTransactionEdit(int x, string y, string z)
        {
            InitializeComponent();

            selectedProduct = x;

            label1.Text = y;

            if(z == "Non-consumable")
            {
                comboBoxProductStatus.Items.Clear();
                comboBoxProductStatus.Items.Add("Normal");
                comboBoxProductStatus.Items.Add("Under Repair");
                comboBoxProductStatus.Items.Add("Damaged");
            }

            comboBoxProductStatus.Text = "Damaged";
        }

        private void buttonInventoryEncode_Click(object sender, EventArgs e)
        {
            using (var con = new MySqlConnection(conClass.connectionString))
            {
                con.Open();
                using (var com = new MySqlCommand("UPDATE inventory SET status = @status WHERE id = @id", con))
                {
                    com.Parameters.AddWithValue("@status", comboBoxProductStatus.Text);
                    com.Parameters.AddWithValue("@id", selectedProduct);

                    com.ExecuteNonQuery();
                }
                con.Close();
            }
        }
    }
}
