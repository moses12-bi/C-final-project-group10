using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class OrgGoals : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string role = Session["Role"] != null ? Session["Role"].ToString().ToLower() : "";
            if (role != "hr" && role != "admin")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadOrgGoals();
            }
        }

        private void LoadOrgGoals()
        {
            // For now, we'll use a placeholder. In a real system, you might have a separate OrgGoals table
            // or mark certain goals as organizational goals
            rptOrgGoals.DataSource = null;
            rptOrgGoals.DataBind();
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
            {
                lblMessage.Text = "Please fill in all fields.";
                lblMessage.Visible = true;
                return;
            }

            // In a real system, you would create organization goals here
            // For now, this is a placeholder
            lblMessage.Text = "Organization goal creation feature coming soon!";
            lblMessage.CssClass = "alert alert-info";
            lblMessage.Visible = true;
        }
    }
}

