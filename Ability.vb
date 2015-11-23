Public MustInherit Class Ability
    Public CurrentCooldown As Double
    Public ReadOnly MaxCooldown As Double
    Public Parent As GameObject
    Public User As Actor

    Public Sub New(Cooldown As Double, Parent As GameObject, User As Actor)
        CurrentCooldown = Cooldown
        MaxCooldown = Cooldown
        Me.Parent = Parent
        Me.User = User
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

    Public InFront As Boolean

    Public Sub New(Cooldown As Double, Parent As GameObject, User As Actor, InFront As Boolean)
        MyBase.New(Cooldown, Parent, User)
        Me.InFront = InFront
    End Sub

    Public Overrides Sub Run()
        If Not CurrentCooldown = 0.0 Then Return

        Dim Direction As Actor.ActorDirection = User.Direction


        Dim X As Integer
        Dim Y As Integer
        Select Case Direction
            Case Actor.ActorDirection.South
                If (InFront) Then
                    X = User.Position.X
                    Y = User.Position.Y + User.Sprite.Height + 1
                Else
                    X = User.Position.X
                    Y = User.Position.Y - Parent.Sprite.Height + 20
                End If
            Case Actor.ActorDirection.North
                If (InFront) Then
                    X = User.Position.X
                    Y = User.Position.Y - Parent.Sprite.Height + 16
                Else
                    X = User.Position.X
                    Y = User.Position.Y + User.Sprite.Height - 3
                End If
            Case Actor.ActorDirection.West
                If (InFront) Then
                    X = User.Position.X - Parent.Sprite.Width - 1
                    Y = User.Position.Y + User.Sprite.Height - Parent.Sprite.Height
                Else
                    X = User.Position.X + User.Sprite.Width
                    Y = User.Position.Y + User.Sprite.Height - Parent.Sprite.Height
                End If
            Case Actor.ActorDirection.East
                If (InFront) Then
                    X = User.Position.X + User.Sprite.Width + 1
                    Y = User.Position.Y + User.Sprite.Height - Parent.Sprite.Height
                Else
                    X = User.Position.X - Parent.Sprite.Width - 1
                    Y = User.Position.Y + User.Sprite.Height - Parent.Sprite.Height
                End If
            Case Actor.ActorDirection.SouthWest
                If (InFront) Then
                    X = User.Position.X - Parent.Sprite.Width - 1
                    Y = User.Position.Y + User.Sprite.Height + 1
                Else
                    X = User.Position.X + User.Sprite.Width
                    Y = User.Position.Y - Parent.Sprite.Height + 20
                End If
            Case Actor.ActorDirection.SouthEast
                If (InFront) Then
                    X = User.Position.X + User.Sprite.Width + 1
                    Y = User.Position.Y + User.Sprite.Height + 1
                Else
                    X = User.Position.X - Parent.Sprite.Width - 1
                    Y = User.Position.Y - Parent.Sprite.Height + 20
                End If
            Case Actor.ActorDirection.NorthWest
                If (InFront) Then
                    X = User.Position.X - Parent.Sprite.Width - 1
                    Y = User.Position.Y - Parent.Sprite.Height + 16
                Else
                    X = User.Position.X + User.Sprite.Width
                    Y = User.Position.Y + User.Sprite.Height - 3
                End If
            Case Actor.ActorDirection.NorthEast
                If (InFront) Then
                    X = User.Position.X + User.Sprite.Width + 1
                    Y = User.Position.Y - Parent.Sprite.Height + 16
                Else
                    X = User.Position.X - Parent.Sprite.Width - 1
                    Y = User.Position.Y + User.Sprite.Height - 3
                End If
        End Select

        Parent.Position = New Vector3(X, Y, Parent.Position.Z)
        Parent.Room = User.Room
        MainForm.ToAddWaitlist.Add(Parent)
        CurrentCooldown = MaxCooldown
    End Sub
End Class
