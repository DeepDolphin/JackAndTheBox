Public Class EXPOrb
    Inherits GameObject
    Public EXP As Integer = 0
    Public Sub New(EXP As Integer, Room As Room, Position As Vector3)
        MyBase.New(New Sprite({My.Resources.EXPOrb1, My.Resources.EXPOrb2, My.Resources.EXPOrb3, My.Resources.EXPOrb4}), Room, Position)
        Me.EXP = EXP
    End Sub
End Class
