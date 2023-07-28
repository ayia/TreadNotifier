namespace NotiificationAPIchartdepth.Models
{
    public class SignalData
    {
        public string Instrument { get; set; }
        public string Action { get; set; }
        public string Type { get; set; }
        public DateTime OpenTime { get; set; }
        public string OpenPrice { get; set; }
       
        public string TakeProfit1 { get; set; }
        public string TakeProfit2 { get; set; }
        public string StopLoss { get; set; }
       

        // Additional methods or constructors can be added as needed.
    }
}
