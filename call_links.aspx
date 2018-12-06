<%@ Page Language="C#" AutoEventWireup="true" CodeFile="call_links.aspx.cs" Inherits="call_links" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <title>ARC IVR Agent Call Links</title>
    <meta name="robots" content="noindex, nofollow" />
    <style type="text/css">
    </style>
    <link rel="shortcut icon" href="favicon.ico" />
    <link href="css/script.css?v=<%=scriptVersion %>" rel="stylesheet" type="text/css" />
    <link href="css/content_general.css?v=<%=scriptVersion %>"" rel="stylesheet" type="text/css" />
    <link href="css/user.css?v=<%=scriptVersion %>"" rel="stylesheet" type="text/css" />
    <link href="css/user_list.css?v=<%=scriptVersion %>"" rel="stylesheet" type="text/css" />
    <link href="css/portal_standard.css?v=<%=scriptVersion %>"" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .Portal_GridView_Standard td
        {
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="header" class="clearfix">
            <div style="float: left;margin-top: 5px;">
                <img src="../../images/GnH-Logo-CMYK.png" alt="Greenwood & Hall" width="300px" height="63px" />
            </div>
            <div style="float: right;">
                <img src="../../images/arc2.JPG" alt="American Red Cross" width="299px" height="105px" />
            </div>
        </div>
        <div id="header_info" style="width: 900px;background-color: <%=scriptColor %>;">
            <div style="position: relative;" class="red">
                <div style="position: absolute; top: 0px; right: 0px;">
                    <asp:Label ID="lblMode" runat="server" Text="" />
                    <asp:Label ID="lblJRE" runat="server" Text="" />
                </div>
            </div>
            <div>
                Agent Call Links
            </div>
        </div>
        <div id="container" style="vertical-align: top;margin-top: 5px;">
            <asp:Panel ID="pnlCallInfo" runat="server" Visible="false">
                Open Call Links
                <div>
                    <asp:GridView ID="gvSearchResults" runat="server" AutoGenerateColumns="False" CssClass="Portal_GridView_Standard" Width="900" Font-Size="14px">
                        <AlternatingRowStyle CssClass="Portal_GridView_Standard_Alternate" />
                        <Columns>
                            <asp:BoundField HeaderText="Agent" DataField="agent_fullname" />
                            <asp:BoundField HeaderText="Campaign" DataField="five9_campaign" />
                            <asp:TemplateField HeaderText="Started">
                                <ItemTemplate>
                                    <asp:Label ID="call_createdate" runat="server" Text='<%# ghFunctions.date_label(Eval("createdate").ToString()) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="DNIS">
                                <ItemTemplate>
                                    <asp:Label ID="call_dnis" runat="server" Text='<%# get_Script_DNIS(Eval("destinator").ToString()) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="ANI" DataField="originator" />
                                    <asp:TemplateField HeaderText="Age">
                                        <ItemTemplate>
                                            <asp:Label ID="age" runat="server" Text='<%# ghFunctions.SecondsTo(Eval("age").ToString()) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                            <asp:TemplateField HeaderText="Call Link" ItemStyle-CssClass="LastCol">
                                <ItemTemplate>
                                    <div style="text-align: center;">
                                        <asp:HyperLink runat="server"
                                            Text='<%#string.Format("{0}",Eval("arc_callid"), Eval("five9_callid"),Eval("interactionid")) %>'
                                            Target='_parent'
                                            NavigateUrl='<%# string.Format(get_Script_URL() + "?ghsource=_AgentPOP&agent.station_id={0}&agent.first_agent={1}&agent.id={2}&agent.full_name={3}&agent.station_type={4}&agent.user_name={5}&call.ani={6}&call.bill_time={7}&call.call_id={8}&call.campaign_id={9}&call.campaign_name={10}&call.comments={11}&call.disposition_id={12}&call.disposition_name={13}&call.dnis={14}&call.end_timestamp={15}&call.handle_time={16}&call.hold_time={17}&call.length={18}&call.mediatype={19}&call.number={20}&call.park_time={21}&call.queue_time={22}&call.session_id={23}&call.skill_id={24}&call.skill_name={25}&call.start_timestamp={26}&call.tcpa_date_of_consent={27}&call.type_name={28}&call.type={29}&call.wrapup_time={30}",
                                            (Eval("agent_stationid").ToString())
                                            ,(Eval("agent_five9id").ToString())
                                            ,(Eval("agent_five9id").ToString())
                                            ,(Eval("agent_fullname").ToString())
                                            ,(Eval("agent_stationtype").ToString())
                                            ,(Eval("agent_username").ToString())
                                            ,(Eval("originator").ToString())
                                            ,(Eval("bill_time").ToString())
                                            ,(Eval("five9_callid").ToString())
                                            ,(Eval("five9_campaignid").ToString())
                                            ,(Eval("five9_campaign").ToString())
                                            ,(Eval("comments").ToString())
                                            ,(Eval("five9_dispositionid").ToString())
                                            ,(Eval("five9_disposition").ToString())
                                            ,(get_URL_DNIS(Eval("destinator").ToString()))                                                    
                                            ,(Eval("dateend").ToString())
                                            ,(Eval("handle_time").ToString())
                                            ,(Eval("hold_time").ToString())
                                            ,(Eval("length").ToString())
                                            ,(Eval("five9_mediatype").ToString())
                                            ,(Eval("originator").ToString())
                                            ,(Eval("park_time").ToString())
                                            ,(Eval("queue_time").ToString())
                                            ,(Eval("sessionid").ToString())
                                            ,(Eval("five9_skillid").ToString())
                                            ,(Eval("five9_skill").ToString())
                                            ,(Eval("datestart").ToString())
                                            ,(Eval("tcpa_date_of_consent").ToString())
                                            ,(Eval("five9_type").ToString())
                                            ,(Eval("five9_typeid").ToString())
                                            ,(Eval("wrapup_time").ToString())
                                                )
                                            %>'
                                            />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Records For Selected Filters
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
                <div>
                    No Open Calls
                    <br /><a href="script_main.aspx" style="text-decoration: none;">Click here for blank Agent Script</a>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlInvalid" runat="server" Visible="false">
                The page request is invalid.
            </asp:Panel>
        </div>
    </form>
</body>
</html>
