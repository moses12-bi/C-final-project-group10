using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace PTMS
{
    public partial class TrackOrgGoals : System.Web.UI.Page
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
                LoadOrgGoalsTracking();
            }
        }

        private void LoadOrgGoalsTracking()
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                
                // Get distinct organizational goal titles first
                string distinctQuery = @"SELECT DISTINCT title, description
                                        FROM Goals
                                        WHERE title LIKE '[Org Goal]%'
                                        ORDER BY created_at DESC";
                
                SqlCommand distinctCmd = new SqlCommand(distinctQuery, conn);
                SqlDataReader distinctReader = distinctCmd.ExecuteReader();
                
                DataTable dt = new DataTable();
                dt.Columns.Add("title", typeof(string));
                dt.Columns.Add("description", typeof(string));
                dt.Columns.Add("total_employees", typeof(int));
                dt.Columns.Add("completed_count", typeof(int));
                dt.Columns.Add("in_progress_count", typeof(int));
                dt.Columns.Add("not_started_count", typeof(int));
                dt.Columns.Add("avg_progress", typeof(double));
                
                while (distinctReader.Read())
                {
                    string fullTitle = distinctReader["title"].ToString();
                    string cleanTitle = fullTitle.Replace("[Org Goal] ", "");
                    string description = distinctReader["description"].ToString();
                    
                    // Get statistics for this specific goal
                    string statsQuery = @"SELECT 
                                        COUNT(DISTINCT user_id) as total_employees,
                                        SUM(CASE WHEN status = 'Completed' THEN 1 ELSE 0 END) as completed_count,
                                        SUM(CASE WHEN status IN ('Approved', 'In Progress', 'InProgress') THEN 1 ELSE 0 END) as in_progress_count,
                                        SUM(CASE WHEN status = 'Pending' THEN 1 ELSE 0 END) as not_started_count,
                                        AVG(CAST(progress AS FLOAT)) as avg_progress
                                        FROM Goals
                                        WHERE title = @title";
                    
                    SqlCommand statsCmd = new SqlCommand(statsQuery, conn);
                    statsCmd.Parameters.AddWithValue("@title", fullTitle);
                    
                    SqlDataReader statsReader = statsCmd.ExecuteReader();
                    if (statsReader.Read())
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["title"] = cleanTitle;
                        newRow["description"] = description;
                        newRow["total_employees"] = statsReader["total_employees"] != DBNull.Value ? Convert.ToInt32(statsReader["total_employees"]) : 0;
                        newRow["completed_count"] = statsReader["completed_count"] != DBNull.Value ? Convert.ToInt32(statsReader["completed_count"]) : 0;
                        newRow["in_progress_count"] = statsReader["in_progress_count"] != DBNull.Value ? Convert.ToInt32(statsReader["in_progress_count"]) : 0;
                        newRow["not_started_count"] = statsReader["not_started_count"] != DBNull.Value ? Convert.ToInt32(statsReader["not_started_count"]) : 0;
                        
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

                rptOrgGoals.DataSource = dt;
                rptOrgGoals.DataBind();
                lblNoOrgGoals.Visible = (dt.Rows.Count == 0);
            }
        }

        protected void rptOrgGoals_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item || 
                e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                string goalTitle = row["title"].ToString();
                PlaceHolder phEmployeeProgress = (PlaceHolder)e.Item.FindControl("phEmployeeProgress");
                
                if (phEmployeeProgress != null)
                {
                    DataTable employeeProgress = GetEmployeeProgress(goalTitle);
                    
                    if (employeeProgress.Rows.Count > 0)
                    {
                        System.Text.StringBuilder html = new System.Text.StringBuilder();
                        html.Append("<table class='table table-striped table-hover'>");
                        html.Append("<thead><tr><th>Employee</th><th>Department</th><th>Progress %</th><th>Status</th><th>Progress Bar</th></tr></thead>");
                        html.Append("<tbody>");
                        
                        foreach (DataRow empRow in employeeProgress.Rows)
                        {
                            string fullName = empRow["full_name"].ToString();
                            string department = empRow["department"].ToString();
                            int progress = Convert.ToInt32(empRow["progress"]);
                            string status = empRow["status"].ToString();
                            
                            html.Append("<tr>");
                            html.Append($"<td>{fullName}</td>");
                            html.Append($"<td>{department}</td>");
                            html.Append($"<td>{progress}%</td>");
                            html.Append($"<td><span class='badge bg-{GetStatusColor(status)}'>{status}</span></td>");
                            html.Append($"<td><div class='progress' style='height: 20px; width: 150px;'>");
                            html.Append($"<div class='progress-bar bg-{GetProgressColor(progress)}' style='width: {progress}%'></div>");
                            html.Append("</div></td>");
                            html.Append("</tr>");
                        }
                        
                        html.Append("</tbody></table>");
                        phEmployeeProgress.Controls.Add(new System.Web.UI.LiteralControl(html.ToString()));
                    }
                    else
                    {
                        phEmployeeProgress.Controls.Add(new System.Web.UI.LiteralControl("<p class='text-muted'>No employee data available.</p>"));
                    }
                }
            }
        }

        protected DataTable GetEmployeeProgress(string goalTitle)
        {
            string connString = ConfigurationManager.ConnectionStrings["PTMS_DB"].ConnectionString;
            string fullTitle = $"[Org Goal] {goalTitle}";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT u.full_name, u.department, g.progress, g.status
                                FROM Goals g
                                INNER JOIN Users u ON g.user_id = u.user_id
                                WHERE g.title = @title
                                ORDER BY u.department, u.full_name";
                
                SqlCommand cmd = new SqlCommand(query, conn);
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

        protected string GetProgressColor(int progress)
        {
            if (progress >= 80)
                return "success";
            else if (progress >= 50)
                return "info";
            else if (progress >= 25)
                return "warning";
            else
                return "danger";
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

        protected string GetProgressColor(int progress)
        {
            if (progress >= 80)
                return "success";
            else if (progress >= 50)
                return "info";
            else if (progress >= 25)
                return "warning";
            else
                return "danger";
        }
    }
}

