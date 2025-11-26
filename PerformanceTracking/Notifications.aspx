<%@ Page Title="Notifications" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="PTMS.Notifications" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .notification-item {
            background: white;
            border-radius: 15px;
            padding: 20px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 15px;
            border-left: 4px solid #667eea;
            transition: all 0.3s ease;
        }

        .notification-item.unread {
            background: #e7f3ff;
            border-left-color: #17a2b8;
        }

        .notification-item:hover {
            transform: translateX(5px);
        }
    </style>

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h3><i class="fas fa-bell"></i> Notifications</h3>
        <asp:Button ID="btnMarkAllRead" CssClass="btn btn-secondary" Text="Mark All as Read" OnClick="btnMarkAllRead_Click" runat="server" />
    </div>

    <asp:Repeater ID="rptNotifications" runat="server">
        <ItemTemplate>
            <div class="notification-item <%# Eval("status").ToString() == "unread" ? "unread" : "" %>">
                <div class="d-flex justify-content-between align-items-start">
                    <div style="flex: 1;">
                        <p class="mb-1"><%# Eval("message") %></p>
                        <small class="text-muted">
                            <i class="fas fa-clock"></i> <%# FormatDate(Eval("created_at")) %>
                        </small>
                    </div>
                    <%# Eval("status").ToString() == "unread" ? "<span class='badge bg-primary'>New</span>" : "" %>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Label ID="lblNoNotifications" runat="server" Visible="false">
        <div class="text-center py-5">
            <i class="fas fa-bell-slash" style="font-size: 4rem; color: #ccc; margin-bottom: 20px;"></i>
            <h4 class="text-muted">No notifications</h4>
            <p class="text-muted">You're all caught up!</p>
        </div>
    </asp:Label>
</asp:Content>

