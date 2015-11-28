Public Class Actor
    Inherits GameObject

    Public Inventory As New Inventory

    Public Enum ActorDirection As Integer
        East = 0
        SouthEast
        South
        SouthWest
        West
        NorthWest
        North
        NorthEast
    End Enum

    Public Shared Function ToActorDirection(radians As Double) As ActorDirection
        While radians < 0
            radians += Math.PI * 2
        End While
        Return CInt((radians Mod (Math.PI * 2)) / (Math.PI / 4)) Mod 8
    End Function
    Public Shared Function ToRadians(direction As ActorDirection) As Double
        Return direction * -Math.PI / 4
    End Function

    Public Direction As ActorDirection = ActorDirection.North

    Public Sub New(Room As Room)
        MyBase.New(My.Resources.CharacterUp1, Room, New Vector3(Room.XOffset + Room.Width / 2, Room.YOffset + Room.Height / 2, 0), 100, {GameObjectProps.CastsShadow, GameObjectProps.Collidable})
        GeneralInit()
        AbilityInit(1, 1)
        MovementInit(25, 8, 2)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3)
        MyBase.New(Image, Room, Position, 100, {GameObjectProps.CastsShadow, GameObjectProps.Collidable})
        GeneralInit()
        AbilityInit(1, 1)
        MovementInit(25, 8, 2)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Speed As Vector2)
        MyBase.New(Image, Room, Position, Speed, 100, {GameObjectProps.CastsShadow, GameObjectProps.Collidable})
        GeneralInit()
        AbilityInit(1, 1)
        MovementInit(25, 8, 2)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Speed As Vector2, Health As Integer)
        MyBase.New(Image, Room, Position, Speed, Health, {GameObjectProps.CastsShadow, GameObjectProps.Collidable})
        GeneralInit()
        AbilityInit(1, 1)
        MovementInit(25, 8, 2)
    End Sub

    Public Sub GeneralInit()
        Flags.Add("Actor")
        HitBox = New RectangleF(0, 22, 16, 10)
    End Sub

    Public Sub AbilityInit(activeCooldown As Double, utilityCooldown As Double)
        Properties.Add("ActiveCurrentCooldown", activeCooldown)
        Properties.Add("ActiveMaxCooldown", activeCooldown)
        Properties.Add("UtilityCurrentCooldown", utilityCooldown)
        Properties.Add("UtilityMaxCooldown", utilityCooldown)
    End Sub

    Public Sub MovementInit(stamina As Double, maxSpeed As Double, acceleration As Double)
        Properties.Add("MaxStamina", stamina)
        Properties.Add("CurrentStamina", stamina)
        Properties.Add("MaxSpeed", maxSpeed)
        Properties.Add("Acceleration", acceleration)
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)




        If (Properties("ActiveCurrentCooldown") > 0.0) Then Properties("ActiveCurrentCooldown") -= t
        If (Properties("CurrentStamina") < CDbl(Properties("MaxStamina"))) Then Properties("CurrentStamina") += t
    End Sub

    Public Overridable Sub Hit(O As GameObject)
        If (Properties("ActiveCurrentCooldown") <= 0.0) Then
            If O.Properties.Keys.Contains("Health") AndAlso O.Properties("Health") >= 0.0 Then
                O.Properties("Health") -= Properties("AttackPower")
            End If
        End If
    End Sub

    Public Overridable Sub Hit(OList As List(Of GameObject))
        For Each gameObject As GameObject In OList
            Hit(gameObject)
        Next
        Properties("AttackCurrentCooldown") = Properties("AttackMaxCooldown")
    End Sub

    Public Overloads Function getNearList(range As Double, angle As Double) As List(Of GameObject)
        Dim objectList As List(Of GameObject) = New List(Of GameObject)
        Dim realRange As Double = range * HitBox.Width

        For Each gameObject As GameObject In Room.GameObjects
            If Not gameObject.Equals(Me) AndAlso getDistanceTo(gameObject) <= realRange Then
                Dim rad As Double = getDirectionTo(gameObject)
                If Math.Abs(ToRadians(Direction) - ToRadians(ToActorDirection(rad))) <= (angle * (Math.PI / 180.0)) / 2 Then
                    objectList.Add(gameObject)
                End If
            End If
        Next

        Return objectList
    End Function
End Class