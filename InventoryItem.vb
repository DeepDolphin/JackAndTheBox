Public Class InventoryItem
    Inherits GameObject

    Public Sub New(Sprite As Sprite, Room As Room, Position As Vector2)
        MyBase.New(Sprite, Room, New Vector3(Position.X, Position.Y, -10))
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
    End Sub
End Class
