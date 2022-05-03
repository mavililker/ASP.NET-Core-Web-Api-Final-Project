namespace EventProjectFinal.DTOs
{
    public class EventDTO
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string CategoryName { get; set; }
        public string CityName { get; set; }

        public int CategoryId { get; set; }
        public int CityId { get; set; }
        public string IsCanceled { get; set; }

    }
}
