using ServiceAPI.Authentication;
using ServiceAPI.Models.TransactionResponse;
using ServiceAPI.Models.TransactionRequest;
using ServiceAPI.Repositories.Interfaces; 

namespace ServiceAPI.Services
{
    public class TransactionService 
    {
        private readonly ITransactionRepository _TransactionRepo;
        private readonly ApplicationUser _applicationUser;
        private readonly ILogger<TransactionService> _logger;
        public TransactionService(
            ITransactionRepository driverPurchaseRepo,
            ApplicationUser applicationUser,
            ILogger<TransactionService> logger)
        {
            _TransactionRepo = driverPurchaseRepo;
            _applicationUser = applicationUser;
            _logger = logger;
        }
        public async Task<CommmandResponseGo> GetDataByCommandCode(CommmandRequestGoFirt request)
        {
            string errUpdate = "";
            CommmandResponseGo commmandResponse = new();
            try
            {
                if (string.IsNullOrEmpty(request.commandCode.ToString()))
                {
                    commmandResponse.errorCode = "03";
                    commmandResponse.description = "Thông tin mã lệnh bắt buộc";
                    commmandResponse.data = null;
                    return commmandResponse;
                }

                // Cập nhật trạng thái đã vào tài
                try
                {
                    errUpdate = await _TransactionRepo.UpdateVersionByCommandCode(request.commandCode); 
                }
                catch (Exception ex)
                {
                    errUpdate += " _ Không update được trạng thái vào tài vui lòng kiểm tra lại (" + ex.Message + ")";
                }

                var transactionCommandData = await _TransactionRepo.GetDatabyCommand(request.commandCode);
                if (transactionCommandData != null && transactionCommandData.Count() > 0)
                {
                    List<TransactionCommandGo> lsttransactionCommand = new();
                    for (int i = 0; i < transactionCommandData.Count; i++)
                    {
                        List<ItemInfoGo> lstIteminfo = new();
                        TransactionCommandGo transactionCommand = new();
                        transactionCommand.commandCode = transactionCommandData[i].command;
                        transactionCommand.transactionCode = transactionCommandData[i].transactionCode;
                        transactionCommand.transactionName = transactionCommandData[i].transactionName;
                        transactionCommand.transactionDate = Utils.DateTimeParse.DateTimeStrBOS(transactionCommandData[i].transactionDate).ToString();
                        transactionCommand.dateStop = Utils.DateTimeParse.DateTimeStrBOS(transactionCommandData[i].dateStop).ToString().ToString();
                        transactionCommand.dateStart = Utils.DateTimeParse.DateTimeStrBOS(transactionCommandData[i].dateStart).ToString();
                        if(transactionCommandData[i].dateReceived != 0)
                            transactionCommand.dateReceived = Utils.DateTimeParse.DateTimeStrBOS(transactionCommandData[i].dateReceived).ToString();
                        else
                            transactionCommand.dateReceived = "";
                        transactionCommand.customerCode = transactionCommandData[i].customerCode;
                        transactionCommand.customerName = transactionCommandData[i].customerName;
                        transactionCommand.stockType = transactionCommandData[i].stockType;
                        transactionCommand.saleType = transactionCommandData[i].saleType;
                        transactionCommand.vehicleCode = transactionCommandData[i].vehicleCode;


                        ItemInfoGo itemInfo = new ItemInfoGo();
                        itemInfo.lineNumber = 1;
                        itemInfo.itemCode = transactionCommandData[i].itemCode;
                        itemInfo.itemName = transactionCommandData[i].itemName;
                        itemInfo.unitCode = transactionCommandData[i].unitCode;
                        itemInfo.unitName = transactionCommandData[i].unitName;
                        itemInfo.quantity = transactionCommandData[i].quantity;
                        itemInfo.warehouseCode = transactionCommandData[i].warehouseCode;
                        itemInfo.warehouseName = transactionCommandData[i].warehouseName;
                        itemInfo.isType = transactionCommandData[i].isType;
                        lstIteminfo.Add(itemInfo);
                        transactionCommand.itemInfo = lstIteminfo;

                        // add vào list
                        lsttransactionCommand.Add(transactionCommand);
                    }
                    commmandResponse.errorCode = "00";
                    commmandResponse.description = "Thành công";
                    commmandResponse.data = lsttransactionCommand;
                }
                else
                {
                    commmandResponse.errorCode = "01";
                    commmandResponse.description = "Không có dữ liệu từ ERP kiểm tra lại thông tin ID lệnh " + request.commandCode;
                    commmandResponse.data = null;
                }

                if (errUpdate == "")
                {
                    //commmandResponse.description += " _ Cập nhật trạng thái xuất hàng thành công";
                }
                else
                {
                    commmandResponse.description += " _ " + errUpdate;
                }    
                
            }
            catch (Exception ex)
            {
                commmandResponse.errorCode = "500";
                commmandResponse.description = "ERR:" + ex.Message + ex.StackTrace;
                commmandResponse.data = null;
            }
            return commmandResponse;
        }

        public async Task<CommmandResponseGo> GetTransactionByCommandCode(CommmandRequestGo request)
        {
            CommmandResponseGo commmandResponse = new(); 
            try
            { 
                if(request.fromDate == "" || request.toDate == "")
                {
                    commmandResponse.errorCode = "03";
                    commmandResponse.description = "Thông tin từ ngày đến ngày bắt buộc nhập";
                    commmandResponse.data = null;
                    return commmandResponse;
                }

                var transactionCommandData = await _TransactionRepo.GetDataTransactionbyCommand(request.fromDate, request.toDate, request.listCommandCode, request.listWarehouse);
                if (transactionCommandData != null && transactionCommandData.Count() > 0)
                {
                    List<TransactionCommandGo> lsttransactionCommand = new();
                    for (int i=0; i< transactionCommandData.Count; i++)
                    {
                        List<ItemInfoGo> lstIteminfo = new();
                        TransactionCommandGo transactionCommand = new();
                        transactionCommand.commandCode = transactionCommandData[i].command;
                        transactionCommand.transactionCode = transactionCommandData[i].transactionCode;
                        transactionCommand.transactionName = transactionCommandData[i].transactionName;
                        transactionCommand.transactionDate = Utils.DateTimeParse.DateTimeStrVN_Viettel(transactionCommandData[i].transactionDate).ToString();
                        transactionCommand.dateStop = Utils.DateTimeParse.DateTimeStrVN_Viettel(transactionCommandData[i].dateStop).ToString().ToString();
                        transactionCommand.dateStart = Utils.DateTimeParse.DateTimeStrVN_Viettel(transactionCommandData[i].dateStart).ToString();
                        transactionCommand.customerCode = transactionCommandData[i].customerCode;
                        transactionCommand.customerName = transactionCommandData[i].customerName;
                        transactionCommand.stockType = transactionCommandData[i].stockType;
                        transactionCommand.saleType = transactionCommandData[i].saleType;
                        transactionCommand.vehicleCode = transactionCommandData[i].vehicleCode;


                        Models.TransactionRequest.ItemInfoGo itemInfo = new Models.TransactionRequest.ItemInfoGo();
                        itemInfo.lineNumber = 1;
                        itemInfo.itemCode = transactionCommandData[i].itemCode;
                        itemInfo.itemName = transactionCommandData[i].itemName;
                        itemInfo.unitCode = transactionCommandData[i].unitCode;
                        itemInfo.unitName = transactionCommandData[i].unitName;
                        itemInfo.quantity = transactionCommandData[i].quantity;
                        itemInfo.warehouseCode = transactionCommandData[i].warehouseCode;
                        itemInfo.warehouseName = transactionCommandData[i].warehouseName;
                        lstIteminfo.Add(itemInfo);
                        transactionCommand.itemInfo = lstIteminfo;

                        // add vào list
                        lsttransactionCommand.Add(transactionCommand); 
                    }
                    commmandResponse.errorCode = "00";
                    commmandResponse.description = "Thành công";
                    commmandResponse.data = lsttransactionCommand;
                }
                else
                {
                    commmandResponse.errorCode = "01";
                    commmandResponse.description = "Không có dữ liệu từ ERP kiểm tra lại thông tin";
                    commmandResponse.data = null;
                }

                
            }
            catch (Exception ex)
            {
                commmandResponse.errorCode = "500";
                commmandResponse.description = "ERR:" + ex.Message + ex.StackTrace;
                commmandResponse.data = null; 
            }
            return commmandResponse;
        }

        public async Task<CommmandResponseTo> SaveTransactionbylistDataWH(CommmandRequestTo request)
        {
            CommmandResponseTo commmandResponse = new();
            try
            {
                if (request.data.Count() <= 0)
                {
                    commmandResponse.errorCode = "03";  
                    commmandResponse.description = "Thông tin gửi đi trống";
                    return commmandResponse;
                }
                else
                {
                    // Thực hiện lưu dữ liệu vào hệ thống ERP
                    var resut = await _TransactionRepo.SaveTransactionCode(request);
                    if (resut.errorCode == "00") 
                    {
                        commmandResponse.errorCode = "00";
                        commmandResponse.description = "Thành công";
                        commmandResponse.listCommandSuccess = resut.listCommandSuccess;
                    }
                    else
                    {
                        commmandResponse = resut;
                    }    
                } 
            }
            catch (Exception ex)
            {
                commmandResponse.errorCode = "500";
                commmandResponse.description = "ERR:" + ex.Message + ex.StackTrace;
            }
            return commmandResponse;
        }

        public async Task<CommmandResponseTo> SaveTransactionDOPToDOP1(CommmandRequestTo request)
        {
            CommmandResponseTo commmandResponse = new();
            try
            {
                if (request.data.Count() <= 0)
                {
                    commmandResponse.errorCode = "03";
                    commmandResponse.description = "Thông tin gửi đi trống";
                    return commmandResponse;
                }
                else
                {
                    // Thực hiện lưu dữ liệu vào hệ thống ERP
                    var resut = await _TransactionRepo.SaveTransactionCodeDOPToDOP1(request);
                    if (resut.errorCode == "00")
                    {
                        commmandResponse.errorCode = "00";
                        commmandResponse.description = "Thành công";
                        commmandResponse.listCommandSuccess = resut.listCommandSuccess;
                    }
                    else
                    {
                        commmandResponse = resut;
                    }
                }
            }
            catch (Exception ex)
            {
                commmandResponse.errorCode = "500";
                commmandResponse.description = "ERR:" + ex.Message + ex.StackTrace;
            }
            return commmandResponse;
        }
    }
}
