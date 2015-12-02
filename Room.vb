Imports System.Xml

Public Class Room
    Public Enum DoorState
        None = 0
        Closed = 1
        Open
    End Enum

    Public Enum DoorDirection
        Up = 0
        Right
        Down
        Left
    End Enum

    Public Filename As String = ""

    Public GameObjects As New List(Of GameObject)
    Public Objectives As New List(Of Objective)

    Public DoorUp As DoorState = DoorState.None
    Public DoorLeft As DoorState = DoorState.None
    Public DoorDown As DoorState = DoorState.None
    Public DoorRight As DoorState = DoorState.None

    Public RoomUp As Room
    Public RoomLeft As Room
    Public RoomDown As Room
    Public RoomRight As Room

    Public XOffset As Double = 0
    Public YOffset As Double = 0

    Public Const RoomWidth As Double = 500
    Public Const RoomHeight As Double = 300
    Public Const RoomBuffer As Double = 48

    Public WallBrush As TextureBrush
    Public GroundBrush As TextureBrush

    Public GraphicsMap As New Bitmap(CInt(RoomWidth), CInt(RoomHeight + 32))
    Private Graphics As Graphics

    Public ReadOnly Property Width As Double
        Get
            Return RoomWidth
        End Get
    End Property
    Public ReadOnly Property Height As Double
        Get
            Return RoomHeight
        End Get
    End Property

    Public Sub AddGameObject(O As GameObject)
        GameObjects.Add(O)
        GameObjects.Sort()
    End Sub

    Public Sub RemoveGameObject(O As GameObject)
        GameObjects.Remove(O)
        GameObjects.Sort()
    End Sub

    Public Sub RemoveGameObject(index As Integer)
        GameObjects.RemoveAt(index)
        GameObjects.Sort()
    End Sub

    Public Event GameObjectChange()

    Public ReadOnly Property Bounds As RectangleF
        Get
            Return New RectangleF(CSng(XOffset), CSng(YOffset), CSng(Width), CSng(Height))
        End Get
    End Property

    Public Sub New(Filename As String)
        Me.Filename = Filename
        Dim doc As New XmlDocument
        doc.Load(Filename)
        Dim RoomElement As XmlElement = doc("Room")
        DoorUp = IIf(RoomElement.GetAttribute("DoorUp"), DoorState.Closed, DoorState.None)
        DoorDown = IIf(RoomElement.GetAttribute("DoorDown"), DoorState.Closed, DoorState.None)
        DoorLeft = IIf(RoomElement.GetAttribute("DoorLeft"), DoorState.Closed, DoorState.None)
        DoorRight = IIf(RoomElement.GetAttribute("DoorRight"), DoorState.Closed, DoorState.None)
        For Each e As XmlElement In RoomElement
            Select Case e.Name
                Case "Crate"
                    Dim c As New Crate(Me, New Vector3(e.GetAttribute("X"), e.GetAttribute("Y"), 0))
                    GameObjects.Add(c)
                Case "SurvivalTime"
                    Dim o As New SurvivalTimeObjective(e.GetAttribute("Time"))
                    Objectives.Add(o)
                Case "KillAllEnemies"
                    Dim o As New KillAllEnemiesObjective()
                    Objectives.Add(o)
            End Select
        Next

        Graphics = Graphics.FromImage(GraphicsMap)
        GroundBrush = New TextureBrush(My.Resources.FloorTile)
        WallBrush = New TextureBrush(My.Resources.WallStrip)
    End Sub

    Public Overrides Function ToString() As String
        Return IO.Path.GetFileName(Filename) & " X:" & XOffset & ", Y:" & YOffset & " GameObjects:" & GameObjects.Count
    End Function

    Private Function CIntFloor(vals() As Double) As Integer
        Dim sum As Integer
        For Each val As Double In vals
            sum += CInt(Math.Floor(val))
        Next
        Return sum
    End Function

    Public Sub Redraw()
        Graphics.FillRectangle(WallBrush, 0, 0, CIntFloor({Width}), 32)
        Graphics.FillRectangle(GroundBrush, 0, 32, CIntFloor({Width}), CIntFloor({Height}))
        Graphics.DrawImage(MainForm.Resources.GradientLeft, 0, 0, 64, 32)
        Graphics.DrawImage(MainForm.Resources.GradientRight, CIntFloor({Width - 63}), 0, 64, 32)
        If Me.Equals(MainForm.Player.Room) Then

            ' Draw the rest of the game objects
            For Each O As GameObject In GameObjects
                Try
                    O.Redraw()
                    Graphics.DrawImage(O.GraphicsMap, CIntFloor({O.Position.X}), CIntFloor({O.Position.Y, O.Position.Z * (10 / 16), O.HitBox.Y, 32}))

#If VersionType = "Debug" Then
                        r.Graphics.DrawRectangle(Pens.Red,
                                                 CIntFloor({O.Position.X, O.HitBox.X}),
                                                 CIntFloor({O.Position.Y, O.Position.Z * (10 / 16), O.HitBox.Y, 32}),
                                                 O.HitBox.Width,
                                                 O.HitBox.Height)
#End If
                Catch ex As Exception
                    Stop
                End Try
            Next
        Else
            Graphics.FillRectangle(MainForm.Resources.ShadeBrush, 0, 0, CIntFloor({Width}), CIntFloor({Height}) + 32)
        End If
#If Not VersionType = "Release" Then
        Graphics.DrawString(IO.Path.GetFileName(Filename), SystemFonts.CaptionFont, Brushes.Red, 0, 0)
#End If
    End Sub

End Class
