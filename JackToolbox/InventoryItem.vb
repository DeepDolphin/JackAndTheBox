Imports JackPhysics

Public Class InventoryItem
    Inherits GameObject
    Public Parent As Actor

    Public Sub New(Sprite As Sprite, Room As Room, Position As Vector2, PropertyArray As Dictionary(Of String, String), ObjectProperties As GameObjectProperties.FlagsEnum())
        MyBase.New(Sprite, Room, New Vector3(Position.X, Position.Y, 0), PropertyArray, ObjectProperties)
    End Sub

    Public Sub New(Item As InventoryItem)
        MyBase.New(Item)
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
    End Sub
End Class
