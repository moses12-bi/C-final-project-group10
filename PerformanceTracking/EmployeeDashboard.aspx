<%@ Page Title="Employee Dashboard" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeDashboard.aspx.cs" Inherits="PTMS.EmployeeDashboard" %>

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

        .welcome-section h2 {
            margin: 0;
            font-weight: 700;
        }

        .welcome-section p {
            margin: 10px 0 0 0;
            opacity: 0.95;
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

        .notification-item {
            padding: 15px;
            border-left: 4px solid #667eea;
            background: #f8f9fa;
            border-radius: 8px;
            margin-bottom: 10px;
            transition: all 0.3s ease;
        }

        .notification-item:hover {
            background: #e9ecef;
            transform: translateX(5px);
        }

        .notification-item.unread {
            background: #e7f3ff;
            border-left-color: #17a2b8;
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

        .goal-progress {
            margin-top: 15px;
        }

        .progress {
            height: 8px;
            border-radius: 10px;
            background: #e9ecef;
        }

        .progress-bar {
            border-radius: 10px;
        }
    </style>

    <div class="welcome-section">
        <h2><i class="fas fa-user-circle"></i> Welcome back, <asp:Label ID="lblEmployeeName" runat="server"></asp:Label>!</h2>
        <p>Here's an overview of your performance and goals</p>
    </div>

    <!-- Statistics Cards -->
    <div class="row mb-4">
        <div class="col-md-3 mb-4">
            <div class="stat-card primary">
                <div class="stat-icon text-primary">
                    <i class="fas fa-bullseye"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblTotalGoals" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Total Goals</div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card success">
                <div class="stat-icon text-success">
                    <i class="fas fa-check-circle"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblCompletedGoals" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Completed Goals</div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card info">
                <div class="stat-icon text-info">
                    <i class="fas fa-comments"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblFeedbackCount" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Feedback Received</div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card warning">
                <div class="stat-icon text-warning">
                    <i class="fas fa-clipboard-check"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblPendingReviews" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Pending Reviews</div>
            </div>
        </div>
    </div>

    <div class="row">
        <!-- Recent Goals -->
        <div class="col-md-6">
            <div class="section-card">
                <h5><i class="fas fa-bullseye"></i> Recent Goals</h5>
                <asp:Repeater ID="rptRecentGoals" runat="server">
                    <ItemTemplate>
                        <div class="mb-3 p-3" style="background: #f8f9fa; border-radius: 10px;">
                            <div class="d-flex justify-content-between align-items-start mb-2">
                                <strong><%# Eval("title") %></strong>
                                <span class="badge bg-<%# GetStatusBadgeColor(Eval("status").ToString()) %>">
                                    <%# Eval("status") %>
                                </span>
                            </div>
                            <p class="text-muted small mb-2"><%# Eval("description") %></p>
                            <div class="goal-progress">
                                <div class="d-flex justify-content-between small mb-1">
                                    <span>Progress</span>
                                    <span><%# Eval("progress") %>%</span>
                                </div>
                                <div class="progress">
                                    <div class="progress-bar bg-primary" role="progressbar" 
                                         style="width: <%# Eval("progress") %>%" 
                                         aria-valuenow="<%# Eval("progress") %>" 
                                         aria-valuemin="0" aria-valuemax="100"></div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoGoals" runat="server" CssClass="text-muted text-center py-3 d-block" Visible="false">
                    <p>No goals yet. <a href="Goals.aspx">Create your first goal!</a></p>
                </asp:Label>
                <div class="text-center mt-3">
                    <a href="Goals.aspx" class="quick-action-btn">View All Goals</a>
                </div>
            </div>
        </div>

        <!-- Recent Notifications -->
        <div class="col-md-6">
            <div class="section-card">
                <h5><i class="fas fa-bell"></i> Recent Notifications</h5>
                <asp:Repeater ID="rptNotifications" runat="server">
                    <ItemTemplate>
                        <div class="notification-item <%# Eval("status").ToString() == "unread" ? "unread" : "" %>">
                            <div class="d-flex justify-content-between align-items-start">
                                <div>
                                    <p class="mb-1"><%# Eval("message") %></p>
                                    <small class="text-muted"><i class="fas fa-clock"></i> <%# FormatDate(Eval("created_at")) %></small>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoNotifications" runat="server" CssClass="text-muted text-center py-3 d-block" Visible="false">
                    <p>No notifications</p>
                </asp:Label>
                <div class="text-center mt-3">
                    <a href="Notifications.aspx" class="quick-action-btn">View All Notifications</a>
                </div>
            </div>
        </div>
    </div>

    <!-- Quick Actions -->
    <div class="section-card mt-4">
        <h5><i class="fas fa-bolt"></i> Quick Actions</h5>
        <div class="row">
            <div class="col-md-3 mb-3">
                <a href="Goals.aspx?action=create" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-plus"></i> Create Goal
                </a>
            </div>
            <div class="col-md-3 mb-3">
                <a href="RequestFeedback.aspx" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-comment-dots"></i> Request Feedback
                </a>
            </div>
            <div class="col-md-3 mb-3">
                <a href="Feedback360.aspx" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-users"></i> 360 Feedback
                </a>
            </div>
            <div class="col-md-3 mb-3">
                <a href="MyReviews.aspx" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-clipboard-check"></i> View Reviews
                </a>
            </div>
        </div>
    </div>
</asp:Content>
