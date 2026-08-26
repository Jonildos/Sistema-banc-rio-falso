using System.Text.RegularExpressions;

namespace Sistema_banc_rio_falso.Utils
{
    public static class ValidadorCpf
    {
        public static bool Validar(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove tudo que não for dígito (pontos, traços, letras como "banana")
            cpf = Regex.Replace(cpf, @"[^\d]", "");

            // CPF precisa ter exatamente 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Barra sequências de dígitos iguais inválidas (ex: 00000000000, 11111111111, etc.)
            bool todosIguaux = true;
            for (int i = 1; i < 11; i++)
            {
                if (cpf[i] != cpf[0])
                {
                    todosIguaux = false;
                    break;
                }
            }
            if (todosIguaux)
                return false;

            // Validação matemática do 1º dígito verificador
            int soma = 0;
            int peso = 10;
            for (int i = 0; i < 9; i++)
            {
                soma += (cpf[i] - '0') * peso;
                peso--;
            }

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            if (digito1 != (cpf[9] - '0'))
                return false;

            // Validação matemática do 2º dígito verificador
            soma = 0;
            peso = 11;
            for (int i = 0; i < 10; i++)
            {
                soma += (cpf[i] - '0') * peso;
                peso--;
            }

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            if (digito2 != (cpf[10] - '0'))
                return false;

            return true;
        }
    }
}