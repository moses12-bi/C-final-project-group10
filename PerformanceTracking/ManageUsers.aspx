<%@ Page Title="Manage Users" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="PTMS.ManageUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .user-card {
            background: white;
            border-radius: 15px;
            padding: 20px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 15px;
        }
    </style>

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h3><i class="fas fa-user-cog"></i> Manage Users</h3>
        <a href="SignUp.aspx" class="btn btn-primary"><i class="fas fa-plus"></i> Add New User</a>
    </div>

    <asp:GridView ID="gvUsers" CssClass="table table-striped table-hover" runat="server" AutoGenerateColumns="false" OnRowCommand="gvUsers_RowCommand">
        <Columns>
            <asp:BoundField DataField="full_name" HeaderText="Name" />
            <asp:BoundField DataField="email" HeaderText="Email" />
            <asp:BoundField DataField="role" HeaderText="Role" />
            <asp:BoundField DataField="department" HeaderText="Department" />
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:LinkButton ID="btnEdit" runat="server" 
                        CommandName="EditUser" 
                        CommandArgument='<%# Eval("user_id") %>'
                        CssClass="btn btn-sm btn-primary">
                        <i class="fas fa-edit"></i> Edit
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>

