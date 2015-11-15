Public MustInherit Class Ability
    Public CurrentCooldown As Double
    Public ReadOnly MaxCooldown As Double

    Public Sub New(Cooldown As Double)
        CurrentCooldown = Cooldown
        MaxCooldown = Cooldown
    End Sub

    Public MustOverride Sub Run()
    Public Overridable Sub Update(t As Double)
        If CurrentCooldown > 0.0 Then
            CurrentCooldown -= t
        ElseIf CurrentCooldown <> 0.0 Then
            CurrentCooldown = 0.0
        End If
    End Sub
End Class

Public Class PlaceCrateAbility
    Inherits Ability

    Private Player As Player = MainForm.Player
    Private Options As Options = MainForm.Options

    Public Sub New(Cooldown As Double)
        MyBase.New(Cooldown)
    End Sub

    Public Overrides Sub Run()
        If Not CurrentCooldown = 0.0 Then Return

        Dim X As Integer
        Dim Y As Integer
        Select Case Player.Direction
            Case Player.ActorDirection.South
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Player.Position.X
                    Y = Player.Position.Y + Player.Sprite.Height + 1
                Else
                    X = Player.Position.X
                    Y = Player.Position.Y - My.Resources.Crate.Height + 20
                End If
            Case Player.ActorDirection.North
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Player.Position.X
                    Y = Player.Position.Y - My.Resources.Crate.Height + 16
                Else
                    X = Player.Position.X
                    Y = Player.Position.Y + Player.Sprite.Height - 3
                End If
            Case Player.ActorDirection.West
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Player.Position.X - My.Resources.Crate.Width - 1
                    Y = Player.Position.Y + Player.Sprite.Height - My.Resources.Crate.Height
                Else
                    X = Player.Position.X + Player.Sprite.Width
                    Y = Player.Position.Y + Player.Sprite.Height - My.Resources.Crate.Height
                End If
            Case Player.ActorDirection.East
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Player.Position.X + Player.Sprite.Width + 1
                    Y = Player.Position.Y + Player.Sprite.Height - My.Resources.Crate.Height
                Else
                    X = Player.Position.X - My.Resources.Crate.Width - 1
                    Y = Player.Position.Y + Player.Sprite.Height - My.Resources.Crate.Height
                End If
            Case Player.ActorDirection.SouthWest
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Player.Position.X - My.Resources.Crate.Width - 1
                    Y = Player.Position.Y + Player.Sprite.Height + 1
                Else
                    X = Player.Position.X + Player.Sprite.Width
                    Y = Player.Position.Y - My.Resources.Crate.Height + 20
                End If
            Case Player.ActorDirection.SouthEast
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Player.Position.X + Player.Sprite.Width + 1
                    Y = Player.Position.Y + Player.Sprite.Height + 1
                Else
                    X = Player.Position.X - My.Resources.Crate.Width - 1
                    Y = Player.Position.Y - My.Resources.Crate.Height + 20
                End If
            Case Player.ActorDirection.NorthWest
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Player.Position.X - My.Resources.Crate.Width - 1
                    Y = Player.Position.Y - My.Resources.Crate.Height + 16
                Else
                    X = Player.Position.X + Player.Sprite.Width
                    Y = Player.Position.Y + Player.Sprite.Height - 3
                End If
            Case Player.ActorDirection.NorthEast
                If (Not Options.OIStatus("Up") Or Options.Preferences("PlayerMovementType") = "ArcadeMovement") Then
                    X = Player.Position.X + Player.Sprite.Width + 1
                    Y = Player.Position.Y - My.Resources.Crate.Height + 16
                Else
                    X = Player.Position.X - My.Resources.Crate.Width - 1
                    Y = Player.Position.Y + Player.Sprite.Height - 3
                End If
        End Select

        MainForm.ToAddWaitlist.Add(New Crate(Player.Room, New Vector3(X, Y, 0)))
        CurrentCooldown = MaxCooldown
    End Sub
End Class
