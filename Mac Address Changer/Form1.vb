﻿Imports System.Management

Public Class Form1
    ' Coded by Weeke

    ' I used the source code from http://forum.codecall.net/topic/43009-see-and-change-your-mac-address-in-vbnet/
    ' I really only had to put it together and fix a couple lines of code and then compile and add labels telling you how to use this tool as well as link a
    ' Mac Address Generator so it would be user friendly for everyone.

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        System.Diagnostics.Process.Start("https://forum.invictus-load.in/profile/3056-weeke/")
    End Sub

    Private Sub FillNetworkAdapters()

        Dim mc As System.Management.ManagementClass
        Dim mo As ManagementObject

        mc = New ManagementClass("Win32_NetworkAdapterConfiguration")

        Dim moc As ManagementObjectCollection = mc.GetInstances()

        For Each mo In moc
            If mo.Item("IPEnabled") = True Then
                Dim strAdapter As String
                strAdapter = mo.Item("Caption").ToString().Substring(11)

                combo_network.Items.Add(strAdapter)
            End If
        Next

    End Sub

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        FillNetworkAdapters()

    End Sub

    Private Function GetMACAddress(ByVal Adapter As String) As String

        Dim mc As System.Management.ManagementClass
        Dim mo As ManagementObject

        mc = New ManagementClass("Win32_NetworkAdapterConfiguration")

        Dim moc As ManagementObjectCollection = mc.GetInstances()

        For Each mo In moc
            If mo.Item("IPEnabled") = True Then
                Dim strAdapter As String
                strAdapter = mo.Item("Caption").ToString().Substring(11)
                If strAdapter = Adapter Then
                    Return mo.Item("MacAddress").ToString()
                End If
            End If
        Next

    End Function

    Private Sub combo_network_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles combo_network.SelectedIndexChanged

        label_mactext.Text = GetMACAddress(combo_network.SelectedItem.ToString)

    End Sub

    Private Function DoPadding(ByVal x As String) As String
        Dim Ret As String
        Dim z As Integer

        Ret = x

        If Len(x) < 4 Then
            For z = 1 To 4 - Len(x)
                Ret = "0" & Ret
            Next
        End If

        Return Ret

    End Function

    Private Sub ShowRestart()

        Dim res As MsgBoxResult = MsgBox("Your MAC Address has been changed. In order to make the changes take effect, either restart your computer or enable and disable the changed Network Adapter.", MsgBoxStyle.Information)

    End Sub

    Private Sub bt_update_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bt_update.Click
        If IsOkay() = False Then
            Exit Sub
        End If

        Dim regKey As Microsoft.Win32.RegistryKey
        Dim Addr As String = GetRoot(combo_network.SelectedItem.ToString())

        regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(Addr, True)
        regKey.SetValue("NetworkAddress", mac_text.Text.Replace(":", ""))

        ShowRestart()
    End Sub

    Private Function GetRoot(ByVal Adapter As String) As String
        Dim regKey As Microsoft.Win32.RegistryKey
        Dim i As Integer = 0

        Do
            Dim Root As String = "SYSTEM\CurrentControlSet\Control\Class\{4D36E972-E325-11CE-BFC1-08002BE10318}\"
            Dim Last As String = DoPadding(i)
            regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(Root & Last, True)

            Try
                Dim cAdapter As String = regKey.GetValue("DriverDesc").ToString()
                If cAdapter = Adapter Then
                    Return Root & Last
                End If
            Catch
                Exit Do
            End Try

            i += 1
        Loop

    End Function

    Private Function IsOkay() As Boolean

        If mac_text.Text = "" Then
            MsgBox("You didn't enter a MAC Address", MsgBoxStyle.Critical)
            Return False
        End If

        Dim ed As String = mac_text.Text.Replace(":", "")

        If ed.Length <> 12 Then
            MsgBox("A MAC Address must have a length of 12", MsgBoxStyle.Critical)
            Return False
        End If

        Try
            If combo_network.SelectedItem.ToString = "" Then
                MsgBox("No Network Adapter selected", MsgBoxStyle.Critical)
                Return False
            End If
        Catch
            MsgBox("No Network Adapter selected", MsgBoxStyle.Critical)
            Return False
        End Try


        Dim noerror As Boolean = True
        Dim i As Integer

        For i = 0 To ed.Length - 1
            If IsHex(ed.Substring(i, 1)) = False Then
                MsgBox("MAC Address in wrong format", MsgBoxStyle.Critical)
                Return False
            End If
        Next

        Return True

    End Function

    Private Function IsHex(ByVal l As String) As Boolean
        Dim table As String = "0123456789ABCDEF"
        Dim i As Integer

        For i = 0 To table.Length - 1
            If l = table.Substring(i, 1) Then
                Return True
            End If
        Next
        Return False

    End Function

    Private Sub bt_defaultmac_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bt_defaultmac.Click
        If IsOkay() = False Then
            Exit Sub
        End If

        Dim regKey As Microsoft.Win32.RegistryKey
        Dim Addr As String = GetRoot(combo_network.SelectedItem.ToString())

        regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(Addr, True)

        Try
            regKey.DeleteValue("NetworkAddress")
        Catch
            'Do NOTHING
        End Try

        ShowRestart()

    End Sub

End Class
