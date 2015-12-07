'Public Class Enemy
'    Inherits Actor

'    Public Sub New(Room As Room, x As Double, y As Double)
'        MyBase.New(My.Resources.CharacterUp1, Room, New Vector3(x, y, 0))
'        Properties("AttackPower") = "1"
'    End Sub

'    Public Overrides Sub Update(t As Double)
'        MyBase.Update(t)
'    End Sub
'End Class

'Public Class NormalEnemy
'    Inherits Enemy

'    Public Sub New(Room As Room, x As Double, y As Double)
'        MyBase.New(Room, x, y)
'    End Sub

'    Public Overrides Sub update(t As Double)
'        MyBase.Update(t)

'        'Gets directly to player
'        Dim toPlayer As Vector2 = Game.Player.Middle.XY - Middle.XY
'        'If Player is outside of detection range then stop
'        If (toPlayer.Length >= 150.0) Then
'            If Not Speed.Length = 0 Then Speed.Length = 0
'            Return
'        End If

'        'Set the length of speed
'        Speed = toPlayer
'        If toPlayer.Length < 75.0 Then
'            Speed.Length = 8
'        ElseIf toPlayer.Length < 150.0 Then
'            Speed.Length = 4
'        End If
'    End Sub
'End Class

'Public Class SmartEnemy
'    Inherits Enemy

'    Public Sub New(Room As Room, x As Double, y As Double)
'        MyBase.New(Room, x, y)
'    End Sub

'    Public Overrides Sub update(t As Double)
'        MyBase.Update(t)

'        'Gets directly to player
'        Dim toPlayer As Vector2 = Game.Player.Middle.XY - Middle.XY
'        'If Player is outside of detection range then stop
'        If (toPlayer.Length >= 150.0) Then
'            If Not Speed.Length = 0 Then Speed.Length = 0
'            Return
'        End If

'        'Get the direction to where the player will be


'        'Set the length of speed
'        Speed = toPlayer
'        If toPlayer.Length < 75.0 Then
'            Speed.Length = 8
'        ElseIf toPlayer.Length < 150.0 Then
'            Speed.Length = 4
'        End If
'    End Sub
'End Class
