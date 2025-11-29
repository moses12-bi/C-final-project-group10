using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;

namespace PTMS
{
    public partial class EmployeeDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check authentication
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Check role
            if (Session["Role"] != null && Session["Role"].ToString().ToLower() != "employee")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadEmployeeData();
                LoadStatistics();
                LoadNotifications();
                LoadRecentGoals();
               

            }
        }

        private void LoadEmployeeData()
        {
            if (Session["FullName"] != null)
            {
                lblEmployeeName.Text = Session["FullName"].ToString();
            }
            else
            {
                string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT full_name FROM Users WHERE user_id = @uid";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@uid", Session["UserID"]);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        lblEmployeeName.Text = result.ToString();
                        Session["FullName"] = result.ToString();
                    }
                }
            }
        }

        private void LoadStatistics()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Total Goals
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Goals WHERE user_id = @uid", conn);
                cmd1.Parameters.AddWithValue("@uid", userId);
                object result1 = cmd1.ExecuteScalar();
                lblTotalGoals.Text = result1 != null ? result1.ToString() : "0";

                // Completed Goals
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Goals WHERE user_id = @uid AND status='Completed'", conn);
                cmd2.Parameters.AddWithValue("@uid", userId);
                object result2 = cmd2.ExecuteScalar();
                lblCompletedGoals.Text = result2 != null ? result2.ToString() : "0";

                // Feedback Count
                SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM Feedback WHERE receiver_id = @uid", conn);
                cmd3.Parameters.AddWithValue("@uid", userId);
                object result3 = cmd3.ExecuteScalar();
                lblFeedbackCount.Text = result3 != null ? result3.ToString() : "0";

                // Pending Reviews
                SqlCommand cmd4 = new SqlCommand(@"SELECT COUNT(*) FROM Performance_Reviews pr 
                                                   INNER JOIN Review_Cycles rc ON pr.cycle_id = rc.cycle_id 
                                                   WHERE pr.employee_id = @uid AND rc.status = 'Active'", conn);
                cmd4.Parameters.AddWithValue("@uid", userId);
                object result4 = cmd4.ExecuteScalar();
                lblPendingReviews.Text = result4 != null ? result4.ToString() : "0";
            }
        }

        private void LoadNotifications()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT TOP 5 notification_id, message, status, created_at 
                                FROM Notifications 
                                WHERE user_id = @uid 
                                ORDER BY created_at DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptNotifications.DataSource = dt;
                rptNotifications.DataBind();
                lblNoNotifications.Visible = (dt.Rows.Count == 0);
            }
        }

        private void LoadRecentGoals()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT TOP 5 goal_id, title, description, status, progress, created_at 
                                FROM Goals 
                                WHERE user_id = @uid 
                                ORDER BY created_at DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptRecentGoals.DataSource = dt;
                rptRecentGoals.DataBind();
                lblNoGoals.Visible = (dt.Rows.Count == 0);
            }
        }

        protected string GetStatusBadgeColor(string status)
        {
            switch (status.ToLower())
            {
                case "completed":
                    return "success";
                case "in progress":
                case "inprogress":
                    return "primary";
                case "pending":
                    return "warning";
                case "rejected":
                    return "danger";
                default:
                    return "secondary";
            }
        }

        protected string FormatDate(object date)
        {
            if (date == null || date == DBNull.Value)
                return "N/A";

            try
            {
                DateTime dt = Convert.ToDateTime(date);
                TimeSpan diff = DateTime.Now - dt;

                if (diff.TotalMinutes < 1)
                    return "Just now";
                else if (diff.TotalMinutes < 60)
                    return $"{(int)diff.TotalMinutes} minutes ago";
                else if (diff.TotalHours < 24)
                    return $"{(int)diff.TotalHours} hours ago";
                else if (diff.TotalDays < 7)
                    return $"{(int)diff.TotalDays} days ago";
                else
                    return dt.ToString("MMM dd, yyyy");
            }
            catch
            {
                return "N/A";
            }
        }
    }
}
