using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class TeamPerformance : System.Web.UI.Page
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
                LoadTeamPerformance();
            }
        }

        private void LoadTeamPerformance()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int managerId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT u.user_id, u.full_name, u.department,
                                (SELECT COUNT(*) FROM Goals WHERE user_id = u.user_id) as total_goals,
                                (SELECT COUNT(*) FROM Goals WHERE user_id = u.user_id AND status = 'Completed') as completed_goals,
                                (SELECT AVG(CAST(rating AS FLOAT)) FROM Performance_Reviews WHERE employee_id = u.user_id AND rating IS NOT NULL) as avg_rating
                                FROM Users u
                                WHERE u.manager_id = @mid
                                ORDER BY u.full_name";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mid", managerId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptTeamMembers.DataSource = dt;
                rptTeamMembers.DataBind();
                lblNoTeamMembers.Visible = (dt.Rows.Count == 0);
            }
        }
    }
}

