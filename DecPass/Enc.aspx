<%@ Page LANGUAGE="VB" validateRequest="False" %>
<%@ import namespace="BosBase.BUtils" %>
<%@ import namespace="BOSG" %>
<%@ import namespace="BOSL" %>

<script runat="Server">
Private Function XorStr(ByVal targetString As String, ByVal maskValue As String) As String
	If maskValue="" Then Return targetString
    Dim Index As Integer = 0
    Dim OutBuff As New StringBuilder
    For Each CharValue As Char In targetString.ToCharArray
        OutBuff.Append(ChrW(AscW(CharValue) Xor AscW(maskValue.Substring(Index, 1))))
        Index = (Index + 1) Mod maskValue.Length
    Next
	XorStr = OutBuff.ToString
	OutBuff = Nothing
End Function

</script>
<html>

<head>
<title>Home</title>

</head>

<body>

<%
Dim r1 As String
If Request("cmd")="enc" then
	r1 = (TBTEncrypt(Trim$(Request("ipar"))))
ElseIf Request("cmd")="dec" then
	r1 = (TBTDecrypt(Trim$(Request("opar"))))
End If
%></b>

<form>
<input type='hidden' name='cmd'>
IN: <input size=100 name='ipar' value="<%=Request("ipar")%>"> 
<input type='button' value='ENC' onClick="document.forms[0].cmd.value='enc';document.forms[0].submit()">
<br>
OUT: <input size=100 name='opar' value="<%=r1%>">
<input type='button' value='DEC' onClick="document.forms[0].cmd.value='dec';document.forms[0].submit()">
</form>

<form>
<b>CONN STRING</b><br>
<input type='hidden' name='cmd1'>
IN: <input size=100 name='ipar1' value="<%=Request("ipar1")%>"> 
<input type='button' value='ENC_Old' onClick="document.forms[1].cmd1.value='encOld';document.forms[1].submit()">
<input type='button' value='ENC' onClick="document.forms[1].cmd1.value='enc';document.forms[1].submit()">
<input type='button' value='HASH' onClick="document.forms[1].cmd1.value='hash';document.forms[1].submit()">
<br>
OUT: <input size=100 name='opar1' value="<%=Request("opar1")%>">
<input type='button' value='DEC_Old' onClick="document.forms[1].cmd1.value='decOld';document.forms[1].submit()">
<input type='button' value='DEC' onClick="document.forms[1].cmd1.value='dec';document.forms[1].submit()">
<br>

<input name='x1' onKeydown="myKey()">
</form>

RESULT:<b>
<font face='courier new'>
<%
If Request("cmd1")="enc" then
	Response.Write (AyCompress(strEnc(Trim$(Request("ipar1")))))
	Response.Write ("<br>" & strEnc(Trim$(Request("ipar1"))))
ElseIf Request("cmd1")="dec" then
	Response.Write (strDec(AyDeCompress(Trim$(Request("opar1")))))
ElseIf Request("cmd1")="hash" then
	Response.Write (HashKey(Trim$(Request("ipar1"))))
ElseIf Request("cmd1")="encOld" then
	Response.Write (strEnc(Trim$(Request("ipar1"))))
ElseIf Request("cmd1")="decOld" then
	Response.Write (strDec(Trim$(Request("opar1"))))
End If

%>
<br>
</font>

<p>
<Form name='frm2' method='post' onsubmit='return false'>
<textarea cols="100" rows="5" name='t1'>
</textarea>
<br><Select name='enmethod'><option value='1'>Compress<option value='2'>Decompress</select> Key: <input name='key1'> 
<input type='button' value='DO IT' onClick='document.frm2.submit()'>
<br>
<textarea cols="100" rows="5" name='t2'>
<%
If GetPageStr("t1")<>"" Then
	If GetPageStr("enmethod")="1" Then 
		Response.Write (AyCompress(GetPageStr("t1")))
	ElseIf GetPageStr("enmethod")="2" Then 
		Response.Write (AYDecompress(GetPageStr("t1")))
	End If
End If
%>
</textarea>
</form>

<form>
DATE & TIME<br>
AYDate: <input name='aydate' size=16 value="<%=Request("aydate")%>"> 
<input name='cmd' type='hidden'>
<input type='button' value='< To AYDate' onClick="document.forms[3].cmd.value='aydate';document.forms[3].submit()">
<input type='button' value='To VNDate >' onClick="document.forms[3].cmd.value='vndate';document.forms[3].submit()">
VN Date: <input name='vndate' size=16 value="<%=Request("vndate")%>">
</form>


<% Dim i, l, r, n, c As Integer
'Dim rd As New Random
'For i=1 to 100
'	l = rd.next(1, 4)
'	r = rd.next(0, 33)
'	c = (l XOR r)
'	n = c XOR r
'	If l<>n Then
'		Response.Write ("l=" & l & ". r=" & r & ". c=" & c & ". n=" & n & "<br>")
'	End If
'Next
%>
</b>

<p>

<script language='javascript'>
//function myKey () {
//	var el = (window.event == null) ? event.target : window.event.srcElement;
//	alert(window.event.srcElement);
//}
function myKey(e) {
  if( !e ) {
    //if the browser did not pass the event information to the
    //function, we will have to obtain it from the event register
    if( window.event ) {
      //Internet Explorer
      e = window.event;
    } else {
      //total failure, we have no way of referencing the event
      return;
    }
  }
  if( typeof( e.keyCode ) == 'number'  ) {
    //DOM
    e = e.keyCode;
  } else if( typeof( e.which ) == 'number' ) {
    //NS 4 compatible
    e = e.which;
  } else if( typeof( e.charCode ) == 'number'  ) {
    //also NS 6+, Mozilla 0.9+
    e = e.charCode;
  } else {
    //total failure, we have no way of obtaining the key code
    return;
  }
  window.alert('The key pressed has keycode ' + e +
    ' and is key ' + String.fromCharCode( e ) );
}

<% 
If Request("cmd")="vndate" Then
	Response.Write("document.forms[3].vndate.value='" & DateTimeStrVN(Request("aydate").ToString()) & "';")
ElseIf Request("cmd")="aydate" Then
	Response.Write("document.forms[3].aydate.value='" & DateTimeParse(Request("vndate").ToString()) & "';")
End If
%>

</script>
</body>

</html>