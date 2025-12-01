<%@ Page Title="Organization Goals" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="OrgGoals.aspx.cs" Inherits="PTMS.OrgGoals" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .goal-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
        }
    </style>

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h3><i class="fas fa-building"></i> Organization Goals</h3>
        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#goalModal">
            <i class="fas fa-plus"></i> Create Org Goal
        </button>
    </div>

    <asp:Repeater ID="rptOrgGoals" runat="server">
        <ItemTemplate>
            <div class="goal-card">
                <h5><%# Eval("title") %></h5>
                <p class="text-muted"><%# Eval("description") %></p>
                <span class="badge bg-primary">Organization Goal</span>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Label ID="lblNoOrgGoals" runat="server" Visible="false">
        <div class="text-center py-5">
            <i class="fas fa-building" style="font-size: 4rem; color: #ccc; margin-bottom: 20px;"></i>
            <h4 class="text-muted">No organization goals yet</h4>
        </div>
    </asp:Label>

    <!-- Create Goal Modal -->
    <div class="modal fade" id="goalModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Create Organization Goal</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label">Goal Title *</label>
                        <asp:TextBox ID="txtTitle" CssClass="form-control" runat="server"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Description *</label>
                        <asp:TextBox ID="txtDescription" CssClass="form-control" TextMode="MultiLine" Rows="4" runat="server"></asp:TextBox>
                    </div>
                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnCreate" CssClass="btn btn-primary" Text="Create Goal" OnClick="btnCreate_Click" runat="server" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

