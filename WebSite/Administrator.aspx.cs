using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DropDownList2.DataSource = Roles.GetUsersInRole("players");
        DropDownList2.DataBind();
        DropDownList3.DataSource = Roles.GetUsersInRole("bannedplayers");
        DropDownList3.DataBind();
    }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Membership.DeleteUser(DropDownList1.SelectedItem.Value,true);
        Server.Transfer("Administrator.aspx");
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        Session["lastUsername"] = "admin";
        Session["username"] = DropDownList1.SelectedItem.Value;
        Server.Transfer("UserInformation.aspx");
    }
    protected void Button3_Click(object sender, EventArgs e)
    {
        string username = DropDownList2.SelectedItem.Value;
        Roles.RemoveUserFromRole(username, "players");
        Roles.AddUserToRole(username, "bannedplayers");
        Server.Transfer("Administrator.aspx");
    }
    protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void Button4_Click(object sender, EventArgs e)
    {
        string username = DropDownList2.SelectedItem.Value;
        Roles.RemoveUserFromRole(username, "bannedplayers");
        Roles.AddUserToRole(username, "players");
        Server.Transfer("Administrator.aspx");
    }
}
