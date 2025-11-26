using System;
using System.Data.SqlClient;
using System.Configuration;
using PTMS.Utilities;

namespace PTMS
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect if already logged in
            if (Session["UserID"] != null)
            {
                string role = Session["Role"].ToString();
                RedirectToDashboard(role);
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowMessage("Please enter both email and password.", true);
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = "SELECT user_id, full_name, role, password FROM Users WHERE email = @Email";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        string dbPassword = dr["password"].ToString();
                        string role = dr["role"].ToString();
                        int userId = Convert.ToInt32(dr["user_id"]);
                        string fullName = dr["full_name"].ToString();

                        // Verify password (supports both hashed and plain text for migration)
                        bool passwordValid = false;
                        if (dbPassword.Length > 50) // Likely hashed
                        {
                            passwordValid = PasswordHelper.VerifyPassword(password, dbPassword);
                        }
                        else
                        {
                            // Plain text comparison for existing records
                            passwordValid = (password == dbPassword);
                        }

                        if (passwordValid)
                        {
                            Session["UserID"] = userId;
                            Session["Role"] = role;
                            Session["FullName"] = fullName;
                            Session["Email"] = email;

                            RedirectToDashboard(role);
                        }
                        else
                        {
                            ShowMessage("Incorrect password.", true);
                        }
                    }
                    else
                    {
                        ShowMessage("Email not found. Please check your email or sign up.", true);
                    }
                }
            }
            catch (Exception)
            {
                ShowMessage("An error occurred. Please try again later.", true);
                // Log error in production
            }
        }

        private void RedirectToDashboard(string role)
        {
            switch (role.ToLower())
            {
                case "employee":
                    Response.Redirect("EmployeeDashboard.aspx");
                    break;
                case "manager":
                    Response.Redirect("ManagerDashboard.aspx");
                    break;
                case "hr":
                case "admin":
                    Response.Redirect("HRDashboard.aspx");
                    break;
                default:
                    ShowMessage("Invalid user role.", true);
                    break;
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
