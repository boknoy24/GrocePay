Imports MySql.Data.MySqlClient
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

Public Class AdminDB
    Dim conn As New MySqlConnection("Server=localhost;Database=grocepay;Uid=root;Pwd=;")

    Private Sub mySave_Click(sender As Object, e As EventArgs) Handles mySave.Click
        Dim name As String = myEmployeeName.Text.Trim()
        Dim email As String = myEmployeeEmail.Text.Trim()
        Dim password As String = myEmployeePassword.Text.Trim()
        Dim hashedPassword As String = HashPassword(password)
        Dim role As String = "employee"

        If Not IsValidEmail(email) Then
            MessageBox.Show("Invalid email format.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If IsEmailExists(email) Then
            MessageBox.Show("This email is already registered.", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If

            Dim userQuery As String = "INSERT INTO users (email, password, role, name) VALUES (@Email, @Password, @Role, @Name)"
            Using userCmd As New MySqlCommand(userQuery, conn)
                userCmd.Parameters.AddWithValue("@Email", email)
                userCmd.Parameters.AddWithValue("@Password", hashedPassword)
                userCmd.Parameters.AddWithValue("@Role", role)
                userCmd.Parameters.AddWithValue("@Name", name)
                userCmd.ExecuteNonQuery()
            End Using

            Dim userId As Integer
            Using cmd As New MySqlCommand("SELECT LAST_INSERT_ID()", conn)
                userId = Convert.ToInt32(cmd.ExecuteScalar())
            End Using

            Dim empQuery As String = "INSERT INTO employees (user_id, time_in, total_sales, time_out, date) VALUES (@UserId, NULL, NULL, NULL, NOW())"
            Using empCmd As New MySqlCommand(empQuery, conn)
                empCmd.Parameters.AddWithValue("@UserId", userId)
                empCmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Employee registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadEmployeeData()
            LoadEmployeeSalesData()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Function HashPassword(password As String) As String
        Using sha256 As New SHA256Managed()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim hash As Byte() = sha256.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "").ToLower()
        End Using
    End Function

    Private Function IsValidEmail(email As String) As Boolean
        Dim emailPattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
        Return Regex.IsMatch(email, emailPattern)
    End Function

    Private Function IsEmailExists(email As String) As Boolean
        Dim query As String = "SELECT COUNT(*) FROM users WHERE email = @Email"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Email", email)
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return count > 0
        End Using
    End Function

    Private Sub LoadEmployeeData()
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            Dim query As String = "SELECT e.user_id, u.name AS 'Name', u.email AS 'email', e.salary, e.deduction, e.time_in AS 'Time In', e.total_sales AS 'Total Sales', e.time_out AS 'Time Out', e.date AS 'Date' " &
                                  "FROM employees e JOIN users u ON e.user_id = u.id"

            Using cmd As New MySqlCommand(query, conn)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                DataGridView1.Rows.Clear()

                While reader.Read()
                    DataGridView1.Rows.Add(
                    reader("Name").ToString(),
                    reader("email").ToString(),
                    reader("salary").ToString(),
                    reader("deduction").ToString(),
                    reader("Time In").ToString(),
                    reader("Total Sales").ToString(),
                    reader("Time Out").ToString(),
                    reader("Date").ToString())
                End While

                reader.Close()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    Private Sub LoadEmployeeSalesData()
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            Dim query As String = "SELECT u.name AS 'Name', u.email AS 'Email' FROM users u"

            Using cmd As New MySqlCommand(query, conn)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                DataGridView2.Rows.Clear()

                While reader.Read()
                    DataGridView2.Rows.Add(
                    reader("Name").ToString(),
                    reader("Email").ToString())
                End While

                reader.Close()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Sub AdminDB_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadEmployeeData()
        LoadEmployeeSalesData()
    End Sub
    Private Sub myLogout_Click(sender As Object, e As EventArgs) Handles myLogout.Click
        Me.Close()
        Form1.Show()
    End Sub

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        Try
            If e.ColumnIndex = 2 Or e.ColumnIndex = 3 Or e.ColumnIndex = 5 Then


                Dim employeeEmail As String = DataGridView1.Rows(e.RowIndex).Cells(1).Value.ToString()

                Dim currentSalary As Decimal = 0
                Dim currentDeduction As Decimal = 0
                Dim currentTotalSales As Decimal = 0

                If Not String.IsNullOrEmpty(DataGridView1.Rows(e.RowIndex).Cells(2).Value.ToString()) Then
                    Decimal.TryParse(DataGridView1.Rows(e.RowIndex).Cells(2).Value.ToString(), currentSalary)
                End If

                If Not String.IsNullOrEmpty(DataGridView1.Rows(e.RowIndex).Cells(3).Value.ToString()) Then
                    Decimal.TryParse(DataGridView1.Rows(e.RowIndex).Cells(3).Value.ToString(), currentDeduction)
                End If

                If Not String.IsNullOrEmpty(DataGridView1.Rows(e.RowIndex).Cells(5).Value.ToString()) Then
                    Decimal.TryParse(DataGridView1.Rows(e.RowIndex).Cells(5).Value.ToString(), currentTotalSales)
                End If

                Using conn As New MySqlConnection("Server=localhost;Database=grocepay;Uid=root;Pwd=;")
                    conn.Open()
                    Dim updateQuery As String = "UPDATE employees SET " &
                                                "salary = IF(@Salary IS NULL, salary, @Salary), " &
                                                "deduction = IF(@Deduction IS NULL, deduction, @Deduction), " &
                                                "total_sales = IF(@TotalSales IS NULL, total_sales, @TotalSales) " &
                                                "WHERE user_id = (SELECT id FROM users WHERE email = @Email)"

                    Using cmd As New MySqlCommand(updateQuery, conn)
                        cmd.Parameters.AddWithValue("@Salary", If(e.ColumnIndex = 2, CType(currentSalary, Object), DBNull.Value))
                        cmd.Parameters.AddWithValue("@Deduction", If(e.ColumnIndex = 3, CType(currentDeduction, Object), DBNull.Value))
                        cmd.Parameters.AddWithValue("@TotalSales", If(e.ColumnIndex = 5, CType(currentTotalSales, Object), DBNull.Value))
                        cmd.Parameters.AddWithValue("@Email", employeeEmail)

                        Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                        If rowsAffected > 0 Then
                            MessageBox.Show("Data updated successfully.", "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            MessageBox.Show("No record updated. Please check the data.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            MessageBox.Show("Error updating data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


End Class
