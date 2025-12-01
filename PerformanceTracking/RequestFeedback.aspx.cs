using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class RequestFeedback : System.Web.UI.Page
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
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int currentUserId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT user_id, full_name, role FROM Users WHERE user_id != @uid ORDER BY full_name";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", currentUserId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlRequestFrom.DataSource = dt;
                ddlRequestFrom.DataTextField = "full_name";
                ddlRequestFrom.DataValueField = "user_id";
                ddlRequestFrom.DataBind();
                ddlRequestFrom.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select User", ""));
            }
        }

        protected void btnRequest_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlRequestFrom.SelectedValue) || string.IsNullOrEmpty(ddlFeedbackType.SelectedValue))
            {
                lblMessage.Text = "Please select both recipient and feedback type.";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
                return;
            }

            int receiverId = Convert.ToInt32(ddlRequestFrom.SelectedValue);
            int senderId = Convert.ToInt32(Session["UserID"]);
            string feedbackType = ddlFeedbackType.SelectedValue;
            string message = txtMessage.Text.Trim();

            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                
                // Create notification for the recipient
                string senderName = Session["FullName"] != null ? Session["FullName"].ToString() : "Someone";
                string notificationMsg = $"{senderName} has requested {feedbackType} feedback from you.";
                if (!string.IsNullOrEmpty(message))
                    notificationMsg += $" Message: {message}";

                string query = @"INSERT INTO Notifications (user_id, message, status, created_at) 
                                VALUES (@uid, @msg, 'unread', GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", receiverId);
                cmd.Parameters.AddWithValue("@msg", notificationMsg);
                cmd.ExecuteNonQuery();

                lblMessage.Text = "Feedback request sent successfully!";
                lblMessage.CssClass = "alert alert-success";
                lblMessage.Visible = true;

                // Clear form
                ddlRequestFrom.SelectedIndex = 0;
                ddlFeedbackType.SelectedIndex = 0;
                txtMessage.Text = "";
            }
        }
    }
}

