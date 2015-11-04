Public Class Crate
    Inherits GameObject

    Public Sub New(Room As Room, Position As Vector3)
        MyBase.New(My.Resources.Crate, Room, Position, 10)
    End Sub

    Public Sub New(Sprite As Sprite, Room As Room, Position As Vector3)
        MyBase.New(My.Resources.Crate, Room, Position, 10)
    End Sub

End Class

Public Class ExplosiveCrate
    Inherits Crate

    Public Sub New(Room As Room, Position As Vector3)
        MyBase.New(Room, Position)
    End Sub

    Public Sub Explode()
        Dim objects As List(Of GameObject) = getNearList(10)




    End Sub
End Class
