using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Server.API;
using Brain;

namespace WindowsClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PlayerPlugin[] plugIns = PlayerFactory.GetPlayerPlugIns();
            lst_Types1.DataSource = plugIns;
            lst_Types1.DisplayMember = "Name";
            lst_Types1.ValueMember = "ID";
            lst_Types1.SelectedIndex = 0;
            lst_Types2.DataSource = plugIns.Clone();
            lst_Types2.DisplayMember = "Name";
            lst_Types2.ValueMember = "ID";
            lst_Types2.SelectedIndex = 0;
            lst_Types3.DataSource = plugIns.Clone();
            lst_Types3.DisplayMember = "Name";
            lst_Types3.ValueMember = "ID";
            lst_Types3.SelectedIndex = 0;
            lst_Types4.DataSource = plugIns.Clone();
            lst_Types4.DisplayMember = "Name";
            lst_Types4.ValueMember = "ID";
            lst_Types4.SelectedIndex = 0;

            lst_Rounds.SelectedIndex = 3;
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            string[] plugIns = new string[4];
            plugIns[0] = (string)lst_Types1.SelectedValue;
            plugIns[1] = (string)lst_Types2.SelectedValue;
            plugIns[2] = (string)lst_Types3.SelectedValue;
            plugIns[3] = (string)lst_Types4.SelectedValue;
            int num_of_rounds = int.Parse((string)lst_Rounds.SelectedItem);
            GameTable gameTable = new GameTable(num_of_rounds);
            gameTable.Show();
            try
            {
                //GameFactory.CreateGame(new GameLogger(@"C:\MyFiles\University Files\ביואינפורמטיקה - שנה ג'\סמסטר א\אינטליגנציה משחקית\cs-whist-2010\Server\logfile.txt"), plugIns, int.Parse((string)lst_Rounds.SelectedItem), 20);
                GameFactory.CreateGame(gameTable, plugIns, num_of_rounds, 0);
                //GameFactory.CreateGame(null, plugIns, int.Parse((string)lst_Rounds.SelectedItem), 20);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
