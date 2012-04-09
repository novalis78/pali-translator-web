<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Translated.aspx.cs" Inherits="PaliTranslatorWeb.Translated" %>
<%
    foreach(string s in wordAnalysisList)
    {
        %><%=s%>
        <%} %>
<hr />
<%=resultHtml%>