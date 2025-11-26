<%@ Page Title="HR Dashboard" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="HRDashboard.aspx.cs" Inherits="PTMS.HRDashboard" %>

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
    </style>

    <div class="welcome-section">
        <h2><i class="fas fa-building"></i> HR/Admin Dashboard</h2>
        <p>Manage organizational performance and analytics</p>
    </div>

    <!-- Statistics Cards -->
    <div class="row mb-4">
        <div class="col-md-3 mb-4">
            <div class="stat-card primary">
                <div class="stat-icon text-primary">
                    <i class="fas fa-users"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblTotalUsers" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Total Users</div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card info">
                <div class="stat-icon text-info">
                    <i class="fas fa-calendar-alt"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblActiveCycles" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Active Cycles</div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card success">
                <div class="stat-icon text-success">
                    <i class="fas fa-chart-line"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblOrgAvgScore" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Org Avg Score</div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card warning">
                <div class="stat-icon text-warning">
                    <i class="fas fa-exclamation-triangle"></i>
                </div>
                <div class="stat-value"><asp:Label ID="lblPendingReviews" runat="server" Text="0"></asp:Label></div>
                <div class="stat-label">Pending Reviews</div>
            </div>
        </div>
    </div>

    <!-- Quick Actions -->
    <div class="section-card">
        <h5><i class="fas fa-bolt"></i> Quick Actions</h5>
        <div class="row">
            <div class="col-md-3 mb-3">
                <a href="ManageUsers.aspx" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-user-cog"></i> Manage Users
                </a>
            </div>
            <div class="col-md-3 mb-3">
                <a href="ReviewCycles.aspx" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-calendar-alt"></i> Review Cycles
                </a>
            </div>
            <div class="col-md-3 mb-3">
                <a href="OrgGoals.aspx" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-building"></i> Org Goals
                </a>
            </div>
            <div class="col-md-3 mb-3">
                <a href="Analytics.aspx" class="quick-action-btn w-100 text-center">
                    <i class="fas fa-chart-line"></i> Analytics
                </a>
            </div>
        </div>
    </div>
</asp:Content>

