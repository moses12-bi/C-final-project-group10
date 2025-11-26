<%@ Page Title="Goal Approvals" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="GoalApprovals.aspx.cs" Inherits="PTMS.GoalApprovals" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .approval-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
            border-left: 4px solid #ffc107;
        }

        .approval-card.approved {
            border-left-color: #28a745;
        }

        .approval-card.rejected {
            border-left-color: #dc3545;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-check-circle"></i> Goal Approvals</h3>

    <asp:Repeater ID="rptPendingGoals" runat="server" OnItemCommand="rptPendingGoals_ItemCommand">
        <ItemTemplate>
            <div class="approval-card">
                <div class="d-flex justify-content-between align-items-start mb-3">
                    <div style="flex: 1;">
                        <h5><%# Eval("title") %></h5>
                        <p class="text-muted mb-2"><%# Eval("description") %></p>
                        <div class="d-flex gap-3 align-items-center mb-2">
                            <span><strong>Employee:</strong> <%# Eval("full_name") %></span>
                            <span class="badge bg-warning">Pending Approval</span>
                        </div>
                        <div class="mb-2">
                            <small class="text-muted">Progress: <%# Eval("progress") %>%</small>
                            <div class="progress" style="height: 6px;">
                                <div class="progress-bar" style="width: <%# Eval("progress") %>%"></div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="mb-3">
                    <label class="form-label">Manager Comment (Optional)</label>
                    <asp:TextBox ID="txtComment" CssClass="form-control" TextMode="MultiLine" Rows="3" runat="server" placeholder="Add your comment..."></asp:TextBox>
                </div>
                <div class="d-flex gap-2">
                    <asp:Button ID="btnApprove" runat="server" 
                        CommandName="Approve" 
                        CommandArgument='<%# Eval("goal_id") %>'
                        CssClass="btn btn-success"
                        Text="Approve" />
                    <asp:Button ID="btnReject" runat="server" 
                        CommandName="Reject" 
                        CommandArgument='<%# Eval("goal_id") %>'
                        CssClass="btn btn-danger"
                        Text="Reject" />
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Label ID="lblNoApprovals" runat="server" Visible="false">
        <div class="text-center py-5">
            <i class="fas fa-check-circle" style="font-size: 4rem; color: #28a745; margin-bottom: 20px;"></i>
            <h4 class="text-muted">No pending approvals</h4>
            <p class="text-muted">All goals have been reviewed!</p>
        </div>
    </asp:Label>

    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success" Visible="false"></asp:Label>
</asp:Content>

