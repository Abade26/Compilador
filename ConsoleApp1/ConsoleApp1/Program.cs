namespace Compilador
{
    public class Program
{
    public static void Main()
    {
        string code = @"
        int main() {
            int a = 5;
            if (a == 5) {
                a = a + 1;
            }
        }";

        Lexer lexer = new Lexer(code);
        List<Token> tokens = lexer.Tokenize();

        foreach (var token in tokens)
        {
            Console.WriteLine($"Type: {token.Type}, Value: {token.Value}");
        }
    }
}

}
