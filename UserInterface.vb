Public Class UserInterface

    Public GraphicsMap As New Bitmap(MainForm.ScreenWidth, MainForm.ScreenHeight)
    Private Graphics As Graphics


    Public Sub New()
        Graphics = Graphics.FromImage(GraphicsMap)
    End Sub

    Public Sub Redraw()

    End Sub

End Class
