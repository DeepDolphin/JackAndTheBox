Public Class EXPOrb
    Inherits GameObject
    Public EXP As Integer = 0

    Public Sub New(EXP As Integer, Room As Room, Position As Vector3)
        MyBase.New(New Sprite(MainForm.expOrb), Room, Position)
        Me.EXP = EXP
    End Sub
End Class
