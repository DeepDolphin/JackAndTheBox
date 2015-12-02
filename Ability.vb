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


        Dim X As Integer = User.Position.X
        Dim Y As Integer = User.Position.Y

        If Not InFront Then
            Direction = InvertDirection(Direction)
        End If

        Select Case Direction
            Case Actor.ActorDirection.South
                Y += User.Sprite.Height + 1
            Case Actor.ActorDirection.North
                Y += User.Sprite.Height - User.HitBox.Height - Parent.HitBox.Height - 1
            Case Actor.ActorDirection.West
                X -= Parent.Sprite.Width + 1
                Y += User.Sprite.Height - Parent.Sprite.Height
            Case Actor.ActorDirection.East
                X += User.Sprite.Width + 1
                Y += User.Sprite.Height - Parent.Sprite.Height
            Case Actor.ActorDirection.SouthWest
                X -= Parent.Sprite.Width + 1
                Y += User.Sprite.Height + 1
            Case Actor.ActorDirection.SouthEast
                X += User.Sprite.Width + 1
                Y += User.Sprite.Height + 1
            Case Actor.ActorDirection.NorthWest
                X -= Parent.Sprite.Width + 1
                Y += User.Sprite.Height - User.HitBox.Height - Parent.HitBox.Height - 1
            Case Actor.ActorDirection.NorthEast
                X += User.Sprite.Width + 1
                Y += User.Sprite.Height - User.HitBox.Height - Parent.HitBox.Height - 1
        End Select

        Parent.Position = New Vector3(X, Y, Parent.Position.Z)
        Parent.Room = User.Room
        Game.ToAddWaitlist.Add(Parent)
        CurrentCooldown = MaxCooldown
    End Sub

    Private Function InvertDirection(direction As Actor.ActorDirection) As Actor.ActorDirection
        Select Case direction
            Case Actor.ActorDirection.South
                Return Actor.ActorDirection.North
            Case Actor.ActorDirection.North
                Return Actor.ActorDirection.South
            Case Actor.ActorDirection.West
                Return Actor.ActorDirection.East
            Case Actor.ActorDirection.East
                Return Actor.ActorDirection.West
            Case Actor.ActorDirection.SouthWest
                Return Actor.ActorDirection.NorthEast
            Case Actor.ActorDirection.SouthEast
                Return Actor.ActorDirection.NorthWest
            Case Actor.ActorDirection.NorthWest
                Return Actor.ActorDirection.SouthEast
            Case Actor.ActorDirection.NorthEast
                Return Actor.ActorDirection.SouthWest
        End Select
        Throw New InvalidOperationException("Unexpected direction: " + direction)
    End Function

End Class
