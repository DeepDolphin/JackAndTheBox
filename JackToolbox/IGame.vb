Imports JackPhysics
Imports System.Drawing
Imports System.Windows.Forms

Public MustInherit Class IGame

#If VersionType = "Beta" Then
    Public Const VersionTN As String = "1"
#ElseIf VersionType = "Release" Then
    Public Const VersionTN As String = "0"
#ElseIf VersionType = "Debug" Then
    Public Const VersionTN As String = "2"
#End If

    Public Const VersionNumber As String = "1.0.0302." + VersionTN + "000"

    Public Shared ToAddWaitlist As New List(Of GameObject)
    Public Shared Player As Player
    Public Shared ViewOffsetX As Double
    Public Shared ViewOffsetY As Double

    Public Shared World As World
    Public Shared UserInterface As IUserInterface

    Protected Buffer As BufferedGraphics
    Protected GameRunning As Boolean
    Protected ScreenWidth As Integer
    Protected ScreenHeight As Integer

    Protected UpdateCompleteEvent As New System.Threading.AutoResetEvent(False)

    Public ReadOnly Property MaxTick As Double
        Get
            Return 1 / Options.Preferences("MaxFPS")
        End Get
    End Property

    Public ReadOnly Property PlayerRoom As Room
        Get
            Return World.RoomAt(Player.Middle.X + Player.Room.XOffset, Player.Middle.Y + Player.Room.YOffset)
        End Get
    End Property

    Public Sub New(Buffer As BufferedGraphics, ScreenWidth As Integer, ScreenHeight As Integer)
        Me.Buffer = Buffer
        Me.ScreenHeight = ScreenHeight
        Me.ScreenWidth = ScreenWidth
    End Sub

    Public MustOverride Sub Start()
    Protected MustOverride Sub Init()
    Public MustOverride Sub StopGame()
    Protected MustOverride Sub GameLoop()
    Protected MustOverride Sub DrawWorld()

    Protected Watch As Stopwatch
    Protected _tick As Double = 0.5

    Public ReadOnly Property Tick As Double
        Get
            Return _tick
        End Get
    End Property

    Protected MustOverride Sub UpdateWorld(t As Double)

    Public Sub KeyDown(e As KeyEventArgs)
        For Each key As Keys In Options.OIMap.Keys
            If (e.KeyCode = key) Then
                Options.OIStatus(Options.OIMap(key)) = True
            End If
        Next
    End Sub

    Public Sub KeyUp(e As KeyEventArgs)
        For Each key As Keys In Options.OIMap.Keys
            If (e.KeyCode = key) Then
                Options.OIStatus(Options.OIMap(key)) = False
            End If
        Next
    End Sub

    Public Sub MouseDown(e As MouseEventArgs)
        For Each key As Keys In Options.OIMap.Keys
            If (e.Button = key) Then
                Options.OIStatus(Options.OIMap(key)) = True
            End If
        Next
    End Sub

    Public Sub MouseUp(e As MouseEventArgs)
        For Each key As Keys In Options.OIMap.Keys
            If (e.Button = key) Then
                Options.OIStatus(Options.OIMap(key)) = False
            End If
        Next
    End Sub

    Public Sub MouseMove(e As MouseEventArgs)
        'Dim direction As Double = Player.getDirectionTo(e.Location + New Size(ViewOffsetX, ViewOffsetY) - New Size(Player.Room.XOffset, Player.Room.YOffset))
        'Player.Direction = Actor.ToActorDirection(direction)
        'Mouse = e.Location + New Size(ViewOffsetX, ViewOffsetY) - New Size(Player.Room.XOffset, Player.Room.YOffset)
    End Sub

    Public Sub MouseWheel(e As MouseEventArgs)
        Options.MouseWheel = e.Delta / 120
    End Sub

    Public Sub Resize(sender As Control)
        'ToDo: make sure bitmap resizes and the objects redraw | User interface has to be updated as well
        ScreenWidth = sender.ClientSize.Width
        ScreenHeight = sender.ClientSize.Height
    End Sub
End Class
