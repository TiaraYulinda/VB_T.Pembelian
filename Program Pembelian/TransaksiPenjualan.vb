Imports System.Data.OleDb

Public Class TransaksiPenjualan

    Sub FakturOtomatis()
        CMD = New OleDbCommand("select no_faktur from tblpenjualan order by no_faktur desc", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If Not DR.HasRows Then
            Label3.Text = "F0001"
        Else
            Label3.Text = "F" + Format(Microsoft.VisualBasic.Right(DR.Item("no_faktur"), 4) + 1, "0000")
        End If
    End Sub


    Private Sub TransaksiPenjualan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call Koneksi()
        Call Bersihkan()

        Call FakturOtomatis()
        Label4.Text = Today

        CMD = New OleDbCommand("select * from tblcustomer", Conn)
        DR = CMD.ExecuteReader
        ListBox1.Items.Clear()
        Do While DR.Read
            ListBox1.Items.Add(DR.Item(0) & Space(2) & DR.Item(1))
        Loop

        CMD = New OleDbCommand("select * from tblbarang", Conn)
        DR = CMD.ExecuteReader
        ListBox2.Items.Clear()
        Do While DR.Read
            ListBox2.Items.Add(DR.Item(0) & Space(2) & DR.Item(1))
        Loop
    End Sub

    Sub HitungTransaksi()
        Dim x As Integer = 0
        For baris As Integer = 0 To DGV.RowCount - 1
            x = x + DGV.Rows(baris).Cells(3).Value
            Label6.Text = x
            Label6.Text = FormatNumber(Label6.Text, 0)
        Next

        Dim y As Integer = 0
        For baris As Integer = 0 To DGV.RowCount - 1
            y = y + DGV.Rows(baris).Cells(4).Value
            Label8.Text = y
            Label8.Text = FormatNumber(Label8.Text, 0)
        Next

        

        Label10.Text = Val(Microsoft.VisualBasic.Str(Label8.Text) * 10) / 100
        Label10.Text = FormatNumber(Label10.Text, 0)

        Label15.Text = Val(Microsoft.VisualBasic.Str(Label8.Text)) + Val(Microsoft.VisualBasic.Str(Label10.Text))
        Label15.Text = FormatNumber(Label15.Text, 0)
    End Sub


    Private Sub DGV_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGV.CellEndEdit
        If e.ColumnIndex = 0 Then
            DGV.Rows(e.RowIndex).Cells(0).Value = UCase(DGV.Rows(e.RowIndex).Cells(0).Value)

            For i As Integer = 0 To DGV.RowCount - 1
                For j As Integer = i + 1 To DGV.RowCount - 1
                    If DGV.Rows(i).Cells(0).Value = DGV.Rows(j).Cells(0).Value Then
                        MessageBox.Show("Kode " & DGV.Rows(i).Cells(0).Value & "Sudah Dientri")
                        DGV.Rows(j).Cells(0).Value = ""
                        SendKeys.Send("{UP}")
                        Exit Sub
                    End If
                Next
            Next

            CMD = New OleDbCommand("select * from tblbarang where kode_barang='" & DGV.Rows(e.RowIndex).Cells(0).Value & "'", Conn)
            DR = CMD.ExecuteReader
            DR.Read()
            If DR.HasRows Then
                DGV.Rows(e.RowIndex).Cells(1).Value = DR.Item("Nama_Barang")
                DGV.Rows(e.RowIndex).Cells(2).Value = DR.Item("Harga")
                DGV.Rows(e.RowIndex).Cells(3).Value = 1
                DGV.Rows(e.RowIndex).Cells(4).Value = DGV.Rows(e.RowIndex).Cells(2).Value * DGV.Rows(e.RowIndex).Cells(3).Value
                Call FormatGrid()
            Else
                MsgBox("Kode barang tidak terdaftar")
                SendKeys.Send("{up}")
                DGV.Rows(e.RowIndex).Cells(0).Value = ""
            End If
        End If


        If e.ColumnIndex = 3 Then
            Try
                CMD = New OleDbCommand("select * from tblbarang where kode_barang='" & DGV.Rows(e.RowIndex).Cells(0).Value & "'", Conn)
                DR = CMD.ExecuteReader
                DR.Read()
                If DR.HasRows Then
                    If DGV.Rows(e.RowIndex).Cells(3).Value > DR.Item("stok") Then
                        MsgBox("Stok hanya ada " & DR.Item("stok") & "")
                        DGV.Rows(e.RowIndex).Cells(3).Value = DR.Item("stok")
                        DGV.Rows(e.RowIndex).Cells(4).Value = DGV.Rows(e.RowIndex).Cells(2).Value * DGV.Rows(e.RowIndex).Cells(3).Value
                    Else
                        DGV.Rows(e.RowIndex).Cells(4).Value = DGV.Rows(e.RowIndex).Cells(2).Value * DGV.Rows(e.RowIndex).Cells(3).Value
                        'DGV.CurrentCell = DGV(0, DGV.CurrentCell.RowIndex)
                    End If
                End If
            Catch ex As Exception
                MsgBox("harus data angka")
                SendKeys.Send("{up}")
                DGV.Rows(e.RowIndex).Cells(3).Value = 1
                DGV.Rows(e.RowIndex).Cells(4).Value = DGV.Rows(e.RowIndex).Cells(2).Value * DGV.Rows(e.RowIndex).Cells(3).Value
            End Try
        End If
        Call Hitungtransaksi()
    End Sub


    Private Sub DGV_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles DGV.KeyPress
        On Error Resume Next
        If e.KeyChar = Chr(27) Then
            DGV.Rows.RemoveAt(DGV.CurrentCell.RowIndex)
            Call Hitungtransaksi()
        End If

        If e.KeyChar = Chr(13) Then
            TextBox1.Focus()
        End If
    End Sub

    Sub FormatGrid()
        DGV.Columns(2).DefaultCellStyle.Format = "###,###,###"
        DGV.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DGV.Columns(3).DefaultCellStyle.Format = "###,###,###"
        DGV.Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DGV.Columns(4).DefaultCellStyle.Format = "###,###,###"
        DGV.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
    End Sub

    Private Sub ListBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox2.SelectedIndexChanged
        Dim baris As Integer = DGV.RowCount - 1
        DGV.Rows.Add(Microsoft.VisualBasic.Left(ListBox2.Text, 5))

        For i As Integer = 0 To DGV.RowCount - 1
            For j As Integer = i + 1 To DGV.RowCount - 1
                If DGV.Rows(i).Cells(0).Value = DGV.Rows(j).Cells(0).Value Then
                    MessageBox.Show("Kode " & DGV.Rows(i).Cells(0).Value & "Sudah Dientri")
                    DGV.Rows.RemoveAt(j)
                    Exit Sub
                End If
            Next
        Next

        CMD = New OleDbCommand("select * from tblbarang where kode_barang='" & DGV.Rows(baris).Cells(0).Value & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()

        If DR.HasRows Then
            DGV.Rows(baris).Cells(1).Value = DR.Item("nama_barang")
            DGV.Rows(baris).Cells(2).Value = DR.Item("harga")
            DGV.Rows(baris).Cells(3).Value = 1
            DGV.Rows(baris).Cells(4).Value = DGV.Rows(baris).Cells(2).Value * DGV.Rows(baris).Cells(3).Value
            Call FormatGrid()
            Call Hitungtransaksi()
        Else
            MsgBox("Kode barang tidak terdaftar")
        End If
        TextBox3.Clear()
    End Sub

    Sub Bersihkan()
        DGV.Rows.Clear()
        Label6.Text = 0
        Label8.Text = 0
        Label10.Text = 0
        Label15.Text = 0
        Label16.Text = 0
        TextBox1.Text = 0
        TextBox2.Text = 0
        TextBox3.Clear()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Me.Close()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Call Bersihkan()
    End Sub

    Private Sub TextBox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox1.KeyPress
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
        If e.KeyChar = Chr(13) Then
            TextBox1.Text = FormatNumber(TextBox1.Text, 0)
            Label15.Text = Val(Microsoft.VisualBasic.Str(Label15.Text)) - Val(Microsoft.VisualBasic.Str(TextBox1.Text))
            Label15.Text = FormatNumber(Label15.Text, 0)
            TextBox2.Focus()
        End If
    End Sub

    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
        If e.KeyChar = Chr(13) Then
            TextBox2.Text = FormatNumber(TextBox2.Text, 0)
            If Val(TextBox2.Text) < Val(Label15.Text) Then
                MsgBox("Pembayaran kurang")
                Exit Sub
            ElseIf Val(TextBox2.Text) >= Val(Label15.Text) Then
                Label16.Text = Val(Microsoft.VisualBasic.Str(TextBox2.Text)) - Val(Microsoft.VisualBasic.Str(Label15.Text))
                Label16.Text = FormatNumber(Label16.Text, 0)
                Button1.Focus()
            End If
        End If
    End Sub

    Private Sub TextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox3.TextChanged
        CMD = New OleDbCommand("select * from tblbarang where nama_barang like '%" & TextBox3.Text & "%'", Conn)
        DR = CMD.ExecuteReader
        ListBox2.Items.Clear()
        Do While DR.Read
            ListBox2.Items.Add(DR.Item(0) & Space(2) & DR.Item(1))
        Loop
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        If ListBox1.Text = "" Or Label6.Text = 0 Or TextBox2.Text = 0 Then
            MsgBox("Transaksi belum lengkap")
            Exit Sub
        End If

        Try
            CMD = New OleDbCommand("insert into tblPenjualan values ('" & Label3.Text & "','" & Label4.Text & "','" & Microsoft.VisualBasic.Left(ListBox1.Text, 5) & "','" & Label6.Text & "','" & Label8.Text & "','" & Label10.Text & "','" & TextBox1.Text & "','" & Label15.Text & "','" & TextBox2.Text & "','" & Label16.Text & "','" & MasterMenu.Panel1.Text & "')", Conn)
            CMD.ExecuteNonQuery()

            For baris As Integer = 0 To DGV.RowCount - 2
                CMD = New OleDbCommand("insert into tbldetail values ('" & Label3.Text & "','" & DGV.Rows(baris).Cells(0).Value & "','" & DGV.Rows(baris).Cells(2).Value & "','" & DGV.Rows(baris).Cells(3).Value & "','" & DGV.Rows(baris).Cells(4).Value & "')", Conn)
                CMD.ExecuteNonQuery()

                CMD = New OleDbCommand("select * from tblbarang where kode_barang='" & DGV.Rows(baris).Cells(0).Value & "'", Conn)
                DR = CMD.ExecuteReader
                DR.Read()
                If DR.HasRows Then
                    CMD = New OleDbCommand("update tblbarang set stok='" & DR.Item("stok") - DGV.Rows(baris).Cells(3).Value & "' where kode_barang='" & DGV.Rows(baris).Cells(0).Value & "'", Conn)
                    CMD.ExecuteNonQuery()
                End If
            Next

            If MessageBox.Show("Cetak Faktur...?", "", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                Cetak.Show()
                Cetak.CRV.ReportSource = Nothing
                Cetak.CRV.SelectionFormula = "{tblPenjualan.no_faktur} = '" & Label3.Text & "'"
                cryRpt.Load("faktur.rpt")
                Call seting_laporan()
                Cetak.CRV.ReportSource = cryRpt
                Cetak.CRV.RefreshReport()
            End If
            Call Bersihkan()
            Call FakturOtomatis()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class