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
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
    End Sub
End Class
