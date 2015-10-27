Public Class Actor
    Inherits GameObject
    Public Speed As Double
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
    Public Shared Function ToDirection(radians As Double) As ActorDirection
        While radians < 0
            radians += Math.PI * 2
        End While
        Return CInt(((radians Mod (Math.PI * 2)) - Math.PI / 8) / (Math.PI / 4))
    End Function
    Public Shared Function ToRadians(direction As ActorDirection) As Double
        Return direction * -Math.PI / 4
    End Function

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
                If Math.Abs(ToRadians(Direction) - rad) <= (angle * (Math.PI / 180.0)) / 2 Then
                    objectList.Add(gameObject)
                End If
            End If
        Next

        Return objectList
    End Function

End Class
