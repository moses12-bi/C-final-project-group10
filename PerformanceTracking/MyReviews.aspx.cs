using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class MyReviews : System.Web.UI.Page
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
                LoadReviews();
            }
        }

        private void LoadReviews()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT pr.review_id, pr.rating, pr.comments, pr.date_reviewed, 
                                rc.cycle_name, m.full_name as manager_name
                                FROM Performance_Reviews pr
                                INNER JOIN Review_Cycles rc ON pr.cycle_id = rc.cycle_id
                                LEFT JOIN Users m ON pr.manager_id = m.user_id
                                WHERE pr.employee_id = @uid
                                ORDER BY pr.date_reviewed DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptReviews.DataSource = dt;
                rptReviews.DataBind();
                lblNoReviews.Visible = (dt.Rows.Count == 0);
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
    }
}

