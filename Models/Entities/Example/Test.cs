namespace Models.Entities.Example
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Extra1 { get; set; } = null!;
        public string Extra2 { get; set; } = null!;
        public DateOnly CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
