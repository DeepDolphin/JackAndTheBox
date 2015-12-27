Imports JackToolbox

Public Class MainForm
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Resources.Init()
        Dim testResources As New ResourcePack("testResources")
        testResources.Export(New IO.DirectoryInfo("resources"))

    End Sub
End Class
