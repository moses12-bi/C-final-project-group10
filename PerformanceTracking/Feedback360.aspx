<%@ Page Title="360 Feedback" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Feedback360.aspx.cs" Inherits="PTMS.Feedback360" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .info-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border-radius: 15px;
            padding: 25px;
            margin-bottom: 30px;
        }

        .request-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-users"></i> 360-Degree Feedback</h3>

    <div class="info-card">
        <h5><i class="fas fa-info-circle"></i> About 360 Feedback</h5>
        <p class="mb-0">Request feedback from peers, subordinates, and supervisors to get a comprehensive view of your performance from multiple perspectives.</p>
    </div>

    <div class="request-card">
        <h5 class="mb-3">Request 360 Feedback</h5>
        <div class="mb-3">
            <label class="form-label">Select Recipients *</label>
            <asp:CheckBoxList ID="chkRecipients" CssClass="form-check" runat="server"></asp:CheckBoxList>
        </div>
        <div class="mb-3">
            <label class="form-label">Message (Optional)</label>
            <asp:TextBox ID="txtMessage" CssClass="form-control" TextMode="MultiLine" Rows="3" runat="server" placeholder="Add a message to your feedback request..."></asp:TextBox>
        </div>
        <asp:Button ID="btnRequest" CssClass="btn btn-primary" Text="Send Requests" OnClick="btnRequest_Click" runat="server" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success mt-3" Visible="false"></asp:Label>
    </div>
</asp:Content>

