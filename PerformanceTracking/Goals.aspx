<%@ Page Title="My Goals" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Goals.aspx.cs" Inherits="PTMS.Goals" %>
<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <style>
        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
        }

        .btn-create {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 10px;
            font-weight: 600;
            text-decoration: none;
            transition: all 0.3s ease;
        }

        .btn-create:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 25px rgba(102, 126, 234, 0.4);
            color: white;
        }

        .goal-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
            transition: all 0.3s ease;
            border-left: 4px solid #667eea;
        }

        .goal-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
        }

        .goal-card.completed {
            border-left-color: #28a745;
        }

        .goal-card.pending {
            border-left-color: #ffc107;
        }

        .goal-card.rejected {
            border-left-color: #dc3545;
        }

        .modal-content {
            border-radius: 15px;
            border: none;
        }

        .form-control, .form-select {
            border-radius: 10px;
            border: 2px solid #e0e0e0;
            padding: 12px;
        }

        .form-control:focus, .form-select:focus {
            border-color: #667eea;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
        }
    </style>

    <div class="page-header">
        <h3><i class="fas fa-bullseye"></i> My Goals</h3>
        <button type="button" class="btn-create" data-bs-toggle="modal" data-bs-target="#goalModal">
            <i class="fas fa-plus"></i> Create New Goal
        </button>
    </div>

    <!-- Goals List -->
    <asp:Repeater ID="rptGoals" runat="server" OnItemCommand="rptGoals_ItemCommand">
        <ItemTemplate>
            <div class="goal-card <%# Eval("status").ToString().ToLower().Replace(" ", "") %>">
                <div class="d-flex justify-content-between align-items-start mb-3">
                    <div style="flex: 1;">
                        <h5 class="mb-2"><%# Eval("title") %></h5>
                        <p class="text-muted mb-2"><%# Eval("description") %></p>
                        <div class="d-flex gap-3 align-items-center">
                            <span class="badge bg-<%# GetStatusBadgeColor(Eval("status").ToString()) %>">
                                <%# Eval("status") %>
                            </span>
                            <small class="text-muted">
                                <i class="fas fa-calendar"></i> Created: <%# FormatDate(Eval("created_at")) %>
                            </small>
                        </div>
                    </div>
                </div>
                <div class="mb-3">
                    <div class="d-flex justify-content-between small mb-1">
                        <span>Progress</span>
                        <span><%# Eval("progress") %>%</span>
                    </div>
                    <div class="progress" style="height: 8px;">
                        <div class="progress-bar bg-primary" role="progressbar" 
                             style="width: <%# Eval("progress") %>%"></div>
                    </div>
                </div>
                <%# GetManagerComment(Eval("manager_comment")) %>
                <div class="d-flex gap-2">
                    <asp:LinkButton ID="btnUpdate" runat="server" 
                        CommandName="UpdateGoal" 
                        CommandArgument='<%# Eval("goal_id") %>'
                        CssClass="btn btn-sm btn-primary">
                        <i class="fas fa-edit"></i> Update
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnDelete" runat="server" 
                        CommandName="DeleteGoal" 
                        CommandArgument='<%# Eval("goal_id") %>'
                        CssClass="btn btn-sm btn-danger"
                        OnClientClick="return confirm('Are you sure you want to delete this goal?');">
                        <i class="fas fa-trash"></i> Delete
                    </asp:LinkButton>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Label ID="lblNoGoals" runat="server" Visible="false">
        <div class="text-center py-5">
            <i class="fas fa-bullseye" style="font-size: 4rem; color: #ccc; margin-bottom: 20px;"></i>
            <h4 class="text-muted">No goals yet</h4>
            <p class="text-muted">Create your first goal to get started!</p>
            <button type="button" class="btn-create mt-3" data-bs-toggle="modal" data-bs-target="#goalModal">
                <i class="fas fa-plus"></i> Create Goal
            </button>
        </div>
    </asp:Label>

    <!-- Create/Edit Goal Modal -->
    <div class="modal fade" id="goalModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Create New Goal"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hdnGoalId" runat="server" />
                    <div class="mb-3">
                        <label class="form-label">Goal Title *</label>
                        <asp:TextBox ID="txtTitle" CssClass="form-control" runat="server" placeholder="Enter goal title"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Description *</label>
                        <asp:TextBox ID="txtDescription" CssClass="form-control" TextMode="MultiLine" Rows="4" runat="server" placeholder="Describe your goal"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Progress (%)</label>
                        <asp:TextBox ID="txtProgress" CssClass="form-control" TextMode="Number" min="0" max="100" runat="server" Text="0"></asp:TextBox>
                    </div>
                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnSaveGoal" CssClass="btn-create" Text="Save Goal" OnClick="btnSaveGoal_Click" runat="server" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

