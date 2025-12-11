<%@ Page Title="Track Organizational Goals" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="TrackOrgGoals.aspx.cs" Inherits="PTMS.TrackOrgGoals" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .tracking-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
        }

        .goal-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 20px;
        }

        .stat-badge {
            display: inline-block;
            padding: 8px 15px;
            border-radius: 20px;
            font-size: 0.9rem;
            font-weight: 600;
            margin: 5px;
        }

        .progress-section {
            margin-top: 15px;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-chart-pie"></i> Track Organizational Goals</h3>

    <asp:Repeater ID="rptOrgGoals" runat="server" OnItemDataBound="rptOrgGoals_ItemDataBound">
        <ItemTemplate>
            <div class="tracking-card">
                <div class="goal-header">
                    <h4><%# Eval("title") %></h4>
                    <p class="mb-0"><%# Eval("description") %></p>
                </div>
                
                <div class="row mb-3">
                    <div class="col-md-3">
                        <div class="text-center">
                            <h5 class="text-primary"><%# Eval("total_employees") %></h5>
                            <small class="text-muted">Total Employees</small>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="text-center">
                            <h5 class="text-success"><%# Eval("completed_count") %></h5>
                            <small class="text-muted">Completed</small>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="text-center">
                            <h5 class="text-info"><%# Eval("in_progress_count") %></h5>
                            <small class="text-muted">In Progress</small>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="text-center">
                            <h5 class="text-warning"><%# Eval("not_started_count") %></h5>
                            <small class="text-muted">Not Started</small>
                        </div>
                    </div>
                </div>

                <div class="progress-section">
                    <div class="d-flex justify-content-between mb-2">
                        <span><strong>Overall Progress</strong></span>
                        <span><strong><%# Eval("avg_progress") %>%</strong></span>
                    </div>
                    <div class="progress" style="height: 25px;">
                        <div class="progress-bar bg-success" role="progressbar" 
                             style="width: <%# Eval("avg_progress") %>%" 
                             aria-valuenow='<%# Eval("avg_progress") %>' 
                             aria-valuemin="0" aria-valuemax="100">
                            <%# Eval("avg_progress") %>%
                        </div>
                    </div>
                </div>

                <div class="mt-4">
                    <h5 class="mb-3">Employee Progress Details</h5>
                    <asp:PlaceHolder ID="phEmployeeProgress" runat="server"></asp:PlaceHolder>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <asp:Label ID="lblNoOrgGoals" runat="server" Visible="false">
        <div class="text-center py-5">
            <i class="fas fa-chart-pie" style="font-size: 4rem; color: #ccc; margin-bottom: 20px;"></i>
            <h4 class="text-muted">No organizational goals to track</h4>
            <p class="text-muted">Create organizational goals to start tracking progress.</p>
        </div>
    </asp:Label>
</asp:Content>

