﻿Imports System.IO

Public Class impfiles
    Public Shared Sub import_directory(dir As String)
        If Directory.Exists(dir) Then
            Dim files As New ArrayList
            Dim ilgen As ilgencode
            If get_colc_yo_files(dir, files, True) Then
                Dim tknfmtclass(files.Count - 1) As tknformat._class
                procresult.set_state("lex")
                For index = 0 To files.Count - 1
                    If cachelexunit.check_lexer_cache(tknfmtclass(index), files(index).ToString) = False Then
                        Dim lex As New lexer(files(index).ToString)
                        lex.lexme(tknfmtclass(index))
                    End If
                    servinterface.check_class_vaild(tknfmtclass(index).attribute, tknfmtclass(index).location)
                    funcdtproc.import_method(tknfmtclass(index))
                Next
                import_include_file(tknfmtclass)
                cachegtr.check_cache_repo(tknfmtclass)
                procresult.set_state("gen")
                ilgen = New ilgencode(tknfmtclass)
                ilgen.codegenerator()
                If compdt.NOCACHE = False AndAlso compdt.DEVMOD = False Then
                    Dim calex As New cachelexunit(tknfmtclass)
                    calex.lex_to_serialization()
                End If
            Else
                Return
            End If
        Else
            dserr.new_error(conserr.errortype.DIRNOTFOUND, -1, Nothing, dir & " => dir not found.")
        End If
    End Sub

    Private Shared Sub import_include_file(ByRef tknfmtclass As tknformat._class())
        cachelexunit.load_includes(tknfmtclass)
        If incfile.incspath.Count = 0 Then Return
        Dim bfindex As Integer = tknfmtclass.Length
        Array.Resize(tknfmtclass, tknfmtclass.Length + incfile.incspath.Count)
        For index = 0 To incfile.incspath.Count - 1
            If cachelexunit.check_lexer_cache(tknfmtclass(bfindex + index), incfile.incspath(index).ToString) = False Then
                Dim lex As New lexer(incfile.incspath(index).ToString)
                lex.lexme(tknfmtclass(bfindex + index))
            End If
            servinterface.check_class_vaild(tknfmtclass(bfindex + index).attribute, tknfmtclass(bfindex + index).location)
            funcdtproc.import_method(tknfmtclass(bfindex + index))
        Next
    End Sub
    ''' <summary>
    ''' Simple get file [ just in master route ]
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <param name="files"></param>
    ''' <returns></returns>
    Friend Shared Function get_yo_files(dir As String, ByRef files As ArrayList) As Boolean

        If Directory.GetFiles(dir).Count = 0 Then
            dserr.new_error(conserr.errortype.YOFILENOTFOUND, -1, Nothing, "path => " & dir)
            Return False
        End If

        Dim file As String = Nothing
        For index = 0 To Directory.GetFiles(dir).Count - 1
            file = Directory.GetFiles(dir)(index)
            If file.ToLower.EndsWith(conrex.YOFORMAT) Then
                files.Add(file)
            End If
        Next

        'TODO : Check files in directory .

        If files.Count = 0 Then
            dserr.new_error(conserr.errortype.YOFILENOTFOUND, -1, Nothing, "path => " & dir)
            Return False
        End If
        Return True
    End Function

    Friend Shared Function get_colc_yo_files(dir As String, ByRef files As ArrayList, Optional head As Boolean = False) As Boolean
        Dim file As String = Nothing
        For index = 0 To Directory.GetFiles(dir).Count - 1
            file = Directory.GetFiles(dir)(index)
            If file.ToLower.EndsWith(conrex.YOFORMAT) Then
                files.Add(file)
            End If
        Next

        Dim route As String = Nothing
        For index = 0 To Directory.GetDirectories(dir).Count - 1
            route = Directory.GetDirectories(dir)(index)
            get_colc_yo_files(route, files)
        Next

        If head Then
            If files.Count = 0 Then
                dserr.new_error(conserr.errortype.YOFILENOTFOUND, -1, Nothing, "path => " & dir)
                Return False
            End If
            Return True
        End If
        Return True
    End Function

End Class
