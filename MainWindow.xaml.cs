using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BD_08toWGS84
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btn_in_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "输入文件";
            openFileDialog.Filter = "excel文件|*.xls|excel文件|*.xlsx|所有文件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "shp";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string inputFile = openFileDialog.FileName;
            this.inTextBox.Text = inputFile;

        }


        private void btn_ex_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "输出文件";
            saveFileDialog.Filter = "excel文件|*.xls|excel文件|*.xlsx|所有文件|*.*";
            saveFileDialog.FileName = string.Empty;
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string outputFile = saveFileDialog.FileName;
            this.exTextBox.Text = outputFile;
        }



        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.inTextBox.Text != string.Empty && this.exTextBox.Text != string.Empty )
            {
                string inFilePath = this.inTextBox.Text;
              
                string outFilePath = this.exTextBox.Text;
               DataSet a= ReadExcelFile(inFilePath);
               DataSet b = BDtoWGS84(a);

               OutputExcelFile(b, outFilePath);
                    
            }
            else
                System.Windows.Forms.MessageBox.Show("路径不能为空！！！");
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public static DataSet ReadExcelFile(string filepath)
        {
            string connStr = "";
            string fileName = System.IO.Path.GetFileName(filepath);
            string fileType = System.IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(fileType))
            { return null; }
            //if (fileType == ".xls")
            // {
            //    connStr = "Provider=Microsoft.Ace.OLEDB.4.0;" + "DataSource=" + filepath + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1;'";


            //}
            //else
            //{
            connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filepath + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1;'";
            // }

            string sql_F = "Select * FROM [{0}]";

            OleDbConnection conn = null;

            OleDbDataAdapter da = null;

            DataTable dtSheetName = null;

            DataSet ds = new DataSet();

            try
            {
                //初始化连接，并打开
                conn = new OleDbConnection(connStr);
                conn.Open();
                //获取数据源的表定义元数据
                string sheetName = "";
                dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                //初始化适配器
                da = new OleDbDataAdapter();
                for (int i = 0; i < dtSheetName.Rows.Count; i++)
                {
                    sheetName = (string)dtSheetName.Rows[i]["TABLE_NAME"];
                    if (sheetName.Contains("$") && !sheetName.Replace("'", "").EndsWith("$"))
                    {
                        continue;
                    }

                    da.SelectCommand = new OleDbCommand(String.Format(sql_F, sheetName), conn);
                    DataSet dsItem = new DataSet();
                    da.Fill(dsItem, sheetName);
                    ds.Tables.Add(dsItem.Tables[0].Copy());

                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                // 关闭连接
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    da.Dispose();
                    conn.Dispose();

                }


            }
            return ds;
        }

        public    int OutputExcelFile(DataSet ds,string path)
        {
          // string connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=2;'";
            String connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" +

      "Data Source=" + path +

      ";Extended Properties=Excel 12.0;";
            
            OleDbConnection cn = new OleDbConnection(connStr);

           string sqlCreate = "CREATE TABLE TestSheet ([ID] INTEGER,[Name] VarChar,[Lat] VarChar,[Lon] VarChar,[Location] VarChar,[Num] VarChar,[TypeC] VarChar,[TypeE] VarChar,[Url] VarChar)";

           OleDbCommand cmd = new OleDbCommand(sqlCreate, cn);
          //创建Excel文件

             cn.Open();
            
         //创建TestSheet工作表

                cmd.ExecuteNonQuery();

              //添加数据
                foreach (DataTable dt in ds.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        int a0 =int.Parse(dr[dt.Columns[0]].ToString());
                        var a1 = dr[dt.Columns[1]];
                        var a2 = dr[dt.Columns[2]];
                        var a3 = dr[dt.Columns[3]];
                        var a4 = dr[dt.Columns[4]];
                        var a5 = dr[dt.Columns[5]];
                        var a6 = dr[dt.Columns[6]];
                        var a7 = dr[dt.Columns[7]];
                        var a8 = dr[dt.Columns[8]];
                       // string instr = string.Format("insert into info values ('{0}','{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}')", a0,a1,a2,a3,a4,a5,a6,a7,a8);
                        //Oledbcommand incmd = new oledbcommand(instr, inconn);
                       // incmd.executenonquery();
                      //  inconn.close();


                        cmd.CommandText = string.Format("INSERT INTO TestSheet VALUES ('{0}','{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}')", a0, a1, a2, a3, a4, a5, a6, a7, a8);
                        //cmd.CommandText = "INSERT INTO TestSheet VALUES(a0,a1,a2,a3,a4,a5,a6,a7,a8)";
                       // cmd.CommandText = "INSERT INTO TestSheet VALUES(1,1,1,1,1,1,1,1,1)";
                        cmd.ExecuteNonQuery();
                    }
                }

              //关闭连接

        cn.Close();
       System.Windows.MessageBox.Show("输出完毕");
            return 1;
        }


        //定义一些常量
        double x_PI = 3.14159265358979324 * 3000.0 / 180.0;
        double  PI = 3.1415926535897932384626;
        double a = 6378245.0;
        double ee = 0.00669342162296594323;

        public  DataSet BDtoWGS84(DataSet ds) 
        {
            DataSet ds1 = new DataSet();
            
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr.BeginEdit();
                    double[] a=new double[2];
                    double[] b= new double[2];
                    a = BD09toGCJ02(double.Parse(dr[dt.Columns[2]].ToString()), double.Parse(dr[dt.Columns[3]].ToString()));
                    b = GCJ02toWGS84(a[0],a[1]);
                    dr[dt.Columns[2]]= (object)b[0];
                    dr[dt.Columns[3]] = (object)b[1];
                    dr.EndEdit();
                }
            }
            ds1.Tables.Add(ds.Tables[0].Copy());

            return ds1;
        }

        public  double[] BD09toGCJ02(double bd_lat, double bd_lon)
        {

            double x_pi = 3.14159265358979324 * 3000.0 / 180.0;
    double x = bd_lon - 0.0065;
    double y= bd_lat - 0.006;
    double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
   double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);
    double gg_lng = z * Math.Cos(theta);
   double gg_lat = z * Math.Sin(theta);
    return new double[] { gg_lat,gg_lng,};
        
        }

        public  double[] GCJ02toWGS84(double lat, double lng)

        {
       double dlat = transformlat(lng - 105.0, lat - 35.0);
        double dlng = transformlng(lng - 105.0, lat - 35.0);
       double radlat = lat / 180.0 * PI;
        double magic = Math.Sin(radlat);
        magic = 1 - ee * magic * magic;
       double sqrtmagic = Math.Sqrt(magic);
        dlat = (dlat * 180.0) / ((a * (1 - ee)) / (magic * sqrtmagic) * PI);
        dlng = (dlng * 180.0) / (a / sqrtmagic * Math.Cos(radlat) * PI);
       double mglat = lat + dlat;
       double  mglng = lng + dlng;
       return new double[] { lat * 2 - mglat, lng * 2 - mglng };
        }

        public  double   transformlat(double lng, double lat)
        {
    var ret = -100.0 + 2.0 * lng + 3.0 * lat + 0.2 * lat * lat + 0.1 * lng * lat + 0.2 * Math.Sqrt(Math.Abs(lng));
    ret += (20.0 * Math.Sin(6.0 * lng * PI) + 20.0 * Math.Sin(2.0 * lng * PI)) * 2.0 / 3.0;
    ret += (20.0 * Math.Sin(lat * PI) + 40.0 * Math.Sin(lat / 3.0 * PI)) * 2.0 / 3.0;
    ret += (160.0 * Math.Sin(lat / 12.0 * PI) + 320 * Math.Sin(lat * PI / 30.0)) * 2.0 / 3.0;
    return ret;
}
        public  double    transformlng(double lng, double lat) {
    var ret = 300.0 + lng + 2.0 * lat + 0.1 * lng * lng + 0.1 * lng * lat + 0.1 * Math.Sqrt(Math.Abs(lng));
    ret += (20.0 * Math.Sin(6.0 * lng * PI) + 20.0 * Math.Sin(2.0 * lng * PI)) * 2.0 / 3.0;
    ret += (20.0 * Math.Sin(lng * PI) + 40.0 * Math.Sin(lng / 3.0 * PI)) * 2.0 / 3.0;
    ret += (150.0 * Math.Sin(lng / 12.0 * PI) + 300.0 * Math.Sin(lng / 30.0 * PI)) * 2.0 / 3.0;
    return ret;
}
    }

}
