Imports MySql.Data.MySqlClient

Public Class EmployeeDB
    Dim conn As New MySqlConnection("Server=localhost;Database=grocepay;Uid=root;Pwd=;")
    Private employeeEmail As String

    Public Sub New(email As String)
        InitializeComponent()
        employeeEmail = email
    End Sub

    Private Sub EmployeeDB_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadEmployeeDashboard()
        WelcomeUser()
        Label2.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mmtt")
    End Sub

    Private Sub WelcomeUser()
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If

            Dim query As String = "SELECT name FROM users WHERE email = @Email"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Email", employeeEmail)

                Dim employeeName As String = cmd.ExecuteScalar()?.ToString()

                If Not String.IsNullOrEmpty(employeeName) Then
                    Label1.Text = "Welcome, " & employeeName
                Else
                    Label1.Text = "Welcome, Employee"
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error fetching user data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
    Private Sub LoadEmployeeDashboard()
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If

            Dim query As String = "SELECT e.date AS 'Date', e.time_in AS 'Time In', e.total_sales AS 'Total Sales', " &
                                  "e.time_out AS 'Time Out', e.salary AS 'Salary', e.deduction AS 'Deduction', u.name AS 'Name' " &
                                  "FROM employees e " &
                                  "JOIN users u ON e.user_id = u.id " &
                                  "WHERE u.email = @Email"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Email", employeeEmail)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    DataGridView1.Rows.Clear()

                    While reader.Read()
                        DataGridView1.Rows.Add(
                            reader("Date").ToString(),
                            reader("Time In").ToString(),
                            reader("Total Sales").ToString(),
                            reader("Time Out").ToString(),
                            reader("Salary").ToString(),
                            reader("Deduction").ToString(),
                            reader("Name").ToString()
                        )
                    End While
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    Private Sub myTimeIN_Click(sender As Object, e As EventArgs) Handles myTimeIN.Click
        Try
            Using conn As New MySqlConnection("Server=localhost;Database=grocepay;Uid=root;Pwd=;")
                conn.Open()
                Dim userId As Integer
                Using userCheckCmd As New MySqlCommand("SELECT id FROM users WHERE email = @Email", conn)
                    userCheckCmd.Parameters.AddWithValue("@Email", employeeEmail)
                    userId = Convert.ToInt32(userCheckCmd.ExecuteScalar())
                End Using

                If userId = 0 Then
                    MessageBox.Show("User not found in database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                ' Check if the user has already clocked in today
                Dim checkQuery As String = "SELECT COUNT(*) FROM employees WHERE user_id = @UserId AND DATE(time_in) = @Date"
                Using checkCmd As New MySqlCommand(checkQuery, conn)
                    checkCmd.Parameters.AddWithValue("@UserId", userId)
                    checkCmd.Parameters.AddWithValue("@Date", DateTime.Now.Date)

                    Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                    If count > 0 Then
                        MessageBox.Show("You have already clocked in today.", "Time In Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End Using

                ' Clock In
                Dim timeIn As DateTime = DateTime.Now
                Dim insertQuery As String = "INSERT INTO employees (user_id, time_in, date) VALUES (@UserId, @TimeIn, @Date)"
                Using cmd As New MySqlCommand(insertQuery, conn)
                    cmd.Parameters.AddWithValue("@UserId", userId)
                    cmd.Parameters.AddWithValue("@TimeIn", timeIn)
                    cmd.Parameters.AddWithValue("@Date", timeIn.Date)

                    If cmd.ExecuteNonQuery() > 0 Then
                        MessageBox.Show("You have successfully clocked in!", "Time In", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        LoadEmployeeDashboard()
                    Else
                        MessageBox.Show("Failed to clock in. Please try again.", "Time In Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub myTimeOut_Click(sender As Object, e As EventArgs) Handles myTimeOut.Click
        Try
            Using conn As New MySqlConnection("Server=localhost;Database=grocepay;Uid=root;Pwd=;")
                conn.Open()

                Dim userId As Integer
                Using userCheckCmd As New MySqlCommand("SELECT id FROM users WHERE email = @Email", conn)
                    userCheckCmd.Parameters.AddWithValue("@Email", employeeEmail)
                    userId = Convert.ToInt32(userCheckCmd.ExecuteScalar())
                End Using

                If userId = 0 Then
                    MessageBox.Show("User not found in database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                ' Check if the user has already clocked in and not clocked out yet
                Dim checkQuery As String = "SELECT time_in, time_out FROM employees WHERE user_id = @UserId AND DATE(time_in) = @Date"
                Using checkCmd As New MySqlCommand(checkQuery, conn)
                    checkCmd.Parameters.AddWithValue("@UserId", userId)
                    checkCmd.Parameters.AddWithValue("@Date", DateTime.Now.Date)

                    Using reader As MySqlDataReader = checkCmd.ExecuteReader()
                        If reader.Read() Then
                            ' If time_in is NULL, user hasn't clocked in yet
                            If reader("time_in") Is DBNull.Value Then
                                MessageBox.Show("You need to clock in before clocking out.", "Time Out Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Return
                            End If

                            ' If time_out is not NULL, user has already clocked out
                            If reader("time_out") IsNot DBNull.Value Then
                                MessageBox.Show("You have already clocked out today.", "Time Out Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Return
                            End If
                        Else
                            MessageBox.Show("No record found for today. Please clock in first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If
                    End Using
                End Using

                ' Clock Out
                Dim timeOut As DateTime = DateTime.Now
                Dim updateQuery As String = "UPDATE employees SET time_out = @TimeOut WHERE user_id = @UserId AND DATE(time_in) = @Date"
                Using updateCmd As New MySqlCommand(updateQuery, conn)
                    updateCmd.Parameters.AddWithValue("@UserId", userId)
                    updateCmd.Parameters.AddWithValue("@TimeOut", timeOut)
                    updateCmd.Parameters.AddWithValue("@Date", timeOut.Date)

                    If updateCmd.ExecuteNonQuery() > 0 Then
                        MessageBox.Show("You have successfully clocked out!", "Time Out", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        LoadEmployeeDashboard()
                    Else
                        MessageBox.Show("Error updating time out. Please try again later.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        If e.ColumnIndex = 2 Then
            Try
                Dim selectedDate As String = DataGridView1.Rows(e.RowIndex).Cells(0).Value.ToString()
                Dim totalSales As Decimal = Convert.ToDecimal(DataGridView1.Rows(e.RowIndex).Cells(2).Value)

                Using conn As New MySqlConnection("Server=localhost;Database=grocepay;Uid=root;Pwd=;")
                    conn.Open()

                    Dim updateQuery As String = "UPDATE employees SET total_sales = @TotalSales WHERE user_id = (SELECT id FROM users WHERE email = @Email) AND DATE(date) = @Date"

                    Using cmd As New MySqlCommand(updateQuery, conn)
                        cmd.Parameters.AddWithValue("@TotalSales", totalSales)
                        cmd.Parameters.AddWithValue("@Email", employeeEmail)
                        cmd.Parameters.AddWithValue("@Date", DateTime.Parse(selectedDate).Date)

                        Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                        If rowsAffected > 0 Then
                            MessageBox.Show("Total Sales updated successfully.", "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            MessageBox.Show("No record updated. Please check the date.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error updating Total Sales: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub



    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
        Form1.Show()
    End Sub
End Class
