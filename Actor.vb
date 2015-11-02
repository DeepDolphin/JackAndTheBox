Public Class Actor
    Inherits GameObject
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
        MyBase.New(My.Resources.CharacterUp1, Room, New Vector3(Room.XOffset + Room.Width / 2, Room.YOffset + Room.Height / 2, 0), 100)
        GeneralInit()
        AttackInit(1, 100, 5, 45)
        MovementInit(25, 8, 2)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3)
        MyBase.New(Image, Room, Position, 100)
        GeneralInit()
        AttackInit(1, 100, 5, 45)
        MovementInit(25, 8, 2)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Speed As Vector2)
        MyBase.New(Image, Room, Position, Speed, 100)
        GeneralInit()
        AttackInit(1, 100, 5, 45)
        MovementInit(25, 8, 2)
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Speed As Vector2, Health As Integer)
        MyBase.New(Image, Room, Position, Speed, Health)
        GeneralInit()
        AttackInit(1, 100, 5, 45)
        MovementInit(25, 8, 2)
    End Sub

    Public Sub GeneralInit()
        Flags.Add("Actor")
        HitBox = New RectangleF(0, 22, 16, 10)
    End Sub

    Public Sub AttackInit(attackCooldown As Double, attackPower As Double, attackRange As Double, attackAngle As Double)
        Properties.Add("AttackCurrentCooldown", attackCooldown)
        Properties.Add("AttackMaxCooldown", attackCooldown)
        Properties.Add("AttackPower", attackPower)
        Properties.Add("AttackRange", attackRange)
        Properties.Add("AttackAngle", attackAngle)
    End Sub

    Public Sub MovementInit(stamina As Double, maxSpeed As Double, acceleration As Double)
        Properties.Add("MaxStamina", stamina)
        Properties.Add("CurrentStamina", stamina)
        Properties.Add("MaxSpeed", maxSpeed)
        Properties.Add("Acceleration", acceleration)
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)


        If (Properties("AttackCurrentCooldown") > 0.0) Then Properties("AttackCurrentCooldown") -= t
        If (Properties("CurrentStamina") < CDbl(Properties("MaxStamina"))) Then Properties("CurrentStamina") += t
    End Sub

    Public Overridable Sub Hit(O As GameObject)
        If (Properties("AttackCurrentCooldown") <= 0.0) Then
            If O.Properties.Keys.Contains("Health") AndAlso O.Properties("Health") >= 0.0 Then
                O.Properties("Health") -= Properties("AttackPower")
                Properties("AttackCurrentCooldown") = Properties("AttackMaxCooldown")
            End If
        End If
    End Sub

    Public Function getNearList(range As Double, angle As Double) As List(Of GameObject)
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

Public Class Player
    Inherits Actor

    Public Sub New(Room As Room)
        MyBase.New(Room)
        Flags.Add("Player")
    End Sub

    Public Sub UpdateMovement(t As Double, OIStatus As Dictionary(Of String, Boolean))
        Dim CurrentPlayerSpeed As Double = Speed.Length
        Dim PlayerMovement As Vector2
        If OIStatus("Sprint") AndAlso Properties("CurrentStamina") > 0.0 Then
            Properties("CurrentStamina") -= t * 10
            Properties("Acceleration") = 3
            Properties("MaxSpeed") = 13
        Else
            Properties("Acceleration") = 2
            Properties("MaxSpeed") = 8
        End If

        'Player arcade movement
        If MainForm.Options.Preferences("PlayerMovementType") = "ArcadeMovement" Then
            If OIStatus("Up") Then
                PlayerMovement.Y -= 1
            End If
            If OIStatus("Down") Then
                PlayerMovement.Y += 1
            End If
            If OIStatus("Right") Then
                PlayerMovement.X += 1
            End If
            If OIStatus("Left") Then
                PlayerMovement.X -= 1
            End If
            If (OIStatus("Up") Or OIStatus("Down") Or OIStatus("Right") Or OIStatus("Left")) AndAlso Not CurrentPlayerSpeed = 0.0 Then Speed.Direction = PlayerMovement.Direction
        End If

        'Player tank movement
        If MainForm.Options.Preferences("PlayerMovementType") = "TankMovement" Then
            PlayerMovement = CType((Cursor.Position + New Size(MainForm.ViewOffsetX, MainForm.ViewOffsetY) - New Size(Room.XOffset, Room.YOffset) - MainForm.Bounds.Location - New Size(8, 31)), PointF) - Middle.XY
            PlayerMovement.Normalize()
            If OIStatus("Up") Then
                If (Not CurrentPlayerSpeed = 0.0) Then
                    Speed.Direction = PlayerMovement.Direction
                End If
            ElseIf OIStatus("Down") Then
                Properties("Acceleration") /= 2
                Properties("MaxSpeed") /= 2
                If (CurrentPlayerSpeed = 0.0) Then
                    PlayerMovement.Direction = PlayerMovement.Direction + Math.PI
                Else
                    Speed.Direction = PlayerMovement.Direction + Math.PI
                End If
            End If

            If OIStatus("Right") AndAlso Not OIStatus("Left") Then
                Properties("Acceleration") /= 1.25
                Properties("MaxSpeed") /= 1.25
                If (CurrentPlayerSpeed = 0.0) Then
                    PlayerMovement.Direction = PlayerMovement.Direction + (Math.PI / 2)
                Else
                    PlayerMovement.Direction = PlayerMovement.Direction + (Math.PI / 2)
                    PlayerMovement.Length = Properties("MaxSpeed")
                    Speed.Direction = (PlayerMovement + Speed).Direction
                End If
            ElseIf OIStatus("Left") AndAlso Not OIStatus("Right") Then
                Properties("Acceleration") /= 1.25
                Properties("MaxSpeed") /= 1.25
                If (CurrentPlayerSpeed = 0.0) Then
                    PlayerMovement.Direction = PlayerMovement.Direction - (Math.PI / 2)
                Else
                    PlayerMovement.Direction = PlayerMovement.Direction - (Math.PI / 2)
                    PlayerMovement.Length = Properties("MaxSpeed")
                    Speed.Direction = (PlayerMovement + Speed).Direction
                End If
            End If
        End If

        'Moving the player
        If (OIStatus("Up") Xor OIStatus("Down")) Or (OIStatus("Right") Xor OIStatus("Left")) Then
            If (CurrentPlayerSpeed = 0.0) Then
                Speed.X = PlayerMovement.X * Properties("Acceleration")
                Speed.Y = PlayerMovement.Y * Properties("Acceleration")
            ElseIf (CurrentPlayerSpeed < Properties("MaxSpeed") - Properties("Acceleration")) Then
                Speed.Length += Properties("Acceleration") / 2
            ElseIf (CurrentPlayerSpeed > Properties("MaxSpeed") + Properties("Acceleration")) Then
                Speed.Length -= Properties("Acceleration")
            ElseIf (CurrentPlayerSpeed <> Properties("MaxSpeed")) Then
                Speed.Length = Properties("MaxSpeed")
            End If
        Else
            If (CurrentPlayerSpeed > Properties("Acceleration")) Then
                Speed.Length -= Properties("Acceleration")
            ElseIf (CurrentPlayerSpeed <> 0.0) Then
                Speed.Length = 0.0
            End If
        End If

    End Sub
End Class