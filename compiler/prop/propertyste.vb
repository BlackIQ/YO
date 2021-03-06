﻿Imports System.Reflection
Imports YOCA

Public Class propertyste
    Friend Shared Sub invoke_property(clinecodestruc() As xmlunpkd.linecodestruc, ilmethod As ilformat._ilmethodcollection, propresult As identvalid._resultidentcvaild, inline As Integer, optval As tokenhared.token)
        If propresult.callintern = True Then
            Throw New NotImplementedException
        Else
            inv_external_property(clinecodestruc, ilmethod, propresult, inline, optval)
        End If
    End Sub

    Friend Shared Sub inv_external_property(clinecodestruc() As xmlunpkd.linecodestruc, ByRef _ilmethod As ilformat._ilmethodcollection, propresult As identvalid._resultidentcvaild, inline As Integer, optval As tokenhared.token)
        Dim classindex, namespaceindex As Integer
        Dim reclassname As String = String.Empty
        Dim isvirtualmethod As Boolean = False
        If libserv.get_extern_index_class(_ilmethod, propresult.exclass, namespaceindex, classindex, isvirtualmethod, reclassname) = -1 Then
            dserr.args.Add("Class '" & propresult.exclass & "' not found.")
            dserr.new_error(conserr.errortype.PROPERTYERROR, clinecodestruc(0).line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(clinecodestruc(0)), clinecodestruc(0).value))
            Return
        End If
        propresult.asmextern = libserv.get_extern_assembly(namespaceindex)
        Dim retpropertyinfo As PropertyInfo
        If libserv.get_extern_index_property(propresult.clident, namespaceindex, classindex, retpropertyinfo) = -1 Then
            dserr.args.Add("Property '" & propresult.clident.ToLower & "' not found.")
            dserr.new_error(conserr.errortype.PROPERTYERROR, clinecodestruc(0).line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(clinecodestruc(0)), propresult.clident))
            Return
        End If
        can_write(retpropertyinfo, propresult, clinecodestruc)
        'TODO : Properties support operators
        If optval <> tokenhared.token.ASSIDB Then
            get_inv_property(clinecodestruc, _ilmethod, propresult, inline)
        End If
        set_property(_ilmethod, retpropertyinfo, isvirtualmethod, inline, clinecodestruc, propresult, optval)
    End Sub

    Private Shared Sub set_property(_ilmethod As ilformat._ilmethodcollection, retpropertyinfo As PropertyInfo, isvirtualmethod As Boolean, inline As Integer, clinecodestruc() As xmlunpkd.linecodestruc, propresult As identvalid._resultidentcvaild, optval As tokenhared.token)
        Dim ldloc As New illdloc(_ilmethod)
        Dim gdatatype As String = retpropertyinfo.PropertyType.Name
        gdatatype = servinterface.vb_to_cil_common_data_type(gdatatype)
        If servinterface.is_cil_common_data_type(gdatatype) = False Then
            gdatatype = retpropertyinfo.PropertyType.ToString
        End If
        ldloc.load_single_in_stack(gdatatype, clinecodestruc(inline))
        Dim paramtype As New ArrayList
        Dim propertyclass As String = retpropertyinfo.ReflectedType.ToString
        Dim propertymethod As String = String.Format("set_{0}", retpropertyinfo.Name)
        paramtype.Add(gdatatype)
        If isvirtualmethod Then
            Dim gidentifier As xmlunpkd.linecodestruc = clinecodestruc(0)
            If gidentifier.value.Contains(conrex.DBCLN) Then
                gidentifier.value = gidentifier.value.Remove(gidentifier.value.IndexOf(conrex.DBCLN))
                gidentifier.ile = gidentifier.value.Length
            End If
            ldloc.load_single_in_stack(retpropertyinfo.ReflectedType.ToString, gidentifier)
        End If
        check_operator(optval, _ilmethod)
        If isvirtualmethod Then
            cil.call_virtual_method(_ilmethod.codes, "void", propresult.asmextern, propertyclass, propertymethod, paramtype)
        Else
            cil.call_extern_method(_ilmethod.codes, "void", propresult.asmextern, propertyclass, propertymethod, paramtype)
        End If
        If convtc.setconvmethod Then convtc.set_type_cast(_ilmethod, gdatatype, propertymethod, clinecodestruc(inline))
    End Sub

    Private Shared Sub check_operator(optval As tokenhared.token, _ilmethod As ilformat._ilmethodcollection)
        Select Case optval
            Case tokenhared.token.ANDEQ
                cil.concat_simple(_ilmethod.codes)
        End Select
    End Sub

    Private Shared Sub can_write(retpropertyinfo As PropertyInfo, propresult As identvalid._resultidentcvaild, clinecodestruc() As xmlunpkd.linecodestruc)
        If retpropertyinfo.CanWrite = False Then
            dserr.args.Add("Property '" & propresult.clident.ToLower & "' is read only.")
            dserr.new_error(conserr.errortype.PROPERTYERROR, clinecodestruc(0).line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(clinecodestruc(0)), propresult.clident))
        End If
    End Sub

    Friend Shared Sub get_inv_property(clinecodestruc() As xmlunpkd.linecodestruc, ilmethod As ilformat._ilmethodcollection, propresult As identvalid._resultidentcvaild, inline As Integer)
        If propresult.callintern = True Then
            Throw New NotImplementedException
        Else
            get_external_property(clinecodestruc, ilmethod, propresult, inline)
        End If
    End Sub

    Friend Shared Sub get_external_property(clinecodestruc() As xmlunpkd.linecodestruc, ByRef _ilmethod As ilformat._ilmethodcollection, propresult As identvalid._resultidentcvaild, inline As Integer)
        Dim classindex, namespaceindex As Integer
        Dim reclassname As String = String.Empty
        Dim isvirtualmethod As Boolean = False
        If libserv.get_extern_index_class(_ilmethod, propresult.exclass, namespaceindex, classindex, isvirtualmethod, reclassname) = -1 Then
            dserr.args.Add("Class '" & propresult.exclass & "' not found.")
            dserr.new_error(conserr.errortype.PROPERTYERROR, clinecodestruc(0).line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(clinecodestruc(0)), clinecodestruc(0).value))
            Return
        End If
        propresult.asmextern = libserv.get_extern_assembly(namespaceindex)
        Dim retpropertyinfo As PropertyInfo
        If libserv.get_extern_index_property(propresult.clident, namespaceindex, classindex, retpropertyinfo) = -1 Then
            dserr.args.Add("Property '" & propresult.clident.ToLower & "' not found.")
            dserr.new_error(conserr.errortype.PROPERTYERROR, clinecodestruc(0).line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(clinecodestruc(0)), propresult.clident))
            Return
        End If
        can_read(retpropertyinfo, propresult, clinecodestruc)
        get_property(_ilmethod, retpropertyinfo, isvirtualmethod, inline, clinecodestruc, propresult)
    End Sub

    Private Shared Sub get_property(_ilmethod As ilformat._ilmethodcollection, retpropertyinfo As PropertyInfo, isvirtualmethod As Boolean, inline As Integer, clinecodestruc() As xmlunpkd.linecodestruc, propresult As identvalid._resultidentcvaild)
        Dim ldloc As New illdloc(_ilmethod)
        Dim gdatatype As String = retpropertyinfo.PropertyType.Name
        gdatatype = servinterface.vb_to_cil_common_data_type(gdatatype)
        If servinterface.is_cil_common_data_type(gdatatype) = False Then
            gdatatype = retpropertyinfo.PropertyType.ToString
        End If
        Dim propertyclass As String = retpropertyinfo.ReflectedType.ToString
        If isvirtualmethod Then
            Dim gidentifier As xmlunpkd.linecodestruc = clinecodestruc(0)
            If gidentifier.value.Contains(conrex.DBCLN) Then
                gidentifier.value = gidentifier.value.Remove(gidentifier.value.IndexOf(conrex.DBCLN))
                gidentifier.ile = gidentifier.value.Length
            End If
            ldloc.load_single_in_stack(retpropertyinfo.ReflectedType.ToString, gidentifier)
        End If
        Dim propertymethod As String = String.Format("get_{0}", retpropertyinfo.Name)
        If isvirtualmethod Then
            cil.call_virtual_method(_ilmethod.codes, gdatatype, propresult.asmextern, propertyclass, propertymethod, Nothing)
        Else
            cil.call_extern_method(_ilmethod.codes, gdatatype, propresult.asmextern, propertyclass, propertymethod, Nothing)
        End If
        If convtc.setconvmethod Then convtc.set_type_cast(_ilmethod, gdatatype, propertymethod, clinecodestruc(inline))
    End Sub

    Private Shared Sub can_read(retpropertyinfo As PropertyInfo, propresult As identvalid._resultidentcvaild, clinecodestruc() As xmlunpkd.linecodestruc)
        If retpropertyinfo.CanRead = False Then
            dserr.args.Add("Property '" & propresult.clident.ToLower & "' is write only.")
            dserr.new_error(conserr.errortype.PROPERTYERROR, clinecodestruc(0).line, ilbodybulider.path, authfunc.get_line_error(ilbodybulider.path, servinterface.get_target_info(clinecodestruc(0)), propresult.clident))
        End If
    End Sub
End Class
