namespace questionaire_back.models
{
    public class GetAnsModel
    {
        public int? Id { get; set; }
        public string FullName { get; set; }
        public int Question_Id { get; set; }
        public string? Question { get; set; }
        public string Answer { get; set; }
    }
}
