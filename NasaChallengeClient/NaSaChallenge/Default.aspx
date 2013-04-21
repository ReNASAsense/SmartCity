<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="NaSaChallenge._Default" %>
    
<%@ Register assembly="Artem.Google" namespace="Artem.Google.UI" tagprefix="artem" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    </asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Welcome to nasa challenge!
    </h2>
    <p />
    <form id="formData">
    <div>
       <asp:Table ID="Table1" runat="server">
       <asp:TableRow>
          <asp:TableCell><asp:GridView ID="gvData" runat="server"/></asp:TableCell>
        </asp:TableRow>
      </asp:Table>
    </div>
    </form>
    <p>
      <h2>Google Map</h2>
      <asp:ScriptManager ID="ScriptManager1" runat="server">
      </asp:ScriptManager>
      <artem:GoogleMap ID="GoogleMap1" runat="server" 
                       Width="800px" Height="600px" 
                       Key="AIzaSyCXbHqNPuwAVl6cYiLRPAicAqL8OBs3WqU"
                       Latitude="42.1229" Longitude="24.7879" Zoom="4">
      </artem:GoogleMap>
    </p>
</asp:Content>
