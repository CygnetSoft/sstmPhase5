<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocumentViewer.aspx.cs" Inherits="SSTM.Views.DocViewer.DocumentViewer" %>
<%@ Register Assembly="WebFormsDocumentViewer" Namespace="WebFormsDocumentViewer" TagPrefix="cc" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <cc:DocumentViewer runat="server" ID="viewer" Width="100%" Height="700" TempDirectoryPath="~/TempFiles" />
        </div>
    </form>
</body>
</html>
