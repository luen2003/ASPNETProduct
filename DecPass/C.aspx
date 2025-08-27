<%@ Page LANGUAGE="VB" validateRequest="False" %>
<%@ import namespace="BosBase.BUtils" %>


<html>

<head>
<title>Home</title>

</head>

<body>
<table><tr><td valign=top>
<form method='POST'>
<textarea cols=80, rows=30 name='inp'></textarea>
<input type='submit'>

<td valign=top>
<textarea cols=80, rows=30 name='outp'>
<% Dim x = Request("inp")
If x<>"" Then
	x = Replace$(x, "[", "")
	x = Replace$(x, "]", "")
	x = Replace$(x, "smallint", "NUMBER(8)", 1)
	x = Replace$(x, "int", "NUMBER(18)", 1)
	x = Replace$(x, "IDENTITY(1,1)", "PRIMARY KEY", 1)
	x = Replace$(x, "nvarchar(", "varchar2(") '// to be usedback as nvarchar
	x = Replace$(x, "varchar(", "VARCHAR2(")
	x = Replace$(x, "datetime", "TIMESTAMP(3)")
	x = Replace$(x, "numeric(", "NUMBER(")
	Response.Write (x)
End If
%>
</textarea>
</form>
</table>

<script language='javascript'>
document.forms(0).outp.select();
</script>

</body>

</html>