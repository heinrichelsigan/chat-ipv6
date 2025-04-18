<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="W.aspx.cs" Inherits="EU.CqrXs.Srv.Util.W" %>
<%@ Register TagPrefix="uc" TagName="ImageFontControl" Src="~/Util/ImageFontControl.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="header" runat="server">
    <title id="title" runat="server" title="My-IPAddr" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <uc:ImageFontControl ID="imageFontControl" runat="server" />
            <hr />
            <a id="ahrefId" runat="server" href="#" target="_blank" />
        </div>        
    </form>
</body>
</html>

