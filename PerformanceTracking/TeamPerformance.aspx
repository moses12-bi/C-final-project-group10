<%@ Page Title="Team Performance" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="TeamPerformance.aspx.cs" Inherits="PTMS.TeamPerformance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .performance-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-users"></i> Team Performance</h3>

    <asp:Repeater ID="rptTeamMembers" runat="server">
        <ItemTemplate>
            <div class="performance-card">
                <div class="d-flex justify-content-between align-items-start mb-3">
                    <div style="flex: 1;">
                        <h5><%# Eval("full_name") %></h5>
                        <p class="text-muted mb-2"><%# Eval("department") %></p>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4 mb-2">
                        <strong>Total Goals:</strong> <%# Eval("total_goals") %>
                    </div>
                    <div class="col-md-4 mb-2">
                        <strong>Completed Goals:</strong> <%# Eval("completed_goals") %>
                    </div>
                    <div class="col-md-4 mb-2">
                        <strong>Avg Rating:</strong> <%# Eval("avg_rating") != DBNull.Value ? Eval("avg_rating") : "N/A" %>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Label ID="lblNoTeamMembers" runat="server" Visible="false">
        <div class="text-center py-5">
            <i class="fas fa-users" style="font-size: 4rem; color: #ccc; margin-bottom: 20px;"></i>
            <h4 class="text-muted">No team members</h4>
        </div>
    </asp:Label>
</asp:Content>

