using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace 用户登录次数限制案例
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //1. 根据用户ID
            string connStr = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = @"SELECT [UserId]
                                          ,[UserName]
                                          ,[UserPwd]
                                          ,[LastErrorDateTime]
                                          ,[ErrorTimes]
                                      FROM [myTestDB].[dbo].[UserInfo] 
                                      WHERE UserName='" + txtUserName.Text.Trim() + "'and UserPwd='" + txtUserPwd.Text.Trim() + "'";
                    bool isHasData = false;
                    UserInfo uInfo = null; //封装查询来的数据
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            uInfo = new UserInfo();
                            uInfo.UserId = int.Parse(reader["UserId"].ToString());
                            uInfo.UserPwd = reader["UserPwd"].ToString();
                            uInfo.LastErrorDate = DateTime.Parse(reader["LastErrorDateTime"].ToString());
                            uInfo.ErrorTimes = int.Parse(reader["ErrorTimes"].ToString());

                        }
                        //判断查询结果中是否有数据，有则返回True,没有则返回False
                        isHasData = reader.HasRows;
                    }//SqlDataReader reader = cmd.ExecuteReader() 该花括号执行结束之前，连接一直没有关闭，这时reader一直占用Connection对象, 其他Command对象不能使用Connection对象
                    if (!isHasData) //如果没有数据
                    {
                        cmd.CommandText = @"update [myTestDB].[dbo].[UserInfo]  set [ErrorTimes]=ErrorTimes+1,[LastErrorDateTime]=getdate() where [UserName]='" + txtUserName.Text.Trim() + "'";
                        cmd.ExecuteNonQuery();
                        return;
                    }
                    if (uInfo.ErrorTimes <= 3 || DateTime.Now.Subtract(uInfo.LastErrorDate).Minutes>15)
                    {
                        MessageBox.Show("登录成功");
                        cmd.CommandText = @"update [myTestDB].[dbo].[UserInfo]  set [ErrorTimes]=0 where [UserId]='" + uInfo.UserId+"'";
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("用户名密码不正确");
                    }
                }
            }
        }
    }
}
