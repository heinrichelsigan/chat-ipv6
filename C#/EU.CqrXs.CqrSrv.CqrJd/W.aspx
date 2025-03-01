<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="W.aspx.cs" Inherits="EU.CqrXs.CqrSrv.CqrJd.W" %>
<%@ Register TagPrefix="uc" TagName="ImageFontControl" Src="~/ImageFontControl.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="header" runat="server">
    <title id="title" runat="server" title="My-IPAddr" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <uc:ImageFontControl ID="imageFontControl" runat="server" />
            <br />
            <a href="#" id="hrefi" runat="server"></a>
        </div>        
    </form>
</body>
</html>

