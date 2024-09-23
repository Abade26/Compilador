using System.Text.RegularExpressions;

namespace Compilador {

    public class Lexer
    {
        private string input;
        private int position;
        private char? currentChar;

        public Lexer(string input)
        {
            this.input = input;
            this.position = 0;
            this.currentChar = input.Length > 0 ? (char?)input[position] : null;
        }

        // Avança para o próximo caractere
        private void NextChar()
        {
            position++;
            currentChar = position < input.Length ? (char?)input[position] : null;
        }

        // Olha o próximo caractere sem avançar
        private char? PeekChar()
        {
            return position + 1 < input.Length ? (char?)input[position + 1] : null;
        }

        // Ignora espaços em branco
        private void SkipWhitespace()
        {
            while (currentChar.HasValue && char.IsWhiteSpace(currentChar.Value))
            {
                NextChar();
            }
        }

        // Ignora comentários
        private void SkipComment()
        {
            if (currentChar == '/')
            {
                NextChar();
                if (currentChar == '/') // Comentário de uma linha
                {
                    while (currentChar.HasValue && currentChar != '\n')
                    {
                        NextChar();
                    }
                }
                else if (currentChar == '*') // Comentário de múltiplas linhas
                {
                    NextChar();
                    while (currentChar.HasValue)
                    {
                        if (currentChar == '*' && PeekChar() == '/')
                        {
                            NextChar(); // Consome o '*'
                            NextChar(); // Consome o '/'
                            break;
                        }
                        NextChar();
                    }
                }
            }
        }

        // Reconhece um identificador ou palavra-chave
        private Token ReadIdentifier()
        {
            string result = string.Empty;
            while (currentChar.HasValue && Regex.IsMatch(currentChar.ToString(), "[a-zA-Z_]"))
            {
                result += currentChar;
                NextChar();
            }
            return new Token("IDENTIFIER", result);
        }

        // Reconhece números
        private Token ReadNumber()
        {
            string result = string.Empty;
            while (currentChar.HasValue && char.IsDigit(currentChar.Value))
            {
                result += currentChar;
                NextChar();
            }
            return new Token("NUMBER", int.Parse(result));
        }

        // Reconhece strings
        private Token ReadString()
        {
            char quote = currentChar.Value; // Guarda o tipo de aspas
            NextChar(); // Avança para o próximo caractere
            string result = string.Empty;

            while (currentChar.HasValue && currentChar != quote)
            {
                result += currentChar;
                NextChar();
            }

            NextChar(); // Avança para o caractere após as aspas
            return new Token("STRING", result);
        }

        // Reconhece operadores
        private Token ReadOperator()
        {
            var operators = new List<string> { "+", "-", "*", "/", "=", "==", "!=", "<", ">", "<=", ">=" };
            foreach (var op in operators)
            {
                if (input.Substring(position).StartsWith(op))
                {
                    position += op.Length;
                    currentChar = position < input.Length ? (char?)input[position] : null;
                    return new Token("OPERATOR", op);
                }
            }
            return null;
        }

        // Tokeniza o input
        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (currentChar.HasValue)
            {
                SkipWhitespace();

                if (currentChar == '/')
                {
                    SkipComment();
                    continue;
                }

                if (currentChar == '\'' || currentChar == '\"')
                {
                    tokens.Add(ReadString());
                    continue;
                }

                if (Regex.IsMatch(currentChar.ToString(), "[a-zA-Z_]"))
                {
                    tokens.Add(ReadIdentifier());
                    continue;
                }

                if (char.IsDigit(currentChar.Value))
                {
                    tokens.Add(ReadNumber());
                    continue;
                }

                var operatorToken = ReadOperator();
                if (operatorToken != null)
                {
                    tokens.Add(operatorToken);
                    continue;
                }

                if ("(){};,[]".Contains(currentChar.Value))
                {
                    tokens.Add(new Token("DELIMITER", currentChar.ToString()));
                    NextChar();
                    continue;
                }

                // Se chegar aqui, é um caractere desconhecido
                tokens.Add(new Token("UNKNOWN", currentChar.ToString()));
                NextChar();
            }

            return tokens;
        }
    }
}