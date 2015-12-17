Public MustInherit Class Objective
    Public MustOverride Function IsComplete(Room As Room) As Boolean
    Public Overridable Sub Update(t As Double)

    End Sub
End Class

Public Class SurvivalTimeObjective
    Inherits Objective

    Public ElapsedTime As Double = 0
    Public TargetTime As Double = 0

    Public Sub New(TargetTime As Double)
        Me.TargetTime = TargetTime
    End Sub

    Public Overrides Function IsComplete(Room As Room) As Boolean
        Return ElapsedTime >= TargetTime
    End Function

    Public Overrides Sub Update(t As Double)
        ElapsedTime += t
    End Sub
End Class

Public Class KillAllEnemiesObjective
    Inherits Objective

    Public Overrides Function IsComplete(Room As Room) As Boolean
        For Each o As GameObject In Room.GameObjects
            'If TypeOf o Is Enemy Then
            '    Return False
            'End If
        Next
        Return True
    End Function
End Class