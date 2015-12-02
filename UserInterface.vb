

Public Class UserInterface

    Public GraphicsMap As New Bitmap(Game.ScreenWidth, Game.ScreenHeight)
    Private Graphics As Graphics


    Public Sub New()
        Graphics = Graphics.FromImage(GraphicsMap)
    End Sub

    Public Sub Redraw()

    End Sub

End Class
