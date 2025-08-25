<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestLogin.aspx.cs" Inherits="EU.CqrXs.Srv.Settings.TestLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CqrJd Testform</title>
    <script type="text/javascript">

        const showButton = document.getElementById("showDialog");
        const spanDialog = document.getElementById("dialogSpan");
        const favDialog = document.getElementById("favDialog");
        const outputBox = document.querySelector("output");
        const selectEl = favDialog.querySelector("select#dialogSelectReason");
        const confirmBtn;
        try {
            confirmBtn = favDialog.querySelector("button#dialogConfirmButton");
        } catch (ex0) {
            console.log("Exception " + ex0 + " on favDialog.querySelector(\"#dialogConfirmButton\")");
        }
        try {
            confirmBtn = favDialog.getElementById("dialogConfirmButton");
        } catch (ex1) {
            console.log("Exception " + ex1 + " on favDialog.getElementById("(\"dialogConfirmButton\")");
        }
        const userName = favDialog.querySelector("input#dialogUserName");
        const passWord = favDialog.querySelector("#dialogPassword");

        //window.onload = addListeners();

        //function addListeners() {
            // "Show the dialog" button opens the <dialog> modally
            showButton.addEventListener("click", () => {
                favDialog.showModal();
            });

            // "Cancel" button closes the dialog without submitting because of [formmethod="dialog"], triggering a close event.
            favDialog.addEventListener("close", (e) => {
                outputBox.value =
                    favDialog.returnValue === "default"
                        ? "No return value."
                        : `ReturnValue: ${favDialog.returnValue}.`; // Have to check for "default" rather than empty string
            });

            // Prevent the "confirm" button from the default behavior of submitting the form, and close the dialog with the `close()` method, which triggers the "close" event.
            confirmBtn.addEventListener("click", (event) => {
                event.preventDefault(); // We don't want to submit this fake form
                favDialog.close(selectEl.value); // Have to send the select box value here.
            });

            // "Cancel" button closes the dialog without submitting because of[formmethod = "dialog"], triggering a close event.
            //favDialog.addEventListener("close", (e) => {
            //    outputBox.value = `ReturnValue: ${favDialog.returnValue}.`; // Have to check for "default" rather than empty string
            //});
            
        //}

        function showModalDialog() {
            if (favDialog == null)
                favDialog = document.getElementById("favDialog");
            favDialog.showModal();
        }

        function closeDialog() {
            if (favDialog == null)
                favDialog = document.getElementById("favDialog");

            var outDiaText = "reason:" + selectEl.value + ";user:" + userName + ";pass:" + passWord + ";";

            console.log(outDiaText);
            // spanDialog.innerText = outDiaText;
            outputBox.value = outDiaText;
            favDialog.closeDialog();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div style="display: block; border-style: outset; border-width: 2px; border-color: azure; max-width: 640px; width: 444px; font-size: larger; font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif;  padding: 2px 2px 2px 2px; margin: 2px 2px 2px 2px;">
                <span style="display: block; border-style: outset; border-width: 1px; border-color: blanchedalmond;">
                    &#x1F464; Username: <asp:TextBox ID="TextBoxUserName" runat="server" AutoPostBack="false" ToolTip="Enter username here" 
                        Width="240px" MaxLength="32" Height="25px"></asp:TextBox>
                </span>
                <span style="display: block; border-style: outset; border-width: 1px; border-color: blanchedalmond;">
                    &#x1F511;&nbsp; Password: <asp:TextBox ID="TextBoxPassword" runat="server" AutoPostBack="false" TextMode="Password" ToolTip="Enter secret password here" 
                        Width="240px" MaxLength="32" Height="25px"></asp:TextBox>
                </span>
                <span style="display: block; border-style: outset; border-width: 1px; border-color: blanchedalmond;">
                    <asp:CheckBox ID="CheckBoxKeepSignIn" runat="server" ToolTip="check to keep sign in" Text="Keep sign in" Checked="false" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="ButtonLogin" runat="server" Text="Login" ToolTip="Login" OnClick="ButtonLogin_Click"  Font-Size="Large" Font-Names="Geneva, Verdana, sans-serif" BorderStyle="Outset"  />
                </span>                
            </div>
            <hr /> 
            <pre id="preOut" runat="server" style="font-size:small; width: 640px; height: 180px; max-height: 192px; border-block-color: palevioletred;">

            </pre>           
        </div>
    </form>
    <!-- A modal dialog containing a form -->
    <dialog id="favDialog">
        <form method="dialog">
            <p>
                <label>
                    Username:
                    <input id="dialogUserName" type="text" required />
                </label>
                <label>
                    Password:
                    <input id="dialogPassword" type="password" required />
                </label>
                <label>
                    Reason for login:
                    <select id="dialogSelectReason">
                        <option value="default">Choose…</option>
                        <option>Admin</option>
                        <option>Test</option>                    
                        <option>Visitor</option>
                    </select>
                </label>
            </p>
            <div>
                <button value="cancel" formmethod="dialog">Cancel</button>
                <button id="dialogConfirmButton" value="default">Confirm</button>
            </div>
        </form>
    </dialog>
    <p>
        <button id="showDialog" onclick="javascript:favDialog.showModal(); return false;">Show the dialog</button>
    </p>
    <!-- span id="dialogSpan"></!-- -->
    <output></output>    
</body>
</html>
