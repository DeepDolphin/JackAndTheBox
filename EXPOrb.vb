Public Class EXPOrb
    Inherits GameObject
    Public EXP As Integer = 0

    Public Sub New(EXP As Integer, Room As Room, Position As Vector3)
        MyBase.New(New Sprite(New List(Of Bitmap)({My.Resources.EXPOrb1, My.Resources.EXPOrb2, My.Resources.EXPOrb3, My.Resources.EXPOrb4})) With {.FPS = 24}, Room, Position)
        Me.EXP = EXP
        Me.Colliable = 1
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
    End Sub
End Class
