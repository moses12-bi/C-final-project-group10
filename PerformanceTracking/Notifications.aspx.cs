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

                // Add a column to store if it's a feedback request and requester ID
                dt.Columns.Add("IsFeedbackRequest", typeof(bool));
                dt.Columns.Add("RequesterId", typeof(int));
                dt.Columns.Add("DisplayMessage", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    string message = row["message"].ToString();
                    bool isFeedbackRequest = message.Contains("requested") && message.Contains("feedback");
                    row["IsFeedbackRequest"] = isFeedbackRequest;
                    
                    // Extract requester ID from message if it's a feedback request
                    int requesterId = 0;
                    string displayMessage = message;
                    if (isFeedbackRequest && message.Contains("[REQ_ID:"))
                    {
                        int startIndex = message.IndexOf("[REQ_ID:") + 8;
                        int endIndex = message.IndexOf("]", startIndex);
                        if (endIndex > startIndex)
                        {
                            string idStr = message.Substring(startIndex, endIndex - startIndex);
                            if (int.TryParse(idStr, out requesterId))
                            {
                                row["RequesterId"] = requesterId;
                                // Remove the ID marker from display message
                                displayMessage = message.Replace($" [REQ_ID:{requesterId}]", "");
                            }
                        }
                    }
                    row["RequesterId"] = requesterId;
                    row["DisplayMessage"] = displayMessage;
                }

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

        protected string GetFeedbackButton(object isFeedbackRequest, object requesterId)
        {
            if (isFeedbackRequest == null || requesterId == null)
                return "";

            bool isRequest = Convert.ToBoolean(isFeedbackRequest);
            int reqId = Convert.ToInt32(requesterId);

            if (isRequest && reqId > 0)
            {
                return $"<div class='mt-2'><a href='GiveFeedback.aspx?requesterId={reqId}' class='btn btn-sm btn-primary'><i class='fas fa-comment-alt'></i> Give Feedback</a></div>";
            }
            return "";
        }
    }
}

