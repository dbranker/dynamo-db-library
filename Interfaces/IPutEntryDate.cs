using CasinoLibrary.Models;
using System;

namespace DynamoDBLibrary.Methods
{
    public interface IPutEntryDate
    {
        void Execute(Guid CompanyID, Guid EmployeeID, DateTime entryDate, CompanyEntry companyEntry);
    }
}