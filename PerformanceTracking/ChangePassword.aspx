<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="PTMS.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .password-form {
            background: white;
            border-radius: 15px;
            padding: 30px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            max-width: 500px;
            margin: 0 auto;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-key"></i> Change Password</h3>

    <div class="password-form">
        <div class="mb-3">
            <label class="form-label">Current Password *</label>
            <asp:TextBox ID="txtCurrentPassword" CssClass="form-control" TextMode="Password" runat="server" placeholder="Enter current password"></asp:TextBox>
        </div>
        <div class="mb-3">
            <label class="form-label">New Password *</label>
            <asp:TextBox ID="txtNewPassword" CssClass="form-control" TextMode="Password" runat="server" placeholder="Enter new password"></asp:TextBox>
            <small class="text-muted">Password must be at least 6 characters long</small>
        </div>
        <div class="mb-3">
            <label class="form-label">Confirm New Password *</label>
            <asp:TextBox ID="txtConfirmPassword" CssClass="form-control" TextMode="Password" runat="server" placeholder="Confirm new password"></asp:TextBox>
        </div>
        <asp:Button ID="btnChangePassword" CssClass="btn btn-primary w-100" Text="Change Password" OnClick="btnChangePassword_Click" runat="server" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success mt-3 d-block" Visible="false"></asp:Label>
    </div>
</asp:Content>

