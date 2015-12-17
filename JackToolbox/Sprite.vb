Imports System.Drawing

Public Class Sprite
    Public Frames As New List(Of Bitmap)
    Private _t As Double = 0
    Public FPS As Integer = 24
    Public ReadOnly Property CurrentIndex As Integer
        Get
            Return _t / (1 / FPS)
        End Get
    End Property
    Public ReadOnly Property Length As Integer
        Get
            Return Frames.Count
        End Get
    End Property
    Public ReadOnly Property CurrentFrame As Bitmap
        Get
            If Frames.Count > 0 Then
                If CurrentIndex >= Length Then
                    Stop
                End If
                Return Frames(CurrentIndex)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Public ReadOnly Property Width As Integer
        Get
            If Frames.Count > 0 Then
                Return Frames(0).Width
            Else
                Return 0
            End If
        End Get
    End Property
    Public ReadOnly Property Height As Integer
        Get
            If Frames.Count > 0 Then
                Return Frames(0).Height
            Else
                Return 0
            End If
        End Get
    End Property
    Public Sub Tick(t As Double)
        _t += t
        While CurrentIndex >= Length
            _t -= (Length * (1 / FPS))
        End While
    End Sub

    Public Sub New()
        ' Leave the frames blank
    End Sub
    Public Sub New(SingleFrame As Bitmap)
        Frames.Add(SingleFrame)
    End Sub
    Public Sub New(Frames As IEnumerable(Of Bitmap))
        Me.Frames = Frames
    End Sub
End Class
