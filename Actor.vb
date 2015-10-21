Public Class Actor
    Inherits GameObject
    Public Speed As Double
    Public Enum ActorDirection
        East = 0
        NorthEast = -(Math.PI / 4)
        North = -(Math.PI / 2)
        NorthWest = -(3 * (Math.PI / 4))
        West = Math.PI
        SouthWest = 3 * (Math.PI / 4)
        South = (Math.PI / 2)
        SouthEast = (Math.PI / 4)
    End Enum
    Public Direction As ActorDirection = ActorDirection.North

    Public Sub New(Room As Room, X As Double, Y As Double, Speed As Double)
        MyBase.New(My.Resources.CharacterUp1, Room, X, Y, 100)
        HitBox = New RectangleF(0, 22, 16, 10)
        Me.Speed = Speed
        Properties.Add("Attack Cooldown", "1")
        Properties.Add("Attack", "10")
        Properties.Add("attackRange", "4")
        Properties.Add("attackAngle", "90")
        Properties.Add("test", "null")
        Flags.Add("actor")
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)


        If (Properties("Attack Cooldown") > 0.0) Then Properties("Attack Cooldown") -= t
    End Sub

    Public Overridable Sub Hit(O As GameObject)
        If (Properties("Attack Cooldown") <= 0.0) Then
            If O.Properties.Keys.Contains("Health") AndAlso O.Properties("Health") >= 0.0 Then
                O.Properties("Health") -= Properties("Attack")
                Properties("Attack Cooldown") = "1"
            End If
        End If
    End Sub

    Public Function getNearList(range As Double, angle As Double) As List(Of GameObject)
        Dim objectList As List(Of GameObject) = New List(Of GameObject)
        Dim realRange As Double = range * HitBox.Width
        Dim myMiddle As PointF = GetMiddle()

        For Each gameObject As GameObject In Room.GameObjects
            Dim objectMiddle As PointF = gameObject.GetMiddle()
            If Not gameObject.Equals(Me) AndAlso getDistanceTo(gameObject) <= realRange Then
                Dim rad As Double = getDirectionTo(gameObject)
                If gameObject.Properties.Count = 1 Then Properties("test") = rad
                If Math.Abs(Direction - rad) <= (angle * (Math.PI / 180.0)) / 2 Then
                    objectList.Add(gameObject)
                End If
            End If
        Next

        Return objectList
    End Function

    Function getClosestDirection(angle As Double) As ActorDirection

    End Function

End Class
