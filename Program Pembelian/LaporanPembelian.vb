Imports System.Data.OleDb

Public Class LaporanPembelian

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub LaporanPembelian_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call Koneksi()
        CMD = New OleDbCommand("select distinct tanggal from tblPembelian", Conn) 'menampilkan tanggal di beberapa combo
        DR = CMD.ExecuteReader
        Do While DR.Read
            ComboBox1.Items.Add(DR.Item(0))
            ComboBox2.Items.Add(DR.Item(0))
            ComboBox3.Items.Add(DR.Item(0))
            ComboBox4.Items.Add(DR.Item(0))
            ComboBox5.Items.Add(DR.Item(0))
        Loop
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        CRV.SelectionFormula = "totext({tblPembelian.TANGGAL}) ='" & ComboBox1.Text & "'"   'mencari data di tanggal teretntu
        cryRpt.Load("laporan harian.rpt")
        Call seting_laporan()
        CRV.ReportSource = cryRpt
        CRV.RefreshReport()
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox3.SelectedIndexChanged
        If ComboBox2.Text = "" Then
            MsgBox("Tanggal awal harus diisi")
            Exit Sub
        End If
        'mencari tanggal antara
        CRV.SelectionFormula = "{tblPembelian.TANGGAL} in date ('" & ComboBox2.Text & "') to date ('" & ComboBox3.Text & "')"
        cryRpt.Load("laporan mingguan.rpt")
        Call seting_laporan()
        CRV.ReportSource = cryRpt
        CRV.RefreshReport()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        'mencari bulan dan tahun tertentu
        CRV.SelectionFormula = "month({tblPembelian.TANGGAL}) = (" & Month(DateTimePicker1.Text) & ") and year({tblPembelian.TANGGAL}) = (" & Year(DateTimePicker1.Text) & ")"
        cryRpt.Load("LAPORAN bulanan.rpt")
        Call seting_laporan()
        CRV.ReportSource = cryRpt
        CRV.RefreshReport()
    End Sub

    Private Sub ComboBox5_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox5.SelectedIndexChanged
        If ComboBox4.Text = "" Then
            MsgBox("Tanggal awal harus diisi")
            Exit Sub
        End If
        'mencari tanggal antara
        CRV.SelectionFormula = "{tblPembelian.TANGGAL} in date ('" & ComboBox4.Text & "') to date ('" & ComboBox5.Text & "')"
        cryRpt.Load("grafik.rpt")
        Call seting_laporan()
        CRV.ReportSource = cryRpt
        CRV.RefreshReport()
    End Sub


End Class
