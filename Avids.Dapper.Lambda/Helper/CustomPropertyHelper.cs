using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Avids.Dapper.Lambda.Helper
{
    public class CustomPropertyHelper
    {
        public static string ConvertPascalCaseToSnakeCase(string pascalCase)
        {
            string keyword = "";
            for (int i = 0; i < pascalCase.Length; i++)
            {
                if (char.IsUpper(pascalCase[i]) && i != 0)
                    keyword += $"_{pascalCase[i].ToString().ToLower()}";
                else keyword += pascalCase[i].ToString().ToLower();
            }
            return keyword;
        }

        public static string ConvertSnakeCaseToPascalCase(string snakeCase)
        {
            string keyword = "";
            for (int i = 0; i < snakeCase.Length; i++)
            {
                if (i == 0 || snakeCase[i] == '_')
                {
                    if (snakeCase[i] == '_') i++;
                    keyword += snakeCase[i].ToString().ToUpper();
                }
                else keyword += snakeCase[i].ToString().ToLower();
            }
            return keyword;
        }
    }
}
