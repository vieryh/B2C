using System;
using System.Data;
using System.Data.SqlClient;

namespace DBUtility
{
    /// <summary>
    ///  实现所有对SQL Server数据库的所有访问操作
    /// </summary>
    public class SqlDBHelp
    {
        private static string _connStr = "server=.;uid=sa;pwd=wecan;database=B2C";
        private static SqlConnection sqlcon;

        /// <summary>
        /// 获取一个可用于数据库操作的连接类
        /// </summary>
        private static SqlConnection Connection
        {
            get
            {
                if (sqlcon == null)
                {
                    sqlcon = new SqlConnection(_connStr);
                    sqlcon.Open();
                }
                else if (sqlcon.State == ConnectionState.Broken || sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Close();
                    sqlcon.Open();
                }

                return sqlcon;
            }
        }

        /// <summary>
        /// 根据查询的语句返回执行受影响的行数
        /// </summary>
        /// <param name="strsql">Insert、Update、Delete语句;</param>
        /// <returns>执行受影响的行数</returns>
        public static int GetExecute(string strsql)
        {
            int i = -1;
            try
            {
                SqlCommand sqlcmd = new SqlCommand(strsql, Connection);
                i = sqlcmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return i;
        }


        /// <summary>
        /// 根据查询的语句返回执行受影响的行数
        /// </summary>
        /// <param name="strsql">Insert、Update、Delete语句</param>
        /// <param name="p">给SQL语句传递的参数集合</param>
        /// <returns>执行受影响的行数</returns>      
        public static int GetExecute(string strsql, params SqlParameter[] p)
        {
            int i = -1;
            try
            {
                SqlCommand sqlcmd = new SqlCommand(strsql, Connection);
                sqlcmd.Parameters.AddRange(p);
                i = sqlcmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return i;
        }

        /// <summary>
        ///  根据查询的语句获取查询的结果集
        /// </summary>
        /// <param name="strsql">Select语句</param>
        /// <returns>查询的结果-表数据</returns>
        public static DataTable GetTable(string strsql)
        {
            DataTable dt = null;
            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(strsql, Connection);
                dt = new DataTable();
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }

        /// <summary>
        /// 根据查询的语句获取查询的结果集
        /// </summary>
        /// <param name="strsql">Select语句</param>
        /// <param name="p">给SQL语句传递的参数集合</param>
        /// <returns>查询的结果-表数据</returns>
        public static DataTable GetTable(string strsql, params SqlParameter[] p)
        {
            DataTable dt = null;
            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(strsql, Connection);
                sda.SelectCommand.Parameters.AddRange(p);
                dt = new DataTable();
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }

        /// <summary>
        /// 根据查询的语句返回一个值
        /// </summary>
        /// <param name="strsql">Select语句</param>
        /// <returns>单值</returns>
        public static string GetSingle(string strsql)
        {
            object o = "";
            try
            {
                SqlCommand sqlcmd = new SqlCommand(strsql, Connection);
                o = sqlcmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return o.ToString();
        }

        /// <summary>
        /// 根据查询的语句返回一个值
        /// </summary>
        /// <param name="strsql">Select语句</param>
        /// <param name="p">给SQL语句传递的参数集合</param>
        /// <returns>单值</returns>
        public static string GetSingle(string strsql, params SqlParameter[] p)
        {
            object o = "";
            try
            {
                SqlCommand sqlcmd = new SqlCommand(strsql, Connection);
                sqlcmd.Parameters.AddRange(p);
                o = sqlcmd.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return o.ToString();
        }


        /// <summary>
        /// 根据查询语句返回轻量级的SqlDataReader对象
        /// </summary>
        /// <param name="strsql">Select语句</param>
        /// <returns>轻量级的SqlDataReader对象</returns>
        public static SqlDataReader GetReader(string strsql)
        {
            SqlCommand sqlcmd = new SqlCommand(strsql, Connection);
            return sqlcmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// 根据查询语句返回轻量级的SqlDataReader对象
        /// </summary>
        /// <param name="strsql">Select语句</param>
        /// <param name="p">给SQL语句传递的参数集合</param>
        /// <returns>轻量级的SqlDataReader对象</returns>
        public static SqlDataReader GetReader(string strsql, params SqlParameter[] p)
        {
            SqlCommand sqlcmd = new SqlCommand(strsql, Connection);
            sqlcmd.Parameters.AddRange(p);
            return sqlcmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// 事务操作
        /// </summary>
        /// <param name="strsqls"></param>
        /// <returns></returns>
        public static bool GetTransOperate(string[] strsqls)
        {
            bool isflag = false;
            SqlTransaction trans = Connection.BeginTransaction();
            SqlCommand sqlcmd = new SqlCommand();

            try
            {
                foreach (string s in strsqls)
                {
                    sqlcmd.CommandText = s;
                    sqlcmd.Connection = sqlcon;
                    sqlcmd.ExecuteNonQuery();
                }
                isflag = true;
                trans.Commit();
            }
            catch (Exception ex)
            {
                isflag = false;
                trans.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return isflag;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        private static void CloseConnection()
        {
            if (sqlcon != null)
            {
                sqlcon.Close();
            }
        }
    }

}
