using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Services.SubjectService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using static LMCM_BE.Controllers.UserControllers.UserController;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LMCM_BE.Controllers.SubjectControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpPost("getSubjectList")]
        public async Task<IActionResult> GetSubjectAsync([FromBody] PagingRequest request)
        {
            try
            {
                var data = await _subjectService.GetSubjectsAsync(request.SearchKey, request.pageIndex, request.PageSize);
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound(new { message = "Data not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("importSubjects")]
        public async Task<IActionResult> ImportSubjectsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Please upload a valid Excel file." });

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        // Validate Headers
                        string[] expectedHeaders = {
                    "SubjectCode", "SubjectName", "English SubjectName", "PreviousCode",
                    "Is Constructivist Subject", "Method", "Duration", "Reality"
                };

                        for (int col = 1; col <= expectedHeaders.Length; col++)
                        {
                            if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                            {
                                return BadRequest(new { message = "Invalid Excel format. Please use the correct template." });
                            }
                        }

                        // Read and Process Data
                        int rowCount = worksheet.Dimension.Rows;

                        List<SubjectInsertDto> subjects = new List<SubjectInsertDto>();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var subject = new SubjectInsertDto
                            {
                                SubjectId = Guid.NewGuid(),
                                SubjectCode = worksheet.Cells[row, 1].Text,
                                SubjectName = worksheet.Cells[row, 2].Text,
                                EnglishSubjectName = worksheet.Cells[row, 3].Text,
                                PreviousSubjectCode = worksheet.Cells[row, 4].Text,
                                IsConstructivistSubject = worksheet.Cells[row, 5].Text.ToLower() == "true",
                                Method = worksheet.Cells[row, 6].Text,
                                Duration = int.TryParse(worksheet.Cells[row, 7].Text, out int duration) ? duration : 0,
                                Reality = int.TryParse(worksheet.Cells[row, 8].Text, out int reality) ? reality : 0
                            };

                            subjects.Add(subject);
                        }

                        var isSuccess = await _subjectService.ImportSubjectsAsync(subjects);

                        if (isSuccess)
                        {
                            return Ok(new { message = "Subjects imported successfully." });
                        }
                        return BadRequest(new { message = "Import failed." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
