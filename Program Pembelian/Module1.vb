Imports System.Data.OleDb
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared


Module Module1

    'mendefinisikan variabel untuk koneksi dan query
    Public Conn As OleDbConnection
    Public DA As OleDbDataAdapter
    Public DS As DataSet
    Public CMD As OleDbCommand
    Public DR As OleDbDataReader

    'mendefinisikan variabel agar laporan dapat dipanggil dengan aman tanpa peduli posisi folder
    Public cryRpt As New ReportDocument
    Public crtableLogoninfos As New TableLogOnInfos
    Public crtableLogoninfo As New TableLogOnInfo
    Public crConnectionInfo As New ConnectionInfo
    Public CrTables As Tables

    Public Sub seting_laporan()
        With crConnectionInfo
            .ServerName = (Application.StartupPath.ToString & "\database1.mdb")
            .DatabaseName = (Application.StartupPath.ToString & "\database1.mdb")
            .UserID = ""
            .Password = ""
        End With

        CrTables = cryRpt.Database.Tables
        For Each CrTable In CrTables
            crtableLogoninfo = CrTable.LogOnInfo
            crtableLogoninfo.ConnectionInfo = crConnectionInfo
            CrTable.ApplyLogOnInfo(crtableLogoninfo)
        Next
    End Sub

    Public Sub Koneksi()
        Try
            'string koneksi untuk database access 2003
            Conn = New OleDbConnection("provider=microsoft.ace.oledb.12.0;data source=database1.mdb")
            Conn.Open()
        Catch ex As Exception
            MsgBox(ex.Message)
            End
        End Try
    End Sub
End Module

