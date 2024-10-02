namespace ELearningAPI.Helpers
{
    public class RandomCode
    {
        public static string GetRandomUserID()
        {
            var year = DateTime.Now.Year.ToString().Substring(2, 2);
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var second = DateTime.Now.Second;
            var random = new Random().Next(1, 100);

            var code = $"user{year}{month}{day}{hour}{minute}{second}{random}";
            return code;
        }
        public static string GetRandomCourseID()
        {
            var year = DateTime.Now.Year.ToString().Substring(2, 2);
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var second = DateTime.Now.Second;
            var random = new Random().Next(1, 100);

            var code = $"course{year}{month}{day}{hour}{minute}{second}{random}";
            return code;
        }
        public static string GetRandomExamID()
        {
            var year = DateTime.Now.Year.ToString().Substring(2, 2);
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var second = DateTime.Now.Second;
            var random = new Random().Next(1, 100);

            var code = $"exam{year}{month}{day}{hour}{minute}{second}{random}";
            return code;
        }
        public static string GetRandomSubmissionID()
        {
            var year = DateTime.Now.Year.ToString().Substring(2, 2);
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var second = DateTime.Now.Second;
            var random = new Random().Next(1, 100);

            var code = $"submission{year}{month}{day}{hour}{minute}{second}{random}";
            return code;
        }   
    }
}
