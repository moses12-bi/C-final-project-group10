using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class ViewFeedback : System.Web.UI.Page
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
                LoadFeedback();
            }
        }

        private void LoadFeedback()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT f.feedback_id, f.feedback_type, f.comments, f.rating, f.date_given, u.full_name 
                                FROM Feedback f 
                                INNER JOIN Users u ON f.sender_id = u.user_id 
                                WHERE f.receiver_id = @uid 
                                ORDER BY f.date_given DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptFeedback.DataSource = dt;
                rptFeedback.DataBind();
                lblNoFeedback.Visible = (dt.Rows.Count == 0);
            }
        }

        protected string GetStars(int rating)
        {
            string stars = "";
            for (int i = 1; i <= 5; i++)
            {
                if (i <= rating)
                    stars += "<i class='fas fa-star'></i>";
                else
                    stars += "<i class='far fa-star'></i>";
            }
            return stars;
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
    }
}

