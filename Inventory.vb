Public Class Inventory
    Public Property ActiveSkill As Skill
        Get
            Return Nothing
        End Get
        Set(value As Skill)

        End Set
    End Property
    Public Property UtilitySkill As Skill
        Get
            Return Nothing
        End Get
        Set(value As Skill)

        End Set
    End Property

    Public Equipped As New Dictionary(Of String, GameObject)
    Public Inventory As New List(Of GameObject)


    Public Sub New()
        EquippedInit()
    End Sub

    Public Sub New(Inventory As List(Of GameObject))
        Me.Inventory = Inventory
        EquippedInit()
    End Sub

    Private Sub EquippedInit()
        Equipped.Add("RightHand", Nothing)
        Equipped.Add("LeftHand", Nothing)

        Equipped.Add("Feet", Nothing)
        Equipped.Add("Arms", Nothing)
        Equipped.Add("Legs", Nothing)
        Equipped.Add("Chest", Nothing)
        Equipped.Add("Head", Nothing)
    End Sub



End Class
