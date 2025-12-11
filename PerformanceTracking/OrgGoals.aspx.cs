using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Web.UI;

namespace PTMS
{
    public partial class OrgGoals : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string role = Session["Role"] != null ? Session["Role"].ToString().ToLower() : "";
            if (role != "hr" && role != "admin")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadOrgGoals();
            }
        }

        private void LoadOrgGoals()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                // Get unique organizational goals (those starting with [Org Goal])
                string query = @"SELECT DISTINCT 
                                REPLACE(title, '[Org Goal] ', '') as title, 
                                description,
                                MIN(created_at) as created_at
                                FROM Goals 
                                WHERE title LIKE '[Org Goal]%'
                                GROUP BY REPLACE(title, '[Org Goal] ', ''), description
                                ORDER BY MIN(created_at) DESC";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptOrgGoals.DataSource = dt;
                rptOrgGoals.DataBind();
                lblNoOrgGoals.Visible = (dt.Rows.Count == 0);
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
            {
                lblMessage.Text = "Please fill in all fields.";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int hrUserId = Convert.ToInt32(Session["UserID"]);
            string hrName = Session["FullName"] != null ? Session["FullName"].ToString() : "HR";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    
                    // Create organizational goal in Goals table with a special marker
                    // We'll use a special user_id (0 or negative) or add a flag. For now, let's use user_id = 0 for org goals
                    // Actually, better approach: create for all employees or use a separate approach
                    // Let's create it as a goal for all employees by inserting for each employee
                    
                    // Get all employees
                    string getEmployeesQuery = "SELECT user_id, manager_id FROM Users WHERE role = 'Employee'";
                    SqlCommand getEmployeesCmd = new SqlCommand(getEmployeesQuery, conn);
                    SqlDataReader employeesReader = getEmployeesCmd.ExecuteReader();
                    
                    List<int> employeeIds = new List<int>();
                    List<int> managerIds = new List<int>();
                    
                    while (employeesReader.Read())
                    {
                        int empId = Convert.ToInt32(employeesReader["user_id"]);
                        employeeIds.Add(empId);
                        
                        if (employeesReader["manager_id"] != DBNull.Value)
                        {
                            int mgrId = Convert.ToInt32(employeesReader["manager_id"]);
                            if (!managerIds.Contains(mgrId))
                                managerIds.Add(mgrId);
                        }
                    }
                    employeesReader.Close();
                    
                    // Create org goal for each employee
                    foreach (int empId in employeeIds)
                    {
                        string insertGoalQuery = @"INSERT INTO Goals (user_id, title, description, status, progress, created_at) 
                                                   VALUES (@uid, @title, @desc, 'Approved', 0, GETDATE())";
                        SqlCommand insertCmd = new SqlCommand(insertGoalQuery, conn);
                        insertCmd.Parameters.AddWithValue("@uid", empId);
                        insertCmd.Parameters.AddWithValue("@title", $"[Org Goal] {title}");
                        insertCmd.Parameters.AddWithValue("@desc", description);
                        insertCmd.ExecuteNonQuery();
                    }
                    
                    // Send notifications to all employees
                    string employeeNotificationMsg = $"HR has created a new organizational goal: {title}. Please review and align your goals accordingly.";
                    foreach (int empId in employeeIds)
                    {
                        string notifQuery = @"INSERT INTO Notifications (user_id, message, status, created_at) 
                                            VALUES (@uid, @msg, 'unread', GETDATE())";
                        SqlCommand notifCmd = new SqlCommand(notifQuery, conn);
                        notifCmd.Parameters.AddWithValue("@uid", empId);
                        notifCmd.Parameters.AddWithValue("@msg", employeeNotificationMsg);
                        notifCmd.ExecuteNonQuery();
                    }
                    
                    // Send notifications to all managers
                    string managerNotificationMsg = $"HR has created a new organizational goal: {title}. Please review with your team members.";
                    foreach (int mgrId in managerIds)
                    {
                        string notifQuery = @"INSERT INTO Notifications (user_id, message, status, created_at) 
                                            VALUES (@uid, @msg, 'unread', GETDATE())";
                        SqlCommand notifCmd = new SqlCommand(notifQuery, conn);
                        notifCmd.Parameters.AddWithValue("@uid", mgrId);
                        notifCmd.Parameters.AddWithValue("@msg", managerNotificationMsg);
                        notifCmd.ExecuteNonQuery();
                    }
                    
                    lblMessage.Text = $"Organization goal '{title}' created successfully! Notifications sent to all employees and managers.";
                    lblMessage.CssClass = "alert alert-success";
                    lblMessage.Visible = true;
                    
                    // Clear form
                    txtTitle.Text = "";
                    txtDescription.Text = "";
                    
                    // Close modal
                    ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", 
                        "var modal = bootstrap.Modal.getInstance(document.getElementById('goalModal')); if(modal) modal.hide();", true);
                    
                    LoadOrgGoals();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred while creating the organizational goal. Please try again.";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
            }
        }
    }
}

