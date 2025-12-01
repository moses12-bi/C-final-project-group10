using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace PTMS
{
    public partial class Goals : System.Web.UI.Page
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
                LoadGoals();
                
                // Check if editing
                if (Request.QueryString["edit"] != null)
                {
                    int goalId = Convert.ToInt32(Request.QueryString["edit"]);
                    LoadGoalForEdit(goalId);
                }
            }
        }

        private void LoadGoals()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT goal_id, title, description, status, progress, created_at, manager_comment 
                                FROM Goals 
                                WHERE user_id = @uid 
                                ORDER BY created_at DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptGoals.DataSource = dt;
                rptGoals.DataBind();
                lblNoGoals.Visible = (dt.Rows.Count == 0);
            }
        }

        private void LoadGoalForEdit(int goalId)
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM Goals WHERE goal_id = @gid AND user_id = @uid";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@gid", goalId);
                cmd.Parameters.AddWithValue("@uid", userId);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    hdnGoalId.Value = goalId.ToString();
                    txtTitle.Text = dr["title"].ToString();
                    txtDescription.Text = dr["description"].ToString();
                    txtProgress.Text = dr["progress"].ToString();
                    lblModalTitle.Text = "Edit Goal";
                }
            }
        }

        protected void btnSaveGoal_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            int progress = 0;
            
            if (!int.TryParse(txtProgress.Text, out progress))
                progress = 0;

            if (progress < 0) progress = 0;
            if (progress > 100) progress = 100;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
            {
                lblMessage.Text = "Please fill in all required fields.";
                lblMessage.Visible = true;
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    if (string.IsNullOrEmpty(hdnGoalId.Value))
                    {
                        // Create new goal
                        string query = @"INSERT INTO Goals (user_id, title, description, status, progress, created_at) 
                                       VALUES (@uid, @title, @desc, 'Pending', @progress, GETDATE())";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@desc", description);
                        cmd.Parameters.AddWithValue("@progress", progress);
                        cmd.ExecuteNonQuery();

                        // Create notification for manager
                        CreateNotificationForManager(userId, title);
                    }
                    else
                    {
                        // Update existing goal
                        int goalId = Convert.ToInt32(hdnGoalId.Value);
                        string query = @"UPDATE Goals 
                                       SET title = @title, description = @desc, progress = @progress 
                                       WHERE goal_id = @gid AND user_id = @uid";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@desc", description);
                        cmd.Parameters.AddWithValue("@progress", progress);
                        cmd.Parameters.AddWithValue("@gid", goalId);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.ExecuteNonQuery();
                    }

                    // Clear form and reload
                    txtTitle.Text = "";
                    txtDescription.Text = "";
                    txtProgress.Text = "0";
                    hdnGoalId.Value = "";
                    lblModalTitle.Text = "Create New Goal";
                    LoadGoals();
                    
                    // Close modal using JavaScript
                    ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", 
                        "var modal = bootstrap.Modal.getInstance(document.getElementById('goalModal')); if(modal) modal.hide();", true);
                }
            }
            catch (Exception)
            {
                lblMessage.Text = "An error occurred. Please try again.";
                lblMessage.Visible = true;
            }
        }

        private void CreateNotificationForManager(int employeeId, string goalTitle)
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                
                // Get manager ID
                string getManagerQuery = "SELECT manager_id FROM Users WHERE user_id = @uid";
                SqlCommand getManagerCmd = new SqlCommand(getManagerQuery, conn);
                getManagerCmd.Parameters.AddWithValue("@uid", employeeId);
                object managerIdObj = getManagerCmd.ExecuteScalar();
                
                if (managerIdObj != null && managerIdObj != DBNull.Value)
                {
                    int managerId = Convert.ToInt32(managerIdObj);
                    string employeeName = Session["FullName"] != null ? Session["FullName"].ToString() : "An employee";
                    
                    string notificationQuery = @"INSERT INTO Notifications (user_id, message, status, created_at) 
                                                VALUES (@mid, @msg, 'unread', GETDATE())";
                    SqlCommand notifCmd = new SqlCommand(notificationQuery, conn);
                    notifCmd.Parameters.AddWithValue("@mid", managerId);
                    notifCmd.Parameters.AddWithValue("@msg", $"{employeeName} has created a new goal: {goalTitle}. Please review and approve.");
                    notifCmd.ExecuteNonQuery();
                }
            }
        }

        protected void rptGoals_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "UpdateGoal")
            {
                int goalId = Convert.ToInt32(e.CommandArgument);
                LoadGoalForEdit(goalId);
                ScriptManager.RegisterStartupScript(this, GetType(), "openModal", 
                    "var modal = new bootstrap.Modal(document.getElementById('goalModal')); modal.show();", true);
            }
            else if (e.CommandName == "DeleteGoal")
            {
                int goalId = Convert.ToInt32(e.CommandArgument);
                DeleteGoal(goalId);
            }
        }

        private void DeleteGoal(int goalId)
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "DELETE FROM Goals WHERE goal_id = @gid AND user_id = @uid";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@gid", goalId);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();
            }

            LoadGoals();
        }

        protected string GetStatusBadgeColor(string status)
        {
            switch (status.ToLower())
            {
                case "completed":
                    return "success";
                case "approved":
                    return "primary";
                case "in progress":
                case "inprogress":
                    return "info";
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
                return dt.ToString("MMM dd, yyyy");
            }
            catch
            {
                return "N/A";
            }
        }

        protected string GetManagerComment(object comment)
        {
            if (comment == null || comment == DBNull.Value || string.IsNullOrEmpty(comment.ToString()))
                return "";

            return $"<div class='alert alert-info mt-2'><strong>Manager Comment:</strong> {comment}</div>";
        }
    }
}

