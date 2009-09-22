using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Security.Principal;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
    {
        Session["username"] = Login1.UserName.ToString();

        string[] roles = Roles.GetRolesForUser(Login1.UserName);
        GenericIdentity userIdentity = new GenericIdentity(Login1.UserName.ToString());
        GenericPrincipal userPrincipal = new GenericPrincipal(userIdentity, roles);
        Context.User = userPrincipal;

        string[] admins = Roles.GetUsersInRole("administrators");
        bool isAdmin = false;
        bool isPlayer = false;
        foreach (string s in admins)
            if (s.Equals(Login1.UserName.ToString()))
                isAdmin = true;

        string[] players = Roles.GetUsersInRole("players");
        foreach (string s in players)
            if (s.Equals(Login1.UserName.ToString()))
                isPlayer = true;

        if (isAdmin)
            Server.Transfer("Administrator.aspx");
        else if (isPlayer)
            Server.Transfer("UserInformation.aspx");
        else 
            Server.Transfer("Default.aspx");
    }
}
