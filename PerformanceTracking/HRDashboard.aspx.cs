using System;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class HRDashboard : System.Web.UI.Page
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
                LoadStatistics();
            }
        }

        private void LoadStatistics()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Total Users
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Users", conn);
                object result1 = cmd1.ExecuteScalar();
                lblTotalUsers.Text = result1 != null ? result1.ToString() : "0";

                // Active Cycles
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Review_Cycles WHERE status = 'Active'", conn);
                object result2 = cmd2.ExecuteScalar();
                lblActiveCycles.Text = result2 != null ? result2.ToString() : "0";

                // Organization Average Score
                SqlCommand cmd3 = new SqlCommand("SELECT AVG(CAST(rating AS FLOAT)) FROM Performance_Reviews WHERE rating IS NOT NULL", conn);
                object result3 = cmd3.ExecuteScalar();
                if (result3 != null && result3 != DBNull.Value)
                {
                    double avg = Convert.ToDouble(result3);
                    lblOrgAvgScore.Text = avg.ToString("F1");
                }
                else
                {
                    lblOrgAvgScore.Text = "N/A";
                }

                // Pending Reviews
                SqlCommand cmd4 = new SqlCommand(@"SELECT COUNT(*) FROM Performance_Reviews pr 
                                                   INNER JOIN Review_Cycles rc ON pr.cycle_id = rc.cycle_id 
                                                   WHERE rc.status = 'Active' AND pr.rating IS NULL", conn);
                object result4 = cmd4.ExecuteScalar();
                lblPendingReviews.Text = result4 != null ? result4.ToString() : "0";
            }
        }
    }
}

