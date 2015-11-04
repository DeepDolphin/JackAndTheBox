Public Class Crate
    Inherits GameObject

    Public Sub New(Room As Room, Position As Vector3, Health As Integer)
        MyBase.New(My.Resources.Crate, Room, Position, Health)
    End Sub


End Class
