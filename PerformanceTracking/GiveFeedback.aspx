<%@ Page Title="Give Feedback" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="GiveFeedback.aspx.cs" Inherits="PTMS.GiveFeedback" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .feedback-form {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-comment-alt"></i> Give Feedback</h3>

    <div class="feedback-form">
        <div class="mb-3">
            <label class="form-label">To *</label>
            <asp:DropDownList ID="ddlReceiver" CssClass="form-select" runat="server"></asp:DropDownList>
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
            <label class="form-label">Rating (1-5) *</label>
            <asp:DropDownList ID="ddlRating" CssClass="form-select" runat="server">
                <asp:ListItem Text="Select Rating" Value="" Selected="True"></asp:ListItem>
                <asp:ListItem Text="1 - Poor" Value="1"></asp:ListItem>
                <asp:ListItem Text="2 - Below Average" Value="2"></asp:ListItem>
                <asp:ListItem Text="3 - Average" Value="3"></asp:ListItem>
                <asp:ListItem Text="4 - Good" Value="4"></asp:ListItem>
                <asp:ListItem Text="5 - Excellent" Value="5"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="mb-3">
            <label class="form-label">Comments *</label>
            <asp:TextBox ID="txtComments" CssClass="form-control" TextMode="MultiLine" Rows="5" runat="server" placeholder="Enter your feedback..."></asp:TextBox>
        </div>
        <asp:Button ID="btnSubmit" CssClass="btn btn-primary" Text="Submit Feedback" OnClick="btnSubmit_Click" runat="server" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success mt-3" Visible="false"></asp:Label>
    </div>
</asp:Content>

