Imports System.Data.OleDb

Public Class TransaksiPembelian

    'mencari faktur terakhir, jika tidak ada maka buat faktur dengan pola "F0001"
    'jika sudah ada, maka 4 digit terakhir faktur + 1
    Sub FakturOtomatis()
        CMD = New OleDbCommand("select no_faktur from tblPembelian order by no_faktur desc", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If Not DR.HasRows Then
            Label3.Text = "F0001"
        Else
            Label3.Text = "F" + Format(Microsoft.VisualBasic.Right(DR.Item("no_faktur"), 4) + 1, "0000")
        End If
    End Sub


    Private Sub TransaksiPembelian_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call Koneksi()
        Call Bersihkan()
        Call FakturOtomatis()
        Label4.Text = Today
        'menampilkan kode dan nama supplier dalam listbox1
        CMD = New OleDbCommand("select * from tblSupplier", Conn)
        DR = CMD.ExecuteReader
        ListBox1.Items.Clear()
        Do While DR.Read
            ListBox1.Items.Add(DR.Item(0) & Space(2) & DR.Item(1))
        Loop

        'menampilkan kode dan nama barang dalam listbox2
        CMD = New OleDbCommand("select * from tblbarang", Conn)
        DR = CMD.ExecuteReader
        ListBox2.Items.Clear()
        Do While DR.Read
            ListBox2.Items.Add(DR.Item(0) & Space(2) & DR.Item(1))
        Loop
    End Sub

    'membuat fungsi untuk menghitung jumlah barang dan total harga
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

        'pajak secara otomatis 10% dari total harga
        Label10.Text = Val(Microsoft.VisualBasic.Str(Label8.Text) * 10) / 100
        Label10.Text = FormatNumber(Label10.Text, 0)

        'grand total secara otomatis dihasilkan dari total harga + pajak
        Label15.Text = Val(Microsoft.VisualBasic.Str(Label8.Text)) + Val(Microsoft.VisualBasic.Str(Label10.Text))
        Label15.Text = FormatNumber(Label15.Text, 0)
    End Sub


    Private Sub DGV_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DGV.CellEndEdit
        If e.ColumnIndex = 0 Then
            'mengubah kode barang menjadi huruf besar
            DGV.Rows(e.RowIndex).Cells(0).Value = UCase(DGV.Rows(e.RowIndex).Cells(0).Value)

            'mencegah duplikasi entri data dalam grid
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

            'mencari kode barang dalam grid
            CMD = New OleDbCommand("select * from tblbarang where kode_barang='" & DGV.Rows(e.RowIndex).Cells(0).Value & "'", Conn)
            DR = CMD.ExecuteReader
            DR.Read()
            If DR.HasRows Then
                'jika datanya ditemukan, tampilkan nama dan harga di kolom berikutnya
                DGV.Rows(e.RowIndex).Cells(1).Value = DR.Item("Nama_Barang")
                DGV.Rows(e.RowIndex).Cells(2).Value = DR.Item("Harga")
                'asumsi pembelian adalah 1 buah
                DGV.Rows(e.RowIndex).Cells(3).Value = 1
                'total dihasilkan dari harga x jumlah
                DGV.Rows(e.RowIndex).Cells(4).Value = DGV.Rows(e.RowIndex).Cells(2).Value * DGV.Rows(e.RowIndex).Cells(3).Value
                Call FormatGrid()
            Else
                'jika kode barang tidak ditemukan munculkan pesan
                MsgBox("Kode barang tidak terdaftar")
                SendKeys.Send("{up}")
                DGV.Rows(e.RowIndex).Cells(0).Value = ""
            End If
        End If

        If e.ColumnIndex = 3 Then
            Try
                DGV.Rows(e.RowIndex).Cells(4).Value = DGV.Rows(e.RowIndex).Cells(2).Value * DGV.Rows(e.RowIndex).Cells(3).Value
            Catch ex As Exception
                'jika terjadi kesalahan entri data abjad
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
        'jika menekan escape maka hapus data di baris tersebut
        If e.KeyChar = Chr(27) Then
            DGV.Rows.RemoveAt(DGV.CurrentCell.RowIndex)
            Call Hitungtransaksi()
        End If

        'jika menekan enter maka pindahkan kursor ke pembayaran
        If e.KeyChar = Chr(13) Then
            TextBox1.Focus()
        End If
    End Sub

    Sub FormatGrid()
        'format grid data angka dengan pemisah ribuan dan posisinya di kanan - tengah
        DGV.Columns(2).DefaultCellStyle.Format = "###,###,###"
        DGV.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DGV.Columns(3).DefaultCellStyle.Format = "###,###,###"
        DGV.Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DGV.Columns(4).DefaultCellStyle.Format = "###,###,###"
        DGV.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
    End Sub

    Private Sub ListBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox2.SelectedIndexChanged
        'jika kode barang di listbox2 di pilih maka kode tersebut masuk kedalam grid
        Dim baris As Integer = DGV.RowCount - 1
        DGV.Rows.Add(Microsoft.VisualBasic.Left(ListBox2.Text, 5))
        'mencegah agar kode barang jangan dientri lebih dari 1 kali
        For i As Integer = 0 To DGV.RowCount - 1
            For j As Integer = i + 1 To DGV.RowCount - 1
                If DGV.Rows(i).Cells(0).Value = DGV.Rows(j).Cells(0).Value Then
                    MessageBox.Show("Kode " & DGV.Rows(i).Cells(0).Value & "Sudah Dientri")
                    DGV.Rows.RemoveAt(j)
                    Exit Sub
                End If
            Next
        Next

        'mencari kode barang yang ada di kolom kode
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
        'hanya boleh diisi data angka 0 - 9
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True

        If e.KeyChar = Chr(13) Then
            Label15.Text = Val(Microsoft.VisualBasic.Str(Label8.Text)) + Val(Microsoft.VisualBasic.Str(Label10.Text))
            Label15.Text = FormatNumber(Label15.Text, 0)
            TextBox1.Text = FormatNumber(TextBox1.Text, 0)
            Label15.Text = Val(Microsoft.VisualBasic.Str(Label15.Text)) - Val(Microsoft.VisualBasic.Str(TextBox1.Text))
            Label15.Text = FormatNumber(Label15.Text, 0)
            TextBox2.Focus()
        End If
    End Sub

    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress
        'hanya dapat diisi angka 0 - 9
        If Not ((e.KeyChar >= "0" And e.KeyChar <= "9") Or e.KeyChar = vbBack) Then e.Handled = True
        'pembayaran tidak boleh kurang dari total harga
        If e.KeyChar = Chr(13) Then
            TextBox2.Text = FormatNumber(TextBox2.Text, 0)
            If Val(TextBox2.Text) < Val(Label15.Text) Then
                MsgBox("Pembayaran kurang")
                Exit Sub
            ElseIf Val(TextBox2.Text) >= Val(Label15.Text) Then
                'terjadi uang kembali jika pembayaran > total harga
                Label16.Text = Val(Microsoft.VisualBasic.Str(TextBox2.Text)) - Val(Microsoft.VisualBasic.Str(Label15.Text))
                Label16.Text = FormatNumber(Label16.Text, 0)
                Button1.Focus()
            End If
        End If
    End Sub

    Private Sub TextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox3.TextChanged
        'mencari nama barang untuk ditampilkan dalam listbox2
        CMD = New OleDbCommand("select * from tblbarang where nama_barang like '%" & TextBox3.Text & "%'", Conn)
        DR = CMD.ExecuteReader
        ListBox2.Items.Clear()
        Do While DR.Read
            ListBox2.Items.Add(DR.Item(0) & Space(2) & DR.Item(1))
        Loop
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'cegah proses simpan jika data transaksi belum lengkap 
        If ListBox1.Text = "" Or Label6.Text = 0 Or TextBox2.Text = 0 Then
            MsgBox("Transaksi belum lengkap")
            Exit Sub
        End If

        Try
            'simpan data ke tabel pembelian
            CMD = New OleDbCommand("insert into tblPembelian values ('" & Label3.Text & "','" & Label4.Text & "','" & Microsoft.VisualBasic.Left(ListBox1.Text, 5) & "','" & Label6.Text & "','" & Label8.Text & "','" & Label10.Text & "','" & TextBox1.Text & "','" & Label15.Text & "','" & TextBox2.Text & "','" & Label16.Text & "','" & MasterMenu.Panel1.Text & "')", Conn)
            CMD.ExecuteNonQuery()


            For baris As Integer = 0 To DGV.RowCount - 2
                'simpan data ke tabel detail secara berulang
                CMD = New OleDbCommand("insert into tbldetail values ('" & Label3.Text & "','" & DGV.Rows(baris).Cells(0).Value & "','" & DGV.Rows(baris).Cells(2).Value & "','" & DGV.Rows(baris).Cells(3).Value & "','" & DGV.Rows(baris).Cells(4).Value & "')", Conn)
                CMD.ExecuteNonQuery()

                CMD = New OleDbCommand("select * from tblbarang where kode_barang='" & DGV.Rows(baris).Cells(0).Value & "'", Conn)
                DR = CMD.ExecuteReader
                DR.Read()
                If DR.HasRows Then
                    'tambah stok barang secara berulang
                    CMD = New OleDbCommand("update tblbarang set stok='" & DR.Item("stok") + DGV.Rows(baris).Cells(3).Value & "' where kode_barang='" & DGV.Rows(baris).Cells(0).Value & "'", Conn)
                    CMD.ExecuteNonQuery()
                End If
            Next

            'mencetak faktur pembelian (laporan dan form cetak harus sudah dibuat sebelumnya)
            If MessageBox.Show("Cetak Faktur...?", "", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                Cetak.Show()
                Cetak.CRV.ReportSource = Nothing
                Cetak.CRV.SelectionFormula = "{tblPembelian.no_faktur} = '" & Label3.Text & "'"
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
