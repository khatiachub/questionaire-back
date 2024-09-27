
namespace questionaire_back.models
{
    public class GetQuestionModel
    {
        public int? Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Required { get; set; }
    }
}
