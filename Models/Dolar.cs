namespace Calcu.Models
{
    public class Dolar
    {
        public string Name { get; set; } = "Dolar";
        // public string Description { get; set; } = "";
        public float Buy { get; set; } = 0;
        public float Sell { get; set; } = 0;
        public int Timestamp { get; set; }
        public float Variation { get; set; }
        public float Spread { get; set; }
    }
}
