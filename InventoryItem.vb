Public Class InventoryItem
    Inherits GameObject
    Public Parent As Actor

    Public Sub New(Sprite As Sprite, Room As Room, Position As Vector2)
        MyBase.New(Sprite, Room, New Vector3(Position.X, Position.Y, -10))
        Flags.Add("InventoryItem")
    End Sub

    Public Sub New(Item As InventoryItem)
        MyBase.New(Item.Sprite, Item.Room, Item.Position)
        Flags.Add("InventoryItem")
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
    End Sub
End Class
