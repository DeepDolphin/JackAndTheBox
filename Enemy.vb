Public Class Enemy
    Inherits GameObject

    Public radToPlayer As Double
    Public distToPlayer As Double

    Public Sub New(x As Double, y As Double)
        MyBase.New(My.Resources.CharacterUp1, x, y) ' TODO: Write a decent constructor.
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        radToPlayer = Math.Atan2(MainForm.Player.Y - Y, MainForm.Player.X - X)
        distToPlayer = Math.Sqrt(Math.Pow((MainForm.Player.Y - Y), 2) + Math.Pow((MainForm.Player.X - X), 2))
    End Sub
End Class

Public Class NormalEnemy
    Inherits Enemy

    Public Sub New(x As Double, y As Double)
        MyBase.New(x, y)
    End Sub

    Public Overrides Sub update()
        MyBase.Update()
        If distToPlayer < 250.0 Then
            XSpeed = 1.5 * Math.Cos(radToPlayer)
            YSpeed = 1.5 * Math.Sign(radToPlayer)
        Else
            XSpeed = 0
            YSpeed = 0
        End If


    End Sub
End Class

Public Class RunningEnemy
    Inherits Enemy

    Public Sub New(x As Double, y As Double)
        MyBase.New(x, y)
    End Sub

    Public Overrides Sub update()
        MyBase.Update()
        If distToPlayer < 100.0 Then
            XSpeed = 2.5 * Math.Cos(radToPlayer)
            YSpeed = 2.5 * Math.Sign(radToPlayer)
        Else
            XSpeed = Math.Cos(radToPlayer)
            YSpeed = Math.Sign(radToPlayer)
        End If


    End Sub
End Class
