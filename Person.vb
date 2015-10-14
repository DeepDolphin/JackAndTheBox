Public Class Person
    Inherits GameObject
    Public Speed As Double
    Public Enum PersonDirection
        Up = 0
        Right
        Down
        Left
    End Enum
    Public Direction As PersonDirection = PersonDirection.Up

    Public Sub New(Room As Room, X As Double, Y As Double, Speed As Double)
        MyBase.New(My.Resources.CharacterUp1, Room, X, Y, 100)
        HitBox = New RectangleF(0, 22, 16, 10)
        Me.Speed = Speed
        Properties.Add("Attack Cooldown", "10")
        Properties.Add("Attack", "0.0")
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
        If (Properties("Attack Cooldown") > 0) Then Properties("Attack Cooldown") -= 1
    End Sub
End Class
