Imports System.Data.OleDb

Public Class MasterCustomer

    Sub KodeOtomatis()
        CMD = New OleDbCommand("select kode_Customer from tblCustomer order by kode_Customer desc", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If Not DR.HasRows Then
            TextBox1.Text = "CUS01"
        Else
            TextBox1.Text = "CUS" + Format(Microsoft.VisualBasic.Right(DR.Item("kode_Customer"), 2) + 1, "00")
        End If
    End Sub

    Sub Kosongkan()
        Call KodeOtomatis()
        TextBox1.Enabled = False
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox5.Clear()
        TextBox6.Clear()
        TextBox2.Focus()
        Call TampilGrid()
    End Sub

    Sub DataBaru()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox5.Clear()
        TextBox6.Clear()
        TextBox2.Focus()
    End Sub

    Sub Ketemu()
        TextBox2.Text = DR.Item("nama_Customer")
        TextBox3.Text = DR.Item("ALAMAT_CUSTOMER")
        TextBox4.Text = DR.Item("TELEPON_CUSTOMER")
        TextBox5.Text = DR.Item("FAX_CUSTOMER")
        TextBox2.Focus()
    End Sub

    Sub TampilGrid()
        DA = New OleDbDataAdapter("select * from tblCustomer", Conn)
        DS = New DataSet
        DA.Fill(DS)
        DGV.DataSource = DS.Tables(0)
        DGV.ReadOnly = True
    End Sub

    Private Sub MasterCustomer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call Koneksi()
        Call Kosongkan()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        CMD = New OleDbCommand("select * from tblCustomer where kode_Customer='" & TextBox1.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        Try
            If Not DR.HasRows Then
                CMD = New OleDbCommand("insert into tblCustomer values ('" & TextBox1.Text & "','" & TextBox2.Text & "','" & TextBox3.Text & "','" & TextBox4.Text & "','" & TextBox5.Text & "')", Conn)
                CMD.ExecuteNonQuery()
                Call Kosongkan()
            Else
                CMD = New OleDbCommand("update tblCustomer set nama_Customer='" & TextBox2.Text & "',ALAMAT_CUSTOMER='" & TextBox3.Text & "',TELEPON_CUSTOMER='" & TextBox4.Text & "',FAX_CUSTOMER='" & TextBox5.Text & "' where kode_Customer='" & TextBox1.Text & "'", Conn)
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
        TextBox3.Text = DGV.Rows(e.RowIndex).Cells(2).Value
        TextBox4.Text = DGV.Rows(e.RowIndex).Cells(3).Value
        TextBox5.Text = DGV.Rows(e.RowIndex).Cells(4).Value
        TextBox2.Focus()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        CMD = New OleDbCommand("select kode_Customer from tblCustomer where kode_Customer='" & TextBox1.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If Not DR.HasRows Then
            MsgBox("Kode Customer belum terdaftar, pilih dulu data dalam Grid")
            Call Kosongkan()
            Exit Sub
        End If

        CMD = New OleDbCommand("select distinct kode_Customer from tblPENJUALAN where kode_Customer='" & TextBox1.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If DR.HasRows Then
            MsgBox("Kode Customer tidak dapat dihapus karena sudah ada dalam transaksi")
            Call Kosongkan()
            Exit Sub
        End If

        If MessageBox.Show("yakin akan dihapus..?", "", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            CMD = New OleDbCommand("delete * from tblCustomer where kode_Customer='" & TextBox1.Text & "'", Conn)
            CMD.ExecuteNonQuery()
            Call Kosongkan()
        Else
            Call Kosongkan()
        End If
    End Sub

    Private Sub TextBox6_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox6.TextChanged
        DA = New OleDbDataAdapter("select * from tblCustomer where nama_Customer like '%" & TextBox6.Text & "%' or ALAMAT_CUSTOMER like '%" & TextBox6.Text & "%'", Conn)
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
            TextBox4.Focus()
        End If
        'If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub

    Private Sub TextBox4_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox4.KeyPress
        If e.KeyChar = Chr(13) Then
            TextBox5.Focus()
        End If
        'If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub

    Private Sub TextBox5_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox5.KeyPress
        If e.KeyChar = Chr(13) Then
            Button1.Focus()
        End If
        'If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Call Kosongkan()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Me.Close()
    End Sub
End Class



