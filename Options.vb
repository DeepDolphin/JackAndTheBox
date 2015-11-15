Imports System.Xml

Public Class Options
    Public Preferences As Dictionary(Of String, String)
    Public OIMap As Dictionary(Of String, String)
    Public OIStatus As Dictionary(Of String, Boolean)

    Public Sub New()
        Preferences = New Dictionary(Of String, String)
        OIMap = New Dictionary(Of String, String)
        OIStatus = New Dictionary(Of String, Boolean)



        'Options File Import/Creation
        If (IO.File.Exists("options.xml") And False) Then 'For now always init (for debugging purposes)
            Dim optionsDoc As New XmlDocument
            optionsDoc.Load("options.xml")
            Dim options As XmlElement = optionsDoc("Options")
            For Each section As XmlElement In options
                Select Case section.Name
                    Case "Preferences"
                        For Each element As XmlElement In section
                            Preferences.Add(element.Name, element.GetAttribute("Value"))
                        Next
                    Case "OIMap"
                        For Each element As XmlElement In section
                            OIMap.Add(element.GetAttribute("KeyCode"), element.Name)
                            OIStatus.Add(element.Name, False)
                        Next
                End Select
            Next
        Else
            InitOptions()
            SaveOptions()
        End If
    End Sub

    Public Sub InitOptions()
        'Preferences:
        'Player:
        Preferences.Add("PlayerMovementType", "ArcadeMovement")
        Preferences.Add("MaxFPS", "60")

        'OIMap:
        'Movement:
        OIMap.Add(Keys.W, "Up")
        OIMap.Add(Keys.S, "Down")
        OIMap.Add(Keys.D, "Right")
        OIMap.Add(Keys.A, "Left")
        OIMap.Add(Keys.ControlKey, "Sprint")
        OIMap.Add(MouseButtons.Right, "UtilityAbility")
        OIMap.Add(Keys.Enter, "Pause")
        OIMap.Add(MouseButtons.Left, "ActiveAbility")

        'OIStatus:
        'Movement:
        OIStatus.Add("Up", False)
        OIStatus.Add("Down", False)
        OIStatus.Add("Right", False)
        OIStatus.Add("Left", False)
        OIStatus.Add("Sprint", False)
        OIStatus.Add("UtilityAbility", False)
        OIStatus.Add("Pause", False)
        OIStatus.Add("ActiveAbility", False)
    End Sub

    Public Sub SaveOptions()
        Dim optionsDoc As New XmlDocument
        Dim Options As XmlElement = optionsDoc.CreateElement("Options")
        optionsDoc.AppendChild(Options)

        'Create Preferences
        Dim PreferencesE As XmlElement = optionsDoc.CreateElement("Preferences")
        Options.AppendChild(PreferencesE)

        For Each Preference As String In Preferences.Keys
            Dim Pref As XmlElement = optionsDoc.CreateElement(Preference)
            PreferencesE.AppendChild(Pref)
            Pref.SetAttribute("Value", Preferences(Preference))
        Next

        'Create OIMap
        Dim OIMapE As XmlElement = optionsDoc.CreateElement("OIMap")
        Options.AppendChild(OIMapE)

        For Each KeyCode As String In OIMap.Keys
            Dim Key As XmlElement = optionsDoc.CreateElement(OIMap(KeyCode))
            OIMapE.AppendChild(Key)
            Key.SetAttribute("KeyCode", KeyCode)
        Next

        optionsDoc.Save("options.xml")
    End Sub
End Class
