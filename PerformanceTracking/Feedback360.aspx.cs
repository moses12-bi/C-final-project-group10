using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace PTMS
{
    public partial class Feedback360 : System.Web.UI.Page
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
                LoadRecipients();
            }
        }

        private void LoadRecipients()
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

                chkRecipients.DataSource = dt;
                chkRecipients.DataTextField = "full_name";
                chkRecipients.DataValueField = "user_id";
                chkRecipients.DataBind();
            }
        }

        protected void btnRequest_Click(object sender, EventArgs e)
        {
            int selectedCount = 0;
            foreach (ListItem item in chkRecipients.Items)
            {
                if (item.Selected)
                    selectedCount++;
            }

            if (selectedCount == 0)
            {
                lblMessage.Text = "Please select at least one recipient.";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
                return;
            }

            string message = txtMessage.Text.Trim();
            string senderName = Session["FullName"] != null ? Session["FullName"].ToString() : "Someone";
            string requestMsg = $"{senderName} has requested 360-degree feedback from you.";
            if (!string.IsNullOrEmpty(message))
                requestMsg += $" Message: {message}";

            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                
                foreach (ListItem item in chkRecipients.Items)
                {
                    if (item.Selected)
                    {
                        int receiverId = Convert.ToInt32(item.Value);
                        
                        string query = @"INSERT INTO Notifications (user_id, message, status, created_at) 
                                        VALUES (@uid, @msg, 'unread', GETDATE())";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@uid", receiverId);
                        cmd.Parameters.AddWithValue("@msg", requestMsg);
                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = $"Feedback requests sent to {selectedCount} recipient(s)!";
                lblMessage.CssClass = "alert alert-success";
                lblMessage.Visible = true;

                // Clear selections
                foreach (ListItem item in chkRecipients.Items)
                    item.Selected = false;
                txtMessage.Text = "";
            }
        }
    }
}

