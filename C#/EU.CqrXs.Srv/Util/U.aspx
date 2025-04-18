<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="U.aspx.cs" Inherits="EU.CqrXs.Srv.Util.U" %>
<%@ Register TagPrefix="uc" TagName="ImageFontControl" Src="~/Util/ImageFontControl.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="header" runat="server">
    <title id="title" runat="server" title="My-IPAddr" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Literal ID="literalUserHost" runat="server"></asp:Literal>
            <bt />
            <uc:ImageFontControl ID="imageFontControl" runat="server" />
            <hr />
        </div>        
    </form>
</body>
</html>

