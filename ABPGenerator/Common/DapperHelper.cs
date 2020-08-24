using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPGenerator.Common
{
    /// <summary>
    /// Dapper帮助类
    /// </summary>
    public class DapperHelper
    {
        /// <summary>
        /// DB Connetction String
        /// </summary>
       // private static string connectionString = ""; //ConfigurationManager.ConnectionStrings["Default"].ToString();

        /// <summary>
        /// 获取所有的数据库
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDatabase(string connectionString)
        {
            string sql = "SELECT NAME FROM MASTER.DBO.SYSDATABASES ORDER BY NAME";
            using (var conn = new SqlConnection(connectionString))
            {
                return conn.Query<string>(sql).ToList();
            }

        }

    
        /// <summary>
        /// 获取所有的表
        /// </summary>
        /// <returns>SELECT NAME FROM SYSOBJECTS WHERE XTYPE='U' ORDER BY NAME</returns>
        public static List<string> GetTable(string connectionString)
        {
            string sql = "SELECT NAME FROM SYSOBJECTS WHERE XTYPE='U' ORDER BY NAME desc";
            using (var conn = new SqlConnection(connectionString))
            {
                return conn.Query<string>(sql).ToList();
            }

        }

    
        /// <summary>
        /// 获取表的字段名 
        /// </summary>
        /// <returns>SELECT NAME FROM SYSCOLUMNS WHERE ID=OBJECT_ID('SysLogger');</returns>
        public static List<string> GetField(string connectionString, string table)
        {
          
            string sql = "SELECT COLLATION_NAME,DATA_TYPE,IS_NULLABLE FROM INFORMATION_SCHEMA.columns WHERE TABLE_NAME='"+table+"'";
           
            using (var conn = new SqlConnection(connectionString))
            {
                return conn.Query<string>(sql).ToList();
            }
        }

        /// <summary>
        /// 链接数据库信息
        /// </summary>
        public class DatabaseInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public string servername { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string userid { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string password { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string dbname { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string table { get; set; }
        }
    }
}
