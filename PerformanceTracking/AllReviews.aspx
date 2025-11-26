<%@ Page Title="All Reviews" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="AllReviews.aspx.cs" Inherits="PTMS.AllReviews" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .review-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-clipboard-list"></i> All Performance Reviews</h3>

    <asp:GridView ID="gvAllReviews" CssClass="table table-striped table-hover" runat="server" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="employee_name" HeaderText="Employee" />
            <asp:BoundField DataField="cycle_name" HeaderText="Cycle" />
            <asp:BoundField DataField="rating" HeaderText="Rating" />
            <asp:BoundField DataField="date_reviewed" HeaderText="Date Reviewed" />
        </Columns>
    </asp:GridView>
</asp:Content>

