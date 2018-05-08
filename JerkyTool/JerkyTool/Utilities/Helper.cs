using JerkyTool;

namespace JerkyTool.Utilities
{
  public static  class Helper
    {

        public static string Encrypt(string plainText)
        {

            string passPhrase = "Pas5pr@se";        // can be any string
            string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes

            // Before encrypting data, we will append plain text to a random
            // salt value, which will be between 4 and 8 bytes long (implicitly
            // used defaults).
            RijndaelEnhanced rijndaelKey =
                new RijndaelEnhanced(passPhrase, initVector);

            return rijndaelKey.Encrypt(plainText);

        }

        public static string Decrypt(string inputText)
        {

            string passPhrase = "Pas5pr@se";        // can be any string
            string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes

            // Before encrypting data, we will append plain text to a random
            // salt value, which will be between 4 and 8 bytes long (implicitly
            // used defaults).
            RijndaelEnhanced rijndaelKey =
                new RijndaelEnhanced(passPhrase, initVector);

            return rijndaelKey.Decrypt(inputText);

        }
    }
}
