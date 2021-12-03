Imports System.Data.OleDb

Public Class MasterUser

    'carilah kode user paling akhir, jika tidak ditemukan buatlah dengan pola "USR01"
    'jika dusah ada maka 2 gidit terakhirnya + 1
    Sub KodeOtomatis()
        CMD = New OleDbCommand("select kode_user from tbluser order by kode_user desc", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If Not DR.HasRows Then
            TextBox1.Text = "USR01"
        Else
            TextBox1.Text = "USR" + Format(Microsoft.VisualBasic.Right(DR.Item("kode_user"), 2) + 1, "00")
        End If
    End Sub

    Sub Kosongkan()
        Call KodeOtomatis()
        TextBox1.Enabled = False
        TextBox2.Clear()
        ComboBox1.Text = ""
        TextBox3.Clear()
        TextBox6.Clear()
        TextBox2.Focus()
        Call TampilGrid()
    End Sub

    Sub DataBaru()
        TextBox2.Clear()
        ComboBox1.Text = ""
        TextBox3.Clear()
        TextBox6.Clear()
        TextBox2.Focus()
    End Sub

    Sub Ketemu()
        TextBox2.Text = DR.Item("nama_User")
        TextBox3.Text = DR.Item("pwd_User")
        ComboBox1.Text = DR.Item("Status_user")
        TextBox2.Focus()
    End Sub

    Sub TampilGrid()
        DA = New OleDbDataAdapter("select * from tblUser", CONN)
        DS = New DataSet
        DA.Fill(DS)
        DGV.DataSource = DS.Tables(0)
        DGV.ReadOnly = True
    End Sub

    Private Sub MasterUser_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call Koneksi()
        Call Kosongkan()
        ComboBox1.Items.Add("ADMIN")
        ComboBox1.Items.Add("USER")
    End Sub

    Private Sub ComboBox1_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.LostFocus
        ComboBox1.Text = UCase(ComboBox1.Text)
    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'status user dibatasi hanya ADMIN dan USER
        If ComboBox1.Text <> "ADMIN" And ComboBox1.Text <> "USER" Then
            MsgBox("Status user tidak valid, silakan pilih dalam combo")
            ComboBox1.Focus()
            Exit Sub
        End If
        'carilah kode user yang diketik di textbox1
        CMD = New OleDbCommand("select * from tblUser where kode_User='" & TextBox1.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        Try
            'jika tidak ditemukan maka simpan datanya
            If Not DR.HasRows Then
                CMD = New OleDbCommand("insert into tblUser values ('" & TextBox1.Text & "','" & TextBox2.Text & "','" & TextBox3.Text & "','" & ComboBox1.Text & "')", Conn)
                CMD.ExecuteNonQuery()
                Call Kosongkan()
            Else
                'jika ditemukan maka edit data tersebut
                CMD = New OleDbCommand("update tblUser set nama_User='" & TextBox2.Text & "',Status_user='" & ComboBox1.Text & "',pwd_User='" & TextBox3.Text & "' where kode_User='" & TextBox1.Text & "'", Conn)
                CMD.ExecuteNonQuery()
                Call Kosongkan()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    'ketika data dalam grid diklik, maka tampilkan data2 tersebut ke masing-masing textbox
    Private Sub DGV_CellMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DGV.CellMouseClick
        On Error Resume Next
        TextBox1.Text = DGV.Rows(e.RowIndex).Cells(0).Value
        TextBox2.Text = DGV.Rows(e.RowIndex).Cells(1).Value
        TextBox3.Text = DGV.Rows(e.RowIndex).Cells(2).Value
        ComboBox1.Text = DGV.Rows(e.RowIndex).Cells(3).Value
        TextBox2.Focus()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        'proses hapus harus mengisi kode user terlebih dahulu
        CMD = New OleDbCommand("select kode_user from tbluser where kode_user='" & TextBox1.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If Not DR.HasRows Then
            MsgBox("Kode user belum terdaftar, pilih dulu data dalam Grid")
            Call Kosongkan()
            Exit Sub
        End If

        'sebelum melakukan penghapusan, cari dulu datanya ke tabel penjualan..
        'jika data sudah ada maka proses penghapusan tidak diperbolehkan karena akan menghilangkan data relasinya ke tabel master
        CMD = New OleDbCommand("select distinct kode_user from tblpenjualan where kode_user='" & TextBox1.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If DR.HasRows Then
            MsgBox("Kode user tidak dapat dihapus, karena sudah ada dalam transaksi penjualanbelum terdaftar")
            Call Kosongkan()
            Exit Sub
        End If

        'jika tidak ada di tabel penjualan, maka tampilkan pertanyaan akan dihapus atau tidak
        'jika dijawab YES maka hapus data tersebut
        If MessageBox.Show("yakin akan dihapus..?", "", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            CMD = New OleDbCommand("delete * from tblUser where kode_User='" & TextBox1.Text & "'", Conn)
            CMD.ExecuteNonQuery()
            Call Kosongkan()
        Else
            Call Kosongkan()
        End If
    End Sub

    'ketika mengetik nama user, maka carilah datanya yang mengandung huruf yang diketik
    Private Sub TextBox6_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox6.TextChanged
        DA = New OleDbDataAdapter("select * from tblUser where nama_User like '%" & TextBox6.Text & "%'", Conn)
        DS = New DataSet
        DA.Fill(DS)
        DGV.DataSource = DS.Tables(0)
    End Sub


    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress
        If e.KeyChar = Chr(13) Then
            TextBox2.Text = UCase(TextBox2.Text)
            TextBox3.Focus()
        End If
    End Sub

    Private Sub TextBox3_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox3.KeyPress
        If e.KeyChar = Chr(13) Then
            ComboBox1.Focus()
        End If
    End Sub

    Private Sub combobox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles ComboBox1.KeyPress
        If e.KeyChar = Chr(13) Then
            ComboBox1.Text = UCase(ComboBox1.Text)
            Button1.Focus()
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Call Kosongkan()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Me.Close()
    End Sub
End Class

