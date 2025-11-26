using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class Notifications : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadNotifications();
            }
        }

        private void LoadNotifications()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT notification_id, message, status, created_at 
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

        protected void btnMarkAllRead_Click(object sender, EventArgs e)
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "UPDATE Notifications SET status = 'read' WHERE user_id = @uid AND status = 'unread'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();
            }

            LoadNotifications();
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

