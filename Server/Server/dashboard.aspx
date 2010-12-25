<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="Server.dashboard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
        Current played games: <br />
    <asp:GridView ID="gv_games" runat="server" AutoGenerateColumns="false" AllowSorting="true" EmptyDataText="no games running :(">
        <Columns>
            <asp:BoundField HeaderText="Game ID" DataField="Name" />
            <asp:BoundField HeaderText="Player 1(creator)" DataField="Player1Name" />
            <asp:BoundField HeaderText="Player 2" DataField="Player2Name" />
            <asp:BoundField HeaderText="Player 3" DataField="Player3Name" />
            <asp:BoundField HeaderText="Player 4" DataField="Player4Name" />
            <asp:BoundField HeaderText="Start time" DataField="StartedAt" />
            <asp:BoundField HeaderText="Current Round" DataField="CurrentRound"  />
        </Columns>
    </asp:GridView>

    </div>
    </form>
</body>
</html>
