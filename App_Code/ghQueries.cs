using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
/// <summary>
/// Summary description for SQL_Queries
/// </summary>
public class ghQueries
{
    public ghQueries()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    static public String de_five9_call_check()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
                SELECT
                TOP 1
                1
                FROM [dbo].[five9_call] [fc]
                WHERE 1=1
                AND [fc].[callid] = @sp_call_call_id
                -- We do not use the Five9 campaign ID
                -- We would need to look up the ItemID
                -- 
                -- AND [fc].[call.campaign_id] = @sp_call_campaign_id
                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String de_five9_call_get()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
                SELECT
                [fc].[interactionid]
                ,[fc].[companyid]
                ,[ia].[arc.callid]
                ,[ia].[arc.disposition_name]
                FROM [dbo].[five9_call] [fc]
                LEFT OUTER JOIN [dbo].[interactions_arc] [ia] ON [ia].[companyid] = [fc].[companyid] AND [ia].[interactionid] = [fc].[interactionid]
                WHERE 1=1
                AND [fc].[call.call_id] = @sp_call_call_id
                AND [fc].[call.campaign_id] = @sp_call_campaign_id
                            ";
        cmdText += "\r";
        #endregion Build cmdText
        return cmdText;
    }
    static public String de_five9_call_insert_old()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
                                INSERT INTO [dbo].[five9_call]
                                           ([companyid]
                                           ,[interactionid]
                                           ,[call.call_id]
                                           ,[call.campaign_id]
                                           ,[createdate]
                                           ,[call.skill_id]
                                           ,[call.type]
                                           ,[call.dnis]
                                           ,[call.ani]
                                           ,[call.mediatype]
                                           ,[call.number]
                                           ,[call.session_id]
                                           ,[call.campaign_name]
                                           ,[call.skill_name]
                                           ,[call.comments]
                                           ,[call.start_timestamp]
                                           ,[call.end_timestamp]
                                           ,[call.tcpa_date_of_consent]
                                           ,[call.queue_time]
                                           ,[call.hold_time]
                                           ,[call.park_time]
                                           ,[call.wrapup_time]
                                           ,[call.bill_time]
                                           ,[call.handle_time]
                                           ,[call.length])
                                     SELECT
			                                @sp_companyid
                                           ,@sp_interactionid
                                           ,@sp_call_call_id
                                           ,@sp_call_campaign_id
                                           ,@sp_createdate
                                           ,@sp_call_skill_id
                                           ,@sp_call_type
                                           ,@sp_call_dnis
                                           ,@sp_call_ani
                                           ,@sp_call_mediatype
                                           ,@sp_call_number
                                           ,@sp_call_session_id
                                           ,@sp_call_campaign_name
                                           ,@sp_call_skill_name
                                           ,@sp_call_comments
                                           ,@sp_call_start_timestamp
                                           ,@sp_call_end_timestamp
                                           ,@sp_call_tcpa_date_of_consent
                                           ,@sp_call_queue_time
                                           ,@sp_call_hold_time
                                           ,@sp_call_park_time
                                           ,@sp_call_wrapup_time
                                           ,@sp_call_bill_time
                                           ,@sp_call_handle_time
                                           ,@sp_call_length

                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String de_five9_call_insert()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	)
BEGIN
	UPDATE [dbo].[five9_call]
		SET [datestart] = CASE WHEN @sp_datestart < [datestart] THEN @sp_datestart ELSE [datestart] END
		,[dateend] = CASE WHEN @sp_datestart > [dateend] THEN @sp_datestart ELSE [dateend] END
		,[campaignid] = CASE WHEN @sp_campaignid IS NOT NULL AND @sp_campaignid > 0 THEN @sp_campaignid ELSE [campaignid] END
		,[skillid] = CASE WHEN @sp_skillid IS NOT NULL AND @sp_skillid > 0 THEN @sp_skillid ELSE [skillid] END
		,[typeid] = CASE WHEN @sp_typeid IS NOT NULL AND @sp_typeid > 0 THEN @sp_typeid ELSE [typeid] END
		,[mediatypeid] = CASE WHEN @sp_mediatypeid IS NOT NULL AND @sp_mediatypeid > 0 THEN @sp_mediatypeid ELSE [mediatypeid] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call]
			   ([companyid],[interactionid],[callid],[datestart],[dateend],[sessionid],[campaignid],[skillid],[typeid],[mediatypeid])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_datestart,@sp_dateend,@sp_sessionid,@sp_campaignid,@sp_skillid,@sp_typeid,@sp_mediatypeid
END
                                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String de_five9_call_counts_insert()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_counts]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	)
BEGIN
	UPDATE [dbo].[five9_call_counts]
		SET [contacted] = CASE WHEN @sp_contacted > [contacted] THEN @sp_contacted ELSE [contacted] END
		,[abandoned] = CASE WHEN @sp_abandoned > [abandoned] THEN @sp_abandoned ELSE [abandoned] END
		,[transfers] = CASE WHEN @sp_transfers > [transfers] THEN @sp_transfers ELSE [transfers] END
		,[parks] = CASE WHEN @sp_parks > [parks] THEN @sp_parks ELSE [parks] END
		,[holds] = CASE WHEN @sp_holds > [holds] THEN @sp_holds ELSE [holds] END
		,[conferences] = CASE WHEN @sp_conferences > [conferences] THEN @sp_conferences ELSE [conferences] END
		,[voicemails] = CASE WHEN @sp_voicemails > [voicemails] THEN @sp_voicemails ELSE [voicemails] END
		,[recordings] = CASE WHEN @sp_recordings > [recordings] THEN @sp_recordings ELSE [recordings] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call_counts]
			   ([companyid],[interactionid],[callid],[contacted],[abandoned],[transfers],[parks],[holds],[conferences],[voicemails],[recordings])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_contacted,@sp_abandoned,@sp_transfers,@sp_parks,@sp_holds,@sp_conferences,@sp_voicemails,@sp_recordings
END
                                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String de_five9_call_time_insert()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_time]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	)
BEGIN
	UPDATE [dbo].[five9_call_time]
		SET [length] = CASE WHEN @sp_length > [length] THEN @sp_length ELSE [length] END
		,[bill_time] = CASE WHEN @sp_bill_time > [bill_time] THEN @sp_bill_time ELSE [bill_time] END
		,[call_time] = CASE WHEN @sp_call_time > [call_time] THEN @sp_call_time ELSE [call_time] END
		,[dial_time] = CASE WHEN @sp_dial_time > [dial_time] THEN @sp_dial_time ELSE [dial_time] END
		,[conference_time] = CASE WHEN @sp_conference_time > [conference_time] THEN @sp_conference_time ELSE [conference_time] END
		,[consult_time] = CASE WHEN @sp_consult_time > [consult_time] THEN @sp_consult_time ELSE [consult_time] END
		,[handle_time] = CASE WHEN @sp_handle_time > [handle_time] THEN @sp_handle_time ELSE [handle_time] END
		,[hold_time] = CASE WHEN @sp_hold_time > [hold_time] THEN @sp_hold_time ELSE [hold_time] END
		,[ivr_time] = CASE WHEN @sp_ivr_time > [ivr_time] THEN @sp_ivr_time ELSE [ivr_time] END
		,[park_time] = CASE WHEN @sp_park_time > [park_time] THEN @sp_park_time ELSE [park_time] END
		,[preview_time] = CASE WHEN @sp_preview_time > [preview_time] THEN @sp_preview_time ELSE [preview_time] END
		,[queue_time] = CASE WHEN @sp_queue_time > [queue_time] THEN @sp_queue_time ELSE [queue_time] END
		,[ring_time] = CASE WHEN @sp_ring_time > [ring_time] THEN @sp_ring_time ELSE [ring_time] END
		,[talk_time] = CASE WHEN @sp_talk_time > [talk_time] THEN @sp_talk_time ELSE [talk_time] END
		,[thirdparty_time] = CASE WHEN @sp_thirdparty_time > [thirdparty_time] THEN @sp_thirdparty_time ELSE [thirdparty_time] END
		,[wrapup_time] = CASE WHEN @sp_wrapup_time > [wrapup_time] THEN @sp_wrapup_time ELSE [wrapup_time] END
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call_time]
			   ([companyid],[interactionid],[callid],[length],[bill_time],[call_time],[dial_time],[conference_time],[consult_time],[handle_time],[hold_time]
			   ,[ivr_time],[park_time],[preview_time],[queue_time],[ring_time],[talk_time],[thirdparty_time],[wrapup_time])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_length,@sp_bill_time,@sp_call_time,@sp_dial_time,@sp_conference_time,@sp_consult_time,@sp_handle_time,@sp_hold_time
			   ,@sp_ivr_time,@sp_park_time,@sp_preview_time,@sp_queue_time,@sp_ring_time,@sp_talk_time,@sp_thirdparty_time,@sp_wrapup_time
END
                                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String de_five9_call_disposition_insert()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_disposition]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	AND [dispositionid] = @sp_dispositionid
	AND [agentid] = @sp_agentid
	)
BEGIN
	SELECT 1
END
ELSE
BEGIN
    -- We need to check if the disposition we want to add exists with the AGENTID == 0 or -1
    IF EXISTS(
	    SELECT TOP 1 1
	    FROM [dbo].[five9_call_disposition]
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
	    AND [dispositionid] = @sp_dispositionid
	    AND [agentid] = 0
	    )
    BEGIN
	    UPDATE [dbo].[five9_call_disposition]
		    SET [agentid] = @sp_agentid
		    ,[datecreated] = CASE WHEN @sp_datecreated > [datecreated] THEN @sp_datecreated ELSE [datecreated] END
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
	    AND [dispositionid] = @sp_dispositionid
	    AND [agentid] = 0
    END
    ELSE
    BEGIN
	    INSERT INTO [dbo].[five9_call_disposition]
			       ([companyid],[interactionid],[callid],[dispositionid],[agentid],[datecreated])
		     SELECT
			       @sp_companyid,@sp_interactionid,@sp_callid,@sp_dispositionid,@sp_agentid,@sp_datecreated
    END
END

                                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String de_five9_call_agent_insert()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_agent]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	AND [agentid] = @sp_agentid
	)
BEGIN
    IF EXISTS(
	    SELECT TOP 1 1
	    FROM [dbo].[five9_call_agent]
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
	    AND [stationid] = 0
	    AND @sp_stationid > 0
	    )
    BEGIN
	    UPDATE [dbo].[five9_call_agent]
	        SET [stationid] = @sp_stationid
	        ,[stationtype] = @sp_stationtype
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
    END
    ELSE
    BEGIN
	    SELECT 1
    END
END
ELSE
BEGIN
	INSERT INTO [dbo].[five9_call_agent]
			   ([companyid],[interactionid],[callid],[agentid],[datecreated],[stationid],[stationtype])
		 SELECT
			   @sp_companyid,@sp_interactionid,@sp_callid,@sp_agentid,@sp_datecreated,@sp_stationid,@sp_stationtype
END
                                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String de_five9_call_note_insert()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
IF EXISTS(
	SELECT TOP 1 1
	FROM [dbo].[five9_call_note]
	WHERE [companyid] = @sp_companyid
	AND [interactionid] = @sp_interactionid
	AND [callid] = @sp_callid
	AND [note] = @sp_note
	AND [agentid] = @sp_agentid
	)
BEGIN
	SELECT 1
END
ELSE
BEGIN
    -- We need to check if the note we want to add exists with the AGENTID == 0 or -1
    IF EXISTS(
	    SELECT TOP 1 1
	    FROM [dbo].[five9_call_note]
	    WHERE [companyid] = @sp_companyid
	    AND [interactionid] = @sp_interactionid
	    AND [callid] = @sp_callid
    	AND [note] = @sp_note
	    AND @sp_agentid = 0
	    )
	BEGIN
		SELECT 1
	END
	ELSE
	BEGIN
		IF EXISTS(
			SELECT TOP 1 1
			FROM [dbo].[five9_call_note]
			WHERE [companyid] = @sp_companyid
			AND [interactionid] = @sp_interactionid
			AND [callid] = @sp_callid
    		AND [note] = @sp_note
			AND [agentid] = 0
			)
		BEGIN
			UPDATE [dbo].[five9_call_note]
				SET [agentid] = @sp_agentid
				,[datecreated] = CASE WHEN @sp_datecreated > [datecreated] THEN @sp_datecreated ELSE [datecreated] END
			WHERE [companyid] = @sp_companyid
			AND [interactionid] = @sp_interactionid
			AND [callid] = @sp_callid
    		AND [note] = @sp_note
			AND [agentid] = 0
		END
		ELSE
		BEGIN
			INSERT INTO [dbo].[five9_call_note]
					   ([companyid],[interactionid],[callid],[datecreated],[agentid],[note])
				 SELECT
					   @sp_companyid,@sp_interactionid,@sp_callid,@sp_datecreated,@sp_agentid,@sp_note
		END
	END
END
                                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String de_interactions_insert()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
                SET NOCOUNT ON
                INSERT INTO [dbo].[interactions]
                ([companyid],[createdate],[interactiontype],[resourcetype],[resourceid],[originator],[destinator],[duration],[status])
                SELECT
                @sp_companyid,@sp_createdate,@sp_interactiontype,@sp_resourcetype,@sp_resourceid,@sp_originator,@sp_destinator,@sp_duration,@sp_status

                SELECT SCOPE_IDENTITY()
                SET NOCOUNT OFF
                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String de_interactions_arc_insert_update()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
IF NOT EXISTS(
SELECT
TOP 1
1
FROM [dbo].[interactions_arc] [ia] WITH(NOLOCK)
WHERE [ia].[companyid] = @sp_companyid
AND [ia].[interactionid] = @sp_interactionid
AND [ia].[arc.callid] = @sp_arc_callid
)BEGIN
                            INSERT INTO [dbo].[interactions_arc]
                            ([companyid]
                            ,[interactionid]
                            ,[arc.callid]
                            ,[createdate]
                            ,[arc.disposition_id]
                            ,[arc.disposition_name])
                            SELECT
                            @sp_companyid
                            ,@sp_interactionid
                            ,@sp_arc_callid
                            ,@sp_createdate
                            ,@sp_arc_disposition_id
                            ,@sp_arc_disposition_name
END
ELSE
BEGIN
    UPDATE [dbo].[interactions_arc]
    SET [arc.disposition_id] = @sp_arc_disposition_id
    ,[arc.disposition_name] = @sp_arc_disposition_name
    WHERE [companyid] = @sp_companyid
    AND [interactionid] = @sp_interactionid
    AND [arc.callid] = @sp_arc_callid
END
                    ";
        #endregion Build cmdText
        return cmdText;
    }

    static public String arc_call_get()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
SELECT
TOP (@sp_top)
[c].[callid]
,[c].[calluuid]
,[di].[id] [orderid]
,[di].[designationid]
,[ds].[displayname] [designation]
,[di].[donationamount]
,LEFT([di].[ccnum],1) + '***' + RIGHT([di].[ccnum],4) [ccnum]
,[di].[ccexpmonth]
,[di].[ccexpyear]
,[cb].[status]
,[c].[dispositionid]
,[c].[logindatetime]
,[c].[callenddatetime]
,DATEDIFF(s,[c].[logindatetime],[c].[callenddatetime]) [duration]
,[cb].[createdate] [cb_createdate]
,(SELECT TOP 1 [d].[displayname] FROM [dbo].[disposition] [d] WHERE [d].[dispositionid] = [c].[dispositionid]) [dispositionname]
,'----' [callinfo]
,[ci].[callid] [ci_callid]
,[ci].[fname] [ci_fname]
,[ci].[lname] [ci_lname]
,[ci].[prefix] [ci_prefix]
,[ci].[companyyn] [ci_companyyn]
,[ci].[address] [ci_address]
,[ci].[suitetype] [ci_suitetype]
,[ci].[suitenumber] [ci_suitenumber]
,[ci].[zip] [ci_zip]
,[ci].[city] [ci_city]
,[ci].[state] [ci_state]
,[ci].[hphone] [ci_hphone]
,[ci].[receiveupdatesyn] [ci_receiveupdatesyn]
,[ci].[email] [ci_email]
,[ci].[companyname] [ci_companyname]
,[ci].[companytypeid] [ci_companytypeid]
,[ci].[imoihoyn] [ci_imoihoyn]
,[ci].[anonymousyn] [ci_anonymousyn]
,[ci].[flag_1] [ci_flag_1]
,[ci].[flag_2] [ci_flag_2]
,[ci].[flag_3] [ci_flag_3]
,[ci].[flag_4] [ci_flag_4]
,[ci].[flag_5] [ci_flag_5]
,[ci].[flag_6] [ci_flag_6]
,[ci].[flag_7] [ci_flag_7]
,[ci].[sendack] [ci_sendack]
,[ci].[ackaddress] [ci_ackaddress]
,[ci].[phone_type] [ci_phone_type]
,[ci].[phone_optin] [ci_phone_optin]
,[ci].[phone2] [ci_phone2]
,[ci].[phone2_type] [ci_phone2_type]
,[ci].[phone2_optin] [ci_phone2_optin]
,[ci].[email2] [ci_email2]
,[ci].[receipt_email] [ci_receipt_email]
,[ci].[country] [ci_country]
,[cc].[callcreateid]
,[ss].[standardselectionid]
,[cd].[chargedateid]
,[cb].[id] [chargeid]
,[rm].[id] [removeid]
--,[di].* -- Check if we need this...
FROM [dbo].[call] [c] WITH(NOLOCK)
LEFT OUTER JOIN [dbo].[donationccinfo] [di] WITH(NOLOCK) ON [di].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[cybersource_log_auth] [cb] WITH(NOLOCK) ON [cb].[externalid] = [di].[id]
LEFT OUTER JOIN [dbo].[callinfo] [ci] WITH(NOLOCK) ON [ci].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[designation] [ds] WITH(NOLOCK) ON [ds].[designationid] = [di].[designationid]
LEFT OUTER JOIN [dbo].[callcreate] [cc] WITH(NOLOCK) ON [cc].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[standardselection] [ss] WITH(NOLOCK) ON [ss].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[chargedate] [cd] WITH(NOLOCK) ON [cd].[callid] = [c].[callid]
LEFT OUTER JOIN [dbo].[remove] [rm] WITH(NOLOCK) ON [rm].[callid] = [c].[callid]
WHERE [c].[callid] = @sp_callid
ORDER BY [cb].[createdate] DESC -- Get the latest record...
                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String arc_call_get_disposition()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText += @"
SELECT
TOP (@sp_top)
[c].[callid]
,[c].[dispositionid]
,(SELECT TOP 1 [d].[displayname] FROM [dbo].[disposition] [d] WHERE [d].[dispositionid] = [c].[dispositionid]) [dispositionname]
FROM [dbo].[call] [c] WITH(NOLOCK)
WHERE [c].[callid] = @sp_callid
                            ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String arc_designation_load()
    {
        #region Build cmdText
        String cmdText = "";
        cmdText = @"
SELECT
[d].[designationid]
,[d].[displayname]
,[d].[pagelocationid]
,[d].[merchantid]
,[d].[fundcode]
,CASE
	WHEN [d].[status] IS NULL OR [d].[status] = '' THEN 'D'
	ELSE [d].[status]
END [status]
,'----------------' [dots]
,[d].[name]
,[d].[sort]
,(SELECT TOP 1 [id].[description] FROM [dbo].[item_description] [id]
		WHERE [id].[itemtypeid] = 1 -- Designation Name
		AND [id].[itemid] = [d].[designationid] 
		AND [id].[languageid] = 0 -- Spanish
		) [name_spanish]
,[d].[status_online]
,[d].[status_adu]
,[d].[continue]
,[d].[description]
--,[d].[description_spanish]
,(SELECT TOP 1 [id].[description] FROM [dbo].[item_description] [id]
		WHERE [id].[itemtypeid] = 2 -- Designation Description
		AND [id].[itemid] = [d].[designationid] 
		AND [id].[languageid] = 0 -- Spanish
		) [description_spanish]
,[d].[agentnote_top]
,[d].[agentnote_bottom]
FROM [dbo].[designation] [d]
WHERE 1=1
AND [d].[status] = 'A'
AND ([d].[status_online] = 1
    -- OR ([d].[designationid] = 176 AND @sp_dnis IN ('1060','1055','9498001060','9498001055'))
	OR ((SELECT TOP 1 [dn].[company] FROM [dbo].[dnis] [dn] WITH(NOLOCK) WHERE [dn].[phonenumber] = @sp_dnis) = [d].[name])
    OR ([d].[designationid] = 184 AND @sp_dnis IN ('5632','5645','9496085632','9496085645')) -- Peru | Univision
    OR ([d].[designationid] = 184 AND @sp_dnis IN ('1055','1060','9498001055','9498001060')) -- Peru | Telemundo
    OR ([d].[designationid] = 185 AND @sp_dnis IN ('1055','1060','9498001055','9498001060')) -- Colombia | Telemundo
    OR ([d].[designationid] IN (189,190) AND @sp_dnis IN (SELECT CASE WHEN LEN(@sp_dnis) = 4 THEN [di].[dnis] ELSE [di].[phonenumber] END FROM [dbo].[dnis] [di] WITH(NOLOCK) WHERE [di].[line] IN ('800-842-2200 UNIVISION','800-596-6567 TELEMUNDO'))) -- Colombia | Telemundo
)
ORDER BY ISNULL([d].[sort],999), [d].[designationid]
                                        ";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_sample()
    {
        #region Build cmdText
        String cmdText = "";
        #endregion Build cmdText
        return cmdText;
    }
    static public String dashboard_sample2()
    {
        #region Build cmdText
        String cmdText = ghQueries.dashboard_sample();
        #endregion Build cmdText
        return cmdText;
    }
}
