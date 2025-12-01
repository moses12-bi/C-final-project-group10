<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="PTMS.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .profile-card {
            background: white;
            border-radius: 15px;
            padding: 30px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-user"></i> My Profile</h3>

    <div class="profile-card">
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Full Name</label>
                <asp:TextBox ID="txtFullName" CssClass="form-control" runat="server"></asp:TextBox>
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Email</label>
                <asp:TextBox ID="txtEmail" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Role</label>
                <asp:TextBox ID="txtRole" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Department</label>
                <asp:TextBox ID="txtDepartment" CssClass="form-control" runat="server"></asp:TextBox>
            </div>
        </div>
        <asp:Button ID="btnUpdate" CssClass="btn btn-primary" Text="Update Profile" OnClick="btnUpdate_Click" runat="server" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success mt-3" Visible="false"></asp:Label>
    </div>
</asp:Content>

