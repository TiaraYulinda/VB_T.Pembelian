Imports System.Data.OleDb

Public Class RincianPenjualan

    Sub Bersihkan()
        DGV.Columns.Clear()
        Label6.Text = 0
        Label8.Text = 0
        Label10.Text = 0
        Label15.Text = 0
        Label16.Text = 0
        TextBox1.Text = 0
        TextBox2.Text = 0
    End Sub

    Private Sub RincianPenjualan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call Koneksi()
        Call Bersihkan()
        CMD = New OleDbCommand("select no_faktur from tblpenjualan", Conn)
        DR = CMD.ExecuteReader
        ListBox1.Items.Clear()
        Do While DR.Read
            ListBox1.Items.Add(DR.Item(0))
        Loop
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        CMD = New OleDbCommand("select * from tblpenjualan where no_faktur='" & ListBox1.Text & "'", Conn)
        DR = CMD.ExecuteReader
        DR.Read()
        If DR.HasRows Then
            Label3.Text = DR.Item("kode_customer")
            Label4.Text = DR.Item("tanggal")
            Label6.Text = DR.Item("jmlbarang")
            Label8.Text = DR.Item("grandtotal")
            Label10.Text = DR.Item("ppn")
            TextBox1.Text = DR.Item("diskon")
            Label15.Text = DR.Item("totalharga")
            TextBox2.Text = DR.Item("dibayar")
            Label16.Text = DR.Item("kembali")
            '===============================
            Label8.Text = FormatNumber(Label8.Text, 0)
            Label10.Text = FormatNumber(Label10.Text, 0)
            TextBox1.Text = FormatNumber(TextBox1.Text, 0)
            Label15.Text = FormatNumber(Label15.Text, 0)
            TextBox2.Text = FormatNumber(TextBox2.Text, 0)
            Label16.Text = FormatNumber(Label16.Text, 0)

            CMD = New OleDbCommand("select * from tblcustomer where kode_customer='" & Label3.Text & "'", Conn)
            DR = CMD.ExecuteReader
            DR.Read()
            If DR.HasRows Then
                Label3.Text = DR.Item("nama_customer")
            End If
        End If

        DA = New OleDbDataAdapter("select tbldetail.kode_barang as [Kode Barang],tblbarang.nama_barang as [Nama Barang],tbldetail.harga_jual as Harga,Qty,Total from tblbarang,tbldetail where tbldetail.kode_barang=tblbarang.kode_barang and tbldetail.no_faktur='" & ListBox1.Text & "'", Conn)
        DS = New DataSet
        DA.Fill(DS)
        DGV.DataSource = DS.Tables(0)
        DGV.ReadOnly = True

        DGV.DefaultCellStyle.BackColor = Color.AliceBlue
        DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.AntiqueWhite
        DGV.Columns(2).DefaultCellStyle.Format = "###,###,###"
        DGV.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        DGV.Columns(3).DefaultCellStyle.Format = "###,###,###"
        DGV.Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DGV.Columns(4).DefaultCellStyle.Format = "###,###,###"
        DGV.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        DGV.Columns(1).Width = 180
        DGV.Columns(3).Width = 50

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Cetak.Show()
        Cetak.CRV.SelectionFormula = "{tblPenjualan.no_faktur} ='" & ListBox1.Text & "'"
        cryRpt.Load("laporan per faktur.rpt")
        Call seting_laporan()
        Cetak.CRV.ReportSource = cryRpt
        Cetak.CRV.RefreshReport()
    End Sub
End Class