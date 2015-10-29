Public Class Enemy
    Inherits Actor

    Public Sub New(Room As Room, x As Double, y As Double)
        MyBase.New(My.Resources.CharacterUp1, Room, New Vector3(x, y, 0))
        Properties("Attack") = "1"
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
    End Sub
End Class

Public Class NormalEnemy
    Inherits Enemy

    Public Sub New(Room As Room, x As Double, y As Double)
        MyBase.New(Room, x, y)
    End Sub

    Public Overrides Sub update(t As Double)
        MyBase.Update(t)

        Dim toPlayer As Vector2 = MainForm.Player.Middle.XY - Middle.XY
        If (toPlayer.Length >= 150.0) Then
            Speed.Length = 0
            Return
        End If

        Speed = toPlayer

        If toPlayer.Length < 75.0 Then
            Speed.Length = 8
        ElseIf toPlayer.Length < 150.0 Then
            Speed.Length = 4
        End If
    End Sub
End Class
