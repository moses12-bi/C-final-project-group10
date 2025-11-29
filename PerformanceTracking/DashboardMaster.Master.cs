using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PTMS
{
    public partial class DashboardMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Set user info
            if (Session["FullName"] != null)
            {
                lblUserName.Text = Session["FullName"].ToString();
                lblTopUserName.Text = Session["FullName"].ToString();
            }

            if (Session["Role"] != null)
            {
                string role = Session["Role"].ToString();
                lblUserRole.Text = role;

                // Build menu based on role
                BuildMenu(role);
            }

            // Set page title
            if (Page.Title != null && !string.IsNullOrEmpty(Page.Title))
            {
                lblPageTitle.Text = Page.Title;
            }
        }

        private void BuildMenu(string role)
        {
            phMenuItems.Controls.Clear();

            // Common menu items
            AddMenuItem("Dashboard", GetDashboardUrl(role), "fas fa-home", true);

            // Role-specific menu items
            switch (role.ToLower())
            {
                case "employee":
                    AddMenuItem("My Goals", "Goals.aspx", "fas fa-bullseye", false);
                    AddMenuItem("Request Feedback", "RequestFeedback.aspx", "fas fa-comment-dots", false);
                    AddMenuItem("View Feedback", "ViewFeedback.aspx", "fas fa-comments", false);
                    AddMenuItem("360 Feedback", "Feedback360.aspx", "fas fa-users", false);
                    AddMenuItem("My Reviews", "MyReviews.aspx", "fas fa-clipboard-check", false);
                    break;

                case "manager":
                    AddMenuItem("Goal Approvals", "GoalApprovals.aspx", "fas fa-check-circle", false);
                    AddMenuItem("Give Feedback", "GiveFeedback.aspx", "fas fa-comment-alt", false);
                    AddMenuItem("Team Performance", "TeamPerformance.aspx", "fas fa-users", false);
                    AddMenuItem("Conduct Reviews", "ConductReviews.aspx", "fas fa-clipboard-list", false);
                    AddMenuItem("Reports", "ManagerReports.aspx", "fas fa-chart-bar", false);
                    break;

                case "hr":
                case "admin":
                    AddMenuItem("Manage Users", "ManageUsers.aspx", "fas fa-user-cog", false);
                    AddMenuItem("Review Cycles", "ReviewCycles.aspx", "fas fa-calendar-alt", false);
                    AddMenuItem("Organization Goals", "OrgGoals.aspx", "fas fa-building", false);
                    AddMenuItem("Analytics", "Analytics.aspx", "fas fa-chart-line", false);
                    AddMenuItem("All Reviews", "AllReviews.aspx", "fas fa-clipboard-list", false);
                    break;
            }

            // Common items for all
            AddMenuItem("Notifications", "Notifications.aspx", "fas fa-bell", false);
        }

        private void AddMenuItem(string text, string url, string icon, bool isActive)
        {
            HyperLink link = new HyperLink();
            link.Text = $"<i class=\"{icon}\"></i> {text}";
            link.NavigateUrl = url;
            link.CssClass = isActive ? "menu-item active" : "menu-item";
            phMenuItems.Controls.Add(link);
        }

        private string GetDashboardUrl(string role)
        {
            switch (role.ToLower())
            {
                case "employee":
                    return "EmployeeDashboard.aspx";
                case "manager":
                    return "ManagerDashboard.aspx";
                case "hr":
                case "admin":
                    return "HRDashboard.aspx";
                default:
                    return "Login.aspx";
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            // Clear all session variables
            Session.Clear();
            Session.Abandon();
            
            // Clear authentication cookie if exists
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }
            
            // Redirect to login page
            Response.Redirect("Login.aspx", true);
        }
    }
}

