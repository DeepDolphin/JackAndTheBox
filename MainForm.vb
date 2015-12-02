Public Class MainForm

#If VersionType = "Beta" Then
    Public Const VersionTN As String = "1"
#ElseIf VersionType = "Release" Then
    Public Const VersionTN As String = "0"
#ElseIf VersionType = "Debug" Then
    Public Const VersionTN As String = "2"
#End If

    Public Const VersionNumber As String = "1.0.0301." + VersionTN + "000"

    Dim g As Game
    Private Buffer As BufferedGraphics

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim task As New System.Threading.Tasks.Task(New Action(Sub() StartGame()))
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
        g.KeyDown(e)
    End Sub

    Private Sub MainForm_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        g.KeyUp(e)
    End Sub

    Private Sub MainForm_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        g.MouseDown(e)
    End Sub

    Private Sub MainForm_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        g.MouseUp(e)
    End Sub

    Private Sub MainForm_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        'g.MouseMove(e)
    End Sub

    Private Sub MainForm_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
        g.MouseWheel(e)
    End Sub

    Private Sub StartGame()
        Buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), New Rectangle(0, 0, ClientSize.Width, ClientSize.Height))
        g = New Game(Buffer, ClientSize.Width, ClientSize.Height)
        g.Start()
    End Sub
End Class
