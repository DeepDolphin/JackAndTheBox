Public Class GameObjectProperties
    Private Parent As GameObject

    Public Sub New(GameObject As GameObject, ObjectProperties As FlagsEnum(), PropertiesArray As String())
        Parent = GameObject

        For Each ObjectProperty As FlagsEnum In ObjectProperties
            Flags.Set(ObjectProperty, True)
        Next

        Properties("MaxHealth") = PropertiesArray(0)
        Properties("Health") = PropertiesArray(0)
    End Sub

    Public ReadOnly Property IsMoving As Boolean
        Get
            Return (Not Parent.Speed.X = 0 OrElse Not Parent.Speed.Y = 0)
        End Get
    End Property

#Region "Properties Dictionary"
    Protected Properties As New Dictionary(Of String, String)

    Public ReadOnly Property MaxHealth As Double
        Get
            Return Properties("MaxHealth")
        End Get
    End Property

    Public Property Health As Double
        Get
            Return Properties("Health")
        End Get
        Set(value As Double)
            Properties("Health") = value
            If Not Invulnerable AndAlso Health <= 0.0 Then
                Flags.Set(FlagsEnum.Dead, True)
            End If
        End Set
    End Property

#End Region

#Region "Flags Enum"
    Protected Flags As New BitArray(FlagsEnum.Max)



    Public ReadOnly Property FloorObject As Boolean
        Get
            Return Flags.Get(FlagsEnum.FloorObject)
        End Get
    End Property

    Public ReadOnly Property CastsShadow As Boolean
        Get
            Return Flags.Get(FlagsEnum.CastsShadow)
        End Get
    End Property

    Public ReadOnly Property Collidable As Boolean
        Get
            Return Flags.Get(FlagsEnum.Collidable)
        End Get
    End Property

    Public ReadOnly Property Invulnerable As Boolean
        Get
            Return Flags.Get(FlagsEnum.Invulnerable)
        End Get
    End Property

    Public ReadOnly Property Visible As Boolean
        Get
            Return Flags.Get(FlagsEnum.Visible)
        End Get
    End Property

    Public ReadOnly Property Dead As Boolean
        Get
            Return Flags.Get(FlagsEnum.Dead)
        End Get
    End Property

    Public Property Collided As Boolean
        Get
            Return Flags.Get(FlagsEnum.Collided)
        End Get
        Set(value As Boolean)
            Flags.Set(FlagsEnum.Collided, value)
        End Set
    End Property

    Public Property Dirty As Boolean
        Get
            Return Flags.Get(FlagsEnum.Dirty)
        End Get
        Set(value As Boolean)
            Flags.Set(FlagsEnum.Dirty, value)
        End Set
    End Property

    Public Enum FlagsEnum
        CastsShadow
        FloorObject
        Collidable
        Invulnerable
        Visible
        Dead
        Collided
        Dirty

        Max ' Don't use, only for bounds of enum
    End Enum
#End Region
End Class

Public Class ActorProperties
    Inherits GameObjectProperties

    Public Sub New(GameObject As GameObject, ObjectProperties As FlagsEnum(), PropertiesArray As String())
        MyBase.New(GameObject, ObjectProperties, PropertiesArray)

        Properties("MaxStamina") = PropertiesArray(1)
        Properties("Stamina") = PropertiesArray(1)
        Properties("MaxMana") = PropertiesArray(2)
        Properties("Mana") = PropertiesArray(2)
        Properties("MaxSpeed") = PropertiesArray(3)
        Properties("Acceleration") = PropertiesArray(4)
    End Sub

#Region "Properties Dictionary"
    Public ReadOnly Property MaxStamina As Double
        Get
            Return Properties("MaxStamina")
        End Get
    End Property

    Public ReadOnly Property MaxMana As Double
        Get
            Return Properties("MaxMana")
        End Get
    End Property

    Public Property Stamina As Double
        Get
            Return Properties("Stamina")
        End Get
        Set(value As Double)
            Properties("Stamina") = value
        End Set
    End Property

    Public Property Mana As Double
        Get
            Return Properties("Mana")
        End Get
        Set(value As Double)
            Properties("Mana") = value
        End Set
    End Property

    Public Property MaxSpeed As Double
        Get
            Return Properties("MaxSpeed")
        End Get
        Set(value As Double)
            Properties("MaxSpeed") = value
        End Set
    End Property

    Public Property Acceleration As Double
        Get
            Return Properties("Acceleration")
        End Get
        Set(value As Double)
            Properties("Acceleration") = value
        End Set
    End Property


#End Region
End Class
