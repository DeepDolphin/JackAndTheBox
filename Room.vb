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
        ResortGameObjects()
    End Sub

    Public Sub RemoveGameObject(O As GameObject)
        GameObjects.Remove(O)
        ResortGameObjects()
    End Sub

    Public Sub RemoveGameObject(index As Integer)
        GameObjects.RemoveAt(index)
        ResortGameObjects()
    End Sub


    Public Sub ResortGameObjects()
        Dim l As New List(Of GameObject)
        While GameObjects.Count > 0
            ' find the smallest y
            Dim b As New List(Of GameObject)
            Dim record As Integer = Integer.MaxValue
            For Each o As GameObject In GameObjects
                If o.Position.Y - o.HitBox.Height + (o.Position.Z * o.HitBox.Height * (10 / 16)) < record Then
                    b.Clear()
                    record = o.Position.Y - o.HitBox.Height + (o.Position.Z * o.HitBox.Height * (10 / 16))
                    b.Add(o)
                ElseIf o.Position.Y - o.HitBox.Height + (o.Position.Z * o.HitBox.Height * (10 / 16)) = record
                    b.Add(o)
                End If
            Next
            For Each o As GameObject In b
                GameObjects.Remove(o)
                l.Add(o)
            Next
        End While

        GameObjects = l
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
    End Sub

    Public Overrides Function ToString() As String
        Return IO.Path.GetFileName(Filename) & " X:" & XOffset & ", Y:" & YOffset & " GameObjects:" & GameObjects.Count
    End Function
End Class
