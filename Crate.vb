Imports JackPhysics

Public Class Crate
    Inherits GameObject

    Public Sub New(Room As Room, Position As Vector3)
        MyBase.New(My.Resources.Crate, Room, Position, {10}, {GameObjectProperties.FlagsEnum.CastsShadow, GameObjectProperties.FlagsEnum.Collidable, GameObjectProperties.FlagsEnum.Visible})
    End Sub

    Public Sub New(Sprite As Sprite, Room As Room, Position As Vector3)
        MyBase.New(Sprite, Room, Position, {10}, {GameObjectProperties.FlagsEnum.CastsShadow, GameObjectProperties.FlagsEnum.Collidable, GameObjectProperties.FlagsEnum.Visible})
    End Sub

End Class

Public Class ExplosiveCrate
    Inherits Crate
    Private timer As Double = 2.5

    Public Sub New(Room As Room, Position As Vector3)
        MyBase.New(Room, Position)
    End Sub

    Public Sub Explode()
        Dim objects As List(Of GameObject) = getNearList(5)

        'For Each o As GameObject In objects
        '    If o.Properties.ContainsKey("Health") Then o.Properties("Health") -= 10
        'Next

        Properties.Health = 0.0
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)

        If timer <= 0.0 Then
            Explode()
        Else
            timer -= t
        End If


    End Sub
End Class
