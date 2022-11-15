using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 用户登录次数限制案例
{
    public class UserInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public int ErrorTimes { get; set; }
        public DateTime LastErrorDate { get; set; }
    }
}
