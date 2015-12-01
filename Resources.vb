Public Class Resources


    Private EXPOrb As Sprite
    Private Player As Sprite


    Public Shadow As Bitmap
    Public GradientLeft As Bitmap
    Public GradientRight As Bitmap
    Public ShadeBrush As SolidBrush = New SolidBrush(Color.FromArgb(175, 0, 0, 0))
    Public HealthBackground As Bitmap
    Public HealthBar As Bitmap

    Public Sub New()
        Shadow = New Bitmap(My.Resources.Shadow)
        GradientRight = New Bitmap(My.Resources.GradientRight)
        GradientLeft = New Bitmap(My.Resources.GradientLeft)
        HealthBackground = New Bitmap(My.Resources.HealthBackground)
        HealthBar = New Bitmap(My.Resources.HealthBar)
    End Sub

    Public Sub New(Directory As String)



    End Sub
End Class
