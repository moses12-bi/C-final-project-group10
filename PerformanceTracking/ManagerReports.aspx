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

    <!-- Organizational Goals Tracking Section -->
    <div class="report-card">
        <h5 class="mb-3"><i class="fas fa-building"></i> Team Organizational Goals Progress</h5>
        <asp:Repeater ID="rptTeamOrgGoals" runat="server">
            <ItemTemplate>
                <div class="mb-4" style="border-bottom: 2px solid #e0e0e0; padding-bottom: 20px;">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <div>
                            <h6 class="mb-1"><%# Eval("title") %></h6>
                            <p class="text-muted small mb-2"><%# Eval("description") %></p>
                        </div>
                        <span class="badge bg-primary">Org Goal</span>
                    </div>
                    
                    <div class="row mb-2">
                        <div class="col-md-3">
                            <small class="text-muted">Team Members: <strong><%# Eval("team_count") %></strong></small>
                        </div>
                        <div class="col-md-3">
                            <small class="text-muted">Completed: <strong class="text-success"><%# Eval("completed_count") %></strong></small>
                        </div>
                        <div class="col-md-3">
                            <small class="text-muted">In Progress: <strong class="text-info"><%# Eval("in_progress_count") %></strong></small>
                        </div>
                        <div class="col-md-3">
                            <small class="text-muted">Avg Progress: <strong><%# Eval("avg_progress") %>%</strong></small>
                        </div>
                    </div>
                    
                    <div class="progress" style="height: 20px;">
                        <div class="progress-bar bg-success" style="width: <%# Eval("avg_progress") %>%">
                            <%# Eval("avg_progress") %>%
                        </div>
                    </div>
                    
                    <div class="mt-3">
                        <%# GetTeamMemberProgressHTML(Eval("title").ToString()) %>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Label ID="lblNoOrgGoals" runat="server" Visible="false">
            <p class="text-muted text-center py-3">No organizational goals assigned to your team yet.</p>
        </asp:Label>
    </div>
</asp:Content>

