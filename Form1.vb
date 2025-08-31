Imports K4os.Compression.LZ4.Encoders
Imports MySql.Data.MySqlClient
Public Class Form1
    Dim db As New DbConn()

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim conn As MySqlConnection = db.GetConnection()

        Try
            conn.Open()
            Dim query As String = "SELECT * FROM students"
            Dim cmd As New MySqlCommand(query, conn)
            Dim adapter As New MySqlDataAdapter(cmd)
            Dim table As New DataTable()

            adapter.Fill(table)
            DataGridView1.DataSource = table

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            conn.Close()
        End Try

        With DataGridView1

            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .RowHeadersVisible = False
            .Columns(0).HeaderText = "Student No"
            .Columns(1).HeaderText = "Name"
            .Columns(2).HeaderText = "Age"
            .Columns(3).Visible = False

            .Columns(0).Width = 100
            .Columns(1).Width = 200
            .Columns(2).Width = 50
            .DefaultCellStyle.Font = New Font("Segoe UI", 10) ' Change font size of DataGridView cells
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11, FontStyle.Bold) ' also increase the column header font

        End With

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim conn As MySqlConnection = db.GetConnection()

        Try
            ' Basic validation first
            If txtStudentID.Text.Trim() = "" Or txtName.Text.Trim() = "" Or cmbAge.Text.Trim() = "" Then
                MessageBox.Show("Please fill in all fields before saving.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            conn.Open()

            '  Check if Student ID already exists
            Dim checkQuery As String = "SELECT COUNT(*) FROM students WHERE student_id = @student_id"
            Dim checkCmd As New MySqlCommand(checkQuery, conn)
            checkCmd.Parameters.AddWithValue("@student_id", txtStudentID.Text)

            Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
            If count > 0 Then
                MessageBox.Show("A student with this ID already exists!", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '  Insert new record if no duplicate
            Dim query As String = "INSERT INTO students (student_id, name, age) VALUES (@student_id, @name, @age)"
            Dim cmd As New MySqlCommand(query, conn)

            cmd.Parameters.AddWithValue("@student_id", txtStudentID.Text)
            cmd.Parameters.AddWithValue("@name", txtName.Text)
            cmd.Parameters.AddWithValue("@age", cmbAge.Text)

            Dim rows As Integer = cmd.ExecuteNonQuery()
            If rows > 0 Then
                MessageBox.Show("Record saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Button1.PerformClick() '  refresh data
                btnClear.PerformClick()
            End If

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            conn.Close()
        End Try

    End Sub

    Private Sub DataGridView1_CellContentDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentDoubleClick


    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim conn As MySqlConnection = db.GetConnection()
        If DataGridView1.SelectedRows.Count > 0 Then
            ' Get student_id and student_name from the selected row
            Dim studentId As String = DataGridView1.SelectedRows(0).Cells(0).Value.ToString()
            Dim studentName As String = DataGridView1.SelectedRows(0).Cells(1).Value.ToString()

            ' Confirm deletion with student's name
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete the record of " & studentName & "?",
                                                         "Confirm Delete",
                                                         MessageBoxButtons.YesNo,
                                                         MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                Try
                    conn.Open()

                    Dim query As String = "DELETE FROM students WHERE student_id = @student_id"
                    Dim cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@student_id", studentId)

                    Dim rows As Integer = cmd.ExecuteNonQuery()
                    If rows > 0 Then
                        MessageBox.Show("Record deleted successfully!", "Saved", MessageBoxButtons.OK)

                        ' Remove from DataGridView visually
                        DataGridView1.Rows.RemoveAt(DataGridView1.SelectedRows(0).Index)
                        Button1.PerformClick()
                        btnClear.PerformClick()

                    Else
                        MessageBox.Show("No record deleted.", "Select Name", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If

                Catch ex As Exception
                    'MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)


                Finally
                    conn.Close()
                End Try
            End If
        Else
            MessageBox.Show("Please select a row to delete.", "Select Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If

    End Sub

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.RowIndex >= 0 Then ' Make sure it's not the header row
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

            ' Transfer values to your input controls
            txtStudentID.Text = row.Cells(0).Value.ToString()
            txtName.Text = row.Cells(1).Value.ToString()
            cmbAge.Text = row.Cells(2).Value.ToString()
        End If

    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim conn As MySqlConnection = db.GetConnection()

        If txtStudentID.Text.Trim() = "" Then
            MessageBox.Show("Please select a record to update by double-clicking from the list.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Try

            conn.Open()

            Dim query As String = "UPDATE students SET name = @name, age = @age WHERE student_id = @student_id"
            Dim cmd As New MySqlCommand(query, conn)

            cmd.Parameters.AddWithValue("@name", txtName.Text)
            cmd.Parameters.AddWithValue("@age", cmbAge.Text)
            cmd.Parameters.AddWithValue("@student_id", txtStudentID.Text)

            Dim rows As Integer = cmd.ExecuteNonQuery()
            If rows > 0 Then
                MessageBox.Show("Record updated successfully!", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Button1.PerformClick()
                btnClear.PerformClick()
            Else
                MessageBox.Show("No record updated. Please check the Student ID.")
            End If

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtName.Text = ""
        txtStudentID.Text = ""
        cmbAge.Text = ""

    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        Dim conn As MySqlConnection = db.GetConnection()

        Try
            conn.Open()
            ' Use LIKE for partial matching on student_no or name
            Dim query As String = "SELECT * FROM students WHERE student_ID LIKE @search OR name LIKE @search"
            Dim cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@search", "%" & txtSearch.Text & "%")
            Dim adapter As New MySqlDataAdapter(cmd)
            Dim table As New DataTable()

            adapter.Fill(table)
            DataGridView1.DataSource = table

            ' Reapply DataGridView formatting
            With DataGridView1
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
                .MultiSelect = False
                .RowHeadersVisible = False
                If .Columns.Count >= 3 Then
                    .Columns(0).HeaderText = "Student No"
                    .Columns(1).HeaderText = "Name"
                    .Columns(2).HeaderText = "Age"
                    If .Columns.Count > 3 Then .Columns(3).Visible = False
                    .Columns(0).Width = 100
                    .Columns(1).Width = 200
                    .Columns(2).Width = 50
                End If
                .DefaultCellStyle.Font = New Font("Segoe UI", 10)
                .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 11, FontStyle.Bold)
            End With

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            conn.Close()
        End Try

    End Sub
End Class
