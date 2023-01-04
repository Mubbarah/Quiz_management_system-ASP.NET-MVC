namespace Quiz_management_system.Models
{
    public class AddSubjectmodel
    {
        [ForeginKey("category")]
        public int category_id { get; set; }
        public string course_name { get; set; }
        public string category_name { get; set; }
    }
}
