Imports JackPhysics
Imports System.Drawing

Public Class EXPOrb
    Inherits InventoryItem
    Public EXP As Integer = 0

    Public Sub New(EXP As Integer, Room As Room, Position As Vector2)
        MyBase.New(New Sprite(New List(Of Bitmap)({Resources.EXPOrb1, Resources.EXPOrb2, Resources.EXPOrb3, Resources.EXPOrb4})) With {.FPS = 24}, Room, Position, {100}, {GameObjectProperties.FlagsEnum.Collidable, GameObjectProperties.FlagsEnum.CastsShadow, GameObjectProperties.FlagsEnum.Visible, GameObjectProperties.FlagsEnum.Invulnerable})
        Me.EXP = EXP
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
    End Sub
End Class
