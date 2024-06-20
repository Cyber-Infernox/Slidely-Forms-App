Imports Newtonsoft.Json
Imports System.Net.Http

Public Class FormSearchSubmission
    Private submissions As List(Of Submission)

    Private Async Sub FormSearchSubmission_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await LoadSubmissionsAsync()
    End Sub

    Private Async Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim email As String = txtSearchEmail.Text.Trim()
        SearchSubmissionByEmail(email)
    End Sub

    Private Async Function LoadSubmissionsAsync() As Task
        Using client As New HttpClient()
            Try
                Dim response As HttpResponseMessage = Await client.GetAsync("http://localhost:3000/read")
                response.EnsureSuccessStatusCode()
                Dim json As String = Await response.Content.ReadAsStringAsync()
                submissions = JsonConvert.DeserializeObject(Of List(Of Submission))(json)
            Catch ex As HttpRequestException
                MessageBox.Show("Error fetching submissions: " & ex.Message)
            End Try
        End Using
    End Function

    Private Sub SearchSubmissionByEmail(email As String)
        Dim foundSubmission As Submission = submissions.FirstOrDefault(Function(submission) submission.Email.Equals(email, StringComparison.OrdinalIgnoreCase))

        If foundSubmission IsNot Nothing Then
            DisplaySubmission(foundSubmission)
        Else
            MessageBox.Show("Submission not found.")
            ClearSubmissionDetails()
        End If
    End Sub

    Private Sub DisplaySubmission(submission As Submission)
        txtName.Text = submission.Name
        txtEmail.Text = submission.Email
        txtPhone.Text = submission.Phone
        txtGithubLink.Text = submission.github_link
        txtStopwatch.Text = submission.stopwatch_time
    End Sub

    Private Sub ClearSubmissionDetails()
        txtName.Text = ""
        txtEmail.Text = ""
        txtPhone.Text = ""
        txtGithubLink.Text = ""
        txtStopwatch.Text = ""
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Select Case keyData
            Case Keys.Control Or Keys.S
                btnSearch.PerformClick()
                Return True
        End Select
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function
End Class

