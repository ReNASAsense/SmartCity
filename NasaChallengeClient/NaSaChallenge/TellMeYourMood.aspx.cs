using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NaSaChallenge
{
    public partial class TellMeYourMood : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button_Click(object sender, EventArgs e)
        {
            //new HttpRequest("", "", "");
            Response.Redirect("Default.aspx");
        }
    }
}