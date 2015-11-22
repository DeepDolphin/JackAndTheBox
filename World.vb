#Const VersionType = "Beta"

Public Class World
    Public Rooms As New List(Of Room)

    ' These methods manufacture a new copy of one of the plug rooms.
    Public ReadOnly Property LeftPlug As Room
        Get
            Return New Room("Rooms\left.xml")
        End Get
    End Property
    Public ReadOnly Property RightPlug As Room
        Get
            Return New Room("Rooms\right.xml")
        End Get
    End Property
    Public ReadOnly Property UpPlug As Room
        Get
            Return New Room("Rooms\up.xml")
        End Get
    End Property
    Public ReadOnly Property DownPlug As Room
        Get
            Return New Room("Rooms\down.xml")
        End Get
    End Property


    Public Sub New(seed As Object, AvailableRoomList As List(Of Room))
        Dim random As New Random(seed.GetHashCode())

        Dim CoreIndex As Integer = random.Next(AvailableRoomList.Count - 1)
        Dim CoreRoom As Room = AvailableRoomList(CoreIndex)
        Dim OpenPorts As New List(Of DoorPort)
        AvailableRoomList.RemoveAt(CoreIndex)
        Rooms.Add(CoreRoom)
        If CoreRoom.DoorUp <> Room.DoorState.None Then
            OpenPorts.Add(New DoorPort(Room.DoorDirection.Up, CoreRoom))
        End If
        If CoreRoom.DoorDown <> Room.DoorState.None Then
            OpenPorts.Add(New DoorPort(Room.DoorDirection.Down, CoreRoom))
        End If
        If CoreRoom.DoorRight <> Room.DoorState.None Then
            OpenPorts.Add(New DoorPort(Room.DoorDirection.Right, CoreRoom))
        End If
        If CoreRoom.DoorLeft <> Room.DoorState.None Then
            OpenPorts.Add(New DoorPort(Room.DoorDirection.Left, CoreRoom))
        End If

        While OpenPorts.Count > 0
            Dim p As DoorPort = OpenPorts(0)

            ' Find a room that can fill the port
            Dim r As Room = Nothing

            ' See if a room is already there
            Dim e As Room = Nothing
            Select Case p.OpenDirection
                Case Room.DoorDirection.Up
                    e = RoomAt(p.OpenRoom.XOffset + Room.RoomWidth / 2, p.OpenRoom.YOffset - Room.RoomHeight / 2)
                Case Room.DoorDirection.Down
                    e = RoomAt(p.OpenRoom.XOffset + Room.RoomWidth / 2, p.OpenRoom.YOffset + 3 * Room.RoomHeight / 2)
                Case Room.DoorDirection.Left
                    e = RoomAt(p.OpenRoom.XOffset - Room.RoomWidth / 2, p.OpenRoom.YOffset + Room.RoomHeight / 2)
                Case Room.DoorDirection.Right
                    e = RoomAt(p.OpenRoom.XOffset + Room.RoomWidth / 2 * 3, p.OpenRoom.YOffset + Room.RoomHeight / 2)
            End Select
            If IsNothing(e) = False Then
                r = e
            End If

            ' Perhaps pick an available map to use?
            If IsNothing(r) Then
                Dim offset As Integer = random.Next(AvailableRoomList.Count + 1) ' How many viable options to pass on to pick a random choice.
                While offset > 0
                    Dim b As Boolean = False
                    For Each room As Room In AvailableRoomList
                        Select Case p.OpenDirection
                            Case Room.DoorDirection.Up
                                If room.DoorDown <> Room.DoorState.None Then
                                    r = room
                                    offset -= 1
                                    b = True
                                End If
                            Case Room.DoorDirection.Down
                                If room.DoorUp <> Room.DoorState.None Then
                                    r = room
                                    offset -= 1
                                    b = True
                                End If
                            Case Room.DoorDirection.Right
                                If room.DoorLeft <> Room.DoorState.None Then
                                    r = room
                                    offset -= 1
                                    b = True
                                End If
                            Case Room.DoorDirection.Left
                                If room.DoorRight <> Room.DoorState.None Then
                                    r = room
                                    offset -= 1
                                    b = True
                                End If
                        End Select
                        If offset = 0 Then Exit For
                    Next
                    If b = False Then Exit While ' We don't have any rooms that are even capable of fitting.
                End While
            End If

            ' If none of the open maps will fill the slot, add one of the plugs instead.
            If IsNothing(r) Then
                Select Case p.OpenDirection
                    Case Room.DoorDirection.Up
                        r = UpPlug
                    Case Room.DoorDirection.Down
                        r = DownPlug
                    Case Room.DoorDirection.Right
                        r = RightPlug
                    Case Room.DoorDirection.Left
                        r = LeftPlug
                End Select
            End If

            ' Add the room we've decided on and any of it's ports that need connecting.
            If IsNothing(e) Then
                Rooms.Add(r)
            End If
            Select Case p.OpenDirection
                Case Room.DoorDirection.Up
                    r.XOffset = p.OpenRoom.XOffset
                    r.YOffset = p.OpenRoom.YOffset - Room.RoomBuffer - r.Height
                    p.OpenRoom.RoomUp = r
                    r.RoomDown = p.OpenRoom
                    If IsNothing(e) Then
                        If r.DoorRight <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Right, r))
                        If r.DoorLeft <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Left, r))
                        If r.DoorUp <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Up, r))
                    End If
                Case Room.DoorDirection.Down
                    r.XOffset = p.OpenRoom.XOffset
                    r.YOffset = p.OpenRoom.YOffset + Room.RoomBuffer + p.OpenRoom.Height
                    p.OpenRoom.RoomDown = r
                    r.RoomUp = p.OpenRoom
                    If IsNothing(e) Then
                        If r.DoorRight <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Right, r))
                        If r.DoorLeft <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Left, r))
                        If r.DoorDown <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Down, r))
                    End If
                Case Room.DoorDirection.Right
                    r.XOffset = p.OpenRoom.XOffset + Room.RoomBuffer + p.OpenRoom.Width
                    r.YOffset = p.OpenRoom.YOffset
                    p.OpenRoom.RoomRight = r
                    r.RoomLeft = p.OpenRoom
                    If IsNothing(e) Then
                        If r.DoorDown <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Down, r))
                        If r.DoorUp <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Up, r))
                        If r.DoorRight <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Right, r))
                    End If
                Case Room.DoorDirection.Left
                    r.XOffset = p.OpenRoom.XOffset - Room.RoomBuffer - r.Width
                    r.YOffset = p.OpenRoom.YOffset
                    p.OpenRoom.RoomLeft = r
                    r.RoomRight = p.OpenRoom
                    If IsNothing(e) Then
                        If r.DoorDown <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Down, r))
                        If r.DoorUp <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Up, r))
                        If r.DoorLeft <> Room.DoorState.None Then OpenPorts.Add(New DoorPort(Room.DoorDirection.Left, r))
                    End If
            End Select

            OpenPorts.RemoveAt(0)
            AvailableRoomList.Remove(r)
        End While

#If Not VersionType = "Debug" Then
        For Each r As Room In Rooms
            For i As Integer = 0 To 10
                Dim l As New GameObject(My.Resources.CeilingLight, r, New Vector3(random.Next(r.Width - My.Resources.CeilingLight.Width), random.Next(r.Height - My.Resources.CeilingLight.Height), 10))
                l.CastsShadow = False
                r.AddGameObject(l)
            Next
        Next
#End If
    End Sub

    Public Function RoomFits(r As Room) As Boolean
        For Each r2 As Room In Rooms
            If r.Bounds.IntersectsWith(r.Bounds) Then
                Return False
            End If
        Next
        Return True
    End Function

    Public Function RoomAt(X As Double, Y As Double) As Room
        For Each r As Room In Rooms
            If r.Bounds.Contains(X, Y) Then
                Return r
            End If
        Next
        Return Nothing
    End Function

End Class

Public Class DoorPort
    Public OpenDirection As Room.DoorDirection
    Public OpenRoom As Room
    Public Sub New(Direction As Room.DoorDirection, Room As Room)
        OpenDirection = Direction
        OpenRoom = Room
    End Sub
End Class
