using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class EditUser : System.Web.UI.Page
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
                if (Request.QueryString["id"] != null)
                {
                    int userId = Convert.ToInt32(Request.QueryString["id"]);
                    LoadUser(userId);
                    LoadManagers();
                }
                else
                {
                    Response.Redirect("ManageUsers.aspx");
                }
            }
        }

        private void LoadUser(int userId)
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT user_id, full_name, email, role, department, manager_id FROM Users WHERE user_id = @uid";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    hdnUserId.Value = dr["user_id"].ToString();
                    txtFullName.Text = dr["full_name"].ToString();
                    txtEmail.Text = dr["email"].ToString();
                    ddlRole.SelectedValue = dr["role"].ToString();
                    txtDepartment.Text = dr["department"].ToString();
                    
                    if (dr["manager_id"] != DBNull.Value)
                    {
                        ddlManager.SelectedValue = dr["manager_id"].ToString();
                    }
                }
                else
                {
                    Response.Redirect("ManageUsers.aspx");
                }
            }
        }

        private void LoadManagers()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT user_id, full_name FROM Users WHERE role IN ('Manager', 'HR') ORDER BY full_name";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlManager.DataSource = dt;
                ddlManager.DataTextField = "full_name";
                ddlManager.DataValueField = "user_id";
                ddlManager.DataBind();
                ddlManager.Items.Insert(0, new System.Web.UI.WebControls.ListItem("None", ""));
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string role = ddlRole.SelectedValue;
            string department = txtDepartment.Text.Trim();
            string managerIdStr = ddlManager.SelectedValue;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(department))
            {
                ShowMessage("Please fill in all required fields.", true);
                return;
            }

            int userId = Convert.ToInt32(hdnUserId.Value);
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"UPDATE Users 
                                SET full_name = @name, role = @role, department = @dept, 
                                    manager_id = CASE WHEN @mid = '' THEN NULL ELSE CAST(@mid AS INT) END
                                WHERE user_id = @uid";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", fullName);
                cmd.Parameters.AddWithValue("@role", role);
                cmd.Parameters.AddWithValue("@dept", department);
                cmd.Parameters.AddWithValue("@mid", string.IsNullOrEmpty(managerIdStr) ? DBNull.Value : (object)managerIdStr);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();

                ShowMessage("User updated successfully!", false);
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

