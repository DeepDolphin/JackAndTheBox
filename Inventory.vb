Public Class Inventory
    Public Parent As Actor

    Public ReadOnly Property ActiveAbility As Ability
        Get
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property UtilityAbility As Ability
        Get
            Return New PlaceAbility(0, New Crate(MainForm.Player.Room, MainForm.Player.Position), MainForm.Player, False)
        End Get
    End Property

    Public Equipped As New Dictionary(Of String, GameObject)
    Public Inventory As New List(Of GameObject)


    Public Sub New()
        EquippedInit()
    End Sub

    Public Sub New(Inventory As List(Of GameObject))
        Me.Inventory = Inventory
        EquippedInit()
    End Sub

    Public Sub AddItem(Item As InventoryItem)
        Inventory.Add(New InventoryItem(Item, {}) With {.Parent = Parent})
        Item.Flags.Add("Delete")
    End Sub

    Private Sub EquippedInit()
        Equipped.Add("RightHand", Nothing)
        Equipped.Add("LeftHand", Nothing)

        Equipped.Add("Feet", Nothing)
        Equipped.Add("Arms", Nothing)
        Equipped.Add("Legs", Nothing)
        Equipped.Add("Chest", Nothing)
        Equipped.Add("Head", Nothing)
    End Sub

    Public Sub Update(t As Double)
        ActiveAbility.Update(t)
    End Sub

End Class
