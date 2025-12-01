<%@ Page Title="My Reviews" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="MyReviews.aspx.cs" Inherits="PTMS.MyReviews" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .review-card {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.08);
            margin-bottom: 20px;
            border-left: 4px solid #667eea;
        }

        .rating-display {
            font-size: 2rem;
            font-weight: 700;
            color: #667eea;
        }
    </style>

    <h3 class="mb-4"><i class="fas fa-clipboard-check"></i> My Performance Reviews</h3>

    <asp:Repeater ID="rptReviews" runat="server">
        <ItemTemplate>
            <div class="review-card">
                <div class="d-flex justify-content-between align-items-start mb-3">
                    <div style="flex: 1;">
                        <h5>Review Cycle: <%# Eval("cycle_name") %></h5>
                        <p class="text-muted mb-2">
                            <i class="fas fa-calendar"></i> 
                            <%# FormatDate(Eval("date_reviewed")) %>
                        </p>
                        <div class="rating-display mb-2">
                            Rating: <%# Eval("rating") != DBNull.Value ? Eval("rating") : "Pending" %>
                        </div>
                        <p><%# Eval("comments") != DBNull.Value ? Eval("comments") : "No comments yet." %></p>
                        <small class="text-muted">
                            Reviewed by: <%# Eval("manager_name") %>
                        </small>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Label ID="lblNoReviews" runat="server" Visible="false">
        <div class="text-center py-5">
            <i class="fas fa-clipboard-check" style="font-size: 4rem; color: #ccc; margin-bottom: 20px;"></i>
            <h4 class="text-muted">No reviews yet</h4>
            <p class="text-muted">Your performance reviews will appear here.</p>
        </div>
    </asp:Label>
</asp:Content>

