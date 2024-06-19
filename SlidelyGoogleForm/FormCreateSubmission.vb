Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Public Class FormCreateSubmission
    Private stopwatch As Stopwatch = New Stopwatch()

    Private Sub btnToggleStopwatch_Click(sender As Object, e As EventArgs) Handles btnToggleStopwatch.Click
        If stopwatch.IsRunning Then
            stopwatch.Stop()
        Else
            stopwatch.Start()
        End If
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        txtStopwatch.Text = String.Format("{0:hh\:mm\:ss}", stopwatch.Elapsed)
    End Sub

    Private Async Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        ' Implement API call to submit form
        Dim name = txtName.Text
        Dim email = txtEmail.Text
        Dim phone = txtPhone.Text
        Dim githubLink = txtGithubLink.Text
        Dim stopwatchTime = txtStopwatch.Text
        Await SubmitFormAsync(name, email, phone, githubLink, stopwatchTime)
    End Sub

    Private Async Function SubmitFormAsync(name As String, email As String, phone As String, githubLink As String, stopwatchTime As String) As Task
        Using client As New HttpClient()
            Try
                Dim formData As New With {
                    .name = name,
                    .email = email,
                    .phone = phone,
                    .github_link = githubLink,
                    .stopwatch_time = stopwatchTime
                }
                Dim json As String = JsonConvert.SerializeObject(formData)
                Dim content As New StringContent(json, Encoding.UTF8, "application/json")
                Dim response As HttpResponseMessage = Await client.PostAsync("http://localhost:3000/submit", content)
                response.EnsureSuccessStatusCode()
                MessageBox.Show("Submission successful!")

                Me.Close()
            Catch ex As HttpRequestException
                MessageBox.Show("Error during submission: " & ex.Message)
            End Try
        End Using
    End Function

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Select Case keyData
            Case Keys.Control Or Keys.T
                btnToggleStopwatch.PerformClick()
                Return True
            Case Keys.Control Or Keys.S
                btnSubmit.PerformClick()
                Return True
        End Select
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function
End Class
