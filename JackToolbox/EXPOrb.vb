Imports JackPhysics
Imports System.Drawing

Public Class EXPOrb
    Inherits InventoryItem
    Public EXP As Integer = 0

    Public Sub New(EXP As Integer, Room As Room, Position As Vector3)
        MyBase.New(Resources.getNewGameObject("EXPOrb", Room, Position))
        Me.EXP = EXP
    End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
    End Sub
End Class
