<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestForm.aspx.cs" Inherits="EU.CqrXs.Srv.Settings.TestForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CqrJd Testform</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div>
                <span style="display: block; border-style: outset; border-width: 1px; border-color: azure; font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif; font-size: larger">
                    ServerIPv4: <asp:Literal ID="LiteralServerIPv4" runat="server"></asp:Literal>
                </span>
                <br />
                <span style="display: block; border-style: outset; border-width: 1px; border-color: azure; font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif; font-size: larger">
                    ServerIPv6: <asp:Literal ID="LiteralServerIPv6" runat="server"></asp:Literal>
                </span>
                <br />
                <span style="display: block; border-style: outset; border-width: 1px; border-color: blanchedalmond; font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif; font-size: larger">
                    Client Ip: <asp:Literal ID="LiteralClientIp" runat="server"></asp:Literal>
                </span>
                <br />
                <span style="display: block; border-style: outset; border-width: 1px; border-color: blanchedalmond; font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif; font-size: larger">
                   🔑 Sec-Key: <asp:TextBox ID="TextBoxKey" runat="server" AutoPostBack="true" ToolTip="Enter secret key here" OnTextChanged="TextBoxKey_TextChanged"></asp:TextBox>
                </span>
                <span style="display: block; border-style: outset; border-width: 1px; border-color: blanchedalmond; font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif; font-size: larger">
                    🔑 <asp:CheckBox ID="CheckBoxDecrypt" runat="server" ToolTip="check to decrypt" Text="Decrypt" Checked="false" />
                </span>
                <span style="display: block; border-style: outset; border-width: 1px; border-color: blanchedalmond; font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif;font-size: larger">
                    <asp:TextBox ID="TextBoxSource" runat="server" TextMode="MultiLine" MaxLength="65536" Rows="10" Columns="48" ValidateRequestMode="Enabled" ToolTip="TextBox source" Text="" Width="480px" />
                </span>                
            </div>
            <hr />
            <div>
                pipe #hash: <asp:TextBox ID="TextBoxPipeHash" runat="server" ToolTip="encrypt pipe hash" ReadOnly="true" Text="" />
                <br />
                <asp:TextBox ID="TextBoxEnDeCrypted" runat="server"  ReadOnly="true" TextMode="MultiLine" MaxLength="65536" Rows="10" Columns="48" ValidateRequestMode="Enabled" ToolTip="TextBox destination" Text="" Width="480px"></asp:TextBox>
                <br />                               
                <br />
                <asp:Button ID="Buttonclear" runat="server" Text="Clear" ToolTip="Clear" OnClick="Buttonclear_Click" />
                <asp:Button ID="ButtonSubmit" runat="server" Text="Submit" ToolTip="Submit" OnClick="ButtonSubmit_Click" />
            </div>
            <hr />
            <pre id="preOut" runat="server" style="font-size:small; width: 640px; height: 180px; max-height: 192px; border-block-color: palevioletred;">

            </pre>           
        </div>
    </form>
</body>
</html>
