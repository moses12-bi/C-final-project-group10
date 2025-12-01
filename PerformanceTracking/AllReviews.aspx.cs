using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class AllReviews : System.Web.UI.Page
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
                LoadAllReviews();
            }
        }

        private void LoadAllReviews()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT u.full_name as employee_name, rc.cycle_name, pr.rating, pr.date_reviewed
                                FROM Performance_Reviews pr
                                INNER JOIN Users u ON pr.employee_id = u.user_id
                                INNER JOIN Review_Cycles rc ON pr.cycle_id = rc.cycle_id
                                WHERE pr.rating IS NOT NULL
                                ORDER BY pr.date_reviewed DESC";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvAllReviews.DataSource = dt;
                gvAllReviews.DataBind();
            }
        }
    }
}

