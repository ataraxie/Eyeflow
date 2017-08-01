^a::
	InputBox, textInput, , Which window?, , 200, 100
	WinGet windows, List
	listString = ""
	Loop %windows%
	{
		thisWindowId := windows%A_Index%
		WinGetTitle windowTitle, ahk_id %thisWindowId%
		StringLower, windowTitle, windowTitle
		IfInString, windowTitle, %textInput%
		{
			ActivateWindow(thisWindowId)
		}
		listString .= windowTitle . "`n"
	}
	if ("list" = textInput) {
		MsgBox %listString%
	}
Return

^y::
	WinGet windows, List
	listString = ""
	Loop %windows%
	{
		thisWindowId := windows%A_Index%
		WinGetTitle windowTitle, ahk_id %thisWindowId%
		WinGet, processId, PID, ahk_id %thisWindowId%
		WinGetClass, winClass, ahk_id %thisWindowId%
		StringLower, windowTitle, windowTitle
		listString .= windowTitle . " - " . processId . " - " . winClass . "`n"
	}
	MsgBox %listString%
Return

;^s::
;	SetTimer, ActivateWinUM, 100
;return

^q::
	WinGet windows, List
	Loop %windows%
	{
		id := windows%A_Index%
		WinGetTitle windowTitle, ahk_id %id%
		WinSet, Transparent, 255, ahk_id %id%
	}
Return

^w::Reload

currentWindowId = ""

ActivateWinUM:
	MouseGetPos,,, cursorWindowId
	if (currentWindowId != cursorWindowId) {
		ActivateWindow(cursorWindowId)
	}
return

ActivateWindow(targetWindowId) {
	global currentWindowId
	currentWindowId = targetWindowId
	WinGet windows, List
	Loop %windows% {
		id := windows%A_Index%
		if (id = targetWindowId) {
			WinSet, Transparent, 255, ahk_id %id%
		} else {
			WinSet, Transparent, 50, ahk_id %id%
		}
	}
}
