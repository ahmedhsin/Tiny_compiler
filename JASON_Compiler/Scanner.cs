using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, String
}
namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            /* Add ReservedWords Dictionary */

            ReservedWords.Add("IF", Token_Class.If);
            ReservedWords.Add("BEGIN", Token_Class.Begin);
            ReservedWords.Add("CALL", Token_Class.Call);
            ReservedWords.Add("DECLARE", Token_Class.Declare);
            ReservedWords.Add("END", Token_Class.End);
            ReservedWords.Add("DO", Token_Class.Do);
            ReservedWords.Add("ELSE", Token_Class.Else);
            ReservedWords.Add("ENDIF", Token_Class.EndIf);
            ReservedWords.Add("ENDUNTIL", Token_Class.EndUntil);
            ReservedWords.Add("ENDWHILE", Token_Class.EndWhile);
            ReservedWords.Add("INTEGER", Token_Class.Integer);
            ReservedWords.Add("PARAMETERS", Token_Class.Parameters);
            ReservedWords.Add("PROCEDURE", Token_Class.Procedure);
            ReservedWords.Add("PROGRAM", Token_Class.Program);
            ReservedWords.Add("READ", Token_Class.Read);
            ReservedWords.Add("REAL", Token_Class.Real);
            ReservedWords.Add("SET", Token_Class.Set);
            ReservedWords.Add("THEN", Token_Class.Then);
            ReservedWords.Add("UNTIL", Token_Class.Until);
            ReservedWords.Add("WHILE", Token_Class.While);
            ReservedWords.Add("WRITE", Token_Class.Write);

            //Operators.Add(".", Token_Class.Dot);
            //Operators.Add(";", Token_Class.Semicolon);
            //Operators.Add(",", Token_Class.Comma);
            //Operators.Add("(", Token_Class.LParanthesis);
            //Operators.Add(")", Token_Class.RParanthesis);
            //Operators.Add("=", Token_Class.EqualOp);
            //Operators.Add("<", Token_Class.LessThanOp);
            //Operators.Add(">", Token_Class.GreaterThanOp);
            //Operators.Add("!", Token_Class.NotEqualOp);
            //Operators.Add("+", Token_Class.PlusOp);
            //Operators.Add("-", Token_Class.MinusOp);
            //Operators.Add("*", Token_Class.MultiplyOp);
            //Operators.Add("/", Token_Class.DivideOp);




        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                //{}
                else if (CurrentChar == '{')
                {
                    while (true)
                    {
                        j++;
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar;
                        if (CurrentChar == '}')
                        {
                            CurrentLexeme += CurrentChar;
                            break;
                        }
                    }
                }
                //()
                else if (CurrentChar == '(')
                {
                    while (true)
                    {
                        j++;
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar;
                        if (CurrentChar == ')')
                        {
                            CurrentLexeme += CurrentChar;
                            break;
                        }
                    }
                }
                else if (CurrentChar >= '\"')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '\"')
                        {
                            CurrentLexeme += CurrentChar;
                            FindTokenClass(CurrentLexeme);
                            break;
                        }
                        CurrentLexeme += CurrentChar;
                    }
                    i = j - 1;
                }
                else
                {
                    FindTokenClass(CurrentLexeme);
                }
            }

            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);

            }
            //Is it an identifier?

            //Is it a String?
            if (isString(Lex) == true)
            {
                Tok.token_type = Token_Class.String;
                Tok.lex = Lex;

                Tokens.Add(Tok);
            }
            //Is it an Operator? (Arithmatic_Operator , Condition_Operator, Boolean_Operator )

            //Is it a Datatype?
        }



        bool isIdentifier(string lex)
        {
            bool isValid = true;
            // Check if the lex is an identifier or not.

            return isValid;
        }
        bool isString(string lex)
        {
            var regString = new Regex("^\"(?:[^\"]|\\\")*\"$");
            bool isValid = true;
            // Check if the lex is a string or not.
            if (!regString.IsMatch(lex))
            {
                isValid = true;
            }
            return isValid;
        }
    }
}
