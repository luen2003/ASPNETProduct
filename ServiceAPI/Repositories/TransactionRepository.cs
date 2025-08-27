using Azure.Core;
using BOSDLL.BOSL;
using Dapper;
using ServiceAPI.Context;
using ServiceAPI.Models.TransactionRequest;
using ServiceAPI.Models.TransactionResponse;
using ServiceAPI.Repositories.Interfaces;
using ServiceAPI.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceAPI.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly string _schemal = "S168";
        private readonly DapperContext _context;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private const string MapCustomter = "MD.Customer";
        private const string MapWareHouse = "S168.WH";
        private const string MapProduct = "MD.P";
        private const string MapStockType = "MD.StockType";
        private const string MapProductUnit = "MD.UoM";
        private const string MapVehicle = "MD.Utrans";
        private const string MapPos = "S168.POS";
        private const string MapUser = "MD.Users";

        public TransactionRepository(DapperContext context, ILogger<TransactionRepository> logger, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
        }
        public async Task<List<TransactionCommandDataGo>> GetDatabyCommand(int CommandCode)
        {
            string SqlWheredate = "";

            if (CommandCode > 0)
            {
                SqlWheredate += " And B = @CommandCode ";
            }


            var query = @"Select TR.T As TransactionCode, TR.N As TransactionName, TR.B As command , TR.D As transactionDate,
                        v_Ca.C As CustomerCode, v_Ca.N As CustomerName, TR.SecondDate As dateStop, TR.D As dateStart, dateReceived,
                        Cast(ST.C as nvarchar) + ' - ' + ST.N as stockType, Cast(S.C as nvarchar) + ' - ' + S.N as SaleTypeCode, PT.C As vehicleCode,
                        v_Pa.C As itemCode, v_Pa.N As itemName, UoM.C As unitCode, UoM.N As unitName, 
                        Tr.I6 as quantity, WH.C As warehouseCode, WH.N As warehouseName,
                        Case when TR2.N <> '' Then 2 Else Case when U2>0 Then 1 Else 0 End End As isType
                        From " + _schemal + @"." + @"T2011 TR  
                        Left Join (Select distinct RefI, N, U9 As dateReceived, i13 From " + _schemal + @"." + @"T2011 where T in ('DOP','DOP1')) TR2 On TR2.RefI=TR.I
                        Inner Join " + _schemal + @"." + @"v_Pa On v_Pa.I=TR.WaterID
                        Inner Join MD.UoM On UoM.I=TR.I1
                        Inner Join " + _schemal + @"." + @"v_Ca On v_Ca.I=TR.SrcID
                        Inner Join " + _schemal + @"." + @"v_SaleType S On S.I=TR.U3
                        Inner Join MD.StockType ST On ST.I=TR.U4
                        Left Join MD.Utrans PT On PT.I=TR2.I13
                        Inner Join " + _schemal + @"." + @"WH On WH.I=TR.DstID
                        where T='SOA' And S1=2 " + SqlWheredate ;
            var param = new DynamicParameters();
            if (CommandCode > 0)
            {
                param.Add(name: "CommandCode", CommandCode);
            }
            //Console.WriteLine(GetSqlWithParams(query, param));

            using IDbConnection connection = _context.CreateConnection();
            var transactionCommandData = await connection.QueryAsync<TransactionCommandDataGo>(query, param); 
            return transactionCommandData.ToList();
        }

        public async Task<List<TransactionCommandDataGo>> GetDataTransactionbyCommand(string fromdate, string todate, List<int> CommandCode, List<string> warehoure)
        {
            string SqlWheredate = "";
            long fromDate = 0, toDate = 0;
            if (fromdate != "")
                fromDate = Utils.DateTimeParse.ConvertDatetimestringToLong(fromdate);
            if (todate != "")
                toDate = Utils.DateTimeParse.ConvertDatetimestringToLong(todate);
            if (fromDate != 0)
            {
                SqlWheredate += " And D>= @fromDate ";
            }
            if (toDate != 0)
            {
                SqlWheredate += " And D<= @toDate ";
            }

            if (CommandCode.Count > 0)
            {
                SqlWheredate += " And U8 in @CommandCode ";
            }

            if (warehoure.Count > 0)
            {
                SqlWheredate += " And WH.C in @warehoure ";
            }

            var query = @"Select TR.N As TransactionCode, TR.T As TransactionName, U8 As command , TR.D As transactionDate,
                        v_Ca.C As CustomerCode, v_Ca.N As CustomerName, TR.D As dateStop, TR.SecondDate As dateStart, 
                        Cast(ST.C as nvarchar) + ' - ' + ST.N as stockType, Cast(S.C as nvarchar) + ' - ' + S.N as SaleTypeCode, '' As vehicleCode,
                        v_Pa.C As itemCode, v_Pa.N As itemName, UoM.C As unitCode, UoM.N As unitName, 
                        Tr.I6 as quantity, WH.C As warehouseCode, WH.N As warehouseName
                        From " + _schemal + @"." + @"T2011 TR  
                        Inner Join " + _schemal + @"." + @"v_Pa On v_Pa.I=TR.WaterID
                        Inner Join MD.UoM On UoM.I=TR.I1
                        Inner Join " + _schemal + @"." + @"v_Ca On v_Ca.I=TR.SrcID
                        Inner Join " + _schemal + @"." + @"v_SaleType S On S.I=TR.U3
                        Inner Join MD.StockType ST On ST.I=TR.U4
                        Inner Join " + _schemal + @"." + @"WH On WH.I=TR.DstID
                        where T='SOA' And S1=2 " + SqlWheredate;
            var param = new DynamicParameters();
            if (CommandCode.Count > 0)
            {
                param.Add(name: "CommandCode", CommandCode);
            }

            if (fromDate != 0)
            {
                param.Add(name: "fromDate", fromDate);
            }
            if (toDate != 0)
            {
                param.Add(name: "toDate", toDate);
            }
            if (warehoure.Count > 0)
            {
                param.Add(name: "warehoure", warehoure);
            }
            //Console.WriteLine(GetSqlWithParams(query, param));

            using IDbConnection connection = _context.CreateConnection();
            var transactionCommandData = await connection.QueryAsync<TransactionCommandDataGo>(query, param);
            return transactionCommandData.ToList();
        }

        public async Task<string> UpdateVersionByCommandCode(int batch)
        {
            string CheckErr = ""; 
            using IDbConnection connection = _context.CreateConnection(); 
            try
            {
                CheckErr = await EcxecUpdateVersionByCommandCode(connection, "T2011", batch); 
            }
            catch (Exception ex)
            {
                CheckErr = "Lỗi Update trạng thái vào xuất hàng" + ex.ToString(); 
            }
            return CheckErr;
        }

        public async Task<CommmandResponseTo> SaveTransactionCode(CommmandRequestTo request)
        {
            bool CheckErr = true;
            using IDbConnection connection = _context.CreateConnection();
            CommmandResponseTo cmdResponse = new(); 
            try
            {
                for (int i = 0; i < request.data.Count(); i++)
                {
                    //var data = await CreateTransactionbyCommmand(request.data[i], "T2011", 1, 0, 0, connection);
                    var data = await CreateTransactionbyCommmandV2(request.data[i], "T2011", 1, 0, 0, connection); 
                    if (data.errorCode == "00")
                    {
                        cmdResponse.listCommandSuccess += data.listCommandSuccess;
                    }
                    else
                    {
                        CheckErr = false;
                        cmdResponse.listCommandFail += data.listCommandFail;
                        cmdResponse.description += data.description;
                    }
                }
                if (CheckErr)
                    cmdResponse.errorCode = "00";
                else
                    cmdResponse.errorCode = "01";
            }
            catch (Exception ex)
            {
                
                if (ex.StackTrace.Contains("System.DateTime.ParseExact"))
                {
                    cmdResponse.errorCode = "04";
                    cmdResponse.description += " Vui lòng kiểm tra lại thông tin ngày tháng năm ";
                }
                else if (ex.StackTrace.Contains("Microsoft.Data.SqlClient.SqlConnection"))
                {
                    cmdResponse.errorCode = "66";
                    cmdResponse.description += " Kiểm tra lại câu lệnh sql ";
                }
                else
                {
                    cmdResponse.errorCode = "04";
                    cmdResponse.description += ex.StackTrace + "_" + ex.ToString() + " ";
                }    
                    
                return cmdResponse;
            }
            
            return cmdResponse;
        }

        public async Task<CommmandResponseTo> SaveTransactionCodeDOPToDOP1(CommmandRequestTo request)
        {
            bool CheckErr = true;
            using IDbConnection connection = _context.CreateConnection();
            CommmandResponseTo cmdResponse = new();
            try
            {
                for (int i = 0; i < request.data.Count(); i++)
                {
                    //var data = await CreateTransactionbyCommmand(request.data[i], "T2011", 1, 0, 0, connection);
                    var data = await CreateTransactionbyCommmandDOPToDOP1(request.data[i], "T2011", 1, 0, 0, connection);
                    if (data.errorCode == "00")
                    {
                        cmdResponse.listCommandSuccess += data.listCommandSuccess;
                    }
                    else
                    {
                        CheckErr = false;
                        cmdResponse.listCommandFail += data.listCommandFail;
                        cmdResponse.description += data.description;
                    }
                }
                if (CheckErr)
                    cmdResponse.errorCode = "00";
                else
                    cmdResponse.errorCode = "01";
            }
            catch (Exception ex)
            {

                if (ex.StackTrace.Contains("System.DateTime.ParseExact"))
                {
                    cmdResponse.errorCode = "04";
                    cmdResponse.description += " Vui lòng kiểm tra lại thông tin ngày tháng năm ";
                }
                else if (ex.StackTrace.Contains("Microsoft.Data.SqlClient.SqlConnection"))
                {
                    cmdResponse.errorCode = "66";
                    cmdResponse.description += " Kiểm tra lại câu lệnh sql ";
                }
                else
                {
                    cmdResponse.errorCode = "04";
                    cmdResponse.description += ex.StackTrace + "_" + ex.ToString() + " ";
                }

                return cmdResponse;
            }

            return cmdResponse;
        }

        public async Task<CommmandResponseTo> CreateTransactionbyCommmand(TransactionCommandTo cmd, string tab, int version = 1, long batch = 0, long Rparam = 0, IDbConnection connection = null)
        { 
            Models.TransactionResponse.CommmandResponseTo cmdResponse = new(); 
            var conn = connection == null ? _context.CreateConnection() : connection;
            var errStr = "";
            var parameters = new DynamicParameters();
            int rowAffected = 0; 
            try
            {
                var TrSOA = await GetTrNameSOAbyCommandCode(conn, "", cmd.commandCode);
                if (TrSOA == null)
                {
                    cmdResponse.errorCode = "02";
                    cmdResponse.description = "Không tồn tại lệnh tương ứng với CommandCode:" + cmd.commandCode;
                    cmdResponse.listCommandFail = cmd.commandCode;
                    return cmdResponse;
                }

                var TrDOP = await GetTrNameDOPbyCommandCode(conn, "", cmd.commandCode);
                if (TrDOP != null) 
                {
                    cmdResponse.errorCode = "02";
                    cmdResponse.description = "Đã tồn lệnh "+ TrDOP + " tương ứng với commandCode:" + cmd.commandCode;
                    cmdResponse.listCommandFail = cmd.commandCode;
                    return cmdResponse;
                }

                var orderTimeAY = DateTimeParse.ConvertDatetimestringToLong(cmd.transactionDate.ToString());

                //Lấy dữ liệu chung từ cấu hình
                var (commonData, R, B) = await GetCommonDataToCreateTR(conn, batch, Rparam, cmd, "DOP", parameters);
                //Dữ liệu chung các tab
                var IDCustomer = await GetIdMapbyCode(conn, MapCustomter, cmd.customerCode);
                if (IDCustomer != 0)
                    parameters.Add("SrcID", IDCustomer);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("06", cmd.customerCode);
                    cmdResponse.listCommandFail += cmd.commandCode;
                    return cmdResponse;
                }

                var IDWarehouse = await GetIdMapbyCode(conn, MapWareHouse, cmd.itemInfo[0].warehouseCode);
                if (IDWarehouse != 0)
                    parameters.Add("DstID", IDWarehouse);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("07", cmd.itemInfo[0].warehouseCode);
                    cmdResponse.listCommandFail += cmd.commandCode;
                    return cmdResponse;
                }
                var IDProduct = await GetIdMapbyCode(conn, MapProduct, cmd.itemInfo[0].itemCode);
                if (IDProduct != 0)
                    parameters.Add("WaterID", IDProduct);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("08", cmd.itemInfo[0].itemCode);
                    cmdResponse.listCommandFail += cmd.commandCode;
                    return cmdResponse;
                }

                var Unumber = await GetTrNamebyCode(conn, "", cmd.commandCode);
                if (Unumber != "")
                    parameters.Add("Unumber", Unumber);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("16", cmd.commandCode);
                    cmdResponse.listCommandFail += cmd.commandCode;
                    return cmdResponse;
                }

                parameters.Add("Remark", null);

                var IDRefI = await GetRefIbyCode(conn, "", cmd.commandCode);
                if (IDRefI != 0)
                    parameters.Add("RefI", IDRefI);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("09", cmd.commandCode);
                    cmdResponse.listCommandFail += cmd.commandCode;
                    return cmdResponse;
                }
                var IDUnit = await GetIdMapbyCode(conn, MapProductUnit, cmd.itemInfo[0].unitCode);
                if (IDUnit != 0)
                    parameters.Add("i1", IDUnit);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("10", cmd.itemInfo[0].unitCode);
                    cmdResponse.listCommandFail += cmd.commandCode;
                    return cmdResponse;
                }
                
                parameters.Add("i6", cmd.itemInfo[0].quantity);
                parameters.Add("i7", cmd.itemInfo[0].lTT);
                parameters.Add("i8", cmd.itemInfo[0].l15);
                parameters.Add("i9", cmd.itemInfo[0].kG);
                var IDVehicle = await GetIdMapbyCode(conn, MapVehicle, cmd.vehicleCode); 
                if (IDVehicle != 0)
                    parameters.Add("i13", IDVehicle);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("12", cmd.vehicleCode);
                    cmdResponse.listCommandFail += cmd.commandCode;
                    return cmdResponse;
                }

                var RefName = await GetTrNamebyCode(conn, "", cmd.commandCode);
                if (RefName != "")
                    parameters.Add("i15", RefName);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("16", cmd.commandCode);
                    cmdResponse.listCommandFail += cmd.commandCode;
                    return cmdResponse;
                }

                parameters.Add("i20", cmd.itemInfo[0].temPerature);
                parameters.Add("i22", cmd.itemInfo[0].d15);
                parameters.Add("i23", cmd.itemInfo[0].vCF);
                parameters.Add("i24", cmd.itemInfo[0].wCF);
                var IDStockType = await GetIdMapbyCode(conn, MapStockType, cmd.stockType);
                if (IDStockType != 0)
                    parameters.Add("i27", IDStockType);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("13", cmd.stockType);
                    return cmdResponse;
                }

                var IDPos = await GetIdMapbyCode(conn, MapPos, cmd.pos);
                if (IDPos != 0)
                    parameters.Add("Dst", IDPos);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("14", cmd.pos);
                    return cmdResponse;
                }
                var IDUser = await GetIdMapbyCode(conn, MapUser, cmd.userCreate != "" ? cmd.userCreate : "168admin");
                if (IDUser != 0)
                    parameters.Add("SysU", IDUser);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("15", cmd.userCreate);
                    return cmdResponse;
                }
                parameters.Add("U1", cmd.commandCode);
                var PTXH = await GetPTXHbyCode(conn, "", cmd.commandCode);
                if (PTXH != 0)
                    parameters.Add("U3", PTXH);
                else
                {
                    cmdResponse = Utils.Utils.CheckErrorTo("17", cmd.commandCode);
                    cmdResponse.listCommandFail += cmd.commandCode;
                    return cmdResponse;
                }

                parameters.Add("U9", DateTimeParse.ConvertDatetimestringToLong(cmd.dateReceived.ToString()));

                long D = localsetting.SQLgetDate();
                long I;

                string commonSqlInsert = "I,M,TT,B,E,ET,RefI,T,N,D,Dir,SrcClass,SrcID,DstClass,DstID,Dst,WaterClass,Stage,Fin,InvLevel,CoGS,S1,S2,SysS,SysU,SysD,SysV,InvSub,Var1";
                string dataSqlInsert = $"@I,2,@TT,@B,@E,1,@refI,'DOP','{"DOP." + R}',{orderTimeAY},@Dir,@SrcClass,@SrcID,@DstClass,@DstID,@Dst,@WaterClass,@Stage,@Fin,@InvLevel,@CoGS,@S1,@S2,1068,@SysU,{D},{version},1,0";

                string sqlInsertField = "Unumber,Remarks,WaterID,i1,i6,i7,i8,i9,i13,i15,i20,i22,i23,i24,i27,U1,U3,U9";
                string dataSqlInsertField = "@Unumber,@Remark,@WaterID,@i1,@i6,@i7,@i8,@i9,@i13,@i15,@i20,@i22,@i23,@i24,@i27,@U1,@U3,@U9";


                string sqlInsert = $"INSERT INTO " + _schemal + $"." + $"{tab} ({commonSqlInsert},{sqlInsertField}) VALUES ({dataSqlInsert},{dataSqlInsertField});";

                if (commonData != null)
                {
                    I = await GetIndexSEQ(conn, "BOS.TRX_SEQ", "ID");
                    parameters.Add("I", I);

                    //string sqllog = "";
                    //sqllog = sqlInsert;
                    //foreach (var paramName in parameters.ParameterNames)
                    //{
                    //    object paramValue = parameters.Get<dynamic>(paramName);

                    //    // Xử lý nếu giá trị là chuỗi thì bao quanh bằng dấu nháy đơn
                    //    string formattedValue = paramValue != null ? paramValue.ToString() : "NULL";
                    //    if (paramValue is string)
                    //    {
                    //        formattedValue = $"'{formattedValue}'";
                    //    }

                    //    // Thay thế trong câu SQL
                    //    sqllog = sqllog.Replace("@" + paramName, formattedValue);
                    //}
                    //Console.WriteLine(sqllog);

                    // Thực hiện insert dữ liệu bảng T2011
                    rowAffected = await conn.ExecuteAsync(sqlInsert, parameters);

                    // thực hiện update PostSQL, GLPosting, PETWPosting
                    errStr = await EcxecPostSQL(conn, B, commonData[0].PostSQL, commonData[0].GLPosting, commonData[0].PETWPosting);
                }
                if (connection == null) conn.Dispose();
                if (rowAffected != 1 || errStr != "")
                {
                    cmdResponse.errorCode = "01";
                    cmdResponse.description = errStr;
                    cmdResponse.listCommandFail = cmd.commandCode;
                }
                else
                {
                    cmdResponse.errorCode = "00";
                    cmdResponse.listCommandSuccess = cmd.commandCode;
                }
            }
            catch(Exception ex)
            {
                cmdResponse.errorCode = "04";
                cmdResponse.description = ex.StackTrace + ex.ToString();
            }
            
            return cmdResponse;
        }

        public async Task<CommmandResponseTo> CreateTransactionbyCommmandV2(TransactionCommandTo cmd, string tab, int version = 1, long batch = 0, long Rparam = 0, IDbConnection connection = null)
        {
            CommmandResponseTo cmdResponse = new();
            var conn = connection == null ? _context.CreateConnection() : connection;
            var errStr = "";
            var parameters = new DynamicParameters();
            int rowAffected = 0;
            
            try
            {
                if (string.IsNullOrEmpty(cmd.commandCode))
                {
                    cmdResponse.errorCode = "02";
                    cmdResponse.description = "Mã lệnh không được để trống ";
                    cmdResponse.listCommandFail = cmd.commandCode;
                    return cmdResponse;
                }

                var TrDOP = await GetTrNameDOPbyCommandCode(conn, "", cmd.commandCode);
                if (TrDOP != null)
                {
                    cmdResponse.errorCode = "02";
                    cmdResponse.description = "Đã tồn lệnh " + TrDOP + " tương ứng với commandCode:" + cmd.commandCode;
                    cmdResponse.listCommandFail = cmd.commandCode;
                    return cmdResponse;
                }

                long B = batch == 0 ? await GetBIndexSEQ(conn, "BOS.T_Batch", "ID") : batch;
                

                var orderTimeAY = DateTimeParse.ConvertDatetimestringToLong(cmd.transactionDate.ToString());

                //Lấy dữ liệu chung từ cấu hình 
                long I;
                string PostSQL = "";
                string GLPosting = "";
                string PETWPosting = "";
                var PTXSOA = await GetPTXbyCommandCode(conn, "", cmd.commandCode);

                if(PTXSOA == "X08")
                {
                    long R = Rparam == 0 ? await GetRunnerSEQ(conn, "T2011", "DOP1") : Rparam;
                    var (commonData, sqlInsert, cmdResponse2) = await GenSQLInsert(conn, B, R, Rparam, cmd, "T2011", "DOP1", "156", parameters);
                    if (cmdResponse2.errorCode != null)
                    {
                        return cmdResponse2;
                    }

                    if (commonData != null)
                    {
                        I = await GetIndexSEQ(conn, "BOS.TRX_SEQ", "ID");
                        parameters.Add("I", I);
                        // Thực hiện insert dữ liệu bảng T2011 
                        rowAffected = await conn.ExecuteAsync(sqlInsert, parameters);

                        PostSQL = "Update [=TR] Set SrcClass='63211', WaterClass='1561' Where B=[=BATCH] Œ¿";
                        PostSQL += "Update TR Set Fin=Case When ST.C in ('X07') Then 0 Else 1 End,SrcClass=Case When ST.C in ('X01','X02','X03') Then '63211' When ST.C in ('X04') Then (Case When PTOP.C in (11) Then '63211' When PTOP.C in (21) Then '63212' When PTOP.C in (31) Then '63213' Else '63222' End) When ST.C in ('X05') Then '1388' When ST.C in ('X06') Then '33881' Else '63211' End, WaterClass='1561' From [=TR] TR Left Join MD.SaleType ST on ST.I = TR.U3 Left join S168.v_P P on TR.WaterID = P.I left join MD.PGrp1 on PGrp1.I = P.Grp1 Left Join MD.PTop on PGrp1.Parent = PTop.I Where B=[=BATCH]Œ¿";
                        PostSQL += commonData[0].PostSQL;
                        GLPosting = commonData[0].GLPosting;
                        PETWPosting = commonData[0].PETWPosting;
                    }
                    var (commonData2, sqlInsert2, cmdResponse3) = await GenSQLInsert(conn, B, R, Rparam, cmd, "T2011", "DOP1", "157", parameters);
                    if (cmdResponse3.errorCode != null)
                    {
                        return cmdResponse3;
                    }

                    if (commonData2 != null)
                    {
                        I = await GetIndexSEQ(conn, "BOS.TRX_SEQ", "ID");
                        parameters.Add("I", I);
                        // Thực hiện insert dữ liệu bảng T2011 
                        rowAffected = await conn.ExecuteAsync(sqlInsert2, parameters);
                    }
                }
                else
                {
                    long R = Rparam == 0 ? await GetRunnerSEQ(conn, "T2011", "DOP") : Rparam;
                    var (commonData, sqlInsert, cmdResponse2) = await GenSQLInsert(conn, B, R, Rparam, cmd, "T2011", "DOP", "156", parameters);
                    if (cmdResponse2.errorCode != null)
                    {
                        return cmdResponse2;
                    }

                    if (commonData != null)
                    {
                        I = await GetIndexSEQ(conn, "BOS.TRX_SEQ", "ID");
                        parameters.Add("I", I);
                        // Thực hiện insert dữ liệu bảng T2011 
                        rowAffected = await conn.ExecuteAsync(sqlInsert, parameters);

                        PostSQL = "Update [=TR] Set SrcClass='63211', WaterClass='1561' Where B=[=BATCH] Œ¿";
                        PostSQL += "Update TR Set Fin=Case When ST.C in ('X07') Then 0 Else 1 End,SrcClass=Case When ST.C in ('X01','X02','X03') Then '63211' When ST.C in ('X04') Then (Case When PTOP.C in (11) Then '63211' When PTOP.C in (21) Then '63212' When PTOP.C in (31) Then '63213' Else '63222' End) When ST.C in ('X05') Then '1388' When ST.C in ('X06') Then '33881' Else '63211' End, WaterClass='1561' From [=TR] TR Left Join MD.SaleType ST on ST.I = TR.U3 Left join S168.v_P P on TR.WaterID = P.I left join MD.PGrp1 on PGrp1.I = P.Grp1 Left Join MD.PTop on PGrp1.Parent = PTop.I Where B=[=BATCH] Œ¿";
                        PostSQL += commonData[0].PostSQL;
                        GLPosting = commonData[0].GLPosting;
                        PETWPosting = commonData[0].PETWPosting;
                    }
                }    

                // thực hiện update PostSQL, GLPosting, PETWPosting
                errStr = await EcxecPostSQL(conn, B, PostSQL, GLPosting, PETWPosting); 


                if (connection == null) conn.Dispose();
                if (rowAffected != 1 || errStr != "")
                {
                    cmdResponse.errorCode = "01";
                    cmdResponse.description = errStr;
                    cmdResponse.listCommandFail = cmd.commandCode;
                }
                else
                {
                    cmdResponse.errorCode = "00";
                    cmdResponse.listCommandSuccess = cmd.commandCode;
                }
            }
            catch (Exception ex)
            {
                cmdResponse.errorCode = "04";
                cmdResponse.description = ex.StackTrace + ex.ToString();
            }

            return cmdResponse;
        }

        public async Task<CommmandResponseTo> CreateTransactionbyCommmandDOPToDOP1(TransactionCommandTo cmd, string tab, int version = 1, long batch = 0, long Rparam = 0, IDbConnection connection = null)
        {
            CommmandResponseTo cmdResponse = new();
            var conn = connection == null ? _context.CreateConnection() : connection;
            var errStr = "";
            var parameters = new DynamicParameters();
            int rowAffected = 0;

            try
            {
                if (string.IsNullOrEmpty(cmd.commandCode))
                {
                    cmdResponse.errorCode = "02";
                    cmdResponse.description = "Mã lệnh không được để trống ";
                    cmdResponse.listCommandFail = cmd.commandCode;
                    return cmdResponse;
                } 

                long B = batch == 0 ? await GetBIndexSEQ(conn, "BOS.T_Batch", "ID") : batch; 

                //Lấy dữ liệu chung từ cấu hình 
                long I;
                string PostSQL = "";
                string GLPosting = "";
                string PETWPosting = ""; 

                long R = Rparam == 0 ? await GetRunnerSEQ(conn, "T2011", "DOP1") : Rparam;
                var (commonData, sqlInsert, cmdResponse2) = await GenSQLInsert2(conn, B, R, Rparam, cmd, "T2011", "DOP1", "156", parameters);
                if (cmdResponse2.errorCode != null)
                {
                    return cmdResponse2;
                }

                if (commonData != null)
                {
                    I = await GetIndexSEQ(conn, "BOS.TRX_SEQ", "ID");
                    parameters.Add("I", I);
                    // Thực hiện insert dữ liệu bảng T2011 
                    rowAffected = await conn.ExecuteAsync(sqlInsert, parameters);

                    PostSQL = "Update [=TR] Set SrcClass='63211', WaterClass='1561' Where B=[=BATCH] Œ¿";
                    PostSQL += "Update TR Set Fin=Case When ST.C in ('X07') Then 0 Else 1 End,SrcClass=Case When ST.C in ('X01','X02','X03') Then '63211' When ST.C in ('X04') Then (Case When PTOP.C in (11) Then '63211' When PTOP.C in (21) Then '63212' When PTOP.C in (31) Then '63213' Else '63222' End) When ST.C in ('X05') Then '1388' When ST.C in ('X06') Then '33881' Else '63211' End, WaterClass='1561' From [=TR] TR Left Join MD.SaleType ST on ST.I = TR.U3 Left join S168.v_P P on TR.WaterID = P.I left join MD.PGrp1 on PGrp1.I = P.Grp1 Left Join MD.PTop on PGrp1.Parent = PTop.I Where B=[=BATCH]Œ¿";
                    PostSQL += commonData[0].PostSQL;
                    GLPosting = commonData[0].GLPosting;
                    PETWPosting = commonData[0].PETWPosting;
                }
                var (commonData2, sqlInsert2, cmdResponse3) = await GenSQLInsert2(conn, B, R, Rparam, cmd, "T2011", "DOP1", "157", parameters);
                if (cmdResponse3.errorCode != null)
                {
                    return cmdResponse3;
                }

                if (commonData2 != null)
                {
                    I = await GetIndexSEQ(conn, "BOS.TRX_SEQ", "ID");
                    parameters.Add("I", I);
                    // Thực hiện insert dữ liệu bảng T2011 
                    rowAffected = await conn.ExecuteAsync(sqlInsert2, parameters);
                }

                // thực hiện update PostSQL, GLPosting, PETWPosting
                errStr = await EcxecPostSQL(conn, B, PostSQL, GLPosting, PETWPosting);


                if (connection == null) conn.Dispose();
                if (rowAffected != 1 || errStr != "")
                {
                    cmdResponse.errorCode = "01";
                    cmdResponse.description = errStr;
                    cmdResponse.listCommandFail = cmd.commandCode;
                }
                else
                {
                    cmdResponse.errorCode = "00";
                    cmdResponse.listCommandSuccess = cmd.commandCode;
                }
            }
            catch (Exception ex)
            {
                cmdResponse.errorCode = "04";
                cmdResponse.description = ex.StackTrace + ex.ToString();
            }

            return cmdResponse;
        }
        public async Task<string> EcxecUpdateVersionByCommandCode(IDbConnection conn, string tab, int Batch)
        {
            string resule = "";
            int rowAffected;
            try
            {
                string sqlUpdate = $" UPDATE " + _schemal + $"." + $"{tab} Set U2=Coalesce(U2,0)+1 where B= {Batch}";
                rowAffected = await conn.ExecuteAsync(sqlUpdate);
                return resule;
            }
            catch (Exception ex)
            {
                return ex.StackTrace + ex.ToString();
            }
        }

        public async Task<(List<BOSTT>, string, CommmandResponseTo)> GenSQLInsert(IDbConnection conn2, long batch, long R, long Rparam, TransactionCommandTo request, string tab, string T, string element, DynamicParameters parameters)
        {
            List<BOSTT> lstBOSTT = new();
            using IDbConnection conn = _context.CreateConnection();
            CommmandResponseTo cmdResponse = new();
            string strStockType = "";

            string sqlGetData = $@"SELECT TT.I TT, TTE.C E, T2E.Integration ET, T2E.Dir,  
                                                             S.C SrcClass, D.C DstClass, W.C WaterClass,  
                                                             T2E.TStage Stage, T2E.FinVal Fin, T2E.InvLevel,  
                                                             T2E.CoGS, T2E.S1, T2E.S2, T2E.GLWATERFILTER,
                                                             TT.PostSQL, TT.GLPosting, TT.PETWPosting
                                                             FROM BOS.TT  
                                                             INNER JOIN BOS.T2E ON TT.I=T2E.T  
                                                             INNER JOIN BOS.TTE ON TTE.I=T2E.E  
                                                             INNER JOIN BOS.AY S ON S.I=Coalesce(T2E.SrcClass2, TTE.SrcClass)  
                                                             INNER JOIN BOS.AY D ON D.I=TTE.DstClass  
                                                             INNER JOIN BOS.AY W ON W.I=Coalesce(T2E.WaterClass2, TTE.WaterClass)  
                                                             WHERE TT.C= '{T}' And T2E.Integration In (1,3,2) And TTE.C={element} 
                                                             Order By T2E.C";
            var commonData = await conn.QueryFirstOrDefaultAsync<BOSTT>(sqlGetData);

            
            long D = localsetting.SQLgetDate();

            parameters.Add("B", batch);
            parameters.Add("TT", commonData?.TT);
            parameters.Add("E", commonData?.E);
            parameters.Add("ET", commonData?.ET);
            parameters.Add("Dir", commonData?.Dir);
            parameters.Add("SrcClass", commonData?.SrcClass);
            parameters.Add("DstClass", commonData?.DstClass);
            parameters.Add("WaterClass", commonData?.WaterClass);
            parameters.Add("Fin", commonData?.Fin);
            parameters.Add("InvLevel", commonData?.InvLevel);
            parameters.Add("S1", commonData?.S1);
            parameters.Add("S2", commonData?.S2);
            parameters.Add("Stage", commonData?.Stage);
            parameters.Add("CoGS", commonData?.CoGS);

            parameters.Add("sysD", D);
            lstBOSTT.Add(commonData);
            
            var orderTimeAY = DateTimeParse.ConvertDatetimestringToLong(request.transactionDate.ToString());

            var TrDOX = await GetDOXbyCommandCode(conn, "", request.commandCode);
            if (TrDOX != null)
            {
                strStockType = "(Select I From MD.StockType where C='106')";
            }
            else
            {
                strStockType = "TR.U4";
            }

            request.vehicleCode = request.vehicleCode.Replace(" ", "").Replace("-", "").Replace("_", "").Replace(".", "");
            var IDVehicle = await GetIdMapbyCode(conn, MapVehicle, request.vehicleCode);
            if (IDVehicle != 0)
                parameters.Add("i13", IDVehicle);
            else
            {
                cmdResponse = Utils.Utils.CheckErrorTo("12", request.vehicleCode);
                cmdResponse.listCommandFail += request.commandCode; 
            }

            parameters.Add("i6", request.itemInfo[0].quantity);
            parameters.Add("i7", request.itemInfo[0].lTT);
            parameters.Add("i8", request.itemInfo[0].l15);
            parameters.Add("i9", request.itemInfo[0].kG);
            parameters.Add("i20", request.itemInfo[0].temPerature);
            parameters.Add("i22", request.itemInfo[0].d15);
            parameters.Add("i23", request.itemInfo[0].vCF);
            parameters.Add("i24", request.itemInfo[0].wCF);


            var IDUser = await GetIdMapbyCode(conn, MapUser, "168admin");
            if (IDUser != 0)
                parameters.Add("SysU", IDUser);
            else
            {
                cmdResponse = Utils.Utils.CheckErrorTo("15", request.userCreate); 
            }

            if (request.dateReceived.ToString() != "")
                parameters.Add("U9", DateTimeParse.ConvertDatetimestringToLong(request.dateReceived.ToString()));
            else
                parameters.Add("U9", DateTimeParse.ConvertDatetimestringToLong(request.transactionDate.ToString()));


            if (R == 0)
                R = 1;

            string commonSqlInsert = "I,M,TT,B,E,ET,RefI,T,N,D,Dir,SrcClass,SrcID,DstClass,DstID,Dst,WaterClass,Stage,Fin,InvLevel,CoGS,S1,S2,SysS,SysU,SysD,SysV,InvSub,Var1"; 

            string sqlInsertField = "Unumber,Remarks,WaterID,i1,i6,i7,i8,i9,i13,i15,i20,i22,i23,i24,i27,U1,U3,U9";

            string dataSqlSelectbyCommand = $" Select @I,2,@TT,@B,{element},1,TR.I,'{T}','{T}.{R}',{orderTimeAY},@Dir,@SrcClass,SrcID,DstClass,DstID,Dst,@WaterClass,@Stage,@Fin,@InvLevel,@CoGS,@S1,2,NULL,@SysU,{D},1,InvSub,0,TR.N,N'ghi chú',WaterID,i1,i6,@i7,@i8,@i9,@i13,TR.N,@i20,@i22,@i23,@i24,{strStockType},TR.B,TR.U3,@U9 From " + _schemal + $"." + $"{tab} TR Where B= " + request.commandCode;

            string sqlInsert = $"INSERT INTO " + _schemal + $"." + $"{tab} ({commonSqlInsert},{sqlInsertField}) {dataSqlSelectbyCommand};";


            return (lstBOSTT.ToList(), sqlInsert, cmdResponse);
        }

        public async Task<(List<BOSTT>, string, CommmandResponseTo)> GenSQLInsert2(IDbConnection conn2, long batch, long R, long Rparam, TransactionCommandTo request, string tab, string T, string element, DynamicParameters parameters)
        {
            List<BOSTT> lstBOSTT = new();
            using IDbConnection conn = _context.CreateConnection();
            CommmandResponseTo cmdResponse = new();
            string strStockType = "";

            string sqlGetData = $@"SELECT TT.I TT, TTE.C E, T2E.Integration ET, T2E.Dir,  
                                                             S.C SrcClass, D.C DstClass, W.C WaterClass,  
                                                             T2E.TStage Stage, T2E.FinVal Fin, T2E.InvLevel,  
                                                             T2E.CoGS, T2E.S1, T2E.S2, T2E.GLWATERFILTER,
                                                             TT.PostSQL, TT.GLPosting, TT.PETWPosting
                                                             FROM BOS.TT  
                                                             INNER JOIN BOS.T2E ON TT.I=T2E.T  
                                                             INNER JOIN BOS.TTE ON TTE.I=T2E.E  
                                                             INNER JOIN BOS.AY S ON S.I=Coalesce(T2E.SrcClass2, TTE.SrcClass)  
                                                             INNER JOIN BOS.AY D ON D.I=TTE.DstClass  
                                                             INNER JOIN BOS.AY W ON W.I=Coalesce(T2E.WaterClass2, TTE.WaterClass)  
                                                             WHERE TT.C= '{T}' And T2E.Integration In (1,3,2) And TTE.C={element} 
                                                             Order By T2E.C";
            var commonData = await conn.QueryFirstOrDefaultAsync<BOSTT>(sqlGetData);

            parameters.Add("B", batch);
            parameters.Add("TT", commonData?.TT);
            parameters.Add("E", commonData?.E);
            parameters.Add("ET", commonData?.ET);
            parameters.Add("Dir", commonData?.Dir);
            parameters.Add("SrcClass", commonData?.SrcClass);
            parameters.Add("DstClass", commonData?.DstClass);
            parameters.Add("WaterClass", commonData?.WaterClass);
            parameters.Add("Fin", commonData?.Fin);
            parameters.Add("InvLevel", commonData?.InvLevel);
            parameters.Add("S1", commonData?.S1);
            parameters.Add("S2", commonData?.S2);
            parameters.Add("Stage", commonData?.Stage);
            parameters.Add("CoGS", commonData?.CoGS);
            lstBOSTT.Add(commonData); 

            string commonSqlInsert = "I,M,TT,B,E,ET,RefI,T,N,D,Dir,SrcClass,SrcID,DstClass,DstID,Dst,WaterClass,Stage,Fin,InvLevel,CoGS,S1,S2,SysS,SysU,SysD,SysV,InvSub,Var1";

            string sqlInsertField = "Unumber,Remarks,WaterID,i1,i6,i7,i8,i9,i13,i15,i20,i22,i23,i24,i27,U1,U3,U9";

            string dataSqlSelectbyCommand = $" Select @I,2,@TT,@B,{element},1,RefI,'{T}','{T}.{R}',D,@Dir,@SrcClass,SrcID,DstClass,DstID,Dst,@WaterClass,@Stage,@Fin,@InvLevel,@CoGS,S1,S2,SysS,SysU,SysD,SysV,InvSub,Var1,Unumber,Remarks,WaterID,i1,i6,i7,i8,i9,i13,i15,i20,i22,i23,i24,i27,U1,U3,U9 From " + _schemal + $"." + $"{tab} TR Where B= " + request.commandCode;

            string sqlInsert = $"INSERT INTO " + _schemal + $"." + $"{tab} ({commonSqlInsert},{sqlInsertField}) {dataSqlSelectbyCommand};";


            return (lstBOSTT.ToList(), sqlInsert, cmdResponse);
        }

        public async Task<(List<BOSTT>, long, long)> GetCommonDataToCreateTR(IDbConnection conn2, long batch, long Rparam, dynamic request, string T, DynamicParameters parameters)
        {
            List<BOSTT> lstBOSTT = new();
            using IDbConnection conn = _context.CreateConnection();
            string sqlGetData = $@"SELECT TT.I TT, TTE.C E, T2E.Integration ET, T2E.Dir,  
                                                             S.C SrcClass, D.C DstClass, W.C WaterClass,  
                                                             T2E.TStage Stage, T2E.FinVal Fin, T2E.InvLevel,  
                                                             T2E.CoGS, T2E.S1, T2E.S2, T2E.GLWATERFILTER,
                                                             TT.PostSQL, TT.GLPosting, TT.PETWPosting
                                                             FROM BOS.TT  
                                                             INNER JOIN BOS.T2E ON TT.I=T2E.T  
                                                             INNER JOIN BOS.TTE ON TTE.I=T2E.E  
                                                             INNER JOIN BOS.AY S ON S.I=Coalesce(T2E.SrcClass2, TTE.SrcClass)  
                                                             INNER JOIN BOS.AY D ON D.I=TTE.DstClass  
                                                             INNER JOIN BOS.AY W ON W.I=Coalesce(T2E.WaterClass2, TTE.WaterClass)  
                                                             WHERE TT.C= '{T}' And T2E.Integration In (1,3,2) And TTE.C=156 
                                                             Order By T2E.C";
            var commonData = await conn.QueryFirstOrDefaultAsync<BOSTT>(sqlGetData);

            long B = batch == 0 ? await GetBIndexSEQ(conn, "BOS.T_Batch", "ID") : batch;
            long R = Rparam == 0 ? await GetRunnerSEQ(conn, "T2011", "DOP") : Rparam;
            long D = localsetting.SQLgetDate(); 

            parameters.Add("B", B);
            parameters.Add("TT", commonData?.TT);
            parameters.Add("E", commonData?.E);
            parameters.Add("ET", commonData?.ET);
            parameters.Add("Dir", commonData?.Dir);
            parameters.Add("SrcClass", commonData?.SrcClass);
            parameters.Add("DstClass", commonData?.DstClass);
            parameters.Add("WaterClass", commonData?.WaterClass);
            parameters.Add("Fin", commonData?.Fin);
            parameters.Add("InvLevel", commonData?.InvLevel);
            parameters.Add("S1", commonData?.S1);
            parameters.Add("S2", commonData?.S2);
            parameters.Add("Stage", commonData?.Stage);
            parameters.Add("CoGS", commonData?.CoGS);

            parameters.Add("sysD", D);
            lstBOSTT.Add(commonData);
            return (lstBOSTT.ToList(), R, B);
        }

        public async Task<long> GetIndexSEQ(IDbConnection conn, string tab, string colIndex)
        {
            string insertQuery = $"INSERT INTO {tab} (iDate) OUTPUT INSERTED.{colIndex} VALUES (@DateTime)";
            int result = await conn.ExecuteScalarAsync<int>(insertQuery, new { DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") });
            return result;
        }

        public async Task<long> GetBIndexSEQ(IDbConnection conn, string tab, string colIndex)
        {
            string insertQuery = $"INSERT INTO {tab} (D) OUTPUT INSERTED.{colIndex} VALUES (@DateTime)";
            int result = await conn.ExecuteScalarAsync<int>(insertQuery, new { DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") });
            return result;
        }

        public async Task<int> GetRunnerSEQ(IDbConnection conn, string tab, string TParam)
        {
            string sql = $"select top 1 CAST(SUBSTRING(N, CHARINDEX('.', N) + 1, LEN(N)) AS INT) + 1 AS Result from " + _schemal + $"." + $"{tab} where T = '{TParam}' order by i desc";
            int runner = await conn.QueryFirstOrDefaultAsync<int>(sql);
            return runner;
        }
        public async Task<int> GetIdMapbyCode(IDbConnection conn, string tab, string Code)
        {
            string sql = $"select top 1 I  from {tab} where C = '{Code}' ";
            int ID = await conn.QueryFirstOrDefaultAsync<int>(sql);
            return ID;
        }

        public async Task<int> GetRefIbyCode(IDbConnection conn, string tab, string commandCode)
        {
            string sql = $"select top 1 I from " + _schemal + $"." + $"T2011 TR WITH (NOLOCK) where T='SOA' and B = {commandCode} ";
            int ID = await conn.QueryFirstOrDefaultAsync<int>(sql);
            return ID;
        } 
        public async Task<string> GetTrNamebyCode(IDbConnection conn, string tab, string commandCode)
        {
            string sql = $"select top 1 N from " + _schemal + $"." + $"T2011 TR WITH (NOLOCK) where T='SOA' and B = {commandCode} ";
            string TrName = await conn.QueryFirstOrDefaultAsync<string>(sql);
            return TrName;
        }

public async Task<string> GetDOXbyCommandCode(IDbConnection conn, string tab, string commandCode)
{
    // Use a parameterized query with placeholders for the dynamic values.
    string sql = $@"
        SELECT TOP 1 N 
        FROM {_schemal}.T2011 TR WITH (NOLOCK) 
        WHERE T='DOX' AND RefI IN (
            SELECT RefI 
            FROM {_schemal}.T2011 TR WITH (NOLOCK) 
            WHERE T='SOA' AND B = @commandCode AND Coalesce(RefI,0) <> 0
        )";

    // Create an anonymous object to hold the parameters.
    var parameters = new { commandCode = commandCode };

    // Dapper will safely replace the @commandCode placeholder with the actual value.
    string TrName = await conn.QueryFirstOrDefaultAsync<string>(sql, parameters);
    
    return TrName;
}

        public async Task<string> GetTrNameSOAbyCommandCode(IDbConnection conn, string tab, string commandCode)
        {
            string sql = $"select top 1 N from " + _schemal + $"." + $"T2011 TR WITH (NOLOCK) where T='SOA' and B = {commandCode} ";
            string TrNameDOP = await conn.QueryFirstOrDefaultAsync<string>(sql);
            return TrNameDOP;
        }

        public async Task<string> GetTrNameDOPbyCommandCode(IDbConnection conn, string tab, string commandCode)
        {
            string sql = $"select top 1 N from " + _schemal + $"." + $"T2011 TR WITH (NOLOCK) where T In ('DOP','DOP1') and U1 = @commandCode";

            // Sử dụng QueryFirstOrDefaultAsync với tham số
            string TrNameDOP = await conn.QueryFirstOrDefaultAsync<string>(sql, new { commandCode = commandCode });

            return TrNameDOP;
        }
        public async Task<string> GetPTXbyCommandCode(IDbConnection conn, string tab, string commandCode)
        {
            string sql = $"select top 1 SaleType.C from " + _schemal + $"." + $"T2011 TR WITH (NOLOCK) Inner Join MD.SaleType On SaleType.I=TR.U3 where T='SOA' and U1 = @commandCode";

            var parameters = new { commandCode = commandCode };

            string TrNameDOP = await conn.QueryFirstOrDefaultAsync<string>(sql, parameters);

            return TrNameDOP;
        }

        public async Task<int> GetPTXHbyCode(IDbConnection conn, string tab, string commandCode)
        {
            string sql = $"select top 1 U3 from " + _schemal + $"." + $"T2011 TR WITH (NOLOCK) where T='SOA' and B = {commandCode} ";
            int ID = await conn.QueryFirstOrDefaultAsync<int>(sql);
            return ID;
        }

        public async Task<string> EcxecPostSQL(IDbConnection conn, long batch, string PostSQL, string GLPosting, string PetwPosting)
        {
            string resule = "";
            int rowAffected;
            try
            {
                string[] strPostSQL = PostSQL.Split("Œ¿");
                for (int i = 0; i < strPostSQL.Count(); i++)
                {
                    if (strPostSQL[i] != "")
                    {
                        strPostSQL[i] = strPostSQL[i].Replace("[=TR]", _schemal + ".T2011");
                        strPostSQL[i] = strPostSQL[i].Replace("[=BATCH]", batch.ToString());
                        rowAffected = await conn.ExecuteAsync(strPostSQL[i]);
                    }
                }

                string SQLGLPosting = $" Insert Into " + _schemal + "." + "GL2011 Select * from " + _schemal + $"." + GLPosting + "2011 where B= " + batch;
                rowAffected = await conn.ExecuteAsync(SQLGLPosting);

                string SQLPetwPosting = $" Insert Into " + _schemal + "." + "PETW (D,PETI,T,RefTab,B,DocType,ReceiptNumber,PR,billUoM,billQty,VoC,StockType,Variant1,WH,WHCoGS,Branch,LTT,KG,L15,Dir,InvLevel,FIN,Co1,CoGS,Amt,DispAmt,UNumber,GiaTruocThue,MucVat,GiaSauThue,ChietKhau,GiaBanLe,MucBVMT,TienBVMT,NgayHD,MauHD,SeriHD,SoHD,IUoM,IQty,LoaiHaoHut,HangDiDuong,PTNX,LoaiGiaoNhan,LoHang,RefI) Select * from " + _schemal + $"." + PetwPosting + "2011 where B= " + batch;
                rowAffected = await conn.ExecuteAsync(SQLPetwPosting);
                return resule;
            }
            catch (Exception ex)
            {
                return ex.StackTrace + ex.ToString();
            } 
        }
    }
}
