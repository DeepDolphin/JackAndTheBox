Public Class World
    Public Rooms As New List(Of Room)
    Public Sub New(seed As Object, AvailableRoomList As List(Of Room), LeftPlug As Room, RightPlug As Room, UpPlug As Room, DownPlug As Room)
        Dim s As Integer = seed.GetHashCode()
        Dim random As New Random(s)

        Dim CoreIndex As Integer = random.Next(AvailableRoomList.Count - 1)
        Dim CoreRoom As Room = AvailableRoomList(CoreIndex)
        AvailableRoomList.RemoveAt(CoreIndex)
        Rooms.Add(CoreRoom)

        For Each r As Room In Rooms
            For i As Integer = 0 To 10
                Dim l As New GameObject(My.Resources.CeilingLight, random.Next(MainForm.ScreenWidth), random.Next(MainForm.ScreenHeight))
                l.CastsShadow = False
                l.Z = 10
                r.AddGameObject(l)
            Next
        Next
    End Sub
End Class
