Public Class GameObject
    Implements IComparable
    Public Speed As Vector2
    Public HitBox As RectangleF
    Public Sprite As Sprite
    Public Room As Room

    Protected _Properties As GameObjectProperties
    Public ReadOnly Property Properties As GameObjectProperties
        Get
            Return _Properties
        End Get
    End Property

    Private _Position As Vector3
    Private _LastPosition As Vector3

    Public Property Position As Vector3
        Get
            Return _Position
        End Get
        Set(value As Vector3)
            _LastPosition = _Position
            _Position = value
        End Set
    End Property

    Public ReadOnly Property LastPosition As Vector3
        Get
            Return _LastPosition
        End Get
    End Property

    Public Ability As Ability

    Public GraphicsMap As Bitmap
    Protected Graphics As Graphics

    Public CollidedWith As New List(Of GameObject)

#Region "Sorting Algorithim"
    Public ReadOnly Property Depth As Double
        Get
            Dim myDepth As Double
            myDepth = Position.Y + (Position.Z * HitBox.Height * (10 / 16))
            If Not (Properties.FloorObject) Then
                myDepth += HitBox.Height + HitBox.Y
            End If
            Return myDepth
        End Get
    End Property

    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        Dim otherObj As GameObject
        otherObj = DirectCast(obj, GameObject)
        Return Depth - otherObj.Depth
    End Function

#End Region

    Public ReadOnly Property Middle As Vector3
        Get
            Return New Vector3(Position.X + (HitBox.Width / 2), Position.Y + (HitBox.Width / 2), Position.Z)
        End Get
    End Property

#Region "Constructors"

    Public Sub New(Sprite As Sprite, Room As Room, Position As Vector3, PropertyArray As String(), ObjectProperties As GameObjectProperties.FlagsEnum())
        Init(Sprite, Room, Position, Me.Speed, PropertyArray, ObjectProperties)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, PropertyArray As String(), ObjectProperties As GameObjectProperties.FlagsEnum())
        Init(New Sprite(Image), Room, Position, Me.Speed, PropertyArray, ObjectProperties)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Speed As Vector2, PropertyArray As String(), ObjectProperties As GameObjectProperties.FlagsEnum())
        Init(New Sprite(Image), Room, Position, Speed, PropertyArray, ObjectProperties)
    End Sub

    Public Overridable Sub Init(Image As Sprite, Room As Room, Position As Vector3, Speed As Vector2, PropertyArray As String(), ObjectProperties As GameObjectProperties.FlagsEnum())
        Me.Position = Position
        Me._LastPosition = Position
        Me.Speed = Speed
        Me.Sprite = Image
        Me.Room = Room

        'If (Health <= 0) Then Throw New SyntaxErrorException("Health is less than or equal to 0")

        _Properties = New GameObjectProperties(Me, ObjectProperties, PropertyArray)

        Dim x As Integer = (((Sprite.Width * 2) \ CInt(Math.Ceiling(Properties.MaxHealth / 100)) + 1) * CInt(Math.Ceiling(Properties.MaxHealth / 100)) - 1) \ 4
        Dim y As Integer = Game.Resources.HealthBackground.Height + 1
        Dim width As Integer = Sprite.Width
        Dim height As Integer = Sprite.Height

        If (Not Properties.FloorObject) Then
            y += Sprite.Height * (10 / 16)
            height = (height / 2) * (10 / 16)
        End If

        HitBox = New Rectangle(x, y, width, height)
        Properties.Dirty = True

        If Graphics Is Nothing Then
            GraphicsMap = New Bitmap(((Sprite.Width * 2) \ CInt(Math.Ceiling(Properties.MaxHealth / 100)) + 1) * CInt(Math.Ceiling(Properties.MaxHealth / 100)) - 1, Sprite.Height + Game.Resources.HealthBackground.Height + 4)
            Graphics = Graphics.FromImage(GraphicsMap)
        End If

        Redraw()
    End Sub

#End Region

    Public Overridable Sub Update(t As Double)
        Sprite.Tick(t)
        'Delete if out of Health
    End Sub

    Public Function SpriteIntersects(Other As GameObject) As Boolean
        Dim MyRect As New RectangleF(Position.X, Position.Y + Position.Z * (10 / 16), GraphicsMap.Width, GraphicsMap.Height)
        Dim otherRect As New RectangleF(Other.Position.X, Other.Position.Y + Other.Position.Z * (10 / 16), Other.GraphicsMap.Width, Other.GraphicsMap.Height)

        Return MyRect.IntersectsWith(otherRect)
    End Function

    Public Overridable Function CollidesWith(Other As GameObject, OtherPosition As Vector2) As Boolean
        'If not on the same level it won't collide
        If (Other.Position.Z <> Position.Z And Not (Position.Z <= 0 And Other.Position.Z <= 0)) Then Return False
        If Not Properties.Collidable OrElse Not Other.Properties.Collidable Then Return False

        Dim otherhitbox As RectangleF = Other.HitBox
        otherhitbox.X += OtherPosition.X
        otherhitbox.Y += If(Other.Position.Z <= 0, OtherPosition.Y + Other.Position.Z * (10 / 16), OtherPosition.Y)
        Dim myhitbox As RectangleF = HitBox
        myhitbox.X += Position.X
        myhitbox.Y += If(Position.Z <= 0, Position.Y + Position.Z * (10 / 16), Position.Y)

        If (myhitbox.IntersectsWith(otherhitbox)) Then
            Return True
        Else
            Return False
        End If

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


    Private Function CIntFloor(vals() As Double) As Integer
        Dim sum As Integer
        For Each val As Double In vals
            sum += CInt(Math.Floor(val))
        Next
        Return sum
    End Function

    Public Sub Redraw()
        Graphics.Clear(Color.Transparent)
        If Properties.CastsShadow Then
            Graphics.DrawImage(Game.Resources.Shadow,
                               (((Sprite.Width * 2) \ CInt(Math.Ceiling(Properties.MaxHealth / 100)) + 1) * CInt(Math.Ceiling(Properties.MaxHealth / 100)) - 1) \ 4,
                               Game.Resources.HealthBackground.Height + Sprite.Height - 7,
                               Sprite.Width,
                               10)
        End If

        Graphics.DrawImage(Sprite.CurrentFrame,
                           (((Sprite.Width * 2) \ CInt(Math.Ceiling(Properties.MaxHealth / 100)) + 1) * CInt(Math.Ceiling(Properties.MaxHealth / 100)) - 1) \ 4,
                           Game.Resources.HealthBackground.Height + 1,
                           Sprite.Width,
                           Sprite.Height)

        If Not TypeOf Me Is Player AndAlso Properties.Health <> Properties.MaxHealth AndAlso Not Properties.Invulnerable Then
            Dim numBars As Integer = Math.Ceiling(Properties.MaxHealth / 100)
            Dim index As Integer = 0
            Dim health As Integer = Properties.Health
            Dim maxHealth As Integer = Properties.MaxHealth

            While index < numBars
                Graphics.DrawImage(Game.Resources.HealthBackground,
                                   (((Sprite.Width * 2) \ numBars) + 1) * index,
                                   0,
                                   (Sprite.Width * 2) \ numBars,
                                   Game.Resources.HealthBackground.Height)

                Graphics.DrawImage(Game.Resources.HealthBar,
                                       New Rectangle(((((Sprite.Width * 2) \ numBars) + 1) * index) + 2,
                                                     2,
                                                     ((Sprite.Width * 2) \ numBars - 4) * (Math.Min(health, 100) / 100),
                                                     Game.Resources.HealthBar.Height),
                                       New Rectangle(0,
                                                     0,
                                                     Game.Resources.HealthBar.Width * (Math.Min(health, 100) / 100),
                                                     Game.Resources.HealthBar.Height),
                                       GraphicsUnit.Pixel)
                index += 1
                health -= 100
                maxHealth -= 100
            End While
        End If
    End Sub
End Class