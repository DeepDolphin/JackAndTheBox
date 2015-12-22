Imports System.IO
Imports System.Drawing
Imports JackPhysics

Public Module Resources
    Private ResourceCatalog As Dictionary(Of String, ResourcePack)
    Private ResourcePacks As List(Of ResourcePack)

    Public Function getNewBitmap(Name As String) As Bitmap
        Try
            Dim Bitmap As New Bitmap(ResourceCatalog(Name).Bitmaps(Name))
            Return Bitmap
        Catch
            Return My.Resources.NoImage
        End Try
    End Function

    Public Function getNewGameObject(Name As String, Room As Room, Position As Vector3) As GameObject
        Try
            Dim GameObject As New GameObject(ResourceCatalog(Name).GameObjects(Name))

            GameObject.Room = Room
            GameObject.Position = Position
            Return GameObject
        Catch
            Return New GameObject(My.Resources.NoImage, Room, Position, {}, {GameObjectProperties.FlagsEnum.Visible})
        End Try
    End Function

    'Private EXPOrb As Sprite
    '    Private Player As Sprite


    '    Public Shadow As Bitmap
    '    Public GradientLeft As Bitmap
    '    Public GradientRight As Bitmap
    Public ShadeBrush As SolidBrush = New SolidBrush(Color.FromArgb(175, 0, 0, 0))
    '    Public HealthBackground As Bitmap
    '    Public HealthBar As Bitmap

    Public Sub Init()
        ResourcePacks.Add(New ResourcePack("other"))

        Dim Reader As New BinaryReader(New FileStream("resources\manifest.jatb", FileMode.Open))
        Dim numFiles As Integer = Reader.ReadInt32

        Dim files As New List(Of FileInfo)(numFiles)

        For value As Integer = 0 To numFiles - 1
            Dim file As New FileInfo(Reader.ReadString)
            files.Add(file)
        Next

        Reader.Dispose()
        For Each file As FileInfo In files
            Import(file)
        Next


        'Shadow = New Bitmap(My.Resources.Shadow)
        'GradientRight = New Bitmap(My.Resources.GradientRight)
        'GradientLeft = New Bitmap(My.Resources.GradientLeft)
        'HealthBackground = New Bitmap(My.Resources.HealthBackground)
        'HealthBar = New Bitmap(My.Resources.HealthBar)

    End Sub

    Public Sub Import(File As FileInfo)
        Dim Reader = New BinaryReader(New FileStream(File.FullName, FileMode.Open))
        Select Case File.Extension
            Case ".jbrp"
                Dim ResourcePack As New ResourcePack(File.Name)
                ResourcePacks.Add(ResourcePack)

                Dim imageCount As Integer = Reader.ReadInt32
                For value As Integer = 0 To imageCount - 1
                    Dim name As String = Reader.ReadString
                    Dim byteCount As Integer = Reader.ReadInt32
                    Dim ms As New MemoryStream
                    ms.Write(Reader.ReadBytes(byteCount), 0, byteCount)
                    Dim bitmap As New Bitmap(ms)
                    ResourcePack.Bitmaps.Add(name, bitmap)
                    ResourceCatalog.Add(name, ResourcePack)
                    ms.Dispose()
                Next

                Dim gameObjectCount As Integer = Reader.ReadInt32()
                For value As Integer = 0 To imageCount - 1
                    Dim name As String = Reader.ReadString
                    Dim gameObject As GameObject = GameObject.fromBytes(Reader)
                    ResourcePack.GameObjects.Add(name, gameObject)
                    ResourceCatalog.Add(name, ResourcePack)
                Next

            Case ".jbgo"
                Dim name As String = Reader.ReadString
                Dim gameObject As GameObject = GameObject.fromBytes(Reader)
                ResourceCatalog.Add(name, ResourcePacks("other"))
                ResourcePacks("other").GameObjects.Add(name, gameObject)
            Case ".jbrm"


            Case Else

        End Select
        Reader.Dispose()
    End Sub

    Public Sub ExportAll(Dir As DirectoryInfo)
        For Each ResourcePack As ResourcePack In ResourcePacks
            ResourcePack.Export(Dir)
        Next
    End Sub

End Module

Public Class ResourcePack
    Public Name As String
    Public Bitmaps As Dictionary(Of String, Bitmap)
    Public GameObjects As Dictionary(Of String, GameObject)
    Public Rooms As Dictionary(Of String, Room)

    Public Sub New(Name As String)
        Me.Name = Name
    End Sub

    Public Sub Export(Dir As DirectoryInfo)
        Dim Writer = New BinaryWriter(New FileStream(Dir.FullName & "\" & Name & ".jbrp", FileMode.Create))

        Writer.Write(Bitmaps.Count)
        For Each Name As String In Bitmaps.Keys
            Writer.Write(Name)
            Dim ms As New MemoryStream
            Bitmaps(Name).Save(ms, Imaging.ImageFormat.Png)
            Writer.Write(ms.Length)
            Writer.Write(ms.GetBuffer)
            ms.Dispose()
        Next

        Writer.Write(GameObjects.Count)
        For Each Name As String In GameObjects.Keys
            Writer.Write(Name)

        Next

        Writer.Dispose()
    End Sub
End Class
