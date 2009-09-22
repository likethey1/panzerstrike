<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserInformation.aspx.cs" Inherits="UserInformation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
            DeleteMethod="Delete" InsertMethod="Insert" 
            OldValuesParameterFormatString="original_{0}" 
            SelectMethod="GetPowerUpByUserName" 
            TypeName="PlayerStatsDataSetTableAdapters.PUpTableTableAdapter" 
            UpdateMethod="Update">
            <DeleteParameters>
                <asp:Parameter Name="Original_PUpID" Type="Int32" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="Name" Type="String" />
                <asp:Parameter Name="Description" Type="String" />
                <asp:Parameter Name="Image" Type="String" />
                <asp:Parameter Name="NextLevelCost" Type="Int32" />
                <asp:Parameter Name="NextLevelBenefit" Type="Int32" />
                <asp:Parameter Name="Effects" Type="Int32" />
                <asp:Parameter Name="Original_PUpID" Type="Int32" />
            </UpdateParameters>
            <SelectParameters>
                <asp:SessionParameter DefaultValue="" Name="userName" SessionField="userName" 
                    Type="String" />
            </SelectParameters>
            <InsertParameters>
                <asp:Parameter Name="Name" Type="String" />
                <asp:Parameter Name="Description" Type="String" />
                <asp:Parameter Name="Image" Type="String" />
                <asp:Parameter Name="NextLevelCost" Type="Int32" />
                <asp:Parameter Name="NextLevelBenefit" Type="Int32" />
                <asp:Parameter Name="Effects" Type="Int32" />
            </InsertParameters>
        </asp:ObjectDataSource>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            DataKeyNames="PUpID" DataSourceID="ObjectDataSource1" Height="127px" 
            Width="210px">
            <Columns>
                <asp:BoundField DataField="PUpID" HeaderText="PUpID" InsertVisible="False" 
                    ReadOnly="True" SortExpression="PUpID" />
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                <asp:BoundField DataField="Description" HeaderText="Description" 
                    SortExpression="Description" />
                <asp:BoundField DataField="Image" HeaderText="Image" SortExpression="Image" />
                <asp:BoundField DataField="NextLevelCost" HeaderText="NextLevelCost" 
                    SortExpression="NextLevelCost" />
                <asp:BoundField DataField="NextLevelBenefit" HeaderText="NextLevelBenefit" 
                    SortExpression="NextLevelBenefit" />
                <asp:BoundField DataField="Effects" HeaderText="Effects" 
                    SortExpression="Effects" />
            </Columns>
        </asp:GridView>
    &nbsp;&nbsp;&nbsp;
        <p style="margin-left: 80px">
            <br />
        </p>
        <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" 
            DeleteMethod="Delete" InsertMethod="Insert" 
            OldValuesParameterFormatString="original_{0}" SelectMethod="GetDataByUserName" 
            TypeName="PlayerStatsDataSetTableAdapters.StatsTableAdapter" 
            UpdateMethod="Update">
            <DeleteParameters>
                <asp:Parameter Name="Original_StatsID" Type="Int32" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="Frags" Type="Int32" />
                <asp:Parameter Name="Deaths" Type="Int32" />
                <asp:Parameter Name="Points" Type="Int32" />
                <asp:Parameter Name="Money" Type="Int32" />
                <asp:Parameter Name="Original_StatsID" Type="Int32" />
            </UpdateParameters>
            <SelectParameters>
                <asp:SessionParameter Name="userName" SessionField="username" Type="String" />
            </SelectParameters>
            <InsertParameters>
                <asp:Parameter Name="Frags" Type="Int32" />
                <asp:Parameter Name="Deaths" Type="Int32" />
                <asp:Parameter Name="Points" Type="Int32" />
                <asp:Parameter Name="Money" Type="Int32" />
            </InsertParameters>
        </asp:ObjectDataSource>
        <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
            DataKeyNames="StatsID" DataSourceID="ObjectDataSource2">
            <Columns>
                <asp:BoundField DataField="StatsID" HeaderText="StatsID" InsertVisible="False" 
                    ReadOnly="True" SortExpression="StatsID" />
                <asp:BoundField DataField="Frags" HeaderText="Frags" SortExpression="Frags" />
                <asp:BoundField DataField="Deaths" HeaderText="Deaths" 
                    SortExpression="Deaths" />
                <asp:BoundField DataField="Points" HeaderText="Points" 
                    SortExpression="Points" />
                <asp:BoundField DataField="Money" HeaderText="Money" SortExpression="Money" />
            </Columns>
        </asp:GridView>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
            Text="Sterge-mi contul" />
        <br />
    </div>
    </form>
</body>
</html>
