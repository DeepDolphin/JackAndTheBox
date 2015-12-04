Public Class GameObject
    Implements IComparable
    Public Position As Vector3
    Public Speed As Vector2
    Public HitBox As RectangleF
    Public Sprite As Sprite
    Public Room As Room

    Public Properties As New Dictionary(Of String, String)
    Public Ability As Ability '

    Public GraphicsMap As Bitmap
    Private Graphics As Graphics

    Protected Flags As New BitArray(GameObjectProps.Max) 'ToDo: change the name of this thing

    Public ReadOnly Property FloorObject As Boolean
        Get
            Return Flags.Get(GameObjectProps.FloorObject)
        End Get
    End Property

    Public ReadOnly Property CastsShadow As Boolean
        Get
            Return Flags.Get(GameObjectProps.CastsShadow)
        End Get
    End Property

    Public ReadOnly Property Collidable As Boolean
        Get
            Return Flags.Get(GameObjectProps.Collidable)
        End Get
    End Property

    Public ReadOnly Property Invulnerable As Boolean
        Get
            Return Flags.Get(GameObjectProps.Invulnerable)
        End Get
    End Property

    Public ReadOnly Property Visible As Boolean
        Get
            Return Flags.Get(GameObjectProps.Visible)
        End Get
    End Property

    Public Property Dead As Boolean
        Get
            Return Flags.Get(GameObjectProps.Dead)
        End Get
        Set(Parameter As Boolean)
            Flags.Set(GameObjectProps.Dead, Parameter)
        End Set
    End Property

    Public Enum GameObjectProps
        CastsShadow
        FloorObject
        Collidable
        Invulnerable
        Visible
        Dead

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

    Public Sub New(Sprite As Sprite, Room As Room, Position As Vector3, Health As Integer, ObjectProperties As GameObjectProps())
        Init(Sprite, Room, Position, Me.Speed, Health, ObjectProperties)
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




        Properties.Add("Health", Health)
        Properties.Add("MaxHealth", Health)




        For Each ObjectProperty As GameObjectProps In ObjectProperties
            Flags.Set(ObjectProperty, True)
        Next

        Dim x As Integer = Sprite.Width / 2
        Dim y As Integer = Game.Resources.HealthBackground.Height + 1
        Dim width As Integer = Sprite.Width
        Dim height As Integer = Sprite.Height

        If (Not FloorObject) Then
            y += Sprite.Height * (10 / 16)
            height = (height / 2) * (10 / 16)
        End If

        HitBox = New Rectangle(x, y, width, height)

    End Sub

    Public Overridable Sub Update(t As Double)
        Sprite.Tick(t)
        'Delete if out of Health
        If Not Invulnerable Then
            If Properties("Health") <= 0.0 Then
                Flags.Set(GameObjectProps.Dead, True)
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

    Private Function CIntFloor(vals() As Double) As Integer
        Dim sum As Integer
        For Each val As Double In vals
            sum += CInt(Math.Floor(val))
        Next
        Return sum
    End Function

    Public Sub Redraw()
        If Graphics Is Nothing Then
            If Graphics Is Nothing Then
                GraphicsMap = New Bitmap(Sprite.Width * 2, Sprite.Height + Game.Resources.HealthBackground.Height + 4)
                Graphics = Graphics.FromImage(GraphicsMap)
            End If

            If CastsShadow Then
                    Graphics.DrawImage(Game.Resources.Shadow,
                                   Sprite.Width \ 2,
                                   Game.Resources.HealthBackground.Height + Sprite.Height - 7,
                                   Sprite.Width,
                                   10)
                End If

                Graphics.DrawImage(Sprite.CurrentFrame,
                               Sprite.Width \ 2,
                               Game.Resources.HealthBackground.Height + 1,
                               Sprite.Width,
                               Sprite.Height)

            If Not TypeOf Me Is Player AndAlso Properties("Health") <> Properties("MaxHealth") Then
                Graphics.DrawImage(Game.Resources.HealthBackground,
                                   0,
                                   0,
                                   Sprite.Width * 2,
                                   Game.Resources.HealthBackground.Height)

                Graphics.DrawImage(Game.Resources.HealthBar,
                                   New Rectangle(2,
                                                 2,
                                                 Sprite.Width * (Properties("Health") / Properties("MaxHealth")) * 2,
                                                 Game.Resources.HealthBar.Height),
                                   New Rectangle(0,
                                                 0,
                                                 Game.Resources.HealthBar.Width * (Properties("Health") / Properties("MaxHealth")),
                                                 Game.Resources.HealthBar.Height),
                                   GraphicsUnit.Pixel)
            End If
        End If
    End Sub
End Class