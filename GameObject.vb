Public Class GameObject
    Implements IComparable
    Public Position As Vector3
    Public Speed As Vector2
    Public HitBox As RectangleF
    Public Sprite As Sprite
    Public Room As Room
    Public Properties As New Dictionary(Of String, String)
    Public Flags As New List(Of String)
    Public Visible As Boolean = True
    Public Ability As Ability '
    Private Const Health_Invulnerable = -1
    Private TestProperties As New BitArray(GameObjectProps.Max) ' change the name of this thing

    Public ReadOnly Property FloorObject As Boolean
        Get
            Return TestProperties.Get(GameObjectProps.FloorObject)
        End Get
    End Property

    Public ReadOnly Property CastsShadow As Boolean
        Get
            Return TestProperties.Get(GameObjectProps.CastsShadow)
        End Get
    End Property

    Public ReadOnly Property Collidable As Boolean
        Get
            Return TestProperties.Get(GameObjectProps.Collidable)
        End Get
    End Property

    Public Enum GameObjectProps
        CastsShadow
        FloorObject
        Collidable

        Max ' Don't use, only for bounds of enum
    End Enum

    Public ReadOnly Property Depth As Double
        Get
            Dim myDepth As Double
            myDepth = Position.Y + (Position.Z * HitBox.Height * (10 / 16))
            If Not (FloorObject) Then
                myDepth += HitBox.Height + HitBox.Y
            End If
            Return myDepth
        End Get
    End Property

    Public ReadOnly Property Middle As Vector3
        Get
            Return New Vector3(Position.X + (HitBox.Width / 2), Position.Y + (HitBox.Width / 2), Position.Z)
        End Get
    End Property

    Public Sub New(Image As Bitmap, Room As Room, ObjectProperties As GameObjectProps())
        Init(New Sprite(Image), Room, Me.Position, Me.Speed, Health_Invulnerable, ObjectProperties)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, ObjectProperties As GameObjectProps())
        Init(New Sprite(Image), Room, Position, Me.Speed, Health_Invulnerable, ObjectProperties)
    End Sub

    Public Sub New(Sprite As Sprite, Room As Room, Position As Vector3, ObjectProperties As GameObjectProps())
        Init(Sprite, Room, Position, Me.Speed, Health_Invulnerable, ObjectProperties)
    End Sub

    Public Sub New(Sprite As Sprite, Room As Room, Position As Vector3, Health As Integer, ObjectProperties As GameObjectProps())
        Init(Sprite, Room, Position, Me.Speed, Health, ObjectProperties)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Speed As Vector2, ObjectProperties As GameObjectProps())
        Init(New Sprite(Image), Room, Position, Speed, Health_Invulnerable, ObjectProperties)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Health As Integer, ObjectProperties As GameObjectProps())
        Init(New Sprite(Image), Room, Position, Me.Speed, Health, ObjectProperties)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Speed As Vector2, Health As Integer, ObjectProperties As GameObjectProps())
        Init(New Sprite(Image), Room, Position, Speed, Health, ObjectProperties)
    End Sub

    Public Sub Init(Image As Sprite, Room As Room, Position As Vector3, Speed As Vector2, Health As Integer, ObjectProperties As GameObjectProps())
        Me.Position = Position
        Me.Speed = Speed
        Me.Sprite = Image
        Me.Room = Room
        HitBox = New Rectangle(0, 0, Sprite.Width, Sprite.Height)
        If Not (Health = Health_Invulnerable) Then
            Properties.Add("Health", Health)
        End If

        For Each ObjectProperty As GameObjectProps In ObjectProperties
            TestProperties.Set(ObjectProperty, True)
        Next
    End Sub

    Public Overridable Sub Update(t As Double)
        Sprite.Tick(t)
        'Delete if out of Health
        If Properties.Keys.Contains("Health") Then
            If Properties("Health") <= 0.0 Then
                Flags.Add("Delete")
            End If
        End If
    End Sub

    Public Overridable Function CollidesWith(Other As GameObject, OtherPosition As Vector2) As Boolean
        'If not on the same level it won't collide
        If (Other.Position.Z <> Position.Z And Not (Position.Z <= 0 And Other.Position.Z <= 0)) Then Return False
        If Not Collidable OrElse Not Other.Collidable Then Return False

        Dim otherhitbox As RectangleF = Other.HitBox
        otherhitbox.X += OtherPosition.X
        otherhitbox.Y += If(Other.Position.Z <= 0, OtherPosition.Y + Other.Position.Z * (10 / 16), OtherPosition.Y)
        Dim myhitbox As RectangleF = HitBox
        myhitbox.X += Position.X
        myhitbox.Y += If(Position.Z <= 0, Position.Y + Position.Z * (10 / 16), Position.Y)

        'ToDo: z under 0 stuff
        Return myhitbox.IntersectsWith(otherhitbox)
    End Function

    Public Overridable Function CollidesWith(Other As GameObject, OtherPosition As Vector3) As Boolean
        If (OtherPosition.Z <> Position.Z And Not (Position.Z <= 0 And OtherPosition.Z <= 0)) Then
            Return False
        Else
            Return CollidesWith(Other, OtherPosition.XY)
        End If
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

    Public Function getNearList(range As Double) As List(Of GameObject)
        Dim objectList As List(Of GameObject) = New List(Of GameObject)
        Dim realRange As Double = range * HitBox.Width

        For Each gameObject As GameObject In Room.GameObjects
            If Not gameObject.Equals(Me) AndAlso getDistanceTo(gameObject) <= realRange Then
                objectList.Add(gameObject)
            End If
        Next

        Return objectList
    End Function

    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        Dim otherObj As GameObject
        otherObj = DirectCast(obj, GameObject)
        Return Depth - otherObj.Depth
    End Function
End Class