Public Class GameObject
    Public Position As Vector3
    Public Speed As Vector2
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

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3)
        Me.Position = Position
        Me.Image = Image
        HitBox = Image.GetBounds(GraphicsUnit.Pixel)
        Me.Room = Room
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Speed As Vector2)
        Me.Position = Position
        Me.Speed = Speed
        Me.Image = Image
        HitBox = Image.GetBounds(GraphicsUnit.Pixel)
        Me.Room = Room
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Speed As Vector2, Health As Integer)
        Me.Position = Position
        Me.Speed = Speed
        Me.Image = Image
        HitBox = Image.GetBounds(GraphicsUnit.Pixel)
        Properties.Add("Health", Health)
        Me.Room = Room
    End Sub

    Public Overridable Sub Update(t As Double)
        'Delete if out of Health
        If Properties.Keys.Contains("Health") Then
            If Properties("Health") <= 0 Then Flags.Add("Delete")
        End If
    End Sub

    Public Overridable Function CollidesWith(O As GameObject, Change As Vector2) As Boolean
        If (O.Position.Z <> Position.Z) Then Return False

        Dim otherhitbox As RectangleF = O.HitBox
        otherhitbox.X += O.Position.X
        otherhitbox.Y += O.Position.Y
        Dim myhitbox As RectangleF = HitBox
        myhitbox.X += Position.X + Change.X
        myhitbox.Y += Position.Y + Change.Y
        Return myhitbox.IntersectsWith(otherhitbox)
    End Function

    Public Overridable Function CollidesWith(O As GameObject, Change As Vector3) As Boolean
        Return If(O.Position.Z <> Position.Z + Change.Z, False, CollidesWith(O, Change.XY))
    End Function

    Public Function GetMiddle() As PointF
        Return New PointF(X + HitBox.Width / 2, Y + HitBox.Width / 2)
    End Function

    Public Function getDistanceTo(o As GameObject) As Double
        Return Math.Sqrt(Math.Pow((o.GetMiddle().Y - GetMiddle().Y), 2) + Math.Pow((o.GetMiddle().X - GetMiddle().X), 2))
    End Function

    Public Function getDirectionTo(o As GameObject) As Double
        Return getDirectionTo(o.GetMiddle())
    End Function

    Public Function getDirectionTo(point As PointF) As Double
        Return Math.Atan2(point.Y - GetMiddle().Y, point.X - GetMiddle().X)
    End Function

End Class