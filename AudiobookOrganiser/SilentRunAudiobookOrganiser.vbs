Dim objShell
Set objShell=CreateObject("WScript.Shell")
strCMD="powershell -sta -noProfile -nologo -command C:\Utilities\AudiobookOrganiser\AudiobookOrganiser.exe -audible-sync"
objShell.Run strCMD,0