<%@ Page Title="View Feedback" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ViewFeedback.aspx.cs" Inherits="PTMS.ViewFeedback" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .feedback-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
            border-left: 4px solid #667eea;
        }

        .rating-stars {
            color: #ffc107;
            font-size: 1.5rem;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-comments"></i> My Feedback</h3>

    <asp:Repeater ID="rptFeedback" runat="server">
        <ItemTemplate>
            <div class="feedback-card">
                <div class="d-flex justify-content-between align-items-start mb-3">
                    <div style="flex: 1;">
                        <h5><%# Eval("full_name") %></h5>
                        <p class="text-muted mb-2"><%# Eval("feedback_type") %> Feedback</p>
                        <div class="rating-stars mb-2">
                            <%# GetStars(Convert.ToInt32(Eval("rating"))) %>
                        </div>
                        <p><%# Eval("comments") %></p>
                        <small class="text-muted">
                            <i class="fas fa-calendar"></i> <%# FormatDate(Eval("date_given")) %>
                        </small>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Label ID="lblNoFeedback" runat="server" Visible="false">
        <div class="text-center py-5">
            <i class="fas fa-comments" style="font-size: 4rem; color: #ccc; margin-bottom: 20px;"></i>
            <h4 class="text-muted">No feedback yet</h4>
            <p class="text-muted">Request feedback from your colleagues to get started!</p>
        </div>
    </asp:Label>
</asp:Content>

