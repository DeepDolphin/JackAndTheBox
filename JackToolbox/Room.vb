Imports System.Xml
Imports JackPhysics
Imports System.Drawing

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

    Public RoomWidth As Double
    Public RoomHeight As Double
    Public Const RoomBuffer As Double = 48

    Public WallBrush As TextureBrush
    Public GroundBrush As TextureBrush

    Public WallMap As Bitmap
    Public GroundMap As Bitmap

    Public GraphicsMap As Bitmap
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

        RoomWidth = RoomElement.GetAttribute("Width")
        RoomHeight = RoomElement.GetAttribute("Height")

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

        WallMap = New Bitmap(CInt(RoomWidth), 32)
        GroundMap = New Bitmap(CInt(RoomWidth), CInt(RoomHeight))
        GraphicsMap = New Bitmap(CInt(RoomWidth), CInt(RoomHeight + 32))

        Graphics = Graphics.FromImage(GraphicsMap)
        GroundBrush = New TextureBrush(Resources.getNewBitmap("FloorTile"))
        WallBrush = New TextureBrush(Resources.getNewBitmap("WallStrip"))

        Dim WallGraphics As Graphics = Graphics.FromImage(WallMap)
        Dim GroundGraphics As Graphics = Graphics.FromImage(GroundMap)

        WallGraphics.FillRectangle(WallBrush, 0, 0, CIntFloor({Width}), 32)
        WallGraphics.DrawImage(Resources.getNewBitmap("GradientLeft"), 0, 0, 64, 32)
        WallGraphics.DrawImage(Resources.getNewBitmap("GradientRight"), CIntFloor({Width - 63}), 0, 64, 32)

        GroundGraphics.FillRectangle(GroundBrush, 0, 0, CIntFloor({Width}), CIntFloor({Height}))

        Graphics.DrawImage(WallMap, 0, 0)
        Graphics.DrawImage(GroundMap, 0, 32)
    End Sub

    Public Sub New(GroundTexture As Bitmap, WallTexture As Bitmap, GameObjects As IEnumerable(Of GameObject), Objectives As IEnumerable(Of Objective))




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

    Public Sub DrawBackground(Rect As Rectangle)

        'ToDo: weird drawing glitch

        If (Rect.Y <= 32) Then
            Graphics.DrawImage(WallMap, Rect, Rect, GraphicsUnit.Pixel)
            Graphics.DrawImage(GroundMap, Rect, New Rectangle(Rect.X, Rect.Y - 32, Rect.Width, Rect.Height), GraphicsUnit.Pixel)
        Else
            Graphics.DrawImage(GroundMap, Rect, New Rectangle(Rect.X, Rect.Y - 32, Rect.Width, Rect.Height), GraphicsUnit.Pixel)
        End If

        'Graphics.FillRectangle(WallBrush, 0, 0, CIntFloor({Width}), 32)
        'Graphics.FillRectangle(GroundBrush, 0, 32, CIntFloor({Width}), CIntFloor({Height}))
        'Graphics.DrawImage(Resources.GradientLeft, 0, 0, 64, 32)
        'Graphics.DrawImage(Resources.GradientRight, CIntFloor({Width - 63}), 0, 64, 32)
    End Sub

    ''' <summary>
    ''' Draw the room
    ''' </summary>
    ''' <param name="DrawRoom">True if we should draw the room contents, false if we should draw the room shaded</param>
    Public Sub Redraw(DrawRoom As Boolean)
        If DrawRoom Then
            ' Draw the background behind the last position
            For Each O As GameObject In GameObjects
                If O.Properties.Dirty Then
                    O.Redraw()
                    Dim LastRect As New Rectangle(CIntFloor({O.LastPosition.X}),
                                       CIntFloor({O.LastPosition.Y, O.LastPosition.Z * (10 / 16), 32}),
                                       O.GraphicsMap.Width + 1,
                                       O.GraphicsMap.Height + 1)
                    DrawBackground(LastRect)
                End If
            Next

            For Each O As GameObject In GameObjects
                ' Draw the object at the current position
#If VersionType = "Debug" Then
                Dim hitboxPen As Pen = Pens.Red
                Dim bitmapPen As Pen = Pens.Blue
                If (O.Properties.Collided) Then
                    hitboxPen = Pens.PaleVioletRed
                    O.Properties.Collided = False
                End If
                If (O.Properties.Dirty) Then
                    bitmapPen = Pens.LightBlue
                End If
#End If

                If O.Properties.Dirty Then
                    Graphics.DrawImage(O.GraphicsMap,
                                       CIntFloor({O.Position.X}),
                                       CIntFloor({O.Position.Y, O.Position.Z * (10 / 16), 32}))
                    O.Properties.Dirty = False
                End If

#If VersionType = "Debug" Then
                Graphics.DrawRectangle(hitboxPen,
                                       CIntFloor({O.Position.X, O.HitBox.X}),
                                       CIntFloor({O.Position.Y, O.Position.Z * (10 / 16), O.HitBox.Y, 32}),
                                       O.HitBox.Width,
                                       O.HitBox.Height)
                Graphics.DrawRectangle(bitmapPen,
                                       CIntFloor({O.Position.X}),
                                       CIntFloor({O.Position.Y, O.Position.Z * (10 / 16), 32}),
                                       O.GraphicsMap.Width,
                                       O.GraphicsMap.Height)
#End If
            Next
        Else

            'ToDo: draw room only once if it's shaded
            'Graphics.DrawImage(WallMap, 0, 0)
            'Graphics.DrawImage(GroundMap, 0, 32)
            'Graphics.FillRectangle(Resources.ShadeBrush, 0, 0, CIntFloor({Width}), CIntFloor({Height}) + 32)
        End If
#If Not VersionType = "Release" Then
        Graphics.DrawString(IO.Path.GetFileName(Filename), SystemFonts.CaptionFont, Brushes.Red, 0, 0)
#End If
    End Sub

End Class
