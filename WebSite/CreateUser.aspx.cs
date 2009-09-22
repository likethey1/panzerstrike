using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Web.Security;  


public partial class Haha : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    protected void ContinueButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("UserInformation.aspx");
    }
    protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
    {

    }
}
