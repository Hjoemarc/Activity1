﻿Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient

Public Class DbConn
    Public Function GetConnection() As MySqlConnection
        Return New MySqlConnection("server=localhost;userid=root;password=;database=student")
    End Function
End Class
