Public Class Player
    Inherits Actor
    Private Options As Options

    Public Sub New(Room As Room)
        MyBase.New(Room)
        Flags.Add("Player")
        Options = MainForm.Options
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
        UpdateMovement(t)
        UpdateActions(t)

    End Sub

    Private Sub UpdateMovement(t As Double)
        Dim CurrentPlayerSpeed As Double = Speed.Length
        Dim PlayerMovement As Vector2
        If Options.OIStatus("Sprint") AndAlso Properties("CurrentStamina") > 0.0 Then
            Properties("CurrentStamina") -= t * 10
            Properties("Acceleration") = 3
            Properties("MaxSpeed") = 13
        Else
            Properties("Acceleration") = 2
            Properties("MaxSpeed") = 8
        End If

        'Player arcade movement
        If Options.Preferences("PlayerMovementType") = "ArcadeMovement" Then
            If Options.OIStatus("Up") Then
                PlayerMovement.Y -= 1
            End If
            If Options.OIStatus("Down") Then
                PlayerMovement.Y += 1
            End If
            If Options.OIStatus("Right") Then
                PlayerMovement.X += 1
            End If
            If Options.OIStatus("Left") Then
                PlayerMovement.X -= 1
            End If
            If (Options.OIStatus("Up") Or Options.OIStatus("Down") Or Options.OIStatus("Right") Or Options.OIStatus("Left")) AndAlso Not CurrentPlayerSpeed = 0.0 Then Speed.Direction = PlayerMovement.Direction
        End If

        'Player tank movement
        If Options.Preferences("PlayerMovementType") = "TankMovement" Then
            PlayerMovement = CType((Cursor.Position + New Size(MainForm.ViewOffsetX, MainForm.ViewOffsetY) - New Size(Room.XOffset, Room.YOffset) - MainForm.Bounds.Location - New Size(8, 31)), PointF) - Middle.XY
            PlayerMovement.Normalize()
            If Options.OIStatus("Up") Then
                If (Not CurrentPlayerSpeed = 0.0) Then
                    Speed.Direction = PlayerMovement.Direction
                End If
            ElseIf Options.OIStatus("Down") Then
                Properties("Acceleration") /= 2
                Properties("MaxSpeed") /= 2
                If (CurrentPlayerSpeed = 0.0) Then
                    PlayerMovement.Direction = PlayerMovement.Direction + Math.PI
                Else
                    Speed.Direction = PlayerMovement.Direction + Math.PI
                End If
            End If

            If Options.OIStatus("Right") AndAlso Not Options.OIStatus("Left") Then
                Properties("Acceleration") /= 1.25
                Properties("MaxSpeed") /= 1.25
                If (CurrentPlayerSpeed = 0.0) Then
                    PlayerMovement.Direction = PlayerMovement.Direction + (Math.PI / 2)
                Else
                    PlayerMovement.Direction = PlayerMovement.Direction + (Math.PI / 2)
                    PlayerMovement.Length = Properties("MaxSpeed")
                    Speed.Direction = (PlayerMovement + Speed).Direction
                End If
            ElseIf Options.OIStatus("Left") AndAlso Not Options.OIStatus("Right") Then
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
        If (Options.OIStatus("Up") Xor Options.OIStatus("Down")) Or (Options.OIStatus("Right") Xor Options.OIStatus("Left")) Then
            If (CurrentPlayerSpeed = 0.0) Then
                Speed.X = PlayerMovement.X * Properties("Acceleration")
                Speed.Y = PlayerMovement.Y * Properties("Acceleration")
            ElseIf (CurrentPlayerSpeed < Properties("MaxSpeed") - Properties("Acceleration")) Then
                Speed.Length += Properties("Acceleration") / 2
            ElseIf (CurrentPlayerSpeed > Properties("MaxSpeed") + CDbl(Properties("Acceleration"))) Then
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

    Private Sub UpdateActions(t As Double)
        If Options.OIStatus("PlaceCrate") Then PlaceCrate()
        If Options.OIStatus("AttackSkill") Then AttackSkill()
    End Sub

    Private Sub PlaceCrate()
        Dim X As Integer
        Dim Y As Integer
        Select Case Direction
            Case ActorDirection.South
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Position.X
                    Y = Position.Y + Image.Height + 1
                Else
                    X = Position.X
                    Y = Position.Y - My.Resources.Crate.Height + 20
                End If
            Case ActorDirection.North
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Position.X
                    Y = Position.Y - My.Resources.Crate.Height + 16
                Else
                    X = Position.X
                    Y = Position.Y + Image.Height - 3
                End If
            Case ActorDirection.West
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Position.X - My.Resources.Crate.Width - 1
                    Y = Position.Y + Image.Height - My.Resources.Crate.Height
                Else
                    X = Position.X + Image.Width
                    Y = Position.Y + Image.Height - My.Resources.Crate.Height
                End If
            Case ActorDirection.East
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Position.X + Image.Width + 1
                    Y = Position.Y + Image.Height - My.Resources.Crate.Height
                Else
                    X = Position.X - My.Resources.Crate.Width - 1
                    Y = Position.Y + Image.Height - My.Resources.Crate.Height
                End If
            Case ActorDirection.SouthWest
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Position.X - My.Resources.Crate.Width - 1
                    Y = Position.Y + Image.Height + 1
                Else
                    X = Position.X + Image.Width
                    Y = Position.Y - My.Resources.Crate.Height + 20
                End If
            Case ActorDirection.SouthEast
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Position.X + Image.Width + 1
                    Y = Position.Y + Image.Height + 1
                Else
                    X = Position.X - My.Resources.Crate.Width - 1
                    Y = Position.Y - My.Resources.Crate.Height + 20
                End If
            Case ActorDirection.NorthWest
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Position.X - My.Resources.Crate.Width - 1
                    Y = Position.Y - My.Resources.Crate.Height + 16
                Else
                    X = Position.X + Image.Width
                    Y = Position.Y + Image.Height - 3
                End If
            Case ActorDirection.NorthEast
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Position.X + Image.Width + 1
                    Y = Position.Y - My.Resources.Crate.Height + 16
                Else
                    X = Position.X - My.Resources.Crate.Width - 1
                    Y = Position.Y + Image.Height - 3
                End If
        End Select

        MainForm.ToAddWaitlist.Add(New Crate(Room, New Vector3(X, Y, 0), 10), Room)
    End Sub

    Private Sub AttackSkill()
        Hit(getNearList(Properties("AttackRange"), Properties("AttackAngle")))
    End Sub
End Class