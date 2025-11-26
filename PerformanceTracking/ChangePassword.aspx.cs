using System;
using System.Data.SqlClient;
using System.Configuration;
using PTMS.Utilities;

namespace PTMS
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            string currentPassword = txtCurrentPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ShowMessage("Please fill in all fields.", true);
                return;
            }

            if (newPassword.Length < 6)
            {
                ShowMessage("New password must be at least 6 characters long.", true);
                return;
            }

            if (newPassword != confirmPassword)
            {
                ShowMessage("New password and confirm password do not match.", true);
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Get current password from database
                    string getPasswordQuery = "SELECT password FROM Users WHERE user_id = @uid";
                    SqlCommand getPasswordCmd = new SqlCommand(getPasswordQuery, conn);
                    getPasswordCmd.Parameters.AddWithValue("@uid", userId);
                    object passwordObj = getPasswordCmd.ExecuteScalar();

                    if (passwordObj == null)
                    {
                        ShowMessage("User not found.", true);
                        return;
                    }

                    string dbPassword = passwordObj.ToString();

                    // Verify current password
                    bool passwordValid = false;
                    if (dbPassword.Length > 50) // Likely hashed
                    {
                        passwordValid = PasswordHelper.VerifyPassword(currentPassword, dbPassword);
                    }
                    else
                    {
                        // Plain text comparison for existing records
                        passwordValid = (currentPassword == dbPassword);
                    }

                    if (!passwordValid)
                    {
                        ShowMessage("Current password is incorrect.", true);
                        return;
                    }

                    // Hash new password
                    string hashedNewPassword = PasswordHelper.HashPassword(newPassword);

                    // Update password
                    string updateQuery = "UPDATE Users SET password = @password WHERE user_id = @uid";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@password", hashedNewPassword);
                    updateCmd.Parameters.AddWithValue("@uid", userId);
                    updateCmd.ExecuteNonQuery();

                    ShowMessage("Password changed successfully!", false);
                    
                    // Clear form
                    txtCurrentPassword.Text = "";
                    txtNewPassword.Text = "";
                    txtConfirmPassword.Text = "";
                }
            }
            catch (Exception)
            {
                ShowMessage("An error occurred. Please try again.", true);
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

