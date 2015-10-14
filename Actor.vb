Public Class Actor
    Inherits GameObject
    Public Speed As Double
    Public Enum ActorDirection
        Up = 0
        Right
        Down
        Left
    End Enum
    Public Direction As ActorDirection = ActorDirection.Up

    Public Sub New(Room As Room, X As Double, Y As Double, Speed As Double)
        MyBase.New(My.Resources.CharacterUp1, Room, X, Y, 100)
        HitBox = New RectangleF(0, 22, 16, 10)
        Me.Speed = Speed
        Properties.Add("Attack Cooldown", "5")
        Properties.Add("Attack", "0.0")
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
        If (Properties("Attack Cooldown") > 0.0) Then Properties("Attack Cooldown") -= t
    End Sub

    Public Overridable Sub Hit(O As GameObject)
        Properties("Attack Cooldown") = "5"
    End Sub
End Class
