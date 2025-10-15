using System.Text.RegularExpressions;

namespace AuthenticationService.Validators
{
    public class MlbValidator
    {
        public static bool IsValidJMBG(string jmbg)
        {

            if (string.IsNullOrWhiteSpace(jmbg) || jmbg.Length != 13)
                return false;


            if (!Regex.IsMatch(jmbg, @"^\d{13}$"))
                return false;



            string dayStr = jmbg.Substring(0, 2);
            string monthStr = jmbg.Substring(2, 2);
            string yearStr = jmbg.Substring(4, 3);

            int day, month, year;

            if (!int.TryParse(dayStr, out day) ||
                !int.TryParse(monthStr, out month) ||
                !int.TryParse(yearStr, out year))
                return false;


            if (year >= 900)
                year = 1000 + year;
            else
                year = 2000 + year;

            try
            {
                DateTime dt = new DateTime(year, month, day);
            }
            catch
            {
                return false;
            }


            return true;

        }
    }
}
