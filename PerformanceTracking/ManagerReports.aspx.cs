using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class ManagerReports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["Role"].ToString().ToLower() != "manager")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadReports();
            }
        }

        private void LoadReports()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int managerId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Team Size
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Users WHERE manager_id = @mid", conn);
                cmd1.Parameters.AddWithValue("@mid", managerId);
                object result1 = cmd1.ExecuteScalar();
                lblTeamSize.Text = result1 != null ? result1.ToString() : "0";

                // Total Goals
                SqlCommand cmd2 = new SqlCommand(@"SELECT COUNT(*) FROM Goals g 
                                                   INNER JOIN Users u ON g.user_id = u.user_id 
                                                   WHERE u.manager_id = @mid", conn);
                cmd2.Parameters.AddWithValue("@mid", managerId);
                object result2 = cmd2.ExecuteScalar();
                lblTotalGoals.Text = result2 != null ? result2.ToString() : "0";

                // Average Rating
                SqlCommand cmd3 = new SqlCommand(@"SELECT AVG(CAST(pr.rating AS FLOAT)) 
                                                   FROM Performance_Reviews pr 
                                                   INNER JOIN Users u ON pr.employee_id = u.user_id 
                                                   WHERE u.manager_id = @mid AND pr.rating IS NOT NULL", conn);
                cmd3.Parameters.AddWithValue("@mid", managerId);
                object result3 = cmd3.ExecuteScalar();
                if (result3 != null && result3 != DBNull.Value)
                {
                    double avg = Convert.ToDouble(result3);
                    lblAvgRating.Text = avg.ToString("F1");
                }
                else
                {
                    lblAvgRating.Text = "N/A";
                }

                // Team Summary
                string query = @"SELECT u.full_name,
                                (SELECT COUNT(*) FROM Goals WHERE user_id = u.user_id) as total_goals,
                                (SELECT COUNT(*) FROM Goals WHERE user_id = u.user_id AND status = 'Completed') as completed_goals,
                                (SELECT AVG(CAST(rating AS FLOAT)) FROM Performance_Reviews WHERE employee_id = u.user_id AND rating IS NOT NULL) as avg_rating
                                FROM Users u
                                WHERE u.manager_id = @mid
                                ORDER BY u.full_name";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mid", managerId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvTeamSummary.DataSource = dt;
                gvTeamSummary.DataBind();
            }
        }
    }
}

