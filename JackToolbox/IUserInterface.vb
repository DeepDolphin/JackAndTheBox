Imports System.Drawing

Public MustInherit Class IUserInterface
    Public GraphicsMap As Bitmap
    Private Graphics As Graphics

    Public Sub New(ScreenWidth As Integer, ScreenHeight As Integer)
        GraphicsMap = New Bitmap(ScreenWidth, ScreenHeight)
        Graphics = Graphics.FromImage(GraphicsMap)
    End Sub

    Public Sub Redraw()

    End Sub

End Class
