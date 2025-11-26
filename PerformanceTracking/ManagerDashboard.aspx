<%@ Page Title="Manager Dashboard" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ManagerDashboard.aspx.cs" Inherits="PTMS.ManagerDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .stat-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            border-left: 4px solid;
            height: 100%;
        }

        .stat-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
        }

        .stat-card.primary { border-left-color: #667eea; }
        .stat-card.success { border-left-color: #28a745; }
        .stat-card.info { border-left-color: #17a2b8; }
        .stat-card.warning { border-left-color: #ffc107; }

        .stat-card .stat-icon {
            font-size: 2.5rem;
            margin-bottom: 15px;
            opacity: 0.8;
        }

        .stat-card .stat-value {
            font-size: 2.5rem;
            font-weight: 700;
            color: #333;
            margin: 10px 0;
        }

        .stat-card .stat-label {
            color: #666;
            font-size: 0.95rem;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .welcome-section {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            border-radius: 15px;
            margin-bottom: 30px;
            box-shadow: 0 10px 30px rgba(102, 126, 234, 0.3);
        }

        .section-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 25px;
        }

        .section-card h5 {
            color: #333;
            font-weight: 600;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #f0f0f0;
        }

        .quick-action-btn {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 10px;
            font-weight: 600;
            transition: all 0.3s ease;
            text-decoration: none;
            display: inline-block;
        }

        .quick-action-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 25px rgba(102, 126, 234, 0.4);
            color: white;
        }

        .team-member-card {
            background: #f8f9fa;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 10px;
            transition: all 0.3s ease;
        }

        .team-member-card:hover {
            background: #e9ecef;
            transform: translateX(5px);
        }
    </style>

    <div class="welcome-section">
        <h2><i class="fas fa-user-tie"></i> Manager Dashboard</h2>
        <p>Manage your team's performance and track their progress</p>
    </div>

    <!-- Statistics Cards -->
    <div class="row mb-4">
        <div class="col-md-3 mb-4">
            <div class="stat-card primary">
                <div class="stat-icon text-primary">
                    <i class="fas fa-users"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblTeamSize" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Team Members</div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card warning">
                <div class="stat-icon text-warning">
                    <i class="fas fa-tasks"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblPendingApprovals" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Pending Approvals</div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card info">
                <div class="stat-icon text-info">
                    <i class="fas fa-clipboard-list"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblReviewsDue" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Reviews Due</div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card success">
                <div class="stat-icon text-success">
                    <i class="fas fa-chart-line"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblTeamAvgScore" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Team Avg Score</div>
            </div>
        </div>
    </div>

    <div class="row">
        <!-- Pending Goal Approvals -->
        <div class="col-md-6">
            <div class="section-card">
                <h5><i class="fas fa-check-circle"></i> Pending Goal Approvals</h5>
                <asp:Repeater ID="rptPendingGoals" runat="server">
                    <ItemTemplate>
                        <div class="team-member-card">
                            <div class="d-flex justify-content-between align-items-start mb-2">
                                <div>
                                    <strong><%# Eval("full_name") %></strong>
                                    <p class="mb-1 small text-muted"><%# Eval("title") %></p>
                                </div>
                                <span class="badge bg-warning">Pending</span>
                            </div>
                            <p class="small mb-2"><%# Eval("description") %></p>
                            <div class="d-flex gap-2">
                                <a href="GoalApprovals.aspx?goalId=<%# Eval("goal_id") %>&action=approve" class="btn btn-sm btn-success">Approve</a>
                                <a href="GoalApprovals.aspx?goalId=<%# Eval("goal_id") %>&action=reject" class="btn btn-sm btn-danger">Reject</a>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoPendingGoals" runat="server" CssClass="text-muted text-center py-3 d-block" Visible="false">
                    <p>No pending approvals</p>
                </asp:Label>
                <div class="text-center mt-3">
                    <a href="GoalApprovals.aspx" class="quick-action-btn">View All Approvals</a>
                </div>
            </div>
        </div>

        <!-- Team Members -->
        <div class="col-md-6">
            <div class="section-card">
                <h5><i class="fas fa-users"></i> Team Members</h5>
                <asp:Repeater ID="rptTeamMembers" runat="server">
                    <ItemTemplate>
                        <div class="team-member-card">
                            <div class="d-flex justify-content-between align-items-center">
                                <div>
                                    <strong><%# Eval("full_name") %></strong>
                                    <p class="mb-0 small text-muted"><%# Eval("department") %></p>
                                </div>
                                <a href="TeamPerformance.aspx?userId=<%# Eval("user_id") %>" class="btn btn-sm btn-primary">View</a>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoTeamMembers" runat="server" CssClass="text-muted text-center py-3 d-block" Visible="false">
                    <p>No team members</p>
                </asp:Label>
            </div>
        </div>
    </div>

    <!-- Quick Actions -->
    <div class="section-card mt-4">
        <h5><i class="fas fa-bolt"></i> Quick Actions</h5>
        <div class="row">
            <div class="col-md-3 mb-3">
                <a href="GoalApprovals.aspx" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-check-circle"></i> Approve Goals
                </a>
            </div>
            <div class="col-md-3 mb-3">
                <a href="GiveFeedback.aspx" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-comment-alt"></i> Give Feedback
                </a>
            </div>
            <div class="col-md-3 mb-3">
                <a href="ConductReviews.aspx" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-clipboard-list"></i> Conduct Reviews
                </a>
            </div>
            <div class="col-md-3 mb-3">
                <a href="ManagerReports.aspx" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-chart-bar"></i> View Reports
                </a>
            </div>
        </div>
    </div>
</asp:Content>

