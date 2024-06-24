using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PiHire.API.Common;
using PiHire.API.Common.Hubs;
using PiHire.BAL.Common.Extensions;
using PiHire.BAL.IRepositories;
using PiHire.BAL.ViewModels;
using PiHire.o_SD.Controllers;

namespace PiHire.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class OffersController : BaseController
    {
        private readonly IHubContext<NotificationHub> hubContext;
        readonly ILogger<OffersController> logger;
        private readonly AppSettings _appSettings;
        private readonly IOffersRepository offerRepository;
        private readonly INotificationRepository notificationRepository;

        public OffersController(ILogger<OffersController> logger,
            AppSettings appSettings,
            IOffersRepository offerRepository, ILoggedUserDetails loginUserDetails, IHubContext<NotificationHub> hubContext, INotificationRepository notificationRepository) : base(offerRepository, loginUserDetails, hubContext, notificationRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings;
            this.offerRepository = offerRepository;
            this.notificationRepository = notificationRepository;
            this.hubContext = hubContext;
        }


        #region SALARY SLAB


        [HttpGet("Salary/Slabs")]
        public async Task<IActionResult> GetSlabsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await offerRepository.GetSlabs();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Salary/Slab/{id:int}")]
        public async Task<IActionResult> GetSlabAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await offerRepository.GetSlab(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Salary/Slab")]
        public async Task<IActionResult> CreateSlabAsync(CreateSlabViewModel createSlabViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.CreateSlab(createSlabViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("Salary/Slab")]
        public async Task<IActionResult> UpdateSlabAsync(UpdateSlabViewModel updateSlabViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await offerRepository.UpdateSlab(updateSlabViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Salary/Slab/{Id:int}")]
        public async Task<IActionResult> DeleteSlabAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.DeleteSlab(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

        #region SALARY Component

        [HttpGet("Salary/Components")]
        public async Task<IActionResult> GetComponentsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await offerRepository.GetSalaryComponets();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Salary/Component/{id:int}")]
        public async Task<IActionResult> GetComponentAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await offerRepository.GetSalaryComponet(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Salary/Component")]
        public async Task<IActionResult> CreateComponentAsync(CreateSalayComponentViewModel createSalayComponentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.CreateSalaryComponet(createSalayComponentViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("Salary/Component")]
        public async Task<IActionResult> UpdateComponentAsync(UpdateSalayComponentViewModel updateSalayComponentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await offerRepository.UpdateSalaryComponet(updateSalayComponentViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Salary/Component/{Id:int}")]
        public async Task<IActionResult> DeleteComponentAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.DeleteSalaryComponet(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

        #region SALARY SLABS WISE COMPS

        [HttpGet("Salary/SlabComponents")]
        public async Task<IActionResult> GetSlabComponentsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await offerRepository.GetSlabComponets();
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Salary/SlabComponent/{id:int}")]
        public async Task<IActionResult> GetSlabComponentAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await offerRepository.GetSlabComponet(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Salary/SlabComponent")]
        public async Task<IActionResult> CreateSlabComponentAsync(CreateSlabComponentViewModel createSlabComponentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.CreateSlabComponet(createSlabComponentViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("Salary/SlabComponent")]
        public async Task<IActionResult> UpdateSlabComponentAsync(UpdateSlabComponentViewModel updateSlabComponentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var response = await offerRepository.UpdateSlabComponet(updateSlabComponentViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Salary/SlabComponent/{Id:int}")]
        public async Task<IActionResult> DeleteSlabComponentAsync(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.DeleteSlabComponet(Id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region OFFER LETTER 

        [HttpGet("GetJobCanOffers/{JobId:int}/{CanProfId:int}")]
        public async Task<IActionResult> GetJobCanOffersAsync(int JobId, int CanProfId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.GetJobOfferLetter(JobId, CanProfId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

    
        
        [HttpGet("DownloadOfferLetter/{OfferId:int}")]
        public async Task<IActionResult> DownloadOfferLetterAsync(int OfferId, [FromQuery] bool logo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.DownloadOfferLetter(OfferId, logo);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
       
        //[HttpGet("DownloadIntentLetter/{OfferId:int}/{logo:int}")]
        //public async Task<IActionResult> DownloadIntentLetterAsync(int OfferId, int logo)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequestCustom(ModelState);
        //    }
        //    try
        //    {               
        //        bool isLogo = logo == 1 ? true : false;

        //        var response = await offerRepository.DownloadIntentLetter(OfferId, isLogo);
        //        return Ok(response);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
                
        [HttpPost("ReleaseIntentOffer")]
        public async Task<IActionResult> ReleaseIntentOfferAsync(ReleaseIntentOfferViewModel releaseIntentOfferViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.ReleaseIntentOffer(releaseIntentOfferViewModel);
                if (response.Item1.Count > 0)
                {
                    await PushNotificationToClientAsync(response.Item1);
                }
                return Ok(response.Item2);
            }
            catch (Exception)
            {
                throw;
            }
        }

   
        
        
        [HttpGet("GetSlabComponetDtls/{Id:int}/{puId:int}")]
        public async Task<IActionResult> GetSlabComponetDtlsAsync(int Id,int puId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.GetSlabComponetDtls(Id, puId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("CreateOfferLetterWithSlab")]
        public async Task<IActionResult> CreateOfferLetterWithSlabAsync(CreateOfferLetterSlabViewModel createOfferLetterSlabViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.CreateOfferLetterWithSlab(createOfferLetterSlabViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("CreateOfferLetterWithOutSlab")]
        public async Task<IActionResult> CreateOfferLetterWithOutSlabAsync(CreateOfferLetterWithSlabViewModel createOfferLetterWithSlabViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.CreateOfferLetterWithOutSlab(createOfferLetterWithSlabViewModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region OFFERS (PRE JOINS + ON BOARDS + Success)


        [HttpPost("OfferdCandidateList")]
        public async Task<IActionResult> OfferdCandidateListAsync(OfferdCandidateSearchModel offerdCandidateSearchModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestCustom(ModelState);
            }
            try
            {
                var response = await offerRepository.GetOfferdCandidates(offerdCandidateSearchModel);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

      
        [HttpPost("Candidates/ExportToExcel")]
        public async Task<IActionResult> CandidatesExportToExcelAsync(OfferdCandidateSearchModel offerdCandidateSearchModel)
        {
            try
            {
                string WebRootFolder = Path.GetTempPath();
                var response = await offerRepository.GetOfferdCandidates(offerdCandidateSearchModel);
                // filename with customer Id and timestamp 
                string FileName = DateTime.UtcNow.ToString("dd_MM_yyyy_hh_mm_ss_fff") + ".xlsx";

                var memory = new MemoryStream();
                // creating dynamic file to append data 
                using (var fs = new FileStream(Path.Combine(WebRootFolder, FileName.Replace(" ", "_")), FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    XSSFCellStyle txtStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    XSSFDataFormat dataFormat = (XSSFDataFormat)workbook.CreateDataFormat();
                    txtStyle.SetDataFormat(dataFormat.GetFormat("@"));

                    XSSFCellStyle numStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    numStyle.SetDataFormat(dataFormat.GetFormat("00"));

                    // sheet one  
                    var CandidateSheet = workbook.CreateSheet("Candidates");

                    var rowIndex = 0;
                    var CandidateSheetHeaderRow = CandidateSheet.CreateRow(rowIndex);
                    XSSFCellStyle headerStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                    CandidateSheetHeaderRow.RowStyle = headerStyle;
                    headerStyle.WrapText = false;

                    CandidateSheetHeaderRow.CreateCell(0).SetCellValue("JoId");
                    CandidateSheetHeaderRow.CreateCell(1).SetCellValue("CandProfID");
                    CandidateSheetHeaderRow.CreateCell(2).SetCellValue("FullName");
                    CandidateSheetHeaderRow.CreateCell(3).SetCellValue("EmailID");
                    CandidateSheetHeaderRow.CreateCell(4).SetCellValue("Status");
                    CandidateSheetHeaderRow.CreateCell(5).SetCellValue("Account");
                    CandidateSheetHeaderRow.CreateCell(6).SetCellValue("Recuiter");
                    CandidateSheetHeaderRow.CreateCell(7).SetCellValue("ContactNo");
                    CandidateSheetHeaderRow.CreateCell(8).SetCellValue("PassportNumber");
                    CandidateSheetHeaderRow.CreateCell(9).SetCellValue("ApporxJoining");
                    CandidateSheetHeaderRow.CreateCell(10).SetCellValue("BaseLocation");

                    // Adding Customer Details to sheet one 
                    rowIndex = 0;
                    for (int i = 0; i < response.Result.OfferdCandidateList.Count; i++)
                    {
                        rowIndex += 1;
                        var CandidateSheetDtls = CandidateSheet.CreateRow(rowIndex);
                        XSSFCellStyle headerStyleData = (XSSFCellStyle)workbook.CreateCellStyle();
                        CandidateSheetDtls.RowStyle = headerStyleData;
                        headerStyle.WrapText = false;

                        CandidateSheetDtls.CreateCell(0).SetCellValue(response.Result.OfferdCandidateList[i].JoId.ToString());
                        CandidateSheetDtls.CreateCell(1).SetCellValue(response.Result.OfferdCandidateList[i].CandProfID);
                        CandidateSheetDtls.CreateCell(2).SetCellValue(response.Result.OfferdCandidateList[i].CandName);
                        CandidateSheetDtls.CreateCell(3).SetCellValue(response.Result.OfferdCandidateList[i].EmailID);
                        CandidateSheetDtls.CreateCell(4).SetCellValue(response.Result.OfferdCandidateList[i].CandProfStatusName);
                        CandidateSheetDtls.CreateCell(5).SetCellValue(response.Result.OfferdCandidateList[i].ClientID + "-" + response.Result.OfferdCandidateList[i].ClientName);
                        CandidateSheetDtls.CreateCell(6).SetCellValue(response.Result.OfferdCandidateList[i].RecruiterName);
                        CandidateSheetDtls.CreateCell(7).SetCellValue(response.Result.OfferdCandidateList[i].ContactNo);
                        CandidateSheetDtls.CreateCell(8).SetCellValue(response.Result.OfferdCandidateList[i].PassportNumber);
                        CandidateSheetDtls.CreateCell(9).SetCellValue(response.Result.OfferdCandidateList[i].ApporxJoining.ToString());
                        CandidateSheetDtls.CreateCell(10).SetCellValue(response.Result.OfferdCandidateList[i].CurrLocation + " " + response.Result.OfferdCandidateList[i].CountryName);
                    }

                    workbook.Write(fs);
                }

                // to returning bytes 
                byte[] bytes = System.IO.File.ReadAllBytes(WebRootFolder + "\\" + FileName);

                string fileBytes = Convert.ToBase64String(bytes);

                // deleting uploaded file
                if ((System.IO.File.Exists(WebRootFolder + "\\" + FileName)))
                {
                    System.IO.File.Delete(WebRootFolder + "\\" + FileName);
                }
                return File(bytes, "application/ms-excel", FileName);

            }
            catch (Exception)
            {
                throw;
            }
        }

       
        [HttpPost("Candidates/ExportToCSV")]
        public async Task<IActionResult> CandidatesExportToCSVAsync(OfferdCandidateSearchModel offerdCandidateSearchModel)
        {
            try
            {

                var comlumHeadrs = new string[]
                {
                     "JoId",
            "CandProfID",
            "FullName",
            "EmailID",
            "Status",
            "Account",
            "Recuiter",
            "ContactNo",
            "PassportNumber",
            "ApporxJoining",
            "BaseLocation"
            };

                var response = await offerRepository.GetOfferdCandidates(offerdCandidateSearchModel);

                var records = (from data in response.Result.OfferdCandidateList
                               select new object[]
                                       {
            data.JoId,
            data.CandProfID,
            data.CandName,
            data.EmailID,
            data.CandProfStatusName,
            data.ClientID +"-"+data.ClientName,
            data.RecruiterName,
            data.ContactNo,
            data.PassportNumber,
            data.ApporxJoining,
            data.CurrLocation +"/"+data.CountryName
                                       }).ToList();

                // Build the file content
                var candidateCSV = new StringBuilder();
                records.ForEach(line =>
                {
                    candidateCSV.AppendLine(string.Join(",", line));
                });

                byte[] buffer = Encoding.ASCII.GetBytes($"{string.Join(",", comlumHeadrs)}\r\n{candidateCSV.ToString()}");
                return File(buffer, "text/csv", $"Candidates.csv");
            }
            catch (Exception)
            {
                throw;
            }

        }



        #endregion

    }
}
