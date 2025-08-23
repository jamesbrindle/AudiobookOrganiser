Dim objShell
Set objShell=CreateObject("WScript.Shell")
strCMD="powershell -sta -noProfile -nologo -command C:\Utilities\AudiobookOrganiser\AudiobookOrganiser.exe -update-tags-only"
objShell.Run strCMD,0