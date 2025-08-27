namespace ServiceAPI.Models.TransactionResponse
{
    public class CommmandRequestTo
    {
        public string? transactionID { get; set; }
        public string? transactionDate { get; set; } 
        public string? description { get; set; }
        public List<TransactionCommandTo> data { get; set; } = new();
    }

    public class CommmandResponseTo
    {
        public string? errorCode { get; set; } 
        public string? description { get; set; }
        public string? listCommandSuccess { get; set; }
        public string? listCommandFail { get; set; }
        
    }

    public class TransactionCommandDataTo
    {
        public string? command { get; set; }
        public int transactionDate { get; set; }
        public string? customerCode { get; set; }
        public string? customerName { get; set; }
        public int dateStart { get; set; }
        public int dateStop { get; set; }
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
    }

    public class TransactionCommandTo
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
        public string? pos { get; set; }
        public string? userCreate { get; set; }
        public List<ItemInfoTo> itemInfo { get; set; } = new();
    }
    public class ItemInfoTo
    {
        public int lineNumber { get; set; }
        public string? itemCode { get; set; }
        public string? itemName { get; set; }
        public string? unitCode { get; set; }
        public string? unitName { get; set; }
        public decimal quantity { get; set; }
        public decimal temPerature { get; set; }
        public decimal d15 { get; set; }
        public decimal vCF { get; set; }
        public decimal wCF { get; set; }
        public decimal lTT { get; set; }
        public decimal l15 { get; set; }
        public decimal kG { get; set; } 
        public string? warehouseCode { get; set; }
        public string? warehouseName { get; set; }  
    }

    public class BOSTT
    {
        public long TT { get; set; }
        public long E { get; set; }
        public int ET { get; set; }
        public int Dir { get; set; }
        public long SrcClass { get; set; }
        public long DstClass { get; set; }
        public long WaterClass { get; set; }
        public int Stage { get; set; }
        public int Fin { get; set; }
        public int InvLevel { get; set; }
        public int CoGS { get; set; }
        public int S1 { get; set; }
        public int S2 { get; set; }
        public string PostSQL { get; set; }
        public string GLPosting { get; set; }
        public string PETWPosting { get; set; }

    }
}
