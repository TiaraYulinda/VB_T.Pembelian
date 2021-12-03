Imports System.Data.OleDb

Public Class MasterPerusahaan

    Sub Ketemu()
        On Error Resume Next
        TextBox2.Text = DR.Item("Nama")
        TextBox3.Text = DR.Item("alamat")
        TextBox4.Text = Microsoft.VisualBasic.Mid(DR.Item("telepon"), 9, 20)
        TextBox5.Text = Microsoft.VisualBasic.Mid(DR.Item("fax"), 5, 20)
        TextBox7.Text = Microsoft.VisualBasic.Mid(DR.Item("email"), 7, 43)
        TextBox8.Text = Microsoft.VisualBasic.Mid(DR.Item("website"), 9, 41)
        TextBox2.Focus()
    End Sub


    Sub Tampilperusahaan()
        CMD = New OleDbCommand("select * from tblperusahaan", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If DR.HasRows Then
            TextBox2.Text = DR.Item("nama")
            TextBox3.Text = DR.Item("alamat")
            TextBox4.Text = Microsoft.VisualBasic.Mid(DR.Item("telepon"), 9, 21)
            TextBox5.Text = Microsoft.VisualBasic.Mid(DR.Item("fax"), 5, 25)
            TextBox7.Text = Microsoft.VisualBasic.Mid(DR.Item("email"), 7, 43)
            TextBox8.Text = Microsoft.VisualBasic.Mid(DR.Item("website"), 9, 41)
        End If
    End Sub

    Private Sub MasterPerusahaan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call Koneksi()
        Call Tampilperusahaan()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Me.Close()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        
        Try
            CMD = New OleDbCommand("delete * from tblperusahaan ", Conn)
            CMD.ExecuteNonQuery()

            CMD = New OleDbCommand("insert into tblperusahaan values ('" & TextBox2.Text & "','" & TextBox3.Text & "','" & Label4.Text & Space(1) & TextBox4.Text & "','" & Label5.Text & Space(1) & TextBox5.Text & "','" & Label6.Text & Space(1) & TextBox7.Text & "','" & Label7.Text & Space(1) & TextBox8.Text & "')", Conn)
            CMD.ExecuteNonQuery()
            MsgBox("Data berhasil disimpan")

            CMD = New OleDbCommand("select nama from tblperusahaan", Conn)
            DR = CMD.ExecuteReader
            DR.Read()
            MasterMenu.Label1.Text = DR.Item(0)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub TextBox2_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox2.LostFocus
        CMD = New OleDbCommand("select * from tblperusahaan where nama='" & TextBox2.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If Not DR.HasRows Then
            TextBox3.Clear()
            TextBox4.Clear()
            TextBox5.Clear()
            TextBox7.Clear()
            TextBox8.Clear()
            TextBox3.Focus()
        Else
            Call Ketemu()
        End If
    End Sub
End Class
