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
    public partial class Confirm2 : Form
    {
        public Confirm2()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Form2 form1 = new Form2();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pl-PL");
            Form2.ConnectToSQL(Form2.connection_string, "delete from cdn.ONE_ZamMacierz where (GIDTyp =" + Form2.gidtyp + " and GIDNumer =" + Form2.gidnumer + " and GIDLp =" + Form2.gidlp + ")");
            string query = "insert into cdn.One_ZamMacierz (GIDTyp, GIDNumer, GIDLp, GIDSubLp, Ilosc, Dlugosc) values";
            int counter = 0;
            foreach (Form2.Newview item in Form2.info)
            {

                query += " (" + Form2.gidtyp + ", " + Form2.gidnumer + ", " + Form2.gidlp + ", " + counter + ", " + item.Ilosc.Replace(",", ".") + ", " + item.Dlugosc.Replace(",", ".") + "),";

                counter++;

            }
            query = query.Remove(query.Length - 1, 1) + ";";
            //MessageBox.Show(query);
            Form2.ConnectToSQL(Form2.connection_string, query);
            Form2.ConnectToSQL(Form2.connection_string, $"update CDN.TraElem set TrE_Ilosc = (TrE_PrzeliczL/TrE_PrzeliczM)*{Form2.result.ToString().Replace(",", ".")} where TrE_GIDNumer = " + Form2.gidnumer + " and TrE_GIDTyp = " + Form2.gidtyp + " and TrE_GIDLp = " + Form2.gidlp);
            //MessageBox.Show($"update CDN.TraElem set TrE_Ilosc = (TrE_PrzeliczL/ZaE_PrzeliczM)*{Form2.result.ToString().Replace(",", ".")}, TrE where ZaE_GIDNumer = " + Form2.gidnumer + " and TrE_GIDTyp = " + Form2.gidtyp + " and TrE_GIDLp = " + Form2.gidlp);
            Button_Handle button_Handle = new Button_Handle();
            button_Handle.UpdateValue(Form2.result);
            MessageBox.Show($"Łączna wartość to {Form2.result}");
            //button_Handle.UpdateValue(Form2.result.ToString());
            Application.OpenForms["Form2"].Close();
            this.Close();
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Confirm2_FormClosing(object sender, FormClosingEventArgs e)
        {


            
        }
    }
}
