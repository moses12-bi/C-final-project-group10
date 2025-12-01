<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="PTMS.SignUp" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>PTMS - Sign Up</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .signup-container {
            background: white;
            border-radius: 20px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            overflow: hidden;
            max-width: 1100px;
            width: 100%;
            display: flex;
            min-height: 700px;
        }

        .signup-left {
            flex: 1;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 60px 50px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            position: relative;
            overflow: hidden;
        }

        .signup-left::before {
            content: '';
            position: absolute;
            width: 200px;
            height: 200px;
            background: rgba(255, 255, 255, 0.1);
            border-radius: 50%;
            top: -50px;
            right: -50px;
        }

        .signup-left::after {
            content: '';
            position: absolute;
            width: 150px;
            height: 150px;
            background: rgba(255, 255, 255, 0.1);
            border-radius: 50%;
            bottom: -30px;
            left: -30px;
        }

        .signup-left h1 {
            font-size: 2.5rem;
            font-weight: 700;
            margin-bottom: 20px;
            position: relative;
            z-index: 1;
        }

        .signup-left p {
            font-size: 1.1rem;
            line-height: 1.8;
            opacity: 0.95;
            position: relative;
            z-index: 1;
        }

        .signup-right {
            flex: 1.2;
            padding: 50px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            overflow-y: auto;
            max-height: 700px;
        }

        .signup-header {
            text-align: center;
            margin-bottom: 30px;
        }

        .signup-header h2 {
            color: #333;
            font-size: 2rem;
            font-weight: 700;
            margin-bottom: 10px;
        }

        .signup-header p {
            color: #666;
            font-size: 0.95rem;
        }

        .form-row {
            display: flex;
            gap: 15px;
            margin-bottom: 20px;
        }

        .form-group {
            margin-bottom: 20px;
            position: relative;
            flex: 1;
        }

        .form-group.full-width {
            flex: 1 1 100%;
        }

        .form-group label {
            display: block;
            margin-bottom: 8px;
            color: #333;
            font-weight: 600;
            font-size: 0.9rem;
        }

        .input-wrapper {
            position: relative;
        }

        .input-wrapper i {
            position: absolute;
            left: 15px;
            top: 50%;
            transform: translateY(-50%);
            color: #667eea;
            font-size: 1.1rem;
        }

        .form-control, .form-select {
            width: 100%;
            padding: 14px 14px 14px 45px;
            border: 2px solid #e0e0e0;
            border-radius: 10px;
            font-size: 1rem;
            transition: all 0.3s ease;
            background: #f8f9fa;
        }

        .form-select {
            padding-left: 45px;
            appearance: none;
            background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%23667eea' d='M6 9L1 4h10z'/%3E%3C/svg%3E");
            background-repeat: no-repeat;
            background-position: right 15px center;
        }

        .form-control:focus, .form-select:focus {
            outline: none;
            border-color: #667eea;
            background: white;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
        }

        .btn-signup {
            width: 100%;
            padding: 14px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            border-radius: 10px;
            font-size: 1.1rem;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            margin-top: 10px;
        }

        .btn-signup:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 25px rgba(102, 126, 234, 0.4);
        }

        .btn-signup:active {
            transform: translateY(0);
        }

        .login-link {
            text-align: center;
            margin-top: 25px;
            color: #666;
            font-size: 0.95rem;
        }

        .login-link a {
            color: #667eea;
            text-decoration: none;
            font-weight: 600;
            transition: color 0.3s ease;
        }

        .login-link a:hover {
            color: #764ba2;
            text-decoration: underline;
        }

        .alert {
            padding: 12px 20px;
            border-radius: 10px;
            margin-bottom: 20px;
            font-size: 0.9rem;
            border: none;
        }

        .alert-danger {
            background: #fee;
            color: #c33;
            border-left: 4px solid #c33;
        }

        .alert-success {
            background: #efe;
            color: #3c3;
            border-left: 4px solid #3c3;
        }

        @media (max-width: 768px) {
            .signup-container {
                flex-direction: column;
                max-width: 100%;
            }

            .signup-left {
                padding: 40px 30px;
                min-height: 200px;
            }

            .signup-left h1 {
                font-size: 1.8rem;
            }

            .signup-right {
                padding: 40px 30px;
            }

            .form-row {
                flex-direction: column;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="signup-container">
            <div class="signup-left">
                <h1><i class="fas fa-user-plus"></i> Join PTMS</h1>
                <p>Create your account and start tracking your performance journey.</p>
                <p style="margin-top: 20px;">Set goals, receive feedback, and grow with our comprehensive performance management platform.</p>
            </div>
            <div class="signup-right">
                <div class="signup-header">
                    <h2>Create Account</h2>
                    <p>Fill in your details to get started</p>
                </div>

                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger" Visible="false"></asp:Label>

                <div class="form-row">
                    <div class="form-group">
                        <label for="txtFullName"><i class="fas fa-user"></i> Full Name</label>
                        <div class="input-wrapper">
                            <i class="fas fa-user"></i>
                            <asp:TextBox ID="txtFullName" CssClass="form-control" placeholder="Enter your full name" runat="server"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="txtEmail"><i class="fas fa-envelope"></i> Email Address</label>
                        <div class="input-wrapper">
                            <i class="fas fa-envelope"></i>
                            <asp:TextBox ID="txtEmail" CssClass="form-control" placeholder="Enter your email" runat="server" TextMode="Email"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="txtPassword"><i class="fas fa-lock"></i> Password</label>
                        <div class="input-wrapper">
                            <i class="fas fa-lock"></i>
                            <asp:TextBox ID="txtPassword" TextMode="Password" CssClass="form-control" placeholder="Create a password" runat="server"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="ddlRole"><i class="fas fa-user-tag"></i> Role</label>
                        <div class="input-wrapper">
                            <i class="fas fa-user-tag"></i>
                            <asp:DropDownList ID="ddlRole" CssClass="form-select" runat="server">
                                <asp:ListItem Text="Select Role" Value="" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Employee" Value="Employee"></asp:ListItem>
                                <asp:ListItem Text="Manager" Value="Manager"></asp:ListItem>
                                <asp:ListItem Text="HR" Value="HR"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="txtDepartment"><i class="fas fa-building"></i> Department</label>
                        <div class="input-wrapper">
                            <i class="fas fa-building"></i>
                            <asp:TextBox ID="txtDepartment" CssClass="form-control" placeholder="Enter your department" runat="server"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <asp:Button ID="btnSignUp" CssClass="btn-signup" Text="Create Account" OnClick="btnSignUp_Click" runat="server" />

                <div class="login-link">
                    Already have an account? <a href="Login.aspx">Sign In</a>
                </div>
            </div>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
