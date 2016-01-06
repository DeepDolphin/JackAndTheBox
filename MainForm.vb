Public Class MainForm
    Dim g As Game
    Private Buffer As BufferedGraphics

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim task As New Task(New Action(Sub() StartGame()))
        task.Start()
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        g.StopGame()
    End Sub

    Protected Overrides Function IsInputKey(
        ByVal keyData As Keys) As Boolean
        Return True
    End Function

    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If g IsNot Nothing Then
            g.KeyDown(e)
        End If
    End Sub

    Private Sub MainForm_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If g IsNot Nothing Then
            g.KeyUp(e)
        End If
    End Sub

    Private Sub MainForm_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        If g IsNot Nothing Then
            g.MouseDown(e)
        End If
    End Sub

    Private Sub MainForm_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        If g IsNot Nothing Then
            g.MouseUp(e)
        End If
    End Sub

    Private Sub MainForm_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If g IsNot Nothing Then
            g.MouseMove(e)
        End If
    End Sub

    Private Sub MainForm_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
        If g IsNot Nothing Then
            g.MouseWheel(e)
        End If
    End Sub

    Private Sub MainForm_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If g IsNot Nothing Then
            g.Resize(sender)
        End If
    End Sub

    Private Sub StartGame()
        Buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), New Rectangle(0, 0, ClientSize.Width, ClientSize.Height))
        g = New Game(Buffer, ClientSize.Width, ClientSize.Height)
        g.Start()
    End Sub
End Class
