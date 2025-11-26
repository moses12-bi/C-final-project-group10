using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class GiveFeedback : System.Web.UI.Page
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
                LoadTeamMembers();
            }
        }

        private void LoadTeamMembers()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int currentUserId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                
                // Load team members if manager, otherwise all users
                string query = "";
                if (Session["Role"].ToString().ToLower() == "manager")
                {
                    query = @"SELECT user_id, full_name FROM Users 
                             WHERE manager_id = @mid AND user_id != @uid 
                             ORDER BY full_name";
                }
                else
                {
                    query = "SELECT user_id, full_name FROM Users WHERE user_id != @uid ORDER BY full_name";
                }
                
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", currentUserId);
                if (Session["Role"].ToString().ToLower() == "manager")
                {
                    cmd.Parameters.AddWithValue("@mid", currentUserId);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlReceiver.DataSource = dt;
                ddlReceiver.DataTextField = "full_name";
                ddlReceiver.DataValueField = "user_id";
                ddlReceiver.DataBind();
                ddlReceiver.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select User", ""));
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlReceiver.SelectedValue) || 
                string.IsNullOrEmpty(ddlFeedbackType.SelectedValue) || 
                string.IsNullOrEmpty(ddlRating.SelectedValue) ||
                string.IsNullOrEmpty(txtComments.Text.Trim()))
            {
                lblMessage.Text = "Please fill in all required fields.";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
                return;
            }

            int receiverId = Convert.ToInt32(ddlReceiver.SelectedValue);
            int senderId = Convert.ToInt32(Session["UserID"]);
            string feedbackType = ddlFeedbackType.SelectedValue;
            int rating = Convert.ToInt32(ddlRating.SelectedValue);
            string comments = txtComments.Text.Trim();

            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"INSERT INTO Feedback (sender_id, receiver_id, feedback_type, comments, rating, date_given) 
                                VALUES (@sid, @rid, @type, @comments, @rating, GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@sid", senderId);
                cmd.Parameters.AddWithValue("@rid", receiverId);
                cmd.Parameters.AddWithValue("@type", feedbackType);
                cmd.Parameters.AddWithValue("@comments", comments);
                cmd.Parameters.AddWithValue("@rating", rating);
                cmd.ExecuteNonQuery();

                // Create notification
                string senderName = Session["FullName"] != null ? Session["FullName"].ToString() : "Someone";
                string notifQuery = @"INSERT INTO Notifications (user_id, message, status, created_at) 
                                     VALUES (@uid, @msg, 'unread', GETDATE())";
                SqlCommand notifCmd = new SqlCommand(notifQuery, conn);
                notifCmd.Parameters.AddWithValue("@uid", receiverId);
                notifCmd.Parameters.AddWithValue("@msg", $"{senderName} has given you {feedbackType} feedback.");
                notifCmd.ExecuteNonQuery();

                lblMessage.Text = "Feedback submitted successfully!";
                lblMessage.CssClass = "alert alert-success";
                lblMessage.Visible = true;

                // Clear form
                ddlReceiver.SelectedIndex = 0;
                ddlFeedbackType.SelectedIndex = 0;
                ddlRating.SelectedIndex = 0;
                txtComments.Text = "";
            }
        }
    }
}

