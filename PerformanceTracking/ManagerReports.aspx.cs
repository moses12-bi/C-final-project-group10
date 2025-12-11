using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PTMS
{
    public partial class ManagerReports : System.Web.UI.Page
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
                LoadReports();
            }
        }

        private void LoadReports()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int managerId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Team Size
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Users WHERE manager_id = @mid", conn);
                cmd1.Parameters.AddWithValue("@mid", managerId);
                object result1 = cmd1.ExecuteScalar();
                lblTeamSize.Text = result1 != null ? result1.ToString() : "0";

                // Total Goals
                SqlCommand cmd2 = new SqlCommand(@"SELECT COUNT(*) FROM Goals g 
                                                   INNER JOIN Users u ON g.user_id = u.user_id 
                                                   WHERE u.manager_id = @mid", conn);
                cmd2.Parameters.AddWithValue("@mid", managerId);
                object result2 = cmd2.ExecuteScalar();
                lblTotalGoals.Text = result2 != null ? result2.ToString() : "0";

                // Average Rating
                SqlCommand cmd3 = new SqlCommand(@"SELECT AVG(CAST(pr.rating AS FLOAT)) 
                                                   FROM Performance_Reviews pr 
                                                   INNER JOIN Users u ON pr.employee_id = u.user_id 
                                                   WHERE u.manager_id = @mid AND pr.rating IS NOT NULL", conn);
                cmd3.Parameters.AddWithValue("@mid", managerId);
                object result3 = cmd3.ExecuteScalar();
                if (result3 != null && result3 != DBNull.Value)
                {
                    double avg = Convert.ToDouble(result3);
                    lblAvgRating.Text = avg.ToString("F1");
                }
                else
                {
                    lblAvgRating.Text = "N/A";
                }

                // Team Summary
                string query = @"SELECT u.full_name,
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

                gvTeamSummary.DataSource = dt;
                gvTeamSummary.DataBind();
                
                // Load Organizational Goals for Team
                LoadTeamOrgGoals(managerId, conn);
            }
        }

        private void LoadTeamOrgGoals(int managerId, SqlConnection conn)
        {
            try
            {
                // Get all unique organizational goals that team members have
                // Use a simpler approach - get distinct titles first
                string distinctQuery = @"SELECT DISTINCT g.title, g.description
                                        FROM Goals g
                                        INNER JOIN Users u ON g.user_id = u.user_id
                                        WHERE u.manager_id = @mid AND g.title LIKE '[Org Goal]%'
                                        ORDER BY g.created_at DESC";
                
                SqlCommand distinctCmd = new SqlCommand(distinctQuery, conn);
                distinctCmd.Parameters.AddWithValue("@mid", managerId);
                
                SqlDataReader distinctReader = distinctCmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Columns.Add("title", typeof(string));
                dt.Columns.Add("description", typeof(string));
                dt.Columns.Add("team_count", typeof(int));
                dt.Columns.Add("completed_count", typeof(int));
                dt.Columns.Add("in_progress_count", typeof(int));
                dt.Columns.Add("avg_progress", typeof(double));
                
                while (distinctReader.Read())
                {
                    string fullTitle = distinctReader["title"].ToString();
                    string cleanTitle = fullTitle.Replace("[Org Goal] ", "");
                    string description = distinctReader["description"].ToString();
                    
                    // Get statistics for this specific goal
                    string statsQuery = @"SELECT 
                                        COUNT(DISTINCT g.user_id) as team_count,
                                        SUM(CASE WHEN g.status = 'Completed' THEN 1 ELSE 0 END) as completed_count,
                                        SUM(CASE WHEN g.status IN ('Approved', 'In Progress', 'InProgress') THEN 1 ELSE 0 END) as in_progress_count,
                                        AVG(CAST(g.progress AS FLOAT)) as avg_progress
                                        FROM Goals g
                                        INNER JOIN Users u ON g.user_id = u.user_id
                                        WHERE u.manager_id = @mid AND g.title = @title";
                    
                    SqlCommand statsCmd = new SqlCommand(statsQuery, conn);
                    statsCmd.Parameters.AddWithValue("@mid", managerId);
                    statsCmd.Parameters.AddWithValue("@title", fullTitle);
                    
                    SqlDataReader statsReader = statsCmd.ExecuteReader();
                    if (statsReader.Read())
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["title"] = cleanTitle;
                        newRow["description"] = description;
                        newRow["team_count"] = statsReader["team_count"] != DBNull.Value ? Convert.ToInt32(statsReader["team_count"]) : 0;
                        newRow["completed_count"] = statsReader["completed_count"] != DBNull.Value ? Convert.ToInt32(statsReader["completed_count"]) : 0;
                        newRow["in_progress_count"] = statsReader["in_progress_count"] != DBNull.Value ? Convert.ToInt32(statsReader["in_progress_count"]) : 0;
                        
                        if (statsReader["avg_progress"] != DBNull.Value)
                        {
                            newRow["avg_progress"] = Math.Round(Convert.ToDouble(statsReader["avg_progress"]), 1);
                        }
                        else
                        {
                            newRow["avg_progress"] = 0;
                        }
                        
                        dt.Rows.Add(newRow);
                    }
                    statsReader.Close();
                }
                distinctReader.Close();

                rptTeamOrgGoals.DataSource = dt;
                rptTeamOrgGoals.DataBind();
                lblNoOrgGoals.Visible = (dt.Rows.Count == 0);
            }
            catch (Exception ex)
            {
                // If error occurs, just hide the section
                lblNoOrgGoals.Visible = true;
                lblNoOrgGoals.Text = "Unable to load organizational goals. Please try again later.";
            }
        }

        protected string GetTeamMemberProgressHTML(string goalTitle)
        {
            DataTable teamProgress = GetTeamMemberProgress(goalTitle);
            
            if (teamProgress.Rows.Count > 0)
            {
                System.Text.StringBuilder html = new System.Text.StringBuilder();
                html.Append("<table class='table table-sm table-bordered'>");
                html.Append("<thead><tr><th>Team Member</th><th>Progress</th><th>Status</th></tr></thead>");
                html.Append("<tbody>");
                
                foreach (DataRow memberRow in teamProgress.Rows)
                {
                    string fullName = memberRow["full_name"].ToString();
                    int progress = Convert.ToInt32(memberRow["progress"]);
                    string status = memberRow["status"].ToString();
                    
                    html.Append("<tr>");
                    html.Append($"<td>{fullName}</td>");
                    html.Append($"<td>{progress}%</td>");
                    html.Append($"<td><span class='badge bg-{GetStatusColor(status)}'>{status}</span></td>");
                    html.Append("</tr>");
                }
                
                html.Append("</tbody></table>");
                return html.ToString();
            }
            else
            {
                return "<p class='text-muted small'>No team member data available.</p>";
            }
        }

        protected DataTable GetTeamMemberProgress(string goalTitle)
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            int managerId = Convert.ToInt32(Session["UserID"]);
            string fullTitle = $"[Org Goal] {goalTitle}";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT u.full_name, g.progress, g.status
                                FROM Goals g
                                INNER JOIN Users u ON g.user_id = u.user_id
                                WHERE u.manager_id = @mid AND g.title = @title
                                ORDER BY u.full_name";
                
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mid", managerId);
                cmd.Parameters.AddWithValue("@title", fullTitle);
                
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                
                return dt;
            }
        }

        protected string GetStatusColor(string status)
        {
            switch (status.ToLower())
            {
                case "completed":
                    return "success";
                case "approved":
                case "in progress":
                case "inprogress":
                    return "info";
                case "pending":
                    return "warning";
                case "rejected":
                    return "danger";
                default:
                    return "secondary";
            }
        }
    }
}

