<%@ Page Title="Review Cycles" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ReviewCycles.aspx.cs" Inherits="PTMS.ReviewCycles" %>
<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <style>
        .cycle-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
        }
    </style>

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h3><i class="fas fa-calendar-alt"></i> Review Cycles</h3>
        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#cycleModal">
            <i class="fas fa-plus"></i> Create Cycle
        </button>
    </div>

    <asp:Repeater ID="rptCycles" runat="server">
        <ItemTemplate>
            <div class="cycle-card">
                <div class="d-flex justify-content-between align-items-start">
                    <div style="flex: 1;">
                        <h5><%# Eval("cycle_name") %></h5>
                        <p class="text-muted mb-2">
                            <i class="fas fa-calendar"></i> 
                            <%# FormatDate(Eval("start_date")) %> - <%# FormatDate(Eval("end_date")) %>
                        </p>
                        <span class="badge bg-<%# GetStatusBadgeColor(Eval("status").ToString()) %>">
                            <%# Eval("status") %>
                        </span>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <!-- Create Cycle Modal -->
    <div class="modal fade" id="cycleModal" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Create Review Cycle</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label">Cycle Name *</label>
                        <asp:TextBox ID="txtCycleName" CssClass="form-control" runat="server"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Start Date *</label>
                        <asp:TextBox ID="txtStartDate" CssClass="form-control" TextMode="Date" runat="server"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">End Date *</label>
                        <asp:TextBox ID="txtEndDate" CssClass="form-control" TextMode="Date" runat="server"></asp:TextBox>
                    </div>
                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnCreate" CssClass="btn btn-primary" Text="Create Cycle" OnClick="btnCreate_Click" runat="server" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

