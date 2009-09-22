using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Security.Principal;

public partial class UserInformation : System.Web.UI.Page
{
    string username;

    protected void Page_Load(object sender, EventArgs e)
    {
        DatabaseManager dbm = new DatabaseManager();
        username = (string)Session["username"];

        string[] sq = Roles.GetUsersInRole("administrators");

        //Response.Write(Roles.IsUserInRole(usr));
        
        /*Session["username"] = "dinel";
        username = "dinel";*/
        //playerStats = dbm.getPlayerStats(username);
        //pUps = dbm.getPlayerPowerUps(username);
        
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Membership.DeleteUser(username);
        Response.Redirect("default.aspx");
    }
}
