<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Administrator.aspx.cs" Inherits="Default2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" InsertMethod="Insert" 
        OldValuesParameterFormatString="original_{0}" SelectMethod="GetDataBy" 
        TypeName="UsersDataSetTableAdapters.aspnet_UsersTableAdapter" 
        UpdateMethod="Update">
        <UpdateParameters>
            <asp:Parameter DbType="Guid" Name="UserId" />
            <asp:Parameter Name="UserName" Type="String" />
            <asp:Parameter Name="MobileAlias" Type="String" />
            <asp:Parameter Name="IsAnonymous" Type="Boolean" />
            <asp:Parameter Name="LastActivityDate" Type="DateTime" />
            <asp:Parameter DbType="Guid" Name="Original_ApplicationId" />
            <asp:Parameter Name="Original_LoweredUserName" Type="String" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter DbType="Guid" Name="ApplicationId" />
            <asp:Parameter DbType="Guid" Name="UserId" />
            <asp:Parameter Name="UserName" Type="String" />
            <asp:Parameter Name="LoweredUserName" Type="String" />
            <asp:Parameter Name="MobileAlias" Type="String" />
            <asp:Parameter Name="IsAnonymous" Type="Boolean" />
            <asp:Parameter Name="LastActivityDate" Type="DateTime" />
        </InsertParameters>
    </asp:ObjectDataSource>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        DataKeyNames="ApplicationId,LoweredUserName" DataSourceID="ObjectDataSource1" 
        onselectedindexchanged="GridView1_SelectedIndexChanged">
        <Columns>
            <asp:BoundField DataField="ApplicationId" HeaderText="ApplicationId" 
                ReadOnly="True" SortExpression="ApplicationId" Visible="False" />
            <asp:BoundField DataField="UserId" HeaderText="UserId" 
                SortExpression="UserId" Visible="False" />
            <asp:BoundField DataField="UserName" HeaderText="UserName" 
                SortExpression="UserName" />
            <asp:BoundField DataField="LoweredUserName" HeaderText="LoweredUserName" 
                ReadOnly="True" SortExpression="LoweredUserName" />
            <asp:BoundField DataField="MobileAlias" HeaderText="MobileAlias" 
                SortExpression="MobileAlias" />
            <asp:CheckBoxField DataField="IsAnonymous" HeaderText="IsAnonymous" 
                SortExpression="IsAnonymous" />
            <asp:BoundField DataField="LastActivityDate" HeaderText="LastActivityDate" 
                SortExpression="LastActivityDate" />
        </Columns>
    </asp:GridView>
    <div>
    
        <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" 
            DataSourceID="ObjectDataSource1" DataTextField="UserName" 
            DataValueField="UserName">
        </asp:DropDownList>
    
        <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="True" 
            onselectedindexchanged="DropDownList2_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:DropDownList ID="DropDownList3" runat="server">
        </asp:DropDownList>
    
    </div>
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
        Text="Sterge userul selectat" />
    <asp:Button ID="Button2" runat="server" onclick="Button2_Click" 
        Text="Vezi pagina userului selectat" />
    <asp:Button ID="Button3" runat="server" onclick="Button3_Click" 
        Text="Blocheaza cont" />
    <asp:Button ID="Button4" runat="server" onclick="Button4_Click" 
        Text="Deblocheaza cont" />
    </form>
</body>
</html>
