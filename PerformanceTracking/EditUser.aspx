<%@ Page Title="Edit User" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EditUser.aspx.cs" Inherits="PTMS.EditUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .user-form {
            background: white;
            border-radius: 15px;
            padding: 30px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            max-width: 600px;
            margin: 0 auto;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-user-edit"></i> Edit User</h3>

    <div class="user-form">
        <asp:HiddenField ID="hdnUserId" runat="server" />
        <div class="mb-3">
            <label class="form-label">Full Name *</label>
            <asp:TextBox ID="txtFullName" CssClass="form-control" runat="server"></asp:TextBox>
        </div>
        <div class="mb-3">
            <label class="form-label">Email *</label>
            <asp:TextBox ID="txtEmail" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
            <small class="text-muted">Email cannot be changed</small>
        </div>
        <div class="mb-3">
            <label class="form-label">Role *</label>
            <asp:DropDownList ID="ddlRole" CssClass="form-select" runat="server">
                <asp:ListItem Text="Employee" Value="Employee"></asp:ListItem>
                <asp:ListItem Text="Manager" Value="Manager"></asp:ListItem>
                <asp:ListItem Text="HR" Value="HR"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="mb-3">
            <label class="form-label">Department *</label>
            <asp:TextBox ID="txtDepartment" CssClass="form-control" runat="server"></asp:TextBox>
        </div>
        <div class="mb-3">
            <label class="form-label">Manager</label>
            <asp:DropDownList ID="ddlManager" CssClass="form-select" runat="server">
                <asp:ListItem Text="None" Value=""></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="d-flex gap-2">
            <asp:Button ID="btnUpdate" CssClass="btn btn-primary" Text="Update User" OnClick="btnUpdate_Click" runat="server" />
            <a href="ManageUsers.aspx" class="btn btn-secondary">Cancel</a>
        </div>
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success mt-3 d-block" Visible="false"></asp:Label>
    </div>
</asp:Content>

