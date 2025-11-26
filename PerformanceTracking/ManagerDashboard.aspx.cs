using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class ManagerDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Session["Role"] != null && Session["Role"].ToString().ToLower() != "manager")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadStatistics();
                LoadPendingGoals();
                LoadTeamMembers();
            }
        }

        private void LoadStatistics()
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

                // Pending Approvals
                SqlCommand cmd2 = new SqlCommand(@"SELECT COUNT(*) FROM Goals g 
                                                   INNER JOIN Users u ON g.user_id = u.user_id 
                                                   WHERE u.manager_id = @mid AND g.status = 'Pending'", conn);
                cmd2.Parameters.AddWithValue("@mid", managerId);
                object result2 = cmd2.ExecuteScalar();
                lblPendingApprovals.Text = result2 != null ? result2.ToString() : "0";

                // Reviews Due
                SqlCommand cmd3 = new SqlCommand(@"SELECT COUNT(*) FROM Performance_Reviews pr 
                                                   INNER JOIN Review_Cycles rc ON pr.cycle_id = rc.cycle_id 
                                                   INNER JOIN Users u ON pr.employee_id = u.user_id 
                                                   WHERE u.manager_id = @mid AND pr.manager_id = @mid 
                                                   AND rc.status = 'Active' AND pr.rating IS NULL", conn);
                cmd3.Parameters.AddWithValue("@mid", managerId);
                object result3 = cmd3.ExecuteScalar();
                lblReviewsDue.Text = result3 != null ? result3.ToString() : "0";

                // Team Average Score
                SqlCommand cmd4 = new SqlCommand(@"SELECT AVG(CAST(pr.rating AS FLOAT)) 
                                                   FROM Performance_Reviews pr 
                                                   INNER JOIN Users u ON pr.employee_id = u.user_id 
                                                   WHERE u.manager_id = @mid AND pr.rating IS NOT NULL", conn);
                cmd4.Parameters.AddWithValue("@mid", managerId);
                object result4 = cmd4.ExecuteScalar();
                if (result4 != null && result4 != DBNull.Value)
                {
                    double avg = Convert.ToDouble(result4);
                    lblTeamAvgScore.Text = avg.ToString("F1");
                }
                else
                {
                    lblTeamAvgScore.Text = "N/A";
                }
            }
        }

        private void LoadPendingGoals()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int managerId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT TOP 5 g.goal_id, g.title, g.description, u.full_name 
                                FROM Goals g 
                                INNER JOIN Users u ON g.user_id = u.user_id 
                                WHERE u.manager_id = @mid AND g.status = 'Pending' 
                                ORDER BY g.created_at DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mid", managerId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptPendingGoals.DataSource = dt;
                rptPendingGoals.DataBind();
                lblNoPendingGoals.Visible = (dt.Rows.Count == 0);
            }
        }

        private void LoadTeamMembers()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int managerId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT user_id, full_name, department 
                                FROM Users 
                                WHERE manager_id = @mid 
                                ORDER BY full_name";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mid", managerId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptTeamMembers.DataSource = dt;
                rptTeamMembers.DataBind();
                lblNoTeamMembers.Visible = (dt.Rows.Count == 0);
            }
        }
    }
}

