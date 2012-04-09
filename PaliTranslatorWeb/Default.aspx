<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PaliTranslatorWeb._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.3/jquery.min.js" type="text/javascript"></script>
    
    <title>Pali English Translation Tool</title>
    <style type="text/css">
        #sourcetext
        {
            height: 184px;
            width: 630px;
        }
    </style>
    <script>

        function translate() {
            var text = $('#sourcetext').val();
            //text = escape(text);
            $.post("Translated.aspx", { s: text },
           function(data) {
               $('#result').html(data);
           });
        }
</script>
</head>
<body>
<h1 style="margin-bottom: 2px;">Experimental Pāli - English Translation Tool</h1>

    
    <span style="font-size: 6pt;">&copy;2010 <a href="http://www.nibbanam.com">Nibbanam.com</a></span><br />
    <hr />
    <p style="float: right;">Back to <a href="http://theravada.org">Tipitaka Search </a></p>
    <div>
        <textarea id="sourcetext">Bhagavā dhammaṃ deseti</textarea>
        <br />
        <br />
        <a href="javascript:translate();">Click here to TRANSLATE</a>
        <hr />
        <br />
        &nbsp;<span id="result"></span></div>
    
    <br />
</body>
</html>
