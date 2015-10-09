Public Class GameObject
    Public X As Double
    Public Y As Double
    Public Z As Double
    Public XSpeed As Double
    Public YSpeed As Double
    Public HitBox As RectangleF
    Public Image As Bitmap
    Public CastsShadow As Boolean = True
    Public Room As Room
    Public Properties As New Dictionary(Of String, String)

    Public Sub New(Image As Bitmap, Room As Room)
        Me.Image = Image
        HitBox = Image.GetBounds(GraphicsUnit.Pixel)
        Me.Room = Room
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, X As Double, Y As Double)
        Me.X = X
        Me.Y = Y
        Me.Image = Image
        HitBox = Image.GetBounds(GraphicsUnit.Pixel)
        Me.Room = Room
    End Sub

    Public Sub New(Image As Bitmap, X As Double, Y As Double, XSpeed As Double, YSpeed As Double)
        Me.X = X
        Me.Y = Y
        Me.XSpeed = XSpeed
        Me.YSpeed = YSpeed
        Me.Image = Image
        HitBox = Image.GetBounds(GraphicsUnit.Pixel)
    End Sub

    Public Overridable Sub Update()
        'HitBox = New RectangleF(CSng(X), CSng(Y), HitBox.Width, HitBox.Height)
        'X += XSpeed
        'Y += YSpeed
    End Sub

    Public Overridable Function CollidesWith(O As GameObject, X As Double, Y As Double) As Boolean
        If (O.Z <> Z) Then Return False

        Dim otherhitbox As RectangleF = O.HitBox
        otherhitbox.X += X
        otherhitbox.Y += Y
        Dim myhitbox As RectangleF = HitBox
        myhitbox.X += Me.X
        myhitbox.Y += Me.Y
        Return myhitbox.IntersectsWith(otherhitbox)
    End Function

End Class