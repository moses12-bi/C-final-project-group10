<%@ Page Title="Request Feedback" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="RequestFeedback.aspx.cs" Inherits="PTMS.RequestFeedback" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .feedback-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-comment-dots"></i> Request Feedback</h3>

    <div class="feedback-card">
        <div class="mb-3">
            <label class="form-label">Request From *</label>
            <asp:DropDownList ID="ddlRequestFrom" CssClass="form-select" runat="server"></asp:DropDownList>
        </div>
        <div class="mb-3">
            <label class="form-label">Feedback Type *</label>
            <asp:DropDownList ID="ddlFeedbackType" CssClass="form-select" runat="server">
                <asp:ListItem Text="Select Type" Value="" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Performance" Value="Performance"></asp:ListItem>
                <asp:ListItem Text="Skills" Value="Skills"></asp:ListItem>
                <asp:ListItem Text="Behavior" Value="Behavior"></asp:ListItem>
                <asp:ListItem Text="General" Value="General"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="mb-3">
            <label class="form-label">Message (Optional)</label>
            <asp:TextBox ID="txtMessage" CssClass="form-control" TextMode="MultiLine" Rows="4" runat="server" placeholder="Add a message to your feedback request..."></asp:TextBox>
        </div>
        <asp:Button ID="btnRequest" CssClass="btn btn-primary" Text="Send Request" OnClick="btnRequest_Click" runat="server" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success mt-3" Visible="false"></asp:Label>
    </div>
</asp:Content>

