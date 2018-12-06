<%@ Page Language="C#" AutoEventWireup="true" CodeFile="script_main.aspx.cs" Inherits="script_main" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <title>ARC IVR Donation Script</title>
    <meta name="robots" content="noindex, nofollow" />
    <link rel="shortcut icon" href="favicon.ico" />
    <link href="css/script.css?v=<%=scriptVersion %>" rel="stylesheet" type="text/css" />
    <script src="js/script_all.js?v=<%=scriptVersion %>" type="text/javascript"></script>
    <script src="js/script.js?v=<%=scriptVersion %>" type="text/javascript"></script>
    <style type="text/css">
    /* css for timepicker */
    .ui-timepicker-div .ui-widget-header { margin-bottom: 8px; }
    .ui-timepicker-div dl { text-align: left; }
    .ui-timepicker-div dl dt { height: 25px; margin-bottom: -25px; }
    .ui-timepicker-div dl dd { margin: 0 10px 10px 65px; }
    #ui-timepicker-div td { font-size: 50%; }
    .ui-tpicker-grid-label { background: none; border: none; margin: 0; padding: 0; }
    .ui-widget{font-size: 12px;}
    .ui-button-text, .ui-button, .ui-button-text {font-size: 10px !important;}
    .ui-selectmenu-menu, .ui-selectmenu, .ui-selectmenu-text {font-size: 10px !important;}
    .wrap {display: inline-block;}
    #ui-datepicker-div{font-size: 12px;}
    #ui-datepicker-div{margin-left: 30px;}
    </style>
    <script src="js/jquery/jquery-1.7.1.min.js?v=<%=scriptVersion %>"" type="text/javascript"></script>
    <script src="js/jquery/jquery-ui-1.8.18.custom.min.js?v=<%=scriptVersion %>"" type="text/javascript"></script>
    <script src="js/jquery/jquery-ui-timepicker-addon.js?v=<%=scriptVersion %>"" type="text/javascript"></script>
    <script src="js/jquery/jquery.ui.selectmenu.js?v=<%=scriptVersion %>"" type="text/javascript"></script>
    <script src="js/script_jquery.js?v=<%=scriptVersion %>" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function() {
            // GH_DatePicker_NoDefault(); // Why is this here?
        });
        $(document).ready(function () {
            // This is to ensure that java loads
            $("#script").show();
        });
        function popWEB01() {
            //
            var pUrl = "#" + $("#ZipCode").val();
            var pName = "ALABAMA";
            var pControl = "width=895,height=800,scrollbars,resizeable=yes,toolbar=1,status=1";
            window.open(pUrl, pName, pControl).focus();
        }
        // Toggle_All('show');
        // callDetails('show'); 
    </script>
    <style type="text/css">
        .step_label
        {
            font-weight: bold;
            color: Red;
            /*display: none;*/
            <% Response.Write(step_label()); %>
        }
        .step_label:before
        {
        	content: " Continue >> ";
        }
        .english
        {
            <% Response.Write(language_english()); %>
        }
        .spanish
        {
            <% Response.Write(language_spanish()); %>
        }
        .calltype_disabled
        {
            display: none;
            visibility: hidden;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            //$('.english').css({ "display": "none" });
            //$('.spanish').css({ "display": "block" });
            //$('.english').removeAttr("display");
            //languageSpanish();
            if ($("#cdLanguage").val() == "English") { languageEnglish(); }
            if ($("#cdLanguage").val() == "Spanish") { languageSpanish(); }
        });
        function languageEnglish() {
            $('.english').css({ "display": "block" });
            $('.spanish').css({ "display": "none" });
            //$('.english').css({ "visibility": "visible" });
            //$('.spanish').css({ "visibility": "collapse" });
            $("#tglLanguage").text("English");
        }
        function languageSpanish() {
            $('.english').css({ "display": "none" });
            $('.spanish').css({ "display": "block" });
            //$('.english').css({ "visibility": "collapse" });
            //$('.spanish').css({ "visibility": "visible" });
            $("#tglLanguage").text("Spanish");
        }
        function languageToggle() {
            //alert($("#tglLanguage").text());
            var tgl = $("#tglLanguage").text();
            if (tgl == "English") {
                languageSpanish();
            } else {
                languageEnglish();
            }
        }
    </script>
</head>
<body onload="pageLoad();">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
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
                Agent Script
            </div>
            <div class="agent_note" id="agent_whisper" runat="server" visible="true" >
                Press 0 (zero) on keypad to accept call
            </div>
        </div>
        <div id="container" style="vertical-align: top;">
            &nbsp;
            <div id="noscript">
                This page requires JavaScript to function.
                <br />You must enable JavaScript to go further.
            </div>
            <div id="header_control" class="script_header" style="width: 900px;background-color: <%=scriptColor %>;" >
                <div class="clearfix">
                    <div style="float: left;">
                        <a style="margin-right: 0px;" href="javascript:popTRAINING()">FAQs</a>
                        <br /><a style="margin-right: 0px;" href="javascript:popTRAINING_DRTV()" runat="server" id="faq_drtv">DRTV FAQs</a>
                        <br /><a style="margin-right: 0px;" href="javascript:popTRAINING_GLOBE()" runat="server" id="faq_globe">Globetrotters FAQs</a>
                    </div>
                    <%--
                    <div style="float: left;width: 285px;margin-left: 10px;" class="red">
                        <asp:Label ID="lblMode" runat="server" Text="" />
                        <asp:Label ID="lblJRE" runat="server" Text="" />
                    </div>
                    --%>
                    <div style="float: right;margin-left: 10px;text-align: right;">
                        <style type="text/css">
                            /*
                                A style to float boxes next to each other
                            */
                        </style>
                        <asp:Panel ID="pnlCallInfo" runat="server" Visible="true">
                            <div>
                                <div>
                                    Agent: <asp:Label ID="lblCI_AgentName" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                    | ID <asp:Label ID="lblCI_AgentID" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                </div>
                                <div>
                                    Call Time: <asp:Label ID="lblCI_CallLength" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                    ID: <asp:Label ID="lblCI_InteractionID" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                </div>
                                <div>
                                    DNIS: <asp:Label ID="lblCI_DNIS" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                    Disposition: <asp:Label ID="lblCI_Disposition" runat="server" Text="" ForeColor="Blue" Font-Bold="true" />
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="pnlControls" runat="server" Visible="false">
                            <asp:HyperLink ID="reload_script" runat="server" style="margin-right: 5px;">Reload Script</asp:HyperLink>
                            <a style="margin-right: 5px;display: none;" id="tglEscape" name="tglEscape" href="javascript:Toggle_Escape('hide')" title="This will escape out to end the call early.">Escape</a>
                            <a style="display: none;margin-right: 5px;" id="tglUsed" name="tglUsed" href="javascript:Toggle_All('show')" title="This will allow you to open all fields, and revert back to previous selection.">Show Used</a>
                            <a style="margin-right: 5px;" id="tglAll" name="tglAll" href="javascript:Toggle_All('show')" title="This will allow you to open all fields, and revert back to previous selection.">Show All</a>
                            <a style="margin-right: 0px;" href="javascript:callDetails('show')" id="call_details_toggle">Call Details</a>
                            <a style="display: none;margin-right: 0px;" href="javascript:popFAQ()">FAQ</a>
                            <a style="margin-right: 0px;display: none;" href="javascript:popHELP()">Help</a>
                            <a style="margin-right: 0px;" href="javascript:languageToggle();" id="tglLanguage" name="tglLanguage">English</a>
                            <a style="margin-right: 5px;" id="tglHistory" name="tglHistory" href="javascript:Toggle_History('show')" title="This will toggle the history.">Show Used</a>
                        </asp:Panel>
                    </div>
                </div>
            </div>
            <asp:Panel ID="pnlAgentStats" runat="server" Visible="false">
                <a href="call_links.aspx?agentid=<%=cdAgentDeID.Text %>" style="text-decoration: none;">
                    <div id="agent_stats" class="script_header" style="width: 900px;text-align: center;background-color: <%=scriptColorStats %>;margin-top: -7px;">
                        <div style="display: inline-block;margin: 3px;">
                            Open Calls: <asp:Label ID="lblCallsOpen" runat="server" Text="" ForeColor="Black" Font-Bold="true" />
                        </div>
                        <div style="display: inline-block;margin: 3px;">
                            Completed Last 12 Hours: <asp:Label ID="lblCallsCompleted" runat="server" Text="" ForeColor="Black" Font-Bold="true" />
                        </div>
                    </div>
                </a>
            </asp:Panel>
            <div id="script" style="display: none;">
                <div runat="server" id="section_A0" class="section">
                    <h2>Information<span>A0</span></h2>
                    <div class="field">
                        <asp:Panel ID="pnlDeclineToPledge" runat="server" Visible="false" BackColor="White">
                            <div class="field">
                                <div class="english agent_note">
                                    I'm sorry, I'm having trouble processing this card.  Would you like to try a different card?
                                    <br />
                                    <br />IF NO: That's fine.  What I can do is send out a pre addressed envelope for you to send back your donation by check or you can send a cancelled check to set up a recurring donation from your bank account.  May I use your billing address to send the letter out right away?
                                    <br />
                                    <br />
                                </div>
                                <div class="field_option">
                                    <asp:RadioButtonList ID="rb0_options" runat="server">
                                        <asp:ListItem Value="YES">YES - Try Different Card<span class="step_label">A4</span></asp:ListItem>
                                        <asp:ListItem Value="BILLING">Envelope - Use Billing Address<span class="step_label">A96</span></asp:ListItem>
                                        <asp:ListItem Value="NEWADDRESS">Envelope - Enter New Address<span class="step_label">A5</span></asp:ListItem>
                                        <asp:ListItem Value="NO">END CALL<span class="step_label">A98</span></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Label ID="processMessage" runat="server" Text="" ForeColor="Red" Font-Bold="true" />
                        <asp:Label ID="ResponseSQL" runat="server" Text="Informational Messages will appear here." />
                        <asp:Label ID="lblError" runat="server" Text="" ForeColor="Red" />
                        <asp:HyperLink ID="chrome_link" runat="server" Visible="false">Copy link to Chrome</asp:HyperLink>
                    </div>
                    <asp:Panel ID="pnlNewCall" runat="server" Visible="false" BackColor="LightGray">
                        <div class="field">
                            <asp:Label ID="lblNewCall" runat="server" Text="Start a new call?" />
                            <br />This will fetch agent information from Five9
                            <br />Use your full name as it appears in Five9
                        </div>
                        <div class="field">
                            <div class="single_line clearfix" style="display: none;">
                                <div class="line_label">
                                    AgentID
                                    <span class="line_required">*</span>
                                </div>
                                <div class="line_input">
                                    <asp:TextBox ID="txtAgentID" runat="server" PlaceHolder="Agent ID" MaxLength="100" />
                                </div>
                            </div>
                        </div>
                        <div class="field">
                            <div class="single_line clearfix">
                                <div class="line_label">
                                    Agent Name
                                </div>
                                <div class="line_input">
                                    <asp:TextBox ID="txtAgentName" runat="server" PlaceHolder="" MaxLength="100" />
                                </div>
                            </div>
                        </div>
                        <div class="field">
                            <div class="single_line clearfix">
                                <div class="line_label">
                                    DNIS Selection
                                    <span class="line_required">*</span>
                                </div>
                                <div class="field_option">
                                    <asp:DropDownList ID="ddlNewDNIS" runat="server" Width="400px" />
                                    <%--rdlNewDNIS--%>
                                </div>
                            </div>
                            <asp:Panel ID="pnlDeBug" runat="server" Visible="false">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        Load Test Data
                                        <span class="line_required">*</span>
                                    </div>
                                    <div class="line_radio">
                                        <asp:CheckBox ID="chkTestData" runat="server" />
                                    </div>
                                </div>
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        Load DeBug Data
                                        <span class="line_required">*</span>
                                    </div>
                                    <div class="line_radio">
                                        <asp:CheckBox ID="chkDeBugData" runat="server" />
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                        <div class="field" style="text-align: center;font-size: 18px;">
                            <asp:Button ID="Button1" runat="server" Text="Start a New Call" OnClick="SubmitNewCall" style="height: 30px;width: 130px;font-size: 14px;" />
                            <br />Only use this if the POP did not work or have to create a 2nd donation for 1 call
                            <br />Report issue to IT
                        </div>
                    </asp:Panel>
                    <div id="control_A0" class="section_control">
                        <input id="continue_A0" type="button" value="Continue >>" onclick="Validate_A0()" />
                        <input id="back_A0" type="button" value="Start Over" onclick="Return_A0()" />
                        <input type="button" value="Close Window" onclick="CloseWindow()" style="display: none;" />
                    </div>
                </div>
                <div runat="server" id="section_A1" class="section">
                    <h2>Greeting<span>A1</span></h2>
                    <div class="field">
                        <asp:Label ID="lblAgentName" runat="server" Text="{agent_name}" ForeColor="Blue" Visible="false" />
                        <div id="dGreetingStandard" runat="server">
                            <div class="english" id="dGreetingStandard_English" runat="server">
                                Thank you for calling the American Red Cross.
                                <br /><br />This is <span class='agent_note'>{agent_name}</span> and I would be happy to assist you with your donation. Shall I take care of that for you now?
                            </div>
                            <div class="spanish" id="dGreetingStandard_Spanish" runat="server">
                                Gracias por llamar a la Cruz Roja Americana.
                                <br /><br />Esta es <span class='agent_note'>{agent_name}</span> y estaría encantado de ayudarle con su donación. ¿Me encargaré de eso ahora?
                            </div>
                        </div>
                        <div id="dGreetingHoliday" runat="server">
                            <div class="english" id="dGreetingHoliday_English" runat="server">
                                Thank you for calling the American Red Cross Holiday Giving Catalog Line.
                                <br /><br />This is <span class='agent_note'>{agent_name}</span>, and I am happy to assist you in making a gift that will bring comfort and hope this season.  Would you like to make a gift to the Holiday Catalog today?
                            </div>
                            <div class="spanish" id="dGreetingHoliday_Spanish" runat="server">
                                Gracias por llamar a la Cruz Roja Americana y el Catologo de donaciones de festividades.
                                <br /><br />Esta es <span class='agent_note'>{agent_name}</span>, y estoy encantado de ayudarle a hacer un regalo que traer&aacute; consuelo y esperanza en esta temporada. &iquest;Te gustar&iacute;a hacer un regalo al Cat&aacute;logo Holiday hoy?
                            </div>
                        </div>
                        <div id="dGreetingDRTV" runat="server">
                            <div class="english" id="dGreetingDRTV_English" runat="server">
                                Thank you for calling the American Red Cross.
                                <br /><br />This is <span class='agent_note'>{agent_name}</span> Are you calling to make a monthly donation today?
                            </div>
                            <div class="spanish" id="dGreetingDRTV_Spanish" runat="server">
                                Gracias por llamar a la Cruz Roja Americana.
                                <br /><br />Mi nombre es <span class='agent_note'>{agent_name}</span> ¿Estás llamando para hacer una donación mensual hoy?
                            </div>
                        </div>
                        <div id="dGreetingDynamic" runat="server">
                            <div class="english" id="dGreetingDynamic_English" runat="server">
                                <asp:Label ID="lblGreetingDynamic_English" runat="server" Text="{call_greeting_english}" />
                            </div>
                            <div class="spanish" id="dGreetingDynamic_Spanish" runat="server">
                                <asp:Label ID="lblGreetingDynamic_Spanish" runat="server" Text="{call_greeting_spanish}" />
                            </div>
                        </div>
                        <%--No Longer Used--%>
                        <asp:Label ID="lblCallGreeting" runat="server" Text="{call_greeting}" ForeColor="Black" Visible="false" />
                    </div>
                    <div class="field">
                        <div class="field_option">
                            <asp:RadioButtonList ID="rb1_options" runat="server" />
                        </div>
                    </div>
                    <div id="control_A1" class="section_control">
                        <input id="continue_A1" type="button" value="Continue >>" onclick="Validate_A1()" tabindex="1" />
                        <input id="escape_A1" type="button" value="Escape >>" onclick="Escape_A1()" />
                        <input id="back_A0A" type="button" value="Back to A0" onclick="Return_A0A()" />
                        <input id="back_A25Y" type="button" value="Back to A25" onclick="Return_A25Y()" />
                        <input id="back_A37Y" type="button" value="Back to A37" onclick="Return_A37Y()" />
                        <input id="back_A38Y" type="button" value="Back to A37" onclick="Return_A38Y()" />
                    </div>
                </div>
                <div runat="server" id="section_A2" class="section">
                    <h2>Donation Collection<span>A2</span></h2>
                    <div class="field agent_note" id="A2_standard_01">
                        <div class="english">
                            How much would you like to donate today?
                        </div>
                        <div class="spanish">
                            ¿Cuánto le gustaría donar hoy?
                        </div>                        
                    </div>
                    <div class="field" id="A2_drtv_01">
                        <div class="english">
                            <div class="A2_drtv_01_standard">
                                <%-- How much would you like your monthly gift to be? --%>
                                Thank you for your generous donation!  $50 or $75 can help provide support to people who have lost everything in a disaster.  How much would you like to give today?
                            </div>
                            <div class="A2_drtv_01_champion">
                                Thank you for your generous monthly donation!  The Red Cross responds to nearly 200 emergencies across the country every day.  A gift of $19 or even $25 a month will help us ensure that we are ready to help when disaster strikes.  How much would you like to give today?
                                <div class="agent_note">
                                    IF ASKED HOW THE MONEY IS USED:
                                </div>
                                <div style="font-style: italic;">
                                    From small house fires to multi-state natural disasters, the American Red Cross goes wherever we’re needed, so people can have clean water, safe shelter, hot meals and hope when they need it most.  
                                </div>
                            </div>
                        </div>
                        <div class="spanish">
                            <div class="A2_drtv_01_standard">
                                <%-- ¿Cuánto le gustaría que tu regalo mensual fuera? --%>
                                ¡Gracias por su generosa donación! $ 50 o $ 75 pueden ayudar a brindar apoyo a las personas que han perdido todo en un desastre. ¿Cuánto te gustaría dar hoy?
                            </div>
                            <div class="A2_drtv_01_champion">
                                ¡Gracias por tu generosa donación mensual! La Cruz Roja responde a casi 200 emergencias en todo el país todos los días. Un regalo de $ 19 o incluso $ 25 al mes nos ayudará a garantizar que estamos listos para ayudar cuando ocurra un desastre. ¿Cuánto te gustaría dar hoy?
                                <div class="agent_note">
                                    IF ASKED HOW THE MONEY IS USED:
                                </div>
                                <div style="font-style: italic;">
                                    Desde los incendios domésticos pequeños hasta los desastres naturales en varios estados, la Cruz Roja Americana va donde sea que se necesite, para que las personas puedan tener agua limpia, refugio seguro, comidas calientes y esperanza cuando más lo necesitan.
                                </div>
                            </div>
                        </div>                        
                    </div>
                    <div class="field">
                        <div class="field_option">
                            $ <asp:TextBox ID="tb2_amount_dollar" runat="server" MaxLength="6" Width="75px" />.<asp:TextBox ID="tb2_amount_cent" runat="server" MaxLength="2" Width="50px" Text="00" />
                            <span class="step_label" id="A2_step_label">A3</span>
                        </div>
                    </div>
                    <div class="field_info" style="width: 600px;">
                        <div id="A2_standard_02">
                            <div class="english">
                                Would you like to make this a monthly gift?  By making a monthly gift you'll join a special group of donors known as Red Cross Champions who are the heart and soul of our mission - whose help with a consistent and reliable source of funding ensures the Red Cross can respond to nearly 200 emergencies a day across the country. Would you like to become a Red Cross Champion today?
                            </div>
                            <div class="spanish">
                                &iquest;Te gustar&iacute;a hacer esto un regalo mensual? Al hacer un regalo mensual que se una a un grupo especial de donantes conocidos como Red de Campeones de la Cruz, que son el coraz&oacute;n y el alma de nuestra misi&oacute;n - cuya ayuda con una fuente consistente y fiable de financiaci&oacute;n garantiza la Cruz Roja puede responder a casi 200 emergencias de un d&iacute;a en todo el pa&iacute;s. &iquest;Le gustar&iacute;a convertirse en un campe&oacute;n de la Cruz Roja de hoy?
                            </div>
                        </div>
                        <div id="A2_drtv_02" style="display: none;">
                            <span class="field agent_note"><br />{Only if donation amount is BELOW $25.00}</span>
                            <div class="english">
                                Many people are rounding up their monthly donation to $25 today to help even more families, would you like to do the same?
                            </div>
                            <div class="spanish">
                                Muchas personas están redondeando su donación mensual a $ 25 hoy para ayudar a más familias, ¿le gustaría hacer lo mismo?                                
                            </div>
                        </div>
                    </div>
                    <div class="field">
                        <div class="field_option" id="A2_standard_03">
                            <asp:RadioButtonList ID="RadioButtonList2" runat="server">
                                <asp:ListItem Value="YES">YES<span class="step_label">A3</span></asp:ListItem>
                                <asp:ListItem Value="NO">NO, ONE TIME DONATION<span class="step_label">A3</span></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div id="control_A2" class="section_control">
                        <input id="continue_A2Catalog" type="button" value="Holiday Catalog >>" onclick="Validate_A2_Catalog()" runat="server" Visible="false" />
                        <input id="continue_A2" type="button" value="Continue >>" onclick="Validate_A2()" />
                        <input id="back_A1Y" type="button" value="Back to A1" onclick="Return_A1Y()" />
                        <input id="back_A35Y" type="button" value="Back to A35" onclick="Return_A35Y()" />
                        <asp:HiddenField ID="sectionA2drtv" runat="server" Value="drtv" />
                    </div>
                </div>
                <div runat="server" id="section_A43" class="section">
                    <h2>Sustainer Charge Date<span>A43</span></h2>
                    <div class="field sectionA43_standard">
                        <div class="field_info">
                            <div class="english">
                                Great! Today's donation will process as usual. Next month, your donation will automatically process. This takes place on the 1st or the 15th of each month. Which day would you like yours to process?
                            </div>
                            <div class="spanish">
                                &iexcl;Excelente! Donaci&oacute;n de hoy procesar&aacute; como de costumbre. El pr&oacute;ximo mes, su donaci&oacute;n procesar&aacute; autom&aacute;ticamente. Esto se lleva a cabo en la primera o el 15 de cada mes. &iquest;Qu&eacute; d&iacute;a te gustar&iacute;a suya para procesar?
                            </div>
                        </div>
                    </div>
                    <div class="field sectionA43_drtv">
                        <div class="field_info">
                            <div class="english">
                                Thank you!  By making a monthly gift you are joining a special group of donors known as Red Cross Champions.  By providing steady monthly support, our Champions make sure that the Red Cross can help people affected by disasters big and small, no matter when or where the disasters strike.
                                <br /><br />Today’s donation will process as usual. Next month, your donation will automatically process. This takes place on the 1st or the 15th of each month. Which day would you like yours to process?
                            </div>
                            <div class="spanish">
                                Thank you!  By making a monthly gift you are joining a special group of donors known as Red Cross Champions.  By providing steady monthly support, our Champions make sure that the Red Cross can help people affected by disasters big and small, no matter when or where the disasters strike.
                                <br /><br />Today’s donation will process as usual. Next month, your donation will automatically process. This takes place on the 1st or the 15th of each month. Which day would you like yours to process?
                            </div>
                        </div>
                    </div>
                    <div class="field">
                        <div class="field_option">
                            <asp:RadioButtonList ID="RadioButtonList43" runat="server">
                                <asp:ListItem Value="1">1<sup>st</sup><span class="step_label">A3</span></asp:ListItem>
                                <asp:ListItem Value="15">15<sup>th</sup><span class="step_label">A3</span></asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:HiddenField id="rdSustainer" runat="server" value="" />
                            <asp:HiddenField id="rdChargeDate" runat="server" value="" />
                            <asp:HiddenField id="rdChargeDateOriginal" runat="server" value="" />                            
                        </div>
                    </div>
                    <div id="control_A43" class="section_control">
                        <input id="continue_A43" type="button" value="Continue >>" onclick="Validate_A43()" tabindex="1" />
                        <input id="back_A2N" type="button" value="Back to A2" onclick="Return_A2N()" />
                        <input id="back_A4F" type="button" value="Back to A4" onclick="Return_A4F()" />
                        <asp:HiddenField ID="backA4F" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A44" class="section">
                    <h2>Sustainer Change Date<span>A44</span></h2>
                    <div id="dev01" runat="server" visible="false">
                        If original donation is placed at the beginning of the month and donor opts to be charge on the 15th  
                        For example, Jan 1 donation and opts to be charge on the 15th - that takes you all the way to Feb 15th which would be 45 days in between charges. 
                        Agents will provide this verbiage any time a donation is 1. Placed on days 1-10 of the month AND 2. Opts to be charged on the 15th of the month  
                        
                        Thank you for your generous donation! As it is, your next monthly contribution is set to process more than 30 days from now. Would you prefer that I adjust your future monthly transactions to the 1st of each month?
                        
                        If original donation is placed late in the month and donor opts for the 1st
                        For example, Jan 29th and opts for the 1st- that would only be 2 days between charges. 
                        Agents will provide this verbiage any time a donation is 1. Placed on days 16-31 of the month AND 2. Opts to be charged on the 1st of the month
                        
                        Thank you for your generous donation! As is it, your next monthly contribution is set to process in less than 20 days from now. Would you prefer that I adjust your future monthly transactions to the 15th of each month?
                    </div>
                    <asp:HiddenField ID="sectionA44long" runat="server" Value="hide" />
                    <asp:HiddenField ID="sectionA44short" runat="server" Value="hide" />
                    <div class="field">
                        <div runat="server" id="section_A44long" class="field_info">
                            <div class="english">
                                Thank you for your generous donation! As it is, your next monthly contribution is set to process more than 30 days from now. Would you prefer that I adjust your future monthly transactions to the 1st of each month?
                            </div>
                            <div class="spanish">
                                Gracias por su generosa donaci&oacute;n usted! Como es, su pr&oacute;xima contribuci&oacute;n mensual se establece para procesar m&aacute;s de 30 d&iacute;as a partir de ahora. &iquest;Preferir&iacute;a que me ajusto sus transacciones mensuales futuros a la primera de cada mes?
                            </div>
                        </div>
                        <div runat="server" id="section_A44short" class="field_info">
                            <div class="english">
                                Thank you for your generous donation! As is it, your next monthly contribution is set to process in less than 20 days from now. Would you prefer that I adjust your future monthly transactions to the 15th of each month?
                            </div>
                            <div class="spanish">
                                Gracias por su generosa donaci&oacute;n usted! Como es, su pr&oacute;xima contribuci&oacute;n mensual se establece en procesar en menos de 20 d&iacute;as a partir de ahora. &iquest;Preferir&iacute;a que me ajusto sus transacciones mensuales futuras para el d&iacute;a 15 de cada mes?
                            </div>
                        </div>
                        <asp:Label ID="Label5" runat="server" Text="{call_greeting}" ForeColor="Black" Visible="false" />
                    </div>
                    <div class="field">
                        <div class="field_option">
                            <asp:RadioButtonList ID="RadioButtonList44" runat="server">
                                <asp:ListItem Value="1">CHANGE DATE TO <span id="rbl4_opt1">txt</span><span class="step_label">A3</span></asp:ListItem>
                                <asp:ListItem Value="0">KEEP DATE TO <span id="rbl4_opt2">txt</span><span class="step_label">A3</span></asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:HiddenField id="rdChargeDateChange" runat="server" value="" />
                        </div>
                    </div>
                    <div id="control_A44" class="section_control">
                        <input id="continue_A44" type="button" value="Continue >>" onclick="Validate_A44()" tabindex="1" />
                        <input id="back_A43N" type="button" value="Back to A43" onclick="Return_A43N()" />
                    </div>
                </div>
                <div runat="server" id="section_A3" class="section">
                    <h2>Designation Selection<span>A3</span></h2>
                    <div class="field_info sectionA3_standard" style="width: 600px;">
                        <div runat="server" id="section_A3high" class="agent_note">
                            <div class="english">
                                Donation is $5,000 or higher
                            </div>
                            <div class="spanish">
                                La donaci&oacute;n es de $5,000 o m&aacute;s alta
                            </div>
                        </div>
                        <div class="field sectionA3_main">
                            <div class="english">
                                How would you like to direct your donation today?
                            </div>
                            <div class="spanish">
                                &iquest;C&oacute;mo le gustar&iacute;a dirigir su donaci&oacute;n hoy?
                            </div>
                        </div>
                        <div class="field sectionA3_disaster">
                            <span class="english">
                                Your donation Today is for
                            </span>
                            <span class="spanish">
                                Su donación Hoy es para
                            </span>
                             <span style="font-weight: bold;" id="sectionA3_disaster_name"></span>
                        </div>
                        <div runat="server" id="section_A3sustainer" class="agent_note">
                            Sustainer Donation: Only standard designations are valid (Disaster, Where It's Needed Most, Local Chapter)
                        </div>
                    </div>
                    <div class="field sectionA3_hidden"  id="sectionA3_hidden">
                        <%--sectionA3_drtv--%>
                        <div runat="server" class="agent_note">
                            AGENT- Continue through this section
                            <div class="field sectionA3_drtv"  id="sectionA3_drtv">
                                DRTV Call
                                <br />You do not need to explain designation to the donor unless asked.
                            </div>
                            <div class="field sectionA3_globe"  id="sectionA3_globe">
                                Harlem Globetrotters Call
                                <br />You do not need to explain designation to the donor unless asked.
                            </div>
                            <div class="field sectionA3_disaster"  id="sectionA3_disaster">
                                Disaster/Telethon Call
                                <br />Pause briefly than continue
                            </div>
                            <br /><br />If donor insists on other designations click below to show the other designations.
                            <br />This is only to be used if asked. DO NOT OFFER these designations.
                            <br /><input id="sectionA3_drtv_btn" type="button" value="Show Designations" onclick="Designation_Toggle_DRTV('show');" />
                            <br /><br />&nbsp;
                        </div>
                    </div>
                    <div class="field sectionA3_standard">
                        <div class="field_option">
                            <asp:RadioButtonList ID="rb3_designation" runat="server">
                                <%-- 2015-05-01 - DESIGNATION *NEPAL* ADDED --%>
                                <%-- 2015-05-26 - DESIGNATION *NEPAL* REMOVED --%>
                                <%--<asp:ListItem Value="173" onclick="Validate_A3_GoTo('A4')" >
                                    <b>Nepal Earthquake Relief</b><span class="step_label">A4</span>
                                    <div style="margin-left: 25px;margin-bottom: 15px;">
                                        Help people affected by the earthquake in Nepal. Your gift enables the Red Cross to respond to and help people recover from this disaster.
                                    </div>
                                </asp:ListItem>--%>
                            </asp:RadioButtonList>
                            <asp:HiddenField ID="gotoA3" runat="server" />
                            <asp:HiddenField ID="sectionA3other" runat="server" Value="hide" />
                            <asp:HiddenField ID="sectionA3gc" runat="server" Value="hide" />
                            <asp:HiddenField ID="sectionA3sc" runat="server" Value="hide" />
                            <asp:HiddenField ID="sectionA3high" runat="server" Value="hide" />
                            <asp:HiddenField ID="sectionA3sustainer" runat="server" Value="hide" />
                            <asp:HiddenField ID="hcDesignation" runat="server" Value="" />
                            <asp:HiddenField ID="hcSection" runat="server" Value="41" />
                            <asp:HiddenField ID="hcSelectName" runat="server" Value="" />
                            <%-- Since only literal and no server controls are allowed inside <asp:ListItem>, need to populate via jQuery instead --%>
                            <asp:HiddenField ID="hcCatalogTitle" runat="server" Value="" />
                            <asp:HiddenField id="hcShowHolidayCatalog" runat="server" value="" />
                            <asp:HiddenField id="hcPremiumGiftSKU" runat="server" value="" />
                            <asp:HiddenField id="hcAlternateAddressOffer" runat="server" value="" />
                            <asp:HiddenField id="hcAlternateAddressUse" runat="server" value="" />
                        </div>
                    </div>
                    <div id="control_A3" class="section_control">
                        <input id="continue_A3" type="button" value="Continue >>" onclick="Validate_A3()" />
                        <input id="back_A2Y" type="button" value="Back to A2" onclick="Return_A2Y()" />
                        <input id="back_A1Yb" type="button" value="Back to A1b" onclick="Return_A1Yb()" /><asp:Label ID="gotoA3B" runat="server" Text="" ForeColor="Red" />
                        <asp:HiddenField id="backA1Yb" runat="server" value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A41" class="section">
                    <h2 id="hcTitle2">[Catalog Title] - Choose Catalog Items<span>A41</span></h2>
                    <asp:ListView ID="lstHolidayCatalog" runat="server" ItemPlaceholderID="itmPlaceholder">
                        <EmptyDataTemplate>
                            <div class="field_info">
                                <i>There's no gift catalog in database</i>
                                <br>
                            </div>
                        </EmptyDataTemplate>
                        <LayoutTemplate>
                              <div id="itmPlaceholder" runat="server"></div>
                        </LayoutTemplate>
                        <ItemTemplate>
                              <div class="field catalogRow">
                                <div class="single_line clearfix">
                                    <div class="line_label" style="width: 455px;">
                                        <%# Eval("Title") %>
                                    </div>
                                    <div class="line_input">
                                        <asp:TextBox ID="txtQuantity" runat="server" PlaceHolder="Quantity" MaxLength="4"  style="width: 70px; padding: 4px;" data-selectoptionid='<%# Eval("SelectOptionId") %>' data-price='<%# Eval("Amount") %>'  data-sku='<%# Eval("sku") %>' CssClass="giftcatalog"/>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                    <%-- [BEGIN] This is how to get gift SKU without being dependent on code-behind. Output to controls and a function will generate JSON obj --%>
                    <div class="premiumGiftHiddenDiv" style="display:none;">
                    <asp:Repeater id="rptPremium" runat="server">
                        <ItemTemplate>
                            <asp:TextBox runat="server" Value='<%# Eval("PremiumGiftId") %>'  CssClass="premiumgift"  />
                            <input type="hidden" value='<%# Eval("Title") %>' id='<%# "title_" + Eval("PremiumGiftId") %>' />
                            <input type="hidden" value='<%# Eval("MinDonationAmount") %>' id='<%# "min_" + Eval("PremiumGiftId") %>' />
                            <input type="hidden" value='<%# Eval("MaxDonationAmount") %>' id='<%# "max_" + Eval("PremiumGiftId") %>' />
                            <input type="hidden" value='<%# Eval("SKU") %>' id='<%# "sku_" + Eval("PremiumGiftId") %>' />
                        </ItemTemplate>
                    </asp:Repeater>
                    <%-- [END] This is how to get gift SKU without being dependent on code-behind. Output to controls and a function will generate JSON obj --%>
                    </div>
                    <div id="control_A41" class="section_control">
                        <input id="continue_A41" type="button" value="Continue >>" onclick="Validate_A41()" />
                        <input id="back_A3d" type="button" value="Back to A3" onclick="Return_A3d()" />   
                    </div>
                </div>
                <div runat="server" id="section_A42" class="section">
                    <h2 id="hcTitle3">[Catalog Title - Select Free Gift<span>A42</span></h2>
                    <div id="giftselection">
                        <div class="field_info">
                               Thank you for ordering a life changing gift from the American Red Cross. Your generosity will help save the day for people in need. Your donation of <span id="giftAmount" style="color:Blue; font-weight:bold;"></span> 
                                qualifies you for a free <span id="whichGift" style="color:Red;">gift</span> which will be mailed to the address provided within 4-6 weeks. If you would like to opt out of receiving your free gift please tell me now. 
                         </div>
                        <div class="field">
                            <div class="field_option">
                                <asp:RadioButtonList ID="rbWantsGift" runat="server">
                                    <asp:ListItem Value="Y">Wants gift</asp:ListItem>
                                    <asp:ListItem Value="N">Does NOT want gift </asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_info">
                           Thank you for giving a gift that means something this holiday season. Your donation to the Holiday Gift Catalog qualifies you for a free holiday printed greeting card for each of the products you ordered. You will also receive a corresponding insert for each product you ordered, which you may wish to enclose along with the card. The printed greeting card(s) and insert(s) will be mailed to you in 5-8 business days.  Once you receive the card(s) you can then add a personal message in the card before you send it to a loved one. You qualify for one printed greeting card and a corresponding product insert for each product you order.
                     </div>
                    <div class="field">
                        <div class="field_option">
                            <asp:RadioButtonList ID="rbWantsGiftCard" runat="server">
                                <asp:ListItem Value="Y">Wants Greeting Card</asp:ListItem>
                                <asp:ListItem Value="N">Does NOT want Greeting Card </asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div id="control_A42" class="section_control">
                        <input id="continue_A42" type="button" value="Continue >>" onclick="Validate_A42()" />
                        <input id="back_A41A" type="button" value="Back to A41" onclick="Return_A41A()" />
                    </div>
                </div>
                <div runat="server" id="section_A4" class="section">
                    <h2>Donor Information: Card Type<span>A4</span></h2>
                    <div class="field_info sectionA4_standard">
                        <div class="english">
                            We appreciate your support! So that we may put your donation to work immediately, we accept all major credit cards.
                        </div>
                        <div class="spanish">
                            Agradecemos su apoyo! Para que podamos poner su donaci&oacute;n a trabajar de inmediato, aceptamos todas las tarjetas de cr&eacute;dito.
                        </div>
                        <br />
                    </div>
                    <div class="field_info sectionA4_drtv">
                        <div class="english">
                            Every 8 minutes, someone is facing an unimaginable disaster.  In order to put your donation to work right away, we're asking everyone to put their gift on a credit card today.  Which credit card will you be using?
                        </div>
                        <div class="spanish">
                            Cada 8 minutos, alguien enfrenta un desastre inimaginable. Con el fin de poner su donación a trabajar de inmediato, les pedimos a todos que pongan su regalo en una tarjeta de crédito hoy. ¿Qué tarjeta de crédito usarás?
                        </div>
                        <br />
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Which credit card would you like to use?
                                </div>
                                <div class="spanish">
                                    &iquest;Qu&eacute; tarjeta de cr&eacute;dito le gustar&iacute;a usar?
                                </div>
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="rb4_card_type" runat="server">
                                    <asp:ListItem Value="CHECK" Text="CHECK <span class='step_label'>A5</span>" />
                                    <asp:ListItem Value="VISA" Text="VISA <span class='step_label'>A7</span>" />
                                    <asp:ListItem Value="MC" Text="MASTERCARD" />
                                    <asp:ListItem Value="AMEX" Text="AMERICAN EXPRESS" />
                                    <asp:ListItem Value="DC" Text="DISCOVER" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input" id="decline_A0Y">
                        <div class="agent_note">
                            [Agent] If the donor wants to change donation amount, use Back to A3 > Back to A2
                        </div>
                    </div>
                    <div id="control_A4" class="section_control">
                        <input id="continue_A4" type="button" value="Continue >>" onclick="Validate_A4()" />
                        <input id="back_A3A" type="button" value="Back to A3" onclick="Return_A3A()" />
                        <input id="back_A41B" type="button" value="Back to A41" onclick="Return_A41B()" />
                        <input id="back_A42A" type="button" value="Back to A42" onclick="Return_A42()" />
                        <input id="back_A0Y" type="button" value="Back to A0" onclick="Return_A0Y()" />
                        <asp:HiddenField ID="backA42x" runat="server" Value="hide" />
                        <asp:HiddenField ID="declineA0Y" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A5" class="section">
                    <h2>Check Donation<span>A5</span></h2>
                    <div class="field_info">
                        <span class="english">
                            Please make your check payable to the American Red Cross. We are really counting on your support, so please send your donation to the American Red Cross as soon as possible. Here is our address:
                        </span>
                        <span class="spanish">
                            Por favor haga su cheque a nombre de la Cruz Roja Americana. Estamos realmente contando con su apoyo, por favor envíe su donación a la Cruz Roja Americana tan pronto como sea posible. Aquí está nuestra dirección:
                        </span>
                    </div>
                    <div class="field_info bold">
                        American Red Cross
                        <br />P.O. Box 97089
                        <br />Washington, DC 20090-7089
                    </div>
                    <div id="control_A5" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A5" type="button" value="Continue >>" onclick="Validate_A5()" />
                        <input id="back_A4Y" type="button" value="Back to A4" onclick="Return_A4Y()" />
                        <input id="back_A3B" type="button" value="Back to A3" onclick="Return_A3B()" />
                    </div>
                </div>
                <div runat="server" id="section_A45" class="section">
                    <h2>Check Donation [DRTV]<span>A45</span></h2>
                    <div class="field_info">
                        <div class="english">
                            I would be happy to send you an envelope with some information so that you can mail back your check.  I just need to get some information from you.
                        </div>
                        <div class="spanish">
                            Con gusto le enviaré un sobre con cierta información para que pueda devolver su cheque por correo. Solo necesito obtener información de usted.
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="field_option">
                                <asp:RadioButtonList ID="tb45_sustainer_optin" runat="server">
                                    <asp:ListItem Value="YES_Sustainer" Text="YES: Wants to receive an envelope to become monthly Red Cross Champion (Sustainer) via mail " />
                                    <asp:ListItem Value="YES_OneTime" Text="YES: Wants to receive an envelope to mail in a one-time gift" />
                                    <asp:ListItem Value="NO" Text="NO ENVELOPE: Wants to mail in check now/Refused address Continue >> A48" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                May I please have your first name?
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb45_first_name" runat="server" PlaceHolder="First Name" MaxLength="50" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                And your last name?
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb45_last_name" runat="server" PlaceHolder="Last Name" MaxLength="50" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label agent_note">
                                Agent: If caller mentions a business name, enter it here.
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb45_business_name" runat="server" PlaceHolder="Business Name" MaxLength="100" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                How do you prefer to be addressed?
                            </div>
                            <div class="line_radio">
	                            <asp:DropDownList ID="tb45_prefix" runat="server" style="width: 125px;">
                                    <asp:ListItem Value="0" Text="NONE" />
                                    <asp:ListItem Value="1" Text="DR" />
                                    <asp:ListItem Value="2" Text="RABBI" />
                                    <asp:ListItem Value="3" Text="MR & MRS" />
                                    <asp:ListItem Value="4" Text="MISS" />
                                    <asp:ListItem Value="5" Text="MRS" />
                                    <asp:ListItem Value="6" Text="MS" />
                                    <asp:ListItem Value="7" Text="MR" />
                                    <asp:ListItem Value="8" Text="REV" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                May I please have your street address?
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb45_address1" runat="server" PlaceHolder="Address" MaxLength="100" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Is there a suite or apartment number?
                            </div>
                            <div class="line_input">
                                <asp:DropDownList ID="tb45_suite_type" runat="server" style="width: 125px;">
                                    <asp:ListItem Value="0" Text="NONE" />
                                    <asp:ListItem Value="1" Text="APT" />
                                    <asp:ListItem Value="2" Text="BLDG" />
                                    <asp:ListItem Value="3" Text="CONDO" />
                                    <asp:ListItem Value="4" Text="FLOOR" />
                                    <asp:ListItem Value="5" Text="HOUSE" />
                                    <asp:ListItem Value="6" Text="LOT" />
                                    <asp:ListItem Value="7" Text="SUITE" />
                                    <asp:ListItem Value="8" Text="UNIT" />
                                </asp:DropDownList>
                                <asp:TextBox ID="tb45_suite_number" runat="server" PlaceHolder="Number" MaxLength="50" Width="75px" />
                            </div>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="tb45_postal_code" EventName="TextChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        <div class="english">
                                            What is your country?
                                        </div>
                                        <div class="spanish">
                                            &iquest;Cu&aacute;l es su pa&iacute;s?
                                        </div>
                                        <div class="agent_note">
                                            Only ask if Country appears to not be United States.
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:DropDownList ID="tb45_country" runat="server" Width="225px" OnChange="return Country_Switch(this)" />                                
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        May I have your zip code please?
                                        <div class="agent_note">
                                            [Agent] press tab after entering zip code and the system will look up the city and state.
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:TextBox ID="tb45_postal_code" runat="server" PlaceHolder="Postal Code" MaxLength="10" Width="125px" OnTextChanged="RunZipEngine" AutoPostBack="true" />
                                        <asp:Label ID="lbltb45_postal_code" runat="server" Text="" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        Your city is
                                    </div>
                                    <div class="line_input required">
                                        <asp:TextBox ID="tb45_city" runat="server" PlaceHolder="City" MaxLength="20" Width="225px" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        Your state is
                                    </div>
                                    <div class="line_input required">
                                        <asp:DropDownList ID="tb45_state" runat="server" Width="225px" /><asp:DropDownList ID="tb45_stateca" runat="server" Width="225px" /><asp:TextBox ID="tb45_stateother" runat="server" PlaceHolder="State" MaxLength="25" Width="225px" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Your pledge amount?
                                <div class="agent_note">
                                    Agent: Confirm the amount the donor mentioned at the beginnig of the call, update if needed.
                                </div>
                            </div>
                            <div class="line_input">
                                $ <asp:TextBox ID="tb45_amount_dollar" runat="server" MaxLength="6" Width="75px" />.<asp:TextBox ID="tb45_amount_cent" runat="server" MaxLength="2" Width="50px" Text="00" />
                            </div>
                        </div>
                    </div>
                    <div class="field_info">
                        <div class="english">
                            Great, thank you! You can expect a reply envelope to arrive in the next 10 days.
                        </div>
                        <div class="spanish">
                            Bien, muchas gracias! Usted puede esperar un sobre de respuesta para llegar en breve.
                        </div>
                    </div>
                    <div id="control_A45" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A45" type="button" value="Continue >>" onclick="Validate_A45()" />
                        <input id="back_A4E" type="button" value="Back to A4" onclick="Return_A4E()" />
                        <input id="back_A0E" type="button" value="Back to A0" onclick="Return_A0E()" />
                        <input id="back_A48N" type="button" value="Back to A48" onclick="Return_A48N()" />
                    </div>
                </div>
                <div runat="server" id="section_A48" class="section">
                    <h2>Check Donation [DRTV] No Envelope<span>A48</span></h2>
                    <div class="field_info">
                        <div class="english">
                            I can send you a letter in the mail with a return envelope for your convenience.  I just need to get some information from you.
                        </div>
                        <div class="spanish">
                            I can send you a letter in the mail with a return envelope for your convenience.  I just need to get some information from you.
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="field_option">
                                <asp:RadioButtonList ID="tb48_envelope" runat="server">
                                    <asp:ListItem Value="YES" Text="YES" />
                                    <asp:ListItem Value="NO" Text="NO" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_info">
                        <div class="agent_note">
                            If NO:<br />
                        </div>
                        <div class="english">
                            If you prefer to mail in your donation without a return envelope, you most certainly can. We ask that you please make your check payable to: The American Red Cross.
                            <br /><br />Please send your donation to the American Red Cross at:
                        </div>
                        <div class="spanish">
                            If you prefer to mail in your donation without a return envelope, you most certainly can. We ask that you please make your check payable to: The American Red Cross.
                            <br /><br />Please send your donation to the American Red Cross at:
                        </div>
                    </div>
                    <div class="field_info bold">
                        American Red Cross
                        <br />P.O. Box 37841
                        <br />Boone, IA 50037-0841 
                    </div>
                    <div id="control_A48" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A48" type="button" value="Continue >>" onclick="Validate_A48()" />
                        <input id="back_A45N" type="button" value="Back to A45" onclick="Return_A45N()" />
                    </div>
                </div>
                <div runat="server" id="section_A49" class="section">
                    <h2>Additional Questions - Telethon<span>A49</span></h2>
                    <div class="field_info">
                        <div class="english">
                            This is an American Red Cross Disaster Telethon line.
                            <br />For questions please call 800 RED-CROSS (1-800-733-2767)
                            <br /><br /><span class="agent_note">Agent if Caller decides to still donate use Back to A1, otherwise use End Call and submit as 'Information Only'</span>
                        </div>
                        <div class="spanish">
                            Esta es una línea de teleton de desastre de la Cruz Roja Americana.
                            <br />Para preguntas por favor llame al 800 RED-CROSS (1-800-733-2767)
                            <br /><br /><span class="agent_note">Agent if Caller decides to still donate use Back to A1, otherwise use End Call and submit as 'Information Only'</span>
                        </div>
                    </div>
                    <div id="control_A49" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A49" type="button" value="End Call >>" onclick="Validate_A49()" />
                        <input id="back_A1A" type="button" value="Back to A1" onclick="Return_A1A()" />
                    </div>
                </div>

                <div runat="server" id="section_A6" class="section">
                    <h2>Special Cause Donation<span>A6</span></h2>
                    <div class="field_info">
                        Thank you for your support! We would be happy to assist you with placing your donation. I would like to connect you with a Donor Services Representative who can assist you in placing your donation to this Cause. Please hold one moment while I connect you.
                    </div>
                    <div class="field_info agent_note">
                        To transfer the caller to English Donor Services, press *7 wait for menu to begin then press 1.
                        <br />To transfer the caller to Spanish Donor Services, press *7 wait for menu to begin then press 2.
                        <br />To cancel the transfer and return to the caller, press 0.
                        <br />
                        <br />In case take back and transfer fails you can tell the caller to call 1-855-266-0723 for English or 855-266-0724 for Spanish.
                        <br />
                        <br />IF ASKED: Normal hours of operation are Monday - Friday 9:00 a.m. to 5:30 p.m. Eastern time (if after hours the donor may leave a message).
                    </div>
                    <div id="control_A6" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A6" type="button" value="Continue >>" onclick="Validate_A6()" />
                        <input id="back_A3c" type="button" value="Back to A3" onclick="Return_A3c()" />
                    </div>
                </div>
                <div runat="server" id="section_A7" class="section">
                    <h2>Donor Information: Card Data<span>A7</span></h2>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    May I please have your credit card number?
                                </div>
                                <div class="spanish">
                                    ¿Puedo por favor tener su número de tarjeta de crédito?
                                </div>
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb7_card_number" runat="server" PlaceHolder="Card Number" MaxLength="16" Width="150px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    And the expiration date?
                                </div>
                                <div class="spanish">
                                    ¿Y la fecha de vencimiento?
                                </div>
                            </div>
                            <div class="line_input">
	                            <asp:DropDownList ID="tb7_card_month" runat="server" style="width: 125px;margin-right:10px;">
                                    <asp:ListItem Value="" Text="Select Month" />
                                    <asp:ListItem Value="01" Text="January" />
                                    <asp:ListItem Value="02" Text="February" />
                                    <asp:ListItem Value="03" Text="March" />
                                    <asp:ListItem Value="04" Text="April" />
                                    <asp:ListItem Value="05" Text="May" />
                                    <asp:ListItem Value="06" Text="June" />
                                    <asp:ListItem Value="07" Text="July" />
                                    <asp:ListItem Value="08" Text="August" />
                                    <asp:ListItem Value="09" Text="September" />
                                    <asp:ListItem Value="10" Text="October" />
                                    <asp:ListItem Value="11" Text="November" />
                                    <asp:ListItem Value="12" Text="December" />                
                                </asp:DropDownList>
                                <asp:DropDownList ID="tb7_card_year" runat="server" style="width: 125px;margin-right:22px;">
                                    <asp:ListItem Value="" Text="Select Year" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    How exactly does the name appear on the card?
                                </div>
                                <div class="spanish">
                                    ¿Cómo aparece exactamente su nombre en la tarjeta?
                                </div>
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb7_first_name" runat="server" PlaceHolder="First Name" MaxLength="50" Width="125px" />
                                <br /><asp:TextBox ID="tb7_middle_initial" runat="server" PlaceHolder="Middle Initial" MaxLength="5" Width="125px" />
                                <br /><asp:TextBox ID="tb7_last_name" runat="server" PlaceHolder="Last Name" MaxLength="50" Width="125px" />
                            </div>
                        </div>
                    </div>
                    <div id="control_A7" class="section_control">
                        <span class="step_label">A8<br /></span>
                        <input id="continue_A7" type="button" value="Continue >>" onclick="Validate_A7()" />
                        <input id="back_A4N" type="button" value="Back to A4" onclick="Return_A4N()" />
                        <input id="back_A43Y" type="button" value="Back to A43" onclick="Return_A43Y()" />
                        <input id="back_A44Y" type="button" value="Back to A44" onclick="Return_A44Y()" />
                    </div>
                </div>
                <div runat="server" id="section_A8" class="section">
                    <h2>Donor Information: Address Data<span>A8</span></h2>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label agent_note">
                                Agent: If caller mentions a business name, enter it here.
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb8_business_name" runat="server" PlaceHolder="Business or Organization" MaxLength="50" Width="325px" />
                            </div>
                        </div>
                    </div>
                    <div id="A5_BusinessTgl" class="field_input" style="display: none;">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Are you calling on behalf of a business or organization?
                                </div>
                                <div class="spanish">
                                    &iquest;Est&aacute; llamando en nombre de una empresa u organizaci&oacute;n?
                                </div>
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="tb8_biz_toggle" runat="server">
                                    <asp:ListItem Value="YES" Text="YES" onclick="Toggle_Field_Form('A5_BusinessTgl','show');" class="required" />
                                    <asp:ListItem Value="NO" Text="NO" onclick="Toggle_Field_Form('A5_BusinessTgl','hide');" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    May I please have your billing street address? (billing address needed to complete donation)
                                </div>
                                <div class="spanish">
                                    Puedo por favor tenga su direcci&oacute;n de facturaci&oacute;n? (direcci&oacute;n de facturaci&oacute;n necesaria para completar la donaci&oacute;n)
                                </div>
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb8_address1" runat="server" PlaceHolder="Address" MaxLength="100" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Is there a suite or apartment number?
                                </div>
                                <div class="spanish">
                                    &iquest;Hay un n&uacute;mero suite o apartamento?
                                </div>
                            </div>
                            <div class="line_input">
	                            <asp:DropDownList ID="tb8_suite_type" runat="server" style="width: 125px;">
                                    <asp:ListItem Value="0" Text="NONE" />
                                    <asp:ListItem Value="1" Text="APT" />
                                    <asp:ListItem Value="2" Text="BLDG" />
                                    <asp:ListItem Value="3" Text="CONDO" />
                                    <asp:ListItem Value="4" Text="FLOOR" />
                                    <asp:ListItem Value="5" Text="HOUSE" />
                                    <asp:ListItem Value="6" Text="LOT" />
                                    <asp:ListItem Value="7" Text="SUITE" />
                                    <asp:ListItem Value="8" Text="UNIT" />
                                </asp:DropDownList>
                                <asp:TextBox ID="tb8_suite_number" runat="server" PlaceHolder="Number" MaxLength="25" Width="75px" />
                            </div>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="ZipEngine8" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="tb8_postal_code" EventName="TextChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        <div class="english">
                                            What is your country?
                                        </div>
                                        <div class="spanish">
                                            &iquest;Cu&aacute;l es su pa&iacute;s?
                                        </div>
                                        <div class="agent_note">
                                            Only ask if Country appears to not be United States.
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:DropDownList ID="tb8_country" runat="server" Width="225px" OnChange="return Country_Switch(this)" />                                
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        <div class="english">
                                            May I have your zip code please?
                                        </div>
                                        <div class="spanish">
                                            &iquest;Me puede dar su c&oacute;digo postal?
                                        </div>
                                        <div class="agent_note">
                                            [Agent] press tab after entering zip code and the system will look up the city and state.
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:TextBox ID="tb8_postal_code" runat="server" PlaceHolder="Postal Code" MaxLength="10" Width="125px" OnTextChanged="RunZipEngine" AutoPostBack="true" />
                                        <asp:Label ID="lbltb8_postal_code" runat="server" Text="" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        <div class="english">
                                            Your city is
                                        </div>
                                        <div class="spanish">
                                            Tu ciudad es
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:TextBox ID="tb8_city" runat="server" PlaceHolder="City" MaxLength="25" Width="225px" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        <div class="english">
                                            Your state is
                                        </div>
                                        <div class="spanish">
                                            Su estado es
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:DropDownList ID="tb8_state" runat="server" Width="225px" /><asp:DropDownList ID="tb8_stateca" runat="server" Width="225px" /><asp:TextBox ID="tb8_stateother" runat="server" PlaceHolder="State" MaxLength="25" Width="225px" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    What phone number would you like us to use if we need to call you about this donation?
                                </div>
                                <div class="spanish">
                                    &iquest;Qu&eacute; n&uacute;mero de tel&eacute;fono le gustar&iacute;a que usemos si tenemos que llamarte acerca de esta donaci&oacute;n?
                                </div>
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb8_phone" runat="server" PlaceHolder="Phone" MaxLength="15" Width="125px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Is this number a landline or mobile?
                                </div>
                                <div class="spanish">
                                    ¿Es este número un teléfono fijo o celular?
                                </div>
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="tb8_phone_type" runat="server">
                                    <asp:ListItem Value="H" Text="Home" />
                                    <asp:ListItem Value="M" Text="Mobile" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    May we use this phone number to contact you with future disaster alerts, preparedness tips, and other ways to get involved in the Red Cross?
                                </div>
                                <div class="spanish">
                                    Podemos utilizar este n&uacute;mero de tel&eacute;fono para contactarlo con alertas de futuros desastres, consejos de preparaci&oacute;n, y otras maneras de involucrarse en la Cruz Roja?
                                </div>
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="tb8_phone_optin" runat="server">
                                    <asp:ListItem Value="YES" Text="YES" />
                                    <asp:ListItem Value="NO" Text="NO" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input" id="field_input_A8_addPhone">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Is there another contact number you would like to give today?
                                </div>
                                <div class="spanish">
                                    &iquest;Hay otro n&uacute;mero de contacto al que le gustar&iacute;a dar hoy?
                                </div>
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="tb8_phone2_add" runat="server">
                                    <asp:ListItem Value="YES" Text="YES" onclick="Toggle_Field_Form('A5_Phone2','show');Toggle_Field_Form('A5_Phone2OptIn','show');" />
                                    <asp:ListItem Value="NO" Text="NO" onclick="Toggle_Field_Form('A5_Phone2','hide');Toggle_Field_Form('A5_Phone2OptIn','hide');" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div id="A5_Phone2" class="field_input" style="display: none;">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Additional Phone
                                </div>
                                <div class="spanish">
                                    Otro tel&eacute;fono
                                </div>
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb8_phone2" runat="server" PlaceHolder="Phone" MaxLength="15" Width="125px" />
                            </div>
                        </div>
                    </div>
                    <div id="A5_Phone2OptIn" class="field_input" style="display: none;">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Is this additional number a landline or mobile?
                                </div>
                                <div class="spanish">
                                    &iquest;Es este n&uacute;mero adicional de un tel&eacute;fono fijo o m&oacute;vil?
                                </div>
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="tb8_phone2_type" runat="server">
                                    <asp:ListItem Value="H" Text="Home" />
                                    <asp:ListItem Value="M" Text="Mobile" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input sectionA8_standard" id="field_input_A8_receiptEmail">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    <%--Would you like a copy of your receipt to be emailed to you today?--%>
                                    Would you prefer that we email you your receipt?
                                </div>
                                <div class="spanish">
                                    <%--&iquest;Quieres una copia de su recibo al recibir un correo electr&oacute;nico a usted hoy?--%>
                                    &iquest;Prefiere que le enviaremos por correo electr&oacute;nico el recibo?
                                </div>
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="tb8_email_receipt" runat="server">
                                    <asp:ListItem Value="YES" Text="YES" />
                                    <asp:ListItem Value="NO" Text="NO" Selected="True" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input sectionA8_standard" id="field_input_A8_receiptEmail2">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Email Address
                                </div>
                                <div class="spanish">
                                    Direcci&oacute;n de correo electr&oacute;nico
                                </div>
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb8_email" runat="server" PlaceHolder="Email" MaxLength="50" Width="325px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input" id="field_input_A8_addEmail">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Would you like to receive occasional updates from The American Red Cross via email?
                                </div>
                                <div class="spanish">
                                    &iquest;Te gustar&iacute;a recibir actualizaciones ocasionales de La Cruz Roja Americana por correo electr&oacute;nico?
                                </div>
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="tb8_email_optin" runat="server">
                                    <asp:ListItem Value="YES" Text="YES" onclick="DuplicateValue('tb8_email','tb8_email2')" />
                                    <asp:ListItem Value="NO" Text="NO" onclick="ClearValue('tb8_email2')" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input" id="field_input_A8_addEmail2">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Email Address
                                </div>
                                <div class="spanish">
                                    Direcci&oacute;n de correo electr&oacute;nico
                                </div>
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb8_email2" runat="server" PlaceHolder="Email" MaxLength="50" Width="325px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input" id="field_input_A8_altadd">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Is the billing address you provided the same as your shipping address?
                                </div>
                                <div class="spanish">
                                    ¿La dirección de facturación que proporcionó es la misma que su dirección de envío?
                                </div>
                                <div class="agent_note">
                                    Agent: Selecting 'No' will allow for an alternate shipping address.
                                </div>
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="tb8_alt_address" runat="server">
                                    <asp:ListItem Value="NO" Text="YES" />
                                    <asp:ListItem Value="YES" Text="NO" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div id="control_A8" class="section_control">
                        <span class="step_label">A9<br /></span>
                        <input id="continue_A8" type="button" value="Continue >>" onclick="Validate_A8()" />
                        <input id="back_A7Y" type="button" value="Back to A7" onclick="Return_A7Y()" />
                    </div>
                </div>
                <div runat="server" id="section_A9" class="section">
                    <h2>Donation Verification<span>A9</span></h2>
                    <div id="donation_onetime" class="field_info">
                        <div class="field" id="confirmation_01_template" style="display: none;">
                            <div class="english">
                                Mr/Mrs <b>{donor_name}</b>, this is to confirm your tax-deductible donation in the amount of $<b>{donation_amount}</b> to <b>{designation_name}</b> designation.
                            </div>
                            <div class="spanish">
                                Sr/Sra <b>{donor_name}</b>, esto es para confirmar su donaci&oacute;n deducible de impuestos por un monto de $<b>{donation_amount}</b> a <b>{designation_name_spanish}</b> designaci&oacute;n.
                            </div>
                        </div>
                        <div class="field" id="confirmation_01" runat="server">
                            <div class="english">
                                Mr/Mrs <b>{donor_name}</b>, this is to confirm your tax-deductible donation in the amount of $<b>{donation_amount}</b> to <b>{designation_name}</b> designation.
                            </div>
                            <div class="spanish">
                                Sr/Sra <b>{donor_name}</b>, esto es para confirmar su donaci&oacute;n deducible de impuestos por un monto de $<b>{donation_amount}</b> a <b>{designation_name_spanish}</b> designaci&oacute;n.
                            </div>
                        </div>
                    </div>
                    <div id="donation_sustainer">
                        <div class="field" id="confirmation_02_template" style="display: none;">
                            <div class="english">
                                Mr/Mrs <b>{donor_name}</b>, this is to confirm your tax-deductible donation in the amount of $<b>{donation_amount}</b> to <b>{designation_name}</b> designation.
                                <br /><br />
                                We have also initiated your monthly gift as a Red Cross Champion, so you will see a charge from the American Red Cross in the amount of <b>${donation_amount}</b> on the <b>{sustainer_date}</b> of next month and each month thereafter.
                                <div id="sectionA9_drtv_template">
                                    You can expect to receive a welcome letter and pin  in the mail. The welcome letterdetails about the impact of your role as a Red Cross Champion.
                                </div>
                                <br />You will be mailed an acknowledgment once yearly, summarizing your monthly gifts. Will that be OK? 
                            </div>
                            <div class="spanish">
                                Sr/Sra <b>{donor_name}</b>, esto es para confirmar su donaci&oacute;n deducible de impuestos por un monto de $<b>{donation_amount}</b> a <b>{designation_name_spanish}</b> designaci&oacute;n.
                                <br /><br />
                                Tambi&eacute;n hemos iniciado su regalo mensual como un campe&oacute;n de la Cruz Roja, por lo que ver&aacute; un cargo de la Cruz Roja de Estados Unidos por un monto de $ <b>${donation_amount}</b> del <b>{sustainer_date}</b> del pr&oacute;ximo mes y cada mes a partir de entonces.
                                <br />Se le enviar&aacute; un acuse de recibo una vez al a&ntilde;o, que resume sus regalos mensuales. &iquest;Ser&aacute; eso est&aacute; bien?
                            </div>
                        </div>
                        <div class="field" id="confirmation_02">
                            <div class="english">
                                Mr/Mrs <b>{donor_name}</b>, this is to confirm your tax-deductible donation in the amount of $<b>{donation_amount}</b> to <b>{designation_name}</b> designation.
                                <br /><br />
                                We have also initiated your monthly gift as a Red Cross Champion, so you will see a charge from the American Red Cross in the amount of <b>${donation_amount}</b> on the <b>{sustainer_date}</b> of next month and each month thereafter.
                                <div id="sectionA9_drtv">
                                    You can expect to receive a welcome letter and pin  in the mail. The welcome letterdetails about the impact of your role as a Red Cross Champion.
                                </div>
                                <br />You will be mailed an acknowledgment once yearly, summarizing your monthly gifts. Will that be OK? 
                            </div>
                            <div class="spanish">
                                Sr/Sra <b>{donor_name}</b>, esto es para confirmar su donaci&oacute;n deducible de impuestos por un monto de $<b>{donation_amount}</b> a <b>{designation_name_spanish}</b> designaci&oacute;n.
                                <br /><br />
                                Tambi&eacute;n hemos iniciado su regalo mensual como un campe&oacute;n de la Cruz Roja, por lo que ver&aacute; un cargo de la Cruz Roja de Estados Unidos por un monto de $ <b>${donation_amount}</b> del <b>{sustainer_date}</b> del pr&oacute;ximo mes y cada mes a partir de entonces.
                                <br />Se le enviar&aacute; un acuse de recibo una vez al a&ntilde;o, que resume sus regalos mensuales. &iquest;Ser&aacute; eso est&aacute; bien?
                            </div>
                        </div>
                        <div class="field">
                            <div class="field_option">
                                <asp:RadioButtonList ID="tb9_drtv_receipt_mode" runat="server">
                                    <asp:ListItem Value="YEARLY">YES<span class="step_label">A96</span></asp:ListItem>
                                    <asp:ListItem Value="MONTHLY">NO<span class="step_label">A96</span></asp:ListItem>
                                </asp:RadioButtonList>
                                <span class="agent_note">[AGENT] If caller is not OK with yearly, you may offer monthly]<br />"If you prefer to receive a receipt for each donation, I will request that we send you a receipt each month instead."</span>
                                <asp:HiddenField id="rdReceiptFrequency" runat="server" value="" />
                            </div>
                        </div>
                    </div>
                    <div id="storedvalue_A9" style="display: none;"></div>
                    <div id="control_A9" class="section_control">
                        <span class="step_label">A96<br /></span>
                        <input id="continue_A9" type="button" value="Continue >>" onclick="Validate_A9()" />
                        <input id="back_A8Y" type="button" value="Back to A8" onclick="Return_A8Y()" />
                        <input id="back_A47Y" type="button" value="Back to A47" onclick="Return_A47Y()" />
                    </div>
                </div>
                <div runat="server" id="section_A10" class="section">
                    <h2>Donation Confirmation<span>A10</span></h2>
                    <div class="field_info">
                        <div id="dConfirmation" runat="server">
                            <div class="english">
                                <div class="agent_note" style="{confirmation_details}">
                                    <br />Confirmation Number: {confirmation_number}
                                    <br />
                                </div>
                                <div style="{confirmation_onetime}">
                                    <br />Thank you for your donation. Is there anything else that I can do for you today?
                                    <br />
                                </div>
                                <div style="{confirmation_sustainer}">
                                    <br />Thank you for your donation. Is there anything else that I can do for you today?
                                    <br />
                                </div>
                                <div style="{confirmation_mail}">
                                    <br />You will receive a receipt for your donation by mail within five to seven business days.
                                    <br />
                                </div>
                                <div style="{confirmation_email}">
                                    <br />You have elected to also receive an emailed receipt, one will be sent within one business day.
                                    <br />
                                </div>
                                <div style="{acknowledgement_standard}">
                                    <br />On behalf of those we serve, thank you for standing with us.
                                    <br />
                                    <br />We appreciate your support!!
                                    <br />
                                    <br />Good Bye.
                                </div>
                                <div style="{acknowledgement_peru}">
                                    <br />The American Red Cross is grateful for your generous gift to support people affected by floods in Peru. The humanitarian efforts of the Red Cross provide comfort and hope to many in their time of need.
                                    <br />
                                    <br />On behalf of those we serve, thank you.
                                    <br />
                                    <br />Good Bye.
                                </div>
                                <div style="{acknowledgement_phonebank_univision}">
                                    <br />On behalf of those we serve, thank you for standing with us.
                                    <br />
                                    <br />Our partners at the Univision phone bank would like to say a special thank you, would you like me to transfer you?
                                    <br />
                                    <br /><span class="agent_note">Agent - if Yes: </span>
                                    <br />I'll transfer you now and Univision will answer as many calls as possible.
                                    <br />
                                    <br />We appreciate your support!!
                                    <br />
                                    <br />Good Bye.
                                    <br /><span class="agent_note">Agent - Transfer caller using your [TRANSFER] button, that will end this call.</span>
                                    <br />
                                    <br /><span class="agent_note">Agent - if No: </span>
                                    <br />
                                    <br />We appreciate your support!!
                                    <br />
                                    <br />Good Bye.
                                </div>
                            </div>
                            <div class="spanish">
                                <div class="agent_note" style="{confirmation_details}">
                                    <br />N&uacute;mero de confirmaci&oacute;n: {confirmation_number}
                                    <br />
                                </div>
                                <div style="{confirmation_onetime}">
                                    <br />Gracias por tu donación. ¿Hay algo más que pueda hacer por usted hoy?
                                    <br />
                                </div>
                                <div style="{confirmation_sustainer}">
                                    <br />Gracias por tu donación. ¿Hay algo más que pueda hacer por usted hoy?
                                    <br />
                                </div>
                                <div style="{confirmation_mail}">
                                    <br />Usted recibirá un recibo por su donación por correo dentro de cinco a siete días laborales.
                                    <br />
                                </div>
                                <div style="{confirmation_email}">
                                    <br />Usted ha elegido recibir tambi&eacute;n un recibo enviado por correo electr&oacute;nico, se le envi&oacute; el plazo de un d&iacute;a h&aacute;bil.
                                    <br />
                                </div>
                                <div style="{acknowledgement_standard}">
                                    <br />En nombre de aquellos a quienes servimos, gracias por estar con nosotros.
                                    <br />
                                    <br />&iexcl;&iexcl;Gracias por su apoyo!!
                                    <br />
                                    <br />Adi&oacute;s.
                                </div>
                                <div style="{acknowledgement_peru}">
                                    <br />La Cruz Roja Americana agradece su donación para ayudar a las víctimas de las inundaciones en Perú. Los esfuerzos humanitarios de la Cruz Roja llevan consuelo y esperanza a muchos cuando más lo necesitan.
                                    <br />
                                    <br />&iexcl;&iexcl;En nombre de las personas a quienes servimos, le damos las gracias.!!
                                    <br />
                                    <br />Adi&oacute;s.
                                </div>
                                <div style="{acknowledgement_phonebank_univision}">
                                    <br />En nombre de aquellos a quienes servimos, gracias por estar con nosotros.
                                    <br />
                                    <br />Nuestros compañeros en el banco de telefono de Univision gustarian decir un agradecimiento especial, gustaria que lo transfiera?
                                    <br />
                                    <br /><span class="agent_note">Agent - if Yes: </span>
                                    <br />Te transferire ahora y Univision contestara tantas llamadas como sea possible.
                                    <br />
                                    <br />&iexcl;&iexcl;Gracias por su apoyo!!
                                    <br />
                                    <br />Adi&oacute;s.
                                    <br /><span class="agent_note">Agent - Transfer caller using your [TRANSFER] button, that will end this call.</span>
                                    <br />
                                    <br /><span class="agent_note">Agent - if No: </span>
                                    <br />
                                    <br />&iexcl;&iexcl;Gracias por su apoyo!!
                                    <br />
                                    <br />Adi&oacute;s.
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="control_A10" class="section_control">
                        <span class="step_label">A</span>
                        <input id="continue_A10" type="button" value="Continue >>" onclick="Validate_A10()" />
                    </div>
                </div>
                <div runat="server" id="section_A20" class="section">
                    <h2>Other Questions<span>A20</span></h2>
                    <div class="field_info">
                        How may I help you?
                    </div>
                    <div class="field">
                        <div class="field_radio">
                            <asp:RadioButtonList ID="RadioButtonList20" runat="server">
                                <asp:ListItem Text="WANTS TO DONATE BLOOD/BLOOD QUESTION <span class='step_label'>A21</span>" Value="21" />
                                <asp:ListItem Text="WANTS TO VOLUNTEER <span class='step_label'>A22</span>" Value="22" />
                                <asp:ListItem Text="INFO ABOUT INTERNATIONAL SERVICES <span class='step_label'>A23</span>" Value="23" />
                                <asp:ListItem Text="WANTS TO DONATE GOODS <span class='step_label'>A24</span>" Value="24" />
                                <asp:ListItem Text="WANTS TO SPONSOR AN EVENT <span class='step_label'>A28</span>" Value="28" />
                                <asp:ListItem Text="WANTS RECEIPT/INFO ABOUT DONATION <span class='step_label'>A29</span>" Value="29" />
                                <asp:ListItem Text="INFORMATION ABOUT RED CROSS <span class='step_label'>A30</span>" Value="30" />
                                <asp:ListItem Text="WANTS LOCAL RED CROSS <span class='step_label'>A31</span>" Value="31" />
                                <asp:ListItem Text="WANTS/NEEDS HELP <span class='step_label'>A32</span>" Value="32" />
                                <asp:ListItem Text="WANTS TO LOCATE MILITARY PERSONNEL <span class='step_label'>A33</span>" Value="33" />
                                <asp:ListItem Text="MEDIA INQUIRY <span class='step_label'>A34</span>" Value="34" />
                                <asp:ListItem Text="WANTS COURSE/CLASS INFORMATION <span class='step_label'>A35</span>" Value="35" />
                                <asp:ListItem Text="WANT INFORMATION ON VEHICLE DONATION <span class='step_label'>A36</span>" Value="36" />
                                <asp:ListItem Text="WANTS INFORMATION ABOUT THE USE OF FUNDS <span class='step_label'>A37</span>" Value="37" />
                                <asp:ListItem Text="WANTS EXPLANATION OF DIFFERENT FUNDS <span class='step_label'>A38</span>" Value="38" />
                                <asp:ListItem Text="PLANNED GIFT/WILL/BEQUEST/STOCK GIFTS/ANNUITIES <span class='step_label'>A39</span>" Value="39" />
                                <asp:ListItem Text="WANTS TO BE REMOVED FROM THE MAILING LIST <span class='step_label'>A40</span>" Value="40" />
                                <asp:ListItem Text="WANTS MORE SUSTAINER INFORMATION <span class='step_label'>A46</span>" Value="46" />
                                <asp:ListItem Text="DONATION <span class='step_label'>A1</span>" Value="1"></asp:ListItem>
                                <asp:ListItem Text="END CALL <span class='step_label'>End Call</span>" Value="96" />
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div id="control_A20" class="section_control">
                        <input id="continue_A20" type="button" value="Continue >>" onclick="Validate_A20()" />
                        <input id="back_A1N" type="button" value="Back to A1" onclick="Return_A1N()" />
                    </div>
                </div>
                <div runat="server" id="section_A21" class="section">
                    <h2>WANTS TO DONATE BLOOD/BLOOD QUESTION<span>A21</span></h2>
                    <div class="field_info">
                        Thank you for your interest in donating blood.  Every 2 seconds someone is in need of life saving blood.  Let me give you the dedicated 1-800 number for Red Cross Blood Services. They will be able to help you locate the nearest Blood Donation Center and can answer any questions you may have about Red Cross Blood Services.
                        <br />
                        <div class="agent_note">
                            <br />To transfer the caller to Blood Donation Services, press *7 wait for menu to begin then press 4.
                            <br />To cancel the transfer and return to the caller, press 0.
                            <br />
                            <br />In case take back and transfer fails you can tell the caller to call 1-800-GIVE-LIFE (1-800-448-3543)
                        </div>
                        <br />You can also visit redcrossblood.org to schedule a donation appointment.
                        <div class="agent_note">
                            IF ASKED: 1-800-GIVE-LIFE connects the caller with the Red Cross Blood Center nearest to them. If there is not a Red Cross Blood Center near the caller, a recorded message will convey further instructions. Hours of operation vary by Red Cross Blood Center.
                        </div>
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                    </div>
                    <div id="control_A21" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A21" type="button" value="Continue >>" onclick="Validate_A21('21')" />
                        <input id="back_A20_21" type="button" value="Back to A20" onclick="Return_A20('21')" />
                        <asp:HiddenField ID="backA20_21" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A22" class="section">
                    <h2>WANTS TO VOLUNTEER<span>A22</span></h2>
                    <div class="field_info">
                        There are many ways you can volunteer with us in your local community. Let me give you the phone number to your local Red Cross. We encourage you to contact your local Red Cross directly regarding local volunteer opportunities. You can also visit redcross.org/volunteer to search for volunteer opportunities both locally and internationally.  You can also download the Team Red Cross: Volunteer App to your smart phone to look for opportunities to join the Red Cross.
                    </div>
                    <asp:UpdatePanel ID="ChapterEngine22" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="TextBox22" EventName="TextChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        May I have your zip code please?
                                        <div class="agent_note">
                                            [Agent] press tab after entering zip code and the system will look up the nearest chapter.
                                        </div>
                                    </div>
                                    <div class="line_input">
                                        <asp:TextBox ID="TextBox22" runat="server" PlaceHolder="Postal Code" MaxLength="10" Width="125px" OnTextChanged="RunChapterEngine" AutoPostBack="true" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_info">
                                <div class="agent_note">
                                    IF ASKED: Hours of operation vary by local chapter.
                                    <br />IF ASKED: Team Red Cross: Volunteer App is currently only available through iTunes or the Google Play app stores. 
                                </div>
                                <br />Great, according to your zip code the phone number for your local Red Cross is:
                                <div class="agent_note" style="margin-left: 10px;">
                                    <asp:Label ID="lblTextBox22" runat="server" Text="{Enter a valid Postal Code and tab out of that field}" />
                                </div>
                                <br />They will be able to answer any questions you may have about programs or services in your area.
                                <br />
                                <div class="english">
                                    <br />Thank you for your support!!
                                    <br />
                                    <br />Good Bye.
                                </div>
                                <div class="spanish">
                                    <br />¡¡Gracias por tu apoyo!!
                                    <br />
                                    <br />Adiós.
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div id="control_A22" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A22" type="button" value="Continue >>" onclick="Validate_A22('22')" />
                        <input id="back_A20_22" type="button" value="Back to A20" onclick="Return_A20('22')" />
                        <asp:HiddenField ID="backA20_22" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A23" class="section">
                    <h2>INFO ABOUT INTERNATIONAL SERVICES<span>A23</span></h2>
                    <div class="field_info">
                        The American Red Cross responds to international disasters by sending financial assistance, pre-positioned relief supplies from our global warehouses, trained emergency response personnel, or any combination of the three. For more information on our International services, please visit redcross.org/support/international-support.
                        <br />
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                    </div>
                    <div id="control_A23" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A23" type="button" value="Continue >>" onclick="Validate_A21('23')" />
                        <input id="back_A20_23" type="button" value="Back to A20" onclick="Return_A20('23')" />
                        <asp:HiddenField ID="backA20_23" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A24" class="section">
                    <h2>WANTS TO DONATE GOODS<span>A24</span></h2>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_radio">
                                <asp:RadioButtonList ID="RadioButtonList24" runat="server">
                                    <asp:ListItem Value="USED">USED GOODS<span class="step_label">A25</span></asp:ListItem>
                                    <asp:ListItem Value="NEW">NEW BULK GOODS<span class="step_label">A26</span></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div id="control_A24" class="section_control">
                        <input id="continue_A24" type="button" value="Continue >>" onclick="Validate_A24()" />
                        <input id="back_A20_24" type="button" value="Back to A20" onclick="Return_A20('24')" />
                        <asp:HiddenField ID="backA20_24" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A25" class="section">
                    <h2>WANTS TO DONATE GOODS [USED GOODS]<span>A25</span></h2>
                    <div class="field_info">
                        <span class="bold">USED GOODS</span>
                        <br />The American Red Cross does not accept or solicit small quantities of individual donations of items for emergency relief purposes. Items such as food, used clothing, and shoes often must be cleaned, sorted, and repackaged.  The American Red Cross does not have enough staff, volunteers or funds for these activities during relief operations.
                        <br />
                        <br />The Red Cross, in partnership with other agencies, suggests that the best use for those types of donations is to support needy agencies within donors' local communities (such as food banks or places of worship).
                        <br />The best way to help people affected by disasters is through a financial donation to the Red Cross.
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Would you like to make a financial donation today?
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="RadioButtonList25" runat="server">
                                    <asp:ListItem Value="YES">YES<span class="step_label">A1</span></asp:ListItem>
                                    <asp:ListItem Value="NO">NO<span class="step_label">A27</span></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div id="control_A25" class="section_control">
                        <input id="continue_A25" type="button" value="Continue >>" onclick="Validate_A25()" />
                        <input id="back_A24Y" type="button" value="Back to A24" onclick="Return_A24Y()" />
                    </div>
                </div>
                <div runat="server" id="section_A26" class="section">
                    <h2>WANTS TO DONATE GOODS [NEW BULK GOODS]<span>A26</span></h2>
                    <div class="field_info">
                        <span class="bold">NEW BULK GOODS</span>
                        <br />The Red Cross does work with corporate partners to accept non-household items, based on need. These donations are in very large quantities of uniform bulk donations, in proper packaging. For more information, please visit redcross.org and complete the form under the Donating Goods section.
                    </div>
                    <div id="control_A26" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A26" type="button" value="Continue >>" onclick="Validate_A21('26')" />
                        <input id="back_A24N" type="button" value="Back to A24" onclick="Return_A24N()" />
                    </div>
                </div>
                <div runat="server" id="section_A27" class="section">
                    <h2>WANTS TO DONATE GOODS [USED GOODS]<span>A27</span></h2>
                    <div class="field_info">
                        I understand, again I suggest that you contact local agencies in your area.  Perhaps they can be of assistance.
                        <div class="agent_note">
                            if caller is persistent about donating item or not knowing who might be able to utilize their gift locally refer the caller to their local chapter and proceed with script.
                        </div>
                    </div>
                    <asp:UpdatePanel ID="ChapterEngine27" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="TextBox27" EventName="TextChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        May I have your zip code please?
                                        <div class="agent_note">
                                            [Agent] press tab after entering zip code and the system will look up the nearest chapter.
                                        </div>
                                    </div>
                                    <div class="line_input">
                                        <asp:TextBox ID="TextBox27" runat="server" PlaceHolder="Postal Code" MaxLength="10" Width="125px" OnTextChanged="RunChapterEngine" AutoPostBack="true" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_info">
                                <br />Let me provide you with the number for your local Red Cross, they may be able to refer you to local agencies in your area that can accept these types of gifts. According to your zip code the phone number for your Red Cross is:
                                <div class="agent_note" style="margin-left: 10px;">
                                    <asp:Label ID="lblTextBox27" runat="server" Text="{Enter a valid Postal Code and tab out of that field}" />
                                </div>
                                <div class="agent_note">
                                    <br />IF DONOR CONTINUES TO BE PERSISTANT OR SAYS THEY HAVE ALREADY SPOKEN TO THEIR LOCAL CHAPTER, PLEASE REFER THEM TO PUBLIC INQUIRY 1-202-303-4498
                                </div>
                                <div class="english">
                                    <br />Thank you for your support!!
                                    <br />
                                    <br />Good Bye.
                                </div>
                                <div class="spanish">
                                    <br />¡¡Gracias por tu apoyo!!
                                    <br />
                                    <br />Adiós.
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div id="control_A27" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A27" type="button" value="Continue >>" onclick="Validate_A22('27')" />
                        <input id="back_A25N" type="button" value="Back to A25" onclick="Return_A25N()" />
                    </div>
                </div>
                <div runat="server" id="section_A28" class="section">
                    <h2>WANTS TO SPONSOR AN EVENT<span>A28</span></h2>
                    <div class="field_info">
                        Let me refer you to your local Red Cross. They will be able to best assist you.
                        <br />You can also visit redcross.org and read the Ways to Fundraise page under the Ways to Help section. 
                    </div>
                    <asp:UpdatePanel ID="ChapterEngine28" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="TextBox28" EventName="TextChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        May I have your zip code please?
                                        <div class="agent_note">
                                            [Agent] press tab after entering zip code and the system will look up the nearest chapter.
                                        </div>
                                    </div>
                                    <div class="line_input">
                                        <asp:TextBox ID="TextBox28" runat="server" PlaceHolder="Postal Code" MaxLength="10" Width="125px" OnTextChanged="RunChapterEngine" AutoPostBack="true" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_info">
                                <div class="agent_note">
                                    IF ASKED: Hours of operation vary by local chapter.
                                </div>
                                <br />Great, according to your zip code the phone number for your local Red Cross is:
                                <div class="agent_note" style="margin-left: 10px;">
                                    <asp:Label ID="lblTextBox28" runat="server" Text="{Enter a valid Postal Code and tab out of that field}" />
                                </div>
                                <br />They will be able to answer any questions you may have about programs or services in your area.
                                <br />
                                <div class="english">
                                    <br />Thank you for your support!!
                                    <br />
                                    <br />Good Bye.
                                </div>
                                <div class="spanish">
                                    <br />¡¡Gracias por tu apoyo!!
                                    <br />
                                    <br />Adiós.
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div id="control_A28" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A28" type="button" value="Continue >>" onclick="Validate_A22('28')" />
                        <input id="back_A20_28" type="button" value="Back to A20" onclick="Return_A20('28')" />
                        <asp:HiddenField ID="backA20_28" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A29" class="section">
                    <h2>WANTS RECEIPT/INFO ABOUT DONATION<span>A29</span></h2>
                    <div class="field_info">
                        You've reached the Financial Donation Line, but I'd be happy to transfer you to our Service and Support Line. Please hold while I transfer you.
                        <div class="agent_note">
                            <br />To transfer the caller to English Donor Services, press *7 wait for menu to begin then press 1.
                            <br />To transfer the caller to Spanish Donor Services, press *7 wait for menu to begin then press 2.
                            <br />To cancel the transfer and return to the caller, press 0.
                            <br />
                            <br />In case take back and transfer fails you can tell the caller to call 1-855-266-0723 for English or 855-266-0724 for Spanish.
                            <br />IF ASKED: Normal hours of operation are Monday - Friday 9:00 a.m. to 5:30 p.m. Eastern time (if after hours the donor may leave a message).
                        </div>
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                    </div>
                    <div id="control_A29" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A29" type="button" value="Continue >>" onclick="Validate_A21('29')" />
                        <input id="back_A20_29" type="button" value="Back to A20" onclick="Return_A20('29')" />
                        <asp:HiddenField ID="backA20_29" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A30" class="section">
                    <h2>INFORMATION ABOUT RED CROSS<span>A30</span></h2>
                    <div class="field_info">
                        American Red Cross
                        <br />The American Red Cross exists to provide compassionate care to those in need. Our network of generous donors, volunteers and employees share a mission of preventing and relieving suffering, here at home and around the world, through five key service areas: Disaster Relief, Supporting America's Military Families, Lifesaving Blood, Health and Safety Services, and International Services.
                        <br />
                        <br />For more information please visit redcross.org and look for the What We Do section.
                        <div class="agent_note">
                            <br />IF CALLER REQUESTS ADDITIONAL INFORMATION ABOUT THE ARC REFER TO American Red Cross Public Inquiry Department: 202-303-4498
                            <br />IF ASKED: Normal hours of operation are Monday - Friday (8:00 a.m. to 4:30 p.m. Eastern time) if after hours please leave a message and someone will contact you.
                        </div>
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                    </div>
                    <div id="control_A30" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A30" type="button" value="Continue >>" onclick="Validate_A21('30')" />
                        <input id="back_A20_30" type="button" value="Back to A20" onclick="Return_A20('30')" />
                        <asp:HiddenField ID="backA20_30" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A31" class="section">
                    <h2>WANTS LOCAL RED CROSS<span>A31</span></h2>
                    <asp:UpdatePanel ID="ChapterEngine31" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="TextBox31" EventName="TextChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        May I have your zip code please?
                                        <div class="agent_note">
                                            [Agent] press tab after entering zip code and the system will look up the nearest chapter.
                                        </div>
                                    </div>
                                    <div class="line_input">
                                        <asp:TextBox ID="TextBox31" runat="server" PlaceHolder="Postal Code" MaxLength="10" Width="125px" OnTextChanged="RunChapterEngine" AutoPostBack="true" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_info">
                                <div class="agent_note">
                                    IF ASKED: Hours of operation vary by local chapter.
                                </div>
                                <br />Great, according to your zip code the phone number for your local Red Cross is:
                                <div class="agent_note" style="margin-left: 10px;">
                                    <asp:Label ID="lblTextBox31" runat="server" Text="{Enter a valid Postal Code and tab out of that field}" />
                                </div>
                                <br />They will be able to answer any questions you may have about programs or services in your area.
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="field_info">
                        <div class="agent_note">
                            IF CALLER WANTS INFO ON LOCAL CHAPTER:
                        </div>
                        Your local Red Cross chapter is committed to meeting the humanitarian needs of the people in your local community, be it in disaster preparedness, disaster relief, first aid or CPR training, or disease prevention.                        
                        <br />
                        <br />
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                    </div>
                    <div id="control_A31" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A31" type="button" value="Continue >>" onclick="Validate_A22('31')" />
                        <input id="back_A20_31" type="button" value="Back to A20" onclick="Return_A20('31')" />
                        <asp:HiddenField ID="backA20_31" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A32" class="section">
                    <h2>WANTS/NEEDS HELP<span>A32</span></h2>
                    <div class="field_info">
                        I'm sorry to hear about your recent troubles.  You have reached our national donation line. To get information on relief and assistance you'll need to speak with a Disaster Assistance representative - let me connect you back to our donor line and please follow the menu for disaster assistance. They will be able to assist you in getting the help you need.
                        <br />You can also visit redcross.org/find-help to get assistance.
                        <div class="agent_note">
                            <br />To transfer the caller to 1-800-RED-CROSS Disaster Assistance, press *7 wait for menu to begin then press 6.
                            <br />To cancel the transfer and return to the caller, press 0.
                            <br />
                            <br />In case take back and transfer fails you can tell the caller to call 800-733-2767
                        </div>
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                    </div>
                    <div id="control_A32" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A32" type="button" value="Continue >>" onclick="Validate_A21('32')" />
                        <input id="back_A20_32" type="button" value="Back to A20" onclick="Return_A20('32')" />
                        <asp:HiddenField ID="backA20_32" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A33" class="section">
                    <h2>WANTS TO LOCATE MILITARY PERSONNEL<span>A33</span></h2>
                    <div class="field_info">
                        You have reached our financial donation line. For the type of assistance you are requesting, you will need to speak with Services to the Armed Forces. I will transfer you now.
                        <br />You can also visit redcross.org/find-help and look for the Military Family Services section. 
                        <div class="agent_note">
                            <br />To transfer the caller to Services to the Armed Forces, press *7 wait for menu to begin then press 3
                            <br />To cancel the transfer and return to the caller, press 0.
                            <br />
                            <br />In case take back and transfer fails you can tell the caller to call 877-272-7337
                        </div>
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                    </div>
                    <div id="control_A33" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A33" type="button" value="Continue >>" onclick="Validate_A21('33')" />
                        <input id="back_A20_33" type="button" value="Back to A20" onclick="Return_A20('33')" />
                        <asp:HiddenField ID="backA20_33" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A34" class="section">
                    <h2>MEDIA INQUIRY<span>A34</span></h2>
                    <div class="field_info">
                        We are not prepared to answer questions from the media at this phone number. For updates you can visit our website at www.redcross.org.
                        <div class="agent_note">
                            IF DONOR CONTINUES TO BE PERSISTANT, PLEASE REFER THEM TO THE PUBLIC INQUIRY DEPT. at 1-202-303-4498
                        </div>
                        <br />Thank you!
                        <br />
                        <br />Good Bye.

                    </div>
                    <div id="control_A34" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A34" type="button" value="Continue >>" onclick="Validate_A21('34')" />
                        <input id="back_A20_34" type="button" value="Back to A20" onclick="Return_A20('34')" />
                        <asp:HiddenField ID="backA20_34" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A35" class="section">
                    <h2>WANTS COURSE/CLASS INFORMATION<span>A35</span></h2>
                    <div class="field_info">
                        You have reached our financial donation line. I would be happy to transfer you to the correct department. You can also visit redcross.org/take-a-class to register for classes in your area.
                        <div class="english">
                            <br />Thank you for your support!!
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                        </div>
                        <div class="agent_note">
                            <br />To transfer the caller to Services to PHSS, press *7 wait for menu to begin then press 5.
                            <br />To cancel the transfer and return to the caller, press 0.
                            <br />
                            <br />In case take back and transfer fails you can tell the caller to call 800-443-3902<%-- Number Change to: 800-443-3902 - From: 800-451-7756 on 2015-07-28 by Nahuel from request by Carrie --%>
                        </div>
                    </div>
                    <div id="control_A35" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A35" type="button" value="Continue >>" onclick="Validate_A21('35')" />
                        <input id="back_A20_35" type="button" value="Back to A20" onclick="Return_A20('35')" />
                        <asp:HiddenField ID="backA20_35" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A36" class="section">
                    <h2>WANT INFORMATION ON VEHICLE DONATION<span>A36</span></h2>
                    <div class="field_info">
                        The American Red Cross in partnership with Insurance Auto Auctions (IAA) has a Vehicle Donation Program available nationwide. The program offers a tax-deductible way to dispose of unwanted vehicles with the proceeds from the sale going to support the Red Cross mission.  Please fill out the online form on redcross.org or call 1-855-927-2227.
                        <br />
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                    </div>
                    <div id="control_A36" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A36" type="button" value="Continue >>" onclick="Validate_A21('36')" />
                        <input id="back_A20_36" type="button" value="Back to A20" onclick="Return_A20('36')" />
                        <asp:HiddenField ID="backA20_36" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A37" class="section">
                    <h2>WANTS INFORMATION ABOUT THE USE OF FUNDS<span>A37</span></h2>
                    <div class="field_info">
                        <div class="english">
                            Your donation helps us to prevent and respond to home fires, save lives by stocking local blood banks, keep communities safe by providing safety training, and ensure military families can stay in touch in times of crisis.  An average of 91 cents of every dollar the American Red Cross spends is invested in humanitarian services and programs.
                        </div>
                        <div class="spanish">
                            Su donación nos ayuda a prevenir y responder a los incendios en el hogar, salvar vidas almacenando bancos de sangre locales, mantener seguras a las comunidades al proporcionar capacitación de seguridad y garantizar que las familias militares puedan mantenerse en contacto en tiempos de crisis. Un promedio de 91 centavos de cada dólar que gasta la Cruz Roja estadounidense se invierte en servicios y programas humanitarios.
                        </div>
                    </div>
                    <div class="field_info">
                        <div class="english">
                            You have reached our financial donation line. For up to date information about how the American Red Cross uses donations please visit redcross.org.
                            <br />I am able to help you make a financial contribution today.
                            <br />
                        </div>
                        <div class="spanish">
                            Usted ha llegado a nuestra línea de donaciones financieras. Para obtener información actualizada sobre cómo la Cruz Roja usa donaciones, visite redcross.org.
                            <br />Puedo ayudarte a hacer una contribución financiera hoy.
                            <br />
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Would you like to make a donation with us today?
                                </div>
                                <div class="spanish">
                                    ¿Te gustaría hacer una donación con nosotros hoy?
                                </div>
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="RadioButtonList37" runat="server">
                                    <asp:ListItem Value="YES">YES<span class="step_label">A1</span></asp:ListItem>
                                    <asp:ListItem Value="NO">NO<span class="step_label">End Call</span></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_info">
                        <div class="agent_note">
                            Use [Back to A20] for other questions
                            <br />IF DONOR CONTINUES TO BE PERSISTANT, PLEASE REFER THEM TO THE PUBLIC INQUIRY DEPT. at 1-202-303-4498
                        </div>
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                    </div>
                    <div id="control_A37" class="section_control">
                        <input id="continue_A37" type="button" value="Continue >>" onclick="Validate_A37()" />
                        <input id="back_A20_37" type="button" value="Back to A20" onclick="Return_A20('37')" />
                        <asp:HiddenField ID="backA20_37" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A38" class="section">
                    <h2>WANTS EXPLANATION OF DIFFERENT FUNDS<span>A38</span></h2>
                    <div class="field_info">
                        <span class="bold"><u>Disaster Relief</u></span>
                        <br />Donations made to Disaster Relief will help people affected by disasters big and small.
                        <br />
                        <br /><span class="bold"><u>Where It's Needed Most</u></span>
                        <br />A donation made to Where it's Needed most will support the urgent needs of the American Red Cross mission, whether it is responding to a disaster, collecting lifesaving blood, teaching skills to save a life, or assisting military members and their families during emergencies.
                        <br />
                        <br /><span class="bold"><u>Your Local Red Cross</u></span>
                        <br />Donations made to your local Red Cross will provide for local Red Cross programs and services in your community.
                        <br />
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Would you like to make a donation with us today?
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="RadioButtonList38" runat="server">
                                    <asp:ListItem Value="YES">YES<span class="step_label">A1</span></asp:ListItem>
                                    <asp:ListItem Value="NO">NO<span class="step_label">End Call</span></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_info">
                        <div class="agent_note">
                            Use [Back to A20] for other questions
                        </div>
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                    </div>
                    <div id="control_A38" class="section_control">
                        <input id="continue_A38" type="button" value="Continue >>" onclick="Validate_A38()" />
                        <input id="back_A20_38" type="button" value="Back to A20" onclick="Return_A20('38')" />
                        <asp:HiddenField ID="backA20_38" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A39" class="section">
                    <h2>PLANNED GIFT/WILL/BEQUEST/STOCK GIFTS/ANNUITIES<span>A39</span></h2>
                    <div class="field_info">
                        American Red Cross greatly appreciates your interest in our estate planning opportunities. For the quickest and easiest way to get information please visit redcrosslegacy.org. If you would like to talk to someone directly - may I connect you with our services and support department?
                        <div class="agent_note">
                            <br />If donor expresses need to speak with someone immediately:
                            <br />To transfer the caller to English Donor Services, press *7 wait for menu to begin then press 1.
                            <br />To transfer the caller to Spanish Donor Services, press *7 wait for menu to begin then press 2.
                            <br />To cancel the transfer and return to the caller, press 0.
                            <br />
                            <br />In case take back and transfer fails you can tell the caller to call 1-855-266-0723 for English or 1-855-266-0724 for Spanish.
                            <br />IF ASKED: Normal hours of operation are Monday - Friday 9:00 a.m. to 5:30 p.m. Eastern time (if after hours the donor may leave a message).
                        </div>
                        <br />
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                    </div>
                    <div id="control_A39" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A39" type="button" value="Continue >>" onclick="Validate_A21('39')" />
                        <input id="back_A20_39" type="button" value="Back to A20" onclick="Return_A20('39')" />
                        <asp:HiddenField ID="backA20_39" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A40" class="section">
                    <h2>WANTS TO BE REMOVED FROM THE MAILING LIST<span>A40</span></h2>
                    <div class="field_info">
                        I apologize for any inconvenience and will make sure that your name and address is removed from our mailing lists.
                        <br />May I have your name and address as it appears on our mailing label?
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Which lists would you like to be removed from?
                            </div>
                            <div class="line_radio">
                                <asp:CheckBox ID="tb40_nomail" runat="server" Text="No Direct Mail" />
                                <br /><asp:CheckBox ID="tb40_nophone" runat="server" Text="No Phone Calls" />
                                <br /><asp:CheckBox ID="tb40_noemail" runat="server" Text="No Emails" onclick="Section_A40_NoEmails()" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                May I please have your first name?
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb40_first_name" runat="server" PlaceHolder="First Name" MaxLength="50" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                And your last name?
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb40_last_name" runat="server" PlaceHolder="Last Name" MaxLength="50" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label agent_note">
                                Agent: If caller mentions a business name, enter it here.
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb40_business_name" runat="server" PlaceHolder="Business Name" MaxLength="100" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                How do you prefer to be addressed?
                            </div>
                            <div class="line_radio">
	                            <asp:DropDownList ID="tb40_prefix" runat="server" style="width: 125px;">
                                    <asp:ListItem Value="0" Text="NONE" />
                                    <asp:ListItem Value="1" Text="DR" />
                                    <asp:ListItem Value="2" Text="RABBI" />
                                    <asp:ListItem Value="3" Text="MR & MRS" />
                                    <asp:ListItem Value="4" Text="MISS" />
                                    <asp:ListItem Value="5" Text="MRS" />
                                    <asp:ListItem Value="6" Text="MS" />
                                    <asp:ListItem Value="7" Text="MR" />
                                    <asp:ListItem Value="8" Text="REV" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                May I please have your street address?
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb40_address1" runat="server" PlaceHolder="Address" MaxLength="100" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Is there a suite or apartment number?
                            </div>
                            <div class="line_input">
                                <asp:DropDownList ID="tb40_suite_type" runat="server" style="width: 125px;">
                                    <asp:ListItem Value="0" Text="NONE" />
                                    <asp:ListItem Value="1" Text="APT" />
                                    <asp:ListItem Value="2" Text="BLDG" />
                                    <asp:ListItem Value="3" Text="CONDO" />
                                    <asp:ListItem Value="4" Text="FLOOR" />
                                    <asp:ListItem Value="5" Text="HOUSE" />
                                    <asp:ListItem Value="6" Text="LOT" />
                                    <asp:ListItem Value="7" Text="SUITE" />
                                    <asp:ListItem Value="8" Text="UNIT" />
                                </asp:DropDownList>
                                <asp:TextBox ID="tb40_suite_number" runat="server" PlaceHolder="Number" MaxLength="50" Width="75px" />
                            </div>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="ZipEngine40" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="tb40_postal_code" EventName="TextChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        <div class="english">
                                            What is your country?
                                        </div>
                                        <div class="spanish">
                                            &iquest;Cu&aacute;l es su pa&iacute;s?
                                        </div>
                                        <div class="agent_note">
                                            Only ask if Country appears to not be United States.
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:DropDownList ID="tb40_country" runat="server" Width="225px" OnChange="return Country_Switch(this)" />                                
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        May I have your zip code please?
                                        <div class="agent_note">
                                            [Agent] press tab after entering zip code and the system will look up the city and state.
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:TextBox ID="tb40_postal_code" runat="server" PlaceHolder="Postal Code" MaxLength="10" Width="125px" OnTextChanged="RunZipEngine" AutoPostBack="true" />
                                        <asp:Label ID="lbltb40_postal_code" runat="server" Text="" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        Your city is
                                    </div>
                                    <div class="line_input required">
                                        <asp:TextBox ID="tb40_city" runat="server" PlaceHolder="City" MaxLength="20" Width="225px" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        Your state is
                                    </div>
                                    <div class="line_input required">
                                        <asp:DropDownList ID="tb40_state" runat="server" Width="225px" /><asp:DropDownList ID="tb40_stateca" runat="server" Width="225px" /><asp:TextBox ID="tb40_stateother" runat="server" PlaceHolder="State" MaxLength="25" Width="225px" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Would you like to receive occasional updates from the Red Cross via email to stay informed on disaster alerts, preparedness tips, and other ways to get involved?
                            </div>
                            <div class="line_radio">
                                <asp:RadioButtonList ID="tb40_email_optin" runat="server">
                                    <asp:ListItem Value="YES" Text="YES" />
                                    <asp:ListItem Value="NO" Text="NO" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Email Address
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb40_email" runat="server" PlaceHolder="Email" MaxLength="100" Width="325px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Phone Number
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb40_phone" runat="server" PlaceHolder="Phone" MaxLength="15" Width="125px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_info">
                        Because of our technology processes, this request could take up to 4 to 6 weeks to process.
                        <br />
                    </div>
                    <div id="control_A40" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A40" type="button" value="Continue >>" onclick="Validate_A40()" />
                        <input id="back_A20_40" type="button" value="Back to A20" onclick="Return_A20('40')" />
                        <asp:HiddenField ID="backA20_40" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A46" class="section">
                    <h2>Wants More Sustainer Information<span>A46</span></h2>
                    <div class="field_info">
                        <div class="english">
                            Thank you for your interest in learning more about the American Red Cross, and the impact of monthly support from our Red Cross Champions. While we are on the phone, are there any questions I can answer for you? 
                            <br />
                            <br />HOW DOES IT WORK: 
                            <br />From small house fires to multi-state natural disasters, the American Red Cross goes wherever we’re needed, so people can have clean water, safe shelter, hot meals and hope when they need it most.  
                            <br />
                            <br />HOW MUCH DO I HAVE TO GIVE:
                            <br />Many people give $19 or even $25 a month, but any donation you give will help us to be prepared to respond to disasters big and small.
                            <br />
                            <br />
                            <br />I’d be happy to send you additional information on the many services the Red Cross provides for disasters big and small.  There will also be an envelope included if you would like to make a gift.  I just need your address so we can send a packet, okay?  
                            <br />                            
                        </div>
                        <div class="spanish">
                            Gracias por su interés en aprender más sobre la Cruz Roja Americana y el impacto del apoyo mensual de nuestros Campeones de la Cruz Roja. Mientras estamos en el teléfono, ¿hay alguna pregunta que pueda responder por usted?
                            <br />
                            <br />COMO FUNCIONA:
                            <br />Desde los incendios domésticos pequeños hasta los desastres naturales en varios estados, la Cruz Roja Americana va donde sea que se necesite, para que las personas puedan tener agua limpia, refugio seguro, comidas calientes y esperanza cuando más lo necesitan.
                            <br />
                            <br />¿CUÁNTO TENGO QUE DAR?
                            <br />Muchas personas donan $ 19 o incluso $ 25 por mes, pero cualquier donación que nos brinde nos ayudará a estar preparados para responder a desastres grandes y pequeños.
                            <br />
                            <br />
                            <br />Con gusto le enviaré información adicional sobre los muchos servicios que la Cruz Roja brinda para desastres grandes y pequeños. También habrá un sobre incluido si desea hacer un regalo. Solo necesito tu dirección para que podamos enviar un paquete, ¿está bien?
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                May I please have your first name?
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb46_first_name" runat="server" PlaceHolder="First Name" MaxLength="50" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                And your last name?
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb46_last_name" runat="server" PlaceHolder="Last Name" MaxLength="50" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label agent_note">
                                Agent: If caller mentions a business name, enter it here.
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb46_business_name" runat="server" PlaceHolder="Business Name" MaxLength="100" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                How do you prefer to be addressed?
                            </div>
                            <div class="line_radio">
	                            <asp:DropDownList ID="tb46_prefix" runat="server" style="width: 125px;">
                                    <asp:ListItem Value="0" Text="NONE" />
                                    <asp:ListItem Value="1" Text="DR" />
                                    <asp:ListItem Value="2" Text="RABBI" />
                                    <asp:ListItem Value="3" Text="MR & MRS" />
                                    <asp:ListItem Value="4" Text="MISS" />
                                    <asp:ListItem Value="5" Text="MRS" />
                                    <asp:ListItem Value="6" Text="MS" />
                                    <asp:ListItem Value="7" Text="MR" />
                                    <asp:ListItem Value="8" Text="REV" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                May I please have your street address?
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb46_address1" runat="server" PlaceHolder="Address" MaxLength="100" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Is there a suite or apartment number?
                            </div>
                            <div class="line_input">
                                <asp:DropDownList ID="tb46_suite_type" runat="server" style="width: 125px;">
                                    <asp:ListItem Value="0" Text="NONE" />
                                    <asp:ListItem Value="1" Text="APT" />
                                    <asp:ListItem Value="2" Text="BLDG" />
                                    <asp:ListItem Value="3" Text="CONDO" />
                                    <asp:ListItem Value="4" Text="FLOOR" />
                                    <asp:ListItem Value="5" Text="HOUSE" />
                                    <asp:ListItem Value="6" Text="LOT" />
                                    <asp:ListItem Value="7" Text="SUITE" />
                                    <asp:ListItem Value="8" Text="UNIT" />
                                </asp:DropDownList>
                                <asp:TextBox ID="tb46_suite_number" runat="server" PlaceHolder="Number" MaxLength="50" Width="75px" />
                            </div>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="tb46_postal_code" EventName="TextChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        <div class="english">
                                            What is your country?
                                        </div>
                                        <div class="spanish">
                                            &iquest;Cu&aacute;l es su pa&iacute;s?
                                        </div>
                                        <div class="agent_note">
                                            Only ask if Country appears to not be United States.
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:DropDownList ID="tb46_country" runat="server" Width="225px" OnChange="return Country_Switch(this)" />                                
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        May I have your zip code please?
                                        <div class="agent_note">
                                            [Agent] press tab after entering zip code and the system will look up the city and state.
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:TextBox ID="tb46_postal_code" runat="server" PlaceHolder="Postal Code" MaxLength="10" Width="125px" OnTextChanged="RunZipEngine" AutoPostBack="true" />
                                        <asp:Label ID="lbltb46_postal_code" runat="server" Text="" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        Your city is
                                    </div>
                                    <div class="line_input required">
                                        <asp:TextBox ID="tb46_city" runat="server" PlaceHolder="City" MaxLength="20" Width="225px" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        Your state is
                                    </div>
                                    <div class="line_input required">
                                        <asp:DropDownList ID="tb46_state" runat="server" Width="225px" /><asp:DropDownList ID="tb46_stateca" runat="server" Width="225px" /><asp:TextBox ID="tb46_stateother" runat="server" PlaceHolder="State" MaxLength="25" Width="225px" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="field_info">
                        <div class="english">
                            Wonderful! You can expect more information about the Red Cross to arrive to the above address shortly.
                        </div>
                        <div class="spanish">
                            ¡Maravilloso! Usted puede contar con más información acerca de la Cruz Roja para llegar a la dirección anterior en breve.
                        </div>
                    </div>
                    <div id="control_A46" class="section_control">
                        <span class="step_label">End Call<br /></span>
                        <input id="continue_A46" type="button" value="Continue >>" onclick="Validate_A46()" />
                        <input id="back_A20_46" type="button" value="Back to A20" onclick="Return_A20('46')" />
                        <asp:HiddenField ID="backA20_46" runat="server" Value="hide" />
                    </div>
                </div>
                <div runat="server" id="section_A47" class="section">
                    <h2>Holiday Catalog - Alternate Shipping Address<span>A47</span></h2>
                    <div class="field_info">
                        <div class="english">
                            What is your shipping address?
                        </div>
                        <div class="spanish">
                            ¿Cuál es su dirección de envío?
                        </div>
                    </div>
                    <div class="field_input hide">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                May I please have your first name?
                                <div class="agent_note">
                                    Skip if same name
                                </div>
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb47_first_name" runat="server" PlaceHolder="First Name" MaxLength="50" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input hide">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                And your last name?
                                <div class="agent_note">
                                    Skip if same name
                                </div>
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb47_last_name" runat="server" PlaceHolder="Last Name" MaxLength="50" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label agent_note">
                                Agent: If caller mentions a business name, enter it here.
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tb47_business_name" runat="server" PlaceHolder="Business Name" MaxLength="100" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input hide">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                How do you prefer to be addressed?
                            </div>
                            <div class="line_radio">
	                            <asp:DropDownList ID="tb47_prefix" runat="server" style="width: 125px;">
                                    <asp:ListItem Value="0" Text="NONE" />
                                    <asp:ListItem Value="1" Text="DR" />
                                    <asp:ListItem Value="2" Text="RABBI" />
                                    <asp:ListItem Value="3" Text="MR & MRS" />
                                    <asp:ListItem Value="4" Text="MISS" />
                                    <asp:ListItem Value="5" Text="MRS" />
                                    <asp:ListItem Value="6" Text="MS" />
                                    <asp:ListItem Value="7" Text="MR" />
                                    <asp:ListItem Value="8" Text="REV" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    May I please have your street address?
                                </div>
                                <div class="spanish">
                                    ¿Puedo por favor tener su dirección?
                                </div>
                            </div>
                            <div class="line_input required">
                                <asp:TextBox ID="tb47_address1" runat="server" PlaceHolder="Address" MaxLength="100" Width="225px" />
                            </div>
                        </div>
                    </div>
                    <div class="field_input">
                        <div class="single_line clearfix">
                            <div class="line_label">
                                <div class="english">
                                    Is there a suite or apartment number?
                                </div>
                                <div class="spanish">
                                    ¿Hay un número de suite o apartamento?
                                </div>
                            </div>
                            <div class="line_input">
                                <asp:DropDownList ID="tb47_suite_type" runat="server" style="width: 125px;">
                                    <asp:ListItem Value="0" Text="NONE" />
                                    <asp:ListItem Value="1" Text="APT" />
                                    <asp:ListItem Value="2" Text="BLDG" />
                                    <asp:ListItem Value="3" Text="CONDO" />
                                    <asp:ListItem Value="4" Text="FLOOR" />
                                    <asp:ListItem Value="5" Text="HOUSE" />
                                    <asp:ListItem Value="6" Text="LOT" />
                                    <asp:ListItem Value="7" Text="SUITE" />
                                    <asp:ListItem Value="8" Text="UNIT" />
                                </asp:DropDownList>
                                <asp:TextBox ID="tb47_suite_number" runat="server" PlaceHolder="Number" MaxLength="50" Width="75px" />
                            </div>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="tb47_postal_code" EventName="TextChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        <div class="english">
                                            What is your country?
                                        </div>
                                        <div class="spanish">
                                            &iquest;Cu&aacute;l es su pa&iacute;s?
                                        </div>
                                        <div class="agent_note">
                                            Only ask if Country appears to not be United States.
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:DropDownList ID="tb47_country" runat="server" Width="225px" OnChange="return Country_Switch(this)" />                                
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        <div class="english">
                                            May I have your zip code please?
                                        </div>
                                        <div class="spanish">
                                            ¿Puedo tener tu código postal, por favor?
                                        </div>
                                        <div class="agent_note">
                                            [Agent] press tab after entering zip code and the system will look up the city and state.
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:TextBox ID="tb47_postal_code" runat="server" PlaceHolder="Postal Code" MaxLength="10" Width="125px" OnTextChanged="RunZipEngine" AutoPostBack="true" />
                                        <asp:Label ID="lbltb47_postal_code" runat="server" Text="" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        <div class="english">
                                            Your city is
                                        </div>
                                        <div class="spanish">
                                            Tu ciudad es
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:TextBox ID="tb47_city" runat="server" PlaceHolder="City" MaxLength="20" Width="225px" />
                                    </div>
                                </div>
                            </div>
                            <div class="field_input">
                                <div class="single_line clearfix">
                                    <div class="line_label">
                                        <div class="english">
                                            Your state is
                                        </div>
                                        <div class="spanish">
                                            Tu estado es
                                        </div>
                                    </div>
                                    <div class="line_input required">
                                        <asp:DropDownList ID="tb47_state" runat="server" Width="225px" /><asp:DropDownList ID="tb47_stateca" runat="server" Width="225px" /><asp:TextBox ID="tb47_stateother" runat="server" PlaceHolder="State" MaxLength="25" Width="225px" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="field_info">
                        <div class="english">
                            We will ship your gift items to this address.
                        </div>
                        <div class="spanish">
                            Enviaremos sus artículos de regalo a esta dirección.
                        </div>
                    </div>
                    <div id="control_A47" class="section_control">
                        <span class="step_label">A9<br /></span>
                        <input id="continue_A47" type="button" value="Continue >>" onclick="Validate_A47()" />
                        <input id="back_A8N" type="button" value="Back to A8" onclick="Return_A8N()" />
                    </div>
                </div>
                <%--
                Closing(s) 90-95
                96 - Dispositions / End Call
                98 - Confirm all information (Not usually used)
                99 - Processing
                0 - Information / Submited Record Details / 
                00 - Call Details
                --%>
                <div runat="server" id="section_A90" class="section">
                    <h2>Closing<span>A90</span></h2>
                    <div class="field_info">
                        Thank you for your support!!
                        <br />Good Bye.
                    </div>
                    <div id="control_A90" class="section_control">
                        <input id="continue_A90" type="button" value="Continue >>" onclick="Validate_A90()" /><span class="step_label">End Call</span>
                        <input id="back_A15Y" type="button" value="Back to A15" onclick="Return_A15Y()" />
                    </div>
                </div>
                <div runat="server" id="section_A96" class="section">
                    <h2>End Call<span>A96</span></h2>
                    <div class="field">
                        <br />
                        <div class="agent_note bold">
                            Agent: You must [Submit Call] in order to process donation/pledge/call record.
                            <br />
                        </div>
                        <div class="agent_note">
                            If a disposition was not pre-selected, you must select one now before submitting the call.
                        </div>
                        <div class="field_option">
                            <asp:ListBox ID="ListBox96" runat="server" Rows="25"  Width="275" CssClass="calltype_listbox">
                                <asp:ListItem disabled="disabled" class="field_option_group calltype_general">General / Escape</asp:ListItem>
                                <asp:ListItem Text="Training" Value="20" class="calltype_general" />
                                <asp:ListItem Text="Complaint on Policies" Value="1" class="calltype_general" />
                                <asp:ListItem Text="No Whisper" Value="2" class="calltype_general" />
                                <asp:ListItem Text="Hung Up" Value="9" class="calltype_general" />
                                <asp:ListItem Text="Information Only" Value="10" class="calltype_general" />
                                <%--<asp:ListItem Text="Referred" Value="18" />--%>
                                <asp:ListItem Text="Wrong Number" Value="22" class="calltype_general" />
                                <%--<asp:ListItem Text="Research" Value="23" />--%>
                                <asp:ListItem disabled="disabled" class="field_option_group calltype_donation">Donations</asp:ListItem>
                                <asp:ListItem Text="One Time [Donation]" Value="41" class="calltype_donation" />
                                <asp:ListItem Text="Sustainer [Donation]" Value="46" class="calltype_donation" />
                                <%--<asp:ListItem Text="Donation [Designation]: High Amount" Value="42" />--%>
                                <asp:ListItem Text="Pledge [One-Time]" Value="42" class="calltype_donation" />
                                <asp:ListItem Text="Pledge [Sustainer]" Value="47" class="calltype_donation" />
                                <asp:ListItem Text="Pledge [No Address]" Value="49" class="calltype_donation" />
                                <%--<asp:ListItem Text="Pledge [Designation]: High Amount" Value="44" />--%>
                                <asp:ListItem Text="Pledge Special Cause" Value="43" class="calltype_donation" />
                                <asp:ListItem disabled="disabled" class="field_option_group calltype_question">Other Questions</asp:ListItem>
                                <asp:ListItem Text="Wanted to Donate Blood/Questions" Value="3" class="calltype_question" />
                                <asp:ListItem Text="Wanted to Volunteer" Value="4" class="calltype_question" />
                                <asp:ListItem Text="Info About International Services" Value="39" class="calltype_question" />
                                <asp:ListItem Text="Wanted to Donate Goods" Value="5" class="calltype_question" />
                                <asp:ListItem Text="Wanted to Sponsor an Event" Value="6" class="calltype_question" />
                                <asp:ListItem Text="Wanted Donation/Receipt Info" Value="8" class="calltype_question" />
                                <asp:ListItem Text="Wanted Information on Red Cross" Value="7" class="calltype_question" />
                                <asp:ListItem Text="Needed Local Chapter Info" Value="11" class="calltype_question" />
                                <asp:ListItem Text="Needed Help" Value="12" class="calltype_question" />
                                <asp:ListItem Text="Needed to Locate Military Personnel" Value="13" class="calltype_question" />
                                <asp:ListItem Text="Needed Media Inquiry" Value="14" class="calltype_question" />
                                <asp:ListItem Text="Wanted Course/Class Information" Value="15" class="calltype_question" />
                                <asp:ListItem Text="Want Information On Vehicle Donation" Value="40" class="calltype_question" />
                                <asp:ListItem Text="Wanted Information About Use of Funds" Value="16" class="calltype_question" />
                                <asp:ListItem Text="Wants Explanation of Different Funds" Value="26" class="calltype_question" />
                                <asp:ListItem Text="Wanted to Donate Planned Gift" Value="17" class="calltype_question" />
                                <asp:ListItem Text="Wants to be Removed From Mailing List" Value="27" class="calltype_question" />
                                <asp:ListItem Text="Wants More Sustainer Information" Value="48" class="calltype_question" />
                                <asp:ListItem disabled="disabled" class="field_option_group calltype_disabled">Not Used</asp:ListItem>
                                <asp:ListItem disabled="disabled" Text="2012 Holiday Giving Catalog" Value="37" class="calltype_disabled" />
                                <asp:ListItem disabled="disabled" Text="2012 Hurricane Sandy Telethon" Value="38" class="calltype_disabled" />
                                <asp:ListItem disabled="disabled" Text="Canceled Order" Value="30" class="calltype_disabled" />
                                <asp:ListItem disabled="disabled" Text="Gift Matching" Value="31" class="calltype_disabled" />
                                <asp:ListItem disabled="disabled" Text="Message" Value="29" class="calltype_disabled" />
                                <asp:ListItem disabled="disabled" Text="Not GE-Canada" Value="21" class="calltype_disabled" />
                                <asp:ListItem disabled="disabled" Text="Wanted to Donate Memorial Gift" Value="19" class="calltype_disabled" />
                                <asp:ListItem disabled="disabled" Text="Wanted to Donate Planned Gift" Value="28" class="calltype_disabled" />
                                <asp:ListItem disabled="disabled" Text="Wanted to Find Loved Ones" Value="25" class="calltype_disabled" />
                            </asp:ListBox>
                        </div>
                        <div runat="server" id="section_A96donation" class="agent_note red">Submitting a donation will attempt to process the credit card</div>
                        <asp:HiddenField ID="sectionA96donation" runat="server" Value="hide" />
                        
                        <span class="agent_note red">Please ensure to use [Training] for any record that is test or training.</span>
                        <div id="escaped" class="agent_note red" style="display: none;">
                            <br />If you used [Escape] you can not submit a donation.
                            <br />You must use [Undo-Escape] and proceed with [Continue] buttons.
                        </div>
                    </div>
                    <div id="control_A96" class="section_control">
                        <asp:Button ID="btnSubmitCall" runat="server" Text="Button" OnClick="Submit_Record" />
                        <input id="continue_A96" type="button" value="Submit Call" onclick="Validate_A96();" />
                        <input id="back_A90Y" type="button" value="Back to A90" onclick="Return_A90Y()" />
                        <input id="back_A17Y" type="button" value="Back to A17" onclick="Return_A17Y()" />
                        <%--Confirmed--%>
                        <input id="back_A1E" type="button" value="Back to Start" onclick="Return_A1E()" />
                        <input id="back_A5Y" type="button" value="Back to A5" onclick="Return_A5Y()" />
                        <input id="back_A6Y" type="button" value="Back to A6" onclick="Return_A6Y()" />
                        <input id="back_A9Y" type="button" value="Back to A9" onclick="Return_A9Y()" />
                        <input id="back_A2E" type="button" value="Back to A2" onclick="Return_A2E()" />
                        <input id="back_A21Y" type="button" value="Back to A##" onclick="Return_AY()" />
                        <input id="back_A20_96" type="button" value="Back to A20" onclick="Return_A20('96')" />
                        <input id="back_A45Y" type="button" value="Back to A45" onclick="Return_A45Y()" />
                        <input id="back_A48Y" type="button" value="Back to A48" onclick="Return_A48Y()" />
                        <input id="back_A0B" type="button" value="Back to A0" onclick="Return_A0B()" />
                        <input id="back_A49Y" type="button" value="Back to A49" onclick="Return_A49Y()" />
                        <asp:HiddenField ID="backA20_96" runat="server" Value="hide" />
                        <input id="clear_A96C" type="button" value="Clear Credit Card" onclick="Clear_A96C()" style="width: 125px;" />
                        <asp:HiddenField ID="clearA96C" runat="server" Value="hide" />
                    </div>
                    <div id="validation_issues" style="display: none;margin-left: 15px;margin-bottom: 10px;">
                        <asp:Label ID="validation_message" runat="server" Text="" ForeColor="DarkRed" />
                    </div>
                </div>
                <div runat="server" id="section_A98" class="section">
                    <h2>Confirm Record<span>A98</span></h2>
                    <div class="field_info">
                        <div class="english">
                            <br />Thank you for your support!!
                            <br />
                            <br />Good Bye.
                        </div>
                        <div class="spanish">
                            <br />¡¡Gracias por tu apoyo!!
                            <br />
                            <br />Adiós.
                        </div>
                        <div class="agent_note">
                            You may close the script at this point.
                        </div>
                    </div>
                    <div id="control_A98" class="section_control">
                        <input id="back_A0N" type="button" value="Back to A0" onclick="Return_A0N()" />
                        <input type="button" value="Close Window" onclick="CloseWindow()" style="display: none;" />
                    </div>
                </div>
                <div runat="server" id="section_A99" class="section">
                    <h2>Processing<span>A99</span></h2>
                    <div class="field">
                        Call Processing, please wait...
                    </div>
                </div>
                <div runat="server" id="section_A" class="section">
                    <h2>Validation<span></span></h2>
                    <div class="field">
                        <asp:Label ID="lblValidation" runat="server" Text="" ForeColor="Red" />
                    </div>
                    <asp:HiddenField ID="sectionA" runat="server" Value="hide" />
                </div>
                <div runat="server" id="section_A00" class="section">
                    <h2>Call Details<span>A00</span></h2>
                    <div class="field">
                        <a href="javascript:callDetails('hide')">Hide</a>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Message
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdMessage" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Company ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCompanyID" runat="server" Text="3" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Interaction ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdInteractionID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Mode
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdMode" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Call UU ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCallUUID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                ARC Call ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCallID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Call Info Created
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCallInfo" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Alternate Address Created
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCallInfo_Alternate" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Disposition
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdDisposition" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Call Login DateTime
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCallLoginDateTime" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Call End DateTime
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCallEndDateTime" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                CB CreateDate
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCBCreateDate" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Five9 Call ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdFive9CallID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Five9 Campaign ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdFive9CampaignID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                ANI
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdANI" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                DNIS
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdDNIS" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Company
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCompany" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Line
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdLine" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Agent DE ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdAgentDeID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Agent ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdAgentID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Agent Name
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdAgentName" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Agent Ext
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdAgentExt" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Start Time
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdSystemStart" runat="server" Enabled="false" />
                                <asp:TextBox ID="cdSystemStartLen" runat="server" Enabled="false" Width="20px" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Call Center
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCallCenter" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Campaign
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCampaign" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Designation
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdDesignation" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Call Start
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCallStart" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Call End
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCallEnd" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Call Duration
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdDuration" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Call Create ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdCallCreateID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Standard Selection ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdStandardSelectionID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Charge Date ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdChargeDateID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                CC Info ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdDonationCCInfoID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Order ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdOrderID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Charge ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdChargeID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Remove ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdRemoveID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Charge Status
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdChargeStatus" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Status
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdStatus" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Holiday Catalog
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdHCCatalogName" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Language
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdLanguage" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Language ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdLanguageID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Holiday DesignationID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdHCDesignationId" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Sustainer Donation [rd]
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="rdSustainer_txt" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                rd Charge Date
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="rdChargeDate_old" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                rd Charge Date Original
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="rdChargeDateOriginal_txt" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                rd Receipt Frequency
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="rdReceiptFrequency_txt" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                rd Call ID
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="rdCallID" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                rd Full Date
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="rdFullDate" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Holiday Catalog Insert
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="hcSelectedGiftInsert" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                DeBug
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="tbDeBug" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Soure
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdSource" runat="server" Enabled="false" />
                            </div>
                        </div>
                        <div class="single_line clearfix">
                            <div class="line_label">
                                Telethon Mode
                            </div>
                            <div class="line_input">
                                <asp:TextBox ID="cdTelethonMode" runat="server" Enabled="false" Text="False" />
                            </div>
                        </div>
                    </div>
                    <div class="field_hidden">
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server">
                        </asp:PlaceHolder>
                        <asp:HiddenField ID="card_typeID" runat="server" />
                        <asp:HiddenField ID="DonType" runat="server" />

                        <asp:HiddenField ID="escapedToggle" runat="server" />

                        <asp:HiddenField ID="LoadMe" runat="server" Value="yes" />

                        <asp:HiddenField ID="sectionA00" runat="server" Value="hide" />

                        <asp:HiddenField ID="selectLanguage" runat="server" />

                        <asp:HiddenField ID="DesignationList" runat="server" />

                        <asp:HiddenField ID="typeDNIS" runat="server" />
                        <asp:HiddenField ID="sectionHistory" runat="server" Value="A1" />
                    </div>
                    <div id="control_A00" class="section_control">
                    </div>
                </div>
                <div runat="server" id="section_A01" class="section" visible="false">
                    <h2>Agent Stats<span></span></h2>
                    <div class="field">
                        <asp:Label ID="lblAgentStats" runat="server" Text="" ForeColor="Red" />
                    </div>
                    <asp:HiddenField ID="sectionA01" runat="server" Value="hide" />
                </div>
            </div>
            <div id="config_error" style="display: none;" runat="server" visible="false">
                <asp:Label ID="lblConfigError" runat="server" Text="There was a validation error on the page; contact IT.<br />This is a fatal error." />
            </div>
            <div>
                <div style="font-weight: bold;font-size: 1em;padding-top: 1em;padding-bottom: 1em;">
                    <asp:HyperLink ID="reload_script2" runat="server" style="margin-right: 5px;">Blank Script</asp:HyperLink> - Use this link to load a blank script if a 2nd transaction is needed
                </div>
                <div style="font-weight: bold;font-size: 1em;padding-top: 1em;padding-bottom: 1em;color: blue;">
                    Agent: Do not use the browser refresh (F5) key - use "Reload Script" link above if you need to start the same call over again.
                </div>
                <hr />
                <asp:Label ID="lblInformation" runat="server" Text="" Visible="true" />
                <asp:Label ID="lblQueryTime" runat="server" Text="" Visible="true" ForeColor="DarkRed" />
            </div>
        </div>
    </form>
</body>
</html>