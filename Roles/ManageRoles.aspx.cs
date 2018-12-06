using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;


using Application___Cash_Incentive;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;

public partial class Roles_ManageRoles : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            // DisplayRolesInGrid();
            DisplayRoles();

        }
    }
    private void DisplayRoles()
    {
        Label1.Text = "Roles:";

        var RoleManager = new RoleManager<CustomRole, int>(new RoleStore<CustomRole, int, CustomUserRole>(new ApplicationDbContext()));
        var roles = RoleManager.Roles;

        foreach (CustomRole role in roles)
        {
            Label1.Text += "<br />" + role.Name;
        }

        //var roleNames = RoleManager.ToString();
        //Label1.Text += "<hr /><br />" + roleNames;
        DisplayRolesInGrid();
    }
    private void DisplayRolesInGrid()
    {
        var RoleManager = new RoleManager<CustomRole, int>(new RoleStore<CustomRole, int, CustomUserRole>(new ApplicationDbContext()));
        var roleNames = RoleManager.Roles;

        RoleList.DataSource = roleNames.ToList();
        RoleList.DataBind();
    }

    protected void CreateRoleButton_Click(object sender, EventArgs e)
    {
        var RoleManager = new RoleManager<CustomRole, int>(new RoleStore<CustomRole, int, CustomUserRole>(new ApplicationDbContext()));


        string newRoleName = RoleName.Text.Trim();

        if (!RoleManager.RoleExists(newRoleName))
        {
            CustomRole newRole = new CustomRole();
            newRole.Name = newRoleName;

            // Create the role
            RoleManager.Create(newRole);

            // Refresh the RoleList Grid
            DisplayRolesInGrid();
        }

        RoleName.Text = string.Empty;
    }

    protected void RoleList_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        var RoleManager = new RoleManager<CustomRole, int>(new RoleStore<CustomRole, int, CustomUserRole>(new ApplicationDbContext()));

        // Get the RoleNameLabel
        Label RoleNameLabel = RoleList.Rows[e.RowIndex].FindControl("RoleNameLabel") as Label;
        string removeRoleName = RoleNameLabel.Text.Trim();

        if (!RoleManager.RoleExists(removeRoleName))
        {
            CustomRole removeRole = new CustomRole();
            removeRole.Name = removeRoleName;

            // Remove the role
            RoleManager.Delete(removeRole);

            // Refresh the RoleList Grid
            DisplayRolesInGrid();
        }

    }
}
