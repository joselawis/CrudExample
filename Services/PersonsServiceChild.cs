using System.Drawing;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RepositoryContracts;

namespace Services;

public class PersonsServiceChild : PersonsService
{
    public PersonsServiceChild(
        IPersonsRepository personsRepository,
        ILogger<PersonsServiceChild> logger
    )
        : base(personsRepository, logger) { }

    public override async Task<MemoryStream> GetPersonsExcel()
    {
        var memoryStream = new MemoryStream();
        using var excelPackage = new ExcelPackage(memoryStream);
        var worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
        worksheet.Cells["A1"].Value = "Person Name";
        worksheet.Cells["B1"].Value = "Email";

        using var headerCells = worksheet.Cells["A1:B1"];
        headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        headerCells.Style.Font.Bold = true;

        var row = 2;
        var persons = await GetAllPersons();

        foreach (var person in persons)
        {
            worksheet.Cells[row, 1].Value = person.PersonName;
            worksheet.Cells[row, 2].Value = person.Email;
            row++;
        }

        worksheet.Cells[$"A1:B{row}"].AutoFitColumns();
        await excelPackage.SaveAsync();

        memoryStream.Position = 0;
        return memoryStream;
    }
}
