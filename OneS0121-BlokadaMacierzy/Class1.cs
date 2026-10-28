using System;
using System.Windows;
using System.Collections.Generic;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydra;
//using cdn_api;
//using ADODB;
using System.Drawing;
using System.Windows.Forms;
using cdn_api;

[assembly: CallbackAssemblyDescription(Name: "ONES0121-SzczegolyIlosci",
                                           Description: "SzczegolyIlosci",
                                           Author: "OneSupport",
                                           Version: "1.0",
                                           SystemVersion: "2020.2.2",
                                           Created: "19-01-2021")]
namespace OneS0121_BlokadaMacierzy
{
    [SubscribeProcedure(Procedures.ZaEEdycja, "BlokadaMacierzy")] //do zmiany 19012021
    public class Button_Handle : Callback
    {
        ClaWindow szczegolyButton;
        ClaWindow szczegolyOpen;

        private int listID;
        private int XLWersjaAPI = 20202;
        private int XLTrybWsadowy =1;
        private int XLWinieta =-1;
        private int XLUtworzWlasnaSesje=1;
        private string XLSQLServer =  "192.168.4.13";//"192.168.137.149";
        private string XLSQLDatabase = "Domar2";//"rpxl_2020_2";
        private int sesja=-1;
        private string query;


        private string XLDatabase = "ERPXL_extra_23";//"ERPXL_extraDach";//"rpxl_2020_2";// "Domar2";
        private string XLUser = "Sabisup"; //"ADMIN";
        private string XLPass = "#0ut2510gro0ne.34";//"Dupa2003!";
        
        /*
        private string XLDatabase = "Domar2";//"rpxl_2020_2";// "Domar2";
        private string XLUser = "OS_AUTOM";//"Sabisup"; //"ADMIN";
        private string XLPass = "DomXL#53&";//"#0ut2510gro0ne.34";//"Dupa2003!";
        */

        private string connection_string;
        long gidtyp, gidnumer, gidlp, gidtwr;
        //private Configuration cnf;
        private ClaWindow StringIlosc, PoleIlosc;
        public override void Cleanup()
        {
        
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
        public override void Init()
        {
            AddSubscription(false, 0, Events.OpenWindow, new TakeEventDelegate(OnOpenwindow));
            AddSubscription(false, 0, Events.ResizeWindow, new TakeEventDelegate(Resize));
            AddSubscription(false, 0, Events.GainFocus, new TakeEventDelegate(Refresh));
        }
        public bool OnOpenwindow(Procedures ProcId, int ControlId, Events Event)
        {
            
            Rectangle WndBnd = GetWindow().Bounds;
            gidtyp = ZamNag.ZaN_GIDTyp;
            gidlp = ZamElem.ZaE_GIDLp;
            gidnumer = ZamNag.ZaN_GIDNumer;
            gidtwr = ZamElem.ZaE_GIDNumer;

            szczegolyButton = GetWindow().AllChildren["?TabOgolne"];
            GetWindow().OnBeforeSized += new TakeEventDelegate(Resize);
            szczegolyOpen = szczegolyButton.Children.Add(ControlTypes.button);
            szczegolyOpen.Visible = true;
            StringIlosc = GetWindow().AllChildren["?ZaE:Ilosc"];
            szczegolyOpen.Bounds = new Rectangle((StringIlosc.Bounds.X + 78), StringIlosc.Bounds.Y, 32, 21);

                szczegolyOpen.TextRaw = "Szcz. ilości";
                szczegolyOpen.ToolTipRaw = "Wprowadź Długość | Ilość towaru";

            szczegolyOpen.OnAfterAccepted += new TakeEventDelegate(OpenForm);
            szczegolyOpen.OnAfterAccepted += new TakeEventDelegate(Resize);
            szczegolyOpen.OnAfterGainFocus += new TakeEventDelegate(Refresh);
            //listID = GetWindow().AllChildren["?ListElementy"].Id;
            //popFiltr = GetWindow().AllChildren["?bUkryjFiltrDodatkowy"];
            //naduszenie przycisku
            return true;
        }

        private bool OpenForm(Procedures ProcedureId, int ControlId, Events Event)
        {
            try { 
            connection_string = GetConnectionString2();
               // MessageBox.Show(connection_string);
            //GetConnectionString2();
            string subquery = "(select AtK_ID from CDN.AtrybutyKlasy where AtK_Nazwa = 'Parametry x#y#z')";
            string query = "if exists (select 1 from cdn.Atrybuty where Atr_ObiTyp =" + TwrKarty.Twr_GIDTyp + " AND Atr_ObiNumer = " + TwrKarty.Twr_GIDNumer + " AND Atr_ObiLp =" + TwrKarty.Twr_GIDLp + " AND Atr_ObiSubLp = 0 AND Atr_AtkId = " + subquery + ") begin "
                + "select Atr_Wartosc from cdn.Atrybuty where Atr_ObiTyp =" + TwrKarty.Twr_GIDTyp + " AND Atr_ObiNumer = " + TwrKarty.Twr_GIDNumer + " AND Atr_ObiLp =" + TwrKarty.Twr_GIDLp + " AND Atr_ObiSubLp = 0 AND Atr_AtkId = " + subquery + " end else begin select 'brak' end";
            string resp = ConnectToSQL(connection_string, query);
            //MessageBox.Show(ConnectIDb("(select AtK_ID from CDN.AtrybutyKlasy where AtK_Nazwa = 'Parametry x#y#z')"));
            //MessageBox.Show(abn("a","(select AtK_ID from CDN.AtrybutyKlasy where AtK_Nazwa = 'Parametry x#y#z')"));
            if (resp == "brak")
            {
                System.Windows.Forms.MessageBox.Show("Brak wymaganego atrybutu!");
                return false;

            } else
            {
                Form1 form = new Form1();
                form.Show();
                

            }
            
            return true;
            }
            catch (Exception en)
            {
                MessageBox.Show("Błąd OpenWindow: " + en.Message);
                return false;

            }

        }
        public int XLLogin()
        {
            try { 
            return cdn_api.cdn_api.XLLogin(new XLLoginInfo_20202()
            {
                Wersja = 20202,
                ProgramID = "ONES0121-SzczegolyIlosci",
                //PlikLog = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\api.log"),
                //PlikLog = (@"C:\OneSupport\ONE-S01_GEN_AWF\api.log"),
                Baza = XLDatabase,
                OpeIdent = XLUser,
                OpeHaslo = XLPass,
                TrybWsadowy = XLTrybWsadowy,
                //TrybNaprawy = 0,
                UtworzWlasnaSesje = XLUtworzWlasnaSesje,
                Winieta = XLWinieta,
            }, ref sesja);
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd XLLogin: " + e.Message);
                return 25052023;
            }
        }
        public void Logout()
        {
            cdn_api.cdn_api.XLLogout(sesja);
        }
        private bool Resize(Procedures ProcedureId, int ControlId, Events Event)
        {
            StringIlosc = GetWindow().AllChildren["?ZaE:Ilosc"];
            szczegolyOpen.Bounds = new Rectangle(StringIlosc.Bounds.X + 100, StringIlosc.Bounds.Y, 32, 21);
            
            return true;
        }
        public void UpdateValue(double val)
        {
            try
            {
                string valZ = Form1.ConnectToSQL(Form1.connection_string, "select top 1 CAST((ZaE_PrzeliczL/ZaE_PrzeliczM) AS FLOAT) from cdn.ZamElem where ZaE_GIDNumer = " + Form1.gidnumer + " and ZaE_GIDTyp = " + Form1.gidtyp + " and ZaE_GIDLp = " + Form1.gidlp);
                if (Double.TryParse(valZ, out double mnoznik))
                {
                    Runtime.WindowController.Change(GetWindow().AllChildren["?ZaE:Ilosc"].Id, (mnoznik * val).ToString());
                    //MessageBox.Show((mnoznik * val).ToString());
                    Runtime.WindowController.Change(GetWindow().AllChildren["?nEkr_IloscPaczek"].Id, val.ToString());
                    //MessageBox.Show((Double.Parse(Runtime.WindowController.Contents(GetWindow().AllChildren["?nEkr_CenaUzgodnionaPaczka"].Id).Replace('.',','))*10).ToString());
                    if (Double.TryParse(Runtime.WindowController.Contents(GetWindow().AllChildren["?nEkr_CenaUzgodnionaPaczka"].Id).Replace('.', ','), out double res) == true)
                    {
                        //MessageBox.Show((res * val).ToString() +" "+ Runtime.WindowController.Contents(GetWindow().AllChildren["?nEkr_CenaUzgodnionaPaczka"].Id));
                        Runtime.WindowController.Change(GetWindow().AllChildren["?ZaE:WartoscPoRabacie"].Id, (res * val).ToString());
                    }
                    else if (Decimal.TryParse((mnoznik * val).ToString(), out decimal value) == true)
                    {
                        Runtime.WindowController.Change(GetWindow().AllChildren["?ZaE:WartoscPoRabacie"].Id, Decimal.Multiply(value, ZamElem.ZaE_CenaUzgodniona).ToString());
                    }
                }
                Runtime.WindowController.PostEvent(0, Events.FullRefresh);
            }catch(Exception e)
            {
                MessageBox.Show("Błąd UpdateValue: " + e.Message);
            }
        }
        private bool Refresh(Procedures ProcedureId, int ControlId, Events Event)
        {
            return true;
        }
        public string GetConnectionString2()
        {
            XLLogin();
            XLPolaczenieInfo_20202 polaczenieInfo = new XLPolaczenieInfo_20202 { Wersja = 20202 };
            cdn_api.cdn_api.XLPolaczenie(polaczenieInfo);

           
            //connection_string = @"Provider=MSOLEDBSQL.1;Password=43gQdfs31so3;Persist Security Info=True;User ID=onesupport_dod;Initial Catalog="";Data Source=SRV1;Initial File Name="";Server SPN="";Authentication="";Access Token=""";
            string connection_string = "Server=" + polaczenieInfo.Serwer + ";Database=" + polaczenieInfo.Baza + ";User Id=" + XLUser + ";Password="+XLPass;
            
            //string connection_string = "Server=" + polaczenieInfo.Serwer + ";Database=" + polaczenieInfo.Baza + ";Integrated Security=SSPI;";
            Logout();
            return connection_string;
        }


    }





    [SubscribeProcedure(Procedures.TrEEdycja, "BlokadaMacierzy")] //do zmiany 19012021
    public class Button_Handle2 : Callback
    {
        ClaWindow szczegolyButton;
        ClaWindow szczegolyOpen;

        private int listID;
        private int XLWersjaAPI = 20202;
        private int XLTrybWsadowy = 1;
        private int XLWinieta = -1;
        private int XLUtworzWlasnaSesje = 1;
        private string XLSQLServer = "192.168.4.13";//"192.168.137.149";
        private string XLSQLDatabase = "Domar2";//"rpxl_2020_2";
        private int sesja = -1;
        private string query;


        private string XLDatabase = "ERPXL_extra_23";//"ERPXL_extraDach";//"rpxl_2020_2";// "Domar2";
        private string XLUser = "Sabisup"; //"ADMIN";
        private string XLPass = "#0ut2510gro0ne.34";//"Dupa2003!";

        /*
        private string XLDatabase = "Domar2";//"rpxl_2020_2";// "Domar2";
        private string XLUser = "OS_AUTOM";//"Sabisup"; //"ADMIN";
        private string XLPass = "DomXL#53&";//"#0ut2510gro0ne.34";//"Dupa2003!";
        */

        private string connection_string;
        long gidtyp, gidnumer, gidlp, gidtwr;
        //private Configuration cnf;
        private ClaWindow StringIlosc, PoleIlosc;
        public override void Cleanup()
        {

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
        public override void Init()
        {
            AddSubscription(false, 0, Events.OpenWindow, new TakeEventDelegate(OnOpenwindow));
            AddSubscription(false, 0, Events.ResizeWindow, new TakeEventDelegate(Resize));
            AddSubscription(false, 0, Events.GainFocus, new TakeEventDelegate(Refresh));
        }
        public bool OnOpenwindow(Procedures ProcId, int ControlId, Events Event)
        {

            Rectangle WndBnd = GetWindow().Bounds;
            gidtyp = TraNag.TrN_GIDTyp;
            gidlp = TraElem.TrE_GIDLp;
            gidnumer = TraNag.TrN_GIDNumer;
            gidtwr = TraElem.TrE_GIDNumer;

            szczegolyButton = GetWindow().AllChildren["?TabOgolne"];
            GetWindow().OnBeforeSized += new TakeEventDelegate(Resize);
            szczegolyOpen = szczegolyButton.Children.Add(ControlTypes.button);
            szczegolyOpen.Visible = true;
            StringIlosc = GetWindow().AllChildren["?nEkr_Wartosc"];
            szczegolyOpen.Bounds = new Rectangle((StringIlosc.Bounds.X + 78), StringIlosc.Bounds.Y, 32, 21);

            szczegolyOpen.TextRaw = "Szcz. ilości";
            szczegolyOpen.ToolTipRaw = "Wprowadź Długość | Ilość towaru";

            szczegolyOpen.OnAfterAccepted += new TakeEventDelegate(OpenForm);
            szczegolyOpen.OnAfterAccepted += new TakeEventDelegate(Resize);
            szczegolyOpen.OnAfterGainFocus += new TakeEventDelegate(Refresh);
            //listID = GetWindow().AllChildren["?ListElementy"].Id;
            //popFiltr = GetWindow().AllChildren["?bUkryjFiltrDodatkowy"];
            //naduszenie przycisku
            return true;
        }

        private bool OpenForm(Procedures ProcedureId, int ControlId, Events Event)
        {
            
                connection_string = GetConnectionString2();
                //GetConnectionString2();
                string subquery = "(select AtK_ID from CDN.AtrybutyKlasy where AtK_Nazwa = 'Parametry x#y#z')";
                string query = "if exists (select 1 from cdn.Atrybuty where Atr_ObiTyp =" + TwrKarty.Twr_GIDTyp + " AND Atr_ObiNumer = " + TwrKarty.Twr_GIDNumer + " AND Atr_ObiLp =" + TwrKarty.Twr_GIDLp + " AND Atr_ObiSubLp = 0 AND Atr_AtkId = " + subquery + ") begin "
                    + "select Atr_Wartosc from cdn.Atrybuty where Atr_ObiTyp =" + TwrKarty.Twr_GIDTyp + " AND Atr_ObiNumer = " + TwrKarty.Twr_GIDNumer + " AND Atr_ObiLp =" + TwrKarty.Twr_GIDLp + " AND Atr_ObiSubLp = 0 AND Atr_AtkId = " + subquery + " end else begin select 'brak' end";
                string resp = ConnectToSQL(connection_string, query);
                //MessageBox.Show(ConnectIDb("(select AtK_ID from CDN.AtrybutyKlasy where AtK_Nazwa = 'Parametry x#y#z')"));
                //MessageBox.Show(abn("a","(select AtK_ID from CDN.AtrybutyKlasy where AtK_Nazwa = 'Parametry x#y#z')"));
                if (resp == "brak")
                {
                    System.Windows.Forms.MessageBox.Show("Brak wymaganego atrybutu!");
                    return false;

                }
                else
                {
                    Form2 form = new Form2();
                    form.Show();


                }

                return true;
           

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
                    TrybWsadowy = XLTrybWsadowy,
                    //TrybNaprawy = 0,
                    UtworzWlasnaSesje = XLUtworzWlasnaSesje,
                    Winieta = XLWinieta,
                }, ref sesja);
           
        }
        public void Logout()
        {
            cdn_api.cdn_api.XLLogout(sesja);
        }
        private bool Resize(Procedures ProcedureId, int ControlId, Events Event)
        {
            StringIlosc = GetWindow().AllChildren["?nEkr_Wartosc"];
            szczegolyOpen.Bounds = new Rectangle(StringIlosc.Bounds.X + 100, StringIlosc.Bounds.Y, 32, 21);

            return true;
        }
        public void UpdateValue(double val)
        {
            string valZ = Form1.ConnectToSQL(Form1.connection_string, "select top 1 CAST((TrE_PrzeliczL/TrE_PrzeliczM) AS FLOAT) from cdn.TraElem where TrE_GIDNumer = " + Form1.gidnumer + " and TrE_GIDTyp = " + Form1.gidtyp + " and TrE_GIDLp = " + Form1.gidlp);
            if (Double.TryParse(valZ, out double mnoznik))
            {
                Runtime.WindowController.Change(GetWindow().AllChildren["?TrE:Ilosc"].Id, (mnoznik * val).ToString());
                //MessageBox.Show((mnoznik * val).ToString());
                Runtime.WindowController.Change(GetWindow().AllChildren["?nEkr_IloscPaczek"].Id, val.ToString());
                //MessageBox.Show((Double.Parse(Runtime.WindowController.Contents(GetWindow().AllChildren["?nEkr_CenaUzgodnionaPaczka"].Id).Replace('.',','))*10).ToString());
                if (Double.TryParse(Runtime.WindowController.Contents(GetWindow().AllChildren["?nEkr_CenaUzgodnionaPaczka"].Id).Replace('.', ','), out double res) == true)
                {
                    //MessageBox.Show((res * val).ToString() +" "+ Runtime.WindowController.Contents(GetWindow().AllChildren["?nEkr_CenaUzgodnionaPaczka"].Id));
                    Runtime.WindowController.Change(GetWindow().AllChildren["?TrE:WartoscPoRabacie"].Id, (res * val).ToString());
                }
                else if (Decimal.TryParse((mnoznik * val).ToString(), out decimal value) == true)
                {
                    Runtime.WindowController.Change(GetWindow().AllChildren["?TrE:WartoscPoRabacie"].Id, Decimal.Multiply(value, ZamElem.ZaE_CenaUzgodniona).ToString());
                }
            }
            Runtime.WindowController.PostEvent(0, Events.FullRefresh);
        }
        private bool Refresh(Procedures ProcedureId, int ControlId, Events Event)
        {
            return true;
        }
        public string GetConnectionString2()
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


    }


}
