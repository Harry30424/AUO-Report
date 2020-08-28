#!/usr/bin/env python
# coding: utf-8

# In[1]:


import cx_Oracle
import mysql.connector
import pandas as pd
# 通用設定
pd.set_option('display.max_columns',None)


# In[2]:


#MySQL 連線資訊
cnx = mysql.connector.connect(user='l8ai2_ap', password='l8ai2$ap', host='TW100040269', database='cfdata', charset="utf8")
cursor = cnx.cursor()

#查詢有量測 OL 的片子
SQL_Array = ["SELECT X.*, Y.Y "
            ,"  FROM (SELECT SUBSTR(CHARTID, 1, LENGTH(CHARTID) - INSTR(REVERSE(CHARTID), '_')) CHARTID,"
            ,"               REPORTTIME AOI_MEAS_TIME,"
            ,"               MEA_OP_ID,"
            ,"               MEA_TOOL_ID MEAS_TOOL,"
            ,"               SHEET_ID,"
            ,"               RAWITEMVALUES X"
            ,"          FROM SPC.CF_SPC_DATA"
            ,"         WHERE 1 = 1"
            ,"           AND REPORTTIME > NOW() - INTERVAL 4"
            ,"         DAY"
            ,"           AND CHARTID LIKE '%/ADI/OL_X'"
            ,"           AND GRAPHTYPE = 'X'"
            ,"           AND MEA_MES_ID = 'MIC-STA') X"
            ,""
            ," INNER JOIN"
            ,""
            ," (SELECT SUBSTR(CHARTID, 1, LENGTH(CHARTID) - INSTR(REVERSE(CHARTID), '_')) CHARTID,"
            ,"         SHEET_ID,"
            ,"         RAWITEMVALUES Y"
            ,"    FROM SPC.CF_SPC_DATA"
            ,"   WHERE 1 = 1"
            ,"     AND REPORTTIME > NOW() - INTERVAL 2 DAY"
            ,"     AND CHARTID LIKE '%/ADI/OL_Y'"
            ,"     AND GRAPHTYPE = 'X'"
            ,"     AND MEA_MES_ID = 'MIC-STA') Y"
            ,""
            ," ON X.CHARTID = Y.CHARTID"
            ," AND X.SHEET_ID = Y.SHEET_ID"]

#查詢 SQL
SQL_Query = ''
SQL_Query = '\n'.join(SQL_Array)
    
#print(SQL_Query)

cursor = cnx.cursor()

#查詢的結果轉 DataFrame
df_OL_GLS_Info = pd.read_sql(SQL_Query, con=cnx)

#關閉資料庫連線
cnx.close()


# In[3]:


# 查詢 Offset Data
def offset_data_SQL(cnx,sheetlist):
    
    SQL_Array = ["SELECT TO_CHAR(ALIGN.REPORT_TIME, 'yyyymmddhh24mi') || ALIGN.LINE_ID || ALIGN.TOOL_ID ID, ALIGN.*, BM_TOOL.BM_EQP"
    ,"  FROM (SELECT TO_CHAR(D1.REPORT_TIME,'YYYY/MM/DD HH24:MI:SS') REPORT_TIME_ok, D1.*,"
    ,"               D2.ALIGNOFFSET_X_S6,"
    ,"               D2.ALIGNOFFSET_Y_S6,"
    ,"               D2.ALIGNOFFSET_Z_S6"
    ,"          FROM (SELECT O.*, T.RECIPE_MASKNAME, T.START_TIME, T.END_TIME"
    ,"                  FROM (SELECT T.REPORT_TIME,"
    ,"                               T.SHEET_ID,"
    ,"                               T.LINE_ID,"
    ,"                               T.TOOL_ID,"
    ,"                               REPLACE(T.ITEM_VALUE173, '*', '') || '_' ||"
    ,"                               REPLACE(T.ITEM_VALUE174, '*', '') AS RECIPE_MASKNAME,"
    ,"                               TO_CHAR(TO_DATE(T.ITEM_VALUE185, 'YYMMDDHH24MISS'), 'YYYY/MM/DD HH24:MI:SS') AS START_TIME,"
    ,"                               TO_CHAR(TO_DATE(T.ITEM_VALUE186, 'YYMMDDHH24MISS'), 'YYYY/MM/DD HH24:MI:SS') AS END_TIME"
    ,"                          FROM CFSPCH.H_DC_KPC T"
    ,"                         WHERE 1 = 1"
    ,"                           AND T.SHEET_ID IN ("+sheetlist+")"
    ,"                           AND T.REPORT_TIME >= SYSDATE - 2"
    ,"                           AND T.TOOL_ID IN"
    ,"                               ('FFRALN10', 'FFGALN10', 'FFBALN10', 'FFSALN10')"
    ,"                           AND T.DC_ITEM_GROUP = '0000') T"
    ,"                 INNER JOIN (SELECT CASE SUBSTR(MODEL_NO,INSTR(MODEL_NO, '-') + 4,1) WHEN 'B' THEN 'ARRAY_GLASS' WHEN '1' THEN 'PSA' ELSE 'CF_GLASS' END Check_GLASS, T.REPORT_TIME,"
    ,"                                   T.SHEET_ID,"
    ,"                                   T.PRODUCT_CODE,"
    ,"                                   T.LINE_ID,"
    ,"                                   T.TOOL_ID,"
    ,"                                   T.OPERATION_ID LAYER,"
    ,"                                   T.UNIT_ID UNIT,"
    ,"                                   T.ACTUAL_RECIPE,"
    ,"                                   ITEM_VALUE012 ALIGNOFFSET_X_S1,"
    ,"                                   ITEM_VALUE013 ALIGNOFFSET_Y_S1,"
    ,"                                   ITEM_VALUE014 ALIGNOFFSET_Z_S1,"
    ,"                                   ITEM_VALUE050 ALIGNOFFSET_X_S2,"
    ,"                                   ITEM_VALUE051 ALIGNOFFSET_Y_S2,"
    ,"                                   ITEM_VALUE052 ALIGNOFFSET_Z_S2,"
    ,"                                   ITEM_VALUE089 ALIGNOFFSET_X_S3,"
    ,"                                   ITEM_VALUE090 ALIGNOFFSET_Y_S3,"
    ,"                                   ITEM_VALUE091 ALIGNOFFSET_Z_S3,"
    ,"                                   ITEM_VALUE126 ALIGNOFFSET_X_S4,"
    ,"                                   ITEM_VALUE127 ALIGNOFFSET_Y_S4,"
    ,"                                   ITEM_VALUE128 ALIGNOFFSET_Z_S4,"
    ,"                                   ITEM_VALUE164 ALIGNOFFSET_X_S5,"
    ,"                                   ITEM_VALUE165 ALIGNOFFSET_Y_S5,"
    ,"                                   ITEM_VALUE166 ALIGNOFFSET_Z_S5"
    ,"                              FROM CFSPCH.H_DC_KPC T"
    ,"                             WHERE 1 = 1"
    ,"                               AND T.SHEET_ID IN ("+sheetlist+")"
    ,"                               AND T.REPORT_TIME >= SYSDATE - 2"
    ,"                               AND T.TOOL_ID IN ('FFRALN10', 'FFGALN10',"
    ,"                                    'FFBALN10', 'FFSALN10')"
    ,"                               AND T.DC_ITEM_GROUP = '0001') O ON T.REPORT_TIME ="
    ,"                                                                  O.REPORT_TIME"
    ,"                                                              AND T.SHEET_ID ="
    ,"                                                                  O.SHEET_ID"
    ,"                                                              AND T.LINE_ID ="
    ,"                                                                  O.LINE_ID"
    ,"                                                              AND T.TOOL_ID ="
    ,"                                                                  O.TOOL_ID) D1"
    ,"         INNER JOIN (SELECT T.REPORT_TIME,"
    ,"                           T.SHEET_ID,"
    ,"                           T.PRODUCT_CODE,"
    ,"                           T.LINE_ID,"
    ,"                           T.TOOL_ID,"
    ,"                           ITEM_VALUE002 ALIGNOFFSET_X_S6,"
    ,"                           ITEM_VALUE003 ALIGNOFFSET_Y_S6,"
    ,"                           ITEM_VALUE004 ALIGNOFFSET_Z_S6"
    ,"                      FROM CFSPCH.H_DC_KPC T"
    ,"                     WHERE 1 = 1"
    ,"                       AND T.REPORT_TIME >= SYSDATE - 2"
    ,"                       AND T.SHEET_ID IN ("+sheetlist+")"
    ,"                       AND T.TOOL_ID IN"
    ,"                           ('FFRALN10', 'FFGALN10', 'FFBALN10', 'FFSALN10')"
    ,"                       AND T.DC_ITEM_GROUP = '0002') D2 ON D1.REPORT_TIME ="
    ,"                                                           D2.REPORT_TIME"
    ,"                                                       AND D1.SHEET_ID ="
    ,"                                                           D2.SHEET_ID"
    ,"                                                       AND D1.LINE_ID ="
    ,"                                                           D2.LINE_ID"
    ,"                                                       AND D1.TOOL_ID ="
    ,"                                                           D2.TOOL_ID) ALIGN"
    ," INNER JOIN (SELECT T.SHEET_ID, T.TOOL_ID BM_EQP"
    ,"               FROM CFSPCH.H_DC_KPC T"
    ,"              WHERE 1 = 1"
    ,"   AND T.REPORT_TIME >= SYSDATE - 7"
    ,"                AND T.SHEET_ID IN ("+sheetlist+")"
    ,"                AND T.TOOL_ID LIKE 'FFMALN%'"
    ,"                AND T.DC_ITEM_GROUP = '0000') BM_TOOL ON BM_TOOL.SHEET_ID ="
    ,"                                                         ALIGN.SHEET_ID"]

    #查詢 SQL
    SQL_Query = ''
    SQL_Query = '\n'.join(SQL_Array)
    
    cursor = cnx.cursor()
    
    #查詢的結果轉 DataFrame
    df = pd.read_sql(SQL_Query, con=cnx)
        
    return df


# In[4]:


# INLine_L 查詢溫度

def inlineL_data_SQL(cnx):
    SQL_Array = ["SELECT TO_CHAR(T.REPORT_TIME, 'yyyymmddhh24mi') || T.LINE_ID || T.TOOL_ID ID,"
    ,"       ITEM_VALUE026 CP_L_TEMP_R,"
    ,"       ITEM_VALUE027 CP_R_TEMP_R,"
    ,"       ITEM_VALUE028 STAGE_L_TEMP1_R,"
    ,"       ITEM_VALUE029 STAGE_L_TEMP2_R,"
    ,"       ITEM_VALUE030 STAGE_R_TEMP1_R,"
    ,"       ITEM_VALUE031 STAGE_R_TEMP2_R"
    ,""
    ,"  FROM CFSPCH.H_DC_KPC T"
    ," WHERE 1 = 1"
    #,"   AND T.REPORT_TIME >= SYSDATE - 24/24"
    ,"   AND T.TOOL_ID IN ('FFRALN10', 'FFGALN10', 'FFBALN10', 'FFSALN10')"
    ,"   AND T.MES_ID = 'INLINE_L'"
    ,"   AND T.DC_ITEM_GROUP = '0000'"
    ," GROUP BY TO_CHAR(T.REPORT_TIME, 'yyyymmddhh24mi'),"
    ,"          T.LINE_ID,"
    ,"          T.TOOL_ID,"
    ,"          ITEM_VALUE026,"
    ,"          ITEM_VALUE027,"
    ,"          ITEM_VALUE028,"
    ,"          ITEM_VALUE029,"
    ,"          ITEM_VALUE030,"
    ,"          ITEM_VALUE031"]

    #查詢 SQL
    SQL_Query = ''
    for s in SQL_Array:
        SQL_Query = SQL_Query + s
    
    cursor = cnx.cursor()
    
    #查詢的結果轉 DataFrame
    df = pd.read_sql(SQL_Query, con=cnx)
        
    return df


# In[5]:


#C8A Oracle 連線
cnx = cx_Oracle.connect('L8AINT_AP', 'L8AINT$AP', 'tcpp104:1551/C8AHSHA')

if df_OL_GLS_Info.shape[0] > 0:
    
    sheet_str = ''
    sheet_List = df_OL_GLS_Info['SHEET_ID']
    for s in sheet_List:
        sheet_str = sheet_str + "'" + s + "',"
    
    sheetlist = sheet_str[:-1]

df_Offset = offset_data_SQL(cnx,sheetlist)
df_inlineL = inlineL_data_SQL(cnx)


#關閉資料庫
cnx.close()


# In[ ]:


df = pd.merge(df_Offset, df_inlineL, how='inner', on=['ID']) 


# In[ ]:


df.head()


# In[9]:


def Check_CVD_Info(cnx,sheetlist):
    SQL_Array = ["SELECT T.LOGOFF_TIME, T.EQP_ID, T.OP_ID, T.CHAMBER_ID_LIST, T.Sheet_Id_Chip_Id SHEET_ID"
                ,"  FROM ARYODS.H_SHEET_OPER_ODS T"
                ," WHERE 1 = 1"
                ,"   AND T.OP_ID = 'AS-CVD'"
                ,"   AND T.SHEET_ID_CHIP_ID IN ("+sheetlist+")"]

    #查詢 SQL
    SQL_Query = ''
    SQL_Query = '\n'.join(SQL_Array)
    
    cursor = cnx.cursor()
    
    #查詢的結果轉 DataFrame
    df = pd.read_sql(SQL_Query, con=cnx)
    
    return df


# In[10]:


if df[df['CHECK_GLASS']=='ARRAY_GLASS'].shape[0] > 0:
    
    sheet_str = ''
    sheet_List = df['SHEET_ID']
    for s in sheet_List:
        sheet_str = sheet_str + "'" + s + "',"
    
    sheetlist = sheet_str[:-1]
    
    #L8A Oracle 連線
    cnx = cx_Oracle.connect('L8AINT_AP', 'L8AINT$AP', 'tcpp104:1523/L8AH')
    
    #查詢 Array CVD 資訊
    df_cvdinfo = Check_CVD_Info(cnx,sheetlist)
    
    #關閉資料庫
    cnx.close()
    
    # 合併 CVD info Data
    df_tmp = pd.merge(df, df_cvdinfo, how='left', on=['SHEET_ID'])     
    
else:
    print('無 Array 來料，無須比對 Array CVD 機台')


# In[11]:


sheet_str = ''
sheet_List = df['SHEET_ID']
for s in sheet_List:
    sheet_str = sheet_str + "'" + s + "',"

sheetlist = sheet_str[:-1]

#L8A Oracle 連線
cnx = cx_Oracle.connect('L8AINT_AP', 'L8AINT$AP', 'tcpp104:1523/L8AH')

#查詢 Array CVD 資訊
df_cvdinfo = Check_CVD_Info(cnx,sheetlist)

#關閉資料庫
cnx.close()


# In[12]:


df_cvdinfo.head()


# In[13]:


df_tmp = pd.merge(df, df_cvdinfo, how='left', on=['SHEET_ID']) 
df_tmp.head()


# In[14]:


#組合成 Key
df_tmp['Key'] = df_tmp['LAYER'] + '_' + df_tmp['SHEET_ID']
df_OL_GLS_Info['Key'] = df_OL_GLS_Info['MEA_OP_ID'] + '_' + df_OL_GLS_Info['SHEET_ID']
df_OL_GLS_Info.drop(columns=['SHEET_ID'],inplace=True)
df = pd.merge(df_OL_GLS_Info, df_tmp, how='left', on=['Key']) 
df.head()


# In[15]:


df = pd.merge(df_OL_GLS_Info, df_tmp, how='left', on=['Key']) 


# In[16]:


#MySQL 連線資訊
cnx = mysql.connector.connect(user='l8ai2_ap', password='l8ai2$ap', host='TW100039807', database='r2r', charset="utf8")
cursor = cnx.cursor()

df = df[~df['SHEET_ID'].isna()].reset_index()
df = df.fillna('')
for r in range(len(df)):
    #print(df['SHEET_ID'][r])
    
    SQL_str = ""
    SQL_str = SQL_str + " Replace Into array_olamp(aoi_meas_time, chart_id, sheet_id, meas_tool, process_tool, BM_EQP, unit, eqp_process_time, actual_recipe, product_code, Recipe_MaskName, layer, Start_Time, end_time, PSA, X, Y, CP_R_TEMP_R, STAGE_R_TEMP1_R, STAGE_R_TEMP2_R, CP_L_TEMP_R, STAGE_L_TEMP1_R, STAGE_L_TEMP2_R, cvd_logofftime, cvd_eqp, cvd_op, cvd_chb, AlignOffset_X_S1, AlignOffset_Y_S1, AlignOffset_Z_S1, AlignOffset_X_S2, AlignOffset_Y_S2, AlignOffset_Z_S2, AlignOffset_X_S3, AlignOffset_Y_S3, AlignOffset_Z_S3, AlignOffset_X_S4, AlignOffset_Y_S4, AlignOffset_Z_S4, AlignOffset_X_S5, AlignOffset_Y_S5, AlignOffset_Z_S5, AlignOffset_X_S6, AlignOffset_Y_S6, AlignOffset_Z_S6, Update_Time ) "
    SQL_str = SQL_str + " VALUE ("
    SQL_str = SQL_str + " '"+str(df['AOI_MEAS_TIME'][r])+"', "
    SQL_str = SQL_str + " '" "', "
    SQL_str = SQL_str + " '"+df['SHEET_ID'][r]+"', "
    SQL_str = SQL_str + " '"+df['MEAS_TOOL'][r]+"', "
    SQL_str = SQL_str + " '"+df['TOOL_ID'][r]+"', "
    SQL_str = SQL_str + " '"+df['BM_EQP'][r]+"', "
    SQL_str = SQL_str + " '"+df['UNIT'][r]+"', "
    SQL_str = SQL_str + " '"+str(df['REPORT_TIME'][r])+"', "
    SQL_str = SQL_str + " '"+df['ACTUAL_RECIPE'][r]+"', "
    SQL_str = SQL_str + " '"+df['PRODUCT_CODE'][r]+"', "
    SQL_str = SQL_str + " '"+df['RECIPE_MASKNAME'][r]+"', "
    SQL_str = SQL_str + " '"+df['LAYER'][r]+"', "
    SQL_str = SQL_str + " '"+df['START_TIME'][r]+"', "
    SQL_str = SQL_str + " '"+df['END_TIME'][r]+"', "
    SQL_str = SQL_str + " '"+df['CHECK_GLASS'][r]+"', "
    SQL_str = SQL_str + " '"+df['X'][r]+"', "
    SQL_str = SQL_str + " '"+df['Y'][r]+"', "
    SQL_str = SQL_str + " '"+df['CP_R_TEMP_R'][r]+"', "
    SQL_str = SQL_str + " '"+df['STAGE_R_TEMP1_R'][r]+"', "
    SQL_str = SQL_str + " '"+df['STAGE_R_TEMP2_R'][r]+"', "
    SQL_str = SQL_str + " '"+df['CP_L_TEMP_R'][r]+"', "
    SQL_str = SQL_str + " '"+df['STAGE_L_TEMP1_R'][r]+"', "
    SQL_str = SQL_str + " '"+df['STAGE_L_TEMP2_R'][r]+"', "
    if df['LOGOFF_TIME'][r] == '':
        SQL_str = SQL_str + " null, "
    else:
        SQL_str = SQL_str + " '"+df['LOGOFF_TIME'][r]+"', "
    SQL_str = SQL_str + " '"+df['EQP_ID'][r]+"', "
    SQL_str = SQL_str + " '"+df['OP_ID'][r]+"', "
    SQL_str = SQL_str + " '"+df['CHAMBER_ID_LIST'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_X_S1'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Y_S1'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Z_S1'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_X_S2'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Y_S2'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Z_S2'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_X_S3'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Y_S3'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Z_S3'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_X_S4'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Y_S4'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Z_S4'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_X_S5'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Y_S5'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Z_S5'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_X_S6'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Y_S6'][r]+"', "
    SQL_str = SQL_str + " '"+df['ALIGNOFFSET_Z_S6'][r]+"', "
    SQL_str = SQL_str + " now() ); "
    
    #print(SQL_str)
    
    cursor.execute(SQL_str)
    cnx.commit()

print('寫入完成！')

#關閉資料庫連線
cnx.close()


# In[ ]:




