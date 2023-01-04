namespace Quiz_management_system.Models
{
    public class category_chose
    {
        public string biggner_is_pass { get; set; }
        public string intermidiate_is_pass { get; set; }

        public string difficult_is_pass { get; set; }

        public string biggner_course1 { get; set; }
        public string biggner_course2 { get; set; }
        public string intermidiate_course1 { get; set; }
        public string intermidiate_course2 { get; set; }
        public string difficult_course1 { get; set; }
        public string difficult_course2 { get; set; }

        public DateTime time { get; set; }
    }
}
