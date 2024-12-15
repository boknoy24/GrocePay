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
    End Sub

    ' Load Employee Data to DataGridView
    Private Sub LoadEmployeeDashboard()
        Try
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If

            Dim query As String = "SELECT e.date AS 'Date', e.time_in AS 'Time In', e.total_sales AS 'Total Sales', " &
                                  "e.time_out AS 'Time Out', e.salary AS 'Salary', e.deduction AS 'Deduction' " &
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
                            reader("Deduction").ToString()
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
End Class
