using System;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class Profile : System.Web.UI.Page
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
                LoadProfile();
            }
        }

        private void LoadProfile()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT full_name, email, role, department FROM Users WHERE user_id = @uid";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtFullName.Text = dr["full_name"].ToString();
                    txtEmail.Text = dr["email"].ToString();
                    txtRole.Text = dr["role"].ToString();
                    txtDepartment.Text = dr["department"].ToString();
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string department = txtDepartment.Text.Trim();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(department))
            {
                lblMessage.Text = "Please fill in all fields.";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "UPDATE Users SET full_name = @name, department = @dept WHERE user_id = @uid";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", fullName);
                cmd.Parameters.AddWithValue("@dept", department);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();

                Session["FullName"] = fullName;
                lblMessage.Text = "Profile updated successfully!";
                lblMessage.CssClass = "alert alert-success";
                lblMessage.Visible = true;
            }
        }
    }
}

