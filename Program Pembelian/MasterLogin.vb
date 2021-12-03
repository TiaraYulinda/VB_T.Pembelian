
Imports System.Data.OleDb
Imports System.Text

Public Class masterLogin

    Private Sub TextBox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox1.KeyPress
        If e.KeyChar = Chr(13) Then TextBox2.Focus()
    End Sub

    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress
        If e.KeyChar = Chr(13) Then Button1.Focus()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Call Koneksi()
        CMD = New OleDbCommand("select * from tbluser where nama_User= '" & TextBox1.Text & "' and pwd_user='" & TextBox2.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If DR.HasRows Then
            'membuat karakter menjadi case sensitif
            If String.Compare(DR.Item("nama_User"), TextBox1.Text, False) Or String.Compare(DR.Item("pwd_user"), TextBox2.Text, False) Then
                MsgBox("Login Gagal")
                TextBox1.Clear()
                TextBox2.Clear()
                TextBox1.Focus()
                Exit Sub
            Else
                Me.Visible = False
                MasterMenu.Show()
                MasterMenu.Panel1.Text = DR.Item("Kode_user")
                MasterMenu.Panel2.Text = DR.Item("nama_User")
                MasterMenu.Panel3.Text = DR.Item("status_user")
            End If
            CMD = New OleDbCommand("select nama from tblperusahaan", Conn)
            DR = CMD.ExecuteReader
            DR.Read()
            MasterMenu.Label1.Text = DR.Item(0)
        Else
            MsgBox("Nama user dan password tidak cocok")
            TextBox1.Clear()
            TextBox2.Clear()
            TextBox1.Focus()
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        End
    End Sub
End Class
