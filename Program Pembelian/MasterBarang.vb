Imports System.Data.OleDb

Public Class MasterBarang

    Sub KodeOtomatis()  'membuat nomor otomatis dengan pola "B0001"
        CMD = New OleDbCommand("select kode_Barang from tblBarang order by kode_Barang desc", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If Not DR.HasRows Then
            TextBox1.Text = "B0001"
        Else
            TextBox1.Text = "B" + Format(Microsoft.VisualBasic.Right(DR.Item("kode_Barang"), 4) + 1, "0000")
        End If
    End Sub

    Sub Kosongkan()
        Call KodeOtomatis()
        TextBox1.Enabled = False
        TextBox2.Clear()
        ComboBox1.Text = ""
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox6.Clear()
        TextBox2.Focus()
        Call TampilGrid()
    End Sub

    Sub DataBaru()
        TextBox2.Clear()
        ComboBox1.Text = ""
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox6.Clear()
        TextBox2.Focus()
    End Sub

    Sub Ketemu()
        TextBox2.Text = DR.Item("nama_Barang")
        ComboBox1.Text = DR.Item("satuan")
        TextBox3.Text = DR.Item("harga")
        TextBox4.Text = DR.Item("stok")
        TextBox2.Focus()
    End Sub

    Sub TampilGrid()
        DA = New OleDbDataAdapter("select * from tblBarang", Conn)
        DS = New DataSet
        DA.Fill(DS)
        DGV.DataSource = DS.Tables(0)
        DGV.ReadOnly = True
    End Sub

    Sub Tampilsatuan()
        CMD = New OleDbCommand("select distinct satuan from tblbarang", Conn)
        DR = CMD.ExecuteReader
        ComboBox1.Items.Clear()
        Do While DR.Read
            ComboBox1.Items.Add(DR.Item("Satuan"))
        Loop
    End Sub

    Private Sub MasterBarang_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call Koneksi()
        Call Kosongkan()
        Call Tampilsatuan()
    End Sub

    Private Sub ComboBox1_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.LostFocus
        ComboBox1.Text = UCase(ComboBox1.Text)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        CMD = New OleDbCommand("select * from tblBarang where kode_Barang='" & TextBox1.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        Try
            If Not DR.HasRows Then
                CMD = New OleDbCommand("insert into tblBarang values ('" & TextBox1.Text & "','" & TextBox2.Text & "','" & ComboBox1.Text & "','" & TextBox3.Text & "','" & TextBox4.Text & "')", Conn)
                CMD.ExecuteNonQuery()
                Call Kosongkan()
            Else
                CMD = New OleDbCommand("update tblBarang set nama_Barang='" & TextBox2.Text & "',satuan='" & ComboBox1.Text & "',harga='" & TextBox3.Text & "',stok='" & TextBox4.Text & "' where kode_Barang='" & TextBox1.Text & "'", Conn)
                CMD.ExecuteNonQuery()
                Call Kosongkan()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub


    Private Sub DGV_CellMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DGV.CellMouseClick
        On Error Resume Next
        TextBox1.Text = DGV.Rows(e.RowIndex).Cells(0).Value
        TextBox2.Text = DGV.Rows(e.RowIndex).Cells(1).Value
        ComboBox1.Text = DGV.Rows(e.RowIndex).Cells(2).Value
        TextBox3.Text = DGV.Rows(e.RowIndex).Cells(3).Value
        TextBox4.Text = DGV.Rows(e.RowIndex).Cells(4).Value
        TextBox2.Focus()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        CMD = New OleDbCommand("select kode_Barang from tblBarang where kode_Barang='" & TextBox1.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If Not DR.HasRows Then
            MsgBox("Kode Barang belum terdaftar, pilih dulu data dalam Grid")
            Call Kosongkan()
            Exit Sub
        End If

        CMD = New OleDbCommand("select distinct kode_Barang from tbldetail where kode_Barang='" & TextBox1.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If DR.HasRows Then
            MsgBox("Kode Barang tidak dapat dihapus karena sudah ada dalam transaksi")
            Call Kosongkan()
            Exit Sub
        End If

        If MessageBox.Show("yakin akan dihapus..?", "", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            CMD = New OleDbCommand("delete * from tblBarang where kode_Barang='" & TextBox1.Text & "'", Conn)
            CMD.ExecuteNonQuery()
            Call Kosongkan()
        Else
            Call Kosongkan()
        End If
    End Sub

    Private Sub TextBox6_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox6.TextChanged
        DA = New OleDbDataAdapter("select * from tblBarang where nama_Barang like '%" & TextBox6.Text & "%' or satuan like '%" & TextBox6.Text & "%'", Conn)
        DS = New DataSet
        DA.Fill(DS)
        DGV.DataSource = DS.Tables(0)
    End Sub


    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress
        If e.KeyChar = Chr(13) Then
            TextBox2.Text = UCase(TextBox2.Text)
            ComboBox1.Focus()
        End If
    End Sub

    Private Sub combobox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles ComboBox1.KeyPress
        If e.KeyChar = Chr(13) Then
            ComboBox1.Text = UCase(ComboBox1.Text)
            TextBox3.Focus()
        End If
    End Sub

    Private Sub TextBox3_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox3.KeyPress
        If e.KeyChar = Chr(13) Then
            TextBox4.Focus()
        End If
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub

    Private Sub TextBox4_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox4.KeyPress
        If e.KeyChar = Chr(13) Then
            Button1.Focus()
        End If
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Call Kosongkan()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Me.Close()
    End Sub

End Class

