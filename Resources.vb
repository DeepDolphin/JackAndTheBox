Imports System.IO

Public Class Resources
    Public Shared ReadOnly Property getInstance As Resources
        Get
            Return Resources
        End Get
    End Property
    Private Shared Resources As Resources
    Private ResourceBitmaps As Dictionary(Of String, Bitmap)
    Private ResourceGameObjects As Dictionary(Of String, GameObject)
    Private ResourceRooms As Dictionary(Of String, Room)

    Private EXPOrb As Sprite
    Private Player As Sprite


    Public Shadow As Bitmap
    Public GradientLeft As Bitmap
    Public GradientRight As Bitmap
    Public ShadeBrush As SolidBrush = New SolidBrush(Color.FromArgb(175, 0, 0, 0))
    Public HealthBackground As Bitmap
    Public HealthBar As Bitmap

    Public Sub New()
        Resources = Me

        For Each file As FileInfo In New DirectoryInfo("resources").GetFiles
            If file.Extension.Equals("jbrp") Then
                Dim Reader As New BinaryReader(New FileStream(file.FullName, FileMode.Open))
                Dim imageCount As Integer = Reader.ReadInt32()

                For value As Integer = 0 To imageCount - 1
                    Dim name As String = Reader.ReadString
                    Dim byteCount As Integer = Reader.ReadInt32()
                    Dim ms As New MemoryStream()
                    ms.Write(Reader.ReadBytes(byteCount), 0, byteCount)
                    Dim bitmap As New Bitmap(ms)
                    ResourceBitmaps.Add(name, bitmap)
                    ms.Dispose()
                Next

                Dim propertyCount As Integer = Reader.ReadInt32()
                Dim propertyArray(propertyCount) As String

                For value As Integer = 0 To propertyCount - 1
                    Dim gottenProperty As String = Reader.ReadString()
                    propertyArray(value) = gottenProperty
                Next

                Dim objectPropertyCount As Integer = Reader.ReadInt32()
                Dim objectPropertyArray(objectPropertyCount) As GameObjectProperties.FlagsEnum

                For value As Integer = 0 To propertyCount - 1
                    Dim gottenProperty As GameObjectProperties.FlagsEnum = Reader.ReadInt32()
                    objectPropertyArray(value) = gottenProperty
                Next

                Reader.Dispose()
            End If
        Next



        Shadow = New Bitmap(My.Resources.Shadow)
        GradientRight = New Bitmap(My.Resources.GradientRight)
        GradientLeft = New Bitmap(My.Resources.GradientLeft)
        HealthBackground = New Bitmap(My.Resources.HealthBackground)
        HealthBar = New Bitmap(My.Resources.HealthBar)

    End Sub
End Class
