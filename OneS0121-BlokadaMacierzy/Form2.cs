using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydra;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using cdn_api;

namespace OneS0121_BlokadaMacierzy
{
    public partial class Form2 : Form
    {
        public partial class Newview
        {
            public string Dlugosc { get; set; }
            public string Ilosc { get; set; }
        }

        public static string connection_string;
        //public static string XLSQLServer = "192.168.137.149";
        // XLSQLDatabase = "rpxl_2020_2"; //"Domar3";
        string resp, query, subquery;
        string initial_length;
        public static double result = 0;
        public static int row_amount = 0;
        private int sesja = -1;
        public static List<Newview> info = new List<Newview>();
        public static long gidtyp, gidnumer, gidtwr, gidlp;
        public static long twrtyp, twrnumer, twrlp;



        private string XLDatabase = "ERPXL_extra_23";//"ERPXL_extraDach";//"rpxl_2020_2";// "Domar2";
        private string XLUser = "Sabisup"; //"ADMIN";
        private string XLPass = "#0ut2510gro0ne.34";//"Dupa2003!";
        
        /*
        private string XLDatabase = "Domar2";//"rpxl_2020_2";// "Domar2";
        private string XLUser = "OS_AUTOM";//"Sabisup"; //"ADMIN";
        private string XLPass = "DomXL#53&";//"#0ut2510gro0ne.34";//"Dupa2003!";
        */

        TextBox editBox = null;
        public Form2()
        {
            Button_Handle button_Handle = new Button_Handle();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pl-PL");
            connection_string = GetConnectionString();
            gidtyp = TraNag.TrN_GIDTyp;
            gidlp = TraElem.TrE_GIDLp;
            gidnumer = TraNag.TrN_GIDNumer;
            gidtwr = TraElem.TrE_GIDNumer;

            twrtyp = TwrKarty.Twr_GIDTyp;
            twrnumer = TwrKarty.Twr_GIDNumer;
            twrlp = TwrKarty.Twr_GIDLp;
            InitializeComponent();
           

            subquery = "(select AtK_ID from CDN.AtrybutyKlasy where AtK_Nazwa = 'Parametry x#y#z')";
            query = "if exists (select 1 from cdn.Atrybuty where Atr_ObiTyp =" + twrtyp + " AND Atr_ObiNumer = " + twrnumer + " AND Atr_ObiLp =" + twrlp + " AND Atr_ObiSubLp = 0 AND Atr_AtkId = " + subquery + ") begin "
                + "select Atr_Wartosc from cdn.Atrybuty where Atr_ObiTyp =" + twrtyp + " AND Atr_ObiNumer = " + twrnumer + " AND Atr_ObiLp =" + twrlp + " AND Atr_ObiSubLp = 0 AND Atr_AtkId = " + subquery + " end else begin select 'brak' end";
            resp = ConnectToSQL(connection_string, query);
            initial_length = ConnectToSQL(connection_string,"Select TrE_Ilosc from CDN.TraNag inner join CDN.TraElem  On TrN_GIDNumer = TrE_GIDNumer where " +
                "TrE_GIDNumer = " + gidnumer + "and TrE_GIDTyp =" + gidtyp + " and TrE_GIDLp = " + gidlp);
            LoadVariables(resp, query);
            FillDataGrid();
                if (TraNag.TrN_Stan >= 3)
                {
                    this.button1.Enabled = false;
                    this.dataGridView1.ReadOnly = true;
                }

        }
        public int XLLogin()
        {
            return cdn_api.cdn_api.XLLogin(new XLLoginInfo_20202()
            {
                Wersja = 20202,
                ProgramID = "ONES0121-SzczegolyIlosci",
                //PlikLog = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\api.log"),
                //PlikLog = (@"C:\OneSupport\ONE-S01_GEN_AWF\api.log"),
                Baza = XLDatabase,
                OpeIdent = XLUser,
                OpeHaslo = XLPass,
                TrybWsadowy = 1,
                //TrybNaprawy = 0,
                UtworzWlasnaSesje = 1,
                Winieta = -1,
            }, ref sesja);
        }
        public void Logout()
        {
            cdn_api.cdn_api.XLLogout(sesja);
        }
        public string GetConnectionString()
        {
            XLLogin();
            XLPolaczenieInfo_20202 polaczenieInfo = new XLPolaczenieInfo_20202 { Wersja = 20202 };
            cdn_api.cdn_api.XLPolaczenie(polaczenieInfo);

            //connection_string = @"Provider=MSOLEDBSQL.1;Password=43gQdfs31so3;Persist Security Info=True;User ID=onesupport_dod;Initial Catalog="";Data Source=SRV1;Initial File Name="";Server SPN="";Authentication="";Access Token=""";
            string connection_string = "Server=" + polaczenieInfo.Serwer + ";Database=" + polaczenieInfo.Baza + ";User Id=" + XLUser + ";Password=" + XLPass;
            //string connection_string = "Server=" + polaczenieInfo.Serwer + ";Database=" + polaczenieInfo.Baza + ";Integrated Security=SSPI;";

            Logout();
            return connection_string;
        }
            public void FillDataGrid()
        {
            //MessageBox.Show(ConnectToSQL(connection_string, "select count(*) from cdn.ONE_ZamMacierz where GIDTyp = " + gidtyp + " and GIDNumer = " + gidnumer + " and GIDlp = " + gidlp));
            this.dataGridView1.Rows.Clear();
            row_amount = row_amount = Int32.Parse(ConnectToSQL(connection_string, "select count(*) from cdn.ONE_ZamMacierz where GIDTyp = " + gidtyp + " and GIDNumer = " + gidnumer + " and GIDlp = " + gidlp));
            for (int i = 0; i < row_amount; i++)
            {
                var index = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells["Column1"].Value = String.Format("{0:G29}", decimal.Parse(ConnectToSQL(connection_string, "select ONE_ZamMacierz.Dlugosc from cdn.ONE_ZamMacierz where GIDTyp = " + gidtyp + " and GIDNumer = " + gidnumer + " and GIDlp = " + gidlp + "and GIDSubLp="+ i)));
                this.dataGridView1.Rows[index].Cells["Column2"].Value = String.Format("{0:G29}", decimal.Parse(ConnectToSQL(connection_string, "select ONE_ZamMacierz.Ilosc from cdn.ONE_ZamMacierz where GIDTyp = " + gidtyp + " and GIDNumer = " + gidnumer + " and GIDlp = " + gidlp + "and GIDSubLp=" + i)));
            }
        }
        public decimal CalculateSize(decimal dlugosc, string atrybut, int index)
        {

            List<decimal> numbers;
            decimal mod_nr;
            try
            {
                numbers = Array.ConvertAll(atrybut.Split('#'), decimal.Parse).ToList();
            }
            catch (Exception e)
            {
                if (atrybut=="")
                {
                    throw new Exception("Atrybut Parametry x#y#z jest pusty!");
                }
                else if (atrybut.Count(f => f == '#')!=2)
                {
                    throw new Exception("Atrybut Parametry x#y#z został błędnie uzupełniony!");
                }
                else throw new Exception(atrybut+"<- Popraw wartość atrybutu Parametry x#y#z, wystąpił błąd " + e);
            }
            
            if (dlugosc <= numbers[0])
            {
                //System.Windows.Forms.MessageBox.Show("Wartosci dl, n0,n1,n2 " +dlugosc+ " "+numbers[0]+" "+ numbers[1] + " " + numbers[2] + " ");
                return numbers[0];
            }
            else
            {
                mod_nr = (dlugosc - numbers[0]) % (numbers[1] + numbers[2]);

                //System.Windows.Forms.MessageBox.Show("Wartosci dl, n0,n1,n2, mod " + dlugosc + " " + numbers[0] + " " + numbers[1] + " " + numbers[2] + " "+mod_nr);
                if (mod_nr == 0)
                {
                    return dlugosc;
                }
                else
                {
                    if (mod_nr <= numbers[1])
                    {
                        //System.Windows.Forms.MessageBox.Show("Mod nr:" + mod_nr + " a nr to "+ dlugosc);
                        return dlugosc;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Wprowadzono koretkę długości do " + (index + 1) + ". wiersza z wartosci: " + dlugosc + " do wartosci: " + (dlugosc + ((numbers[1]+numbers[2]) - mod_nr)));
                        return (dlugosc + ((numbers[1] + numbers[2]) - mod_nr));
                    }
                }
                    // modulo(Długość-x)/(z+y) <= y -> bład zaokrąglić do x+int(Długość-x)/(z+y))*(z+y)+y
                    /*
                    if ((numbers[0] + Math.Truncate((dlugosc - numbers[0]) / (numbers[1] + numbers[2])) * (numbers[1] + numbers[2]) + numbers[1]) != dlugosc)
                    {
                        System.Windows.Forms.MessageBox.Show("Wprowadzono koretkę długości do " + (index + 1) + ". wiersza o wartosci: " + (numbers[0] + Math.Truncate((dlugosc - numbers[0]) / (numbers[1] + numbers[2])) * (numbers[1] + numbers[2]) + numbers[1]).ToString());
                    }
                    return (numbers[0] + Math.Truncate((dlugosc-numbers[0])/(numbers[1]+numbers[2]))*(numbers[1] + numbers[2])+numbers[1]);
                    */
            }


}

        private void DataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_EditingControlShowing(object sender,
                                   DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox) editBox = e.Control as TextBox;
        }
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DataGridView1_EditingControlShowing_1(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress += new KeyPressEventHandler(CheckKey);
        }
        private void CheckKey(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
                e.Handled = false;
            }
            else if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar)
                && e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }
        private void DataGridView1_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        /*
        private void DataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != ','))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }
        */

        private void Button1_Click(object sender, EventArgs e)
        {
            result = 0;
            info.Clear();
            //listView1.Items.Add(new ListViewItem(new string[] { "0", "0" } ) );
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Column1"].Value != null && row.Cells["Column2"].Value != null)
                {
                    if (decimal.TryParse(row.Cells["Column1"].Value.ToString().Replace(".",","), out decimal res) == true && decimal.TryParse(row.Cells["Column2"].Value.ToString(), out decimal res2) == true)
                    {
                        string fixedValue = CalculateSize(res, label4.Text.Replace(".", ","),row.Index).ToString();
                        row.Cells["Column1"].Value = fixedValue.Replace(".",",");
                        if (editBox != null) editBox.Text = fixedValue;
                        info.Add(new Newview { Dlugosc = row.Cells["Column1"].Value.ToString(), Ilosc = row.Cells["Column2"].Value.ToString() });
                        result += double.Parse(row.Cells["Column1"].Value.ToString().Replace(".", ",")) * double.Parse(row.Cells["Column2"].Value.ToString().Replace(".", ","));
                    }
                }
            }
            Confirm2 f2 = new Confirm2();
            f2.ShowDialog();
    }
        
        public static string ConnectToSQL(string connection_string, string query)
        {
            string str = "";
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = connection_string;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "Query");
                    if (dataSet.Tables.Count == 0)
                    {
                        return "";
                    }
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        str = row[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                str = str + "Bład SQL:" + ex.ToString() + "\n";
            }
            return str;
        }

        public static string Update(string connection_string, string sql)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    connection.Open();
                    adapter.UpdateCommand = new SqlCommand(sql, connection);
                    adapter.UpdateCommand.ExecuteNonQuery();
                    //MessageBox.Show("Row updated !! ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sql;
        }

    }

}
