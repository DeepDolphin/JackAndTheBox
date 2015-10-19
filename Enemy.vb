Public Class Enemy
    Inherits Actor

    Public distToPlayer As Double

    Public Sub New(Room As Room, x As Double, y As Double)
        MyBase.New(Room, x, y, 0)
        Properties("Attack") = "1"
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
        distToPlayer = getDistanceTo(MainForm.Player)
    End Sub
End Class

Public Class NormalEnemy
    Inherits Enemy

    Public Sub New(Room As Room, x As Double, y As Double)
        MyBase.New(Room, x, y)
    End Sub

    Public Overrides Sub update(t As Double)
        MyBase.Update(t)
        Dim radToPlayer As Double = getDirectionTo(MainForm.Player)
        ' ToDo: Use Vectors stuffs
        XSpeed = Math.Cos(radToPlayer) + MainForm.Player.XSpeed
        YSpeed = Math.Sign(radToPlayer) + MainForm.Player.YSpeed

        If distToPlayer < 75.0 Then
            XSpeed *= 8
            YSpeed *= 8
        ElseIf distToPlayer < 150.0 Then
            XSpeed *= 4
            YSpeed *= 4
        Else
            XSpeed = 0
            YSpeed = 0
        End If
    End Sub
End Class
