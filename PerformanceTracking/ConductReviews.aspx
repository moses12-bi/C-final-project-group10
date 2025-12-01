<%@ Page Title="Conduct Reviews" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ConductReviews.aspx.cs" Inherits="PTMS.ConductReviews" %>

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

    <h3 class="mb-4"><i class="fas fa-clipboard-list"></i> Conduct Performance Reviews</h3>

    <asp:Repeater ID="rptEmployees" runat="server" OnItemCommand="rptEmployees_ItemCommand">
        <ItemTemplate>
            <div class="review-card">
                <div class="d-flex justify-content-between align-items-start mb-3">
                    <div style="flex: 1;">
                        <h5><%# Eval("full_name") %></h5>
                        <p class="text-muted mb-2"><%# Eval("department") %></p>
                        <p class="mb-2"><strong>Review Cycle:</strong> <%# Eval("cycle_name") %></p>
                    </div>
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
                    <asp:TextBox ID="txtComments" CssClass="form-control" TextMode="MultiLine" Rows="4" runat="server" placeholder="Enter review comments..."></asp:TextBox>
                </div>
                <asp:Button ID="btnSubmit" runat="server" 
                    CommandName="SubmitReview" 
                    CommandArgument='<%# Eval("review_id") + "|" + Eval("employee_id") %>'
                    CssClass="btn btn-primary"
                    Text="Submit Review" />
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Label ID="lblNoReviews" runat="server" Visible="false">
        <div class="text-center py-5">
            <i class="fas fa-clipboard-list" style="font-size: 4rem; color: #ccc; margin-bottom: 20px;"></i>
            <h4 class="text-muted">No reviews pending</h4>
            <p class="text-muted">All employee reviews are complete!</p>
        </div>
    </asp:Label>

    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success" Visible="false"></asp:Label>
</asp:Content>

