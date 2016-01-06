Imports System.IO
Imports System.Drawing
Imports JackPhysics

Public Module Resources
    Public ResourceCatalog As New Dictionary(Of String, ResourcePack)
    Public ResourcePacks As New List(Of ResourcePack)

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
            Dim Properties As New Dictionary(Of String, String)
            Properties.Add("Name", Name)
            Return New GameObject(My.Resources.NoImage, Room, Position, Properties, {GameObjectProperties.FlagsEnum.Visible})
        End Try
    End Function

    Public ShadeBrush As SolidBrush = New SolidBrush(Color.FromArgb(175, 0, 0, 0))

    Public Sub Init()
        ResourcePacks.Add(New ResourcePack("other"))
        Dim ResourceFolder As New DirectoryInfo("resources")
        If Not ResourceFolder.Exists Then
            ResourceFolder.Create()
        End If

        Dim Manifest As New FileInfo("resources\manifest.jbmn")
        If Not Manifest.Exists Then
            Dim Writer As New BinaryWriter(New FileStream(Manifest.FullName, FileMode.Create))
            Writer.Write(0)
            Writer.Dispose()
        End If

        Dim Reader As New BinaryReader(New FileStream(Manifest.FullName, FileMode.Open))
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
    End Sub

    Public Sub Import(File As FileInfo)
        Dim Reader = New BinaryReader(New FileStream(File.FullName, FileMode.Open))
        Select Case File.Extension
            Case ".jbrp"
                Dim ResourcePack As New ResourcePack(File.Name, File)
                ResourcePacks.Add(ResourcePack)
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
            If Not ResourcePack.Name.Equals("other") Then ResourcePack.ExportTo(Dir)
        Next
    End Sub

End Module

Public Class ResourcePack
    Public Name As String
    Public Bitmaps As New Dictionary(Of String, Bitmap)
    Public GameObjects As New Dictionary(Of String, GameObject)
    Public Rooms As New Dictionary(Of String, Room)

    Public Sub New(ResourcesName As String, File As FileInfo)
        Name = ResourcesName

        Dim Reader = New BinaryReader(New FileStream(File.FullName, FileMode.Open))

        Dim imageCount As Integer = Reader.ReadInt32
        For value As Integer = 0 To imageCount - 1
            Dim name As String = Reader.ReadString
            Dim byteCount As Integer = Reader.ReadInt32
            Dim ms As New MemoryStream
            ms.Write(Reader.ReadBytes(byteCount), 0, byteCount)
            Dim bitmap As New Bitmap(ms)
            Bitmaps.Add(name, bitmap)
            Resources.ResourceCatalog.Add(name, Me)
            ms.Dispose()
        Next

        Dim gameObjectCount As Integer = Reader.ReadInt32()
        For value As Integer = 0 To imageCount - 1
            Dim name As String = Reader.ReadString
            Dim gameObject As GameObject = GameObject.FromBytes(Reader)
            GameObjects.Add(name, gameObject)
            Resources.ResourceCatalog.Add(name, Me)
        Next

        Reader.Dispose()
    End Sub

    Public Sub New(Name As String)
        Me.Name = Name
    End Sub

    Public Sub ExportTo(Dir As DirectoryInfo)
        Dim Writer = New BinaryWriter(New FileStream(Dir.FullName & "\" & Name & ".jbrp", FileMode.Create))
        ExportTo(Writer)
        Writer.Dispose()
    End Sub

    Public Sub ExportTo(Writer As BinaryWriter)
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
            GameObjects(Name).ExportTo(Writer)
        Next
    End Sub
End Class
