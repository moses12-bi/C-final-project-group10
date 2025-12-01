using System;
using System.Configuration;
using System.Data.SqlClient;
using PTMS.Utilities;

namespace PTMS
{
    public partial class SignUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect if already logged in
            if (Session["UserID"] != null)
            {
                string role = Session["Role"].ToString();
                if (role == "Employee")
                    Response.Redirect("EmployeeDashboard.aspx");
                else if (role == "Manager")
                    Response.Redirect("ManagerDashboard.aspx");
                else if (role == "HR")
                    Response.Redirect("HRDashboard.aspx");
            }
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = ddlRole.SelectedValue;
            string department = txtDepartment.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role) || 
                string.IsNullOrEmpty(department))
            {
                ShowMessage("Please fill in all fields.", true);
                return;
            }

            if (password.Length < 6)
            {
                ShowMessage("Password must be at least 6 characters long.", true);
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Check if email already exists
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE email = @Email";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Email", email);

                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        ShowMessage("Email already exists! Please use a different email or sign in.", true);
                        return;
                    }

                    // Hash password
                    string hashedPassword = PasswordHelper.HashPassword(password);

                    // Insert new user
                    string sql = @"INSERT INTO Users (full_name, email, password, role, department)
                                   VALUES (@FullName, @Email, @Password, @Role, @Department)";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@Department", department);

                    cmd.ExecuteNonQuery();

                    ShowMessage("Account created successfully! Redirecting to login...", false);
                    
                    // Redirect after 2 seconds
                    Response.AddHeader("REFRESH", "2;URL=Login.aspx");
                }
            }
            catch (Exception)
            {
                ShowMessage("An error occurred while creating your account. Please try again.", true);
                // Log error in production
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = isError ? "alert alert-danger" : "alert alert-success";
            lblMessage.Visible = true;
        }
    }
}
