namespace eastwest.Utils
{
    public class VerifyCode
    {
        public VerifyCode()
        {
        }

        public string random(int length)
        {
            var result = "";
            var characters = "0123456789";
            var characterslength = characters.Length;
            for (var i = 0; i < length; i++)
            {
                Random random = new Random();
                int index = random.Next(characterslength);
                result += characters[i];
            }

            return result;
        }
    }
}