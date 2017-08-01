ShowWindow(processId) {
	winId := GetWinIdFromPid(processId)
	WinSet, Transparent, 255, ahk_id %winId%
}

ShowActiveWindow() {
	WinGet, activeId, ID, A
	WinSet, Transparent, 255, ahk_id %activeId%
}

HideWindow(processId) {
	winId := GetWinIdFromPid(processId)
	WinSet, Transparent, 50, ahk_id %winId%
}

GetWinIdFromPid(processId) {
	pidStr := "ahk_pid " . processId
    WinGet, hWnd, ID, %pidStr%
	return %hWnd%
}

Temp() {
	WinGet, activeId, ID, A
	WinGetTitle, windowTitle, ahk_id %activeId%
	MsgBox, %windowTitle%
}

HandleWindowsByCoords(x, y) {
	winId := DllCall("WindowFromPoint", Int,x, Int,y)
	ActivateWindow(winId)
}

ActivateWindow(targetWindowId) {
	global currentWindowId
	if (currentWindowId != targetWindowId) {
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
}