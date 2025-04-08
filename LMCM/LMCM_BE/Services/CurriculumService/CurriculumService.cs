using AutoMapper;
using LMCM_BE.DTOs.CurriculumDtos;
using LMCM_BE.DTOs.CurriculumsSubjectDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Models.Constant;
using LMCM_BE.Repositories.CurriculumRepository;
using LMCM_BE.Repositories.CurriculumsSubjectRepository;
using LMCM_BE.Repositories.PloRepository;
using LMCM_BE.Repositories.PloSubjectRepository;
using LMCM_BE.Services.CurriculumsSubjectService;
using LMCM_BE.Services.PloService;
using LMCM_BE.Services.PloSubjectService;
using LMCM_BE.Services.SubjectService;
using LMCM_BE.UnitOfWork;
using LMCM_BE.Utilities;
using OfficeOpenXml;

namespace LMCM_BE.Services.CurriculumService
{
    public class CurriculumService : ICurriculumService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly ICurriculumsSubjectRepository _curriculumSubjectRepository;
        private readonly ICurriculumsSubjectService _curriculumsSubjectService;
        private readonly IPloRepository _ploRepository;
        private readonly IPloService _ploService;
        private readonly IPloSubjectRepository _ploSubjectRepository;
        private readonly IPloSubjectService _ploSubjectService;
        private readonly ISubjectService _subjectService;

        public CurriculumService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurriculumRepository curriculumRepository,
            ICurriculumsSubjectRepository curriculumSubjectRepository,
            ICurriculumsSubjectService curriculumsSubjectService,
            IPloRepository ploRepository,
            IPloService ploService,
            IPloSubjectRepository ploSubjectRepository,
            IPloSubjectService ploSubjectService,
            ISubjectService subjectService
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _curriculumRepository = curriculumRepository;
            _curriculumSubjectRepository = curriculumSubjectRepository;
            _curriculumsSubjectService = curriculumsSubjectService;
            _ploRepository = ploRepository;
            _ploService = ploService;
            _ploSubjectRepository = ploSubjectRepository;
            _ploSubjectService = ploSubjectService;
            _subjectService = subjectService;
        }

        public async Task<PagedResult<CurriculumDto>> GetCurriculumsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var (curriculums, totalCount) = await _curriculumRepository.GetCurriculumsAsync(searchKey, pageIndex, pageSize);

            var data = _mapper.Map<List<CurriculumDto>>(curriculums);

            return new PagedResult<CurriculumDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<bool> ImportCurriculumAsync(Curriculum curriculum)
        {
            if (curriculum == null)
                throw new ArgumentNullException(nameof(curriculum));
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await SoftCascadeDeleteCurriculumByCodeAsync(curriculum.CurriculumCode);
                await _curriculumRepository.ImportCurriculumAsync(curriculum);

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> SoftDeleteCurriculumAsync(Guid curriculumId)
        {
            var curriculum = await _curriculumRepository.GetActiveCurriculumByIdAsync(curriculumId);
            if (curriculum == null)
                throw new KeyNotFoundException("Không tìm thấy chương trình giảng dạy.");
            if (await _curriculumSubjectRepository.HasActiveCurriculumsSubjectsAsync(curriculumId) ||
                await _ploRepository.HasActivePloAsync(curriculumId) ||
                await _ploSubjectRepository.HasActivePloSubjectByCurriculumIdAsync(curriculumId))
                throw new InvalidOperationException("Không thể xóa môn học khi có thực thể liên quan đang hoạt động.");
 
            curriculum.Status = GenericStatus.Inactive;
            curriculum.UpdatedAt = DateTime.UtcNow;

            return await _curriculumRepository.UpdateCurriculumAsync(curriculum);
        }
        public async Task<bool> SoftCascadeDeleteCurriculumByCodeAsync(string curriculumCode)
        {
            var curriculum = await _curriculumRepository.GetActiveCurriculumByCodeAsync(curriculumCode);
            if (curriculum == null)
                return true;

            curriculum.Status = GenericStatus.Inactive;
            curriculum.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                await _curriculumsSubjectService.DeleteCurriculumsSubjectAsync(curriculum.CurriculumId);
                await _ploService.DeletePlosAsync(curriculum.CurriculumId);
                await _ploSubjectService.DeletePloSubjectsAsync(curriculum.CurriculumId);

                await _curriculumRepository.UpdateCurriculumAsync(curriculum);

                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<CurriculumDetailDto?> GetCurriculumDetailAsync(Guid curriculumId)
        {
            var curriculum = await _curriculumRepository.GetCurriculumDetailAsync(curriculumId);
            if (curriculum == null)
                throw new KeyNotFoundException("Không tìm thấy chương trình giảng dạy.");

            return curriculum == null ? null : _mapper.Map<CurriculumDetailDto>(curriculum);
        }
        public async Task<bool> ValidateSheets(ExcelWorkbook workbook, Dictionary<string, List<(string Header, string Cell)>> expectedHeaders)
        {
            var invalidSheetsName = new List<string>();
            var invalidSheetsHeader = new List<string>();

            foreach (var sheetName in expectedHeaders.Keys)
            {
                var worksheet = workbook.Worksheets[sheetName];

                if (worksheet == null)
                {
                    invalidSheetsName.Add(sheetName); // Sheet is missing
                    continue;
                }

                foreach (var (expectedHeader, cellAddress) in expectedHeaders[sheetName])
                {
                    string actualHeader = worksheet.Cells[cellAddress].Text.Trim();
                    if (!expectedHeader.Equals(actualHeader, StringComparison.OrdinalIgnoreCase))
                    {
                        invalidSheetsHeader.Add($"{sheetName} (ô {cellAddress}): kỳ vọng '{expectedHeader}' nhưng thấy '{actualHeader}'");
                    }
                }
            }

            if (invalidSheetsName.Count > 0)
            {
                throw new InvalidOperationException($"Không tìm thấy sheet: {string.Join(", ", invalidSheetsName)}");
            }

            if (invalidSheetsHeader.Count > 0)
            {
                throw new InvalidOperationException($"Sai header ở các ô sau:\n{string.Join("\n", invalidSheetsHeader)}");
            }

            return true;
        }
        public async Task<bool> ImportCurriculumFromWorkbookAsync(ExcelWorkbook workbook)
        {
            if (await ValidateSheets(workbook, ExpectedHeaders.CurriculumImportHeaders))
            {
                ExcelWorksheet curriculumSubjectSheet = workbook.Worksheets["Curriculum Subject"];
                ExcelWorksheet curriculumSheet = workbook.Worksheets["Curriculum"];
                ExcelWorksheet ploSheet = workbook.Worksheets["PLO"];
                ExcelWorksheet ploSubjectSheet = workbook.Worksheets["PLO Mappings"];

                // Read Curriculum data
                var curriculum = new Curriculum
                {
                    CurriculumId = Guid.NewGuid(),
                    CurriculumCode = curriculumSheet.Cells["C2"].Text,
                    CurriculumName = curriculumSheet.Cells["C3"].Text,
                    CurriculumNameEnglish = curriculumSheet.Cells["C4"].Text,
                    CurriculumDescription = curriculumSheet.Cells["C5"].Text,
                    VocationalCode = curriculumSheet.Cells["C6"].Text,
                    VocationalName = curriculumSheet.Cells["C7"].Text,
                    EnglishVocationalName = curriculumSheet.Cells["C8"].Text,
                    DecisionNo = curriculumSheet.Cells["C9"].Text,
                    ApprovedDate = DateTime.TryParse(curriculumSheet.Cells["C10"].Text, out DateTime approvedDate) ? approvedDate : null,
                    Status = GenericStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Plos = new List<Plo>(),
                    CurriculumsSubjects = new List<CurriculumsSubject>()
                };

                // Retrieve Curriculum Subjects
                var tempCurriculumsSubjects = new List<TempCurriculumsSubject>();
                var subjectCodes = new List<string>();

                int row = 2; // Start from row 2 (row 1 contains headers)
                while (!string.IsNullOrWhiteSpace(curriculumSubjectSheet.Cells[row, 1].Text))
                {
                    var tempCurriculumsSubject = new TempCurriculumsSubject
                    {
                        SubjectCode = curriculumSubjectSheet.Cells[row, 1].Text,
                        TermNo = int.TryParse(curriculumSubjectSheet.Cells[row, 4].Text, out int termNo) ? termNo : (int?)null,
                        Credit = int.TryParse(curriculumSubjectSheet.Cells[row, 5].Text, out int credit) ? credit : (int?)null,
                        Options = int.TryParse(curriculumSubjectSheet.Cells[row, 6].Text, out int options) ? options : (int?)null,
                    };

                    subjectCodes.Add(tempCurriculumsSubject.SubjectCode);
                    tempCurriculumsSubjects.Add(tempCurriculumsSubject);
                    row++;
                }

                // Validate Subjects in Database
                var existingSubjects = await _subjectService.GetActiveSubjectsByCodesAsync(subjectCodes);
                var missingSubjects = subjectCodes.Except(existingSubjects.Select(s => s.SubjectCode)).ToList();

                if (missingSubjects.Any())
                {
                    throw new KeyNotFoundException("Các môn học sau đây không tồn tại hoặc không hoạt động: " +
                        string.Join(", ", missingSubjects));
                }


                // Step 4: Convert Temporary Subjects to CurriculumsSubjects
                var curriculumsSubjects = new List<CurriculumsSubject>();
                foreach (var tempSubject in tempCurriculumsSubjects)
                {
                    var existingSubject = existingSubjects.FirstOrDefault(s => s.SubjectCode == tempSubject.SubjectCode);
                    if (existingSubject == null)
                        continue; // Skip if not found in DB

                    curriculumsSubjects.Add(new CurriculumsSubject
                    {
                        CurriculumId = curriculum.CurriculumId,
                        SubjectId = existingSubject.SubjectId,
                        TermNo = tempSubject.TermNo,
                        Credit = tempSubject.Credit,
                        Options = tempSubject.Options,
                        Status = GenericStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                // Read PLO data
                var plos = new List<Plo>();
                int ploRow = 2; // first row is headers
                while (!string.IsNullOrWhiteSpace(ploSheet.Cells[ploRow, 2].Text)) // Check if PLO Name exists
                {
                    plos.Add(new Plo
                    {
                        PloId = Guid.NewGuid(),
                        CurriculumId = curriculum.CurriculumId,
                        PloName = ploSheet.Cells[ploRow, 2].Text,
                        PloDescription = ploSheet.Cells[ploRow, 3].Text,
                        Status = GenericStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        PloSubjects = new List<PloSubject>(),
                    });
                    ploRow++;
                }

                // Read PLO-Subject Mapping (PloSubject)
                var ploSubjectMappings = new List<PloSubject>();
                int columnStart = 2; // PLOs start from column B
                int subjectRowStart = 3; // Subjects start from row 3

                for (int subjectRow = subjectRowStart; subjectRow <= ploSubjectSheet.Dimension.End.Row; subjectRow++)
                {
                    var subjectCode = ploSubjectSheet.Cells[subjectRow, 1].Text;

                    // Skip merged header rows
                    if (string.IsNullOrWhiteSpace(subjectCode) || ploSubjectSheet.Cells[subjectRow, 1].Merge)
                        continue;

                    // Get subject from curriculum subjects
                    var subject = existingSubjects.FirstOrDefault(s => s.SubjectCode == subjectCode);
                    if (subject == null)
                        continue;

                    for (int ploCol = columnStart; ploCol <= ploSubjectSheet.Dimension.End.Column; ploCol++)
                    {
                        var ploName = ploSubjectSheet.Cells[2, ploCol].Text; // PLO name from row 2
                        var plo = plos.FirstOrDefault(p => p.PloName == ploName);
                        if (plo != null && ploSubjectSheet.Cells[subjectRow, ploCol].Text == "ü")
                        {
                            ploSubjectMappings.Add(new PloSubject
                            {
                                PloId = plo.PloId,
                                SubjectId = subject.SubjectId,
                                Status = GenericStatus.Active,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }

                // Assign curriculum subjects
                curriculum.CurriculumsSubjects = curriculumsSubjects;

                // Assign PLOs to the curriculum
                curriculum.Plos = plos;

                // Assign PLO-Subject mappings to each PLO
                foreach (var plo in curriculum.Plos)
                {
                    plo.PloSubjects = ploSubjectMappings.Where(ps => ps.PloId == plo.PloId).ToList();
                }

                return await ImportCurriculumAsync(curriculum);
            }
            return false;
        }
    }
}