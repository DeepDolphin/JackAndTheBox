Public Class Sprite
    Public Frames As New List(Of Bitmap)
    Private _t As Double = 0
    Public FPS As Integer = 24
    Public ReadOnly Property CurrentFrame As Integer
        Get
            Return _t / FPS
        End Get
    End Property
    Public ReadOnly Property Length As Integer
        Get
            Return Frames.Count
        End Get
    End Property

    Public Sub Tick(t As Double)
        _t += t
        While _t > Length * (1 / FPS)
            _t -= (Length * (1 / FPS))
        End While
    End Sub
End Class
