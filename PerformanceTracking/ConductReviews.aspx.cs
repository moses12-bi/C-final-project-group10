using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace PTMS
{
    public partial class ConductReviews : System.Web.UI.Page
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
                LoadPendingReviews();
            }
        }

        private void LoadPendingReviews()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int managerId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT pr.review_id, pr.employee_id, u.full_name, u.department, rc.cycle_name
                                FROM Performance_Reviews pr
                                INNER JOIN Users u ON pr.employee_id = u.user_id
                                INNER JOIN Review_Cycles rc ON pr.cycle_id = rc.cycle_id
                                WHERE pr.manager_id = @mid AND pr.rating IS NULL AND rc.status = 'Active'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mid", managerId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptEmployees.DataSource = dt;
                rptEmployees.DataBind();
                lblNoReviews.Visible = (dt.Rows.Count == 0);
            }
        }

        protected void rptEmployees_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SubmitReview")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int reviewId = Convert.ToInt32(args[0]);
                int employeeId = Convert.ToInt32(args[1]);

                DropDownList ddlRating = (DropDownList)e.Item.FindControl("ddlRating");
                TextBox txtComments = (TextBox)e.Item.FindControl("txtComments");

                if (string.IsNullOrEmpty(ddlRating.SelectedValue) || string.IsNullOrEmpty(txtComments.Text.Trim()))
                {
                    lblMessage.Text = "Please fill in rating and comments.";
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Visible = true;
                    return;
                }

                int rating = Convert.ToInt32(ddlRating.SelectedValue);
                string comments = txtComments.Text.Trim();

                string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = @"UPDATE Performance_Reviews 
                                    SET rating = @rating, comments = @comments, date_reviewed = GETDATE() 
                                    WHERE review_id = @rid";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@rating", rating);
                    cmd.Parameters.AddWithValue("@comments", comments);
                    cmd.Parameters.AddWithValue("@rid", reviewId);
                    cmd.ExecuteNonQuery();

                    // Create notification for employee
                    string notifQuery = @"INSERT INTO Notifications (user_id, message, status, created_at) 
                                         VALUES (@uid, @msg, 'unread', GETDATE())";
                    SqlCommand notifCmd = new SqlCommand(notifQuery, conn);
                    notifCmd.Parameters.AddWithValue("@uid", employeeId);
                    notifCmd.Parameters.AddWithValue("@msg", "Your performance review has been completed. Please check your reviews.");
                    notifCmd.ExecuteNonQuery();

                    lblMessage.Text = "Review submitted successfully!";
                    lblMessage.CssClass = "alert alert-success";
                    lblMessage.Visible = true;
                    LoadPendingReviews();
                }
            }
        }
    }
}

