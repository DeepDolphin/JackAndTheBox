Public MustInherit Class Ability
    Public CurrentCooldown As Double
    Public ReadOnly MaxCooldown As Double
    Protected Parent As GameObject

    Public Sub New(Cooldown As Double, Parent As GameObject)
        CurrentCooldown = Cooldown
        MaxCooldown = Cooldown
        Me.Parent = Parent
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

Public Class PlaceAbility
    Inherits Ability

    Public Placer As Actor
    Public InFront As Boolean
    Private Options As Options = MainForm.Options

    Public Sub New(Cooldown As Double, Parent As GameObject, Placer As Actor, InFront As Boolean)
        MyBase.New(Cooldown, Parent)
        Me.Placer = Placer
        Me.InFront = InFront
    End Sub

    Public Overrides Sub Run()
        If Not CurrentCooldown = 0.0 Then Return

        Dim X As Integer
        Dim Y As Integer
        Select Case Placer.Direction
            Case Actor.ActorDirection.South
                If (InFront) Then
                    X = Placer.Position.X
                    Y = Placer.Position.Y + Placer.Sprite.Height + 1
                Else
                    X = Placer.Position.X
                    Y = Placer.Position.Y - My.Resources.Crate.Height + 20
                End If
            Case Actor.ActorDirection.North
                If (InFront) Then
                    X = Placer.Position.X
                    Y = Placer.Position.Y - My.Resources.Crate.Height + 16
                Else
                    X = Placer.Position.X
                    Y = Placer.Position.Y + Placer.Sprite.Height - 3
                End If
            Case Actor.ActorDirection.West
                If (InFront) Then
                    X = Placer.Position.X - My.Resources.Crate.Width - 1
                    Y = Placer.Position.Y + Placer.Sprite.Height - My.Resources.Crate.Height
                Else
                    X = Placer.Position.X + Placer.Sprite.Width
                    Y = Placer.Position.Y + Placer.Sprite.Height - My.Resources.Crate.Height
                End If
            Case Actor.ActorDirection.East
                If (InFront) Then
                    X = Placer.Position.X + Placer.Sprite.Width + 1
                    Y = Placer.Position.Y + Placer.Sprite.Height - My.Resources.Crate.Height
                Else
                    X = Placer.Position.X - My.Resources.Crate.Width - 1
                    Y = Placer.Position.Y + Placer.Sprite.Height - My.Resources.Crate.Height
                End If
            Case Actor.ActorDirection.SouthWest
                If (InFront) Then
                    X = Placer.Position.X - My.Resources.Crate.Width - 1
                    Y = Placer.Position.Y + Placer.Sprite.Height + 1
                Else
                    X = Placer.Position.X + Placer.Sprite.Width
                    Y = Placer.Position.Y - My.Resources.Crate.Height + 20
                End If
            Case Actor.ActorDirection.SouthEast
                If (InFront) Then
                    X = Placer.Position.X + Placer.Sprite.Width + 1
                    Y = Placer.Position.Y + Placer.Sprite.Height + 1
                Else
                    X = Placer.Position.X - My.Resources.Crate.Width - 1
                    Y = Placer.Position.Y - My.Resources.Crate.Height + 20
                End If
            Case Actor.ActorDirection.NorthWest
                If (InFront) Then
                    X = Placer.Position.X - My.Resources.Crate.Width - 1
                    Y = Placer.Position.Y - My.Resources.Crate.Height + 16
                Else
                    X = Placer.Position.X + Placer.Sprite.Width
                    Y = Placer.Position.Y + Placer.Sprite.Height - 3
                End If
            Case Actor.ActorDirection.NorthEast
                If (InFront) Then
                    X = Placer.Position.X + Placer.Sprite.Width + 1
                    Y = Placer.Position.Y - My.Resources.Crate.Height + 16
                Else
                    X = Placer.Position.X - My.Resources.Crate.Width - 1
                    Y = Placer.Position.Y + Placer.Sprite.Height - 3
                End If
        End Select

        Parent.Position = New Vector3(X, Y, Parent.Position.Z)
        MainForm.ToAddWaitlist.Add(Parent)
        'MainForm.ToAddWaitlist.Add(New Crate(Player.Room, New Vector3(X, Y, 0)))
        CurrentCooldown = MaxCooldown
    End Sub
End Class
