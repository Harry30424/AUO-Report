using AUO.L6A;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        String S_url = "";


        if(Session["S_url"] ==null)
        {
            if (Request.QueryString["url"] != "")
                S_url = Request.QueryString["url"];
            else
                S_url = "main.aspx";
        }
        else if (String.IsNullOrEmpty(Session["S_url"].ToString()))
        {
            if (Request.QueryString["url"] != "")
                S_url = Request.QueryString["url"];
            else
                S_url = "main.aspx";
        }
        else
           
            S_url = Session["S_url"].ToString();

        //ViewState["S_url"] = Request.Url.AbsoluteUri.Split({ "URL="})[1].Replace("!", "&");
        ViewState["S_url"] = Regex.Split(Request.Url.AbsoluteUri, "URL=", RegexOptions.IgnoreCase)[1].Replace("!", "&");


        getUAC();


        Session["S_url"] = null;
        Session["Logined"] = 1;
        Response.Redirect(ViewState["S_url"].ToString());
        Response.Write(Session["UserName"]);
    }

    public void getUAC()
    {
        String sMessage = Request.Params["uacReturnMessage"];
        UAC_decoder UAC_decoder = new UAC_decoder(Request, "big5");
        String U_url = Request.Url.AbsoluteUri;
        if (!String.IsNullOrEmpty(sMessage) && Session["UserID"]==null)
        {
            String[] itemArray = new String[14];

            if (UAC_decoder.err == null)
            {
                int i = 0;
                foreach (String k in UAC_decoder.result.Keys)
                {
                    itemArray[i] = UAC_decoder.result[k];
                    i = i + 1;
                }

                /* '0  UserID =>= 1103370
                 '1  Name =>= 林英佐
                 '2  Alias ==> yingtsolin
                 '3  DeptNo =>= ABD0A0
                 '4  DeptarementName =>= 先進製造技術部
                 '5  Location =>= L6A



                 '6  Authkey =>= 113073.0F05171920789733
                 '7  Jobtitle =>= IDL
                 '8  ClassNo =>= 12
                 '9  Bossno =>= 9929008
                 '10 EMail =>= YingTso.Lin@auo.com
                 '11 Tel =>= 8606 - 5720;
                 '12 ReturnCode =>= Pass
                 '13 ReturnMsg =>= UAC - 000 : 通過(hcuac01)
                 */

                Session["uac_name"] = itemArray[1];
                Session["UserID"] = itemArray[0];
                Session["UserName"] = itemArray[1];
                Session["UserAlias"] = itemArray[2];
                Session["UserDept"] = itemArray[3];
                Session["UserDeptName"] = itemArray[4];
                Session["UserLocation"] = itemArray[5];
                Session["UserJobtitle"] = itemArray[7];
                Session["UserBossbo"] = itemArray[9];
                Session["UserMail"] = itemArray[10];
                Session["UserTel"] = itemArray[11].ToString().Replace(";", "");


                Session["Logined"] = 1;


            }
        }
        else if (!String.IsNullOrEmpty(sMessage) && Session["UserID"].ToString() == "")
        {
            String[] itemArray = new String[14];

            if (UAC_decoder.err==null)
            {
                int i = 0;
                foreach (String k in UAC_decoder.result.Keys)
                {
                    itemArray[i] = UAC_decoder.result[k];
                    i = i + 1;
                }

                /* '0  UserID =>= 1103370
                 '1  Name =>= 林英佐
                 '2  Alias ==> yingtsolin
                 '3  DeptNo =>= ABD0A0
                 '4  DeptarementName =>= 先進製造技術部
                 '5  Location =>= L6A



                 '6  Authkey =>= 113073.0F05171920789733
                 '7  Jobtitle =>= IDL
                 '8  ClassNo =>= 12
                 '9  Bossno =>= 9929008
                 '10 EMail =>= YingTso.Lin@auo.com
                 '11 Tel =>= 8606 - 5720;
                 '12 ReturnCode =>= Pass
                 '13 ReturnMsg =>= UAC - 000 : 通過(hcuac01)
                 */

                Session["uac_name"] = itemArray[1];
                Session["UserID"] = itemArray[0];
                Session["UserName"] = itemArray[1];
                Session["UserAlias"] = itemArray[2];
                Session["UserDept"] = itemArray[3];
                Session["UserDeptName"] = itemArray[4];
                Session["UserLocation"] = itemArray[5];
                Session["UserJobtitle"] = itemArray[7];
                Session["UserBossbo"] = itemArray[9];
                Session["UserMail"] = itemArray[10];
                Session["UserTel"] = itemArray[11].ToString().Replace(";", "");


                Session["Logined"] = 1;


            }
        }
        else
            Response.Redirect("http://cimuac:7102/UAC/?UL=" + U_url);
    }
}