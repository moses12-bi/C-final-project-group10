<%@ Page Title="Reports" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ManagerReports.aspx.cs" Inherits="PTMS.ManagerReports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .report-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-chart-bar"></i> Manager Reports</h3>

    <div class="row mb-4">
        <div class="col-md-4">
            <div class="report-card text-center">
                <h5>Team Size</h5>
                <h2 class="text-primary"><asp:Label ID="lblTeamSize" runat="server" Text="0"></asp:Label></h2>
            </div>
        </div>
        <div class="col-md-4">
            <div class="report-card text-center">
                <h5>Total Goals</h5>
                <h2 class="text-success"><asp:Label ID="lblTotalGoals" runat="server" Text="0"></asp:Label></h2>
            </div>
        </div>
        <div class="col-md-4">
            <div class="report-card text-center">
                <h5>Average Rating</h5>
                <h2 class="text-info"><asp:Label ID="lblAvgRating" runat="server" Text="N/A"></asp:Label></h2>
            </div>
        </div>
    </div>

    <div class="report-card">
        <h5 class="mb-3">Team Performance Summary</h5>
        <asp:GridView ID="gvTeamSummary" CssClass="table table-striped" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="full_name" HeaderText="Employee" />
                <asp:BoundField DataField="total_goals" HeaderText="Total Goals" />
                <asp:BoundField DataField="completed_goals" HeaderText="Completed" />
                <asp:BoundField DataField="avg_rating" HeaderText="Avg Rating" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

