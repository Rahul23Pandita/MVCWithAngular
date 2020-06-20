using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace MVCWithAngular
{
    public class DbConn
    {
        public static MySqlConnection SQLConn;

        public static int ExecuteQuerySql(string Query)
        {
            int intRecs = 0;
            MySqlConnection SQLConn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            try
            {

                if (SQLConn.State != ConnectionState.Open)
                {
                    SQLConn.Open();
                }
                MySqlCommand SQLcmd = new MySqlCommand(Query, SQLConn);
                SQLcmd.CommandText = Query;
                intRecs = SQLcmd.ExecuteNonQuery();
                SQLcmd.Dispose();

            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (SQLConn.State == ConnectionState.Open)
                {

                    SQLConn.Close();
                }
            }
            return intRecs;
        }

        public static DataSet ReturnDataSet(string Query, string conName = "connectionString")
        {
            try
            {

                SQLConn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[conName].ConnectionString);
                SQLConn.Open();
                MySqlCommand cmd = new MySqlCommand(Query, SQLConn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                cmd.Dispose();
                adp.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                string xyz = ex.Message;
                return null;
                //throw (ex.Message);
            }
            finally
            {
                SQLConn.Close();
            }

        }



        public static DataTable ReturnDataTable(string Query, string conName = "connectionString")
        {
            try
            {

                SQLConn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[conName].ConnectionString);
                SQLConn.Open();
                MySqlCommand cmd = new MySqlCommand(Query, SQLConn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                cmd.Dispose();
                adp.Dispose();
                return dt;

            }
            catch (Exception ex)
            {
                string xyz = ex.Message;
                return null;
                //throw (ex.Message);
            }
            finally
            {
                SQLConn.Close();
            }

        }
        public static DataTable ReturnSQLData(string sql)
        {
            DataSet ds = ReturnDataSet(sql);
            return ds.Tables[0];
        }




        public static int ExecuteQuery(string SQuery, string strConnection = "connectionString", SqlConnection DBConn = null, SqlTransaction DBTrans = null)
        {
            MySqlConnection cn = new MySqlConnection();
            int intRecs = 0;

            if (DBConn == null)
            {
                cn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[strConnection].ConnectionString);
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = SQuery;
                cmd.Connection = cn;
                cn.Open();
                cmd.CommandTimeout = 0;
                intRecs = cmd.ExecuteNonQuery();
                cn.Close();

            }
            else
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SQuery;
                cmd.Connection = DBConn;
                cmd.Transaction = DBTrans;
                cmd.CommandTimeout = 0;
                intRecs = cmd.ExecuteNonQuery();

            }
            return intRecs;

        }

        public static MySqlConnection GetDBConnect(string strConnection = "connectionString")
        {
            MySqlConnection cn = new MySqlConnection(GetDBConnectionString(strConnection));
            return cn;
        }

        public static string GetDBConnectionString(string strConnection = "connectionString")
        {
            string strConn = System.Configuration.ConfigurationManager.ConnectionStrings[strConnection].ConnectionString;
            return strConn;
        }

        public static DataTable GetDataTable(string sQuery, string TableName, string strDBConnection = "connectionString", MySqlConnection DBConn = null, MySqlTransaction DBTrans = null)
        {
            DataSet ds = new DataSet();
            if ((DBConn != null))
            {
                MySqlDataAdapter da = new MySqlDataAdapter(sQuery, DBConn);
                da.SelectCommand.CommandTimeout = 0;
                if ((DBTrans != null))
                    da.SelectCommand.Transaction = DBTrans;

                da.Fill(ds, TableName);

            }
            else
            {
                SqlDataAdapter da = new SqlDataAdapter(sQuery, GetDBConnectionString(strDBConnection));
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds, TableName);
            }
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;

            }
        }

        public static DataSet GetDataSet(string sQuery, string TableName, string strDBConnection = "connectionString", MySqlConnection DBConn = null, MySqlTransaction DBTrans = null)
        {
            DataSet ds = new DataSet();
            if ((DBConn != null))
            {
                MySqlDataAdapter da = new MySqlDataAdapter(sQuery, DBConn);
                da.SelectCommand.CommandTimeout = 0;
                if ((DBTrans != null))
                    da.SelectCommand.Transaction = DBTrans;

                da.Fill(ds, TableName);

            }
            else
            {
                SqlDataAdapter da = new SqlDataAdapter(sQuery, GetDBConnectionString(strDBConnection));
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds, TableName);
            }
            if (ds.Tables.Count > 0)
            {
                return ds;
            }
            else
            {
                return null;

            }
        }






        public static DataTable GetData(MySqlCommand cmd, string TableName, string strDBConnection = "connectionString", MySqlConnection DBConn = null, MySqlTransaction DBTrans = null)
        {
            DataSet ds = new DataSet();
            if ((DBConn != null))
            {
                MySqlDataAdapter da = new MySqlDataAdapter();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = DBConn;
                cmd.Transaction = DBTrans;
                da.SelectCommand = cmd;
                da.SelectCommand.CommandTimeout = 0;
                if ((DBTrans != null))
                    da.SelectCommand.Transaction = DBTrans;

                da.Fill(ds, TableName);

            }
            else
            {
                DBConn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[strDBConnection].ConnectionString);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = DBConn;
                cmd.Transaction = DBTrans;
                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = cmd;
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds, TableName);
            }

            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;

            }
        }


        public static MySqlDataReader GetDataReaded(MySqlCommand cmd, string TableName, string strDBConnection = "connectionString", MySqlConnection DBConn = null, MySqlTransaction DBTrans = null)
        {
            DataSet ds = new DataSet();
            if ((DBConn != null))
            {
                //MySqlDataAdapter da = new MySqlDataAdapter();

                cmd.CommandType = CommandType.Text;
                cmd.Connection = DBConn;
                cmd.Transaction = DBTrans;
                MySqlDataReader dr = cmd.ExecuteReader();
                return dr;
            }
            else
            {
                DBConn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[strDBConnection].ConnectionString);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = DBConn;
                cmd.Transaction = DBTrans;
                MySqlDataReader dr = cmd.ExecuteReader();
                return dr;
            }


        }

        public static string ConvertAsciiToText(string pValue)
        {
            pValue = pValue.Replace("&#32;", " ");
            pValue = pValue.Replace("&#33;", "!");
            pValue = pValue.Replace("&#34;", "\"");
            pValue = pValue.Replace("&#35;", "#");
            pValue = pValue.Replace("&#36;", "$");
            pValue = pValue.Replace("&#37;", "%");
            pValue = pValue.Replace("&#38;", "&");
            pValue = pValue.Replace("&#39;", "'");
            pValue = pValue.Replace("&#40;", "(");
            pValue = pValue.Replace("&#41;", ")");
            pValue = pValue.Replace("&#42;", "*");
            pValue = pValue.Replace("&#43;", "+");
            pValue = pValue.Replace("&#44;", ",");
            pValue = pValue.Replace("&#45;", "-");
            pValue = pValue.Replace("&#46;", ".");
            pValue = pValue.Replace("&#47;", "/");
            pValue = pValue.Replace("&#48;", "0");
            pValue = pValue.Replace("&#49;", "1");
            pValue = pValue.Replace("&#50;", "2");
            pValue = pValue.Replace("&#51;", "3");
            pValue = pValue.Replace("&#52;", "4");
            pValue = pValue.Replace("&#53;", "5");
            pValue = pValue.Replace("&#54;", "6");
            pValue = pValue.Replace("&#55;", "7");
            pValue = pValue.Replace("&#56;", "8");
            pValue = pValue.Replace("&#57;", "9");
            pValue = pValue.Replace("&#58;", ":");
            pValue = pValue.Replace("&#59;", ";");
            pValue = pValue.Replace("&#60;", "<");
            pValue = pValue.Replace("&#61;", "=");
            pValue = pValue.Replace("&#62;", ">");
            pValue = pValue.Replace("&#63;", "?");
            pValue = pValue.Replace("&#64;", "@");
            pValue = pValue.Replace("&#65;", "A");
            pValue = pValue.Replace("&#66;", "B");
            pValue = pValue.Replace("&#67;", "C");
            pValue = pValue.Replace("&#68;", "D");
            pValue = pValue.Replace("&#69;", "E");
            pValue = pValue.Replace("&#70;", "F");
            pValue = pValue.Replace("&#71;", "G");
            pValue = pValue.Replace("&#72;", "H");
            pValue = pValue.Replace("&#73;", "I");
            pValue = pValue.Replace("&#74;", "J");
            pValue = pValue.Replace("&#75;", "K");
            pValue = pValue.Replace("&#76;", "L");
            pValue = pValue.Replace("&#77;", "M");
            pValue = pValue.Replace("&#78;", "N");
            pValue = pValue.Replace("&#79;", "O");
            pValue = pValue.Replace("&#80;", "P");
            pValue = pValue.Replace("&#81;", "Q");
            pValue = pValue.Replace("&#82;", "R");
            pValue = pValue.Replace("&#83;", "S");
            pValue = pValue.Replace("&#84;", "T");
            pValue = pValue.Replace("&#85;", "U");
            pValue = pValue.Replace("&#86;", "V");
            pValue = pValue.Replace("&#87;", "W");
            pValue = pValue.Replace("&#88;", "X");
            pValue = pValue.Replace("&#89;", "Y");
            pValue = pValue.Replace("&#90;", "Z");
            pValue = pValue.Replace("&#91;", "[");
            pValue = pValue.Replace("&#92;", @"\");
            pValue = pValue.Replace("&#93;", "]");
            pValue = pValue.Replace("&#94;", "^");
            pValue = pValue.Replace("&#95;", "_");
            pValue = pValue.Replace("&#96;", "`");
            pValue = pValue.Replace("&#97;", "a");
            pValue = pValue.Replace("&#98;", "b");
            pValue = pValue.Replace("&#99;", "c");
            pValue = pValue.Replace("&#100;", "d");
            pValue = pValue.Replace("&#101;", "e");
            pValue = pValue.Replace("&#102;", "f");
            pValue = pValue.Replace("&#103;", "g");
            pValue = pValue.Replace("&#104;", "h");
            pValue = pValue.Replace("&#105;", "i");
            pValue = pValue.Replace("&#106;", "j");
            pValue = pValue.Replace("&#107;", "k");
            pValue = pValue.Replace("&#108;", "l");
            pValue = pValue.Replace("&#109;", "m");
            pValue = pValue.Replace("&#110;", "n");
            pValue = pValue.Replace("&#111;", "o");
            pValue = pValue.Replace("&#112;", "p");
            pValue = pValue.Replace("&#113;", "q");
            pValue = pValue.Replace("&#114;", "r");
            pValue = pValue.Replace("&#115;", "s");
            pValue = pValue.Replace("&#116;", "t");
            pValue = pValue.Replace("&#117;", "u");
            pValue = pValue.Replace("&#118;", "v");
            pValue = pValue.Replace("&#119;", "w");
            pValue = pValue.Replace("&#120;", "x");
            pValue = pValue.Replace("&#121;", "y");
            pValue = pValue.Replace("&#122;", "z");
            pValue = pValue.Replace("&#123;", "{");
            pValue = pValue.Replace("&#124;", "|");
            pValue = pValue.Replace("&#125;", "}");
            pValue = pValue.Replace("&#126;", "~");
            pValue = pValue.Replace("&#127;", "");
            pValue = pValue.Replace("&#128;", "€");
            pValue = pValue.Replace("&#129;", "");
            pValue = pValue.Replace("&#130;", "‚");
            pValue = pValue.Replace("&#131;", "ƒ");
            pValue = pValue.Replace("&#132;", "„");
            pValue = pValue.Replace("&#133;", "…");
            pValue = pValue.Replace("&#134;", "†");
            pValue = pValue.Replace("&#135;", "‡");
            pValue = pValue.Replace("&#136;", "ˆ");
            pValue = pValue.Replace("&#137;", "‰");
            pValue = pValue.Replace("&#138;", "Š");
            pValue = pValue.Replace("&#139;", "‹");
            pValue = pValue.Replace("&#140;", "Œ");
            pValue = pValue.Replace("&#141;", "");
            pValue = pValue.Replace("&#142;", "Ž");
            pValue = pValue.Replace("&#143;", "");
            pValue = pValue.Replace("&#144;", "");
            pValue = pValue.Replace("&#145;", "‘");
            pValue = pValue.Replace("&#146;", "’");
            pValue = pValue.Replace("&#147;", "“");
            pValue = pValue.Replace("&#148;", "”");
            pValue = pValue.Replace("&#149;", "•");
            pValue = pValue.Replace("&#150;", "–");
            pValue = pValue.Replace("&#151;", "—");
            pValue = pValue.Replace("&#152;", "˜");
            pValue = pValue.Replace("&#153;", "™");
            pValue = pValue.Replace("&#154;", "š");
            pValue = pValue.Replace("&#155;", "›");
            pValue = pValue.Replace("&#156;", "œ");
            pValue = pValue.Replace("&#157;", "");
            pValue = pValue.Replace("&#158;", "ž");
            pValue = pValue.Replace("&#159;", "Ÿ");
            pValue = pValue.Replace("&#160;", " ");
            pValue = pValue.Replace("&#161;", "¡");
            pValue = pValue.Replace("&#162;", "¢");
            pValue = pValue.Replace("&#163;", "£");
            pValue = pValue.Replace("&#164;", "¤");
            pValue = pValue.Replace("&#165;", "¥");
            pValue = pValue.Replace("&#166;", "¦");
            pValue = pValue.Replace("&#167;", "§");
            pValue = pValue.Replace("&#168;", "¨");
            pValue = pValue.Replace("&#169;", "©");
            pValue = pValue.Replace("&#170;", "ª");
            pValue = pValue.Replace("&#171;", "«");
            pValue = pValue.Replace("&#172;", "¬");
            pValue = pValue.Replace("&#173;", "&shy;");
            pValue = pValue.Replace("&#174;", "®");
            pValue = pValue.Replace("&#175;", "¯");
            pValue = pValue.Replace("&#176;", "°");
            pValue = pValue.Replace("&#177;", "±");
            pValue = pValue.Replace("&#178;", "²");
            pValue = pValue.Replace("&#179;", "³");
            pValue = pValue.Replace("&#180;", "´");
            pValue = pValue.Replace("&#181;", "µ");
            pValue = pValue.Replace("&#182;", "¶");
            pValue = pValue.Replace("&#183;", "·");
            pValue = pValue.Replace("&#184;", "¸");
            pValue = pValue.Replace("&#185;", "¹");
            pValue = pValue.Replace("&#186;", "º");
            pValue = pValue.Replace("&#187;", "»");
            pValue = pValue.Replace("&#188;", "¼");
            pValue = pValue.Replace("&#189;", "½");
            pValue = pValue.Replace("&#190;", "¾");
            pValue = pValue.Replace("&#191;", "¿");
            pValue = pValue.Replace("&#192;", "À");
            pValue = pValue.Replace("&#193;", "Á");
            pValue = pValue.Replace("&#194;", "Â");
            pValue = pValue.Replace("&#195;", "Ã");
            pValue = pValue.Replace("&#196;", "Ä");
            pValue = pValue.Replace("&#197;", "Å");
            pValue = pValue.Replace("&#198;", "Æ");
            pValue = pValue.Replace("&#199;", "Ç");
            pValue = pValue.Replace("&#200;", "È");
            pValue = pValue.Replace("&#201;", "É");
            pValue = pValue.Replace("&#202;", "Ê");
            pValue = pValue.Replace("&#203;", "Ë");
            pValue = pValue.Replace("&#204;", "Ì");
            pValue = pValue.Replace("&#205;", "Í");
            pValue = pValue.Replace("&#206;", "Î");
            pValue = pValue.Replace("&#207;", "Ï");
            pValue = pValue.Replace("&#208;", "Ð");
            pValue = pValue.Replace("&#209;", "Ñ");
            pValue = pValue.Replace("&#210;", "Ò");
            pValue = pValue.Replace("&#211;", "Ó");
            pValue = pValue.Replace("&#212;", "Ô");
            pValue = pValue.Replace("&#213;", "Õ");
            pValue = pValue.Replace("&#214;", "Ö");
            pValue = pValue.Replace("&#215;", "×");
            pValue = pValue.Replace("&#216;", "Ø");
            pValue = pValue.Replace("&#217;", "Ù");
            pValue = pValue.Replace("&#218;", "Ú");
            pValue = pValue.Replace("&#219;", "Û");
            pValue = pValue.Replace("&#220;", "Ü");
            pValue = pValue.Replace("&#221;", "Ý");
            pValue = pValue.Replace("&#222;", "Þ");
            pValue = pValue.Replace("&#223;", "ß");
            pValue = pValue.Replace("&#224;", "à");
            pValue = pValue.Replace("&#225;", "á");
            pValue = pValue.Replace("&#226;", "â");
            pValue = pValue.Replace("&#227;", "ã");
            pValue = pValue.Replace("&#228;", "ä");
            pValue = pValue.Replace("&#229;", "å");
            pValue = pValue.Replace("&#230;", "æ");
            pValue = pValue.Replace("&#231;", "ç");
            pValue = pValue.Replace("&#232;", "è");
            pValue = pValue.Replace("&#233;", "é");
            pValue = pValue.Replace("&#234;", "ê");
            pValue = pValue.Replace("&#235;", "ë");
            pValue = pValue.Replace("&#236;", "ì");
            pValue = pValue.Replace("&#237;", "í");
            pValue = pValue.Replace("&#238;", "î");
            pValue = pValue.Replace("&#239;", "ï");
            pValue = pValue.Replace("&#240;", "ð");
            pValue = pValue.Replace("&#241;", "ñ");
            pValue = pValue.Replace("&#242;", "ò");
            pValue = pValue.Replace("&#243;", "ó");
            pValue = pValue.Replace("&#244;", "ô");
            pValue = pValue.Replace("&#245;", "õ");
            pValue = pValue.Replace("&#246;", "ö");
            pValue = pValue.Replace("&#247;", "÷");
            pValue = pValue.Replace("&#248;", "ø");
            pValue = pValue.Replace("&#249;", "ù");
            pValue = pValue.Replace("&#250;", "ú");
            pValue = pValue.Replace("&#251;", "û");
            pValue = pValue.Replace("&#252;", "ü");
            pValue = pValue.Replace("&#253;", "ý");
            pValue = pValue.Replace("&#254;", "þ");
            pValue = pValue.Replace("&#255;", "ÿ");
            pValue = pValue.Replace("&amp;", "&");
            pValue = pValue.Replace("&quot;", "\"");
            pValue = pValue.Replace("&lt;", "<");
            pValue = pValue.Replace("&gt;", ">");
            pValue = pValue.Replace("&Agrave;", "À");
            pValue = pValue.Replace("&Aacute;", "Á");
            pValue = pValue.Replace("&Acirc;", "Â");
            pValue = pValue.Replace("&Atilde;", "Ã");
            pValue = pValue.Replace("&Auml;", "Ä");
            pValue = pValue.Replace("&Aring;", "Å");
            pValue = pValue.Replace("&AElig;", "Æ");
            pValue = pValue.Replace("&Ccedil;", "Ç");
            pValue = pValue.Replace("&Egrave;", "È");
            pValue = pValue.Replace("&Eacute;", "É");
            pValue = pValue.Replace("&Ecirc;", "Ê");
            pValue = pValue.Replace("&Euml;", "Ë");
            pValue = pValue.Replace("&Igrave;", "Ì");
            pValue = pValue.Replace("&Iacute;", "Í");
            pValue = pValue.Replace("&Icirc;", "Î");
            pValue = pValue.Replace("&Iuml;", "Ï");
            pValue = pValue.Replace("&ETH;", "Ð");
            pValue = pValue.Replace("&Ntilde;", "Ñ");
            pValue = pValue.Replace("&Otilde;", "Õ");
            pValue = pValue.Replace("&Ouml;", "Ö");
            pValue = pValue.Replace("&Ouml;", "Ø");
            pValue = pValue.Replace("&Oslash;", "Ø");
            pValue = pValue.Replace("&copy;", "©");
            pValue = pValue.Replace("&reg;", "®");
            pValue = pValue.Replace(" ", " ");
            return pValue;
        }


        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ' || c == '&' || c == '\n' || c == '"' || c == ',' || c == '%' || c == '-')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();

        }


    }
}