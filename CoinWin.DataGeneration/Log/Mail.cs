using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using QPP.MES.BLL;
using QPP.MES.Domain;
using System.Configuration;
using System.Data;

/// <summary>
///  E-Mail发送对象Helper类
/// </summary>
/// <remarks>
/// ******************************************
/// 創建人：IT_xiaoyh
/// 創建時間：2018/09/25 14:30:48
/// ******************************************
/// </remarks>
namespace CheckDevice
{
    public class MailHelper
    {

        /// <summary>
        /// 發送郵件
        /// </summary>
        /// <param name="subject">主題</param>
        /// <param name="msg">內容</param>
        /// <param name="isCC">是否抄送</param>
        /// <returns></returns>
        static public bool Send(string subject, string msg, bool isCC)
        {
            string ip = ConfigurationManager.AppSettings["Host"].ToString();
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());
            try
            {
                string to = ConfigurationManager.AppSettings["SendTo"].ToString();
                string cc = ConfigurationManager.AppSettings["CopyTo"].ToString();
                string postmaster = ConfigurationManager.AppSettings["PostMaster"].ToString();
                string pwd = ConfigurationManager.AppSettings["Pwd"].ToString();
                //Config conf =
                //ip = "192.168.0.60";//邮件服务器SMTP
                //port =25;//邮件服务器端口
                //to = "fishyue@local.qpp.com.cn"; //發送
                //cc = "lausf@local.qpp.com.cn"; //抄送
                //postmaster = "fishyue@local.qpp.com.cn"; //發件人地址

                System.Net.Mail.SmtpClient sc = new SmtpClient(ip, port); //创建smtp实例对象
                MailMessage mm = new MailMessage();
                mm.Sender = new MailAddress(postmaster);
                mm.Subject = subject;   //郵件標題
                mm.Body = msg;   //錯誤信息
                mm.IsBodyHtml = true;
                mm.SubjectEncoding = Encoding.UTF8;//标题编码

                mm.BodyEncoding = Encoding.UTF8;
                mm.From = new MailAddress(postmaster);
                sc.Credentials = new System.Net.NetworkCredential(postmaster, pwd);//邮件服务器验证信息       
                string[] v;
                v = to.Split(';', ',');

                //if (v == null || (v != null && v.Length == 0))
                //{
                //    LogHelper.WriteLog("配置文件中沒有設定收件人地址或者地址無效, 無法發送郵件.");
                //    return false;
                //}

                foreach (string t in v)
                {
                    mm.To.Add(new MailAddress(t));
                }

                if (isCC)
                {
                    v = cc.Split(';', ',');

                    if (v != null)
                    {
                        foreach (string t in v)
                        {
                            if (string.IsNullOrEmpty(t) == false)
                            {
                                mm.CC.Add(new MailAddress(t));
                            }
                        }
                    }
                }
                sc.Send(mm);
                return true;

            }
            catch (Exception e)
            {

            }
            return false;
        }


        //单一表格邮件内容
        public static string  SendMsg(DataTable data)
        {
            string MailBody = "<p style=\"font-size: 10pt\">以下内容为系统自动发送，请勿直接回复，谢谢。</p><table cellspacing=\"1\" cellpadding=\"3\" border=\"0\" bgcolor=\"000000\" style=\"font-size: 10pt;line-height: 15px;\">";
            MailBody += "<div align=\"center\">";
            MailBody += "<tr>";
            for (int hcol = 0; hcol < data.Columns.Count; hcol++)
            {
                MailBody += "<td bgcolor=\"999999\">&nbsp;&nbsp;&nbsp;";
                MailBody += data.Columns[hcol].ColumnName;
                MailBody += "&nbsp;&nbsp;&nbsp;</td>";
            }
            MailBody += "</tr>";

            for (int row = 0; row < data.Rows.Count; row++)
            {
                MailBody += "<tr>";
                for (int col = 0; col < data.Columns.Count; col++)
                {
                    MailBody += "<td bgcolor=\"dddddd\">&nbsp;&nbsp;&nbsp;";
                    MailBody += data.Rows[row][col].ToString();
                    MailBody += "&nbsp;&nbsp;&nbsp;</td>";
                }
                MailBody += "</tr>";
            }
            MailBody += "</table>";
            MailBody += "</div>";
            return MailBody;
        }

    }
}
