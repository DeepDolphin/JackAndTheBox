Public Class GameObject
    Public Position As Vector3
    Public Speed As Vector2
    Public HitBox As RectangleF
    Public Image As Bitmap
    Public CastsShadow As Boolean = True
    Public Room As Room
    Public Properties As New Dictionary(Of String, String)
    Public Flags As New List(Of String)

    Public ReadOnly Property Middle As Vector3
        Get
            Return New Vector3(Position.X + (HitBox.Width / 2), Position.Y + (HitBox.Width / 2), Position.Z)
        End Get
    End Property

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

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Health As Integer)
        Me.Position = Position
        Me.Image = Image
        Me.Room = Room

        Init(Health)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Speed As Vector2, Health As Integer)
        Me.Position = Position
        Me.Speed = Speed
        Me.Image = Image
        Me.Room = Room

        Init(Health)
    End Sub

    Public Sub Init(health As Double)
        HitBox = Image.GetBounds(GraphicsUnit.Pixel)
        Properties.Add("Health", health)
    End Sub

    Public Overridable Sub Update(t As Double)
        'Delete if out of Health
        If Properties.Keys.Contains("Health") Then
            If Properties("Health") <= 0.0 Then
                Flags.Add("Delete")
            End If
        End If
    End Sub

    Public Overridable Function CollidesWith(Other As GameObject, OtherPosition As Vector2) As Boolean
        'If not on the same level it won't collide
        If (Other.Position.Z <> Position.Z) Then Return False

        Dim otherhitbox As RectangleF = Other.HitBox
        otherhitbox.X += OtherPosition.X
        otherhitbox.Y += OtherPosition.Y
        Dim myhitbox As RectangleF = HitBox
        myhitbox.X += Position.X
        myhitbox.Y += Position.Y
        If myhitbox.IntersectsWith(otherhitbox) Then
            Dim test = True
        End If

        Return myhitbox.IntersectsWith(otherhitbox)
    End Function

    Public Overridable Function CollidesWith(Other As GameObject, OtherPosition As Vector3) As Boolean
        Return If(OtherPosition.Z <> Position.Z, False, CollidesWith(Other, OtherPosition.XY))
    End Function

    Public Function getDistanceTo(O As GameObject) As Double
        Return (O.Middle - Middle).Length
    End Function

    Public Function getDirectionTo(O As GameObject) As Double
        Return getDirectionTo(O.Middle.XY)
    End Function

    Public Function getDirectionTo(point As PointF) As Double
        Return Math.Atan2(point.Y - Middle.Y, point.X - Middle.X)
    End Function

End Class