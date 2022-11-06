using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm_Province_City
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //加载数据库中的所有的省的数据
            string connStr = ConfigurationManager.ConnectionStrings["sqlConn"].ConnectionString;

            //创建连接对象
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = @"select AreaId,AreaName,AreaPid from [dbo].[AreaFull] where AreaPid=0";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //int AreaId = int.Parse(reader["AreaId"].ToString());
                            //把表格数据转换成对象的数据
                            AreaInfo areaInfo = new AreaInfo();
                            areaInfo.AreaId = int.Parse(reader["AreaId"].ToString());
                            areaInfo.AreaName = reader["AreaName"].ToString();
                            areaInfo.AreaPid = int.Parse(reader["AreaPid"].ToString());
                            //将生成的对象放到Combox。Combox的显示信息是Item对象的ToString()
                            this.cbxProvince.Items.Add(areaInfo);
                        }
                    }//SqlDataReader
                }//SqlCommand

            }// SqlConnection
            this.cbxProvince.SelectedIndex = 0;
        }

        private void cbxProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
           AreaInfo provinceInfo= this.cbxProvince.SelectedItem as AreaInfo;
            if (provinceInfo == null)
            {
                return;
            }
            //根据选择的省的ID获取城市信息
            //provinceInfo.AreaId
            //加载数据库中的所有的省的数据
            string connStr = ConfigurationManager.ConnectionStrings["sqlConn"].ConnectionString;

            //创建连接对象
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = @"select AreaId,AreaName,AreaPid from [dbo].[AreaFull] where AreaPid=" + provinceInfo.AreaId;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        this.cbxCity.Items.Clear();
                        while (reader.Read())
                        {
                            //int AreaId = int.Parse(reader["AreaId"].ToString());
                            //把表格数据转换成对象的数据
                            AreaInfo areaInfo = new AreaInfo();
                            areaInfo.AreaId = int.Parse(reader["AreaId"].ToString());
                            areaInfo.AreaName = reader["AreaName"].ToString();
                            areaInfo.AreaPid = int.Parse(reader["AreaPid"].ToString());
                            //将生成的对象放到Combox。Combox的显示信息是Item对象的ToString()
                           
                            this.cbxCity.Items.Add(areaInfo);
                        }
                    }//SqlDataReader
                }//SqlCommand

            }// SqlConnection
            this.cbxCity.SelectedIndex = 0;
        }

    }
}
