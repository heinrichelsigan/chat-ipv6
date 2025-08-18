<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EU.CqrXs.Service.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CqrService v1.2</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta id="metaRefreshId"  runat="server" http-equiv="refresh"  content="8; url=CqrService.asmx" />    
</head>
<body>
    <form id="form1" runat="server">
        <div style="font-size: large; background-color: antiquewhite">
            Redirecting to <a id="aHrefId" runat="server" href="https://srv.cqrxs.eu/v1.0/CqrService.asmx" target="_top">CqrService.asmx</a> ...
        </div>
        <div id="DivInvisible" runat="server">            
            <asp:Literal ID="LiteralHtmlCommentBegin" runat="server" Text="<!--" ></asp:Literal>

            <span id="SpanInfo" runat="server">                                   
            </span>
            <asp:Literal ID="LiteralHtmlCommentEnd" runat="server" Text="-->" ></asp:Literal>
        </div> 
    </form>
</body>
</html>
