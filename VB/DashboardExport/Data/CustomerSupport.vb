﻿Imports System
Imports System.Collections.Generic
Imports System.Data

Namespace DashboardExport
	Public Class CustomerSupportData
		Private customerCount As Integer
		Private employeeCount As Integer
		Private employees, products, issueTypes As DataTable
		Private endDate As Date = Date.Today
		Private issueDistributionCount As Integer
		Private issueTypesCount As Integer
		Private ReadOnly issueTypesTableName As String = "IssueTypes"
		Private ReadOnly items As New List(Of CustomerSupportItem)()
		Private productCount As Integer
		Private ReadOnly productsTableName As String = "Products"

		Private ReadOnly rand As New Random()
		Private startDate As New Date(Date.Today.Year - 1, 1, 1)
		Private startYear As Integer = Date.Today.Year

		Public Sub New(ByVal dsCustomerSupport As DataSet, ByVal dsEmployees As DataSet)
			LoadDataTables(dsCustomerSupport, dsEmployees)
			Dim monthResolvedDeviation As List(Of Integer) = GetMonthResolvedDeviation()
			Dim monthIssuesDeviation As List(Of Integer) = GetMonthIssuesDeviation()
			Dim yearDeviation As Dictionary(Of Integer, Integer) = GetYearDeviation()
			Dim employeeProducts As Dictionary(Of Integer, Integer) = GetEmployeeByProduct()
			Dim employeeSolvedDev As List(Of List(Of Integer)) = GetEmployeeSolvedDeviation()
			Dim issueDistribution As List(Of List(Of Integer)) = GetIssueDistribution()

			Do While startDate < endDate
				Dim count As Integer = rand.Next(monthIssuesDeviation(startDate.Month), monthIssuesDeviation(startDate.Month) + yearDeviation(startDate.Year) + 5)
				For i As Integer = 0 To count - 1
					Dim employeeIndex As Integer = rand.Next(0, employeeCount)
					Dim customerIndex As Integer = rand.Next(employeeCount - 1, customerCount)
					Dim productIndex As Integer = employeeProducts(employeeIndex)
					Dim issueTypeIndex As Integer = issueDistribution(productIndex)(rand.Next(0, issueDistributionCount))

					Dim issueSolvedAverage As Integer = CInt(Fix(issueTypes.Rows(issueTypeIndex)(3)))
					Dim issueSolvedDev As Integer = CInt(Fix(issueTypes.Rows(issueTypeIndex)(4))) + employeeSolvedDev(employeeIndex)(issueTypeIndex)

					items.Add(New CustomerSupportItem() With {.ProductName = CStr(products.Rows(productIndex)(1)), .Customer = CStr(employees.Rows(customerIndex)(1)), .Employee = CStr(employees.Rows(employeeIndex)(1)), .IssueType = CStr(issueTypes.Rows(issueTypeIndex)(1)), .IssueTypeIndex = CInt(Fix(issueTypes.Rows(issueTypeIndex)(2))), .ResolvedTime = rand.Next(Math.Max(0, issueSolvedAverage - issueSolvedDev), issueSolvedAverage + issueSolvedDev + monthResolvedDeviation(startDate.Month) - 2 * (startDate.Year - startYear)), .Opened = startDate})
				Next i
				startDate = startDate.AddDays(1)
			Loop
		End Sub

		Private Function GetEmployeeByProduct() As Dictionary(Of Integer, Integer)
			Dim employeeProducts As New Dictionary(Of Integer, Integer)()
			employeeCount = -1
			For i As Integer = 0 To productCount - 1
				For j As Integer = 0 To (CInt(Fix(products.Rows(i)(2)))) - 1
					employeeCount += 1
					employeeProducts.Add(employeeCount, i)
				Next j
			Next i
			employeeCount += 1
			Return employeeProducts
		End Function
		Private Function GetEmployeeSolvedDeviation() As List(Of List(Of Integer))
			Dim employeeSolvedDev As New List(Of List(Of Integer))()
			For i As Integer = 0 To employeeCount - 1
				employeeSolvedDev.Add(New List(Of Integer)(issueTypesCount))
				Dim solveDev As Integer = rand.Next(0, 5)
				For j As Integer = 0 To issueTypesCount - 1
					employeeSolvedDev(i).Add(rand.Next(solveDev - j, solveDev + j))
				Next j
			Next i
			Return employeeSolvedDev
		End Function
		Private Function GetIssueDistribution() As List(Of List(Of Integer))
			Dim issueDistribution As New List(Of List(Of Integer))()
			For k As Integer = 0 To productCount - 1
				issueDistribution.Add(New List(Of Integer)())
				For i As Integer = 0 To issueTypesCount - 1
					Dim count As Integer = rand.Next(CInt(Fix(issueTypes.Rows(i)(5))) - 1, CInt(Fix(issueTypes.Rows(i)(5))) + 1)
					For j As Integer = 0 To count - 1
						issueDistribution(k).Add(i)
					Next j
				Next i
			Next k
			issueDistributionCount = Int32.MaxValue
			For i As Integer = 0 To issueDistribution.Count - 1
				issueDistributionCount = Math.Min(issueDistributionCount, issueDistribution(i).Count)
			Next i
			Return issueDistribution
		End Function
		Private Function GetMonthIssuesDeviation() As List(Of Integer)
			Dim monthIssuesDeviation As New List(Of Integer)(12)
			For i As Integer = 0 To 12
				monthIssuesDeviation.Add(rand.Next(15, 20))
			Next i
			monthIssuesDeviation(5) -= 2
			monthIssuesDeviation(6) -= 7
			monthIssuesDeviation(7) -= 8
			monthIssuesDeviation(8) -= 9
			Return monthIssuesDeviation
		End Function
		Private Function GetMonthResolvedDeviation() As List(Of Integer)
			Dim monthResolvedDeviation As New List(Of Integer)(12)
			For i As Integer = 0 To 12
				monthResolvedDeviation.Add(rand.Next(0, 10))
			Next i
			Return monthResolvedDeviation
		End Function
		Private Function GetYearDeviation() As Dictionary(Of Integer, Integer)
			Dim yearDeviation As New Dictionary(Of Integer, Integer)()
			For i As Integer = startDate.Year To endDate.Year
				yearDeviation.Add(i, rand.Next(0, 10) + i - startYear)
			Next i
			Return yearDeviation
		End Function

		Private Sub LoadDataTables(ByVal dsCustomerSupport As DataSet, ByVal dsEmployees As DataSet)
			employees = dsEmployees.Tables("Employees")
			products = dsCustomerSupport.Tables(productsTableName)
			issueTypes = dsCustomerSupport.Tables(issueTypesTableName)
			productCount = products.Rows.Count
			customerCount = employees.Rows.Count
			issueTypesCount = issueTypes.Rows.Count
		End Sub

		Public ReadOnly Property CustomerSupport() As IEnumerable(Of CustomerSupportItem)
			Get
				Return items
			End Get
		End Property

		Public Class CustomerSupportItem
			Public Property Customer() As String
			Public Property Employee() As String
			Public Property IssueType() As String
			Public Property IssueTypeIndex() As Integer
			Public Property Opened() As Date
			Public Property ProductName() As String
			Public Property ResolvedTime() As Integer
		End Class
	End Class
End Namespace
