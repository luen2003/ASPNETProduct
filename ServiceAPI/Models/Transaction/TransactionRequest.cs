namespace ServiceAPI.Models.TransactionRequest
{
    public class CommmandRequestGoFirt
    {
        public int commandCode { get; set; }
    }
    public class CommmandRequestGo
    {
        public int requestID { get; set; }
        public string? fromDate { get; set; }
        public string? toDate { get; set; }
        public List<int>? listCommandCode { get; set; }
        public List<string>? listWarehouse { get; set; } 
    }

    public class CommmandResponseGo
    {
        public string? errorCode { get; set; }
        public string? description { get; set; }
        public List<TransactionCommandGo> data { get; set; } = new();
    }

    public class TransactionCommandDataGo
    {
        public string? transactionCode { get; set; }
        public string? transactionName { get; set; }
        public string? command { get; set; }
        public int transactionDate { get; set; }
        public string? customerCode { get; set; }
        public string? customerName { get; set; }
        public int dateStart { get; set; }
        public int dateStop { get; set; } 
        public int dateReceived { get; set; }
        public string? stockType { get; set; }
        public string? saleType { get; set; }
        public string? vehicleCode { get; set; } 
        public string? itemCode { get; set; }
        public string? itemName { get; set; }
        public string? unitCode { get; set; }
        public string? unitName { get; set; }
        public decimal quantity { get; set; }
        public string? warehouseCode { get; set; }
        public string? warehouseName { get; set; } 
        public string? isType { get; set; }
    }

    public class TransactionCommandGo
    {
        public string? transactionCode { get; set; }
        public string? transactionName { get; set; } 
        public string? commandCode { get; set; } 
        public string? transactionDate { get; set; }
        public string? customerCode { get; set; }
        public string? customerName { get; set; }
        public string? dateStart { get; set; }
        public string? dateStop { get; set; } 
        public string? dateReceived { get; set; }
        public string? stockType { get; set; } 
        public string? saleType { get; set; }
        public string? vehicleCode { get; set; } 
        public List<ItemInfoGo> itemInfo { get; set; } = new();
    }
    public class ItemInfoGo
    {
        public int lineNumber { get; set; }
        public string? itemCode { get; set; }
        public string? itemName { get; set; }
        public string? unitCode { get; set; }
        public string? unitName { get; set; }
        public decimal quantity { get; set; }
        public string? warehouseCode { get; set; }
        public string? warehouseName { get; set; }
        public string? isType { get; set; }
    }

}
