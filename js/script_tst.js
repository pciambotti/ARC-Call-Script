/*
This script will hold functions that can change based on the script content
*/
var vfMsg = "";
function pageLoad() {
    $(document).ready(function () {
        activatePlaceholders();
        Toggle_From_Hidden();

        $("#lblJRE").text("03");

        $("#noscript").hide();
        $("#script").show();
        $("#header_control").show();
        Country_Switch(document.getElementById("tb8_country"));
        Country_Switch(document.getElementById("tb40_country"));
        Country_Switch(document.getElementById("tb45_country"));
        Country_Switch(document.getElementById("tb46_country"));
        Country_Switch(document.getElementById("tb47_country"));

        //$("#Button37").hide();
        //$("#Button38").hide();
        //$("#Button39").hide();
        $("#btnSubmitCall").hide();

        $("#lblAgentName2").html($("#cdAgentName").val());


        if ($("#LoadMe").val() == "yes") {
            // This will load some default stuff, mainly used for debugging
            //            $("#RadioButtonList2").find("input[value='YES']").attr("checked", "checked");
            //            Validate_A();
        }
        else if ($("#LoadMe").val() == "close") {
            $("#tglAll").hide();
        }

        var tgl;
        tgl = document.getElementById("sectionA3other").value;
        Toggle_Field("section", "A3other", tgl);
        tgl = document.getElementById("sectionA3gc").value;
        Toggle_Field("section", "A3gc", tgl);
        tgl = document.getElementById("sectionA3sc").value;
        Toggle_Field("section", "A3sc", tgl);
        tgl = document.getElementById("sectionA3high").value;
        Toggle_Field("section", "A3high", tgl);
        //tgl = document.getElementById("backA3B").value;
        //Toggle_Field("back", "A3B", tgl);
        tgl = document.getElementById("sectionA96donation").value;
        Toggle_Field("section", "A96donation", tgl);
        Toggle_Field("back", "A20_96", "hide");
        tgl = document.getElementById("backA1Yb").value;
        Toggle_Field("back", "A1Yb", tgl);
        tgl = document.getElementById("sectionA44long").value;
        Toggle_Field("section", "A44long", tgl);
        tgl = document.getElementById("sectionA44short").value;
        Toggle_Field("section", "A44short", tgl);

        tgl = document.getElementById("declineA0Y").value;
        Toggle_Field("decline", "A0Y", tgl);

        //tgl = document.getElementById("backA0A").value;
        //Toggle_Field("back", "A0A", tgl);

        tgl = document.getElementById("backA4F").value;
        Toggle_Field("back", "A4F", tgl);

        tgl = document.getElementById("sectionA3sustainer").value;
        Toggle_Field("section", "A3sustainer", tgl);

        tgl = document.getElementById("clearA96C").value;
        Toggle_Field("clear", "A96C", tgl);

        //tgl = document.getElementById("backA0B").value;
        //Toggle_Field("back", "A0B", tgl);

        tgl = document.getElementById("sectionA").value;
        Toggle_Field("section", "A", tgl);

        $("#hcTitle1").text($("#hcCatalogTitle").val());
        $("#hcTitle2").html($("#hcCatalogTitle").val() + " - Choose Catalog Items<span>A41</span>");
        $("#hcTitle3").html($("#hcCatalogTitle").val() + " - Select Free Gift<span>A42</span>");

        premiumGiftObj = [];
        $(".giftcatalog").each(function (index, value) {
            var x = $(this);
            x.blur(function () {
                if (x.val() != "") {
                    if (x.val() == "0") { x.val(""); x.css("background-color", ""); x.css("color", "#000000"); }
                    else { x.css("background-color", "#3a4875"); x.css("color", "#ffffff"); }

                } else {
                    x.css("background-color", ""); x.css("color", "#000000");
                }
            });
        });
        $(".premiumGiftHiddenDiv .premiumgift").each(function (index, value) {
            var x = $(this);
            premiumGiftObj.push({
                Title: $("#title_" + x.val()).val()
                , Min: parseInt($("#min_" + x.val()).val())
                , Max: parseInt($("#max_" + x.val()).val())
                , SKU: $("#sku_" + x.val()).val()

            });
        });
        //[END] 10.14.2014 - Added for holiday catalog 
        // $(".sectionA3_hidden").hide();
        $("#sectionA9_drtv").hide();
    }
    );
}
function pageLoad2() {
    if (document.getElementById("Donation_Script_Capture")) {
        if (document.getElementById("DN_DonStart").value.length == 0) {
            var now = new Date();
            var dtNow = now.getMonth() + 1 + '/' + now.getDate() + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds() + '.' + now.getMilliseconds();
            document.getElementById("DN_DonStart").value = dtNow;
        }
    }
    EscapeToStart();
}
/* Validation */
function Validate_A1() {
    validationStarted();

    var rdValue = $("#rb1_options input:checked").val();
    if (!rdValue) {
        // validationStarted();
        vfMsg = "You must select an option above.";
        validationFailed("A1");
    } else {
        // Else If Not "No" && Not "Holiday"
        if (rdValue == "YES" || rdValue == "DRTV_YES" || rdValue == "DRTV_NO") {
            // If Holiday is Active and we are not doing a Holiday donation, hide that designation
            var hDesignationID = $("#cdHCDesignationId").val();
            if (hDesignationID.length > 0) {
                Designation_Toggle_Holiday_Hide();
            }

            // Continue_A1D (Together with A)
            // else if
            if ($("#cdCompany").val() == "DRTV") {
                $("#A2_standard_01").hide();
                $("#A2_standard_02").hide();
                $("#A2_standard_03").hide();
                $("#A2_drtv_01").show();
                //$("#A2_drtv_02").show();
                $("#sectionA2drtv").val(rdValue);
                if (rdValue == "DRTV_NO") {
                    $(".A2_drtv_01_standard").show();
                    $(".A2_drtv_01_champion").hide();
                    $("#A2_step_label").html("A3");
                } else {
                    $(".A2_drtv_01_standard").hide();
                    $(".A2_drtv_01_champion").show();
                    // $("#A2_step_label").html("A43");
                    // var rdValue = $("#rb1_options input:checked").val();
                    // validate_A4
                    $("#A2_step_label").html("A3"); // We move A43 to after CREDIT CARD
                }
            } else if ($("#cdSource").val() == "_Ansafone" || $("#cdCompany").val() == "Harvey Telethon" || $("#cdCompany").val() == "Irma Telethon" || $("#cdTelethonMode").val() == "True") {
                $("#A2_drtv_01").hide();
                $("#A2_drtv_02").hide();
                $("#A2_standard_02").hide();
                $("#A2_standard_03").hide();                
            } else {
                $("#A2_standard_01").show();
                $("#A2_standard_02").show();
                $("#A2_standard_03").show();
                $("#A2_drtv_01").hide();
                $("#A2_drtv_02").hide();
            }

            // Continue_A1A
            Toggle_Field("section", "A1", "hide");
            Toggle_Field("section", "A2", "show");
            Toggle_Field("control", "A2", "show");
            Toggle_Field("back", "A1Y", "show");
            Toggle_Field("continue", "A2", "show");
            $("#tb2_amount_dollar").focus();
        }
        else if (rdValue == "NO") {
            // Continue_A1B
            //if ($("#cdCompany").val() == "CG" || $("#cdCompany").val() == "CG (SP)")
            //{
            //    Toggle_Field("section", "A1", "hide");
            //    Toggle_Field("section", "A20", "show");
            //    Toggle_Field("control", "A20", "show");
            //    Toggle_Field("back", "A1N", "show");
            //    Toggle_Field("continue", "A20", "show");
            //}
            if (($("#cdCompany").val() == "Harvey Telethon" || $("#cdCompany").val() == "Irma Telethon" || $("#cdTelethonMode").val() == "True") && $("#cdLine").val() != "800 RED-CROSS")
            {
                // Telethon mode - go to A49
                Toggle_Field("section", "A1", "hide");
                Toggle_Field("section", "A49", "show");
                Toggle_Field("control", "A49", "show");
                Toggle_Field("back", "A1A", "show");
                Toggle_Field("continue", "A49", "show");

            }
            else
            {
                Toggle_Field("section", "A1", "hide");
                Toggle_Field("section", "A20", "show");
                Toggle_Field("control", "A20", "show");
                Toggle_Field("back", "A1N", "show");
                Toggle_Field("continue", "A20", "show");
            }
        }
        else if (rdValue == "HOLIDAY") {
            // Continue_A1C

            Designation_Toggle_Holiday_Show();

            Toggle_Field("section", "A1", "hide");
            Toggle_Field("section", "A3", "show");
            Toggle_Field("control", "A3", "show");
            Toggle_Field("back", "A1Yb", "show");
            Toggle_Field("continue", "A3", "show");
            Toggle_Field("section", "A3gc", "hide");
            Toggle_Field("section", "A3sc", "hide");
            Toggle_Field("section", "A3high", "hide");
            Toggle_Field("section", "A3other", "show");

            // Harlem Globetrotters Call
            // sectionA3_globe
        }
        else { vfMsg = "Unhandled Response\n\rContact IT with screenshot.."; validationFailed("A1"); }
    }
}
function Escape_A1() {
    ToggleCallType("s", "h", "h");

    Toggle_Field("section", "A1", "hide");
    Toggle_Field("section", "A96", "show");
    Toggle_Field("control", "A96", "show");
    Toggle_Field("back", "A1E", "show");
    Toggle_Field("continue", "A96", "show"); $("#validation_message").html("");
    $("#ListBox96").val(""); // Pledge - Sustainer
}
function Return_A1E() {
    ToggleCallType("s", "s", "s");

    Toggle_Field("section", "A1", "show");
    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("back", "A1E", "hide");
    Toggle_Field("continue", "A96", "hide");
    Toggle_Field("clear", "A96C", "hide");
}
function Return_A1Y() {
    Toggle_Field("section", "A1", "show");
    Toggle_Field("section", "A2", "hide");
    Toggle_Field("control", "A2", "hide");
    Toggle_Field("back", "A1Y", "hide");
    Toggle_Field("continue", "A2", "hide");
}
function Return_A1N() {
    Toggle_Field("section", "A1", "show");
    Toggle_Field("section", "A20", "hide");
    Toggle_Field("control", "A20", "hide");
    Toggle_Field("back", "A1N", "hide");
    Toggle_Field("continue", "A20", "hide");
}
function Return_A1Yb() {
    Toggle_Field("section", "A1", "show");
    Toggle_Field("section", "A3", "hide");
    Toggle_Field("control", "A3", "hide");
    Toggle_Field("back", "A1Yb", "hide");
    Toggle_Field("continue", "A3", "hide");
    Toggle_Field("section", "A3gc", "hide");
    Toggle_Field("section", "A3sc", "hide");
    Toggle_Field("section", "A3high", "hide");
    Toggle_Field("section", "A3other", "hide");
}
function Return_A1A() {
    Toggle_Field("section", "A1", "show");
    Toggle_Field("section", "A49", "hide");
    Toggle_Field("control", "A49", "hide");
    Toggle_Field("back", "A1A", "hide");
    Toggle_Field("continue", "A49", "hide");
}
function Validate_A2() {
    validationStarted();

    var vfField = "";
    if (!vf_Required("tb2_amount_dollar")) {
        vfMsg = vfMsg + "Dollar Amount is required.\n\r";
        if (vfField.length == 0) { vfField = "tb2_amount_dollar"; }
    }
    if (!vf_Amount("tb2_amount_dollar")) {
        vfMsg = vfMsg + "Dollar Amount must be between 1 and 999999.\n\r";
        if (vfField.length == 0) { vfField = "tb2_amount_dollar"; }
    }
    if (!vf_Required("tb2_amount_cent")) {
        vfMsg = vfMsg + "Cents Amount is required.\n\r";
        if (vfField.length == 0) { vfField = "tb2_amount_cent"; }
    }
    if (!vf_AmountCents("tb2_amount_cent")) {
        vfMsg = vfMsg + "Cent Amount must be between 1 and 99.\n\r";
        if (vfField.length == 0) { vfField = "tb2_amount_cent"; }
    }

    if ($("#cdCompany").val() == "DRTV") {
        Designation_Toggle_DRTV("hide");
        $('input[name="rb3_designation"][value="158"]').prop('checked', true);
        Validate_A3_GoTo('A4');

        var rdValue = "NO";
        if ($("#sectionA2drtv").val() == "DRTV_YES") {
            rdValue = "YES";
        }
    } else if ($("#cdCompany").val() == "Globetrotters") {
        Designation_Toggle_GLOBE("hide");
        $('input[name="rb3_designation"][value="158"]').prop('checked', true);
        Validate_A3_GoTo('A4');

        // var rdValue = "NO";
        var rdValue = $("#RadioButtonList2 input:checked").val();
        //if ($("#sectionA2drtv").val() == "DRTV_YES") {
        //    rdValue = "YES";
        //}
    } else if ($("#cdCompany").val() == "Harvey Telethon" || $("#cdCompany").val() == "Irma Telethon" || $("#cdTelethonMode").val() == "True") {

        // Default Stuff
        var desArray = "35,109,158,179,187,188,189,191"; // 187|HURRICANE HARVEY, 188|HURRICANE IRMA, 189|HURRICANE MARIA, 190|MEXICO EARTHQUAKE
        var desSelected = "158";
        var desName = "Disaster Relief";

        //if ($("#cdCompany").val() == "Harvey Telethon") { desSelected = "187"; desSelected2 = "188"; desSelected3 = "189";}
        //if ($("#cdCompany").val() == "Special Bio-Med Cross") { desSelected = "188"; desSelected2 = ""; }
        //if ($("#cdLine").val() == "800-842-2200 UNIVISION" || $("#cdLine").val() == "800-596-6567 TELEMUNDO") { desSelected = "189"; desSelected2 = "190"; }
        //if (($("#cdLine").val() == "800-842-2200 UNIVISION" || $("#cdLine").val() == "800-596-6567 TELEMUNDO") && $("#cdCompany").val() == "Mexico Earthquake") { desSelected = "190"; desSelected2 = "189"; }
        //if ($("#cdCompany").val() == "Hurricane Maria") { desSelected = "189"; desSelected2 = "187"; desSelected3 = "188"; }

        //Designation_Toggle_Custom("hide", desSelected, desSelected2, desSelected3);
        var cdCompany = $("#cdCompany").val();
        var cdLine = $("#cdLine").val();
        // Designation List - 1st Designation is the 'Selected' - rest is visible

        if ($("#cdCompany").val() == "Harvey Telethon" || $("#cdCompany").val() == "Hurricane Harvey") { desArray = "35,109,158,179,187,188,189,191"; desSelected = "187"; desName = "Hurricane Harvey"; }
        else if ($("#cdCompany").val() == "Irma Telethon" || $("#cdCompany").val() == "Hurricane Irma") { desArray = "35,109,158,179,187,188,189,191"; desSelected = "188"; desName = "Hurricane Irma"; }
        else if ($("#cdCompany").val() == "Hurricane Maria") { desArray = "35,109,158,179,187,188,189,190,191"; desSelected = "189"; desName = "Hurricane Maria"; }
        else if ($("#cdCompany").val() == "Mexico Earthquake") { desArray = "35,109,158,179,187,188,189,190,191"; desSelected = "190"; desName = "Mexico Earthquake"; }
        else if ($("#cdCompany").val() == "Disaster Relief") { desArray = "35,109,158,179,187,188,189,191"; desSelected = "158"; desName = "Disaster Relief"; }
        else if ($("#cdCompany").val() == "WONIG") { desArray = "35,109,158,179,187,188,189,191"; desSelected = "109"; desName = "Where It's Needed Most"; }
        else if ($("#cdCompany").val() == "Home Fires") { desArray = "35,109,158,179,187,188,189,191"; desSelected = "179"; desName = "Home Fires"; }
        else if ($("#cdCompany").val() == "California Wildfires") { desArray = "35,109,158,179,187,188,189,191"; desSelected = "191"; desName = "California Wildfires"; }
        else if ($("#cdCompany").val() == "Local Red Cross") { desArray = "35,109,158,179,187,188,189,191"; desSelected = "35"; desName = "Your Local Red Cross"; }
        else if (cdCompany.includes("800 RED-CROSS") || cdLine.includes("800 RED-CROSS") || cdCompany.includes("800 HELP-NOW") || cdLine.includes("800 HELP-NOW")) { desArray = "35,109,158,179,187,188,189,191"; desSelected = "158"; desName = "Disaster Relief"; }
        // 109|WHERE THE NEED IS GREATEST, 158|DISASTER RELIEF, 179|HOME FIRES, 35|LOCAL CHAPTER SUPPORT

        $("#sectionA3_disaster_name").html(desName);
        $('input[name="rb3_designation"][value="' + desSelected + '"]').prop('checked', true);
        Designation_Toggle_Array("hide", desArray);

        var rdValue = "NO";
    } else if ($("#cdSource").val() == "_Ansafone") {
        var rdValue = "NO";
    } else {
        var rdValue = $("#RadioButtonList2 input:checked").val();
    }

    if (!rdValue) {
        vfMsg = vfMsg + "Must select an option.\n\r";
    }

    if (vfMsg.length > 0) {
        validationFailed("A2");
        $("#" + vfField).focus();
        $("#" + vfField).focus(function () { this.select(); });
    } else {
        if (rdValue == "YES") {
            //$(".sectionA8_standard").hide();
            //if ($("#cdCompany").val() == "DRTV") {
            //    $(".sectionA43_standard").hide();
            //    $(".sectionA43_drtv").show();
            //} else {
            //    $(".sectionA43_standard").show();
            //    $(".sectionA43_drtv").hide();
            //}
            //Toggle_Field("section", "A2", "hide");
            //Toggle_Field("section", "A43", "show");
            //Toggle_Field("control", "A43", "show");
            //Toggle_Field("back", "A2N", "show");
            //Toggle_Field("continue", "A43", "show");
            //if ($("#cdCompany").val() != "DRTV") {
            //    Toggle_Field("section", "A3sustainer", "show");
            //}

            $("#rdSustainer").val(rdValue);
            $("#rdSustainer_txt").val(rdValue);

            Toggle_Field("section", "A2", "hide");
            Toggle_Field("section", "A3", "show");
            Toggle_Field("control", "A3", "show");
            Toggle_Field("back", "A2Y", "show");
            Toggle_Field("continue", "A3", "show");
            Toggle_Field("section", "A3gc", "hide");
            Toggle_Field("section", "A3sc", "hide");
            Toggle_Field("section", "A3high", "hide");
            Toggle_Field("section", "A3other", "show");
            Toggle_Field("section", "A3sustainer", "hide");
        } else {
            $(".sectionA8_standard").show();
            var amount = $("#tb2_amount_dollar").val();
            if (amount >= 5000) {
                Toggle_Field("section", "A2", "hide");
                Toggle_Field("section", "A3", "show");
                Toggle_Field("control", "A3", "show");
                Toggle_Field("back", "A2Y", "show");
                Toggle_Field("continue", "A3", "show");
                Toggle_Field("section", "A3gc", "show");
                Toggle_Field("section", "A3sc", "show");
                Toggle_Field("section", "A3high", "show");
                Toggle_Field("section", "A3other", "hide");
                Toggle_Field("section", "A3sustainer", "hide");
            } else {
                Toggle_Field("section", "A2", "hide");
                Toggle_Field("section", "A3", "show");
                Toggle_Field("control", "A3", "show");
                Toggle_Field("back", "A2Y", "show");
                Toggle_Field("continue", "A3", "show");
                Toggle_Field("section", "A3gc", "hide");
                Toggle_Field("section", "A3sc", "hide");
                Toggle_Field("section", "A3high", "hide");
                Toggle_Field("section", "A3other", "show");
                Toggle_Field("section", "A3sustainer", "hide");
            }
        }
    }
}
function Validate_A2_Catalog() {
    var hcDesignation = $("#hcDesignation").val();
    var hcSection = $("#hcSection").val()
    $("#tb2_amount_dollar").val("1");
    $('input[name="rb3_designation"][value="' + hcDesignation + '"]').prop('checked', true);
    Validate_A2();
    Validate_A3_GoTo('A' + hcSection);
}
function Return_A2Y() {
    Toggle_Field("section", "A2", "show");
    Toggle_Field("section", "A3", "hide");
    Toggle_Field("control", "A3", "hide");
    Toggle_Field("back", "A2Y", "hide");
    Toggle_Field("continue", "A3", "hide");
    Toggle_Field_Form("sectionA3_sc", "hide");
    Toggle_Field_Form("sectionA3_high", "hide");
    Toggle_Field_Form("sectionA3_other", "hide");
    Toggle_Field("section", "A3sustainer", "hide");
}
function Return_A2N() {
    Toggle_Field("section", "A2", "show");
    Toggle_Field("section", "A43", "hide");
    Toggle_Field("control", "A43", "hide");
    Toggle_Field("back", "A2N", "hide");
    Toggle_Field("continue", "A43", "hide");
    Toggle_Field("section", "A3sustainer", "hide");

    // Both methods work
    // $('input[name="RadioButtonList2"][value="' + 'YES' + '"]').prop('checked', false);
    $("#RadioButtonList2 input").removeAttr('checked');

    $("#rdSustainer").val("");
    $("#rdSustainer_txt").val("");

}
function Return_A2E() {
    ToggleCallType("s", "s", "s");

    Toggle_Field("section", "A2", "show");
    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("back", "A2E", "hide");
    Toggle_Field("continue", "A96", "hide");
}
function Validate_A3() {
    validationStarted();

    //recurring | sustainer
    //$("#rdSustainer").val(rdValue);
    //$("#rdSustainer_txt").val(rdValue);
    var rdValue = $("#rb3_designation input:checked").val();
    var gtGoTo = "";
    if (!rdValue) {
        vfMsg = "You must select a designation.\n\r";
    }

    // Disaster Relief (158) is the only valid designation for a sustainer donation
    if ($("#rdSustainer_txt").val() == "YES" && (rdValue != "35" && rdValue != "109" && rdValue != "158" && rdValue != "179")) {
        vfMsg = "Sustainer Donation\n\rOnly valid designation are standard ones (Disaster, Where It's Needed Most, Local Chapter).";
    }

    if (vfMsg.length > 0) {
        validationFailed("A3");
    } else {
        gtGoTo = $("#gotoA3").val();
        if (gtGoTo == "A4") {
            Toggle_Field("section", "A3", "hide");
            Toggle_Field("section", "A4", "show");
            Toggle_Field("control", "A4", "show");
            Toggle_Field("back", "A3A", "show");
            Toggle_Field("back", "A41B", "hide");
            Toggle_Field("back", "A42A", "hide");
            Toggle_Field("continue", "A4", "show");
        }
        else if (gtGoTo == "A5") {
            Toggle_Field("section", "A3", "hide");
            Toggle_Field("section", "A5", "show");
            Toggle_Field("control", "A5", "show");
            Toggle_Field("back", "A3B", "show");
            Toggle_Field("continue", "A5", "show");

            $("#TextBox6").focus();
            $("#TextBox6").focus(function () { this.select(); });
        }
        else if (gtGoTo == "A6") {
            Toggle_Field("section", "A3", "hide");
            Toggle_Field("section", "A6", "show");
            Toggle_Field("control", "A6", "show");
            Toggle_Field("back", "A3c", "show");
            Toggle_Field("continue", "A6", "show");
        }

            //[BEGIN] 10.14.2014 - Added for holiday catalog 
        else if (gtGoTo == "A41") {
            Toggle_Field("section", "A3", "hide");
            Toggle_Field("section", "A41", "show");
            Toggle_Field("control", "A41", "show");
            Toggle_Field("back", "A3d", "show");
            Toggle_Field("continue", "A41", "show");
        }
            //[END] 10.14.2014 - Added for holiday catalog 

        else if (gtGoTo == "") { }
        else { vfMsg = "Unhandled Response\n\rContact IT with screenshot.."; validationFailed("A3"); }
    }
}
function Validate_A3_GoTo(value) {
    // Rename to Continue_A3_GoTo [?]
    $("#gotoA3B").html("GoTo >> " + value);
    $("#gotoA3").val(value);
}
function Return_A3A() {
    Toggle_Field("section", "A3", "show");
    Toggle_Field("section", "A4", "hide");
    Toggle_Field("control", "A4", "hide");
    Toggle_Field("back", "A3c", "hide");
    Toggle_Field("continue", "A4", "hide");
}
function Return_A3B() {
    Toggle_Field("section", "A3", "show");
    Toggle_Field("section", "A5", "hide");
    Toggle_Field("control", "A5", "hide");
    Toggle_Field("back", "A3B", "hide");
    Toggle_Field("continue", "A5", "hide");
}
function Return_A3c() {
    Toggle_Field("section", "A3", "show");
    Toggle_Field("section", "A6", "hide");
    Toggle_Field("control", "A6", "hide");
    Toggle_Field("back", "A3c", "hide");
    Toggle_Field("continue", "A6", "hide");
}
function Validate_A4() {
    validationStarted();

    var rdValue = $("#rb4_card_type input:checked").val();
    if (!rdValue) {
        vfMsg = "You must select an option above.";
        validationFailed("A4");
    }
    else if (rdValue == "CHECK") {
        var typeDNIS = $("#typeDNIS").val();
        // If DRTV || Holiday we go to A45 not A5
        // typeDNIS | .va
        // holiday catalog too - validate_A3(
        // cdHCDesignationId
        var rdDes = $("#rb3_designation input:checked").val();
        var hDesignationID = $("#cdHCDesignationId").val();

        if (typeDNIS == "DRTV" || rdDes == hDesignationID) {
            $("#tb45_amount_dollar").val($("#tb2_amount_dollar").val())
            $("#tb45_amount_cent").val($("#tb2_amount_cent").val())

            Toggle_Field("section", "A4", "hide");
            Toggle_Field("section", "A45", "show");
            Toggle_Field("control", "A45", "show");
            Toggle_Field("back", "A4E", "show");
            Toggle_Field("continue", "A45", "show");
        } else {
            Toggle_Field("section", "A4", "hide");
            Toggle_Field("section", "A5", "show");
            Toggle_Field("control", "A5", "show");
            Toggle_Field("back", "A4Y", "show");
            Toggle_Field("continue", "A5", "show");
        }
    } else {
        // Credit Card
        // If DRTV - We send to A43
        // $("#A2_step_label").html("A43");
        // validate_A4
        var rdValueDRTV = $("#rb1_options input:checked").val();
        var typeDNIS = $("#typeDNIS").val();
        if (typeDNIS == "DRTV" && rdValueDRTV == "DRTV_YES") {
            $(".sectionA8_standard").hide(); // ?
            if ($("#cdCompany").val() == "DRTV") {
                $(".sectionA43_standard").hide();
                $(".sectionA43_drtv").show();
            } else {
                $(".sectionA43_standard").show();
                $(".sectionA43_drtv").hide();
            }

            if ($("#cdCompany").val() != "DRTV") {
                Toggle_Field("section", "A3sustainer", "show");
            }
            //$("#rdSustainer").val(rdValue);
            //$("#rdSustainer_txt").val(rdValue);

            Toggle_Field("section", "A4", "hide");
            Toggle_Field("section", "A43", "show");
            Toggle_Field("control", "A43", "show");
            Toggle_Field("back", "A4F", "show"); // back_A4E | back_A4F | Return_A4E | Return_A4F
            // Toggle_Field("back", "A2N", "show");
            Toggle_Field("continue", "A43", "show");
        } else {
            Toggle_Field("section", "A4", "hide");
            Toggle_Field("section", "A7", "show");
            Toggle_Field("control", "A7", "show");
            Toggle_Field("back", "A4N", "show");
            Toggle_Field("continue", "A7", "show");
            $("#tb7_card_number").focus();
            //$("#tb7_card_number").focus(function() { this.select(); });
        }
    }
}
function Return_A4Y() {
    Toggle_Field("section", "A4", "show");
    Toggle_Field("section", "A5", "hide");
    Toggle_Field("control", "A5", "hide");
    Toggle_Field("back", "A4Y", "hide");
    Toggle_Field("continue", "A5", "hide");
}
function Return_A4N() {
    Toggle_Field("section", "A4", "show");
    Toggle_Field("section", "A7", "hide");
    Toggle_Field("control", "A7", "hide");
    Toggle_Field("back", "A4N", "hide");
    Toggle_Field("continue", "A7", "hide");
}
function Return_A4E() {
    Toggle_Field("section", "A4", "show");
    Toggle_Field("section", "A45", "hide");
    Toggle_Field("control", "A45", "hide");
    Toggle_Field("back", "A4E", "hide");
    Toggle_Field("continue", "A45", "hide");
}
function Return_A4F() {
    Toggle_Field("section", "A4", "show");
    Toggle_Field("section", "A43", "hide");
    Toggle_Field("control", "A43", "hide");
    Toggle_Field("back", "A4F", "hide");
    Toggle_Field("continue", "A43", "hide");
}
function Validate_A5() {
    ToggleCallType("h", "h", "s");

    Toggle_Field("section", "A5", "hide");
    Toggle_Field("section", "A96", "show");
    Toggle_Field("control", "A96", "show");
    Toggle_Field("back", "A5Y", "show");
    Toggle_Field("continue", "A96", "show"); $("#validation_message").html("");
    var amount = $("#tb2_amount_dollar").val();
    if (amount >= 5000) {} else {} // Why?

    if ($("#rdSustainer").val() == "YES") {
        $("#ListBox96").val("47"); // Pledge - Sustainer
    } else {
        $("#ListBox96").val("42"); // Pledge - One Time
    }

}
function Return_A5Y() {
    ToggleCallType("s", "s", "s");

    Toggle_Field("section", "A5", "show");
    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("back", "A5Y", "hide");
    Toggle_Field("continue", "A96", "hide");
}
function Validate_A45() {
    validationStarted();

    Toggle_Field("section", "A0", "hide");
    $("#validation_message").html(""); // Why?
    $("#lblValidation").html(""); // Why?
    $("#ResponseSQL").html(""); // Why?
    
    var vfField = "";
    var vContinue = true;

    // var rdValue = $("#tb45_sustainer_optin input:checked").val();
    var tb45_sustainer_optin = $("#tb45_sustainer_optin input:checked").val();

    if (vf_RequiredRadio("tb45_sustainer_optin")) {
    } else {
    }

    if (!tb45_sustainer_optin) {
        vfMsg = vfMsg + "Must select type of pledge (Sustainer or One-Time).\n\r";
    } else if (tb45_sustainer_optin == "NO") {
        vContinue = false;
    }
    if (vContinue) {
        if (!vf_Required("tb45_first_name")) {
            vfMsg = vfMsg + "First Name is required.\n\r";
            if (vfField.length == 0) { vfField = "tb45_first_name"; }
        } else if (!vf_Len("tb45_first_name", 2, 100)) {
            vfMsg = vfMsg + "First Name must be between 2 and 100 characters.\n\r";
            if (vfField.length == 0) { vfField = "tb45_first_name"; }
        }
        if (!vf_Required("tb45_last_name")) {
            vfMsg = vfMsg + "Last Name is required.\n\r";
            if (vfField.length == 0) { vfField = "tb45_last_name"; }
        } else if (!vf_Len("tb45_last_name", 3, 100)) {
            vfMsg = vfMsg + "Last Name must be between 3 and 100 characters.\n\r";
            if (vfField.length == 0) { vfField = "tb45_last_name"; }
        }
        if (!vf_Len("tb45_business_name", 0, 50)) {
            vfMsg = vfMsg + "Business Name must be 50 characters or less.\n\r";
            if (vfField.length == 0) { vfField = "tb45_business_name"; }
        }
        if (!vf_Required("tb45_address1")) {
            vfMsg = vfMsg + "Address is required.\n\r";
            if (vfField.length == 0) { vfField = "tb45_address1"; }
        } else if (!vf_Len("tb45_address1", 5, 100)) {
            vfMsg = vfMsg + "Address must be between 5 and 100 characters.\n\r";
            if (vfField.length == 0) { vfField = "tb45_address1"; }
        }
        if (!vf_Len("tb45_suite_number", 0, 25)) {
            vfMsg = vfMsg + "Suite Number must be 25 characters or less.\n\r";
            if (vfField.length == 0) { vfField = "tb45_suite_number"; }
        }
        if (!vf_Required("tb45_postal_code")) {
            vfMsg = vfMsg + "Postal Code is required.\n\r";
            if (vfField.length == 0) { vfField = "tb45_postal_code"; }
        } else if (!vf_Len("tb45_postal_code", 5, 20)) {
            vfMsg = vfMsg + "Postal Code must be between 5 and 20 characters.\n\r";
            if (vfField.length == 0) { vfField = "tb45_postal_code"; }
        }
        if (!vf_Required("tb45_city")) {
            vfMsg = vfMsg + "City is required.\n\r";
            if (vfField.length == 0) { vfField = "tb45_city"; }
        } else if (!vf_Len("tb45_city", 3, 25)) {
            vfMsg = vfMsg + "City must be between 5 and 25 characters.\n\r";
            if (vfField.length == 0) { vfField = "tb45_city"; }
        }
        if (!vf_Required("tb45_country")) {
            vfMsg = vfMsg + "Country is required.\n\r";
            if (vfField.length == 0) { vfField = "tb45_country"; }
        } else {
            var country = $("#tb45_country").val();
            if (country == "USA") {
                if (!vf_Required("tb45_state")) {
                    vfMsg = vfMsg + "State is required.\n\r";
                    if (vfField.length == 0) { vfField = "tb45_state"; }
                }
            } else if (country == "CAN") {
                if (!vf_Required("tb45_stateca")) {
                    vfMsg = vfMsg + "Province is required.\n\r";
                    if (vfField.length == 0) { vfField = "tb45_stateca"; }
                }
            } else {
                if (!vf_Required("tb45_stateother")) {
                    vfMsg = vfMsg + "State field is required.\n\r";
                    if (vfField.length == 0) { vfField = "tb45_stateother"; }
                }
            }
        }

        if (!vf_Required("tb45_amount_dollar")) {
            vfMsg = vfMsg + "Dollar Amount is required.\n\r";
            if (vfField.length == 0) { vfField = "tb45_amount_dollar"; }
        }
        if (!vf_Amount("tb45_amount_dollar")) {
            vfMsg = vfMsg + "Dollar Amount must be between 1 and 999999.\n\r";
            if (vfField.length == 0) { vfField = "tb45_amount_dollar"; }
        }
        if (!vf_Required("tb45_amount_cent")) {
            vfMsg = vfMsg + "Cents Amount is required.\n\r";
            if (vfField.length == 0) { vfField = "tb45_amount_cent"; }
        }
        if (!vf_AmountCents("tb45_amount_cent")) {
            vfMsg = vfMsg + "Cent Amount must be between 1 and 99.\n\r";
            if (vfField.length == 0) { vfField = "tb45_amount_cent"; }
        }
    }
    if (vfMsg.length > 0) {
        $("#" + vfField).focus();
        $("#" + vfField).focus(function () { this.select(); });
        $("#lblValidation").html("Latest Validation Errors:<br />" + vfMsg.replace(/\n\r/g, "<br />"));
        validationFailed("A45");
    } else {
        $("#tb2_amount_dollar").val($("#tb45_amount_dollar").val())
        $("#tb2_amount_cent").val($("#tb45_amount_cent").val())

        if (tb45_sustainer_optin == "NO") {
            Toggle_Field("section", "A45", "hide");
            Toggle_Field("section", "A48", "show");
            Toggle_Field("control", "A48", "show");
            Toggle_Field("back", "A45N", "show");
            Toggle_Field("continue", "A48", "show");

        } else {
            ToggleCallType("h", "h", "s");

            Toggle_Field("section", "A45", "hide");
            Toggle_Field("section", "A96", "show");
            Toggle_Field("control", "A96", "show");
            Toggle_Field("back", "A45Y", "show");
            Toggle_Field("continue", "A96", "show");

            if (tb45_sustainer_optin == "YES_Sustainer") {
                $("#ListBox96").val("47"); // Pledge - Sustainer
            } else {
                $("#ListBox96").val("42"); // Pledge - One Time
            }
        }
    }
}
function Return_A45Y() {
    ToggleCallType("s", "s", "s");

    Toggle_Field("section", "A45", "show");
    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("back", "A45Y", "hide");
    Toggle_Field("continue", "A96", "hide");

}
function Return_A45N() {
    Toggle_Field("section", "A45", "show");
    Toggle_Field("section", "A48", "hide");
    Toggle_Field("control", "A48", "hide");
    Toggle_Field("back", "A45N", "hide");
    Toggle_Field("continue", "A48", "hide");

    Toggle_Field("control", "A45", "show");
    Toggle_Field("continue", "A45", "show");
    Toggle_Field("back", "A48Y", "hide");
}
function Validate_A48() {
    validationStarted();
    var rdValue = $("#tb48_envelope input:checked").val();

    if (!rdValue) {
        vfMsg = "You must select a designation.\n\r";
    }

    if (vfMsg.length > 0) {
        validationFailed("A48");
    } else {
        $("#validation_message").html("");
        if (rdValue == "YES") {
            Toggle_Field("section", "A48", "hide");
            Toggle_Field("section", "A45", "show");
            Toggle_Field("control", "A45", "show");
            Toggle_Field("back", "A48N", "show");
            Toggle_Field("continue", "A45", "show");

        } else {
            ToggleCallType("h", "h", "s");

            Toggle_Field("section", "A48", "hide");
            Toggle_Field("section", "A96", "show");
            Toggle_Field("control", "A96", "show");
            Toggle_Field("back", "A48Y", "show");
            Toggle_Field("continue", "A96", "show");
            var amount = $("#tb2_amount_dollar").val();
            if ($("#rdSustainer").val() == "YES") {
                $("#ListBox96").val("47"); // Pledge - Sustainer
            } else {
                $("#ListBox96").val("42"); // Pledge - One Time
            }

        }
    }
}
function Return_A48N() {
    Toggle_Field("section", "A48", "show");
    Toggle_Field("section", "A45", "hide");
    Toggle_Field("control", "A45", "hide");
    Toggle_Field("back", "A48N", "hide");
    Toggle_Field("continue", "A45", "hide");

    Toggle_Field("control", "A48", "show");
    Toggle_Field("continue", "A48", "show");
}
function Return_A48Y() {
    ToggleCallType("s", "s", "s");

    Toggle_Field("section", "A48", "show");
    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("back", "A48Y", "hide");
    Toggle_Field("continue", "A96", "hide");
}
function Validate_A6() {
    ToggleCallType("h", "h", "s");

    Toggle_Field("section", "A6", "hide");
    Toggle_Field("section", "A96", "show");
    Toggle_Field("control", "A96", "show");
    Toggle_Field("back", "A6Y", "show");
    Toggle_Field("continue", "A96", "show"); $("#validation_message").html("");
    $("#ListBox96").val("43"); // Special Cause Pledge
}
function Return_A6Y() {
    ToggleCallType("s", "s", "s");

    Toggle_Field("section", "A6", "show");
    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("back", "A6Y", "hide");
    Toggle_Field("continue", "A96", "hide");
}
function Validate_A7() {
    validationStarted();

    var vfField = "";
    if (!vf_Required("tb7_card_number")) {
        vfMsg = vfMsg + "Card Number is required.\n\r";
        if (vfField.length == 0) { vfField = "tb7_card_number"; }
    } else {
        // Mod 10
        if (!vf_CreditCard("tb7_card_number")) {
            vfMsg = vfMsg + "Card Number is not a valid card.\n\r";
            if (vfField.length == 0) { vfField = "tb7_card_number"; }
        }
    }
    // Check for expired card
    if (vf_Required("tb7_card_month") && vf_Required("tb7_card_year")) {
        if (!vf_CardExpired("tb7_card_month", "tb7_card_year")) {
            vfMsg = vfMsg + "Card Card is expired.\n\r";
        }
    } else {
        vfMsg = vfMsg + "Card Expiration Month and Year are required.\n\r";
    }

    if (!vf_Required("tb7_first_name")) {
        vfMsg = vfMsg + "First Name is required.\n\r";
        if (vfField.length == 0) { vfField = "tb7_first_name"; }
    } else if (!vf_Len("tb7_first_name", 2, 100)) {
        vfMsg = vfMsg + "First Name must be between 2 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb7_first_name"; }
    }
    if (!vf_Len("tb7_middle_initial", 0, 5)) {
        vfMsg = vfMsg + "Middle Initial must be 5 characters or less.\n\r";
        if (vfField.length == 0) { vfField = "tb7_middle_initial"; }
    }
    if (!vf_Required("tb7_last_name")) {
        vfMsg = vfMsg + "Last Name is required.\n\r";
        if (vfField.length == 0) { vfField = "tb7_last_name"; }
    } else if (!vf_Len("tb7_last_name", 2, 100)) {
        vfMsg = vfMsg + "Last Name must be between 2 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb7_last_name"; }
    }

    // Holiday Catalog - Alternate Shipping Address
    // var showcatalog = ($("#hcShowHolidayCatalog").val() == "true") ? true : false;
    if ($("#hcAlternateAddressOffer").val() == "YES") {
        $("#field_input_A8_altadd").show();
    }
    else {
        $("#field_input_A8_altadd").hide();
    }

    if (vfMsg.length > 0) {
        validationFailed("A7");
        $("#" + vfField).focus();
        $("#" + vfField).focus(function () { this.select(); });
    } else {
        $('input[name="tb8_email_receipt"][value="NO"]').prop('checked', true);
        $("#field_input_A8_receiptEmail").hide();
        $("#field_input_A8_receiptEmail2").hide();


        if ($("#cdCompany").val() == "Harvey Telethon" || $("#cdCompany").val() == "Irma Telethon" || $("#cdTelethonMode").val() == "True") {
            // Hide additional Phone & Email
            //$("#tb8_phone2_add").val("NO");
            $('input[name="tb8_phone2_add"][value="NO"]').prop('checked', true);
            $("#field_input_A8_addPhone").hide();
            // $('input[name="tb8_email_optin"][value="NO"]').prop('checked', true);
            // $("#field_input_A8_addEmail").hide();
            // $("#field_input_A8_addEmail2").hide();            
        }


        Toggle_Field("section", "A7", "hide");
        Toggle_Field("section", "A8", "show");
        Toggle_Field("control", "A8", "show");
        Toggle_Field("back", "A7Y", "show");
        Toggle_Field("continue", "A8", "show");
        $("#tb8_address1").focus();
        $("#tb8_address1").focus(function () { this.select(); });
    }
}
function Return_A7Y() {
    Toggle_Field("section", "A7", "show");
    Toggle_Field("section", "A8", "hide");
    Toggle_Field("control", "A8", "hide");
    Toggle_Field("back", "A7Y", "hide");
    Toggle_Field("continue", "A8", "hide");
    $("#tb7_card_number").focus();
    $("#tb7_card_number").focus(function () { this.select(); });
}
function Validate_A8() {
    validationStarted();

    var vfField = "";
    //if (!vf_RequiredRadio("tb8_biz_toggle")) {
    //    vfMsg = vfMsg + "Business Toggle is required.\n\r";
    //}
    if (!vf_Len("tb8_business_name", 0, 50)) {
        vfMsg = vfMsg + "Business Name must be 100 characters or less.\n\r";
        if (vfField.length == 0) { vfField = "tb8_business_name"; }
    }
    if (!vf_Required("tb8_address1")) {
        vfMsg = vfMsg + "Address is required.\n\r";
        if (vfField.length == 0) { vfField = "tb8_address1"; }
    } else if (!vf_Len("tb8_address1", 5, 100)) {
        vfMsg = vfMsg + "Address must be between 5 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb8_address1"; }
    }
    if (!vf_Len("tb8_suite_number", 0, 25)) {
        vfMsg = vfMsg + "Suite Number must be 25 characters or less.\n\r";
        if (vfField.length == 0) { vfField = "tb8_suite_number"; }
    }
    if (!vf_Required("tb8_country")) {
        vfMsg = vfMsg + "Country is required.\n\r";
        if (vfField.length == 0) { vfField = "tb8_country"; }
    } else {
        var country = $("#tb8_country").val();
        if (country == "USA") {
            if (!vf_Required("tb8_state")) {
                vfMsg = vfMsg + "State is required.\n\r";
                if (vfField.length == 0) { vfField = "tb8_state"; }
            }
        } else if (country == "CAN") {
            if (!vf_Required("tb8_stateca")) {
                vfMsg = vfMsg + "Province is required.\n\r";
                if (vfField.length == 0) { vfField = "tb8_stateca"; }
            }
        } else {
            if (!vf_Required("tb8_stateother")) {
                vfMsg = vfMsg + "State field is required.\n\r";
                if (vfField.length == 0) { vfField = "tb8_stateother"; }
            }
        }
    }
    if (!vf_Required("tb8_postal_code")) {
        vfMsg = vfMsg + "Postal Code is required.\n\r";
        if (vfField.length == 0) { vfField = "tb8_postal_code"; }
        // If Postal code present it should be between 5 or 10 with a dash for USA
        // For NON-US it should be between 5 to 10
    } else {
        var country = $("#tb8_country").val();
        if (country == "USA") {
            if (!vf_PostalCode_USA("tb8_postal_code")) {
                vfMsg = vfMsg + "USA Postal Code must be 5 digits, or 10 digits if using +4.\n\r";
                if (vfField.length == 0) { vfField = "tb8_postal_code"; }
            }
        } else {
            if (!vf_Len("tb8_postal_code", 5, 20)) {
                vfMsg = vfMsg + "Postal Code must be between 5 and 20 characters.\n\r";
                if (vfField.length == 0) { vfField = "tb8_postal_code"; }
            }
        }
    }
    if (!vf_Required("tb8_city")) {
        vfMsg = vfMsg + "City is required.\n\r";
        if (vfField.length == 0) { vfField = "tb8_city"; }
    } else if (!vf_Len("tb8_city", 3, 25)) {
        vfMsg = vfMsg + "City must be between 5 and 25 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb8_city"; }
    }
    if (vf_Required("tb8_phone")) {
        if (!vf_Phone("tb8_phone")) {
            vfMsg = vfMsg + "Phone: must be a valid phone number.\n\r";
            if (vfField.length == 0) { vfField = "tb8_phone"; }
        }
        if (!vf_RequiredRadio("tb8_phone_type")) {
            vfMsg = vfMsg + "Phone: must select a phone type.\n\r";
            if (vfField.length == 0) { vfField = "tb8_phone"; }
        }
        if (!vf_RequiredRadio("tb8_phone_optin")) {
            vfMsg = vfMsg + "Phone: must select phone opt in or opt out.\n\r";
            if (vfField.length == 0) { vfField = "tb8_phone"; }
        }
    }
    if (vf_RequiredRadio("tb8_phone2_add")) {
        var Phone2Tgl = $("#tb8_phone2_add input:checked").val();
        if (Phone2Tgl == "YES") {
            if (!vf_Required("tb8_phone2")) {
                vfMsg = vfMsg + "Phone2: is required if toggled on.\n\r";
                if (vfField.length == 0) { vfField = "tb8_phone2"; }
            } else {
                if (!vf_Phone("tb8_phone2")) {
                    vfMsg = vfMsg + "Phone2: must be a valid phone number.\n\r";
                    if (vfField.length == 0) { vfField = "tb8_phone2"; }
                }
            }
            if (!vf_RequiredRadio("tb8_phone_type")) {
                vfMsg = vfMsg + "Phone2: must select a phone type.\n\r";
                if (vfField.length == 0) { vfField = "tb8_phone2"; }
            }
        }
    } else {
        vfMsg = vfMsg + "Phone2: must select if providing or not.\n\r";
    }
    if (vf_RequiredRadio("tb8_email_receipt")) {
        var EmailTgl = $("#tb8_email_receipt input:checked").val();
        if (EmailTgl == "YES") {
            if (!vf_Required("tb8_email")) {
                vfMsg = vfMsg + "Email is required if requesting receipt.\n\r";
                if (vfField.length == 0) { vfField = "tb8_email"; }
            }
            if (!vf_RequiredRadio("tb8_email_optin")) {
                vfMsg = vfMsg + "Email opt in is required if requesting receipt.\n\r";
                if (vfField.length == 0) { vfField = "tb8_email_optin"; }
            }
        }
    } else {
        if ($("#rdSustainer").val() != "YES") {
            vfMsg = vfMsg + "Must select if email receipt is desired or not.\n\r";
            if (vfField.length == 0) { vfField = "tb8_email"; }
        }
    }
    if (!vf_Email("tb8_email")) {
        vfMsg = vfMsg + "Email must be a valid email address.\n\r";
        if (vfField.length == 0) { vfField = "tb8_email"; }
    }
    if (!vf_Email("tb8_email2")) {
        vfMsg = vfMsg + "Secondary Email must be a valid email address.\n\r";
        if (vfField.length == 0) { vfField = "tb8_email2"; }
    }
    if (vf_RequiredRadio("tb8_email_optin")) {
        var EmailOptIn = $("#tb8_email_optin input:checked").val();
        if (EmailOptIn == "YES") {
            if (!vf_Required("tb8_email2")) {
                vfMsg = vfMsg + "Email 2 is required if requesting information.\n\r";
                if (vfField.length == 0) { vfField = "tb8_email2"; }
            }
        }
    }

    if ($("#hcAlternateAddressOffer").val() == "YES") {
        if (!vf_RequiredRadio("tb8_alt_address")) {
            vfMsg = vfMsg + "Holiday Alternate Address Toggle is required.\n\r";
        } else {
            var rdValueAltAdd = $("#tb8_alt_address input:checked").val();
            $("#hcAlternateAddressUse").val(rdValueAltAdd);
        }
    }
    else {
        $("#hcAlternateAddressUse").val("NO");
    }

    if (vfMsg.length > 0) {
        $("#" + vfField).focus().select();
        // $("#" + vfField).focus(function () { this.select(); });
        $("#ResponseSQL").html("");
        $("#lblValidation").html("Latest Validation Errors:<br />" + vfMsg.replace(/\n\r/g, "<br />"));
        // Replace All http://dumpsite.com/forum/index.php?topic=4.msg8#msg8
        validationFailed("A8");
    } else {
        Toggle_Field("section", "A0", "hide");
        if ($("#hcAlternateAddressUse").val() == "YES") {
            // We use alternate address, so we need to capture it - Move on to A47 then A9
            $("#tb47_first_name").val($("#tb7_first_name").val());
            $("#tb47_last_name").val($("#tb7_last_name").val());

            Toggle_Field("section", "A8", "hide");
            Toggle_Field("section", "A47", "show");
            Toggle_Field("control", "A47", "show");
            Toggle_Field("back", "A8N", "show");
            Toggle_Field("continue", "A47", "show");
            $("#tb47_first_name").focus();
            $("#tb47_first_name").focus(function () { this.select(); });
        } else {
            // Move on to A9
            if ($("#cdCompany").val() == "DRTV") {
                $("#sectionA9_drtv").show();
            }
            Toggle_Field("section", "A8", "hide");
            Toggle_Field("section", "A9", "show");
            Toggle_Field("control", "A9", "show");
            Toggle_Field("back", "A8Y", "show");
            Toggle_Field("continue", "A9", "show");
        }

        Populate_A9();
    }
}
function Return_A8Y() {
    Toggle_Field("section", "A8", "show");
    Toggle_Field("section", "A9", "hide");
    Toggle_Field("control", "A9", "hide");
    Toggle_Field("back", "A8Y", "hide");
    Toggle_Field("continue", "A9", "hide");
    $("#tb8_address1").focus();
    $("#tb8_address1").focus(function () { this.select(); });
}
function Return_A8N() {
    Toggle_Field("section", "A8", "show");
    Toggle_Field("section", "A47", "hide");
    Toggle_Field("control", "A47", "hide");
    Toggle_Field("back", "A8N", "hide");
    Toggle_Field("continue", "A47", "hide");
    $("#tb8_address1").focus();
    $("#tb8_address1").focus(function () { this.select(); });
}
function Validate_A9() {
    validationStarted();
    var sustainer = false;
    if ($("#rdSustainer").val() == "YES") { sustainer = true; }

    if (sustainer) {
        var rdValue = $("#tb9_drtv_receipt_mode input:checked").val();
        if (!rdValue) {
            vfMsg = "You must select an option above.";
            validationFailed("A9");
        } else {
            ToggleCallType("h", "h", "s");

            $("#rdReceiptFrequency").val(rdValue);

            Toggle_Field("section", "A9", "hide");
            Toggle_Field("section", "A96", "show");
            Toggle_Field("control", "A96", "show");
            Toggle_Field("back", "A9Y", "show");
            Toggle_Field("continue", "A96", "show");
            $("#validation_message").html("");

            //Toggle_Field("back", "A2E", "show");
            Toggle_Field("back", "A1E", "show");

            //$("#ListBox96").val("41"); // Donation: One Time
            $("#ListBox96").val("46"); // Donation: Sustainer

            Toggle_Field("section", "A96donation", "show");


        }
    } else {
        ToggleCallType("h", "h", "s");

        Toggle_Field("section", "A9", "hide");
        Toggle_Field("section", "A96", "show");
        Toggle_Field("control", "A96", "show");
        Toggle_Field("back", "A9Y", "show");
        Toggle_Field("continue", "A96", "show");
        $("#validation_message").html("");

        //Toggle_Field("back", "A2E", "show");
        Toggle_Field("back", "A1E", "show");

        $("#ListBox96").val("41"); // Donation: One Time

        Toggle_Field("section", "A96donation", "show");
    }
}
function Return_A9Y() {
    ToggleCallType("s", "s", "s");

    Toggle_Field("section", "A9", "show");
    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("back", "A9Y", "hide");
    Toggle_Field("continue", "A96", "hide");
    Toggle_Field("section", "A96donation", "hide");
    Toggle_Field("clear", "A96C", "hide");

    Populate_A9();

}
function Populate_A9() {

    var sustainer = false;
    if ($("#rdSustainer").val() == "YES") { sustainer = true; }
    if (sustainer) {
        $("#donation_onetime").hide();
        $("#donation_sustainer").show();
        var rdValue = $("#confirmation_02").html();

        if (rdValue.indexOf("{designation_name}") > -1) {
            $("#storedvalue_A9").html(rdValue);
        } else {
            var rdValue = $("#storedvalue_A9").html();
        }
        var sustainer_date = $("#rdChargeDate").val();
        if (sustainer_date == "1") { sustainer_date = "1<sup>st</sup>"; }
        else if (sustainer_date == "15") { sustainer_date = "15<sup>th</sup>"; }

        var fullname = $("#tb7_first_name").val() + " " + $("#tb7_last_name").val();
        var designation = DesignationFromID($("#rb3_designation input:checked").val());
        var designation_spanish = DesignationFromID_Spanish($("#rb3_designation input:checked").val());

        rdValue = rdValue.replace(/{donor_name}/g, fullname);
        rdValue = rdValue.replace(/{designation_name}/g, designation);
        rdValue = rdValue.replace(/{designation_name_spanish}/g, designation_spanish);

        rdValue = rdValue.replace(/{sustainer_date}/g, sustainer_date);
        rdValue = rdValue.replace(/{donation_amount}/g, $("#tb2_amount_dollar").val() + "." + $("#tb2_amount_cent").val());
        $("#confirmation_02").html(rdValue);
    } else {
        $("#donation_onetime").show();
        $("#donation_sustainer").hide();

        var rdValue = $("#confirmation_01").html();
        if (rdValue.indexOf("{designation_name}") > -1) {
            $("#storedvalue_A9").html(rdValue);
        } else {
            var rdValue = $("#storedvalue_A9").html();
        }

        var fullname = $("#tb7_first_name").val() + " " + $("#tb7_last_name").val();
        var designation = DesignationFromID($("#rb3_designation input:checked").val());
        var designation_spanish = DesignationFromID_Spanish($("#rb3_designation input:checked").val());

        rdValue = rdValue.replace(/{donor_name}/g, fullname);
        rdValue = rdValue.replace(/{donation_amount}/g, $("#tb2_amount_dollar").val() + "." + $("#tb2_amount_cent").val());
        rdValue = rdValue.replace(/{designation_name}/g, designation);
        rdValue = rdValue.replace(/{designation_name_spanish}/g, designation_spanish);

        $("#confirmation_01").html(rdValue);
    }

    // Do LANGUAGE swap | Spanish | languageToggle | #tglLanguage
    if ($("#tglLanguage").text() == "English") { languageEnglish(); } else { languageSpanish(); }
    // languageToggle(); languageToggle();
}
function Validate_A20() {
    validationStarted();

    //var rdValue = $("#RadioButtonList20 input:checked").val();
    var gtGoTo = $("#RadioButtonList20 input:checked").val();
    if (!gtGoTo) {
        vfMsg = "You must select a question from the list.";
        validationFailed("A20");
    }
    else {
        // If gtGoTo == 30 && DRTV > A46
        if (gtGoTo == "30" && $("#cdCompany").val() == "DRTV") { gtGoTo = "46"; }

        Toggle_Field("section", "A20", "hide");
        Toggle_Field("section", "A" + gtGoTo, "show");
        Toggle_Field("control", "A" + gtGoTo, "show");
        Toggle_Field("back", "A20_" + gtGoTo, "show");
        Toggle_Field("continue", "A" + gtGoTo, "show");
        var disposition = "1003";
        if (gtGoTo == "96") {
            ToggleCallType("s", "h", "h");
        }
        if (gtGoTo == "21") { disposition = "1003"; }
        if (gtGoTo == "21") { disposition = "1003"; }
    }
}
function Return_A20(value) {
    var gtGoTo = value;
    if (gtGoTo == "96") {
        ToggleCallType("s", "s", "s");
    }
    Toggle_Field("section", "A20", "show");
    Toggle_Field("section", "A" + gtGoTo, "hide");
    Toggle_Field("control", "A" + gtGoTo, "hide");
    Toggle_Field("back", "A20_" + gtGoTo, "hide");
    Toggle_Field("continue", "A" + gtGoTo, "hide");
}
function Validate_A21(value) {
    ToggleCallType("h", "s", "h");

    var gtGoTo = value;
    Toggle_Field("section", "A" + gtGoTo, "hide");
    Toggle_Field("section", "A96", "show");
    Toggle_Field("control", "A96", "show");
    Toggle_Field("back", "A21Y", "show");
    Toggle_Field("continue", "A96", "show"); $("#validation_message").html("");

    var disposition = DispositionFromSection(gtGoTo);
    $("#ListBox96").val(disposition);

    $("#back_A21Y").val("Back to A" + gtGoTo);
    $("#back_A21Y").attr("onclick", "javascript:Return_A21Y('" + gtGoTo + "')");
}
function Return_A21Y(value) {
    ToggleCallType("s", "s", "s");

    var gtGoTo = value;
    Toggle_Field("section", "A" + gtGoTo, "show");
    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("back", "A21Y", "hide");
    Toggle_Field("continue", "A96", "hide");

    $("#back_A21Y").val("Back to A##");
    $("#back_A21Y").attr("onclick", "javascript:Return_AY('0')");
}
function Validate_A22(value) {
    validationStarted();

    var field = "TextBox" + value;
    if (!vf_Required(field)) {
        vfMsg = "Postal Code is required.";
        validationFailed("A22");

        $("#" + field).focus();
    } else {
        ToggleCallType("h", "s", "h");

        var gtGoTo = value;
        Toggle_Field("section", "A" + gtGoTo, "hide");
        Toggle_Field("section", "A96", "show");
        Toggle_Field("control", "A96", "show");
        Toggle_Field("back", "A21Y", "show");
        Toggle_Field("continue", "A96", "show"); $("#validation_message").html("");

        var disposition = DispositionFromSection(gtGoTo);
        $("#ListBox96").val(disposition);

        $("#back_A21Y").val("Back to A" + gtGoTo);
        $("#back_A21Y").attr("onclick", "javascript:Return_A21Y('" + gtGoTo + "')");
    }
}
function Validate_A24() {
    validationStarted();

    var rdValue = $("#RadioButtonList24 input:checked").val();
    if (!rdValue) {
        vfMsg = "You must select an option above.";
        validationFailed("A24");
    }
    else if (rdValue == "USED") {
        Toggle_Field("section", "A24", "hide");
        Toggle_Field("section", "A25", "show");
        Toggle_Field("control", "A25", "show");
        Toggle_Field("back", "A24Y", "show");
        Toggle_Field("continue", "A25", "show");
        $("#tb2_amount_dollar").focus();
    }
    else if (rdValue == "NEW") {
        Toggle_Field("section", "A24", "hide");
        Toggle_Field("section", "A26", "show");
        Toggle_Field("control", "A26", "show");
        Toggle_Field("back", "A24N", "show");
        Toggle_Field("continue", "A26", "show");
    }
    else { vfMsg = "Unhandled Response\n\rContact IT with screenshot.."; validationFailed("A24"); }
}
function Return_A24Y() {
    Toggle_Field("section", "A24", "show");
    Toggle_Field("section", "A25", "hide");
    Toggle_Field("control", "A25", "hide");
    Toggle_Field("back", "A24Y", "hide");
    Toggle_Field("continue", "A25", "hide");
}
function Return_A24N() {
    Toggle_Field("section", "A24", "show");
    Toggle_Field("section", "A26", "hide");
    Toggle_Field("control", "A26", "hide");
    Toggle_Field("back", "A24N", "hide");
    Toggle_Field("continue", "A26", "hide");
}
function Validate_A25() {
    validationStarted();

    var rdValue = $("#RadioButtonList25 input:checked").val();
    if (!rdValue) {
        vfMsg = "You must select an option above.";
        validationFailed("A25");
    }
    else if (rdValue == "YES") {

        if ($("#cdCompany").val() == "DRTV") {
            $("#A2_standard_01").hide();
            $("#A2_standard_02").hide();
            $("#A2_standard_03").hide();
            $("#A2_drtv_01").show();
            //$("#A2_drtv_02").show();
        } else {
            $("#A2_standard_01").show();
            $("#A2_standard_02").show();
            $("#A2_standard_03").show();
            $("#A2_drtv_01").hide();
            $("#A2_drtv_02").hide();
        }

        Toggle_Field("section", "A25", "hide");
        Toggle_Field("section", "A1", "show");
        Toggle_Field("control", "A1", "show");
        Toggle_Field("back", "A25Y", "show");
        Toggle_Field("continue", "A1", "show");
        $("#tb2_amount_dollar").focus();
    }
    else if (rdValue == "NO") {
        Toggle_Field("section", "A25", "hide");
        Toggle_Field("section", "A27", "show");
        Toggle_Field("control", "A27", "show");
        Toggle_Field("back", "A25N", "show");
        Toggle_Field("continue", "A27", "show");
    }
    else { vfMsg = "Unhandled Response\n\rContact IT with screenshot.."; validationFailed("A25"); }
}
function Return_A25Y() {
    Toggle_Field("section", "A25", "show");
    Toggle_Field("section", "A1", "hide");
    Toggle_Field("control", "A1", "hide");
    Toggle_Field("back", "A25Y", "hide");
    Toggle_Field("continue", "A1", "hide");
}
function Return_A25N() {
    Toggle_Field("section", "A25", "show");
    Toggle_Field("section", "A27", "hide");
    Toggle_Field("control", "A27", "hide");
    Toggle_Field("back", "A25N", "hide");
    Toggle_Field("continue", "A27", "hide");
}
function Validate_A37() {
    validationStarted();

    var rdValue = $("#RadioButtonList37 input:checked").val();
    if (!rdValue) {
        vfMsg = "You must select an option above.";
        validationFailed("A37");
    }
    else if (rdValue == "YES") {
        if ($("#cdCompany").val() == "DRTV") {
            $("#A2_standard_01").hide();
            $("#A2_standard_02").hide();
            $("#A2_standard_03").hide();
            $("#A2_drtv_01").show();
            //$("#A2_drtv_02").show();
        } else {
            $("#A2_standard_01").show();
            $("#A2_standard_02").show();
            $("#A2_standard_03").show();
            $("#A2_drtv_01").hide();
            $("#A2_drtv_02").hide();
        }
        Toggle_Field("section", "A37", "hide");
        Toggle_Field("section", "A1", "show");
        Toggle_Field("control", "A1", "show");
        Toggle_Field("back", "A37Y", "show");
        Toggle_Field("continue", "A1", "show");

    }
    else if (rdValue == "NO") {
        ToggleCallType("h", "s", "h");

        var gtGoTo = "37";
        Toggle_Field("section", "A" + gtGoTo, "hide");
        Toggle_Field("section", "A96", "show");
        Toggle_Field("control", "A96", "show");
        Toggle_Field("back", "A21Y", "show");
        Toggle_Field("continue", "A96", "show"); $("#validation_message").html("");
        var disposition = DispositionFromSection(gtGoTo);
        $("#ListBox96").val(disposition);

        $("#back_A21Y").val("Back to A" + gtGoTo);
        $("#back_A21Y").attr("onclick", "javascript:Return_A21Y('" + gtGoTo + "')");
    }
    else { vfMsg = "Unhandled Response\n\rContact IT with screenshot.."; validationFailed("A37"); }
}
function Return_A37Y() {
    Toggle_Field("section", "A37", "show");
    Toggle_Field("section", "A1", "hide");
    Toggle_Field("control", "A1", "hide");
    Toggle_Field("back", "A37Y", "hide");
    Toggle_Field("continue", "A1", "hide");
}
function Validate_A38() {
    validationStarted();

    var rdValue = $("#RadioButtonList38 input:checked").val();
    if (!rdValue) {
        vfMsg = "You must select an option above.";
        validationFailed("A38");
    }
    else if (rdValue == "YES") {
        if ($("#cdCompany").val() == "DRTV") {
            $("#A2_standard_01").hide();
            $("#A2_standard_02").hide();
            $("#A2_standard_03").hide();
            $("#A2_drtv_01").show();
            //$("#A2_drtv_02").show();
        } else {
            $("#A2_standard_01").show();
            $("#A2_standard_02").show();
            $("#A2_standard_03").show();
            $("#A2_drtv_01").hide();
            $("#A2_drtv_02").hide();
        }

        Toggle_Field("section", "A38", "hide");
        Toggle_Field("section", "A1", "show");
        Toggle_Field("control", "A1", "show");
        Toggle_Field("back", "A38Y", "show");
        Toggle_Field("continue", "A1", "show");
        $("#tb2_amount_dollar").focus();
    }
    else if (rdValue == "NO") {
        ToggleCallType("h", "s", "h");

        var gtGoTo = "38";
        Toggle_Field("section", "A" + gtGoTo, "hide");
        Toggle_Field("section", "A96", "show");
        Toggle_Field("control", "A96", "show");
        Toggle_Field("back", "A21Y", "show");
        Toggle_Field("continue", "A96", "show"); $("#validation_message").html("");
        var disposition = DispositionFromSection(gtGoTo);
        $("#ListBox96").val(disposition);

        $("#back_A21Y").val("Back to A" + gtGoTo);
        $("#back_A21Y").attr("onclick", "javascript:Return_A21Y('" + gtGoTo + "')");
    }
    else { vfMsg = "Unhandled Response\n\rContact IT with screenshot.."; validationFailed("A38"); }
}
function Return_A38Y() {
    Toggle_Field("section", "A38", "show");
    Toggle_Field("section", "A1", "hide");
    Toggle_Field("control", "A1", "hide");
    Toggle_Field("back", "A38Y", "hide");
    Toggle_Field("continue", "A1", "hide");
}
function Section_A40_NoEmails() {
    //var rdValue = $("#tb40_noemail input:checked").val();
    var rdValue = $("#tb40_noemail");
    if (rdValue.prop("checked")) {
        $("#tb40_email_optin").find("input[value='NO']").attr("checked", "checked");
        $("#tb40_email").val("")
    } else {
        //$("#tb40_email_optin").find("input[value='YES']").attr("checked", "checked");
    }
}
function Validate_A40() {
    validationStarted();

    var vfField = "";

    if (!vf_RequiredCheckbox("tb40_nomail") && !vf_RequiredCheckbox("tb40_nophone") && !vf_RequiredCheckbox("tb40_noemail")) {
        vfMsg = vfMsg + "Must select at least 1 list.\n\r";
    }

    if (!vf_Required("tb40_first_name")) {
        vfMsg = vfMsg + "First Name is required.\n\r";
        if (vfField.length == 0) { vfField = "tb40_first_name"; }
    } else if (!vf_Len("tb40_first_name", 2, 100)) {
        vfMsg = vfMsg + "First Name must be between 2 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb40_first_name"; }
    }
    if (!vf_Required("tb40_last_name")) {
        vfMsg = vfMsg + "Last Name is required.\n\r";
        if (vfField.length == 0) { vfField = "tb40_last_name"; }
    } else if (!vf_Len("tb40_last_name", 3, 100)) {
        vfMsg = vfMsg + "Last Name must be between 3 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb40_last_name"; }
    }
    if (!vf_Len("tb40_business_name", 0, 100)) {
        vfMsg = vfMsg + "Business Name must be 100 characters or less.\n\r";
        if (vfField.length == 0) { vfField = "tb40_business_name"; }
    }
    if (!vf_Required("tb40_address1")) {
        vfMsg = vfMsg + "Address is required.\n\r";
        if (vfField.length == 0) { vfField = "tb40_address1"; }
    } else if (!vf_Len("tb40_address1", 3, 100)) {
        vfMsg = vfMsg + "Address must be between 5 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb40_address1"; }
    }
    if (!vf_Len("tb40_suite_number", 0, 50)) {
        vfMsg = vfMsg + "Suite Number must be 50 characters or less.\n\r";
        if (vfField.length == 0) { vfField = "tb40_suite_number"; }
    }
    if (!vf_Required("tb40_postal_code")) {
        vfMsg = vfMsg + "Postal Code is required.\n\r";
        if (vfField.length == 0) { vfField = "tb40_postal_code"; }
    } else if (!vf_Len("tb40_postal_code", 5, 20)) {
        vfMsg = vfMsg + "Postal Code must be between 5 and 20 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb40_postal_code"; }
    }
    if (!vf_Required("tb40_city")) {
        vfMsg = vfMsg + "City is required.\n\r";
        if (vfField.length == 0) { vfField = "tb40_city"; }
    } else if (!vf_Len("tb40_city", 3, 20)) {
        vfMsg = vfMsg + "City must be between 5 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb40_city"; }
    }
    if (!vf_Required("tb40_state")) {
        vfMsg = vfMsg + "State is required.\n\r";
        if (vfField.length == 0) { vfField = "tb40_state"; }
    }
    if (vf_RequiredRadio("tb40_email_optin")) {
        var EmailTgl = $("#tb40_email_optin input:checked").val();
        if (EmailTgl == "YES") {
            if (!vf_Required("tb40_email")) {
                vfMsg = vfMsg + "Email is required if requesting opt-in.\n\r";
                if (vfField.length == 0) { vfField = "tb40_email"; }
            }
        }
    } else {
        vfMsg = vfMsg + "Must select Yes or No for email opt in.\n\r";
        if (vfField.length == 0) { vfField = "tb40_email"; }
    }
    if (!vf_Email("tb40_email")) {
        vfMsg = vfMsg + "Email must be a valid email address.\n\r";
        if (vfField.length == 0) { vfField = "tb40_email"; }
    }
    if (!vf_Phone("tb40_phone")) {
        vfMsg = vfMsg + "Phone: must be a valid phone number.\n\r";
        if (vfField.length == 0) { vfField = "tb40_phone"; }
    }

    if (vfMsg.length > 0) {
        validationFailed("A40");
        $("#" + vfField).focus();
        $("#" + vfField).focus(function () { this.select(); });
    } else {
        ToggleCallType("h", "s", "h");

        var gtGoTo = "40";
        Toggle_Field("section", "A" + gtGoTo, "hide");
        Toggle_Field("section", "A96", "show");
        Toggle_Field("control", "A96", "show");
        Toggle_Field("back", "A21Y", "show");
        Toggle_Field("continue", "A96", "show"); $("#validation_message").html("");
        var disposition = DispositionFromSection(gtGoTo);
        $("#ListBox96").val(disposition);

        $("#back_A21Y").val("Back to A" + gtGoTo);
        $("#back_A21Y").attr("onclick", "javascript:Return_A21Y('" + gtGoTo + "')");
    }
}
function Validate_A46() {
    validationStarted();

    // Validate_A40
    // Validate_A8
    
    var vfField = "";

    if (!vf_Required("tb46_first_name")) {
        vfMsg = vfMsg + "First Name is required.\n\r";
        if (vfField.length == 0) { vfField = "tb46_first_name"; }
    } else if (!vf_Len("tb46_first_name", 2, 100)) {
        vfMsg = vfMsg + "First Name must be between 2 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb46_first_name"; }
    }
    if (!vf_Required("tb46_last_name")) {
        vfMsg = vfMsg + "Last Name is required.\n\r";
        if (vfField.length == 0) { vfField = "tb46_last_name"; }
    } else if (!vf_Len("tb46_last_name", 3, 100)) {
        vfMsg = vfMsg + "Last Name must be between 3 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb46_last_name"; }
    }
    if (!vf_Len("tb46_business_name", 0, 50)) {
        vfMsg = vfMsg + "Business Name must be 50 characters or less.\n\r";
        if (vfField.length == 0) { vfField = "tb46_business_name"; }
    }
    if (!vf_Required("tb46_address1")) {
        vfMsg = vfMsg + "Address is required.\n\r";
        if (vfField.length == 0) { vfField = "tb46_address1"; }
    } else if (!vf_Len("tb46_address1", 5, 100)) {
        vfMsg = vfMsg + "Address must be between 5 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb46_address1"; }
    }
    if (!vf_Len("tb46_suite_number", 0, 25)) {
        vfMsg = vfMsg + "Suite Number must be 25 characters or less.\n\r";
        if (vfField.length == 0) { vfField = "tb46_suite_number"; }
    }
    if (!vf_Required("tb46_postal_code")) {
        vfMsg = vfMsg + "Postal Code is required.\n\r";
        if (vfField.length == 0) { vfField = "tb46_postal_code"; }
    } else if (!vf_Len("tb46_postal_code", 5, 20)) {
        vfMsg = vfMsg + "Postal Code must be between 5 and 20 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb46_postal_code"; }
    }
    if (!vf_Required("tb46_city")) {
        vfMsg = vfMsg + "City is required.\n\r";
        if (vfField.length == 0) { vfField = "tb46_city"; }
    } else if (!vf_Len("tb46_city", 3, 25)) {
        vfMsg = vfMsg + "City must be between 5 and 25 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb46_city"; }
    }
    if (!vf_Required("tb46_country")) {
        vfMsg = vfMsg + "Country is required.\n\r";
        if (vfField.length == 0) { vfField = "tb46_country"; }
    } else {
        var country = $("#tb46_country").val();
        if (country == "USA") {
            if (!vf_Required("tb46_state")) {
                vfMsg = vfMsg + "State is required.\n\r";
                if (vfField.length == 0) { vfField = "tb46_state"; }
            }
        } else if (country == "CAN") {
            if (!vf_Required("tb46_stateca")) {
                vfMsg = vfMsg + "Province is required.\n\r";
                if (vfField.length == 0) { vfField = "tb46_stateca"; }
            }
        } else {
            if (!vf_Required("tb46_stateother")) {
                vfMsg = vfMsg + "State field is required.\n\r";
                if (vfField.length == 0) { vfField = "tb46_stateother"; }
            }
        }
    }

    if (vfMsg.length > 0) {
        validationFailed("A46");
        $("#" + vfField).focus();
        $("#" + vfField).focus(function () { this.select(); });
        Toggle_Field("section", "A", "show");
        $("#ResponseSQL").html("");
        $("#lblValidation").html("Latest Validation Errors:<br />" + vfMsg.replace(/\n\r/g, "<br />"));
    } else {
        ToggleCallType("h", "s", "h");

        var gtGoTo = "46";
        Toggle_Field("section", "A" + gtGoTo, "hide");
        Toggle_Field("section", "A96", "show");
        Toggle_Field("control", "A96", "show");
        Toggle_Field("back", "A21Y", "show");
        Toggle_Field("continue", "A96", "show"); $("#validation_message").html("");
        var disposition = DispositionFromSection(gtGoTo);
        $("#ListBox96").val(disposition);

        $("#back_A21Y").val("Back to A" + gtGoTo);
        $("#back_A21Y").attr("onclick", "javascript:Return_A21Y('" + gtGoTo + "')");

    }
}
function Validate_A47() {
    validationStarted();

    var vfField = "";

    if (!vf_Required("tb47_first_name")) {
        vfMsg = vfMsg + "First Name is required.\n\r";
        if (vfField.length == 0) { vfField = "tb47_first_name"; }
    } else if (!vf_Len("tb47_first_name", 2, 100)) {
        vfMsg = vfMsg + "First Name must be between 2 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb47_first_name"; }
    }
    if (!vf_Required("tb47_last_name")) {
        vfMsg = vfMsg + "Last Name is required.\n\r";
        if (vfField.length == 0) { vfField = "tb47_last_name"; }
    } else if (!vf_Len("tb47_last_name", 3, 100)) {
        vfMsg = vfMsg + "Last Name must be between 3 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb47_last_name"; }
    }
    if (!vf_Len("tb47_business_name", 0, 50)) {
        vfMsg = vfMsg + "Business Name must be 50 characters or less.\n\r";
        if (vfField.length == 0) { vfField = "tb47_business_name"; }
    }
    if (!vf_Required("tb47_address1")) {
        vfMsg = vfMsg + "Address is required.\n\r";
        if (vfField.length == 0) { vfField = "tb47_address1"; }
    } else if (!vf_Len("tb47_address1", 5, 100)) {
        vfMsg = vfMsg + "Address must be between 5 and 100 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb47_address1"; }
    }
    if (!vf_Len("tb47_suite_number", 0, 25)) {
        vfMsg = vfMsg + "Suite Number must be 25 characters or less.\n\r";
        if (vfField.length == 0) { vfField = "tb47_suite_number"; }
    }
    if (!vf_Required("tb47_postal_code")) {
        vfMsg = vfMsg + "Postal Code is required.\n\r";
        if (vfField.length == 0) { vfField = "tb47_postal_code"; }
    } else if (!vf_Len("tb47_postal_code", 5, 20)) {
        vfMsg = vfMsg + "Postal Code must be between 5 and 20 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb47_postal_code"; }
    }
    if (!vf_Required("tb47_city")) {
        vfMsg = vfMsg + "City is required.\n\r";
        if (vfField.length == 0) { vfField = "tb47_city"; }
    } else if (!vf_Len("tb47_city", 3, 25)) {
        vfMsg = vfMsg + "City must be between 5 and 25 characters.\n\r";
        if (vfField.length == 0) { vfField = "tb47_city"; }
    }
    if (!vf_Required("tb47_country")) {
        vfMsg = vfMsg + "Country is required.\n\r";
        if (vfField.length == 0) { vfField = "tb47_country"; }
    } else {
        var country = $("#tb47_country").val();
        if (country == "USA") {
            if (!vf_Required("tb47_state")) {
                vfMsg = vfMsg + "State is required.\n\r";
                if (vfField.length == 0) { vfField = "tb47_state"; }
            }
        } else if (country == "CAN") {
            if (!vf_Required("tb47_stateca")) {
                vfMsg = vfMsg + "Province is required.\n\r";
                if (vfField.length == 0) { vfField = "tb47_stateca"; }
            }
        } else {
            if (!vf_Required("tb47_stateother")) {
                vfMsg = vfMsg + "State field is required.\n\r";
                if (vfField.length == 0) { vfField = "tb47_stateother"; }
            }
        }
    }

    if (vfMsg.length > 0) {
        validationFailed("A47");
        $("#" + vfField).focus();
        $("#" + vfField).focus(function () { this.select(); });
        Toggle_Field("section", "A", "show");
        $("#ResponseSQL").html("");
        $("#lblValidation").html("Latest Validation Errors:<br />" + vfMsg.replace(/\n\r/g, "<br />"));
    } else {
        Toggle_Field("section", "A0", "hide");

        Toggle_Field("section", "A47", "hide");
        Toggle_Field("section", "A9", "show");
        Toggle_Field("control", "A9", "show");
        Toggle_Field("back", "A47Y", "show");
        Toggle_Field("continue", "A9", "show");

        // Populate_A9(); // ??
    }
}
function Return_A47Y() {
    Toggle_Field("section", "A47", "show");
    Toggle_Field("section", "A9", "hide");
    Toggle_Field("control", "A9", "hide");
    Toggle_Field("back", "A47Y", "hide");
    Toggle_Field("continue", "A9", "hide");
    $("#tb47_first_name").focus();
    $("#tb47_first_name").focus(function () { this.select(); });
}
function Return_A47Y() {
    Toggle_Field("section", "A47", "show");
    Toggle_Field("section", "A9", "hide");
    Toggle_Field("control", "A9", "hide");
    Toggle_Field("back", "A47Y", "hide");
    Toggle_Field("continue", "A9", "hide");
    $("#tb47_first_name").focus();
    $("#tb47_first_name").focus(function () { this.select(); });
}
function Validate_A49() {
    //A96
    ToggleCallType("s", "h", "h");

    Toggle_Field("section", "A49", "hide");
    Toggle_Field("section", "A96", "show");
    Toggle_Field("control", "A96", "show");
    Toggle_Field("back", "A49Y", "show");
    Toggle_Field("continue", "A96", "show");

    $("#ListBox96").val("10"); // Information ONly
}
function Return_A49Y() {
    Toggle_Field("section", "A49", "show");
    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("back", "A49Y", "hide");
    Toggle_Field("continue", "A96", "hide");

    $("#ListBox96").val("");

}
/* ^^ Re-Done / Verified ^^ */


function Return_A3d() {
    Toggle_Field("section", "A3", "show");
    Toggle_Field("section", "A41", "hide");
    Toggle_Field("control", "A41", "hide");
    Toggle_Field("back", "A3d", "hide");
    Toggle_Field("continue", "A41", "hide");
}
function Validate_A41() {
    validationStarted();

    var totalAmount = 0;
    $(".giftcatalog").each(function (index, value) {
        var x = $(this);
        var q = x.val();
        var qInt = 0;
        if (q == "") qInt = 0;
        else qInt = parseInt(q);
        price = parseInt(x.data("price"));
        totalAmount = totalAmount + (qInt * price);
    });

    //reset gift incase back button was pressed
    $("#whichGift").text("");
    $("#giftAmount").text("");
    $("#hcPremiumGiftSKU").val("");

    if (totalAmount > 0) {
        $("#tb2_amount_dollar").val(totalAmount);
        $("#tb2_amount_cent").val("00");

        var giftFound = false;
        for (var i = 0; i < premiumGiftObj.length; i++) {
            if (totalAmount < premiumGiftObj[i].Max && totalAmount >= premiumGiftObj[i].Min) {
                $("#whichGift").text(premiumGiftObj[i].Title);
                $("#giftAmount").text("$" + totalAmount + ".00");
                $("#hcPremiumGiftSKU").val(premiumGiftObj[i].SKU);
                giftFound = true;
                break;
            }
        }
        // We have to go there either way, the difference is hiding the "Wants gift" section...
        if (giftFound == false) {
            $("#giftselection").hide();
            // var rdValue = $("#rbWantsGift input:checked").val();

            //Toggle_Field("section", "A41", "hide");
            //Toggle_Field("section", "A4", "show");
            //Toggle_Field("control", "A4", "show");
            //Toggle_Field("back", "A3A", "hide");
            //Toggle_Field("back", "A41B", "show");
            //Toggle_Field("back", "A42A", "hide");
            //Toggle_Field("continue", "A4", "show");
        } else {
            $("#giftselection").show();
            //Toggle_Field("section", "A41", "hide");
            //Toggle_Field("section", "A42", "show");
            //Toggle_Field("control", "A42", "show");
            //Toggle_Field("back", "A41A", "show");
            //Toggle_Field("continue", "A42", "show");
        }
        Toggle_Field("section", "A41", "hide");
        Toggle_Field("section", "A42", "show");
        Toggle_Field("control", "A42", "show");
        Toggle_Field("back", "A41A", "show");
        Toggle_Field("continue", "A42", "show");
    } else {
        vfMsg = "Please select quantity!";
        validationFailed("A41");
    }
}


function Return_A41A() {
    Toggle_Field("section", "A41", "show");
    Toggle_Field("section", "A42", "hide");
    Toggle_Field("control", "A42", "hide");
    Toggle_Field("back", "A41", "hide");
    Toggle_Field("continue", "A42", "hide");
}
function Return_A41B() {
    Toggle_Field("section", "A41", "show");
    Toggle_Field("section", "A4", "hide");
    Toggle_Field("control", "A4", "hide");
    Toggle_Field("back", "A4", "hide");
    Toggle_Field("continue", "A4", "hide");
}
function Validate_A42() {
    validationStarted();

    var rdValue = $("#rbWantsGift input:checked").val();
    var rdValue2 = $("#rbWantsGiftCard input:checked").val();

    
    if (!rdValue && $("#whichGift").text().length > 0) { vfMsg += "You must select whether they want the premium gift or not.\n\r" }
    if (!rdValue2){ vfMsg += "You must select whether they want the greeting card or not.\n\r"}

    if (vfMsg.length > 0) {
        validationFailed("A41B");
    } else {
        if (rdValue == "Y" || rdValue2 == "Y") {
            $("#hcAlternateAddressOffer").val("YES");
        } else {
            $("#hcAlternateAddressOffer").val("NO");
        }

        Toggle_Field("section", "A42", "hide");
        Toggle_Field("section", "A4", "show");
        Toggle_Field("control", "A4", "show");
        Toggle_Field("back", "A42A", "show");
        Toggle_Field("back", "A3A", "hide");
        Toggle_Field("back", "A41B", "hide");
        Toggle_Field("continue", "A4", "show");
    }
}
function Return_A42() {
    Toggle_Field("section", "A42", "show");
    Toggle_Field("section", "A4", "hide");
    Toggle_Field("control", "A4", "hide");
    Toggle_Field("back", "A42A", "hide");
    Toggle_Field("continue", "A4", "hide");
}

function Validate_A43() {
    validationStarted();

    var rdValue = $("#RadioButtonList43 input:checked").val();
    if (!rdValue) {
        vfMsg = "You must select an option above.";
        validationFailed("A43");
    } else {
        var now = new Date();
        var day = now.getDate();
        $("#rdChargeDate").val(rdValue);
        $("#rdChargeDate_txt").val(rdValue);
        $("#rdChargeDateOriginal").val(rdValue);
        $("#rdChargeDateOriginal_txt").val(rdValue);

        // If Today is between 1 and 10 and charge date is 15 - display warning
        // If Today is between 16 and 31 and charge date is 1 - display warning
        // Else move along
        if (rdValue == 15 && day >= 1 && day <= 10) {
            Toggle_Field("section", "A43", "hide");
            Toggle_Field("section", "A44", "show");
            Toggle_Field("control", "A44", "show");
            Toggle_Field("back", "A43N", "show");
            Toggle_Field("continue", "A44", "show");
            Toggle_Field("section", "A44long", "show");
            Toggle_Field("section", "A44short", "hide");
            $("#rbl4_opt1").html("1<sup>st</sup>");
            $("#rbl4_opt2").html("15<sup>th</sup>");
            $("#rdChargeDateChange").val("1");
        }
        else if (rdValue == 1 && day >= 16 && day <= 31) {
            Toggle_Field("section", "A43", "hide");
            Toggle_Field("section", "A44", "show");
            Toggle_Field("control", "A44", "show");
            Toggle_Field("back", "A43N", "show");
            Toggle_Field("continue", "A44", "show");
            Toggle_Field("section", "A44short", "show");
            Toggle_Field("section", "A44long", "hide");
            $("#rbl4_opt1").html("15<sup>th</sup>");
            $("#rbl4_opt2").html("1<sup>st</sup>");
            $("#rdChargeDateChange").val("15");
        }
        else {
            // From A3 to A7
            Toggle_Field("section", "A43", "hide");
            Toggle_Field("section", "A7", "show");
            Toggle_Field("control", "A7", "show");
            Toggle_Field("back", "A43Y", "show");
            Toggle_Field("continue", "A7", "show");
            //Toggle_Field("section", "A3gc", "hide");
            //Toggle_Field("section", "A3sc", "hide");
            //Toggle_Field("section", "A3high", "hide");
            //Toggle_Field("section", "A3other", "show");
        }
    }
}
function Return_A43Y() {
    Toggle_Field("section", "A43", "show");
    Toggle_Field("section", "A7", "hide");
    Toggle_Field("control", "A7", "hide");
    Toggle_Field("back", "A43Y", "hide");
    Toggle_Field("continue", "A7", "hide");
}
function Return_A43N() {
    Toggle_Field("section", "A43", "show");
    Toggle_Field("section", "A44", "hide");
    Toggle_Field("control", "A44", "hide");
    Toggle_Field("back", "A43N", "hide");
    Toggle_Field("continue", "A44", "hide");
    Toggle_Field("section", "A44long", "show");
    Toggle_Field("section", "A44short", "show");
}
function Validate_A44() {
    validationStarted();

    var rdValue = $("#RadioButtonList44 input:checked").val();
    if (!rdValue) {
        vfMsg = "You must select an option above.";
        validationFailed("A44");
    } else {
        // From A3 to A7
        if (rdValue == 1) {
            // Change Date
            var rdChargeDateNew = $("#rdChargeDateChange").val();
            $("#rdChargeDate").val(rdChargeDateNew);
            $("#rdChargeDate_text").val(rdChargeDateNew);

            Toggle_Field("section", "A44", "hide");
            Toggle_Field("section", "A7", "show");
            Toggle_Field("control", "A7", "show");
            Toggle_Field("back", "A44Y", "show");
            Toggle_Field("continue", "A7", "show");
            //Toggle_Field("section", "A3gc", "hide");
            //Toggle_Field("section", "A3sc", "hide");
            //Toggle_Field("section", "A3high", "hide");
            //Toggle_Field("section", "A3other", "show");
        }
        else {
            var rdChargeDateNew = $("#rdChargeDateOriginal").val();
            $("#rdChargeDate").val(rdChargeDateNew);
            $("#rdChargeDate_txt").val(rdChargeDateNew);

            Toggle_Field("section", "A44", "hide");
            Toggle_Field("section", "A7", "show");
            Toggle_Field("control", "A7", "show");
            Toggle_Field("back", "A44Y", "show");
            Toggle_Field("continue", "A7", "show");
            //Toggle_Field("section", "A3gc", "hide");
            //Toggle_Field("section", "A3sc", "hide");
            //Toggle_Field("section", "A3high", "hide");
            //Toggle_Field("section", "A3other", "show");
        }
    }
}
function Return_A44Y() {
    Toggle_Field("section", "A44", "show");
    Toggle_Field("section", "A7", "hide");
    Toggle_Field("control", "A7", "hide");
    Toggle_Field("back", "A44Y", "hide");
    Toggle_Field("continue", "A7", "hide");
}

/*
Remove Below
*/
function Return_A777() {
    Toggle_Field("section", "A7", "show");
    Toggle_Field("section", "A21", "hide");
    Toggle_Field("control", "A21", "hide");
    Toggle_Field("back", "A7N", "hide");
    Toggle_Field("continue", "A21", "hide");
    $("#lblProvider21a").html("{provider_name}");
    $("#lblProvider21b").html("{provider_name}");
    $("#lblProvider21c").html("{provider_name}");
}

function Validate_A90() {
    Toggle_Field("section", "A90", "hide");
    Toggle_Field("section", "A96", "show");
    Toggle_Field("control", "A96", "show");
    Toggle_Field("back", "A90Y", "show");
    Toggle_Field("continue", "A96", "show");
    $("#validation_message").html("");
}
function Return_A90Y() {
    Toggle_Field("section", "A90", "show");
    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("back", "A90Y", "hide");
    Toggle_Field("continue", "A96", "hide");
}
function Validate_A96() {
    validationStarted();

    Toggle_Field("clear", "A96C", "hide");

    if (!vf_Required("ListBox96")) {
        vfMsg = "Must select a disposition.";
        validationFailed("A496");
    } else {
        //
        //ListBox18
        // Should not be Donation or Check

        var dspValue = $("#ListBox96").val();
        var cardValue = $("#tb7_card_number").val();
        if (dspValue == "Donation" || dspValue == "Check") {
            vfMsg += "You can not select Donation or Check from this point.\n\rPlease use the [Back] functions if they wish to Donate or Pledge.";
        }
        if (dspValue == "41" || dspValue == "46" || dspValue == "42" || dspValue == "43") {
            if ($("#tglEscape").text() == "Undo-Escape") {
                vfMsg += "You may not submit a donation while using Escape\rPlease Undo-Escape first.";
            }
        }

        if (dspValue == "41" || dspValue == "46") {
            if (cardValue.length == 0) {
                vfMsg += "A donation disposition must have a valid credit card.";
            }
        } else {
            if (cardValue.length > 0) {
                vfMsg += "The credit card field is populated, but this is not a donation.\n\rYou must clear the credit card field before proceeding, or change disposition.";
                Toggle_Field("clear", "A96C", "show");
            }

        }
        // If not -- credit card must be empty
        // if ($("#cdDisposition").val() != "Initiated" && $("#cdDisposition").val() != "ERROR" && $("#cdDisposition").val() != "REJECTED") {
        if ($("#cdDisposition").val() != "Initiated" && $("#cdDisposition").val() != "REJECT") {
            vfMsg += "This call has already been dispositioned, start a new call.";
            vfMsg += "\n\rDisposition: " + $("#cdDisposition").val();
        }
        if (vfMsg != "") {
            vfMsg = "You must select an option above.";
            validationFailed("A96");

            /* No longer needed ^^^ */
            // $("#validation_issues").show();
            // $("#validation_message").html("" + msg.replace(/\n\r/g, "<br />").replace(/\r/g, "<br />").replace(/\n/g, "<br />"));
            /* No longer needed ^^^ */

            // var rdValue = $("#confirmation_01").html();
            // var fullname = $("#tb7_first_name").val() + " " + $("#tb7_last_name").val();
            // $("#confirmation_01").html(rdValue);
            //$("#lblValidation").html("Latest Validation Errors:<br />" + vfMsg.replace(/\n\r/g, "<br />"));
            // replace

        } else {
            /* No longer needed ^^^ */
            // $("#validation_issues").hide();
            // $("#validation_message").html("");
            /* No longer needed ^^^ */

            $("#continue_A96").val("Call Processing");
            $("#continue_A96").attr('disabled', 'disabled');
            //Hide_All_After_Submit();
            SubmitForm('record');
        }
    }
}
function Clear_A96C() {
    validationStarted();
    Toggle_Field("clear", "A96C", "hide");

    $("#tb7_card_number").val("");
    $("#tb7_card_month").val("");
    $("#tb7_card_year").val("");
    $("#tb7_first_name").val("");
    $("#tb7_middle_initial").val("");
    $("#tb7_last_name").val("");

    var cardValue = $("#tb7_card_number").val();
    if (cardValue.length > 0) {
        vfMsg += "Was not able to clear fields, use Back to Start to manually clear.";
        Toggle_Field("clear", "A96C", "show");
    } else {
        vfMsg += "Credit Card data fields cleared, try submitting non-donation record again.";

    }
    validationFailed("A46");
}
/*
Script Handling

This is here because of the hard coded field names
*/
function NotUsed() {
    return false;
}
/* Above is questionable */

function Validate_A0() {
    validationStarted();

    /*
        Continue_A0A --> YES - Try Different Card --> A4
        Continue_A0B --> Envelope - Use Billing Address --> A96
        Continue_A0C --> Envelope - Enter New Address --> A5
        Continue_A0D --> END CALL --> A98

        backA0A
        backA0B
        backA3A
        backA3B
        backA41A
        backA41B
        backA96B
        backA96C

        backA42x ??
    */
    var rdValue = $("#rb0_options input:checked").val();

    if (!rdValue) {
        vfMsg = "You must select an option above.";
        validationFailed();
    }
    else if (rdValue == "YES") {
        Toggle_Field("section", "A0", "hide");
        Toggle_Field("section", "A4", "show");
        Toggle_Field("control", "A4", "show");
        Toggle_Field("back", "A0Y", "show");
        Toggle_Field("continue", "A4", "show");
        Toggle_Field("decline", "A0Y", "show");
    }
    else if (rdValue == "BILLING") {

        // sustainer
        if ($("#rdSustainer").val() == "YES") {
            $('input[name="tb45_sustainer_optin"][value="' + "YES_Sustainer" + '"]').prop('checked', true);
            $("#ListBox96").val("47"); // Pledge - Sustainer
        } else {
            $('input[name="tb45_sustainer_optin"][value="' + "YES_OneTime" + '"]').prop('checked', true);
            $("#ListBox96").val("42"); // Pledge - One Time
        }

        //$("#tb45_sustainer_optin").val("YES_Sustainer");
        //$("#tb45_sustainer_optin").val("YES_OneTime");
        //$("#tb45_sustainer_optin").val("NO");

        $("#tb45_first_name").val($("#tb7_first_name").val());
        $("#tb45_last_name").val($("#tb7_last_name").val());

        $("#tb45_business_name").val($("#tb8_business_name").val());
        // $("#tb45_prefix").val($("#tb8_prefix").val());
        $("#tb45_address1").val($("#tb8_address1").val());
        $("#tb45_suite_type").val($("#tb8_suite_type").val());
        $("#tb45_suite_number").val($("#tb8_suite_number").val());
        $("#tb45_postal_code").val($("#tb8_postal_code").val());
        $("#tb45_country").val($("#tb8_country").val());
        $("#tb45_city").val($("#tb8_city").val());
        $("#tb45_state").val($("#tb8_state").val());
        $("#tb45_stateca").val($("#tb8_stateca").val());
        $("#tb45_stateother").val($("#tb8_stateother").val());
        $("#tb45_amount_dollar").val($("#tb2_amount_dollar").val());
        $("#tb45_amount_cent").val($("#tb2_amount_cent").val());

        $("#tb7_card_number").val("");
        $("#tb7_card_month").val("");
        $("#tb7_card_year").val("");
        $("#tb7_first_name").val("");
        $("#tb7_middle_initial").val("");
        $("#tb7_last_name").val("");

        ToggleCallType("h", "h", "s");

        Toggle_Field("section", "A0", "hide");
        Toggle_Field("section", "A45", "show");
        Toggle_Field("control", "A45", "show");
        Toggle_Field("back", "A0E", "show");
        Toggle_Field("continue", "A45", "show");

        //Toggle_Field("section", "A0", "hide");
        //Toggle_Field("section", "A96", "show");
        //Toggle_Field("control", "A96", "show");
        //Toggle_Field("back", "A0B", "show");
        //Toggle_Field("continue", "A96", "show"); $("#validation_message").html("");
    }
    else if (rdValue == "NEWADDRESS") {
        $("#tb7_card_number").val("");
        $("#tb7_card_month").val("");
        $("#tb7_card_year").val("");
        $("#tb7_first_name").val("");
        $("#tb7_middle_initial").val("");
        $("#tb7_last_name").val("");

        $("#tb45_amount_dollar").val($("#tb2_amount_dollar").val())
        $("#tb45_amount_cent").val($("#tb2_amount_cent").val())

        Toggle_Field("section", "A0", "hide");
        Toggle_Field("section", "A45", "show");
        Toggle_Field("control", "A45", "show");
        Toggle_Field("back", "A0E", "show");
        Toggle_Field("continue", "A45", "show");


    }
    else if (rdValue == "NO") {
        Toggle_Field("section", "A0", "hide");
        Toggle_Field("section", "A98", "show");
        Toggle_Field("control", "A98", "show");
        Toggle_Field("back", "A0N", "show");
        Toggle_Field("continue", "A98", "show");
    }
    else { vfMsg = "Unhandled Response\n\rContact IT with screenshot.."; validationFailed("A0"); }
}
function Continue_A0YA() {
}
function Return_A0() {
    Toggle_Field("section", "A0", "hide");
    Toggle_Field("section", "A1", "show");
    Toggle_Field("control", "A1", "show");
    Toggle_Field("back", "A0A", "show");
    Toggle_Field("continue", "A1", "show");
}
function Return_A0A() {
    Toggle_Field("section", "A0", "show");
    Toggle_Field("section", "A1", "hide");
    Toggle_Field("control", "A1", "hide");
    Toggle_Field("back", "A0A", "hide");
    Toggle_Field("continue", "A1", "hide");
}
function Return_A0Y() {
    Toggle_Field("section", "A0", "show");
    Toggle_Field("section", "A4", "hide");
    Toggle_Field("control", "A4", "hide");
    Toggle_Field("back", "A0Y", "hide");
    Toggle_Field("continue", "A4", "hide");
    Toggle_Field("decline", "A0Y", "hide");
}
function Return_A0B() {
    Toggle_Field("section", "A0", "show");
    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("back", "A0B", "hide");
    Toggle_Field("continue", "A96", "hide");
}
function Return_A0E() {
    Toggle_Field("section", "A0", "show");
    Toggle_Field("section", "A45", "hide");
    Toggle_Field("control", "A45", "hide");
    Toggle_Field("back", "A0E", "hide");
    Toggle_Field("continue", "A45", "hide");
}
function Return_A0N() {
    Toggle_Field("section", "A0", "show");
    Toggle_Field("section", "A98", "hide");
    Toggle_Field("control", "A98", "hide");
    Toggle_Field("back", "A0N", "hide");
    Toggle_Field("continue", "A98", "hide");
}

function Continue_A1A() {
}


function Escape() {
    ToggleCallType("s", "h", "h");

    Toggle_Field("section", "A96", "show");
    Toggle_Field("control", "A96", "show");
    Toggle_Field("continue", "A96", "show"); $("#validation_message").html("");
}
function EscapeUndo() {
    ToggleCallType("s", "s", "s");

    Toggle_Field("section", "A96", "hide");
    Toggle_Field("control", "A96", "hide");
    Toggle_Field("continue", "A96", "hide");
}
/* Holiday */
function Designation_Toggle_Holiday_Hide() {
    /*
        The Holiday Catalog hides designations
        So we need to show them, and we need to hide the Holiday (if it exists)
    */
    var tstMsg = "Designation ID List";
    tstMsg += "\nDesignation ID: " + $("#cdHCDesignationId").val();
    var hDesignationID = $("#cdHCDesignationId").val();
    var span = document.getElementById("rb3_designation");
    var inputs = span.getElementsByTagName("input");
    for (var i = 0; i < inputs.length; ++i) {
        //tstMsg += "\n" + $(inputs[i]).val() + "|" + $(inputs[i]).attr('id') + "|" + i;
        if ($(inputs[i]).val() != hDesignationID) {
            $(inputs[i]).show();
            $('label[for="' + $(inputs[i]).attr('id') + '"]').show();
        } else {
            $(inputs[i]).hide();
            $('label[for="' + $(inputs[i]).attr('id') + '"]').hide();
        }
    }
    $(".sectionA3_hidden").show();
    $(".sectionA3_globe").hide();
    $(".sectionA3_drtv").hide();
    $(".sectionA3_standard").show();
    $(".sectionA3_main").show();
    $(".sectionA3_disaster").hide();
}
function Designation_Toggle_Holiday_Show() {
    /*
        Hide all designations that are NOT Holiday Designation
        cdHCDesignationId.Text
    */
    var hDesignationID = $("#cdHCDesignationId").val();
    var span = document.getElementById("rb3_designation");
    var inputs = span.getElementsByTagName("input");
    for (var i = 0; i < inputs.length; ++i) {
        if ($(inputs[i]).val() != hDesignationID) {
            $(inputs[i]).hide();
            $('label[for="' + $(inputs[i]).attr('id') + '"]').hide();
        }
        else if ($(inputs[i]).val() == hDesignationID) {
            $(inputs[i]).show();
            $('label[for="' + $(inputs[i]).attr('id') + '"]').show();
            $('input[name="rb3_designation"][value="' + hDesignationID + '"]').prop('checked', true);
            Validate_A3_GoTo('A41');
        }
    }
    $(".sectionA3_hidden").hide();
    $(".sectionA3_globe").hide();
    $(".sectionA3_drtv").hide();
    $(".sectionA3_standard").show();
    $(".sectionA3_main").show();
    $(".sectionA3_disaster").hide();
}
function Designation_Toggle_DRTV(tgle) {
    /*
        For DRTV we default to [158	DISASTER RELIEF]
        Initially hide all of them
        Enable a toggle to show main 3 [35, 109, 158]

        Designations
        35	LOCAL CHAPTER SUPPORT
        109	WHERE THE NEED IS GREATEST
        158	DISASTER RELIEF
        169	OTHER
        170	SPECIAL CAUSE DONATION
    */
    var hDesignationID = $("#cdHCDesignationId").val();
    var span = document.getElementById("rb3_designation");
    var inputs = span.getElementsByTagName("input");

    var rdValue = $("#rb1_options input:checked").val();

    for (var i = 0; i < inputs.length; ++i) {
        if ($(inputs[i]).val() == hDesignationID) {
            $(inputs[i]).hide();
            $('label[for="' + $(inputs[i]).attr('id') + '"]').hide();
        } else {
            if (tgle == "show") {
                // If Sustainer - Show the main 3 only
                // rdValue != "35" && rdValue != "109" && rdValue != "158" && rdValue != "179"
                // If Standard - Show all except OTHER/SPECIAL CAUSE (?)
                // Show all except: 169	OTHER | 170	SPECIAL CAUSE DONATION
                if (rdValue == "DRTV_YES") {
                    if ($(inputs[i]).val() == "35" || $(inputs[i]).val() == "109" || $(inputs[i]).val() == "158") {
                        $(inputs[i]).show();
                        $('label[for="' + $(inputs[i]).attr('id') + '"]').show();
                    }
                } else {
                    $(inputs[i]).show();
                    $('label[for="' + $(inputs[i]).attr('id') + '"]').show();
                    if ($(inputs[i]).val() != "169" && $(inputs[i]).val() != "170") {
                    }
                }
            } else {
                // Hide all except 158
                if ($(inputs[i]).val() != "158") {
                    $(inputs[i]).hide();
                    $('label[for="' + $(inputs[i]).attr('id') + '"]').hide();
                }
            }
        }
    }
    if (tgle == "show") {
        // $(".sectionA3_standard").show(); // section_A3high | sectionA3_main | sectionA3_disaster | section_A3sustainer
        $(".sectionA3_main").hide();
        // $(".sectionA3_hidden").hide(); // sectionA3_drtv | sectionA3_globe | sectionA3_disaster
        $(".sectionA3_drtv").hide();
        $(".sectionA3_globe").hide();
        $(".sectionA3_disaster").hide();
    } else {
        // $(".sectionA3_hidden").show();
        $(".sectionA3_globe").hide();
        $(".sectionA3_drtv").show();
        // $(".sectionA3_standard").hide();
        $(".sectionA3_main").hide();
        $(".sectionA3_disaster").hide();
    }
}
function Designation_Toggle_GLOBE(tgle) {
    /*
        For Globetrotters we only show [Disaster Relief == 158]
        We hide/show all others (Except for Holiday Catalog)
    */
    var hDesignationID = $("#cdHCDesignationId").val();
    var span = document.getElementById("rb3_designation");
    var inputs = span.getElementsByTagName("input");

    var rdValue = $("#rb1_options input:checked").val();

    for (var i = 0; i < inputs.length; ++i) {
        if ($(inputs[i]).val() == hDesignationID) {
            $(inputs[i]).hide();
            $('label[for="' + $(inputs[i]).attr('id') + '"]').hide();
        } else {
            if (tgle == "show") {
                // If Sustainer - Show the main 3 only
                // rdValue != "35" && rdValue != "109" && rdValue != "158" && rdValue != "179"
                // If Standard - Show all except OTHER/SPECIAL CAUSE (?)
                // Show all except: 169	OTHER | 170	SPECIAL CAUSE DONATION
                if (rdValue == "DRTV_YES") {
                    if ($(inputs[i]).val() == "35" || $(inputs[i]).val() == "109" || $(inputs[i]).val() == "158") {
                        $(inputs[i]).show();
                        $('label[for="' + $(inputs[i]).attr('id') + '"]').show();
                    }
                } else {
                    $(inputs[i]).show();
                    $('label[for="' + $(inputs[i]).attr('id') + '"]').show();
                    if ($(inputs[i]).val() != "169" && $(inputs[i]).val() != "170") {
                    }
                }
            } else {
                // Hide all except 158
                if ($(inputs[i]).val() != "158") {
                    $(inputs[i]).hide();
                    $('label[for="' + $(inputs[i]).attr('id') + '"]').hide();
                }
            }
        }
    }
    if (tgle == "show") {
        // $(".sectionA3_hidden").hide();
        $(".sectionA3_globe").hide();
        $(".sectionA3_drtv").hide();
        // $(".sectionA3_standard").show();
        $(".sectionA3_main").hide();
        $(".sectionA3_disaster").hide();
    } else {
        // $(".sectionA3_hidden").show();
        $(".sectionA3_globe").show();
        $(".sectionA3_drtv").hide();
        // $(".sectionA3_standard").hide();
        $(".sectionA3_main").hide();
        $(".sectionA3_disaster").hide();
    }
}
function Designation_Toggle_Custom(tgle, tglDes, tglDes2, tglDes3) {
    /*
        For Globetrotters we only show [Disaster Relief == 158]
        We hide/show all others (Except for Holiday Catalog)
        Hide all Designations that are not the selected one
    */
    var hDesignationID = $("#cdHCDesignationId").val();
    var span = document.getElementById("rb3_designation");
    var inputs = span.getElementsByTagName("input");

    var rdValue = $("#rb1_options input:checked").val();

    for (var i = 0; i < inputs.length; ++i) {
        if ($(inputs[i]).val() == hDesignationID) {
            $(inputs[i]).hide();
            $('label[for="' + $(inputs[i]).attr('id') + '"]').hide();
        } else {
            if (tgle == "show") {
                // If Sustainer - Show the main 3 only
                // rdValue != "35" && rdValue != "109" && rdValue != "158" && rdValue != "179"
                // If Standard - Show all except OTHER/SPECIAL CAUSE (?)
                // Show all except: 169	OTHER | 170	SPECIAL CAUSE DONATION
                if (rdValue == "DRTV_YES") {
                    if ($(inputs[i]).val() == "35" || $(inputs[i]).val() == "109" || $(inputs[i]).val() == "158") {
                        $(inputs[i]).show();
                        $('label[for="' + $(inputs[i]).attr('id') + '"]').show();
                    }
                } else {
                    $(inputs[i]).show();
                    $('label[for="' + $(inputs[i]).attr('id') + '"]').show();
                    if ($(inputs[i]).val() != "169" && $(inputs[i]).val() != "170") {
                    }
                }
            } else {
                // Hide all except custom
                if ($(inputs[i]).val() != tglDes && $(inputs[i]).val() != tglDes2 && $(inputs[i]).val() != tglDes3) {
                    $(inputs[i]).hide();
                    $('label[for="' + $(inputs[i]).attr('id') + '"]').hide();
                }
            }
        }
    }
    if (tgle == "show") {
        // $(".sectionA3_hidden").hide();
        $(".sectionA3_globe").hide();
        $(".sectionA3_drtv").hide();
        // $(".sectionA3_standard").show();
        $(".sectionA3_main").show();
        $(".sectionA3_disaster").hide();
    } else {
        // $(".sectionA3_hidden").show();
        $(".sectionA3_globe").hide();
        $(".sectionA3_drtv").hide();
        // $(".sectionA3_standard").show();
        $(".sectionA3_main").hide();
        $(".sectionA3_disaster").show();
    }
}
function Designation_Toggle_Array(tgle, tglArray) {
    /*
        For Globetrotters we only show [Disaster Relief == 158]
        We hide/show all others (Except for Holiday Catalog)
        Hide all Designations that are not the selected one
    */

    var desArray = tglArray.split(",");

    var hDesignationID = $("#cdHCDesignationId").val();
    var span = document.getElementById("rb3_designation");
    var inputs = span.getElementsByTagName("input");

    var rdValue = $("#rb1_options input:checked").val();

    for (var i = 0; i < inputs.length; ++i) {
        if ($(inputs[i]).val() == hDesignationID) {
            $(inputs[i]).hide();
            $('label[for="' + $(inputs[i]).attr('id') + '"]').hide();
        } else {
            if (tgle == "show") {
                // If Sustainer - Show the main 3 only
                // rdValue != "35" && rdValue != "109" && rdValue != "158" && rdValue != "179"
                // If Standard - Show all except OTHER/SPECIAL CAUSE (?)
                // Show all except: 169	OTHER | 170	SPECIAL CAUSE DONATION
                if (rdValue == "DRTV_YES") {
                    if ($(inputs[i]).val() == "35" || $(inputs[i]).val() == "109" || $(inputs[i]).val() == "158") {
                        $(inputs[i]).show();
                        $('label[for="' + $(inputs[i]).attr('id') + '"]').show();
                    }
                } else {
                    $(inputs[i]).show();
                    $('label[for="' + $(inputs[i]).attr('id') + '"]').show();
                    if ($(inputs[i]).val() != "169" && $(inputs[i]).val() != "170") {
                    }
                }
            } else {
                // Hide all except custom
                if (desArray.indexOf($(inputs[i]).val()) < 0) {
                    $(inputs[i]).hide();
                    $('label[for="' + $(inputs[i]).attr('id') + '"]').hide();
                }
            }
        }
    }
    if (tgle == "show") {
        // $(".sectionA3_hidden").hide();
        $(".sectionA3_globe").hide();
        $(".sectionA3_drtv").hide();
        // $(".sectionA3_standard").show();
        $(".sectionA3_main").show();
        $(".sectionA3_disaster").hide();
    } else {
        // $(".sectionA3_hidden").show();
        $(".sectionA3_globe").hide();
        $(".sectionA3_drtv").hide();
        // $(".sectionA3_standard").show();
        $(".sectionA3_main").hide();
        $(".sectionA3_disaster").show();
    }
}
/* Holiday */
function CloseWindow() {
    window.opener = self;

    close();
    window.close();
    parent.window.close();
    if (!window.closed) {
        // aelert("We were not able to close the tabn\rPlease close it manually.");
    }
}
