<%@ Page Language="C#" Title="Tell Me Your Mood" AutoEventWireup="true" MasterPageFile="~/Site.master" 
    CodeBehind="TellMeYourMood.aspx.cs" Inherits="NaSaChallenge.TellMeYourMood" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        How do you feel right now? 
    </h2>
    <p>
      <div>
        <asp:RadioButtonList ID="RadioButtonList1" runat="server">
            <asp:ListItem Text="bored" Selected=True />
            <asp:ListItem Text="happy" />
            <asp:ListItem Text="so so" />
            <asp:ListItem Text="cold"  />
            <asp:ListItem Text="Other:" />
        </asp:RadioButtonList>
        <asp:TextBox ID="TextBox1" Text="Nothing..." runat="server"></asp:TextBox>
      </div>
    </p>
    <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button_Click"/>
</asp:Content>
