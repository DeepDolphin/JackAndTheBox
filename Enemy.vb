Public Class Enemy
    Inherits GameObject

    Public radToPlayer As Double
    Public distToPlayer As Double

    Public Sub New(Room As Room, x As Double, y As Double)
        MyBase.New(My.Resources.CharacterUp1, Room, x, y, 100)
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        radToPlayer = Math.Atan2(MainForm.Player.Y - Y, MainForm.Player.X - X)
        distToPlayer = Math.Sqrt(Math.Pow((MainForm.Player.Y - Y), 2) + Math.Pow((MainForm.Player.X - X), 2))
    End Sub
End Class

Public Class NormalEnemy
    Inherits Enemy

    Public Sub New(Room As Room, x As Double, y As Double)
        MyBase.New(Room, x, y)
    End Sub

    Public Overrides Sub update()
        MyBase.Update()
        If distToPlayer < 75.0 Then
            XSpeed = 1.5 * Math.Cos(radToPlayer)
            YSpeed = 1.5 * Math.Sign(radToPlayer)
        ElseIf distToPlayer < 150.0 Then
            XSpeed = 0.5 * Math.Cos(radToPlayer)
            YSpeed = 0.5 * Math.Sign(radToPlayer)
        Else
            XSpeed = 0
            YSpeed = 0
        End If
    End Sub
End Class
