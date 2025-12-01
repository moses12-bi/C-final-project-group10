<%@ Page Title="Analytics" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Analytics.aspx.cs" Inherits="PTMS.Analytics" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .stat-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
            text-align: center;
        }

        .stat-value {
            font-size: 2.5rem;
            font-weight: 700;
            margin: 10px 0;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-chart-line"></i> Organizational Analytics</h3>

    <div class="row">
        <div class="col-md-3 mb-4">
            <div class="stat-card">
                <h5>Total Employees</h5>
                <div class="stat-value text-primary"><asp:Label ID="lblTotalEmployees" runat="server" Text="0"></asp:Label></div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card">
                <h5>Active Goals</h5>
                <div class="stat-value text-success"><asp:Label ID="lblActiveGoals" runat="server" Text="0"></asp:Label></div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card">
                <h5>Avg Performance</h5>
                <div class="stat-value text-info"><asp:Label ID="lblAvgPerformance" runat="server" Text="N/A"></asp:Label></div>
            </div>
        </div>
        <div class="col-md-3 mb-4">
            <div class="stat-card">
                <h5>Total Reviews</h5>
                <div class="stat-value text-warning"><asp:Label ID="lblTotalReviews" runat="server" Text="0"></asp:Label></div>
            </div>
        </div>
    </div>

    <div class="stat-card">
        <h5 class="mb-3">Department Performance</h5>
        <asp:GridView ID="gvDepartmentStats" CssClass="table table-striped" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="department" HeaderText="Department" />
                <asp:BoundField DataField="employee_count" HeaderText="Employees" />
                <asp:BoundField DataField="avg_rating" HeaderText="Avg Rating" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

