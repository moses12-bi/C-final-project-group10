using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace PTMS
{
    public partial class GoalApprovals : System.Web.UI.Page
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
                LoadPendingGoals();
                
                // Handle quick approve/reject from dashboard
                if (Request.QueryString["goalId"] != null && Request.QueryString["action"] != null)
                {
                    int goalId = Convert.ToInt32(Request.QueryString["goalId"]);
                    string action = Request.QueryString["action"];
                    
                    if (action == "approve")
                        ApproveGoal(goalId, "");
                    else if (action == "reject")
                        RejectGoal(goalId, "");
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
                string query = @"SELECT g.goal_id, g.title, g.description, g.progress, u.full_name 
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
                lblNoApprovals.Visible = (dt.Rows.Count == 0);
            }
        }

        protected void rptPendingGoals_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int goalId = Convert.ToInt32(e.CommandArgument);
            TextBox txtComment = (TextBox)e.Item.FindControl("txtComment");
            string comment = txtComment != null ? txtComment.Text.Trim() : "";

            if (e.CommandName == "Approve")
            {
                ApproveGoal(goalId, comment);
            }
            else if (e.CommandName == "Reject")
            {
                RejectGoal(goalId, comment);
            }
        }

        private void ApproveGoal(int goalId, string comment)
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int managerId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                
                // Verify manager has authority
                string verifyQuery = @"SELECT g.goal_id FROM Goals g 
                                      INNER JOIN Users u ON g.user_id = u.user_id 
                                      WHERE g.goal_id = @gid AND u.manager_id = @mid";
                SqlCommand verifyCmd = new SqlCommand(verifyQuery, conn);
                verifyCmd.Parameters.AddWithValue("@gid", goalId);
                verifyCmd.Parameters.AddWithValue("@mid", managerId);
                
                if (verifyCmd.ExecuteScalar() == null)
                {
                    lblMessage.Text = "You don't have permission to approve this goal.";
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Visible = true;
                    return;
                }

                // Update goal status
                string updateQuery = @"UPDATE Goals 
                                      SET status = 'Approved', manager_comment = @comment 
                                      WHERE goal_id = @gid";
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@gid", goalId);
                updateCmd.Parameters.AddWithValue("@comment", string.IsNullOrEmpty(comment) ? DBNull.Value : (object)comment);
                updateCmd.ExecuteNonQuery();

                // Get employee ID and create notification
                string getEmployeeQuery = "SELECT user_id FROM Goals WHERE goal_id = @gid";
                SqlCommand getEmpCmd = new SqlCommand(getEmployeeQuery, conn);
                getEmpCmd.Parameters.AddWithValue("@gid", goalId);
                object empIdObj = getEmpCmd.ExecuteScalar();
                
                if (empIdObj != null)
                {
                    int employeeId = Convert.ToInt32(empIdObj);
                    CreateNotification(employeeId, $"Your goal has been approved by your manager.");
                }

                lblMessage.Text = "Goal approved successfully!";
                lblMessage.CssClass = "alert alert-success";
                lblMessage.Visible = true;
                LoadPendingGoals();
            }
        }

        private void RejectGoal(int goalId, string comment)
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int managerId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                
                // Verify manager has authority
                string verifyQuery = @"SELECT g.goal_id FROM Goals g 
                                      INNER JOIN Users u ON g.user_id = u.user_id 
                                      WHERE g.goal_id = @gid AND u.manager_id = @mid";
                SqlCommand verifyCmd = new SqlCommand(verifyQuery, conn);
                verifyCmd.Parameters.AddWithValue("@gid", goalId);
                verifyCmd.Parameters.AddWithValue("@mid", managerId);
                
                if (verifyCmd.ExecuteScalar() == null)
                {
                    lblMessage.Text = "You don't have permission to reject this goal.";
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Visible = true;
                    return;
                }

                // Update goal status
                string updateQuery = @"UPDATE Goals 
                                      SET status = 'Rejected', manager_comment = @comment 
                                      WHERE goal_id = @gid";
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@gid", goalId);
                updateCmd.Parameters.AddWithValue("@comment", string.IsNullOrEmpty(comment) ? DBNull.Value : (object)comment);
                updateCmd.ExecuteNonQuery();

                // Get employee ID and create notification
                string getEmployeeQuery = "SELECT user_id FROM Goals WHERE goal_id = @gid";
                SqlCommand getEmpCmd = new SqlCommand(getEmployeeQuery, conn);
                getEmpCmd.Parameters.AddWithValue("@gid", goalId);
                object empIdObj = getEmpCmd.ExecuteScalar();
                
                if (empIdObj != null)
                {
                    int employeeId = Convert.ToInt32(empIdObj);
                    CreateNotification(employeeId, $"Your goal has been rejected. Please review manager's comment.");
                }

                lblMessage.Text = "Goal rejected.";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                LoadPendingGoals();
            }
        }

        private void CreateNotification(int userId, string message)
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"INSERT INTO Notifications (user_id, message, status, created_at) 
                                VALUES (@uid, @msg, 'unread', GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@msg", message);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

