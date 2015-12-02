Public Class InventoryItem
    Inherits GameObject
    Public Parent As Actor

    Public Sub New(Sprite As Sprite, Room As Room, Position As Vector2, Health As Integer, ObjectProperties As GameObjectProps())
        MyBase.New(Sprite, Room, New Vector3(Position.X, Position.Y, -10), Health, ObjectProperties)
    End Sub

    Public Sub New(Item As InventoryItem)
        MyBase.New(Item.Sprite, Item.Room, Item.Position, Item.Properties("MaxHealth"), {})
        Flags = Item.Flags
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
    End Sub
End Class
