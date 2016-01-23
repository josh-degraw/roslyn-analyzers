// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.UnitTests;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Desktop.Analyzers.UnitTests
{
    public partial class CA3075DiagnosticAnalyzerTests : DiagnosticAnalyzerTestBase
    {
        private DiagnosticResult GetCA3075DataTableReadXmlSchemaCSharpResultAt(int line, int column, string name)
        {
            return GetCSharpResultAt(line, column, CA3075RuleId, string.Format(CA3075LoadXmlMessage, name, "ReadXmlSchema"));
        }

        private DiagnosticResult GetCA3075DataTableReadXmlSchemaBasicResultAt(int line, int column, string name)
        {
            return GetBasicResultAt(line, column, CA3075RuleId, string.Format(CA3075LoadXmlMessage, name, "ReadXmlSchema"));
        }

        [Fact]
        public void UseDataTableReadXmlSchemaShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.IO;
using System.Xml;
using System.Data;

namespace FxCopUnsafeXml
{
    public class UseXmlReaderForDataTableReadXmlSchema
    {
        public void TestMethod(Stream stream)
        {
            DataTable table = new DataTable();
            table.ReadXmlSchema(stream);
        }
    }
}
",
                GetCA3075DataTableReadXmlSchemaCSharpResultAt(13, 13, "TestMethod")
            );

            VerifyBasic(@"
Imports System.IO
Imports System.Xml
Imports System.Data

Namespace FxCopUnsafeXml
    Public Class UseXmlReaderForDataTableReadXmlSchema
        Public Sub TestMethod(stream As Stream)
            Dim table As New DataTable()
            table.ReadXmlSchema(stream)
        End Sub
    End Class
End Namespace",
                GetCA3075DataTableReadXmlSchemaBasicResultAt(10, 13, "TestMethod")
            );
        }

        [Fact]
        public void UseDataTableReadXmlSchemaInGetShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Data;

class TestClass
{
    public DataTable Test
    {
        get {
            var src = """";
            DataTable dt = new DataTable();
            dt.ReadXmlSchema(src);
            return dt;
        }
    }
}",
                GetCA3075DataTableReadXmlSchemaCSharpResultAt(11, 13, "TestClass")
            );

            VerifyBasic(@"
Imports System.Data

Class TestClass
    Public ReadOnly Property Test() As DataTable
        Get
            Dim src = """"
            Dim dt As New DataTable()
            dt.ReadXmlSchema(src)
            Return dt
        End Get
    End Property
End Class",
                GetCA3075DataTableReadXmlSchemaBasicResultAt(9, 13, "TestClass")
            );
        }

        [Fact]
        public void UseDataTableReadXmlSchemaInSetShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Data;

class TestClass
{
    DataTable privateDoc;
    public DataTable GetDoc
    {
        set
        {
            if (value == null)
            {
                var src = """";
                DataTable dt = new DataTable();
                dt.ReadXmlSchema(src);
                privateDoc = dt;
            }
            else
                privateDoc = value;
        }
    }
}",
                GetCA3075DataTableReadXmlSchemaCSharpResultAt(15, 17, "TestClass")
            );

            VerifyBasic(@"
Imports System.Data

Class TestClass
    Private privateDoc As DataTable
    Public WriteOnly Property GetDoc() As DataTable
        Set
            If value Is Nothing Then
                Dim src = """"
                Dim dt As New DataTable()
                dt.ReadXmlSchema(src)
                privateDoc = dt
            Else
                privateDoc = value
            End If
        End Set
    End Property
End Class",
                GetCA3075DataTableReadXmlSchemaBasicResultAt(11, 17, "TestClass")
            );
        }

        [Fact]
        public void UseDataTableReadXmlSchemaInTryBlockShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
  using System;
  using System.Data;

    class TestClass
    {
        private void TestMethod()
        {
            try
            {
                var src = """";
                DataTable dt = new DataTable();
                dt.ReadXmlSchema(src);
            }
            catch (Exception) { throw; }
            finally { }
        }
    }",
                GetCA3075DataTableReadXmlSchemaCSharpResultAt(13, 17, "TestMethod")
            );

            VerifyBasic(@"
Imports System.Data

Class TestClass
    Private Sub TestMethod()
        Try
            Dim src = """"
            Dim dt As New DataTable()
            dt.ReadXmlSchema(src)
        Catch generatedExceptionName As Exception
            Throw
        Finally
        End Try
    End Sub
End Class",
                GetCA3075DataTableReadXmlSchemaBasicResultAt(9, 13, "TestMethod")
            );
        }

        [Fact]
        public void UseDataTableReadXmlSchemaInCatchBlockShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
   using System;
   using System.Data;

    class TestClass
    {
        private void TestMethod()
        {
            try { }
            catch (Exception)
            {
                var src = """";
                DataTable dt = new DataTable();
                dt.ReadXmlSchema(src);
            }
            finally { }
        }
    }",
                GetCA3075DataTableReadXmlSchemaCSharpResultAt(14, 17, "TestMethod")
            );

            VerifyBasic(@"
Imports System.Data

Class TestClass
    Private Sub TestMethod()
        Try
        Catch generatedExceptionName As Exception
            Dim src = """"
            Dim dt As New DataTable()
            dt.ReadXmlSchema(src)
        Finally
        End Try
    End Sub
End Class",
                GetCA3075DataTableReadXmlSchemaBasicResultAt(10, 13, "TestMethod")
            );
        }

        [Fact]
        public void UseDataTableReadXmlSchemaInFinallyBlockShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
   using System;
   using System.Data;

    class TestClass
    {
        private void TestMethod()
        {
            try { }
            catch (Exception) { throw; }
            finally
            {
                var src = """";
                DataTable dt = new DataTable();
                dt.ReadXmlSchema(src);
            }
        }
    }",
                GetCA3075DataTableReadXmlSchemaCSharpResultAt(15, 17, "TestMethod")
            );

            VerifyBasic(@"
Imports System.Data

Class TestClass
    Private Sub TestMethod()
        Try
        Catch generatedExceptionName As Exception
            Throw
        Finally
            Dim src = """"
            Dim dt As New DataTable()
            dt.ReadXmlSchema(src)
        End Try
    End Sub
End Class",
                GetCA3075DataTableReadXmlSchemaBasicResultAt(12, 13, "TestMethod")
            );
        }

        [Fact]
        public void UseDataTableReadXmlSchemaInAsyncAwaitShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
 using System.Threading.Tasks;
using System.Data;

    class TestClass
    {
        private async Task TestMethod()
        {
            await Task.Run(() => {
                var src = """";
                DataTable dt = new DataTable();
                dt.ReadXmlSchema(src);
            });
        }

        private async void TestMethod2()
        {
            await TestMethod();
        }
    }",
                GetCA3075DataTableReadXmlSchemaCSharpResultAt(12, 17, "TestMethod")
            );

            VerifyBasic(@"
Imports System.Threading.Tasks
Imports System.Data

Class TestClass
    Private Function TestMethod() As Task
        Await Task.Run(Function() 
        Dim src = """"
        Dim dt As New DataTable()
        dt.ReadXmlSchema(src)

End Function)
    End Function

    Private Sub TestMethod2()
        Await TestMethod()
    End Sub
End Class",
                GetCA3075DataTableReadXmlSchemaBasicResultAt(10, 9, "TestMethod")
            );
        }

        [Fact]
        public void UseDataTableReadXmlSchemaInDelegateShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Data;

class TestClass
{
    delegate void Del();

    Del d = delegate () {
        var src = """";
        DataTable dt = new DataTable();
        dt.ReadXmlSchema(src);
    };
}",
                GetCA3075DataTableReadXmlSchemaCSharpResultAt(11, 9, "TestMethod")
            );

            VerifyBasic(@"
Imports System.Data

Class TestClass
    Private Delegate Sub Del()

    Private d As Del = Sub() 
    Dim src = """"
    Dim dt As New DataTable()
    dt.ReadXmlSchema(src)

End Sub
End Class",
                GetCA3075DataTableReadXmlSchemaBasicResultAt(10, 5, "TestMethod")
            );
        }

        [Fact]
        public void UseDataTableReadXmlSchemaWithXmlReaderShouldNotGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;
using System.Data;

namespace FxCopUnsafeXml
{
    public class UseXmlReaderForDataTableReadXmlSchema
    {
        public void TestMethod(XmlReader reader)
        {
            DataTable table = new DataTable();
            table.ReadXmlSchema(reader);
        }
    }
}
"
            );

            VerifyBasic(@"
Imports System.Xml
Imports System.Data

Namespace FxCopUnsafeXml
    Public Class UseXmlReaderForDataTableReadXmlSchema
        Public Sub TestMethod(reader As XmlReader)
            Dim table As New DataTable()
            table.ReadXmlSchema(reader)
        End Sub
    End Class
End Namespace");
        }
    }
}