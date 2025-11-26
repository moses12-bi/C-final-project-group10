using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class Analytics : System.Web.UI.Page
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
                LoadAnalytics();
            }
        }

        private void LoadAnalytics()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Total Employees
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Users WHERE role = 'Employee'", conn);
                object result1 = cmd1.ExecuteScalar();
                lblTotalEmployees.Text = result1 != null ? result1.ToString() : "0";

                // Active Goals
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Goals WHERE status IN ('Pending', 'Approved', 'In Progress')", conn);
                object result2 = cmd2.ExecuteScalar();
                lblActiveGoals.Text = result2 != null ? result2.ToString() : "0";

                // Average Performance
                SqlCommand cmd3 = new SqlCommand("SELECT AVG(CAST(rating AS FLOAT)) FROM Performance_Reviews WHERE rating IS NOT NULL", conn);
                object result3 = cmd3.ExecuteScalar();
                if (result3 != null && result3 != DBNull.Value)
                {
                    double avg = Convert.ToDouble(result3);
                    lblAvgPerformance.Text = avg.ToString("F1");
                }
                else
                {
                    lblAvgPerformance.Text = "N/A";
                }

                // Total Reviews
                SqlCommand cmd4 = new SqlCommand("SELECT COUNT(*) FROM Performance_Reviews WHERE rating IS NOT NULL", conn);
                object result4 = cmd4.ExecuteScalar();
                lblTotalReviews.Text = result4 != null ? result4.ToString() : "0";

                // Department Stats
                string query = @"SELECT department,
                                COUNT(*) as employee_count,
                                AVG(CAST(pr.rating AS FLOAT)) as avg_rating
                                FROM Users u
                                LEFT JOIN Performance_Reviews pr ON u.user_id = pr.employee_id
                                WHERE u.role = 'Employee'
                                GROUP BY department
                                ORDER BY department";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvDepartmentStats.DataSource = dt;
                gvDepartmentStats.DataBind();
            }
        }
    }
}

