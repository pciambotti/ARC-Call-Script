<%@ Page Title="Sent Emails" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Emails.aspx.cs" Inherits="Emails" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function onLoad() {
            $(".datepicker-add").datepicker({
                dateFormat: "mm-dd-yy"
                , changeMonth: true
                , changeYear: true
                , showOtherMonths: true
                , selectOtherMonths: true
                , numberOfMonths: 2
                , showButtonPanel: true
                , maxDate: "+0D"
                , yearRange: "-200:+0"
            });
            $("#searchclear").click(function () {
                $(".datepicker-add").val('');
            });
        }
        function dobleScroll() {
            var element = document.getElementById('divGrid');
            if (element == null) {
                setTimeout('dobleScroll();', 1000);
            }
            else {
                var scrollbar = document.createElement('div');
                scrollbar.appendChild(document.createElement('div'));
                scrollbar.style.overflow = 'auto';
                scrollbar.style.overflowY = 'hidden';
                scrollbar.style.width = element.clientWidth + 'px';
                scrollbar.firstChild.style.width = element.scrollWidth + 'px';
                scrollbar.firstChild.style.paddingTop = '1px';
                scrollbar.firstChild.appendChild(document.createTextNode('\xA0'));
                scrollbar.onscroll = function () {
                    element.scrollLeft = scrollbar.scrollLeft;
                };
                element.onscroll = function () {
                    scrollbar.scrollLeft = element.scrollLeft;
                };
                element.parentNode.insertBefore(scrollbar, element);
            }
        }
    </script>
    <div class="jumbotron">
        <h1>Agent Emails</h1>
        <div style="float: left;">
            List of agent sent emails
        </div>
    </div>
    <div>
        <asp:UpdatePanel ID="upStats" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnStatsHide" />
            </Triggers>
            <ContentTemplate>
                <div class="panel panel-default">
                    <div class="panel-heading" style="position: relative;">
                        <strong>Stats</strong><asp:Literal runat="server" ID="litStats" />
                        <div style="position: absolute;right: 1px;top: 4px;">
                            <asp:Button runat="server" ID="btnStatsRefresh" Text="Refresh Stats" OnClick="Stats_Refresh" CssClass="btn btn-primary cancel" />
                            <asp:Button runat="server" ID="btnStatsHide" Text="Hide Stats" OnClick="Stats_Hide" CssClass="btn btn-primary cancel" />
                            <asp:Button runat="server" ID="btnStatsDisable" Text="Disable Stats" OnClick="Stats_Disable" CssClass="btn btn-primary cancel" />
                        </div>
                    </div>
                    <div class="panel-body" runat="server" id="pnlStats">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        Today <%: Custom.dateShort(DateTime.UtcNow.ToString()) %>
                                    </div>
                                    <div class="panel-body panel-nomargin">
                                        <asp:GridView ID="gvStatsEmailsToday" runat="server" AutoGenerateColumns="false" GridLines="None"
                                            CssClass="table table-hover table-striped table-bordered table-condensed"
                                            UseAccessibleHeader="true"
                                            DataKeyNames="callcenter"
                                            OnRowCommand="GridView_Grid_RowCommand"
                                            OnRowDataBound="GridView_Grid_RowDataBound"
                                            ShowFooter="true"
                                            >
                                            <Columns>
                                                <asp:TemplateField HeaderText="Center" ItemStyle-Width="55px" >
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="callcenter" Text='<%# Eval("callcenter") %>' />
                                                        <asp:HiddenField runat="server" ID="cnt" Value='<%# Eval("count").ToString() %>' />
                                                        <asp:HiddenField runat="server" ID="cntbounce" Value='<%# Eval("bounced").ToString() %>' />
                                                        <asp:HiddenField runat="server" ID="cntunsub" Value='<%# Eval("unsubscribe").ToString() %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="count" HeaderText="Emails" ItemStyle-Width="55px" />
                                                <asp:TemplateField HeaderText="Bounced" ItemStyle-Width="80px" >
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="bounced" Text='<%# Stats_Get_Percent(Eval("count").ToString(), Eval("bounced").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Unsub" ItemStyle-Width="65px">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="unsubscribe" Text='<%# Stats_Get_Percent(Eval("count").ToString(), Eval("unsubscribe").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Last Sent">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="datecreated" Text='<%# Custom.datetimeShort(Eval("last").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle Font-Bold="true" />
                                            <EmptyDataTemplate>
                                                No Records For Today
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        Month <%: DateTime.UtcNow.ToString("MMMM") %>
                                    </div>
                                    <div class="panel-body panel-nomargin">
                                        <asp:GridView ID="gvStatsEmailsMonth" runat="server" AutoGenerateColumns="false" GridLines="None"
                                            CssClass="table table-hover table-striped table-bordered table-condensed"
                                            UseAccessibleHeader="true"
                                            DataKeyNames="callcenter"
                                            OnRowCommand="GridView_Grid_RowCommand"
                                            OnRowDataBound="GridView_Grid_RowDataBound"
                                            ShowFooter="true"
                                            >
                                            <Columns>
                                                <asp:TemplateField HeaderText="Center" ItemStyle-Width="55px" >
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="callcenter" Text='<%# Eval("callcenter") %>' />
                                                        <asp:HiddenField runat="server" ID="cnt" Value='<%# Eval("count").ToString() %>' />
                                                        <asp:HiddenField runat="server" ID="cntbounce" Value='<%# Eval("bounced").ToString() %>' />
                                                        <asp:HiddenField runat="server" ID="cntunsub" Value='<%# Eval("unsubscribe").ToString() %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="count" HeaderText="Emails" ItemStyle-Width="55px" />
                                                <asp:TemplateField HeaderText="Bounced" ItemStyle-Width="80px" >
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="bounced" Text='<%# Stats_Get_Percent(Eval("count").ToString(), Eval("bounced").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Unsub" ItemStyle-Width="65px">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="unsubscribe" Text='<%# Stats_Get_Percent(Eval("count").ToString(), Eval("unsubscribe").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Last Sent">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="datecreated" Text='<%# Custom.datetimeShort(Eval("last").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle Font-Bold="true" />
                                            <EmptyDataTemplate>
                                                No Records For Today
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        All
                                    </div>
                                    <div class="panel-body panel-nomargin">
                                        <asp:GridView ID="gvStatsEmails" runat="server" AutoGenerateColumns="false" GridLines="None"
                                            CssClass="table table-hover table-striped table-bordered table-condensed"
                                            UseAccessibleHeader="true"
                                            DataKeyNames="callcenter"
                                            OnRowCommand="GridView_Grid_RowCommand"
                                            OnRowDataBound="GridView_Grid_RowDataBound"
                                            ShowFooter="true"
                                            >
                                            <Columns>
                                                <asp:TemplateField HeaderText="Center" ItemStyle-Width="55px" >
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="callcenter" Text='<%# Eval("callcenter") %>' />
                                                        <asp:HiddenField runat="server" ID="cnt" Value='<%# Eval("count").ToString() %>' />
                                                        <asp:HiddenField runat="server" ID="cntbounce" Value='<%# Eval("bounced").ToString() %>' />
                                                        <asp:HiddenField runat="server" ID="cntunsub" Value='<%# Eval("unsubscribe").ToString() %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="count" HeaderText="Emails" ItemStyle-Width="55px" />
                                                <asp:TemplateField HeaderText="Bounced" ItemStyle-Width="80px" >
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="bounced" Text='<%# Stats_Get_Percent(Eval("count").ToString(), Eval("bounced").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Unsub" ItemStyle-Width="65px">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="unsubscribe" Text='<%# Stats_Get_Percent(Eval("count").ToString(), Eval("unsubscribe").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Last Sent">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="datecreated" Text='<%# Custom.datetimeShort(Eval("last").ToString()) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle Font-Bold="true" />
                                            <EmptyDataTemplate>
                                                No Records
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <asp:Label runat="server" Text="" ID="lblPrint" />
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="gvEmails" />
                <asp:PostBackTrigger ControlID="btnGridExport" />
            </Triggers>
            <ContentTemplate>
                <script type="text/javascript">
                    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (evt, args) { $(document).ready(function () { onLoad(); }); });
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
                    function EndRequestHandler(sender, args) {
                        // This goes off on a re-load/partial update
                        var Error = args.get_error();
                        if (Error != null)
                        {
                            console.log("An error occurred while processing request. Please try again.");
                        }
                    }
                </script>
                <asp:Panel runat="server" id="hasEmails" Visible="false" CssClass="rowmb results" ScrollBars="Auto">
                    <div class="panel panel-default">
                        <div class="panel-heading" style="position: relative;">
                            <strong>Sent Agent Email</strong>
                            <div style="position: absolute;right: 1px;top: 4px;">
                                <asp:Button runat="server" ID="btnStatsEnable" Text="Enable Stats" OnClick="Stats_Enable" CssClass="btn btn-primary cancel" Visible="false" />
                            </div>
                        </div>
                        <div class="panel-body" id="divGrid">
                            <div class="row rowmb">
                                <div class="col-md-12">
                                    <div style="float: left;max-width: 300px;margin-right: 10px;">
                                        <div class="input-group">
                                            <span class="input-group-addon">Email Address</span>
                                            <asp:TextBox runat="server" CssClass="form-control" ID="tbEmailAddress" data-name="Email Address" />
                                        </div>
                                    </div>
                                    <div style="float: left;;margin-right: 10px;">
                                        <asp:Button runat="server" ID="btnGridSearch" Text="Search Emails" OnClick="Emails_Search" CssClass="btn btn-primary cancel" />
                                        <asp:Button runat="server" ID="btnGridExport" Text="Export Emails" OnClick="Custom_Export_Excel_SearchGrid" CssClass="btn btn-primary cancel" Visible="false" />
                                    </div>
                                    <div style="float: left;;margin-right: 10px;">
                                        <div class="input-group">
                                            <asp:DropDownList runat="server" ID="ddlCallCenters" CssClass="form-control" data-name="Call Centers" />
                                        </div>
                                    </div>
                                    <div style="float: left;;margin-right: 10px;">
                                        <div class="input-group">
                                            <asp:DropDownList runat="server" ID="ddlActiveStatus" CssClass="form-control" data-name="Status" />
                                        </div>
                                    </div>
                                    <div style="float: left;margin-right: 10px;">
                                        <div class="input-group">
                                            <asp:DropDownList runat="server" ID="ddlActiveAgents" CssClass="form-control" data-name="Agents" />
                                        </div>
                                    </div>
                                    <div style="float: left;max-width: 125px;">
                                        <div class="input-group">
                                            <asp:TextBox runat="server" CssClass="form-control datepicker-add" ID="tbDate" data-name="Date" PlaceHolder="Date" />
                                            <span id="searchclear" class="glyphicon glyphicon-remove-circle"></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="help-block">
                                        <asp:Label runat="server" Text="" ID="lblProcessMessage" />
                                    </div>
                                </div>
                            </div>
                            <asp:GridView ID="gvEmails" runat="server" AutoGenerateColumns="false" GridLines="None"
                                CssClass="table table-hover table-striped table-bordered table-condensed"
                                UseAccessibleHeader="true"
                                DataKeyNames="id"
                                EditRowStyle-CssClass="gvRowEdit"
                                OnRowCommand="GridView_Grid_RowCommand"
                                OnRowDataBound="GridView_Grid_RowDataBound"
                                OnRowEditing="GridView_Grid_RowEditing"
                                OnRowCancelingEdit="GridView_Grid_RowCancelingEdit"
                                OnRowUpdating="GridView_Grid_RowUpdating"
                                OnSelectedIndexChanged="GridView_Grid_Select" AllowSorting="true" OnSorting="GridView_Sorting"
                                AllowPaging="true" PageSize="50" 
                                OnPageIndexChanging="GridView_Grid_PageChange"
                                >
                                <Columns>
                                    <asp:TemplateField HeaderText="Business" ItemStyle-Width="150px">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="businessname" Text='<%# Eval("businessname").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfID" Value='<%# Eval("id").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfMiddleName" Value='<%# Eval("middlename").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfAgentFirstName" Value='<%# Eval("agentfirstname").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfAgentLastName" Value='<%# Eval("agentlastname").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfANI" Value='<%# Eval("ani").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfDNIS" Value='<%# Eval("dnis").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfCallID" Value='<%# Eval("callid").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfCallTime" Value='<%# Eval("calltime").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfDateCreated" Value='<%# Eval("datecreated").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfFirstName" Value='<%# Eval("firstname").ToString() %>' />
                                            <asp:HiddenField runat="server" ID="hfLastName" Value='<%# Eval("lastname").ToString() %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Phone" ItemStyle-Width="115px" Visible="true">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="businessphone" Text='<%# Eval("businessphone").ToString() %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email" ItemStyle-Width="275px">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="businessemail" Text='<%# Eval("businessemail").ToString() %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="110px">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="name" Text='<%# Custom.getFullName(Eval("firstname").ToString(), Eval("lastname").ToString()) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Center" ItemStyle-Width="90px" SortExpression="callcenter">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="callcenter" Text='<%# Eval("callcenter").ToString() %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:DropDownList runat="server" ID="ddlCenter" />
                                            <asp:HiddenField runat="server" ID="callcenter" Value='<%# Eval("callcenter").ToString() %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Agent" ItemStyle-Width="90px" SortExpression="agentid">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="agentid" Text='<%# Eval("agentid").ToString() %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:DropDownList runat="server" ID="ddlAgent" />
                                            <asp:HiddenField runat="server" ID="agentid" Value='<%# Eval("agentid").ToString() %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px" SortExpression="status">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="status" Text='<%# Custom.getLibraryItem(Eval("status").ToString()) %>' />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:DropDownList runat="server" ID="ddlStatus">
                                                <asp:ListItem Value="10002010" Text="Sent" />
                                                <asp:ListItem Value="10002020" Text="Clicked" />
                                                <asp:ListItem Value="10002030" Text="Bounced" />
                                                <asp:ListItem Value="10002040" Text="Registered" />
                                                <asp:ListItem Value="10002050" Text="Converted" />
                                                <asp:ListItem Value="10002060" Text="Unsubscribe" />
                                                <asp:ListItem Value="10002070" Text="Dead" />
                                                <asp:ListItem Value="10002100" Text="Test" />
                                            </asp:DropDownList>
                                            <asp:HiddenField runat="server" ID="status" Value='<%# Eval("status").ToString() %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date Sent" ItemStyle-Width="110px">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="datecreated" Text='<%# Custom.dateLong(Eval("datecreated").ToString()) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cmnd" ItemStyle-Width="90px">
                                        <ItemTemplate>
                                            <asp:Button runat="server" ID="btnEdit" Text="Edit" Visible='<%# IsEditReady() %>' CommandName="Edit" CssClass="btn btn-primary btn-xs" />
                                            <asp:Button runat="server" ID="btnRecordSelect" Text="View" Visible="true" CssClass="btn btn-primary btn-xs" OnClick="GridView_Grid_Select" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Button runat="server" Text="Update" Visible='<%# IsEditReady() %>' CommandName="Update" CssClass="btn btn-primary btn-xs" />
                                            <asp:Button runat="server" Text="Cancel" Visible='<%# IsEditReady() %>' CommandName="Cancel" CssClass="btn btn-primary btn-xs" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No Records For Selected Filters
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:GridView ID="gvEmailsExport" runat="server" AutoGenerateColumns="false" Visible="false">
                                <Columns>
                                    <asp:BoundField DataField="id" HeaderText="Id" />
                                    <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="status" Text='<%# Custom.getLibraryItem(Eval("status").ToString()) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="businessname" HeaderText="Business" />
                                    <asp:BoundField DataField="businessphone" HeaderText="Phone" />
                                    <asp:BoundField DataField="businessemail" HeaderText="Email" />
                                    <asp:BoundField DataField="firstname" HeaderText="First Name" />
                                    <asp:BoundField DataField="middlename" HeaderText="Middle" />
                                    <asp:BoundField DataField="lastname" HeaderText="Last Name" />
                                    <asp:BoundField DataField="callcenter" HeaderText="Call Center" />
                                    <asp:BoundField DataField="agentid" HeaderText="Agent ID" />
                                    <asp:BoundField DataField="agentfirstname" HeaderText="Agent First Name" />
                                    <asp:BoundField DataField="agentlastname" HeaderText="Agent Last Name" />
                                    <asp:BoundField DataField="ani" HeaderText="ANI" />
                                    <asp:BoundField DataField="dnis" HeaderText="DNIS" />
                                    <asp:BoundField DataField="callid" HeaderText="CallID" />
                                    <asp:BoundField DataField="calltime" HeaderText="CallTime" />
                                    <asp:BoundField DataField="datecreated" HeaderText="Sent" />
                                </Columns>
                            </asp:GridView>
                            <div style="margin-top: 20px;">
                                <asp:Label runat="server" Text="" ID="lblGridMessage" />
                            </div>
                            <div class="modal fade" id="myModal" role="dialog">
                                <div class="modal-dialog modal-xlg">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                                            <h4 class="modal-title" id="myModalLabel">Email Record Details</h4>
                                        </div>
                                        <div class="modal-body">
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Email ID</div>
                                                        <asp:Label runat="server" Text="" ID="lblEmailID" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Status</div>
                                                        <asp:Label runat="server" Text="" ID="lblStatus" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Business Name</div>
                                                        <asp:Label runat="server" Text="" ID="lblBusinessName" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Business Phone</div>
                                                        <asp:Label runat="server" Text="" ID="lblBusinessPhone" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Business Email</div>
                                                        <asp:Label runat="server" Text="" ID="lblEmailAddress" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">First Name</div>
                                                        <asp:Label runat="server" Text="" ID="lblFirstName" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Last Name</div>
                                                        <asp:Label runat="server" Text="" ID="lblLastName" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Call Center</div>
                                                        <asp:Label runat="server" Text="" ID="lblCallCenter" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Agent ID</div>
                                                        <asp:Label runat="server" Text="" ID="lblAgentID" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Agent F.Name</div>
                                                        <asp:Label runat="server" Text="" ID="lblAgentFirstName" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Agent L.Name</div>
                                                        <asp:Label runat="server" Text="" ID="lblAgentLastName" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">ANI</div>
                                                        <asp:Label runat="server" Text="" ID="lblANI" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">DNIS</div>
                                                        <asp:Label runat="server" Text="" ID="lblDNIS" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Call ID</div>
                                                        <asp:Label runat="server" Text="" ID="lblCallID" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Call Time</div>
                                                        <asp:Label runat="server" Text="" ID="lblCallTime" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                        <div class="input-group-addon minwidth-addon minwidth125-addon strong">Date Created</div>
                                                        <asp:Label runat="server" Text="" ID="lblDateCreated" CssClass="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                                        <Triggers>
                                                            <asp:AsyncPostBackTrigger ControlID="btnNotesAdd" />
                                                            <asp:AsyncPostBackTrigger ControlID="btnNotesAddCancel" />
                                                        </Triggers>
                                                        <ContentTemplate>
                                                            <div class="panel panel-default">
                                                                <div class="panel-heading" style="position: relative;">
                                                                    <strong>Notes</strong>
                                                                    <div style="position: absolute;right: 1px;top: 4px;">
                                                                        <asp:Button runat="server" ID="btnNotesAdd" Text="Add Note" OnClick="AddNote_Show" CssClass="btn btn-primary cancel" Visible="true" />
                                                                    </div>
                                                                </div>
                                                                <div class="panel-body" id="divGridNotes" runat="server" visible="true">
                                                                    <asp:GridView ID="gvNoteList" runat="server" AutoGenerateColumns="false" GridLines="None"
                                                                        CssClass="table table-hover table-striped table-bordered table-condensed"
                                                                        UseAccessibleHeader="true"
                                                                        DataKeyNames="index"
                                                                        OnRowCommand="GridView_Grid_RowCommand"
                                                                        OnRowDataBound="GridView_Grid_RowDataBound"
                                                                        ShowFooter="false"
                                                                        AllowPaging="true" PageSize="25"
                                                                        >
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Created" ItemStyle-Width="175px">
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="actor" Text='<%# Eval("actor").ToString() %>' />
                                                                                    <br /><asp:Label runat="server" ID="datecreated" Text='<%# Custom.dateLong(Eval("datecreated").ToString()) %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="note" HeaderText="note" />
                                                                        </Columns>
                                                                        <FooterStyle Font-Bold="true" />
                                                                        <EmptyDataTemplate>
                                                                            No Notes to Display
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                    <div class="input-group has-error">
                                                                        <div class="help-block">
                                                                            <asp:Label runat="server" ID="lblMsgNotes2"></asp:Label>
                                                                        </div>
                                                                    </div>

                                                                </div>
                                                                <div class="panel-body" id="divGridNoteAdd" runat="server" visible="false">
                                                                    <div style="margin-bottom: 10px;">
                                                                        <asp:TextBox runat="server" ID="tbNote" TextMode="MultiLine" Rows="5" Columns="50"></asp:TextBox>
                                                                    </div>
                                                                    <div>
                                                                        <asp:Button runat="server" ID="btnNotesAddSubmit" Text="Add Note" OnClick="AddNote_Add" CssClass="btn btn-primary cancel" Visible="true" />
                                                                        <asp:Button runat="server" ID="btnNotesAddCancel" Text="Cancel" OnClick="AddNote_Hide" CssClass="btn btn-primary cancel" Visible="true" />
                                                                    </div>
                                                                    <div class="input-group has-error">
                                                                        <div class="help-block">
                                                                            <asp:Label runat="server" ID="lblMsgNotes"></asp:Label>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>                                                    
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <div style="margin-top: 20px;">
                    <%--<asp:Label runat="server" Text="" ID="lblProcessMessage" />--%>
                </div>
                <script type='text/javascript'>
                    function openModal() {
                        $('#myModal').modal('show');
                    }
                </script>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
