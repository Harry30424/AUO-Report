<%@ Page Language="C#" AutoEventWireup="true" Debug="true" CodeFile="OL_main.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="web" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>
<%@ Register Assembly="netchartdir" Namespace="ChartDirector" TagPrefix="chart" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>

    <style type="text/css">
        PagerCss TD A:hover { WIDTH: 20px; COLOR: black; padding-left: 4px; padding-right:4px; }
        .PagerCss TD A:active { WIDTH: 20px; COLOR: black; padding-left: 4px; padding-right:4px; }
        .PagerCss TD A:link { WIDTH: 20px; COLOR: black; padding-left: 4px; padding-right:4px; }
        .PagerCss TD A:visited { WIDTH: 20px; COLOR: black;padding-left: 4px; padding-right:4px; }
        .PagerCss TD SPAN { FONT-WEIGHT: bold; FONT-SIZE: 20px; WIDTH: 20px; COLOR: red; padding-left: 4px; padding-right:4px;} 

    .CalendarCSS 
        {
            background-color:white;
            color:Snow;
            border:solid 1px Black;
            }
        .ajax__calendar_days
        {
            background-color:white;
            color:Black;
            }
        .ajax__calendar_day
        {
            background-color:white;
            font-style:italic;
            font-family:Black;
            }
        .ajax__calendar_header
        {
            background-color:gray;
            
            }     

        .D1{
            width:724px;
            height:137px;
            background-image:url(http://tw100043343.corpnet.auo.com/L7B/back_ground/l7b01_title2.png);
            background-repeat:no-repeat;
            background-size:contain;
}
    </style>

</head>

<body>
    <form id="form1" runat="server">
        
        
    <center>
        <br />
        <Table ID="Table3" runat="server" border="0" cellpadding="1" 
            cellspacing="2"   class="D1" width="724px" height ="137px" >       
                <tr>
                <td width="15%">

                </td>
                <td colspan ="2" aligb="left">
                            <asp:Label ID="L_title" runat="server" Font-Size="24pt" Font-Names="Arial" Font-Bold="True" >OL Mapping Query</asp:Label>

                </td>
                    </tr>
                
            </>
            </Table>

        <Table ID="Table5" runat="server" border="0" cellpadding="1" 
            cellspacing="2"  width="1000px" >
                <tr>
                    <td width ="50%" align="left">
                        <asp:Button ID="TOP_BT1" runat="server" Font-Names="Arial" Height="25px"
                            Text="OL" Width="90px" Font-Size="12"  />
                         <asp:Button ID="TOP_BT2" runat="server" Font-Names="Arial" Height="25px"
                                Text="Tsuno" Width="90px" Font-Size="12" OnClick="TOP_BT2_Click" />
                         <asp:Button ID="TOP_BT3" runat="server" Font-Names="Arial" Height="25px"
                                Text="CD" Width="90px" Font-Size="12" OnClick="TOP_BT3_Click" />
                        <asp:Button ID="TOP_BT5" runat="server" Font-Names="Arial" Height="25px"
                            Text="TTP" Width="90px" Font-Size="12"  OnClick="TOP_BT5_Click"/>
                         <asp:Button ID="TOP_BT4" runat="server" Font-Names="Arial" Height="25px"
                                Text="History" Width="90px" Font-Size="12" OnClick="TOP_BT4_Click"/>
                    </td>
                    <td width="50%" align="right">
                        <asp:Label ID="lb_view1" runat="server" Font-Names="Arial" Font-Size="10pt" ForeColor="Blue">今日瀏覽人次:</asp:Label>
                    <asp:Label ID="lb_view_count" runat="server" Font-Names="Arial" Font-Size="10" ForeColor="Blue" Text=""></asp:Label>&nbsp;&nbsp; 
                    <asp:Label ID="lb_view2" runat="server" Font-Names="Arial" Font-Size="10" ForeColor="Blue">昨日瀏覽人次:</asp:Label>
                    <asp:Label ID="lb_view_count2" runat="server" Font-Names="Arial" Font-Size="10" ForeColor="Blue" Text=""></asp:Label>&nbsp;&nbsp; 
                    <asp:Label ID="lb_view3" runat="server" Font-Names="Arial" Font-Size="10" ForeColor="Blue">累計瀏覽人次:</asp:Label>
                    <asp:Label ID="lb_view_count_total" runat="server" Font-Names="Arial" Font-Size="10" ForeColor="Blue" Text=""></asp:Label>&nbsp;&nbsp; 
                    </td>
                </tr>
            </>
            </Table>


        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

       
  
        
        
        <Table ID="Table1" runat="server" border="0" cellpadding="1" 
            cellspacing="2" rules="all"  style="font-size: 15pt" width="1000px">

            <tr style="background-color: #f3f3f3">
                <td align="right" width="18%">
                    <asp:Label ID="Label2" runat="server" Font-Names="標楷體" Text="開始時間："></asp:Label>&nbsp;

                </td>
                				
				 <td align="left" width="20%">
                    
                        <asp:TextBox ID="start_time" runat="server" Width="120px"></asp:TextBox>&nbsp;&nbsp;
                     <ajaxtoolkit:calendarextender ID="Calendarextender1" runat="server" BehaviorID="Calendarextender1" TargetControlID="start_time" format="yyyy-MM-dd" CssClass="CalendarCSS "/>
                </td>
                <td align="right" width="15%">
                    <asp:Label ID="Label3" runat="server" Font-Names="標楷體" Text="結束時間："></asp:Label>&nbsp;
                   </td>

                <td align="left" width="15%">
                    
                    <asp:TextBox ID="end_time" runat="server" Width="120px"></asp:TextBox>&nbsp;&nbsp;
                    <ajaxtoolkit:calendarextender ID="Calendarextender3" runat="server" BehaviorID="Calendarextender2" TargetControlID="end_time" format="yyyy-MM-dd" CssClass="CalendarCSS"/>

                </td>     
                <td width="10%" align="right">
                    <asp:Label ID="Label7" runat="server"  Font-Names="Arial" Text="Layer："></asp:Label>&nbsp;
                    </td>

                <td width="22%" align="left">
                    <asp:RadioButtonList ID="RB_Layer" runat="server"  RepeatDirection="Horizontal" Visible="true" Font-Names="Arial" style="text-align: left">
                    <asp:ListItem Value="ALL" Selected="True"></asp:ListItem>
                    <asp:ListItem Value="R"></asp:ListItem>
					<asp:ListItem Value="G"></asp:ListItem>
                    <asp:ListItem Value="B"></asp:ListItem>
                    </asp:RadioButtonList>

                </td>  
            </tr>
            <tr align="left" style="background-color: #f3f3f3">
                <td align="right" width="18%">
                    <asp:Label ID="Label12" runat="server" Font-Names="Arial">Measure Tool：</asp:Label>&nbsp;
                    
                </td>
                <td align="left" width="20%">
                    <asp:CheckBox ID="CB_AOI10" runat="server"  Text="FDAAOI10" Font-Names="Arial" />&nbsp;<br />
                    <asp:CheckBox ID="CB_TTP10" runat="server"  Text="FDATTP10" Font-Names="Arial" />&nbsp;

                    
                    
                </td>
                <td align="right" width="15%">
                    <asp:Label ID="Label5" runat="server" Font-Names="Arial">Expo Tool：</asp:Label>&nbsp;
                </td>
                <td align="left" width="15%">
                    <asp:DropDownList ID="list_expo_tool" runat="server" Width="125px"></asp:DropDownList>  
                    
                </td>

				 <td align="right" width="10%">
                    <asp:Label ID="Label13" runat="server" Font-Names="Arial">Chart：</asp:Label>&nbsp;
                </td>
                <td align="left" width="22%">
                    <asp:DropDownList ID="list_model" runat="server" Width="200px"></asp:DropDownList>  
                    
                </td>
            </tr>
            <tr align="left" style="background-color: #f3f3f3">
                <td align="right" width="18%">
                    <asp:Label ID="Label8" runat="server" Font-Names="Arial">Sheet ID：</asp:Label>&nbsp;                    
                </td>
                <td width="15%">
                    <asp:TextBox ID="tb_sheet_id" runat="server" Width="120px"></asp:TextBox>&nbsp;&nbsp;
                </td>

                <td align="center" colspan ="2">
                   <asp:RadioButtonList ID="rb_time" runat="server"  RepeatDirection="Horizontal" Visible="true" Font-Names="Arial" style="text-align: left" CellPadding="5">
                    <asp:ListItem Value="Meas Time" Selected="True"></asp:ListItem>
					<asp:ListItem Value="Process Time"></asp:ListItem>                  
                    </asp:RadioButtonList>    
                </td>

                <td align="right" colspan ="2">
                    <asp:Button ID="BT_select" runat="server" Font-Names="標楷體" Height="30px"
                            Text="查詢" Width="120px" Font-Size="15" BackColor="#B9FFFF" OnClick="BT_select_Click" />&nbsp;
                </td>
            </tr>
            <tr align="right">
                <td colspan="2" align="right">
                    <asp:CheckBox ID="CB_ol" runat="server"  Text="多片重疊" Font-Names="標楷體" />
                    <asp:CheckBox ID="CB_avg" runat="server"  Text="平均疊圖" Font-Names="標楷體" />
                    <asp:Button ID="Button1" runat="server" Font-Names="標楷體" Height="30px"
                            Text="查看圖表" Width="120px" Font-Size="15" BackColor="#FFD9E8" OnClick="Button1_Click" />
                   
                    

                </td>
                <td colspan="4" align="left">
                    <asp:Button ID="Button3" runat="server" Font-Names="標楷體" Height="30px"
                            Text="單片補值" Width="120px" Font-Size="15" BackColor="#B9FFFF" OnClick="Button3_Click" />
             
                </td>
            </tr>
            
            <tr>
                <td colspan="2" align="right">
                    <asp:Label ID="Label6" runat="server" Font-Size="12pt" Font-Names="標楷體" >最多12片</asp:Label>
                </td>
                <td colspan ="4" align="right">
                    <asp:Label ID="Label4" runat="server" Font-Names="Arial" Text="This web is maintained by 林暐峰 8606-1831/余昀澔 8606-2863" Font-Size="11"></asp:Label>
                </td>

            </tr>
           <tr align="center">
                <td colspan="6" >
                    
                    
                    
                    
                    <br />
                    
                </td>
               
            </tr>

            </Table>
   
        <asp:Label ID="lb_error" runat="server" Font-Names="Arial" ForeColor="Red"></asp:Label>&nbsp;
        
         <asp:Panel ID="panel_fillvalue" runat="server" Width ="1600" visible="false">

             <asp:Label ID="lb_fill_x1" runat="server" Font-Names="Arial" Text="" Visible="false"></asp:Label>
             <asp:Label ID="lb_fill_y1" runat="server" Font-Names="Arial" Text="" Visible="false"></asp:Label>
             <asp:Label ID="lb_fill_x2" runat="server" Font-Names="Arial" Text="" Visible="false"></asp:Label>
             <asp:Label ID="lb_fill_y2" runat="server" Font-Names="Arial" Text="" Visible="false"></asp:Label>
             <asp:Label ID="lb_fill_offset_x" runat="server" Font-Names="Arial" Text="" Visible="false"></asp:Label>
             <asp:Label ID="lb_fill_offset_y" runat="server" Font-Names="Arial" Text="" Visible="false"></asp:Label>
             <asp:Label ID="lb_fill_stitch" runat="server" Font-Names="Arial" Text="" Visible="false"></asp:Label>
             <asp:Label ID="lb_fill_stitch_point" runat="server" Font-Names="Arial" Text="" Visible="false"></asp:Label>
             <asp:Label ID="lb_fill_pointcount" runat="server" Font-Names="Arial" Text="" Visible="false"></asp:Label>

             <Table ID="Table6" runat="server" border="0" cellpadding="2" 
            cellspacing="2" rules="all"  width="1600px" >
                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image_fill_value_1" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img_fill_1" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label>
                        <asp:Label ID="lb_img_fill_1_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image_fill_1" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer_fill1" runat="server" />
                    </td>
                </tr>
                 <tr>
                     <td colspan="3" style="background-color:#316293;">
                         <asp:Label ID="Label9" runat="server" Font-Names="Arial" Text="補值專區" Font-Size="16pt" Font-Bold="True" ForeColor="White"></asp:Label>
                     </td>
                 </tr>
                 <tr>
                     <td>

                     </td>
                     <td align="center">
                         <br />
                         <Table ID="Table7" runat="server" border="1" cellpadding="2" 
                            cellspacing="2" rules="all"  style="font-size: 12pt" width="410px" >
                             <tr>
                                 <td>
                                     &nbsp;
                                 </td>
                                 <td>
                                     <asp:Label ID="Label15" runat="server" Font-Names="Arial" Text="預補_X" Font-Bold="True"></asp:Label>
                                 </td>
                                 <td>
                                     <asp:Label ID="Label16" runat="server" Font-Names="Arial" Text="預補_Y" Font-Bold="True"></asp:Label>
                                 </td>
                                 <td>
                                     <asp:Label ID="Label17" runat="server" Font-Names="Arial" Text="預補_θ" Font-Bold="True"></asp:Label>
                                 </td>
                             </tr>
                             <tr>
                                 <td>
                                     <asp:Label ID="Label18" runat="server" Font-Names="Arial" Text="Shot_1" Font-Bold="True"></asp:Label>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_x_1" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_y_1" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_c_1" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                             </tr>
                             <tr>
                                 <td>
                                     <asp:Label ID="Label19" runat="server" Font-Names="Arial" Text="Shot_2" Font-Bold="True"></asp:Label>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_x_2" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_y_2" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_c_2" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                             </tr>
                             <tr>
                                 <td>
                                     <asp:Label ID="Label20" runat="server" Font-Names="Arial" Text="Shot_3" Font-Bold="True"></asp:Label>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_x_3" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_y_3" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_c_3" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                             </tr>
                             <tr>
                                 <td>
                                     <asp:Label ID="Label21" runat="server" Font-Names="Arial" Text="Shot_4" Font-Bold="True"></asp:Label>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_x_4" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_y_4" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_c_4" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                             </tr>
                             <tr>
                                 <td>
                                     <asp:Label ID="Label22" runat="server" Font-Names="Arial" Text="預補溫度" Font-Bold="True"></asp:Label>
                                 </td>
                                 <td>
                                     <asp:TextBox ID="tb_fill_t" runat="server" Text="0" width="50px" style="text-align:center"></asp:TextBox>
                                 </td>
                                 <td colspan="2">
                                    <asp:Button ID="Button2" runat="server" Text="補值" OnClick="Button2_Click"></asp:Button>
                                 </td>
   
                             </tr>

                             </Table><br /><br />
                     </td>
                     <td>

                     </td>
                 </tr>
                  <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image_fill_value_2" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img_fill_2" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label>
                        <asp:Label ID="lb_img_fill_2_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image_fill_2" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer_fill2" runat="server" />
                    </td>
                </tr>
                 </Table>
             </asp:Panel>


        <asp:Panel ID="panel_ol" runat="server" Width ="1500" visible="false">
            <asp:Image ID="Image_ol" runat="server" />
            <br /><br /><br />
        </asp:Panel>

        <asp:Panel ID="panel_avg" runat="server" Width ="1500" visible="false">
            <Table ID="Table4" runat="server" border="0" cellpadding="2" 
            cellspacing="2" rules="all"  style="font-size: 15pt" width="1500px" >
                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image_value_avg" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                         <asp:Label ID="Label1" runat="server" Font-Names="標楷體" Font-Size="20pt" Text="多片對應點位平均圖" BackColor="#00CC00" ForeColor="White">
                         </asp:Label><br /><br />
                        <asp:Image ID="Image_avg" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer_avg" runat="server" />
                    </td>
                </tr>
            </Table>
        </asp:Panel>

        <asp:Panel ID="panel_imgall" runat="server" Width ="1500" visible="false">
            <Table ID="Table2" runat="server" border="0" cellpadding="2" 
            cellspacing="2" rules="all"  style="font-size: 15pt" width="1500px" >
                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image1_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img1" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label>
                        <asp:Label ID="lb_img1_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image1" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer1" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image2_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img2" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label>
                        <asp:Label ID="lb_img2_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image2" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer2" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image3_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img3" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label>
                        <asp:Label ID="lb_img3_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image3" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer3" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image4_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img4" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label>
                        <asp:Label ID="lb_img4_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image4" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer4" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image5_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img5" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label>
                        <asp:Label ID="lb_img5_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image5" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer5" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image6_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img6" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label>
                        <asp:Label ID="lb_img6_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image6" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer6" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image7_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img7" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label>
                        <asp:Label ID="lb_img7_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image7" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer7" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image8_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img8" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label><br />
                        <asp:Label ID="lb_img8_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image8" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer8" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image9_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img9" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label><br />
                        <asp:Label ID="lb_img9_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image9" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer9" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image10_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img10" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label><br />
                        <asp:Label ID="lb_img10_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image10" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer10" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image11_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img11" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label><br />
                        <asp:Label ID="lb_img11_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image11" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer11" runat="server" />
                    </td>
                </tr>

                <tr>
                    <td width ="33%" align ="center">
                        <br /><br />
                        <asp:Image ID="Image12_value" runat="server" />
                    </td>
                    <td width ="33%" align ="center">
                        <asp:Label ID="lb_img12" runat="server" Font-Names="Arial" Text="" Font-Bold="True"></asp:Label><br />
                        <asp:Label ID="lb_img12_1" runat="server" Font-Names="Arial" Text="" Font-Size="11pt" ForeColor="#333333"></asp:Label><br />
                        <asp:Image ID="Image12" runat="server" />
                    </td>
                    <td width ="34%" align ="center">
                        <chart:WebChartViewer ID="WebChartViewer12" runat="server" />
                    </td>
                </tr>

            </Table>
        </asp:Panel>
        
        
       
      
        </center>
        

        <asp:GridView ID="GridView1" runat="server" BorderStyle="Inset" onrowdatabound ="GridView1_RowCreated" 
            allowpaging="true" onpageindexchanging="GridView1_PageIndexChanging" pagesize="40"
            BorderWidth="1px"  Font-Size="11pt" 
           ForeColor="#333333" AutoGenerateColumns="False" BorderColor="#333333"  align="center" style="word-break:break-all; word-wrap:normal"  >
            <PagerStyle CssClass="PagerCss"></PagerStyle>
            <Columns >        
	
                <asp:TemplateField HeaderText="" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "White"/>		

                    <HeaderTemplate>
                      <asp:CheckBox ID="cb_checkall" runat="server" Font-Names="Arial" OnCheckedChanged="cb_checkall_CheckedChanged" AutoPostBack="True" />
                    </HeaderTemplate>		

					<ControlStyle Width="40px" />
					<ItemStyle HorizontalAlign="center" ></ItemStyle>					
					<ItemTemplate>  
						<asp:CheckBox ID="cb_record" runat="server" Font-Names="Arial" /><br />
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Measure<br>Time" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "White"/>				
					<ControlStyle Width="70px" />
					<ItemStyle HorizontalAlign="center" ></ItemStyle>					
					<ItemTemplate>  
						<asp:Label ID="g_label1" runat="server" Text='<%# Bind("aoi_meas_time") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Sheet ID" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "White"/>				
					<ControlStyle Width="100px" />
					<ItemStyle HorizontalAlign="center" ></ItemStyle>					
					<ItemTemplate>  
						<asp:Label ID="g_label7" runat="server" Text='<%# Bind("sheet_id") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Meas Tool" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="80px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label2" runat="server" Text='<%# Bind("meas_tool") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                 <asp:TemplateField HeaderText="Tool ID" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="90px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label3" runat="server" Text='<%# Bind("tool_id") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Unit" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="90px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label4" runat="server" Text='<%# Bind("unit") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Chart" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="100px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_chart" runat="server" Text='<%# Bind("chart") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Layer" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="70px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_layer" runat="server" Text='<%# Bind("layer") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>


                <asp:TemplateField HeaderText="Create<br>Time" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="80px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label5" runat="server" Text='<%# Bind("eqp_process_time") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Product<br>Code" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="90px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label6" runat="server" Text='<%# Bind("model") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                

                <asp:TemplateField HeaderText="Actual<br>Recipe" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="50px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label8" runat="server" Text='<%# Bind("actual_recipe") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Recipe<br>Maskname" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="90px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label9" runat="server" Text='<%# Bind("Recipe_MaskName") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>


                <asp:TemplateField HeaderText="Process<br>Time(min)" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="70px" />
					<ItemStyle HorizontalAlign="center" ></ItemStyle>					
					<ItemTemplate>  
						<asp:Label ID="g_label10" runat="server" Text='<%# Bind("diff") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="RLX-RRX<br>(S2)" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="70px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_diff_X_S21" runat="server" Text='<%# Bind("diff_X_S2") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Offset_X" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="120px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_oft_x" runat="server" Text='<%# Bind("oft_x") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Offset_Y" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="120px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_oft_y" runat="server" Text='<%# Bind("oft_y") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Offset_T" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="120px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_oft_t" runat="server" Text='<%# Bind("oft_t") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>


				<asp:TemplateField HeaderText="PSA" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="60px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label11" runat="server" Text='<%# Bind("PSA") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="AS-CVD<br>Time" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="75px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_cvdtime" runat="server" Text='<%# Bind("cvd_time") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="AS-CVD<br>Unit" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="80px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_cvd" runat="server" Text='<%# Bind("cvd") %>'></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>


                <asp:TemplateField HeaderText="OOS" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="100px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_oos" runat="server" Text='<%# Bind("OOS") %>' Font-Size="9pt" ForeColor="Red"></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="OOC" > 
					<HeaderStyle HorizontalAlign="Center"  BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="100px" />  
					<ItemStyle HorizontalAlign="center"></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_ooc" runat="server" Text='<%# Bind("OOC") %>' Font-Size="9pt" ForeColor="#CC6600"></asp:Label>  
					</ItemTemplate>  
				</asp:TemplateField>


                <asp:TemplateField HeaderText="CP_L_<br>TEMP_R" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF" />				
					<ControlStyle Width="70px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label12" runat="server" Text='<%# Bind("CP_L_TEMP_R") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="STAGE_L_<br>TEMP1_R" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="80px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label13" runat="server" Text='<%# Bind("STAGE_L_TEMP1_R") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="STAGE_L_<br>TEMP2_R" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="80px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label14" runat="server" Text='<%# Bind("STAGE_L_TEMP2_R") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="CP_R_<br>TEMP_R" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="70px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label15" runat="server" Text='<%# Bind("CP_R_TEMP_R") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="STAGE_R_<br>TEMP1_R" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="80px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label16" runat="server" Text='<%# Bind("STAGE_R_TEMP1_R") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="STAGE_R_<br>TEMP2_R" > 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="80px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_label17" runat="server" Text='<%# Bind("STAGE_R_TEMP2_R") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="X" Visible = "False"> 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="300px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_X" runat="server" Text='<%# Bind("X") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="Y" Visible = "False"> 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="300px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_Y" runat="server" Text='<%# Bind("Y") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                

                <asp:TemplateField HeaderText="point_count" Visible = "False"> 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="50px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_point_count" runat="server" Text='<%# Bind("point_count") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="shot_point" Visible = "False"> 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="300px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_shot_point" runat="server" Text='<%# Bind("shot_point") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>
                <asp:TemplateField HeaderText="map_point" Visible = "False"> 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="50px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_map_point" runat="server" Text='<%# Bind("map_point") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="x_offset" Visible = "False"> 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="300px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_x_offset" runat="server" Text='<%# Bind("x_offset") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>
                <asp:TemplateField HeaderText="y_offset" Visible = "False"> 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="50px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_y_offset" runat="server" Text='<%# Bind("y_offset") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="stiching_point" Visible = "False"> 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="50px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_stiching_point" runat="server" Text='<%# Bind("stiching_point") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>

                <asp:TemplateField HeaderText="shot_count" Visible = "False"> 
					<HeaderStyle HorizontalAlign="Center" BackColor="#316293" Font-Bold="True" ForeColor= "#FFFFFF"/>				
					<ControlStyle Width="50px" />  
					<ItemStyle HorizontalAlign="center" ></ItemStyle>	
					<ItemTemplate>  
						<asp:Label ID="g_shot_count" runat="server" Text='<%# Bind("shot_count") %>' ></asp:Label>
					</ItemTemplate>  
				</asp:TemplateField>
                
				
        </Columns>
        </asp:GridView>
        <asp:GridView ID="GridView2" runat="server"></asp:GridView>



        </form>
</body>
</html>
