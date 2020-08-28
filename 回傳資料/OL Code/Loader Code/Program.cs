using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace b1olmap_getdata
{
    class Program
    {
        static DateTime now = DateTime.Parse("2020-05-10 00:00:00");
        static DateTime now2 = DateTime.Now;
        static String connString = "Database=array_olamp_config Data Source=host_name;User Id=userid;Password=userpw;CharSet=utf8;SslMode=None;allowPublicKeyRetrieval=true";
        static MySqlConnection conn;

        static void Main(string[] args)
        {

            



            try {

                conn = new MySqlConnection(connString);
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Open();

                //now = getdbtime();

                
                Console.WriteLine(now + "///" + now2);
            
                DataTable dt_insert = get_mapdata(); //get mqc data
                dt_insert = get_cvd_data(dt_insert); //get cvd data
                
                

                Console.WriteLine(dt_insert.Rows.Count);

                if (dt_insert.Rows.Count > 0) 
                    insert_ol_db(dt_insert); //insert mqc db
                
                

                String[] sheet_id = get_sheet_str();
                
                if (sheet_id.Length > 0)
                {

                    DataTable dt_kpc_insert = get_kpc_data(sheet_id); //get_kpc_data
                    insert_kpc_db(dt_kpc_insert); // insert kpc data

                    dt_kpc_insert = get_kpc_data_bm30(sheet_id); //get_kpc_data_bm30
                    insert_kpc_db(dt_kpc_insert); // insert kpc data


                    Console.WriteLine("join");
                    insert_join_two();  //join two table in mysql
                }

                delete_db(); //delete data 3 years ago
            

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }

            }        
            catch(Exception ee)
            {
                send_mail(ee.Message);
            }
            

        }
        static void delete_db()
        {
            

            String sql2 = "delete FROM l7b_olmap.l7bb1_olmap where aoi_meas_time < now()-interval 1125 day;";
            MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
            cmd2.ExecuteNonQuery();
        }
        static DateTime getdbtime()
        {
            MySqlConnection conn = new MySqlConnection(connString);

            string sql = "select aoi_meas_time from l7b_olmap.l7bb1_olmap order by aoi_meas_time desc limit 1;";
            MySqlDataAdapter DA = new MySqlDataAdapter(sql, conn);
            DataTable DT = new DataTable();

            DA.Fill(DT);

            conn.Close();

            DateTime d = DateTime.Parse(DT.Rows[0][0].ToString());
            return d;

        }

        static DataTable get_cvd_data(DataTable dt_insert)
        {
            List<String> sheet_id_list = new List<String>();
            foreach(DataRow r in dt_insert.Rows)
            {
                if (Array.IndexOf(sheet_id_list.ToArray(), r["sheet_id"].ToString()) < 0)
                    sheet_id_list.Add(r["sheet_id"].ToString());
            }
            String[] sheet_id = sheet_id_list.ToArray();
            String sheet_id_str = "";
            foreach(String s in sheet_id)
            {
                if (sheet_id_str == "")
                    sheet_id_str = "'" + s + "'";
                else
                    sheet_id_str += ",'" + s + "'";
            }



            String sql = "";
            sql += " select t.sheet_id_chip_id as sheet_id, ";
            sql += " TO_CHAR(t.logoff_time, 'YYYY-MM-DD HH24:MI:SS') AS cvd_logofftime, ";
            sql += " t.eqp_id as cvd_eqp, ";
            sql += " op_id as cvd_op, ";
            sql += " t.chamber_id_list as cvd_chb  ";
            sql += " from aryods.h_sheet_oper_ods t ";
            sql += " where 1=1 ";
            sql += " and t.sheet_id_chip_id in (" + sheet_id_str + ") ";
            sql += " and t.pep_level = 'PEP2' ";
            sql += " and t.EQP_ID like '%CVD%00' ";
            sql += " order by t.logoff_time desc ";
            DataTable dt_cvd = GetDTL7B(sql, 1);
            if (dt_cvd.Rows.Count > 0)
            {
                for (int i = 0; i < dt_insert.Rows.Count; i++)
                {
                    DataRow[] dr = dt_cvd.Select("sheet_id = '" + dt_insert.Rows[i]["sheet_id"].ToString() + "'", "");
                    if (dr.Length > 0)
                    {
                        if (DateTime.Parse(dt_insert.Rows[i]["aoi_meas_time"].ToString()) > DateTime.Parse(dr[0]["cvd_logofftime"].ToString()))
                        {
                            dt_insert.Rows[i]["cvd_logofftime"] = dr[0]["cvd_logofftime"].ToString();
                            dt_insert.Rows[i]["cvd_eqp"] = dr[0]["cvd_eqp"].ToString();
                            dt_insert.Rows[i]["cvd_op"] = dr[0]["cvd_op"].ToString();
                            dt_insert.Rows[i]["cvd_chb"] = dr[0]["cvd_chb"].ToString();
                        }
                    }
                }
            }
            return dt_insert;
        }

        static void insert_join_two()
        {
            String sql = "";
            sql += " replace into l7b_olmap.l7bb1_olmap ";
            sql += " select t.aoi_meas_time,t.chart_id,t.sheet_id,t.meas_tool,t.process_tool,t.unit2,t.process_time,  ";
            sql += " t.actual_recipe2,t.product_code2,t.Recipe_MaskName2,t.layer,t.Start_Time2,t.end_time2,  ";
            sql += " t.PSA2,t.X,t.Y, ";
            sql += " t.CP_R_TEMP_R2,t.STAGE_R_TEMP1_R2,t.STAGE_R_TEMP2_R2,t.CP_L_TEMP_R2,t.STAGE_L_TEMP1_R2,t.STAGE_L_TEMP2_R2,t.cvd_logofftime,t.cvd_eqp,t.cvd_op,t.cvd_chb, ";

            sql += " t.AlignOffset_X_S1_2, ";
            sql += " t.AlignOffset_Y_S1_2, ";
            sql += " t.AlignOffset_Z_S1_2, ";

            sql += " t.AlignOffset_X_S2_2, ";
            sql += " t.AlignOffset_Y_S2_2, ";
            sql += " t.AlignOffset_Z_S2_2, ";

            sql += " t.AlignOffset_X_S3_2, ";
            sql += " t.AlignOffset_Y_S3_2, ";
            sql += " t.AlignOffset_Z_S3_2, ";

            sql += " t.AlignOffset_X_S4_2, ";
            sql += " t.AlignOffset_Y_S4_2, ";
            sql += " t.AlignOffset_Z_S4_2, ";

            sql += " t.AlignF_RLX_S2_2, ";
            sql += " t.RRX_S2_2 ";

            sql += " from (  ";
            sql += " select * from   ";
            sql += " (  ";
            sql += " SELECT * FROM l7b_olmap.l7bb1_olmap where unit is null and aoi_meas_time > NOW() - INTERVAL 10 DAY  ";
            sql += " ) olmap  ";
            sql += " inner join  ";
            sql += " (  ";
            sql += " select process_time,sheet_id as sheet_id2,product_code as product_code2,process_tool as process_tool2,unit as unit2,actual_recipe as actual_recipe2,Recipe_MaskName as Recipe_MaskName2,layer as layer2,Start_Time as Start_Time2,end_time as end_time2,  ";
            sql += " PSA as PSA2,CP_R_TEMP_R as CP_R_TEMP_R2,STAGE_R_TEMP1_R as STAGE_R_TEMP1_R2,STAGE_R_TEMP2_R as STAGE_R_TEMP2_R2,CP_L_TEMP_R as CP_L_TEMP_R2,STAGE_L_TEMP1_R as STAGE_L_TEMP1_R2,STAGE_L_TEMP2_R as STAGE_L_TEMP2_R2  ";
            sql += " ,AlignOffset_X_S1 as AlignOffset_X_S1_2  ";
            sql += " ,AlignOffset_Y_S1 as AlignOffset_Y_S1_2  ";
            sql += " ,AlignOffset_Z_S1 as AlignOffset_Z_S1_2 ";
            sql += " ,AlignOffset_X_S2 as AlignOffset_X_S2_2 ";
            sql += " ,AlignOffset_Y_S2 as AlignOffset_Y_S2_2 ";
            sql += " ,AlignOffset_Z_S2 as AlignOffset_Z_S2_2 ";
            sql += " ,AlignOffset_X_S3 as AlignOffset_X_S3_2  ";
            sql += " ,AlignOffset_Y_S3 as AlignOffset_Y_S3_2 ";
            sql += " ,AlignOffset_Z_S3 as AlignOffset_Z_S3_2 ";
            sql += " ,AlignOffset_X_S4 as AlignOffset_X_S4_2 ";
            sql += " ,AlignOffset_Y_S4 as AlignOffset_Y_S4_2  ";
            sql += " ,AlignOffset_Z_S4 as AlignOffset_Z_S4_2 ";
            sql += " ,AlignF_RLX_S2 as AlignF_RLX_S2_2 ";
            sql += " ,RRX_S2 as RRX_S2_2 ";


            sql += " FROM l7b_olmap.l7bb1_olmap_kpc ";
            sql += " ) kpc  ";
            sql += " on olmap.sheet_id = kpc.sheet_id2 and CONCAT(SUBSTRING(olmap.process_tool,1,LENGTH(olmap.process_tool)-1),'0') = CONCAT(SUBSTRING(kpc.process_tool2,1,LENGTH(kpc.process_tool2)-1),'0')  and olmap.layer = kpc.layer2  ";
            sql += " and olmap.aoi_meas_time > kpc.process_time  ";
            sql += " order by kpc.process_time asc  ";
            sql += " ) t ";

            MySqlCommand cmd = new MySqlCommand(sql, conn);

            String sql2 = "Truncate table l7b_olmap.l7bb1_olmap_kpc";
            MySqlCommand cmd2 = new MySqlCommand(sql2, conn);

            cmd.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
        }
        static DataTable get_kpc_data(String[] sheet_id)
        {
            String sql = "";
            sql += " select * from ( ";
            sql += "     select k.*,psa.LINE_ID as PSA,temp.*,rank() over(partition by k.process_time,k.sheet_id,k.unit,k.start_time order by temp.report_time desc) rank from   ";
            sql += "     (  ";
            sql += "         select             TO_CHAR(x.report_time, 'YYYY/MM/DD HH24:MI:SS') as process_time,  ";
            sql += "                                                                 x.sheet_id as sheet_id,  ";
            sql += "                                                             x.product_code as product_code, ";
            sql += "                                                                  x.tool_id as process_tool,  ";
            sql += "                                                                  x.unit_id as unit,  ";
            sql += "                                                            x.actual_recipe as actual_recipe,  ";
            sql += "         replace(x.item_value003,'*','') || replace(x.item_value004,'*','') as Recipe_MaskName,  ";
            sql += "                                                             x.operation_id as layer,  ";
            sql += "         CASE WHEN (x.tool_id = 'FDSALN20') THEN to_date(x.item_value014,'YYMMDDHH24MISS')  ";
            sql += "         WHEN (x.tool_id like 'FD%ALN10' or x.tool_id like 'FD%ALN20' or x.tool_id = 'FDMALN30') THEN to_date(x.item_value015,'YYMMDDHH24MISS')   ";
            sql += "         else null end  ";
            sql += "             as start_time,  ";
            sql += "         CASE WHEN (x.tool_id = 'FDSALN20') THEN to_date(x.item_value015,'YYMMDDHH24MISS')  ";
            sql += "         WHEN (x.tool_id like 'FD%ALN10' or x.tool_id like 'FD%ALN20' or x.tool_id = 'FDMALN30') THEN to_date(x.item_value016,'YYMMDDHH24MISS')  ";
            sql += "         else null end  ";
            sql += "             as end_time,  ";

            sql += " case when tool_id='FDSALN20' then x.item_value084 ";
            sql += " else x.item_value102 end as A_RLX_S2, ";
            sql += " case when tool_id='FDSALN20' then x.item_value086 ";
            sql += " else x.item_value104 end as RRX_S2, ";
            sql += " case when tool_id='FDSALN20' then x.item_value034 ";
            sql += " else x.item_value042 end as A_OFFSET_X_S1, ";
            sql += " case when tool_id='FDSALN20' then x.item_value035 ";
            sql += " else x.item_value043 end as A_OFFSET_Y_S1, ";
            sql += " case when tool_id='FDSALN20' then x.item_value036 ";
            sql += " else x.item_value044 end as A_OFFSET_T_S1, ";
            sql += " case when tool_id='FDSALN20' then x.item_value073 ";
            sql += " else x.item_value080 end as A_OFFSET_X_S2, ";
            sql += " case when tool_id='FDSALN20' then x.item_value074 ";
            sql += " else x.item_value081 end as A_OFFSET_Y_S2, ";
            sql += " case when tool_id='FDSALN20' then x.item_value075 ";
            sql += " else x.item_value082 end as A_OFFSET_T_S2, ";
            sql += " case when tool_id='FDSALN20' then x.item_value112 ";
            sql += " else x.item_value118 end as A_OFFSET_X_S3, ";
            sql += " case when tool_id='FDSALN20' then x.item_value113 ";
            sql += " else x.item_value119 end as A_OFFSET_Y_S3, ";
            sql += " case when tool_id='FDSALN20' then x.item_value114 ";
            sql += " else x.item_value120 end as A_OFFSET_T_S3, ";
            sql += " case when tool_id='FDSALN20' then x.item_value151 ";
            sql += " else x.item_value156 end as A_OFFSET_X_S4, ";
            sql += " case when tool_id='FDSALN20' then x.item_value152 ";
            sql += " else x.item_value157 end as A_OFFSET_Y_S4, ";
            sql += " case when tool_id='FDSALN20' then x.item_value153 ";
            sql += " else x.item_value158 end as A_OFFSET_T_S4 ";



            sql += "         from cfspch.h_raw_kpc x   ";
            sql += "         where 1=1  ";

            String s_sql = " and (x.sheet_id in ('123'";
            for (int i = 0; i < sheet_id.Length; i++)
            {
                s_sql += "," + sheet_id[i];
                if (i % 900 == 0 || i == sheet_id.Length - 1)
                {
                    sql += s_sql + " )";
                    s_sql = " or x.sheet_id in ('123'";
                }
            }
            sql += " )";

            sql += "         and ( x.tool_id like 'FD%ALN10' or x.tool_id like 'FD%ALN20') ";
            sql += "         and x.tool_id <> 'FDMALN10' ";
            sql += "         and x.dc_item_group = '0000'  ";
            sql += "     ) k  ";
            sql += "     left join  ";
            sql += "     (  ";
            sql += "         select report_time,sheet_id,line_id  ";
            sql += "         from ";
            sql += "         (  ";
            sql += "             select x.report_time,x.sheet_id,x.tool_id as line_id,rank() over(partition by x.sheet_id,x.line_id order by x.report_time desc) rank  ";
            sql += "             from cfspch.h_raw_kpc x  ";
            sql += "             where 1=1  ";

            s_sql = " and (x.sheet_id in ('123'";
            for (int i = 0; i < sheet_id.Length; i++)
            {
                s_sql += "," + sheet_id[i];
                if (i % 900 == 0 || i == sheet_id.Length - 1)
                {
                    sql += s_sql + " )";
                    s_sql = " or x.sheet_id in ('123'";
                }
            }
            sql += " )";

            sql += "             and x.operation_id ='BM'  ";
            sql += "             and x.dc_item_group = '0000' ";
            sql += "             and x.tool_id like 'FDMMPA%' ";
            sql += "         ) tt  ";
            sql += "         where tt.rank = 1  ";
            sql += "     ) psa  ";
            sql += "     on k.sheet_id = psa.sheet_id and to_date(k.process_time,'YYYY-MM-DD HH24:MI:SS') > psa.report_time  ";
            sql += "     left join  ";
            sql += "     (  ";
            sql += "         select  ";
            sql += "             TO_DATE(TO_CHAR(x.report_time, 'YYYY/MM/DD HH24:MI:SS'),'YYYY/MM/DD HH24:MI:SS') AS report_time,  ";
            sql += "             x.tool_id as process_tool2,  ";
            sql += "             CASE WHEN (x.tool_id = 'FDSALN20') THEN x.item_value024  ";
            sql += "             WHEN (x.tool_id = 'FDMALN30') THEN x.item_value004 ";
            sql += "             else x.item_value022 end  ";
            sql += "                 as CP_R_TEMP_R,  ";
            sql += "             CASE WHEN (x.tool_id = 'FDSALN20') THEN x.item_value027  ";
            sql += "             WHEN (x.tool_id = 'FDMALN30') THEN x.item_value007 ";
            sql += "             else x.item_value025 end  ";
            sql += "                 as STAGE_R_TEMP1_R,  ";
            sql += "             CASE WHEN (x.tool_id = 'FDSALN20') THEN x.item_value028  ";
            sql += "             WHEN (x.tool_id = 'FDMALN30') THEN x.item_value008 ";
            sql += "             else x.item_value026 end  ";
            sql += "                 as STAGE_R_TEMP2_R,  ";
            sql += "             CASE WHEN (x.tool_id = 'FDSALN20') THEN x.item_value023  ";
            sql += "             WHEN (x.tool_id = 'FDMALN30') THEN x.item_value003 ";
            sql += "             else x.item_value021 end  ";
            sql += "                 as CP_L_TEMP_R,  ";
            sql += "             CASE WHEN (x.tool_id = 'FDSALN20') THEN x.item_value025  ";
            sql += "             WHEN (x.tool_id = 'FDMALN30') THEN x.item_value005 ";
            sql += "             else x.item_value023 end  ";
            sql += "                 as STAGE_L_TEMP1_R,  ";
            sql += "             CASE WHEN (x.tool_id = 'FDSALN20') THEN x.item_value026  ";
            sql += "             WHEN (x.tool_id = 'FDMALN30') THEN x.item_value006 ";
            sql += "             else x.item_value024 end  ";
            sql += "                 as STAGE_L_TEMP2_R  ";
            sql += "         from cfspch.h_raw_kpc x   ";
            sql += "         where 1=1  ";
            sql += "         and ( x.tool_id like 'FD%ALN10' or x.tool_id like 'FD%ALN20')   ";
            sql += "         and x.mes_id = 'INLINE_L'  ";
            sql += "         and x.rpt_unit = 'T'  ";
            sql += "         and x.dc_item_group = '0000'  ";
            sql += "     ) temp  ";
            sql += "     on k.process_tool = temp.process_tool2 and k.START_TIME < temp.report_time and k.END_TIME > temp.report_time  ";
            sql += " ) y  ";
            sql += " where y.rank = 1  ";
            //Console.WriteLine(sql);
            //Console.ReadKey();
            DataTable DT = new DataTable();
            DT = GetDTL7B(sql,0);

            return DT;

        }

        static DataTable get_kpc_data_bm30(String[] sheet_id)
        {
            String sql = "";
            sql += " select * from ( ";
            sql += "        select k.*,psa.LINE_ID as PSA,temp.*,rank() over(partition by k.process_time,k.sheet_id,k.unit,k.start_time order by temp.report_time desc) rank from ";
            sql += "     ( ";
            sql += " select             TO_CHAR(x.report_time, 'YYYY/MM/DD HH24:MI:SS') as process_time,   ";
            sql += "                                                                  x.sheet_id as sheet_id,   ";
            sql += "                                                              x.product_code as product_code,  ";
            sql += "                                                                   x.tool_id as process_tool,   ";
            sql += "                                                                   x.unit_id as unit,   ";
            sql += "                                                             x.actual_recipe as actual_recipe,   ";
            sql += "          replace(x.item_value003,'*','') || replace(x.item_value004,'*','') as Recipe_MaskName,   ";
            sql += "                                                              x.operation_id as layer,   ";
            sql += "                                   to_date(x.item_value015,'YYMMDDHH24MISS') as start_time,   ";
            sql += "                                   to_date(x.item_value016,'YYMMDDHH24MISS') as end_time,   ";
            sql += "                                                             x.item_value176 as A_RLX_S2, ";
            sql += "                                                             x.item_value178 as RRX_S2, ";
            sql += "                                                             x.item_value071 as A_OFFSET_X_S1, ";
            sql += "                                                             x.item_value072 as A_OFFSET_Y_S1, ";
            sql += "                                                             x.item_value073 as A_OFFSET_Z_S1, ";
            sql += "                                                             x.item_value154 as A_OFFSET_X_S2, ";
            sql += "                                                             x.item_value155 as A_OFFSET_Y_S2, ";
            sql += "                                                             x.item_value156 as A_OFFSET_Z_S2 ";
            sql += "           ";
            sql += "          from cfspch.h_raw_kpc x    ";
            sql += "          where 1=1   ";

            String s_sql = " and (x.sheet_id in ('123'";
            for (int i = 0; i < sheet_id.Length; i++)
            {
                s_sql += "," + sheet_id[i];
                if (i % 900 == 0 || i == sheet_id.Length - 1)
                {
                    sql += s_sql + " )";
                    s_sql = " or x.sheet_id in ('123'";
                }
            }
            sql += " )";

            sql += "          and x.tool_id = 'FDMALN30' ";
            sql += "          and x.dc_item_group = '0000' ";
            sql += "     ) k ";
            sql += " left join ";
            sql += "     ( ";
            sql += "     select                                                       x.sheet_id as sheet_id2, ";
            sql += "                                                             x.item_value037 as A_OFFSET_X_S3, ";
            sql += "                                                             x.item_value038 as A_OFFSET_Y_S3, ";
            sql += "                                                             x.item_value039 as A_OFFSET_Z_S3, ";
            sql += "                                                             x.item_value120 as A_OFFSET_X_S4, ";
            sql += "                                                             x.item_value121 as A_OFFSET_Y_S4, ";
            sql += "                                                             x.item_value122 as A_OFFSET_Z_S4 ";
            sql += "           ";
            sql += "          from cfspch.h_raw_kpc x    ";
            sql += "          where 1=1   ";

            s_sql = " and (x.sheet_id in ('123'";
            for (int i = 0; i < sheet_id.Length; i++)
            {
                s_sql += "," + sheet_id[i];
                if (i % 900 == 0 || i == sheet_id.Length - 1)
                {
                    sql += s_sql + " )";
                    s_sql = " or x.sheet_id in ('123'";
                }
            }
            sql += " )";

            sql += "          and x.tool_id = 'FDMALN30' ";
            sql += "          and x.dc_item_group = '0001' ";
            sql += "    ) k2 ";
            sql += " on k.sheet_id = k2.sheet_id2 ";
            sql += " left join   ";
            sql += "      (   ";
            sql += "          select report_time,sheet_id,line_id   ";
            sql += "          from  ";
            sql += "          (   ";
            sql += "              select x.report_time,x.sheet_id,x.tool_id as line_id,rank() over(partition by x.sheet_id,x.line_id order by x.report_time desc) rank   ";
            sql += "              from cfspch.h_raw_kpc x   ";
            sql += "              where 1=1   ";

            s_sql = " and (x.sheet_id in ('123'";
            for (int i = 0; i < sheet_id.Length; i++)
            {
                s_sql += "," + sheet_id[i];
                if (i % 900 == 0 || i == sheet_id.Length - 1)
                {
                    sql += s_sql + " )";
                    s_sql = " or x.sheet_id in ('123'";
                }
            }
            sql += " )";

            sql += "              and x.operation_id ='BM'   ";
            sql += "              and x.dc_item_group = '0000'  ";
            sql += "              and x.tool_id like 'FDMMPA%'  ";
            sql += "          ) tt   ";
            sql += "          where tt.rank = 1   ";
            sql += "      ) psa   ";
            sql += "      on k.sheet_id = psa.sheet_id and to_date(k.process_time,'YYYY-MM-DD HH24:MI:SS') > psa.report_time  ";
            sql += " left join   ";
            sql += "      (   ";
            sql += "          select   ";
            sql += "              TO_DATE(TO_CHAR(x.report_time, 'YYYY/MM/DD HH24:MI:SS'),'YYYY/MM/DD HH24:MI:SS') AS report_time,   ";
            sql += "              x.tool_id as process_tool2,   ";
            sql += "              x.item_value004 as CP_R_TEMP_R,   ";
            sql += "              x.item_value007 as STAGE_R_TEMP1_R,   ";
            sql += "              x.item_value008 as STAGE_R_TEMP2_R,   ";
            sql += "              x.item_value003 as CP_L_TEMP_R,   ";
            sql += "              x.item_value005 as STAGE_L_TEMP1_R,   ";
            sql += "              x.item_value006 as STAGE_L_TEMP2_R   ";
            sql += "          from cfspch.h_raw_kpc x    ";
            sql += "          where 1=1   ";
            sql += "          and x.tool_id = 'FDMALN30' ";
            sql += "          and x.mes_id = 'INLINE_L'   ";
            sql += "          and x.rpt_unit = 'T'   ";
            sql += "          and x.dc_item_group = '0000'   ";
            sql += "      ) temp   ";
            sql += "      on k.process_tool = temp.process_tool2 and k.START_TIME < temp.report_time and k.END_TIME > temp.report_time   ";
            sql += "  ) y   ";
            sql += "  where y.rank = 1  ";

            DataTable DT = new DataTable();
            DT = GetDTL7B(sql, 0);

            return DT;
        }

        static void insert_ol_db(DataTable dt_insert)
        {
            String sql = "";
            sql += " INSERT IGNORE INTO `l7b_olmap`.`l7bb1_olmap` ";
            sql += " (`aoi_meas_time`,`chart_id`,`sheet_id`,`meas_tool`,`process_tool`, `layer`,`X`,`Y`,`cvd_logofftime`,`cvd_eqp`,`cvd_op`,`cvd_chb` ) VALUES ";

            String op2 = "";
            String sql_insert = sql;
            for (int i = 0; i < dt_insert.Rows.Count; i++)
            {
                String insert_s = "";


                
                String[] OL_X = dt_insert.Rows[i]["OL_X"].ToString().Split(',');
                String[] OL_Y = dt_insert.Rows[i]["OL_Y"].ToString().Split(',');
                for (int j = 0; j < OL_X.Length; j++)
                {
                    try
                    {
                       OL_X[j] = double.Parse(OL_X[j]).ToString("##0.00000");
                       
                     }
                     catch (Exception ex)
                     {

                      }
                }
                
                
                for (int j = 0; j < OL_Y.Length; j++)
                {
                    try
                    {
                        OL_Y[j] = double.Parse(OL_Y[j]).ToString("##0.0000000");
                       
                     }
                     catch (Exception ex)
                     {

                     }
                }
                // double[] OL_Y = new double[OL_Y_str.Length];

                //if (OL_X.Length != OL_Y.Length)
                String X = "";
                String Y = "";
                String op = "";
                foreach (String s in OL_X)
                {
                    X += op+s;                    
                    op = ",";
                }
                op = "";
                foreach (String s in OL_Y)
                {
                    Y += op + s;
                    op = ",";
                }


                op = "";
                insert_s += "( ";
                insert_s += "'" + dt_insert.Rows[i]["aoi_meas_time"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["chart_id"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["sheet_id"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["meas_tool"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["process_tool"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["layer"].ToString() + "'";
                insert_s += ",'" + X + "'";
                insert_s += ",'" + Y + "'";
                if(dt_insert.Rows[i]["cvd_logofftime"].ToString() != "")
                    insert_s += ",'" + dt_insert.Rows[i]["cvd_logofftime"].ToString() + "'";
                else
                    insert_s += ",null ";
                insert_s += ",'" + dt_insert.Rows[i]["cvd_eqp"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["cvd_op"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["cvd_chb"].ToString() + "'";
                insert_s += " )";
                sql_insert += op2 + insert_s;
                op2 = ",";
                if ((i!=0 && i % 10000 == 0) || i == dt_insert.Rows.Count - 1)
                {
                    MySqlCommand cmd = new MySqlCommand(sql_insert, conn);
                    //Console.WriteLine(sql_insert);
                    cmd.ExecuteNonQuery();
                    sql_insert = sql;
                    op2 = "";
                    op = "";
                }
                else if (i == dt_insert.Rows.Count - 1)
                {
                    MySqlCommand cmd = new MySqlCommand(sql_insert, conn);
                    cmd.ExecuteNonQuery();
                    sql_insert = sql;
                    op2 = "";
                    op = "";
                }

            }

        }
        static void insert_kpc_db(DataTable dt_insert)
        {
            String sql = "";
            sql += " INSERT IGNORE INTO `l7b_olmap`.`l7bb1_olmap_kpc` ";
            sql += " (`process_time`,`sheet_id`,`product_code`,`process_tool`,`unit`,`actual_recipe`, ";
            sql += " `Recipe_MaskName`,`layer`,`Start_Time`,`end_time`,`PSA`, ";
            sql += " `CP_R_TEMP_R`,`STAGE_R_TEMP1_R`,`STAGE_R_TEMP2_R`,`CP_L_TEMP_R`,`STAGE_L_TEMP1_R`,`STAGE_L_TEMP2_R` , ";
            sql += " `AlignOffset_X_S1`,`AlignOffset_Y_S1`,`AlignOffset_Z_S1`,`AlignOffset_X_S2`,`AlignOffset_Y_S2`, ";
            sql += " `AlignOffset_Z_S2`,`AlignOffset_X_S3`,`AlignOffset_Y_S3`,`AlignOffset_Z_S3`,`AlignOffset_X_S4`, ";
            sql += " `AlignOffset_Y_S4`,`AlignOffset_Z_S4`,`AlignF_RLX_S2`,`RRX_S2` ";
            sql += " ) VALUES ";
            String op2 = "";
            String sql_insert = sql;
            for (int i = 0; i < dt_insert.Rows.Count; i++)
            {
                String insert_s = "";

                String op = "";
                insert_s += "( ";
                insert_s += "'" + dt_insert.Rows[i]["process_time"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["sheet_id"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["product_code"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["process_tool"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["unit"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["actual_recipe"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["Recipe_MaskName"].ToString() + "'";
                insert_s += ",'" + dt_insert.Rows[i]["layer"].ToString() + "'";
                try {
                    insert_s += ",'" + DateTime.Parse(dt_insert.Rows[i]["Start_Time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    insert_s += ",'" + DateTime.Parse(dt_insert.Rows[i]["End_Time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                }
                catch(Exception ee)
                {
                    insert_s += ",null";
                    insert_s += ",null";                    
                }
                if(dt_insert.Rows[i]["PSA"].ToString() == "")
                    insert_s += ",null";
                else
                    insert_s += ",'" + dt_insert.Rows[i]["PSA"].ToString() + "'";


                if (dt_insert.Rows[i]["CP_R_TEMP_R"].ToString() == "")
                    insert_s += ",null";
                else
                    insert_s += ",'" + dt_insert.Rows[i]["CP_R_TEMP_R"].ToString() + "'";

                if (dt_insert.Rows[i]["STAGE_R_TEMP1_R"].ToString() == "")
                    insert_s += ",null";
                else
                    insert_s += ",'" + dt_insert.Rows[i]["STAGE_R_TEMP1_R"].ToString() + "'";
                if (dt_insert.Rows[i]["STAGE_R_TEMP2_R"].ToString() == "")
                    insert_s += ",null";
                else
                    insert_s += ",'" + dt_insert.Rows[i]["STAGE_R_TEMP2_R"].ToString() + "'";

                if (dt_insert.Rows[i]["CP_L_TEMP_R"].ToString() == "")
                    insert_s += ",null";
                else
                    insert_s += ",'" + dt_insert.Rows[i]["CP_L_TEMP_R"].ToString() + "'";

                if (dt_insert.Rows[i]["STAGE_L_TEMP1_R"].ToString() == "")
                    insert_s += ",null";
                else
                    insert_s += ",'" + dt_insert.Rows[i]["STAGE_L_TEMP1_R"].ToString() + "'";
                if (dt_insert.Rows[i]["STAGE_L_TEMP2_R"].ToString() == "")
                    insert_s += ",null";
                else
                    insert_s += ",'" + dt_insert.Rows[i]["STAGE_L_TEMP2_R"].ToString() + "'";

                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_X_S1"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }
                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_Y_S1"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }
                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_T_S1"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }

                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_X_S2"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }
                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_Y_S2"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }
                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_T_S2"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }

                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_X_S3"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }
                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_Y_S3"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }
                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_T_S3"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }
                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_X_S4"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }
                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_Y_S4"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }
                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_OFFSET_T_S4"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }
                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["A_RLX_S2"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }
                try
                {
                    insert_s += "," + double.Parse(dt_insert.Rows[i]["RRX_S2"].ToString()).ToString();
                }
                catch
                {
                    insert_s += ",null";
                }





                insert_s += " )";

                sql_insert += op2 + insert_s;
                op2 = ",";
                if ((i != 0 && i % 10000 == 0) || i == dt_insert.Rows.Count - 1)
                {
                    MySqlCommand cmd = new MySqlCommand(sql_insert, conn);
                   //Console.WriteLine(sql_insert);
                    cmd.ExecuteNonQuery();
                    sql_insert = sql;
                    op2 = "";
                    op = "";
                }
                else if (i == dt_insert.Rows.Count - 1)
                {
                    MySqlCommand cmd = new MySqlCommand(sql_insert, conn);
                    cmd.ExecuteNonQuery();
                    sql_insert = sql;
                    op2 = "";
                    op = "";
                }

            }

        }

        static String[] get_sheet_str()
        {
            String sql = "";

            sql += "SELECT distinct sheet_id  FROM l7b_olmap.l7bb1_olmap where aoi_meas_time > NOW() - INTERVAL 8 DAY and  unit is null;";
            //sql = "SELECT distinct sheet_id  FROM l7b_olmap.l7bb1_olmap where aoi_meas_time > NOW() - INTERVAL 8 DAY;";

            //sql += "SELECT distinct sheet_id FROM l7bary_kpi.l7b_pep3_cvd where H_EQP_ID is null and RP_MFG_DAY > current_date()-7";
            //sql = "SELECT distinct sheet_id FROM l7bary_kpi.l7b_pep3_cvd where CHIP_ID=''";

            DataTable dt = GetDTL7B_mysql(sql);
            ArrayList myAL = new ArrayList();

            foreach (DataRow r in dt.Rows)
            {
                myAL.Add("'" + r[0].ToString() + "'");
                /*str += op + "'" + str + "'";
                op = ",";*/
            }
            String[] str1 = (string[])myAL.ToArray(Type.GetType("System.String"));
            return str1;

        }

        static String[] get_chart_id()
        {
            String sql = "";

            sql += "SELECT distinct chart_id FROM l7b_olmap.l7bb1_olmap_config;";

            DataTable dt = GetDTL7B_mysql(sql);
            ArrayList myAL = new ArrayList();

            foreach (DataRow r in dt.Rows)
            {
                myAL.Add(r[0].ToString());
            }
            String[] str1 = (string[])myAL.ToArray(Type.GetType("System.String"));
            return str1;

        }

        static DataTable get_mapdata()
        {
            String[] chart_id = get_chart_id();
            String chart_id_x = "";
            String chart_id_y = "";
            String op = "";
            foreach (String s in chart_id)
            {
                chart_id_x += op + "'" + s + "_X'";
                chart_id_y += op + "'" + s + "_Y'";
                op = ",";
            }



            String sql = "";

            


            sql += " SELECT TO_CHAR(x.reporttime, 'YYYY-MM-DD HH24:MI:SS') AS aoi_meas_time,x.inforvalue1 as meas_tool,Substr(x.chartid,0,LENGTH(x.chartid)-2) as chart_id,x.inforvalue5 as sheet_id ";
            sql += " ,CASE WHEN (x.inforvalue11 like 'BMEXP30%' or x.inforvalue10 = 'FDM30') THEN 'FDMALN30' ";
            sql += " else x.inforvalue11 end as process_tool ";
            sql += " ,x.rawitemvalues as OL_X,y.rawitemvalues as OL_Y  ";
            sql += " ,x.inforvalue8 as layer,'' as cvd_logofftime,'' as cvd_eqp, '' as cvd_op,'' as cvd_chb ";
            sql += " FROM c7bcfspch.spchis x ,c7bcfspch.spchis y  ";
            sql += " where 1=1 ";
            sql += " and x.inforvalue5 = y.inforvalue5 ";
            sql += " and x.reporttime > to_date('" + now.ToString("yyyy/MM/dd HH:mm:ss") + "', 'YYYY/MM/DD HH24:MI:SS')  ";
            sql += " and y.reporttime > to_date('" + now.ToString("yyyy/MM/dd HH:mm:ss") + "', 'YYYY/MM/DD HH24:MI:SS')  ";
            sql += " and x.chartid in (" + chart_id_x + ") ";
            sql += " and y.chartid in (" + chart_id_y + ") ";
            sql += " and Substr(x.chartid,0,LENGTH(x.chartid)-1) = Substr(y.chartid,0,LENGTH(y.chartid)-1) ";
            sql += " and y.graphtype = x.graphtype ";
            sql += " and x.graphtype = 'X'  ";
            sql += " order by aoi_meas_time ";
            //Console.WriteLine(sql);
            //Console.ReadKey();
            DataTable DT = new DataTable();
            DT = GetDTL7B(sql);
      
            return DT;
        }

        static DataTable GetDTL7B_mysql(String SQL)
        {
            MySqlDataAdapter DA = new MySqlDataAdapter(SQL, conn);
            DataTable DT = new DataTable();
            DA.Fill(DT);
            return DT;
        }

        static DataTable GetDTL7B(String SQL,int type = 2)
        {
            String Conn_Str = "Provider=MSDAORA;User ID=l7bint_ap;Data Source=TCSPCH_7BHMTS;Password=l7bint$ap";
            if(type==0)
                Conn_Str = "Provider=MSDAORA;User ID=l7bint_ap;Data Source=C7BH_SHA;Password=l7bint$ap";
            else if(type==1)
                Conn_Str = "Provider=MSDAORA;User ID=l7bint_ap;Data Source=L7BH;Password=l7bint$ap";
            OleDbConnection Conn = new OleDbConnection(Conn_Str);
            OleDbCommand Cmd = new OleDbCommand();
            Cmd.CommandText = SQL;
            Cmd.Connection = Conn;
            Conn.Open();
            OleDbDataAdapter ad = new OleDbDataAdapter(Cmd);
            DataTable dt = new DataTable();
            try
            {
                ad.Fill(dt);
            }
            catch { };
            Conn.Close();

            Conn.Dispose();

            return dt;
        }
        static void send_mail(String s)
        {
            string MESSAGE_Body = "";
            string oSubject, Mail_To, Mail_CC;
            oSubject = "TW100043343主機上OL_MAP程式執行失敗 !!";
            Mail_To = "Jacky.WF.Lin@auo.com";
            Mail_CC = "Jacky.WF.Lin@auo.com";
            //Mail_CC = "Meichun.Chen@auo.com";

            MESSAGE_Body += "<FONT style=font-size:20pt;font-family:Arial>";
            MESSAGE_Body += "<Br>";
            MESSAGE_Body += s;
            MESSAGE_Body += "<Br>";
            MESSAGE_Body += "DB_TIME:" + now.ToString("yyyy/MM/dd HH:mm:ss");
            MESSAGE_Body += "<Br>";
            MESSAGE_Body += "請確認程式的執行狀態";

            IDS.Mail sendmail = new IDS.Mail();
            sendmail.ManualSend_07("", Mail_To, Mail_CC, oSubject, MESSAGE_Body);

        }

    }
}
