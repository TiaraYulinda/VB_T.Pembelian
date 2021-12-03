Imports System.Data.OleDb

Public Class LaporanPenjualan


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub LaporanPenjualan_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call Koneksi()
        CMD = New OleDbCommand("select distinct tanggal from tblpenjualan", Conn)
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
        CRV.SelectionFormula = "totext({tblPenjualan.TANGGAL}) ='" & ComboBox1.Text & "'"
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
        CRV.SelectionFormula = "{tblPenjualan.TANGGAL} in date ('" & ComboBox2.Text & "') to date ('" & ComboBox3.Text & "')"
        cryRpt.Load("laporan mingguan.rpt")
        Call seting_laporan()
        CRV.ReportSource = cryRpt
        CRV.RefreshReport()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        CRV.SelectionFormula = "month({tblPenjualan.TANGGAL}) = (" & Month(DateTimePicker1.Text) & ") and year({tblPenjualan.TANGGAL}) = (" & Year(DateTimePicker1.Text) & ")"
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
        CRV.SelectionFormula = "{tblPenjualan.TANGGAL} in date ('" & ComboBox4.Text & "') to date ('" & ComboBox5.Text & "')"
        cryRpt.Load("grafik.rpt")
        Call seting_laporan()
        CRV.ReportSource = cryRpt
        CRV.RefreshReport()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        CRV.SelectionFormula = "month({tblPenjualan.TANGGAL}) = (" & Month(DateTimePicker2.Text) & ") and year({tblPenjualan.TANGGAL}) = (" & Year(DateTimePicker2.Text) & ")"
        cryRpt.Load("grafik1.rpt")
        Call seting_laporan()
        CRV.ReportSource = cryRpt
        CRV.RefreshReport()
    End Sub
End Class