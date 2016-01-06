Imports JackToolbox

Public Class MainForm
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Resources.Init()
        Dim testResources As New ResourcePack("testResources")
        testResources.GameObjects("test") = Resources.getNewGameObject("test", Nothing, Nothing)

        'testResources.GameObjects("test").ExportTo(New IO.DirectoryInfo("resources"))
        testResources.ExportTo(New IO.DirectoryInfo("resources"))

    End Sub
End Class
