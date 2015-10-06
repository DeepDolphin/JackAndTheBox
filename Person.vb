Public Class Person
    Inherits GameObject
    Public LastMove As Point
    Public Enum PersonDirection
        Up = 0
        Right
        Down
        Left
    End Enum
    Public Direction As PersonDirection = PersonDirection.Up

    Public Sub New(X As Double, Y As Double)
        MyBase.New(My.Resources.CharacterUp1, X, Y)
        HitBox = New RectangleF(0, 22, 16, 10)
    End Sub

    Public Overrides Sub Update()
        MyBase.Update()
    End Sub

    'Public Sub StepUp()
    '    Y -= 1
    '    LastMove = New Point(0, -1)
    'End Sub

    'Public Sub StepDown()
    '    Y += 1
    '    LastMove = New Point(0, 1)
    'End Sub

    'Public Sub StepRight()
    '    X += 1
    '    LastMove = New Point(1, 0)
    'End Sub

    'Public Sub StepLeft()
    '    X -= 1
    '    LastMove = New Point(-1, 0)
    'End Sub

    'Public Sub UndoMove()
    '    X -= LastMove.X
    '    Y -= LastMove.Y
    'End Sub
End Class
