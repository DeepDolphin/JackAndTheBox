Public Class Actor
    Inherits GameObject
    Public Speed As Double
    Public Enum ActorDirection
        Up = 0
        Right
        Down
        Left
    End Enum
    Public Direction As ActorDirection = ActorDirection.Up

    Public Sub New(Room As Room, X As Double, Y As Double, Speed As Double)
        MyBase.New(My.Resources.CharacterUp1, Room, X, Y, 100)
        HitBox = New RectangleF(0, 22, 16, 10)
        Me.Speed = Speed
        Properties.Add("Attack Cooldown", "1")
        Properties.Add("Attack", "10")
        Properties.Add("attackRange", "5")
        Properties.Add("attackAngle", "180")
        Flags.Add("actor")
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
        If (Properties("Attack Cooldown") > 0.0) Then Properties("Attack Cooldown") -= t
    End Sub

    Public Overridable Sub Hit(O As GameObject)
        If (Properties("Attack Cooldown") <= 0.0) Then
            If O.Properties.Keys.Contains("Health") AndAlso O.Properties("Health") >= 0.0 Then O.Properties("Health") -= Properties("Attack")
            Properties("Attack Cooldown") = "1"
        End If
    End Sub

    Public Function getHit() As List(Of GameObject)
        Dim objectList As List(Of GameObject) = New List(Of GameObject)
        Dim realRange As Double = Properties("attackRange") * HitBox.Width
        Dim angleAsRadUpper As Double = ((Direction * -90) + Properties("attackAngle")) * (Math.PI / 180.0)
        Dim angleAsRadLower As Double = (Direction * -90) * (Math.PI / 180.0)

        For Each gameObject As GameObject In Room.GameObjects
            If Math.Sqrt(Math.Pow((gameObject.Y - Y), 2) + Math.Pow((gameObject.X - X), 2)) <= realRange Then
                Dim rad As Double = Math.Atan2(gameObject.Y - Y, gameObject.X - X)
                If rad <= angleAsRadUpper AndAlso rad >= angleAsRadLower Then
                    objectList.Add(gameObject)
                End If
            End If
        Next

        Return objectList
    End Function



End Class
