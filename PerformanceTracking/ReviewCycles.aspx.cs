using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace PTMS
{
    public partial class ReviewCycles : System.Web.UI.Page
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
                LoadCycles();
            }
        }

        private void LoadCycles()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT cycle_id, cycle_name, start_date, end_date, status FROM Review_Cycles ORDER BY start_date DESC";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptCycles.DataSource = dt;
                rptCycles.DataBind();
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string cycleName = txtCycleName.Text.Trim();
            string startDate = txtStartDate.Text;
            string endDate = txtEndDate.Text;

            if (string.IsNullOrEmpty(cycleName) || string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
            {
                lblMessage.Text = "Please fill in all fields.";
                lblMessage.Visible = true;
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"INSERT INTO Review_Cycles (cycle_name, start_date, end_date, status) 
                               VALUES (@name, @start, @end, 'Active')";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", cycleName);
                cmd.Parameters.AddWithValue("@start", startDate);
                cmd.Parameters.AddWithValue("@end", endDate);
                cmd.ExecuteNonQuery();

                txtCycleName.Text = "";
                txtStartDate.Text = "";
                txtEndDate.Text = "";
                LoadCycles();
                
                ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", 
                    "var modal = bootstrap.Modal.getInstance(document.getElementById('cycleModal')); if(modal) modal.hide();", true);
            }
        }

        protected string GetStatusBadgeColor(string status)
        {
            return status.ToLower() == "active" ? "success" : "secondary";
        }

        protected string FormatDate(object date)
        {
            if (date == null || date == DBNull.Value)
                return "N/A";

            try
            {
                DateTime dt = Convert.ToDateTime(date);
                return dt.ToString("MMM dd, yyyy");
            }
            catch
            {
                return "N/A";
            }
        }
    }
}

