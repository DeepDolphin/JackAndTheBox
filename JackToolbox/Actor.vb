﻿Imports JackPhysics
Imports System.Drawing

Public Class Actor
    Inherits GameObject

    Public Inventory As New Inventory

    Public Shadows ReadOnly Property Properties As ActorProperties
        Get
            Return CType(_Properties, ActorProperties)
        End Get
    End Property

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

    Public Shared Function ToActorDirection(radians As Double) As ActorDirection
        While radians < 0
            radians += Math.PI * 2
        End While
        Return CInt((radians Mod (Math.PI * 2)) / (Math.PI / 4)) Mod 8
    End Function
    Public Shared Function ToRadians(direction As ActorDirection) As Double
        Return direction * -Math.PI / 4
    End Function

    Public Direction As ActorDirection = ActorDirection.North

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, PropertyArray As Dictionary(Of String, String))
        MyBase.New(Image, Room, Position, PropertyArray, {GameObjectProperties.FlagsEnum.CastsShadow, GameObjectProperties.FlagsEnum.Collidable})
    End Sub

    Public Sub New(Image As Bitmap, Room As Room, Position As Vector3, Speed As Vector2, PropertyArray As Dictionary(Of String, String))
        MyBase.New(Image, Room, Position, Speed, PropertyArray, {GameObjectProperties.FlagsEnum.CastsShadow, GameObjectProperties.FlagsEnum.Collidable})
    End Sub

    'Public Overrides Sub Init(Image As Sprite, Room As Room, Position As Vector3, Speed As Vector2, PropertyArray As Dictionary(Of String, String), ObjectProperties As GameObjectProperties.FlagsEnum())
    '    MyBase.Init(Image, Room, Position, Speed, PropertyArray, ObjectProperties)
    'End Sub

    Public Overrides Sub Update(t As Double)
        MyBase.Update(t)
    End Sub

    Public Overridable Sub Hit(O As GameObject)
        'If (Properties("ActiveCurrentCooldown") <= 0.0) Then
        '    If O.Properties.Keys.Contains("Health") AndAlso O.Properties("Health") >= 0.0 Then
        '        O.Properties("Health") -= Properties("AttackPower")
        '    End If
        'End If

        'ToDo: make hit an ability or something
    End Sub

    Public Overridable Sub Hit(OList As List(Of GameObject))
        'For Each gameObject As GameObject In OList
        '    Hit(gameObject)
        'Next
        'Properties("AttackCurrentCooldown") = Properties("AttackMaxCooldown")
    End Sub

    Public Overloads Function getNearList(range As Double, angle As Double) As List(Of GameObject)
        Dim objectList As List(Of GameObject) = New List(Of GameObject)
        Dim realRange As Double = range * HitBox.Width

        For Each gameObject As GameObject In Room.GameObjects
            If Not gameObject.Equals(Me) AndAlso getDistanceTo(gameObject) <= realRange Then
                Dim rad As Double = getDirectionTo(gameObject)
                If Math.Abs(ToRadians(Direction) - ToRadians(ToActorDirection(rad))) <= (angle * (Math.PI / 180.0)) / 2 Then
                    objectList.Add(gameObject)
                End If
            End If
        Next

        Return objectList
    End Function
End Class