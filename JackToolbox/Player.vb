Imports JackPhysics
Imports System.Drawing
Imports System.Windows.Forms

Public Class Player
    Inherits Actor

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, PropertyArray As String())
        MyBase.New(Image, Room, Position, PropertyArray)
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
        UpdateMovement(t)
        UpdateActions(t)
    End Sub

    Private Sub UpdateMovement(t As Double)
        Dim CurrentPlayerSpeed As Double = Speed.Length
        Dim PlayerMovement As Vector2
        If Options.OIStatus("Sprint") AndAlso Properties.Stamina > 0.0 Then
            Properties.Stamina -= t * 10
            Properties.Acceleration = 3
            Properties.MaxSpeed = 13
        Else
            Properties.Acceleration = 2
            Properties.MaxSpeed = 8
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
            If ((Options.OIStatus("Up") Xor Options.OIStatus("Down")) Or (Options.OIStatus("Right") Xor Options.OIStatus("Left"))) AndAlso Not CurrentPlayerSpeed = 0.0 Then Speed.Direction = PlayerMovement.Direction
        End If

        'Player tank movement
        If Options.Preferences("PlayerMovementType") = "TankMovement" Then
            'PlayerMovement = CType((Cursor.Position + New Size(IGame.ViewOffsetX, IGame.ViewOffsetY) - New Size(Room.XOffset, Room.YOffset) - Mainform.size.Location - New Size(8, 31)), PointF) - Middle.XY
            PlayerMovement.Normalize()
            If Options.OIStatus("Up") Then
                If (Not CurrentPlayerSpeed = 0.0) Then
                    Speed.Direction = PlayerMovement.Direction
                End If
            ElseIf Options.OIStatus("Down") Then
                Properties.Acceleration /= 2
                Properties.MaxSpeed /= 2
                If (CurrentPlayerSpeed = 0.0) Then
                    PlayerMovement.Direction = PlayerMovement.Direction + Math.PI
                Else
                    Speed.Direction = PlayerMovement.Direction + Math.PI
                End If
            End If

            If Options.OIStatus("Right") AndAlso Not Options.OIStatus("Left") Then
                Properties.Acceleration /= 1.25
                Properties.MaxSpeed /= 1.25
                If (CurrentPlayerSpeed = 0.0) Then
                    PlayerMovement.Direction = PlayerMovement.Direction + (Math.PI / 2)
                Else
                    PlayerMovement.Direction = PlayerMovement.Direction + (Math.PI / 2)
                    PlayerMovement.Length = Properties.MaxSpeed
                    Speed.Direction = (PlayerMovement + Speed).Direction
                End If
            ElseIf Options.OIStatus("Left") AndAlso Not Options.OIStatus("Right") Then
                Properties.Acceleration /= 1.25
                Properties.MaxSpeed /= 1.25
                If (CurrentPlayerSpeed = 0.0) Then
                    PlayerMovement.Direction = PlayerMovement.Direction - (Math.PI / 2)
                Else
                    PlayerMovement.Direction = PlayerMovement.Direction - (Math.PI / 2)
                    PlayerMovement.Length = Properties.MaxSpeed
                    Speed.Direction = (PlayerMovement + Speed).Direction
                End If
            End If
        End If

        'Moving the player
        If (Options.OIStatus("Up") Xor Options.OIStatus("Down")) Or (Options.OIStatus("Right") Xor Options.OIStatus("Left")) Then
            If (CurrentPlayerSpeed = 0.0) Then
                Speed.X = PlayerMovement.X * Properties.Acceleration
                Speed.Y = PlayerMovement.Y * Properties.Acceleration
            ElseIf (CurrentPlayerSpeed < Properties.MaxSpeed - Properties.Acceleration) Then
                Speed.Length += Properties.Acceleration / 2
            ElseIf (CurrentPlayerSpeed > Properties.MaxSpeed + CDbl(Properties.Acceleration)) Then
                Speed.Length -= Properties.Acceleration
            ElseIf (CurrentPlayerSpeed <> Properties.MaxSpeed) Then
                Speed.Length = Properties.MaxSpeed
            End If
        Else
            If (CurrentPlayerSpeed > Properties.Acceleration) Then
                Speed.Length -= Properties.Acceleration
            ElseIf (CurrentPlayerSpeed <> 0.0) Then
                Speed.Length = 0.0
            End If
        End If

        'Update Direction of the Player
        If (Options.OIStatus("Up") Xor Options.OIStatus("Down")) Or (Options.OIStatus("Right") Xor Options.OIStatus("Left")) Then
            Direction = ToActorDirection(Speed.Direction)
        End If

    End Sub

    Private Sub UpdateActions(t As Double)
        If Options.OIStatus("UtilityAbility") Then
            If Inventory.UtilityAbility IsNot Nothing Then Inventory.UtilityAbility.Run()
            Options.OIStatus("UtilityAbility") = False
        End If
        If Options.OIStatus("ActiveAbility") Then
            If Inventory.ActiveAbility IsNot Nothing Then Inventory.ActiveAbility.Run()
            Options.OIStatus("ActiveAbility") = False
        End If
        If Options.MouseWheel <> 0 Then

            Options.MouseWheel = 0
        End If

    End Sub

    Private Sub ActiveAbility()
        'Hit(getNearList(Properties("AttackRange"), Properties("AttackAngle")))

        '    Dim X As Integer
        '    Dim Y As Integer
        '    Select Case Placer.Direction
        '        Case Player.ActorDirection.South
        '            If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
        '                X = Placer.Position.X
        '                Y = Placer.Position.Y + Placer.Sprite.Height + 1
        '            Else
        '                X = Placer.Position.X
        '                Y = Placer.Position.Y - My.Resources.Crate.Height + 20
        '            End If
        '        Case Player.ActorDirection.North
        '            If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
        '                X = Placer.Position.X
        '                Y = Placer.Position.Y - My.Resources.Crate.Height + 16
        '            Else
        '                X = Placer.Position.X
        '                Y = Placer.Position.Y + Placer.Sprite.Height - 3
        '            End If
        '        Case Player.ActorDirection.West
        '            If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
        '                X = Placer.Position.X - My.Resources.Crate.Width - 1
        '                Y = Placer.Position.Y + Placer.Sprite.Height - My.Resources.Crate.Height
        '            Else
        '                X = Placer.Position.X + Placer.Sprite.Width
        '                Y = Placer.Position.Y + Placer.Sprite.Height - My.Resources.Crate.Height
        '            End If
        '        Case Player.ActorDirection.East
        '            If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
        '                X = Placer.Position.X + Placer.Sprite.Width + 1
        '                Y = Placer.Position.Y + Placer.Sprite.Height - My.Resources.Crate.Height
        '            Else
        '                X = Placer.Position.X - My.Resources.Crate.Width - 1
        '                Y = Placer.Position.Y + Placer.Sprite.Height - My.Resources.Crate.Height
        '            End If
        '        Case Player.ActorDirection.SouthWest
        '            If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
        '                X = Placer.Position.X - My.Resources.Crate.Width - 1
        '                Y = Placer.Position.Y + Placer.Sprite.Height + 1
        '            Else
        '                X = Placer.Position.X + Placer.Sprite.Width
        '                Y = Placer.Position.Y - My.Resources.Crate.Height + 20
        '            End If
        '        Case Player.ActorDirection.SouthEast
        '            If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
        '                X = Placer.Position.X + Placer.Sprite.Width + 1
        '                Y = Placer.Position.Y + Placer.Sprite.Height + 1
        '            Else
        '                X = Placer.Position.X - My.Resources.Crate.Width - 1
        '                Y = Placer.Position.Y - My.Resources.Crate.Height + 20
        '            End If
        '        Case Player.ActorDirection.NorthWest
        '            If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
        '                X = Placer.Position.X - My.Resources.Crate.Width - 1
        '                Y = Placer.Position.Y - My.Resources.Crate.Height + 16
        '            Else
        '                X = Placer.Position.X + Placer.Sprite.Width
        '                Y = Placer.Position.Y + Placer.Sprite.Height - 3
        '            End If
        '        Case Player.ActorDirection.NorthEast
        '            If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
        '                X = Placer.Position.X + Placer.Sprite.Width + 1
        '                Y = Placer.Position.Y - My.Resources.Crate.Height + 16
        '            Else
        '                X = Placer.Position.X - My.Resources.Crate.Width - 1
        '                Y = Placer.Position.Y + Placer.Sprite.Height - 3
        '            End If
        '    End Select

    End Sub


End Class