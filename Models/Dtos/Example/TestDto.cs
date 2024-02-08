namespace Models.Dtos.Example
{
    public class TestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Extra1 { get; set; } = null!;
        public string Extra2 { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
