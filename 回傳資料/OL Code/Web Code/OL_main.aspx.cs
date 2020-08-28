using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing.Imaging;
using ChartDirector;

public partial class web : System.Web.UI.Page
{
    static String connString = "Database=l7b_olmap;Data Source=host_name;User Id=userid;Password=userpw;CharSet=utf8;SslMode=None;allowPublicKeyRetrieval=true";
    Color[] color = { Color.Blue, Color.Red, Color.Brown, Color.Green, Color.Purple, Color.Yellow, Color.Orange, Color.GreenYellow, Color.DarkGray, Color.Gold };
    int color_index = 1;
    int wid = 400;
    int hei = 400;

    int x_start = 4; //基準點 x
    int y_start = 4; //基準點 y
    int d = 11; // 板子大小
    int ratio = 10; //縮放倍率 //1格 10 pixel

    protected void Page_Load(object sender, EventArgs e)
    {
        L_title.Text = "<a href='./OL_main.aspx' target='_self' style='text-decoration:none; color:black;'>OL Mapping Query</a>";

        if (!IsPostBack)
        {

            start_time.Text = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
            end_time.Text = DateTime.Now.ToString("yyyy-MM-dd");
            init_BT();
            try {
                init_model();
                init_expo_tool();
                getdata();
            }
            catch (Exception ex)
            {
                lb_error.Text = ex.Message;
            }
        }

    }


    
    
    public void init_expo_tool()
    {
        String sql = "";
        sql += "SELECT distinct concat(substring(process_tool,1,length(process_tool)-1),'0') as expo_tool FROM l7b_olmap.l7bb1_olmap where ( process_tool like '%ALN%' ) order by expo_tool;";
        DataTable dt = GetDTL7B_mysql(sql);
        list_expo_tool.Items.Clear();
        list_expo_tool.Items.Add("ALL");
        foreach (DataRow r in dt.Rows)
        {
            list_expo_tool.Items.Add(r["expo_tool"].ToString());
        }
    }

    public void init_model()
    {
        String sql = "";
        sql += "SELECT distinct SUBSTRING_INDEX(chart_id, '/', 1) as model FROM l7b_olmap.l7bb1_olmap order by model asc";
        DataTable dt = GetDTL7B_mysql(sql);
        list_model.Items.Clear();
        list_model.Items.Add("ALL");
        foreach (DataRow r in dt.Rows)
        {
            list_model.Items.Add(r["model"].ToString());
        }
    }

    static DataTable GetDTL7B_mysql(String SQL)
    {
        MySqlConnection conn = new MySqlConnection(connString);
        conn.Open();
        MySqlDataAdapter DA = new MySqlDataAdapter(SQL, conn);
        DataTable DT = new DataTable();
        DA.Fill(DT);
        conn.Close();
        conn.Dispose();
        return DT;
    }
    public DataTable getdata()
    {
        String time_type = "";
        if (rb_time.SelectedValue == "Meas Time")
            time_type = "aoi_meas_time";
        if (rb_time.SelectedValue == "Process Time")
            time_type = "eqp_process_time";


        String sql = "";


        sql += " select olmap.*,config.* ,'' as OOS,'' as OOC from ( ";
        sql += " SELECT DATE_FORMAT(aoi_meas_time,'%Y-%m-%d %H:%i:%S') as aoi_meas_time,SUBSTRING_INDEX(chart_id, '/', 1) as chart,chart_id,sheet_id,meas_tool   ";
        sql += " ,concat(substring(process_tool,1,length(process_tool)-1),'0') as tool_id,";
        sql += " case when unit is not null then unit ";
        sql += " when unit is null and substring(process_tool,length(process_tool))<>'0' then process_tool ";
        sql += " else null end as unit, ";
        sql += " DATE_FORMAT(eqp_process_time,'%Y-%m-%d %H:%i:%S') as eqp_process_time    ";
        sql += " ,actual_recipe,product_code as model,Recipe_MaskName,layer   ";
        sql += " ,CONCAT(ROUND(TIMESTAMPDIFF(SECOND,start_time,end_time)/60,1),'') as diff,   ";
        sql += " AlignF_RLX_S2 - RRX_S2 as diff_X_S2, ";
        sql += "  concat(case when AlignOffset_X_S1 is null and AlignOffset_X_S2 is not null then 'na' else AlignOffset_X_S1 end,'/',AlignOffset_X_S2,'/',AlignOffset_X_S3,'/',AlignOffset_X_S4) as oft_x, ";
        sql += "  concat(AlignOffset_Y_S1,'/',AlignOffset_Y_S2,'/',AlignOffset_Y_S3,'/',AlignOffset_Y_S4) as oft_y, ";
        sql += "  concat(AlignOffset_Z_S1,'/',AlignOffset_Z_S2,'/',AlignOffset_Z_S3,'/',AlignOffset_Z_S4) as oft_t, ";

        sql += " substring(PSA,length(PSA)-4,length(PSA)) as PSA,CP_R_TEMP_R,STAGE_R_TEMP1_R,STAGE_R_TEMP2_R,CP_L_TEMP_R,STAGE_L_TEMP1_R,STAGE_L_TEMP2_R,   ";
        sql += " X,   ";
        sql += " Y,   ";
        sql += " DATE_FORMAT(cvd_logofftime,'%Y-%m-%d %H:%i:%s') as cvd_time,";
        sql += " case when cvd_eqp <> '' then concat(replace(cvd_eqp,'AD',''),'_',cvd_chb) else '' end as cvd ";
        sql += " FROM l7b_olmap.l7bb1_olmap  ";
        sql += " where 1=1   ";
        if (tb_sheet_id.Text != "")
            sql += " and sheet_id = '" + tb_sheet_id.Text + "'";
        else
        {
            sql += " and " + time_type + " >= '" + start_time.Text + " 00:00:00' ";
            sql += " and " + time_type + " <= '" + end_time.Text + " 23:59:59' ";

            if (RB_Layer.SelectedValue != "ALL")
                sql += "and layer = '" + RB_Layer.SelectedValue + "'";

            if (CB_AOI10.Checked && CB_TTP10.Checked)
                sql += " and ( meas_tool='FDAAOI10' or meas_tool='FDATTP10')";
            else if (CB_AOI10.Checked)
                sql += " and meas_tool='FDAAOI10'";
            else if (CB_TTP10.Checked)
                sql += " and meas_tool='FDATTP10'";

            if (list_expo_tool.SelectedValue != "ALL")
                sql += "and concat(substring(process_tool,1,length(process_tool)-1),'0') = '" + list_expo_tool.SelectedValue + "'";

            if (list_model.SelectedValue != "ALL")
                sql += "and chart_id like '" + list_model.SelectedValue + "%'";
        }
        sql += " ) olmap ";
        sql += " left join ";
        sql += " ( ";
        sql += " SELECT x.chart_id,x.point_count,x.chip_point,x.shot_point,x.map_point,x.x_offset,x.y_offset,x.stiching_point,x.shot_count, ";
        sql += " x.value_LSL as x_value_LSL,x.value_LCL as x_value_LCL,x.value_UCL as x_value_UCL,x.value_USL as x_value_USL,x.value_avg as x_value_avg, ";
        sql += " x.stitch_LSL as x_stitch_LSL,x.stitch_LCL as x_stitch_LCL,x.stitch_UCL as x_stitch_UCL,x.stitch_USL as x_stitch_USL,x.stitch_avg as x_stitch_avg, ";
        sql += " x.delta_LSL as x_delta_LSL,x.delta_LCL as x_delta_LCL,x.delta_UCL as x_delta_UCL,x.delta_USL as x_delta_USL,x.delta_avg as x_delta_avg, ";
        sql += " y.value_LSL as y_value_LSL,y.value_LCL as y_value_LCL,y.value_UCL as y_value_UCL,y.value_USL as y_value_USL,y.value_avg as y_value_avg, ";
        sql += " y.stitch_LSL as y_stitch_LSL,y.stitch_LCL as y_stitch_LCL,y.stitch_UCL as y_stitch_UCL,y.stitch_UCL as y_stitch_USL,y.stitch_avg as y_stitch_avg, ";
        sql += " y.delta_LSL as y_delta_LSL,y.delta_LCL as y_delta_LCL,y.delta_UCL as y_delta_UCL,y.delta_USL as y_delta_USL,y.delta_avg as y_delta_avg ";
        sql += " FROM l7b_olmap.l7bb1_olmap_config as x , l7b_olmap.l7bb1_olmap_config as y ";
        sql += " where concat(x.chart_id,'_Y') = y.chart_id ";
        sql += " ) config ";
        sql += " on olmap.chart_id = config.chart_id ";

        sql += " order by " + time_type + " desc";





        DataTable dt = GetDTL7B_mysql(sql);
        //test1.Text = sql;

        //DataTable dt = new DataTable();
        dt = change_point_order(dt);
        //GridView1.Columns[1].HeaderText = rb_time.SelectedValue.Replace(" ","<br>");
        GridView1.DataSource = dt;
        GridView1.DataBind();
        
        return dt;        

    }
    public DataTable change_point_order(DataTable dt)
    {
        String[] tp = { "value", "stitch", "delta" };
        String[] tp2 = { "x", "y"};
        for (int i = 0;i<dt.Rows.Count;i++)
        {
            String ng_oos = "";
            String ng_ooc = "";
            String ngop_oos = "";
            String ngop_ooc = "";

            foreach (String t in tp)
            {
                foreach (String t2 in tp2)
                {
                    int ii = judge_value(dt.Rows[i][t2].ToString(), dt.Rows[i][t2 + "_" + t + "_avg"].ToString(), dt.Rows[i][t2 + "_" + t + "_LSL"].ToString(), dt.Rows[i][t2 + "_" + t + "_USL"].ToString(), dt.Rows[i][t2 + "_" + t + "_LCL"].ToString(), dt.Rows[i][t2 + "_" + t + "_UCL"].ToString(), t, dt.Rows[i]["stiching_point"].ToString());
                    if (ii==2)
                    {
                        ng_oos += ngop_oos + t + "_" + t2;
                        ngop_oos = "<br>";
                    }
                    else if(ii==1)
                    {
                        ng_ooc +=  ngop_ooc + t + "_" + t2 ;
                        ngop_ooc = "<br>";
                    }

                }
            }
            dt.Rows[i]["OOS"] = ng_oos.ToUpper();
            dt.Rows[i]["OOC"] = ng_ooc.ToUpper();

            String X_new = "";
            String Y_new = "";
            String x_stich = "";
            String y_stich = "";
            String op = "";
            String[] X = dt.Rows[i]["X"].ToString().Split(',');
            String[] Y = dt.Rows[i]["Y"].ToString().Split(',');
            String[] order = dt.Rows[i]["map_point"].ToString().Split(',');
            String[] stich_order = dt.Rows[i]["stiching_point"].ToString().Split(',');
            try {
                foreach (String s in order)
                {                    
                    X_new += op + X[int.Parse(s) - 1];
                    Y_new += op + Y[int.Parse(s) - 1];
                    op = ",";
                }

                op = "";
                foreach (String s in stich_order)
                {
                    x_stich += op + X[int.Parse(s) - 1];
                    y_stich += op + Y[int.Parse(s) - 1];
                    op = ",";
                }

                dt.Rows[i]["X"] = X_new;
                dt.Rows[i]["Y"] = Y_new;
                dt.Rows[i]["stiching_point"] = x_stich;

            }
            catch
            {
                dt.Rows[i]["X"] = "";
                dt.Rows[i]["Y"] = "";
                dt.Rows[i]["stiching_point"] = "";
            }

            

        }

        return dt;
    }

    int judge_value(String value_str, String avg, String LSL_str, String USL_str, String LCL_str, String UCL_str, String type = "value", String stitch_point_str = "") //平均 / 下限 / 上限
    {

        int judge_ng = 0;

        double LSL = -99999;
        double LCL = -99999;
        double USL = 99999;
        double UCL = 99999;

        if (LSL_str != "")
            LSL = double.Parse(LSL_str);
        if (LCL_str != "")
            LCL = double.Parse(LCL_str);
        if (USL_str != "")
            USL = double.Parse(USL_str);
        if (UCL_str != "")
            UCL = double.Parse(UCL_str);



        String[] value_temp = value_str.Split(',');
        List<double> value_list = new List<double>();



        if (type == "value")
        {
            try
            {
                foreach (String s in value_temp)
                    value_list.Add(double.Parse(s));
            }
            catch
            {
                return 3;
            }
        }
        else
        {
            String[] stitch_temp = stitch_point_str.Split(',');
            List<double> stitch_value_list = new List<double>();
            try
            {
                foreach (String s in stitch_temp)
                {
                    stitch_value_list.Add(double.Parse(value_temp[int.Parse(s) - 1]));
                    //Console.WriteLine(s);
                }
            }
            catch (Exception ee)
            {
                return 3;
            }

            if (type == "stitch")
            {
                value_list = stitch_value_list;
            }
            else
            {
                double[] sti_value = stitch_value_list.ToArray();


                for (int i = 0; i < sti_value.Length / 4; i++)
                {
                    double diff_temp = sti_value[i + sti_value.Length / 4] - sti_value[i];
                    value_list.Add(diff_temp);
                }

                for (int i = sti_value.Length / 2; i < sti_value.Length / 4 * 3; i++)
                {
                    double diff_temp = -1 * (sti_value[i + sti_value.Length / 4] - sti_value[i]);
                    value_list.Add(diff_temp);
                }

            }
        }


        double[] value = value_list.ToArray();

        double total_value = 0;
        foreach (double d in value)
        {
            // Console.WriteLine(USL + "," + d.ToString() + "," + LSL);

            total_value += d;
            if (d >= USL || d <= LSL)
            {
                if (avg == "0")
                {
                    //Console.WriteLine(USL + "," + d.ToString() + "," + LSL);
                    judge_ng = 2;
                    return judge_ng;
                }
            }
            else if (d >= UCL || d <= LCL)
            {
                if (avg == "0")
                {
                    //Console.WriteLine(UCL + "," + d.ToString() + "," + LCL);
                    judge_ng = 1;
                    return judge_ng;
                }
            }
        }
        double avg_value = total_value / value.Length;



        if (avg == "1")
        {
            judge_ng = 0;
            if (avg_value >= USL || avg_value <= LSL)
            {
                judge_ng = 2;

            }
            else if (avg_value >= UCL || avg_value <= LCL)
            {
                judge_ng = 1;

            }
        }

        return judge_ng;

    }


    public void boxplot(String stiching_point,int shot_count, WebChartViewer w)
    {

        
        String[] sti_temp = stiching_point.Split(',');
        List<double> temp_x_list = new List<double>();
        List<double> delta_x1_list = new List<double>();
        List<double> delta_x2_list = new List<double>();
        double delta_s1s2_avg = 0;
        double delta_s3s4_avg = 0;
        


        //String sss = "";
        foreach (String s in sti_temp)
        {
            temp_x_list.Add(double.Parse(s));
            // += mp_x_temp[i-1] + "//";
        }
        //L_title.Text = sss;


        double[] mp_x = temp_x_list.ToArray();

        

       
        

        int count = mp_x.Length / shot_count;

        List<double> l_x = new List<double>();
        List<double> Q0Data = new List<double>();
        List<double> Q1Data = new List<double>();
        List<double> Q2Data = new List<double>();
        List<double> Q3Data = new List<double>();
        List<double> Q4Data = new List<double>();
        List<double> avgData = new List<double>();

        for (int i = 0; i < shot_count; i++)
        {
            l_x = new List<double>();
            double total = 0;
            for (int j = 0; j < count; j++)
            {
                l_x.Add(mp_x[count * i + j]);
                total += mp_x[count * i + j];
                //l_y.Add(mp_y[count * i + j]);
            }
            avgData.Add(total / count);
            double[] temp_x = l_x.ToArray();
            //double[] temp_y = l_y.ToArray();
            Array.Sort(temp_x);
            //Array.Sort(temp_y);
            Q0Data.Add(temp_x[0]);
            //Q0Data.Add(temp_y[0]);
            Q4Data.Add(temp_x[temp_x.Length - 1]);
            //Q4Data.Add(temp_y[temp_y.Length-1]);
            int q1_count = Convert.ToInt32(Math.Floor(temp_x.Length * 0.25));
            int q2_count = Convert.ToInt32(Math.Floor(temp_x.Length * 0.5));
            int q3_count = Convert.ToInt32(Math.Floor(temp_x.Length * 0.75));

            Q1Data.Add(temp_x[q1_count]);
            Q2Data.Add(temp_x[q2_count]);
            Q3Data.Add(temp_x[q3_count]);
            
        }
        
        if (shot_count == 4)
        //shot_count=4要有delta
        {
            for (int i = 0; i < mp_x.Length / 4; i++)
            {
                double diff_temp = mp_x[i + mp_x.Length / 4] - mp_x[i];
                delta_x1_list.Add(diff_temp);
                delta_s1s2_avg += diff_temp;
            }

            for (int i = mp_x.Length / 2; i < mp_x.Length / 4 * 3; i++)
            {
                double diff_temp = -1 * (mp_x[i + mp_x.Length / 4] - mp_x[i]);
                delta_x2_list.Add(diff_temp);
                delta_s3s4_avg += diff_temp;
            }
            double[] delta_s1s2 = delta_x1_list.ToArray();
            double[] delta_s3s4 = delta_x2_list.ToArray();
            Array.Sort(delta_s1s2);
            Array.Sort(delta_s3s4);

            delta_s1s2_avg = delta_s1s2_avg / delta_s1s2.Length;
            delta_s3s4_avg = delta_s3s4_avg / delta_s3s4.Length;


            Q0Data.Add(delta_s1s2[0]);
            Q0Data.Add(delta_s3s4[0]);
            Q1Data.Add(delta_s1s2[Convert.ToInt32(Math.Floor(delta_s1s2.Length * 0.25))]);
            Q1Data.Add(delta_s3s4[Convert.ToInt32(Math.Floor(delta_s3s4.Length * 0.25))]);
            Q2Data.Add(delta_s1s2[Convert.ToInt32(Math.Floor(delta_s1s2.Length * 0.5))]);
            Q2Data.Add(delta_s3s4[Convert.ToInt32(Math.Floor(delta_s3s4.Length * 0.5))]);
            Q3Data.Add(delta_s1s2[Convert.ToInt32(Math.Floor(delta_s1s2.Length * 0.75))]);
            Q3Data.Add(delta_s3s4[Convert.ToInt32(Math.Floor(delta_s3s4.Length * 0.75))]);
            Q4Data.Add(delta_s1s2[delta_s1s2.Length - 1]);
            Q4Data.Add(delta_s3s4[delta_s3s4.Length - 1]);
        }
        
        List<String> lbs = new List<String>();
        for (int i = 1; i <= shot_count; i++)
            lbs.Add("S" + i + "_X");

        if (shot_count == 4)
        {
            lbs.Add("delta_s1s2");
            lbs.Add("delta_s3s4");
        }


        // The labels for the chart
        //string[] labels = { "S1_X", "S1_Y", "S2_X", "S2_Y", "S3_X", "S3_Y", "S4_X", "S4_Y" };
        string[] labels = lbs.ToArray();
        // Create a XYChart object of size 550 x 250 pixels
        XYChart c = new XYChart(550, 250);

        // Set the plotarea at (50, 25) and of size 450 x 200 pixels. Enable both horizontal and
        // vertical grids by setting their colors to grey (0xc0c0c0)
        c.setPlotArea(50, 25, 450, 200).setGridColor(0xc0c0c0, 0xc0c0c0);

        // Add a title to the chart
        c.addTitle("Stitch OLX Boxplot By Shot");

        // Set the labels on the x axis and the font to Arial Bold
        c.xAxis().setLabels(labels).setFontStyle("Arial Bold");
        
        // Set the font for the y axis labels to Arial Bold
        c.yAxis().setLabelStyle("Arial Bold");

        // Add a Box Whisker layer using light blue 0x9999ff as the fill color and blue (0xcc) as the
        // line color. Set the line width to 2 pixels
        c.addBoxWhiskerLayer(Q3Data.ToArray(), Q1Data.ToArray(), Q4Data.ToArray(), Q0Data.ToArray(), Q2Data.ToArray(), 0x9999ff, 0x0000cc).setLineWidth(2);


        
        if (shot_count == 4)
        {
            for (int i = 0; i < shot_count + 2; i++)
            {
                if (i == shot_count)
                    c.addText(Convert.ToInt32(280 / shot_count + 2) * (i) + 70, 200, delta_s1s2_avg.ToString("##0.00"), "Arial", 14);
                else if (i == shot_count + 1)
                    c.addText(Convert.ToInt32(280 / shot_count + 2) * (i) + 70, 200, delta_s3s4_avg.ToString("##0.00"), "Arial", 14);
                else
                    c.addText(Convert.ToInt32(280 / shot_count + 2) * (i) + 70, 200, avgData[i].ToString("##0.00"), "Arial", 14);
            }
        }
        else
        {
            for (int i = 0; i < shot_count; i++)
            c.addText(Convert.ToInt32(450 / shot_count) * (i) + 80, 200, avgData[i].ToString("##0.00"), "Arial", 14);

        }
        


        // Output the chart
        w.Image = c.makeWebImage(ChartDirector.Chart.PNG);

        // Include tool tip for the chart
        w.ImageMap = c.getHTMLImageMap("", "",
            "title='{xLabel}: min/med/max = {min}/{med}/{max}\nInter-quartile range: {bottom} to {top}'"
            );
    }


    public void set_img()
    {
        List<string> position_x = new List<string>();
        List<string> position_y = new List<string>();
        List<string> sheet_id_list = new List<string>();
        List<string> tool_id_list = new List<string>();
        List<string> stich_list = new List<string>();
        List<string> shot_count_list = new List<string>();
        List<string> x_offset_list = new List<string>();
        List<string> y_offset_list = new List<string>();

        List<string> eqp_time_list = new List<string>();
        List<string> aoi_time_list = new List<string>();
        List<string> oos_list = new List<string>();
        List<string> ooc_list = new List<string>();
        List<string> cvd_list = new List<string>();


        for (int i = 0; i<GridView1.Rows.Count;i++)
        {
            CheckBox cb = (CheckBox)GridView1.Rows[i].FindControl("cb_record");
        
            if (cb.Checked)
            {
                position_x.Add(((Label)GridView1.Rows[i].FindControl("g_X")).Text);
                position_y.Add(((Label)GridView1.Rows[i].FindControl("g_Y")).Text);
                sheet_id_list.Add(((Label)GridView1.Rows[i].FindControl("g_label7")).Text);
                tool_id_list.Add(((Label)GridView1.Rows[i].FindControl("g_label4")).Text);
                stich_list.Add(((Label)GridView1.Rows[i].FindControl("g_stiching_point")).Text);
                shot_count_list.Add(((Label)GridView1.Rows[i].FindControl("g_shot_count")).Text);
                x_offset_list.Add(((Label)GridView1.Rows[i].FindControl("g_x_offset")).Text);
                y_offset_list.Add(((Label)GridView1.Rows[i].FindControl("g_y_offset")).Text);

                eqp_time_list.Add(((Label)GridView1.Rows[i].FindControl("g_label5")).Text);
                aoi_time_list.Add(((Label)GridView1.Rows[i].FindControl("g_label1")).Text);
                oos_list.Add(((Label)GridView1.Rows[i].FindControl("g_oos")).Text);
                ooc_list.Add(((Label)GridView1.Rows[i].FindControl("g_ooc")).Text);
                cvd_list.Add(((Label)GridView1.Rows[i].FindControl("g_cvd")).Text);
            }
        }
        String[] x = position_x.ToArray();
        String[] y = position_y.ToArray();
        String[] sheet_id = sheet_id_list.ToArray();
        String[] tool_id = tool_id_list.ToArray();
        String[] stich = stich_list.ToArray();
        String[] shot_count = shot_count_list.ToArray();
        String[] x_offset = x_offset_list.ToArray();
        String[] y_offset = y_offset_list.ToArray();
        String[] eqp_time = eqp_time_list.ToArray();
        String[] aoi_time = aoi_time_list.ToArray();
        String[] oos = oos_list.ToArray();
        String[] ooc = ooc_list.ToArray();
        String[] cvd = cvd_list.ToArray();

        int index = -1;
        /*
        Image1.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[0], y[0]);
        lb_img1.Text = sheet_id[0] + "</br>" + "(" + tool_id[0] + ")";
        Image2.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[1], y[1]);
        lb_img2.Text = sheet_id[1] + "</br>" + "(" + tool_id[1] + ")";
        */

        if (CB_ol.Checked)
        {
            panel_ol.Visible = true;
            color_index = 1;
            Image_ol.ImageUrl = "data:image/jpeg;base64," + TransIntToImage_ol(x, y,sheet_id,tool_id, x_offset, y_offset);
            
        }
        if(CB_avg.Checked && sheet_id.Length>0)
        {
            panel_avg.Visible = true;
            Boolean LL = true;
            int pt_count = 0;

            for (int i = 0; i < x.Length; i++)
            {
                if (pt_count != x[i].Split(',').Length && pt_count != 0)
                {
                    LL = false;
                    lb_error.Text = "無法畫平均點位圖，因所選sheet點位數量不一致!";
                    break;
                }
                else
                    pt_count = x[i].Split(',').Length;

            }
           
            if (LL)
            {
                double[] x_avg = new double[pt_count];
                double[] y_avg = new double[pt_count];
                for (int i = 0; i<x.Length;i++)
                {
                    String[] x_avg_str = x[i].Split(',');
                    String[] y_avg_str = y[i].Split(',');

                    for (int j=0;j<x_avg_str.Length;j++)
                    {
                        x_avg[j] += double.Parse(x_avg_str[j]) / x.Length;
                        y_avg[j] += double.Parse(y_avg_str[j]) / x.Length;
                    }

                }
                String xx = "";
                String yy = "";
                String op = "";
                for (int i=0;i< x_avg.Length;i++)
                {
                    xx += op + x_avg[i].ToString("##0.0000");
                    yy += op + y_avg[i].ToString("##0.0000");
                    op = ",";
                }

                Image_avg.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(xx, yy, int.Parse(x_offset[0]), int.Parse(y_offset[0]));
                Image_value_avg.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(xx, yy);

                List<string> stich_all = new List<string>();


                int st_point = stich[0].Split(',').Length; //單片stich總點數

                String stich_all_str = "";
                foreach(String s in stich)
                {
                    if (stich_all_str != "")
                        stich_all_str += ",";
                    stich_all_str += s;
                }
                String[] stich_all_temp = stich_all_str.Split(',');

                

                for (int i =0;i < int.Parse(shot_count[0]);i++) //幾shot
                {
                    for (int j = 0; j < stich.Length; j++) //總共幾片
                    {
                        for (int k = 0; k < st_point / int.Parse(shot_count[0]); k++) //一個shot幾點
                        {
                            int idx = st_point * j + i * (st_point / int.Parse(shot_count[0])) + k;
                           
                            stich_all.Add(stich_all_temp[idx]);
                        }
                    }
                }
                
                String stich_all_order = "";
                String[] stich_all_order_temp = stich_all.ToArray();
                foreach (String s in stich_all_order_temp)
                {
                    if (stich_all_order != "")
                        stich_all_order += ",";
                    stich_all_order += s;
                }
                boxplot(stich_all_order, int.Parse(shot_count[0]), WebChartViewer_avg);
                
            }
            
        }
        if(!CB_ol.Checked && !CB_avg.Checked)
        {
            panel_imgall.Visible = true;
            for (int i = 0; i < x.Length; i++)
            {
                if (x.Length == y.Length)
                {
                    index++;
                    if (index == 0)
                    {
                        try {
                            Image1.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image1_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);
                            lb_img1.Text = sheet_id[i];
                            lb_img1.Text += "<br>(" + tool_id[i];
                            if(cvd[i]!="")
                                lb_img1.Text += " / " + cvd[i];
                            lb_img1.Text += ")";

                            lb_img1_1.Text  = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img1_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer1);
                            if (oos[i] != "")
                                lb_img1.ForeColor = Color.Red;
                            else if(ooc[i] != "")
                                lb_img1.ForeColor = Color.FromArgb(0xCC6600);
                            else
                                lb_img1.ForeColor = Color.Black;
                        }
                        catch(Exception ee)
                        {
                            lb_img1.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                        

                    }
                    else if (index == 1)
                    {
                        try
                        {
                            Image2.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image2_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);

                            lb_img2.Text = sheet_id[i];
                            lb_img2.Text += "<br>(" + tool_id[i];
                            if (cvd[i] != "")
                                lb_img2.Text += " / " + cvd[i];
                            lb_img2.Text += ")";

                            lb_img2_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img2_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer2);
                            if (oos[i] != "")
                                lb_img2.ForeColor = Color.Red;
                            else if (ooc[i] != "")
                                lb_img2.ForeColor = Color.FromArgb(0xCC6600);
                            else
                                lb_img2.ForeColor = Color.Black;
                        }
                        catch (Exception ee)
                        {
                            lb_img2.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                    }
                    else if (index == 2)
                    {
                        try
                        {
                            Image3.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image3_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);

                            lb_img3.Text = sheet_id[i];
                            lb_img3.Text += "<br>(" + tool_id[i];
                            if (cvd[i] != "")
                                lb_img3.Text += " / " + cvd[i];
                            lb_img3.Text += ")";

                            lb_img3_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img3_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer3);
                            if (oos[i] != "")
                                lb_img3.ForeColor = Color.Red;
                            else if (ooc[i] != "")
                                lb_img3.ForeColor = Color.FromArgb(0xCC6600);
                            else
                                lb_img3.ForeColor = Color.Black;
                        }
                        catch (Exception ee)
                        {
                            lb_img3.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                    }
                    else if (index == 3)
                    {
                        try
                        {
                            Image4.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image4_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);

                            lb_img4.Text = sheet_id[i];
                            lb_img4.Text += "<br>(" + tool_id[i];
                            if (cvd[i] != "")
                                lb_img4.Text += " / " + cvd[i];
                            lb_img4.Text += ")";

                            lb_img4_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img4_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer4);
                            if (oos[i] != "")
                                lb_img4.ForeColor = Color.Red;
                            else if (ooc[i] != "")
                                lb_img4.ForeColor = Color.FromArgb(0xCC6600);
                            else
                                lb_img4.ForeColor = Color.Black;
                        }
                        catch (Exception ee)
                        {
                            lb_img4.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                    }
                    else if (index == 4)
                    {
                        try
                        {
                            Image5.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image5_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);

                            lb_img5.Text = sheet_id[i];
                            lb_img5.Text += "<br>(" + tool_id[i];
                            if (cvd[i] != "")
                                lb_img5.Text += " / " + cvd[i];
                            lb_img5.Text += ")";

                            lb_img5_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img5_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer5);
                            if (oos[i] != "")
                                lb_img5.ForeColor = Color.Red;
                            else if (ooc[i] != "")
                                lb_img5.ForeColor = Color.FromArgb(0xCC6600);
                            else
                                lb_img5.ForeColor = Color.Black;
                        }
                        catch (Exception ee)
                        {
                            lb_img5.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                    }
                    else if (index == 5)
                    {
                        try
                        {
                            Image6.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image6_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);

                            lb_img6.Text = sheet_id[i];
                            lb_img6.Text += "<br>(" + tool_id[i];
                            if (cvd[i] != "")
                                lb_img6.Text += " / " + cvd[i];
                            lb_img6.Text += ")";

                            lb_img6_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img6_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer6);
                            if (oos[i] != "")
                                lb_img6.ForeColor = Color.Red;
                            else if (ooc[i] != "")
                                lb_img6.ForeColor = Color.FromArgb(0xCC6600);
                            else
                                lb_img6.ForeColor = Color.Black;
                        }
                        catch (Exception ee)
                        {
                            lb_img6.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                    }
                    else if (index == 6)
                    {
                        try
                        {
                            Image7.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image7_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);

                            lb_img7.Text = sheet_id[i];
                            lb_img7.Text += "<br>(" + tool_id[i];
                            if (cvd[i] != "")
                                lb_img7.Text += " / " + cvd[i];
                            lb_img7.Text += ")";

                            lb_img7_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img7_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer7);
                            if (oos[i] != "")
                                lb_img7.ForeColor = Color.Red;
                            else if (ooc[i] != "")
                                lb_img7.ForeColor = Color.FromArgb(0xCC6600);
                            else
                                lb_img7.ForeColor = Color.Black;
                        }
                        catch (Exception ee)
                        {
                            lb_img7.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                    }
                    else if (index == 7)
                    {
                        try
                        {
                            Image8.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image8_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);

                            lb_img8.Text = sheet_id[i];
                            lb_img8.Text += "<br>(" + tool_id[i];
                            if (cvd[i] != "")
                                lb_img8.Text += " / " + cvd[i];
                            lb_img8.Text += ")";

                            lb_img8_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img8_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer8);
                            if (oos[i] != "")
                                lb_img8.ForeColor = Color.Red;
                            else if (ooc[i] != "")
                                lb_img8.ForeColor = Color.FromArgb(0xCC6600);
                            else
                                lb_img8.ForeColor = Color.Black;
                        }
                        catch (Exception ee)
                        {
                            lb_img8.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                    }
                    else if (index == 8)
                    {
                        try
                        {
                            Image9.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image9_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);

                            lb_img9.Text = sheet_id[i];
                            lb_img9.Text += "<br>(" + tool_id[i];
                            if (cvd[i] != "")
                                lb_img9.Text += " / " + cvd[i];
                            lb_img9.Text += ")";

                            lb_img9_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img9_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer9);
                            if (oos[i] != "")
                                lb_img9.ForeColor = Color.Red;
                            else if (ooc[i] != "")
                                lb_img9.ForeColor = Color.FromArgb(0xCC6600);
                            else
                                lb_img9.ForeColor = Color.Black;
                        }
                        catch (Exception ee)
                        {
                            lb_img9.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                    }
                    else if (index == 9)
                    {
                        try
                        {
                            Image10.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image10_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);

                            lb_img10.Text = sheet_id[i];
                            lb_img10.Text += "<br>(" + tool_id[i];
                            if (cvd[i] != "")
                                lb_img10.Text += " / " + cvd[i];
                            lb_img10.Text += ")";

                            lb_img10_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img10_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer10);
                            if (oos[i] != "")
                                lb_img10.ForeColor = Color.Red;
                            else if (ooc[i] != "")
                                lb_img10.ForeColor = Color.FromArgb(0xCC6600);
                            else
                                lb_img10.ForeColor = Color.Black;
                        }
                        catch (Exception ee)
                        {
                            lb_img10.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                    }
                    else if (index == 10)
                    {
                        try
                        {
                            Image11.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image11_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);

                            lb_img11.Text = sheet_id[i];
                            lb_img11.Text += "<br>(" + tool_id[i];
                            if (cvd[i] != "")
                                lb_img11.Text += " / " + cvd[i];
                            lb_img11.Text += ")";

                            lb_img11_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img11_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer11);
                            if (oos[i] != "")
                                lb_img11.ForeColor = Color.Red;
                            else if (ooc[i] != "")
                                lb_img11.ForeColor = Color.FromArgb(0xCC6600);
                            else
                                lb_img11.ForeColor = Color.Black;
                        }
                        catch (Exception ee)
                        {
                            lb_img11.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                    }
                    else if (index == 11)
                    {
                        try
                        {
                            Image12.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x[i], y[i], int.Parse(x_offset[i]), int.Parse(y_offset[i]));
                            Image12_value.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x[i], y[i]);

                            lb_img12.Text = sheet_id[i];
                            lb_img12.Text += "<br>(" + tool_id[i];
                            if (cvd[i] != "")
                                lb_img12.Text += " / " + cvd[i];
                            lb_img12.Text += ")";

                            lb_img12_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time[i];
                            lb_img12_1.Text += "</br>Process Time : " + eqp_time[i];
                            boxplot(stich[i], int.Parse(shot_count[i]), WebChartViewer12);
                            if (oos[i] != "")
                                lb_img12.ForeColor = Color.Red;
                            else
                                lb_img12.ForeColor = Color.Black;
                        }
                        catch (Exception ee)
                        {
                            lb_img12.Text = sheet_id[i] + "</br>" + "(" + tool_id[i] + ") 此片點數異常";
                        }
                    }

                }
            }
        }


    }



    protected void BT_select_Click(object sender, EventArgs e)
    {
        try {
            lb_error.Text = "";
            getdata();
            Button1.Text = "查看圖表";
            Button3.Text = "單片補值";
            panel_fillvalue.Visible = false;
            GridView1.Visible = true;
            panel_imgall.Visible = false;
            panel_ol.Visible = false;
            img_init();
            Button1.Visible = true;
            Button3.Visible = true;
            CB_ol.Visible = true;
            Label6.Visible = true;
        }
        catch(Exception ex)
        {
            lb_error.Text = ex.Message;
        }
    }



    public Graphics graph_paint(Graphics Gpi, float[,] ori, float[] offset_x, float[] offset_y,int x ,int y, Boolean r = false,Boolean ol = false)
    {
        Pen pen = new Pen(color[1], 2);
        Pen pen_point = new Pen(color[1], 10);
        Color cl = Color.Red;

        if (r)
        {
            pen = new Pen(color[0], 2);
            pen.DashPattern = new float[] { 5, 2, 6, 4 };
            cl = color[0];
        }
        if(ol)
        {
            cl = color[color_index];
            pen = new Pen(color[color_index++], 2);
        }

        int s_count = offset_x.Length / 4;
        /*
        float x = 0;
        float y = 0;
        if (s_count == 4 || s_count == 6)
        {
            x = -1;
            y = 1;
        }
        else if (s_count == 5)
        {
            x = -1;
            y = -1;
        }
        */
        Brush brush = new SolidBrush(cl);

        // Label1.Text = offset.GetLength(0).ToString() + "//" + s_count.ToString();
        ///16點位
        /// 

        for (int i = 0; i < offset_x.Length; i += s_count)
        {
            for (int j = i; j < i + s_count; j++)
            {
                float x1 = ori[j, 0] + x * offset_x[j] * ratio;
                float y1 = ori[j, 1] - y * offset_y[j] * ratio;
                float x2;
                float y2;


                if (j == i + s_count - 1)
                {
                    x2 = ori[i, 0] + x * offset_x[i] * ratio;
                    y2 = ori[i, 1] - y * offset_y[i] * ratio;
                }
                else
                {
                    x2 = ori[j + 1, 0] + x * offset_x[j + 1] * ratio;
                    y2 = ori[j + 1, 1] - y * offset_y[j + 1] * ratio;
                }

                Gpi.DrawLine(pen, x1, y1, x2, y2);
                if(!r)
                    Gpi.FillEllipse(brush, x1-4, y1-4, 8,8);

            }
        }



        return Gpi;
    }

    public string TransIntTovaluemap(String xx, String yy)
    {

        string[] num_16 = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16" };
        int[,] delta_16 = { { 5, 2 }, { 8, 3 }, { 9, 14 }, { 12, 15 } };
        

        string[] num_20 = { "1", "2", "17", "3", "4", "5", "6", "7", "8", "18", "9", "10", "11", "12", "19", "13", "14", "20", "15", "16" };
        int[,] delta_20 = { { 6, 2 }, { 10, 3 }, { 9, 4 }, { 11, 17 }, { 15, 18 }, { 14, 19 } };

        string[] num_24 = { "1", "4", "5", "6", "3", "2", "7", "10", "11", "12", "9", "8", "13", "16", "17", "18", "15", "14", "19", "22", "23", "24", "21", "20" };
        int[,] delta_24 = { { 7, 2 }, { 12, 3 }, { 11, 4 }, { 13, 20 }, { 18, 21 }, { 17, 22 } };

        float x_zero = wid / 2;
        float y_zero = hei / 2;

        Font ft = new Font("Arial", 10);

        Bitmap Bmp = new Bitmap(wid + 100, hei + 2);  //建立實體圖檔並設定大小
        float wid2 = wid + 100;

        //SolidBrush brush = new SolidBrush(pen.Color);

        Graphics Gpi = Graphics.FromImage(Bmp);

        Gpi.Clear(Color.White);

        MemoryStream stream = new MemoryStream();

        Gpi.Clear(Color.White);

        Pen pen = new Pen(Color.Silver, 1);

        //畫座標
        /*
        Gpi.DrawLine(pen, 0, 0, 0, hei);
        Gpi.DrawLine(pen, 0, hei, wid, hei);
        Gpi.DrawLine(pen, wid, hei, wid, 0);
        Gpi.DrawLine(pen, wid, 0, 0, 0);
        for (int i = 0; i < hei; i += ratio)
            Gpi.DrawLine(pen, 0, i, wid, i);
        for (int i = 0; i < wid; i += ratio)
            Gpi.DrawLine(pen, i, 0, i, hei);
        */

        //X軸Y軸
        pen = new Pen(Color.Green, 2);
        Gpi.DrawLine(pen, 0, y_zero, wid, y_zero);
        Gpi.DrawLine(pen, x_zero, 0, x_zero, hei);


        float x11 = 40;
        float x12 = x11 + 90;
        float x21 = x12 + 120;
        float x22 = x21 + 90;

        float y11 = 20 ;
        float y12 = y11+110 ;
        float y21 = y12+110 ;
        float y22 = y21+110  ;

        float[,] shot_position = { { wid/4-10, 10 }, { wid / 4 * 3, 10 }, { wid / 4 * 3, hei / 2+10 }, { wid / 4 - 10, hei / 2 +10 } };
        



        float[,] label1 = { { x11, y11 }, { x11, y12 }, { x11, y21 }, { x11, y22 }, { x21, y11 }, { x21, y12 }, { x21, y21 }, { x21, y22 } };
        float[,] label2 = { { x11, y11 }, { x11, (y11+y12)/2 }, { x11, y12 }, { x11, y21 }, { x11, (y21 + y22) / 2 }, { x11, y22 }, { x21, y11 }, { x21, (y11 + y12) / 2 }, { x21, y12 }, { x21, y21 }, { x21, (y21 + y22) / 2 }, { x21, y22 } };

        float[,] delta_4_position = { { x_zero-15, y11+15 }, { x_zero-15, y12 + 15 }, { x_zero-15, y21 + 15 }, { x_zero-15, y22 + 15 } };
        float[,] delta_6_position = { { x_zero - 15, y11 + 15 }, { x_zero - 15, (y11 + y12)  / 2 + 15 }, { x_zero - 15, y12 + 15 }, { x_zero - 15, y21 + 15 }, { x_zero - 15, (y21 + y22) / 2 + 15 }, { x_zero - 15, y22 + 15 } };

        float[,] ori_16 = new float[,] { { x11, y11 }, { x12, y11 }, { x12, y12 }, { x11, y12 }
                                        , { x21, y11 }, { x22, y11 }, { x22, y12 }, { x21, y12}
                                        , { x21, y21 }, { x22, y21 }, { x22, y22 }, { x21,y22 }
                                        , { x11, y21 }, { x12, y21 }, { x12, y22 }, { x11,y22 } };

        float[,] ori_20 = new float[,] { { x11, y11 }, { x12, y11 }, { x12,(y11+y12)/2 }, { x12, y12 }, { x11, y12 }
                                        , { x21, y11 }, { x22, y11 }, { x22, y12 }, { x21, y12},{ x21,(y11+y12)/2 }
                                        , { x21, y21 }, { x22, y21 }, { x22, y22 }, { x21,y22 },{ x21,(y21+y22)/2 }
                                        , { x11, y21 }, { x12, y21 },{ x12,(y21+y22)/2 }, { x12, y22 }, { x11,y22 } };


        float[,] ori_24 = new float[,] { { x11, y11 }, { x12, y11 },{ x12,(y11+y12)/2 }, { x12, y12 }, { x11, y12 } , {x11,(y11+y12)/2 }
                                        , { x21, y11 }, { x22, y11 },{ x22,(y11+y12)/2 }, { x22, y12 }, { x21, y12},{ x21,(y11+y12)/2 }
                                        , { x21, y21 }, { x22, y21 },{ x22,(y21+y22)/2 }, { x22, y22 }, { x21,y22 },{ x21,(y21+y22)/2 }
                                        , { x11, y21 }, { x12, y21 },{ x12,(y21+y22)/2 }, { x12, y22 }, { x11,y22 },{ x11,(y21+y22)/2 } };


        float[] offset_16 = new float[16];
        float[] offset_20 = new float[20];
        float[] offset_24 = new float[24];



        String[] mp_x_temp = xx.Split(',');
        String[] mp_y_temp = yy.Split(',');

        float[] mp_x = Array.ConvertAll(mp_x_temp, x => float.Parse(x));
        float[] mp_y = Array.ConvertAll(mp_y_temp, x => float.Parse(x));

        float[,] ori;
        float[] offset;
        Font drawFont = new Font("Arial", 12, FontStyle.Bold);
        Font str_map = new Font("Arial", 11, FontStyle.Bold);
        SolidBrush drawBrush = new SolidBrush(Color.Black);

        Font label_font = new Font("Arial", 9, FontStyle.Bold);



        if (mp_x.Length == 16)
        {
            for(int i = 0; i <label1.GetLength(0);i++)
            {
                Gpi.DrawString("X", label_font, Brushes.Black, label1[i, 0] -15, label1[i, 1]+15);
                Gpi.DrawString("Y", label_font, Brushes.Black, label1[i, 0] -15, label1[i, 1]+30);
            }

     
            for (int i = 0; i < delta_16.GetLength(0); i++)
            {
                int x_position_offset = 0;
                
                float diff = mp_x[delta_16[i, 0]-1] - mp_x[delta_16[i, 1]-1];
                if (diff > 0)
                    x_position_offset = 5;

                SizeF sizeF = Gpi.MeasureString(" " + diff.ToString("##0.00"), str_map);
                Gpi.FillRectangle(Brushes.Pink, new RectangleF(new PointF(delta_4_position[i, 0], delta_4_position[i, 1]), sizeF));
                Gpi.DrawString((diff).ToString("##0.00"), str_map, Brushes.Black, delta_4_position[i, 0]+ x_position_offset, delta_4_position[i, 1]);

            }


            ori = ori_16;
            for (int i = 0; i < mp_x.Length; i++)
            {
                int x_position_offset = 0;
                int y_position_offset = 0;
                if (mp_x[i] > 0)
                    x_position_offset = 5;
                if (mp_y[i] > 0)
                    y_position_offset = 5;

                SizeF sizeF = Gpi.MeasureString(" "+mp_x[i].ToString("##0.00"), str_map);
                Gpi.FillRectangle(Brushes.Yellow, new RectangleF(new PointF(ori[i, 0] , ori[i, 1] + 15), sizeF));
                Gpi.FillRectangle(Brushes.PowderBlue, new RectangleF(new PointF(ori[i, 0] , ori[i, 1] + 30), sizeF));

                

                Gpi.DrawString(num_16[i], str_map, Brushes.Black, ori[i, 0] + x_position_offset + 10, ori[i, 1]);
                Gpi.DrawString(mp_x[i].ToString("##0.00"), str_map, Brushes.Black, ori[i, 0] + x_position_offset, ori[i, 1]+15);
                Gpi.DrawString(mp_y[i].ToString("##0.00"), str_map, Brushes.Black, ori[i, 0]  + y_position_offset, ori[i, 1] + 30);
            }
        }
        else if (mp_x.Length == 20)
        {
            ori = ori_20;
            for (int i = 0; i < label2.GetLength(0); i++)
            {
                Gpi.DrawString("X", label_font, Brushes.Black, label2[i, 0] - 15, label2[i, 1] + 15);
                Gpi.DrawString("Y", label_font, Brushes.Black, label2[i, 0] - 15, label2[i, 1] + 30);
            }


            for (int i = 0; i < delta_20.GetLength(0); i++)
            {
                int x_position_offset = 0;
                
                float diff = mp_x[delta_20[i, 0] - 1] - mp_x[delta_20[i, 1] - 1];
                if (diff > 0)
                    x_position_offset = 5;

                SizeF sizeF = Gpi.MeasureString(" " + diff.ToString("##0.00"), str_map);
                Gpi.FillRectangle(Brushes.Pink, new RectangleF(new PointF(delta_6_position[i, 0], delta_6_position[i, 1]), sizeF));
                Gpi.DrawString((diff).ToString("##0.00"), str_map, Brushes.Black, delta_6_position[i, 0] + x_position_offset, delta_6_position[i, 1]);

            }


            for (int i = 0; i < mp_x.Length; i++)
            {
                int x_position_offset = 0;
                int y_position_offset = 0;
                if (mp_x[i] > 0)
                    x_position_offset = 5;
                if (mp_y[i] > 0)
                    y_position_offset = 5;

                SizeF sizeF = Gpi.MeasureString(" " + mp_x[i].ToString("##0.00"), str_map);
                Gpi.FillRectangle(Brushes.Yellow, new RectangleF(new PointF(ori[i, 0] , ori[i, 1] + 15), sizeF));
                Gpi.FillRectangle(Brushes.PowderBlue, new RectangleF(new PointF(ori[i, 0] , ori[i, 1] + 30), sizeF));

                

                Gpi.DrawString(num_20[i], str_map, Brushes.Black, ori[i, 0] + x_position_offset + 10, ori[i, 1]);
                Gpi.DrawString( mp_x[i].ToString("##0.00"), str_map, Brushes.Black, ori[i, 0] + x_position_offset, ori[i, 1] + 15);
                Gpi.DrawString( mp_y[i].ToString("##0.00"), str_map, Brushes.Black, ori[i, 0] + y_position_offset, ori[i, 1] + 30);
            }


        }
        else
        {
            ori = ori_24;
            for (int i = 0; i < label2.GetLength(0); i++)
            {
                Gpi.DrawString("X", label_font, Brushes.Black, label2[i, 0] - 15, label2[i, 1] + 15);
                Gpi.DrawString("Y", label_font, Brushes.Black, label2[i, 0] - 15, label2[i, 1] + 30);
            }

            for (int i = 0; i < delta_24.GetLength(0); i++)
            {
                int x_position_offset = 0;

                float diff = mp_x[delta_24[i, 0] - 1] - mp_x[delta_24[i, 1] - 1];
                if (diff > 0)
                    x_position_offset = 5;

                SizeF sizeF = Gpi.MeasureString(" " + diff.ToString("##0.00"), str_map);
                Gpi.FillRectangle(Brushes.Pink, new RectangleF(new PointF(delta_6_position[i, 0], delta_6_position[i, 1]), sizeF));
                Gpi.DrawString((diff).ToString("##0.00"), str_map, Brushes.Black, delta_6_position[i, 0] + x_position_offset, delta_6_position[i, 1]);

            }

            for (int i = 0; i < mp_x.Length; i++)
            {
                int x_position_offset = 0;
                int y_position_offset = 0;
                if (mp_x[i] > 0)
                    x_position_offset = 5;
                if (mp_y[i] > 0)
                    y_position_offset = 5;

                SizeF sizeF = Gpi.MeasureString(" " + mp_x[i].ToString("##0.00"), str_map);
                Gpi.FillRectangle(Brushes.Yellow, new RectangleF(new PointF(ori[i, 0] , ori[i, 1] + 15), sizeF));
                Gpi.FillRectangle(Brushes.PowderBlue, new RectangleF(new PointF(ori[i, 0] , ori[i, 1] + 30), sizeF));

                Gpi.DrawString(num_24[i], str_map, Brushes.Black, ori[i, 0] + x_position_offset + 10, ori[i, 1]);
                Gpi.DrawString(mp_x[i].ToString("##0.00"), str_map, Brushes.Black, ori[i, 0] + x_position_offset, ori[i, 1] + 15);
                Gpi.DrawString(mp_y[i].ToString("##0.00"), str_map, Brushes.Black, ori[i, 0] + y_position_offset, ori[i, 1] + 30);
            }

        }




        //float ori_x[16] = {x12,x11,x11,x12,x21,x22 };


        //graph_paint(Gpi, ori, offset, offset, 1,1,true);
        //graph_paint(Gpi, ori, mp_x, mp_y);




        //標slot// Create point for upper-left corner of drawing.

        for (int i = 0; i < shot_position.GetLength(0); i++)
        {
            Gpi.FillRectangle(Brushes.Black, new RectangleF(new PointF(shot_position[i, 0], shot_position[i, 1]), Gpi.MeasureString("S1", drawFont)));
            Gpi.DrawString("S" + (i+1).ToString(), drawFont, Brushes.White, shot_position[i, 0], shot_position[i, 1]);
        }
        
        //Gpi.DrawString(mp_x.Length.ToString(), drawFont, Brushes.Black, (x11 + x21) / 2 - 10, y22 + 10);


        Bmp.Save(stream, ImageFormat.Jpeg);
        byte[] byteArray = stream.GetBuffer(); //將Bitmap轉為Byte[]
        stream.Dispose();
        Gpi.Dispose();
        return Convert.ToBase64String(byteArray); //轉為Base64sting
    }

   
    public string TransIntToImage(String xx,String yy,int x_offset,int y_offset)
    {


        float x_zero = wid / 2;
        float y_zero = hei / 2;

        Font ft = new Font("Arial", 10);

        Bitmap Bmp = new Bitmap(wid + 2, hei + 2);  //建立實體圖檔並設定大小

        //SolidBrush brush = new SolidBrush(pen.Color);

        Graphics Gpi = Graphics.FromImage(Bmp);

        Gpi.Clear(Color.White);

        MemoryStream stream = new MemoryStream();

        Gpi.Clear(Color.White);

        Pen pen = new Pen(Color.Silver, 1);

        //畫座標
        Gpi.DrawLine(pen, 0, 0, 0, hei);
        Gpi.DrawLine(pen, 0, hei, wid, hei);
        Gpi.DrawLine(pen, wid, hei, wid, 0);
        Gpi.DrawLine(pen, wid, 0, 0, 0);
        for (int i = 0; i < hei; i += ratio)
            Gpi.DrawLine(pen, 0, i, wid, i);
        for (int i = 0; i < wid; i += ratio)
            Gpi.DrawLine(pen, i, 0, i, hei);

        //X軸Y軸
        pen = new Pen(Color.Green, 2);
        Gpi.DrawLine(pen, 0, y_zero, wid, y_zero);
        Gpi.DrawLine(pen, x_zero, 0, x_zero, hei);


        float x11 = x_zero - (x_start + d) * ratio;
        float x12 = x_zero - x_start * ratio;
        float x21 = x_zero + x_start * ratio;
        float x22 = x_zero + (x_start + d) * ratio;
        float y11 = y_zero - (y_start + d) * ratio;
        float y12 = y_zero - y_start * ratio;
        float y21 = y_zero + y_start * ratio;
        float y22 = y_zero + (y_start + d) * ratio;

        float[,] ori_16 = new float[,] { { x11, y11 }, { x12, y11 }, { x12, y12 }, { x11, y12 } 
                                        , { x21, y11 }, { x22, y11 }, { x22, y12 }, { x21, y12}
                                        , { x21, y21 }, { x22, y21 }, { x22, y22 }, { x21,y22 }
                                        , { x11, y21 }, { x12, y21 }, { x12, y22 }, { x11,y22 } };

        float[,] ori_20 = new float[,] { { x11, y11 }, { x12, y11 }, { x12,(y11+y12)/2 }, { x12, y12 }, { x11, y12 }
                                        , { x21, y11 }, { x22, y11 }, { x22, y12 }, { x21, y12},{ x21,(y11+y12)/2 }
                                        , { x21, y21 }, { x22, y21 }, { x22, y22 }, { x21,y22 },{ x21,(y21+y22)/2 }
                                        , { x11, y21 }, { x12, y21 },{ x12,(y21+y22)/2 }, { x12, y22 }, { x11,y22 } };

        float[,] ori_24 = new float[,] { { x11, y11 }, { x12, y11 },{ x12,(y11+y12)/2 }, { x12, y12 }, { x11, y12 } , {x11,(y11+y12)/2 }
                                        , { x21, y11 }, { x22, y11 },{ x22,(y11+y12)/2 }, { x22, y12 }, { x21, y12},{ x21,(y11+y12)/2 }
                                        , { x21, y21 }, { x22, y21 },{ x22,(y21+y22)/2 }, { x22, y22 }, { x21,y22 },{ x21,(y21+y22)/2 }
                                        , { x11, y21 }, { x12, y21 },{ x12,(y21+y22)/2 }, { x12, y22 }, { x11,y22 },{ x11,(y21+y22)/2 } };

        
        float[] offset_16 = new float[16];
        float[] offset_20 = new float[20];
        float[] offset_24 = new float[24];
        


        String[] mp_x_temp = xx.Split(',');
        String[] mp_y_temp = yy.Split(',');

        float[] mp_x = Array.ConvertAll(mp_x_temp, x => float.Parse(x));
        float[] mp_y = Array.ConvertAll(mp_y_temp, x => float.Parse(x));

        float[,] ori;
        float[] offset;

        if (mp_x.Length == 16)
        {
            ori = ori_16;
            offset = offset_16;
        }
        else if (mp_x.Length == 20)
        {
            ori = ori_20;
            offset = offset_20;
        }
        else
        {
            ori = ori_24;
            offset = offset_24;
        }


        //float ori_x[16] = {x12,x11,x11,x12,x21,x22 };


        graph_paint(Gpi, ori, offset, offset,1,1, true);
        graph_paint(Gpi, ori, mp_x, mp_y,x_offset,y_offset);




        //標slot
        Font drawFont = new Font("Arial", 12, FontStyle.Bold);
        SolidBrush drawBrush = new SolidBrush(Color.Black);// Create point for upper-left corner of drawing.



        Gpi.DrawString("S1", drawFont, Brushes.Black, (x11 + x12) / 2 - 10, (y11 + y12) / 2 - 10);
        Gpi.DrawString("S2", drawFont, Brushes.Black, (x21 + x22) / 2 - 10, (y11 + y12) / 2 - 10);
        Gpi.DrawString("S3", drawFont, Brushes.Black, (x21 + x22) / 2 - 10, (y21 + y22) / 2 - 10);
        Gpi.DrawString("S4", drawFont, Brushes.Black, (x11 + x12) / 2 - 10, (y21 + y22) / 2 - 10);
        //Gpi.DrawString(mp_x.Length.ToString(), drawFont, Brushes.Black, (x11 + x21) / 2 - 10, y22 + 10);


        Bmp.Save(stream, ImageFormat.Jpeg);
        byte[] byteArray = stream.GetBuffer(); //將Bitmap轉為Byte[]
        stream.Dispose();
        return Convert.ToBase64String(byteArray); //轉為Base64sting
    }

    public string TransIntToImage_ol(String[] xx, String[] yy,String[] sheet_id,String[] tool_id,String[] x_offset,String[] y_offset)
    {


        float x_zero = wid / 2;
        float y_zero = hei / 2;

        Font ft = new Font("Arial", 10);

        Bitmap Bmp = new Bitmap(wid + 200, hei + 2);  //建立實體圖檔並設定大小

        //SolidBrush brush = new SolidBrush(pen.Color);

        Graphics Gpi = Graphics.FromImage(Bmp);

        Gpi.Clear(Color.White);

        MemoryStream stream = new MemoryStream();

        Gpi.Clear(Color.White);

        Pen pen = new Pen(Color.Silver, 1);

        //畫座標
        Gpi.DrawLine(pen, 0, 0, 0, hei);
        Gpi.DrawLine(pen, 0, hei, wid, hei);
        Gpi.DrawLine(pen, wid, hei, wid, 0);
        Gpi.DrawLine(pen, wid, 0, 0, 0);
        for (int i = 0; i < hei; i += ratio)
            Gpi.DrawLine(pen, 0, i, wid, i);
        for (int i = 0; i < wid; i += ratio)
            Gpi.DrawLine(pen, i, 0, i, hei);

        //X軸Y軸
        pen = new Pen(Color.Green, 2);
        Gpi.DrawLine(pen, 0, y_zero, wid, y_zero);
        Gpi.DrawLine(pen, x_zero, 0, x_zero, hei);


        float x11 = x_zero - (x_start + d) * ratio;
        float x12 = x_zero - x_start * ratio;
        float x21 = x_zero + x_start * ratio;
        float x22 = x_zero + (x_start + d) * ratio;
        float y11 = y_zero - (y_start + d) * ratio;
        float y12 = y_zero - y_start * ratio;
        float y21 = y_zero + y_start * ratio;
        float y22 = y_zero + (y_start + d) * ratio;

        float[,] ori_16 = new float[,] { { x11, y11 }, { x12, y11 }, { x12, y12 }, { x11, y12 }
                                        , { x21, y11 }, { x22, y11 }, { x22, y12 }, { x21, y12}
                                        , { x21, y21 }, { x22, y21 }, { x22, y22 }, { x21,y22 }
                                        , { x11, y21 }, { x12, y21 }, { x12, y22 }, { x11,y22 } };

        float[,] ori_20 = new float[,] { { x11, y11 }, { x12, y11 }, { x12,(y11+y12)/2 }, { x12, y12 }, { x11, y12 }
                                        , { x21, y11 }, { x22, y11 }, { x22, y12 }, { x21, y12},{ x21,(y11+y12)/2 }
                                        , { x21, y21 }, { x22, y21 }, { x22, y22 }, { x21,y22 },{ x21,(y21+y22)/2 }
                                        , { x11, y21 }, { x12, y21 },{ x12,(y21+y22)/2 }, { x12, y22 }, { x11,y22 } };

        float[,] ori_24 = new float[,] { { x11, y11 }, { x12, y11 },{ x12,(y11+y12)/2 }, { x12, y12 }, { x11, y12 } , {x11,(y11+y12)/2 }
                                        , { x21, y11 }, { x22, y11 },{ x22,(y11+y12)/2 }, { x22, y12 }, { x21, y12},{ x21,(y11+y12)/2 }
                                        , { x21, y21 }, { x22, y21 },{ x22,(y21+y22)/2 }, { x22, y22 }, { x21,y22 },{ x21,(y21+y22)/2 }
                                        , { x11, y21 }, { x12, y21 },{ x12,(y21+y22)/2 }, { x12, y22 }, { x11,y22 },{ x11,(y21+y22)/2 } };


        float[] offset_16 = new float[16];
        float[] offset_20 = new float[20];
        float[] offset_24 = new float[24];
        graph_paint(Gpi, ori_16, offset_16, offset_16,1,1, true);

        int count = 0;


        for (int i = 0; i < xx.Length; i++)
        {
            if (xx.Length == yy.Length)
            {
                String[] mp_x_temp = xx[i].Split(',');
                String[] mp_y_temp = yy[i].Split(',');

                float[] mp_x = Array.ConvertAll(mp_x_temp, x => float.Parse(x));
                float[] mp_y = Array.ConvertAll(mp_y_temp, x => float.Parse(x));

                float[,] ori;
                float[] offset;

                if (mp_x.Length == 16)
                {
                    ori = ori_16;
                    offset = offset_16;
                }
                else if (mp_x.Length == 20)
                {
                    ori = ori_20;
                    offset = offset_20;
                }
                else
                {
                    ori = ori_24;
                    offset = offset_24;
                }
                //float ori_x[16] = {x12,x11,x11,x12,x21,x22 };
                graph_paint(Gpi, ori, mp_x, mp_y , int.Parse(x_offset[i]), int.Parse(y_offset[i]), false, true);
                pen = new Pen(color[count+1], 2);
                Font f_str = new Font("Arial", 11, FontStyle.Bold);
                Gpi.DrawLine(pen, 420, 10+count*43, 550, 10 + count * 43);
                Gpi.DrawString(sheet_id[count], f_str, Brushes.Black , 430, 12 + count * 43);
                Gpi.DrawString("("+tool_id[count]+")", f_str, Brushes.Black, 430, 25 + count * 43);

                count++;
                if (count >= 9)
                    break;
            }
        }

        //標slot
        Font drawFont = new Font("Arial", 12, FontStyle.Bold);
        SolidBrush drawBrush = new SolidBrush(Color.Black);// Create point for upper-left corner of drawing.



        Gpi.DrawString("S1", drawFont, Brushes.Black, (x11 + x12) / 2 - 10, (y11 + y12) / 2 - 10);
        Gpi.DrawString("S2", drawFont, Brushes.Black, (x21 + x22) / 2 - 10, (y11 + y12) / 2 - 10);
        Gpi.DrawString("S3", drawFont, Brushes.Black, (x21 + x22) / 2 - 10, (y21 + y22) / 2 - 10);
        Gpi.DrawString("S4", drawFont, Brushes.Black, (x11 + x12) / 2 - 10, (y21 + y22) / 2 - 10);
        //Gpi.DrawString(mp_x.Length.ToString(), drawFont, Brushes.Black, (x11 + x21) / 2 - 10, y22 + 10);


        Bmp.Save(stream, ImageFormat.Jpeg);
        byte[] byteArray = stream.GetBuffer(); //將Bitmap轉為Byte[]
        stream.Dispose();
        return Convert.ToBase64String(byteArray); //轉為Base64sting
    }

    protected void GridView1_RowCreated(Object sender, GridViewRowEventArgs e)
    {
        DataRowView drv = (DataRowView)e.Row.DataItem;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            String unit = drv["unit"].ToString();
            //String ng = drv["oos"].ToString();
            try {
                if (unit.Substring(unit.Length - 1) == "1")
                {
                    ((Label)(e.Row.FindControl("g_label15"))).Text = "";
                    ((Label)(e.Row.FindControl("g_label16"))).Text = "";
                    ((Label)(e.Row.FindControl("g_label17"))).Text = "";
                }
                else if (unit.Substring(unit.Length - 1) == "2")
                {
                    ((Label)(e.Row.FindControl("g_label12"))).Text = "";
                    ((Label)(e.Row.FindControl("g_label13"))).Text = "";
                    ((Label)(e.Row.FindControl("g_label14"))).Text = "";
                }
                /*if (ng != "")
                {
                    //e.Row.ForeColor = Color.Red;
                    ((Label)(e.Row.FindControl("g_oos"))).ForeColor = Color.Red;
                }*/
            }
            catch(Exception ex)
            {

            }

        }

    }
    protected void Button1_Click(object sender, EventArgs e)
    {

        try {
            if (Button1.Text == "查看圖表")
            {
                lb_error.Text = "";
                Button1.Text = "返回";
                set_img();
                GridView1.Visible = false;
                CB_ol.Visible = false;
                CB_avg.Visible = false;
                Label6.Visible = false;
                Button3.Visible = false;

            }
            else
            {
                lb_error.Text = "";
                Button1.Text = "查看圖表";
                GridView1.Visible = true;
                panel_imgall.Visible = false;
                panel_ol.Visible = false;
                panel_avg.Visible = false;
                img_init();
                CB_ol.Visible = true;
                CB_avg.Visible = true;
                Label6.Visible = true;
                Button3.Visible = true;

            }
        }
        catch(Exception ex)
        {
            lb_error.Text = ex.Message;
        }
    }
    public void img_init()
    {
        Image1.ImageUrl = "";
        Image2.ImageUrl = "";
        Image3.ImageUrl = "";
        Image4.ImageUrl = "";
        Image5.ImageUrl = "";
        Image6.ImageUrl = "";
        Image7.ImageUrl = "";
        Image8.ImageUrl = "";
        Image9.ImageUrl = "";
        Image10.ImageUrl = "";
        Image11.ImageUrl = "";
        Image12.ImageUrl = "";
        Image_ol.ImageUrl = "";
        Image_avg.ImageUrl = "";
        Image_fill_1.ImageUrl = "";
        Image_fill_2.ImageUrl = "";

        Image1.Dispose();
        Image2.Dispose();
        Image3.Dispose();
        Image4.Dispose();
        Image5.Dispose();
        Image6.Dispose();
        Image7.Dispose();
        Image8.Dispose();
        Image9.Dispose();
        Image10.Dispose();
        Image11.Dispose();
        Image12.Dispose();
        Image_ol.Dispose();
        Image_avg.Dispose();
        Image_fill_1.Dispose();
        Image_fill_2.Dispose();


        lb_img1.Text = "";
        lb_img2.Text = "";
        lb_img3.Text = "";
        lb_img4.Text = "";
        lb_img5.Text = "";
        lb_img6.Text = "";
        lb_img7.Text = "";
        lb_img8.Text = "";
        lb_img9.Text = "";
        lb_img10.Text = "";
        lb_img11.Text = "";
        lb_img12.Text = "";
        lb_img_fill_1.Text = "";
        lb_img_fill_2.Text = "";


        lb_img1_1.Text = "";
        lb_img2_1.Text = "";
        lb_img3_1.Text = "";
        lb_img4_1.Text = "";
        lb_img5_1.Text = "";
        lb_img6_1.Text = "";
        lb_img7_1.Text = "";
        lb_img8_1.Text = "";
        lb_img9_1.Text = "";
        lb_img10_1.Text = "";
        lb_img11_1.Text = "";
        lb_img12_1.Text = "";
        lb_img_fill_1_1.Text = "";
        lb_img_fill_2_1.Text = "";

        Image1_value.ImageUrl = "";
        Image2_value.ImageUrl = "";
        Image3_value.ImageUrl = "";
        Image4_value.ImageUrl = "";
        Image5_value.ImageUrl = "";
        Image6_value.ImageUrl = "";
        Image7_value.ImageUrl = "";
        Image8_value.ImageUrl = "";
        Image9_value.ImageUrl = "";
        Image10_value.ImageUrl = "";
        Image11_value.ImageUrl = "";
        Image12_value.ImageUrl = "";
        Image_value_avg.ImageUrl = "";
        Image_fill_value_1.ImageUrl = "";
        Image_fill_value_2.ImageUrl = "";


        Image1_value.Dispose();
        Image2_value.Dispose();
        Image3_value.Dispose();
        Image4_value.Dispose();
        Image5_value.Dispose();
        Image6_value.Dispose();
        Image7_value.Dispose();
        Image8_value.Dispose();
        Image9_value.Dispose();
        Image10_value.Dispose();
        Image11_value.Dispose();
        Image12_value.Dispose();
        Image_value_avg.Dispose();
        Image_fill_value_1.Dispose();
        Image_fill_value_2.Dispose();


        WebChartViewer_avg.Dispose();
        WebChartViewer1.Dispose();
        WebChartViewer2.Dispose();
        WebChartViewer3.Dispose();
        WebChartViewer4.Dispose();
        WebChartViewer5.Dispose();
        WebChartViewer6.Dispose();
        WebChartViewer7.Dispose();
        WebChartViewer8.Dispose();
        WebChartViewer9.Dispose();
        WebChartViewer10.Dispose();
        WebChartViewer11.Dispose();
        WebChartViewer12.Dispose();
        WebChartViewer_fill1.Dispose();
        WebChartViewer_fill2.Dispose();

    }

    public void init_fillimg()
    {
        lb_fill_x1.Text = "";
        lb_fill_y1.Text = "";
        lb_fill_x2.Text = "";
        lb_fill_y2.Text = "";
        lb_fill_offset_x.Text = "";
        lb_fill_offset_y.Text = "";
        lb_fill_stitch.Text = "";
        lb_fill_pointcount.Text = "";
        lb_fill_stitch_point.Text = "";
        String stitch_16 = "2,3,5,8,9,12,14,15";
        String stitch_20 = "2,3,4,6,9,10,11,14,15,17,18,19";
        String stitch_24 = "2,3,4,7,11,12,13,17,18,20,21,22";
        


        String x = "";
        String y = "";
        String x_offset = "";
        String y_offset = "";
        String stich = "";
        String sheet_id = "";
        String tool_id = "";
        String eqp_time = "";
        String aoi_time = "";
        String oos = "";
        String ooc = "";
        String point_count = "";
        for (int i = 0; i < GridView1.Rows.Count; i++)
        {
            CheckBox cb = (CheckBox)GridView1.Rows[i].FindControl("cb_record");

            if (cb.Checked)
            {
                x = ((Label)GridView1.Rows[i].FindControl("g_X")).Text;
                y = ((Label)GridView1.Rows[i].FindControl("g_Y")).Text;
                x_offset = ((Label)GridView1.Rows[i].FindControl("g_x_offset")).Text;
                y_offset = ((Label)GridView1.Rows[i].FindControl("g_y_offset")).Text;
                stich = ((Label)GridView1.Rows[i].FindControl("g_stiching_point")).Text;
                

                sheet_id = ((Label)GridView1.Rows[i].FindControl("g_label7")).Text;
                tool_id = ((Label)GridView1.Rows[i].FindControl("g_label4")).Text;

                eqp_time = ((Label)GridView1.Rows[i].FindControl("g_label5")).Text;
                aoi_time = ((Label)GridView1.Rows[i].FindControl("g_label1")).Text;
                oos = ((Label)GridView1.Rows[i].FindControl("g_oos")).Text;
                ooc = ((Label)GridView1.Rows[i].FindControl("g_ooc")).Text;
                point_count = ((Label)GridView1.Rows[i].FindControl("g_point_count")).Text;

                if (point_count == "16")
                    lb_fill_stitch_point.Text = stitch_16;
                else if (point_count == "20")
                {
                    //y_offset = "1";
                    lb_fill_stitch_point.Text = stitch_20;
                }
                else
                {
                    lb_fill_stitch_point.Text = stitch_24;
                }

                /*
                x = "0.56,-0.52,-0.28,-0.22,0.30,1.54,-0.89,-0.54,-0.35,-0.47,-0.22,0.59,-0.83,-0.57,0.79,1.32,-0.57,0.95,0.69,-0.13,0.75,0.69,-0.03,1.70";
                y = "-0.78,0.60,0.32,0.35,-0.97,-0.31,-0.44,0.46,0.38,-0.16,-0.84,-0.17,-1.27,0.21,0.26,0.85,-0.38,-0.46,-0.71,-0.05,0.03,0.57,-1.74,-0.74";

                x_offset = "-1";
                y_offset = "1";
                point_count = "24";
                */

                lb_fill_x1.Text = x;
                lb_fill_y1.Text = y;
                lb_fill_offset_x.Text = x_offset;
                lb_fill_offset_y.Text = y_offset;
                lb_fill_stitch.Text = stich;
                lb_fill_pointcount.Text = point_count;
                break;
            }
        }
        
        if (x != "" && y != "")
        {
            Image_fill_1.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(x, y, int.Parse(x_offset), int.Parse(y_offset));
            Image_fill_value_1.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(x, y);
            lb_img_fill_1.Text = sheet_id + " (" + tool_id + ")";
            lb_img_fill_1_1.Text = "</br>&nbsp;&nbsp;&nbsp;&nbsp;Meas Time : " + aoi_time;
            lb_img_fill_1_1.Text += "</br>Process Time : " + eqp_time;
            boxplot(stich, 4, WebChartViewer_fill1);
            if (oos != "")
                lb_img_fill_1.ForeColor = Color.Red;
            else if (ooc != "")
                lb_img_fill_1.ForeColor = Color.FromArgb(0xCC6600);
            else
                lb_img_fill_1.ForeColor = Color.Black;
        }
    }
    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
        try
        {
            getdata();

        }
        catch (Exception ex)
        {
            lb_error.Text = ex.Message;
        }
    }


    

    protected void TOP_BT1_Click(object sender, EventArgs e)
    {
        Response.Redirect("OL_main.aspx", true);
    }
    protected void TOP_BT2_Click(object sender, EventArgs e)
    {
        Response.Redirect("Tsuno_main.aspx", true);
    }

    protected void TOP_BT3_Click(object sender, EventArgs e)
    {
        Response.Redirect("CD_main.aspx", true);
    }
    protected void TOP_BT4_Click(object sender, EventArgs e)
    {
        Response.Redirect("History.aspx", true);
    }
    protected void TOP_BT5_Click(object sender, EventArgs e)
    {
        Response.Redirect("CF_TTP_main.aspx", true);
    }
    public void init_BT()
    {
        TOP_BT1.BackColor = Color.Gainsboro;
        TOP_BT1.Font.Bold = false;
        TOP_BT1.ForeColor = Color.Black;

        TOP_BT2.BackColor = Color.Gainsboro;
        TOP_BT2.Font.Bold = false;
        TOP_BT2.ForeColor = Color.Black;

        TOP_BT3.BackColor = Color.Gainsboro;
        TOP_BT3.Font.Bold = false;
        TOP_BT3.ForeColor = Color.Black;

        TOP_BT4.BackColor = Color.Gainsboro;
        TOP_BT4.Font.Bold = false;
        TOP_BT4.ForeColor = Color.Black;

        TOP_BT5.BackColor = Color.Gainsboro;
        TOP_BT5.Font.Bold = false;
        TOP_BT5.ForeColor = Color.Black;

        TOP_BT1.BackColor = Color.SteelBlue;
        TOP_BT1.Font.Bold = true;
        TOP_BT1.ForeColor = Color.White;


    }

    protected void cb_checkall_CheckedChanged(object sender, EventArgs e)
    {
        Boolean bl = ((CheckBox)GridView1.HeaderRow.FindControl("cb_checkall")).Checked;
        for (int i = 0; i < GridView1.Rows.Count; i++)
        {
            CheckBox cb = (CheckBox)GridView1.Rows[i].FindControl("cb_record");
            cb.Checked = bl;
        }
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        if (Button3.Text == "單片補值")
        {
            panel_fillvalue.Visible = true;
            init_fillimg();
            Button3.Text = "取消";
            GridView1.Visible = false;
            CB_ol.Visible = false;
            CB_avg.Visible = false;
            Label6.Visible = false;
            Button1.Visible = false;
            //lb_error.Text = "補值功能目前測試中，如有錯誤還請告知";
            tb_fill_x_1.Text = "0";
            tb_fill_y_1.Text = "0";
            tb_fill_c_1.Text = "0";
            tb_fill_x_2.Text = "0";
            tb_fill_y_2.Text = "0";
            tb_fill_c_2.Text = "0";
            tb_fill_x_3.Text = "0";
            tb_fill_y_3.Text = "0";
            tb_fill_c_3.Text = "0";
            tb_fill_x_4.Text = "0";
            tb_fill_y_4.Text = "0";
            tb_fill_c_4.Text = "0";
            tb_fill_t.Text = "0";
        }
        else
        {
            panel_fillvalue.Visible = false;
            Button3.Text = "單片補值";
            GridView1.Visible = true;
            CB_ol.Visible = true;
            CB_avg.Visible = true;
            Label6.Visible = true;
            Button1.Visible = true;
            lb_error.Text = "";
            img_init();
        }
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        try { set_fillimg(); }
        catch(Exception ee)
        { lb_error.Text = ee.Message; }

    }
    public void set_fillvalue()
    {
        String[] x = lb_fill_x1.Text.Split(',');
        String[] y = lb_fill_y1.Text.Split(',');
        
        int n = int.Parse(lb_fill_pointcount.Text);
        double[] fill_x = { double.Parse(tb_fill_x_1.Text), double.Parse(tb_fill_x_2.Text), double.Parse(tb_fill_x_3.Text), double.Parse(tb_fill_x_4.Text) };
        double[] fill_y = { double.Parse(tb_fill_y_1.Text), double.Parse(tb_fill_y_2.Text), double.Parse(tb_fill_y_3.Text), double.Parse(tb_fill_y_4.Text) };
        double[] fill_c = { double.Parse(tb_fill_c_1.Text), double.Parse(tb_fill_c_2.Text), double.Parse(tb_fill_c_3.Text), double.Parse(tb_fill_c_4.Text) };
        double temp = double.Parse(tb_fill_t.Text);
        int offset_y = 1;
        if (n == 20) 
            offset_y = -1;

        //theta角度的偏重(正負)
        int[] type_c_x_16 = { -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1 };
        int[] type_c_x_20 = { -1, -1, 0, 1, 1, -1, -1, 1, 1, 0, -1, -1, 1, 1, 0, -1, -1, 0, 1, 1 };
        int[] type_c_x_24 = { -1, -1,  0, 1, 1, 0,  -1,  -1,  0, 1, 1, 0,  -1,  -1,  0, 1, 1, 0,  -1,  -1,  0, 1, 1, 0 };

        int[] type_c_y_16 = { 1, -1, -1,  1, 1, -1, -1,  1,  1, -1, -1,  1,  1, -1, -1,  1 };
        int[] type_c_y_20 = { 1, -1, -1, -1, 1, 1, -1, -1, 1, 1, 1, -1, -1, 1, 1, 1, -1, -1, -1, 1 };
        int[] type_c_y_24 = { 1, -1, -1, -1, 1,  1,  1, -1, -1, -1,  1,  1,  1, -1, -1, -1,  1,  1,  1, -1, -1, -1,  1,  1 };

        //溫度的偏重(正負)
        int[] type_t_x_16 = { 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1 };
        int[] type_t_x_20 = { 1, -1, -1, -1, 1, 1, -1, -1, 1, 1, 1, -1, -1, 1, 1, 1, -1, -1, -1, 1 };
        int[] type_t_x_24 = { 1, -1, -1, -1, 1, 1, 1, -1, -1, -1, 1, 1, 1, -1, -1, -1, 1, 1, 1, -1, -1, -1, 1, 1 };

        int[] type_t_y_16 = { 1, 1, -1, -1,  1, 1, -1, -1,  1,  1, -1, -1,  1,  1, -1, -1 };
        int[] type_t_y_20 = { 1, 1, 1, -1, -1, 1, 1, -1, -1, -1, 1, 1, -1, -1, -1, 1, 1, 1, -1, -1 };
        int[] type_t_y_24 = { 1, 1,  0, -1, -1, 0,  1,  1,  0, -1, -1,  0,  1,  1,  0, -1, -1, 0,  1,  1,  0, -1, -1,  0 };

        int[] type_c_x;
        int[] type_c_y;
        int[] type_t_x;
        int[] type_t_y;
        if (n == 16)
        {
            type_c_x = type_c_x_16;
            type_c_y = type_c_y_16;
            type_t_x = type_t_x_16;
            type_t_y = type_t_y_16;
        }
        else if (n == 20)
        {
            type_c_x = type_c_x_20;
            type_c_y = type_c_y_20;
            type_t_x = type_t_x_20;
            type_t_y = type_t_y_20;
        }
        else
        {
            type_c_x = type_c_x_24;
            type_c_y = type_c_y_24;
            type_t_x = type_t_x_24;
            type_t_y = type_t_y_24;
        }



        double[] x2 = new double[n];
        double[] y2 = new double[n];

        for (int i = 0; i < 4; i++)
        {            
            for (int j = 0; j < n / 4; j++)
            {
                int index = n / 4 * i + j;
                
                x2[index] =  double.Parse(x[index]) - fill_x[i] + type_t_x[index] * temp + type_c_x[index] * fill_c[i] / 1.32;
                y2[index] = offset_y * (offset_y * double.Parse(y[index]) + fill_y[i] + type_t_y[index] * temp + type_c_y[index] * fill_c[i] / 1.53);
            }
        }
        String x_str = "";
        String y_str = "";
        String op = "";
        for (int i = 0; i < n; i++)
        {
            x_str += op + x2[i];
            y_str += op + y2[i];
            op = ",";
        }
        lb_fill_x2.Text = x_str;
        lb_fill_y2.Text = y_str;
      
    }

    public void set_fillimg()
    {
        boxplot(lb_fill_stitch.Text, 4, WebChartViewer_fill1);
        set_fillvalue();
        Image_fill_2.ImageUrl = "data:image/jpeg;base64," + TransIntToImage(lb_fill_x2.Text, lb_fill_y2.Text, int.Parse(lb_fill_offset_x.Text), int.Parse(lb_fill_offset_y.Text));
        Image_fill_value_2.ImageUrl = "data:image/jpeg;base64," + TransIntTovaluemap(lb_fill_x2.Text, lb_fill_y2.Text);
        lb_img_fill_2.Text = lb_img_fill_1.Text;
        lb_img_fill_2_1.Text = lb_img_fill_1_1.Text;

        String[] stitch_point = lb_fill_stitch_point.Text.Split(',');
        String[] x2 = lb_fill_x2.Text.Split(',');
        String stich = "";
        foreach (String i in stitch_point)
        {
            if (stich == "")
                stich = x2[int.Parse(i) - 1];
            else
                stich += "," + x2[int.Parse(i) - 1];
        }
        boxplot(stich, 4, WebChartViewer_fill2);
    }

   

}

