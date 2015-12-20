Imports System.IO
Imports System.Drawing
Imports JackPhysics

Public Module Resources
    Private ResourceBitmaps As Dictionary(Of String, Bitmap)
    Private ResourceGameObjects As Dictionary(Of String, GameObject)
    Private ResourceRooms As Dictionary(Of String, Room)

    Public Function getNewGameObject(Name As String, Room As Room, Position As Vector3)
        Dim GameObject As New GameObject(ResourceGameObjects(Name))

        GameObject.Room = Room
        GameObject.Position = Position
        Return GameObject
    End Function

    Private EXPOrb As Sprite
        Private Player As Sprite


        Public Shadow As Bitmap
        Public GradientLeft As Bitmap
        Public GradientRight As Bitmap
        Public ShadeBrush As SolidBrush = New SolidBrush(Color.FromArgb(175, 0, 0, 0))
        Public HealthBackground As Bitmap
        Public HealthBar As Bitmap

    Public Sub Import()

        For Each file As FileInfo In New DirectoryInfo("resources").GetFiles
            If file.Extension.Equals(".jbrp") Then
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

                Dim gameObjectCount As Integer = Reader.ReadInt32()
                For value As Integer = 0 To imageCount - 1
                    Dim name As String = Reader.ReadString
                    Dim gameObject As GameObject = GameObject.fromBytes(Reader)
                    ResourceGameObjects.Add(name, gameObject)
                Next

                Reader.Dispose()
            End If
        Next



        'Shadow = New Bitmap(My.Resources.Shadow)
        'GradientRight = New Bitmap(My.Resources.GradientRight)
        'GradientLeft = New Bitmap(My.Resources.GradientLeft)
        'HealthBackground = New Bitmap(My.Resources.HealthBackground)
        'HealthBar = New Bitmap(My.Resources.HealthBar)

    End Sub

    Public Sub Export()

    End Sub

End Module
