﻿Public Class GameObject
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
    Public Flags As New List(Of String)

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

    Public Sub New(Image As Bitmap, Room As Room, X As Double, Y As Double, Health As Integer)
        Me.X = X
        Me.Y = Y
        Me.XSpeed = XSpeed
        Me.YSpeed = YSpeed
        Me.Image = Image
        HitBox = Image.GetBounds(GraphicsUnit.Pixel)
        Properties.Add("Health", Health)
        Me.Room = Room
    End Sub

    Public Overridable Sub Update(t As Double)
        If Properties.Keys.Contains("Health") Then
            If Properties("Health") <= 0 Then Flags.Add("Delete")
        End If
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

    Public Function GetMiddle() As PointF
        Return New PointF(X + HitBox.Width / 2, Y + HitBox.Width / 2)
    End Function

    Public Function getDistanceTo(o As GameObject) As Double
        Return Math.Sqrt(Math.Pow((o.GetMiddle().Y - GetMiddle().Y), 2) + Math.Pow((o.GetMiddle().X - GetMiddle().X), 2))
    End Function

    Public Function getDirectionTo(o As GameObject) As Double
        Return Math.Atan2(o.GetMiddle().Y - GetMiddle().Y, o.GetMiddle().X - GetMiddle().X)
    End Function

End Class