using Hydra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using cdn_api;

namespace OneS0121_BlokadaMacierzy
{
    public partial class Confirm : Form
    {
        public Confirm()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pl-PL");
            Form1.ConnectToSQL(Form1.connection_string, "delete from cdn.ONE_ZamMacierz where (GIDTyp =" + Form1.gidtyp + " and GIDNumer =" + Form1.gidnumer + " and GIDLp =" + Form1.gidlp + ")");
            string query = "insert into cdn.One_ZamMacierz (GIDTyp, GIDNumer, GIDLp, GIDSubLp, Ilosc, Dlugosc) values";
            int counter = 0;
            foreach (Form1.Newview item in Form1.info)
            {

                query += " (" + Form1.gidtyp + ", " + Form1.gidnumer + ", " + Form1.gidlp + ", " + counter + ", " + item.Ilosc.Replace(",", ".") + ", " + item.Dlugosc.Replace(",", ".") + "),";

                counter++;

            }
            query = query.Remove(query.Length - 1, 1) + ";";
            //MessageBox.Show(query);
            Form1.ConnectToSQL(Form1.connection_string, query);
            Form1.ConnectToSQL(Form1.connection_string, $"update CDN.ZamElem set ZaE_Ilosc = (ZaE_PrzeliczL/ZaE_PrzeliczM)*{Form1.result.ToString().Replace(",", ".")}, ZaE where ZaE_GIDNumer = " + Form1.gidnumer + " and ZaE_GIDTyp = " + Form1.gidtyp + " and ZaE_GIDLp = " + Form1.gidlp);
            Button_Handle button_Handle = new Button_Handle();
            button_Handle.UpdateValue(Form1.result);
            MessageBox.Show($"Łączna wartość to {Form1.result}");
            //button_Handle.UpdateValue(Form1.result.ToString());
            Application.OpenForms["Form1"].Close();
            this.Close();
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Confirm_FormClosing(object sender, FormClosingEventArgs e)
        {


            
        }
    }
}
