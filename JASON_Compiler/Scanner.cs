using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    String, Int, Float, read, write, repeat, until, If, elseif, Else, then, Return,
    endl, comment, STRING, Identifier, Number, Leftbracket, Rightbracket, Leftprant, Rightprant,
    comma, semicolon, EqualOp, LessThanOp, GreaterThanOp, NotEqualOp,
    PlusOp, MinusOp, DivideOp, MultiplyOp, And, Or, AssignOp,Main, end
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

            ReservedWords.Add("read", Token_Class.read);
            ReservedWords.Add("write", Token_Class.write);
            ReservedWords.Add("repeat", Token_Class.repeat);
            ReservedWords.Add("until", Token_Class.until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.endl);
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.STRING);
            ReservedWords.Add("number", Token_Class.Number);
            ReservedWords.Add("main", Token_Class.Main);
            ReservedWords.Add("(", Token_Class.Leftbracket);
            ReservedWords.Add(")", Token_Class.Rightbracket);
            ReservedWords.Add("{", Token_Class.Leftprant);
            ReservedWords.Add("}", Token_Class.Rightprant);
            ReservedWords.Add(";", Token_Class.semicolon);
            ReservedWords.Add(",", Token_Class.comma);
            ReservedWords.Add("end", Token_Class.end);

            Operators.Add(":=", Token_Class.AssignOp);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("&&", Token_Class.And);
            Operators.Add("||", Token_Class.Or);

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

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {

                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if ((CurrentChar >= 'A' && CurrentChar <= 'z') || (CurrentChar >= '0' && CurrentChar <= '9'))
                        {
                            CurrentLexeme += CurrentChar;
                        }
                        else
                        {
                            break;
                        }
                    }

                    i = j - 1;

                    FindTokenClass(CurrentLexeme);

                }
                else if (CurrentChar == '"')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '\n') break;
                        if (CurrentChar == '"')
                        {
                            CurrentLexeme += CurrentChar;
                            //FindTokenClass(CurrentLexeme);
                            break;
                        }
                        CurrentLexeme += CurrentChar;
                    }
                    FindTokenClass(CurrentLexeme);

                    i = j;
                }
                else if (CurrentChar == '/')
                {
                    if (SourceCode.Length > i + 1 && SourceCode[i + 1] == '*')
                    {

                        CurrentChar = SourceCode[j + 1];
                        CurrentLexeme += CurrentChar;

                        for (j = i + 2; j < SourceCode.Length; j++)
                        {
                            CurrentChar = SourceCode[j];
                            if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                                continue;
                            else if (CurrentChar == '*')
                            {
                                if (SourceCode.Length > j + 1 && SourceCode[j + 1] == '/')
                                {
                                    CurrentLexeme += CurrentChar;
                                    CurrentLexeme += SourceCode[j + 1];
                                    break;
                                }
                                else
                                {

                                    CurrentLexeme += CurrentChar;
                                }
                            }

                            else
                            {
                                CurrentLexeme += CurrentChar;
                            }
                        }

                        i = j + 1;
                    }
                    else
                    {
                        i = j;

                    }
                    FindTokenClass(CurrentLexeme);



                }
                // Number
                // i=1 curr=0 j=2
                //curr= A 
                else if ((CurrentChar >= '0' && CurrentChar <= '9')
                    || CurrentChar == '.')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if ((CurrentChar >= '0' && CurrentChar <= '9')
                            || CurrentChar == '.' || (CurrentChar >= 'A'
                            && CurrentChar <= 'z'))
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        else
                        {
                            break;
                        }

                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                }
                // operators
                else if (CurrentLexeme == "=" || CurrentLexeme == "<" || CurrentLexeme == ">")
                {
                    if (j + 1 < SourceCode.Length && CurrentLexeme == "<" && SourceCode[j + 1] == '>')
                    {
                        i += 1;
                        FindTokenClass("<>");
                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                    }
                }
                else if (CurrentLexeme == "+" || CurrentLexeme == "-" || CurrentLexeme == "*" || CurrentLexeme == "/")
                {
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentLexeme == "|" || CurrentLexeme == "&")
                {
                    if (j + 1 < SourceCode.Length && CurrentLexeme == SourceCode[i + 1].ToString())
                    {
                        i += 1;
                        FindTokenClass(CurrentLexeme + SourceCode[j].ToString());
                    }
                }
                else if (CurrentLexeme == ":")
                {
                    if (j + 1 <= SourceCode.Length && SourceCode[i + 1].ToString() == "=")
                    {
                        i += 1;
                        FindTokenClass(CurrentLexeme + SourceCode[j + 1].ToString());
                    }
                }
                else if (CurrentChar == ';' || CurrentChar == ',')
                {
                    FindTokenClass(CurrentChar.ToString());
                }
                else if (CurrentChar == '(')
                {
                    FindTokenClass(CurrentChar.ToString());
                }
                else if (CurrentChar == ')')
                {
                    FindTokenClass(CurrentChar.ToString());
                }
                else if (CurrentChar == '{')
                {
                    FindTokenClass(CurrentChar.ToString());
                }
                else if (CurrentChar == '}')
                {
                    FindTokenClass(CurrentChar.ToString());
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
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            //Is it an Operator? (Arithmatic_Operator , Condition_Operator, Boolean_Operator )
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);
            }
            //is string
            else if (isString(Lex) == true)
            {
                Tok.token_type = Token_Class.String;
                Tok.lex = Lex;

                Tokens.Add(Tok);
            }
            // is Number
            else if (isNumber(Lex) == true)
            {
                Tok.token_type = Token_Class.Number;
                Tok.lex = Lex;
                Tokens.Add(Tok);
            }
            ////Is it an comment
            else if (iscomment(Lex) == true)
            {
                Tok.token_type = Token_Class.comment;

                //Tokens.Add(Tok);
            }

            //Is it an undefined?else{
            else
            {
                Errors.Error_List.Add(Lex);
            }
        }

        bool isIdentifier(string lex)
        {

            bool isValid = true;
            // Check if the lex is an identifier or not.
            Regex regString = new Regex("^[a-zA-Z]([a-zA-Z0-9])*$");
            if (!regString.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;

        }
        bool isString(string lex)
        {
            var regString = new Regex("^\"([^\"]|\\\")*\"$");
            bool isValid = true;
            // Check if the lex is a string or not.
            if (!regString.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }
        bool isNumber(string Lex)
        {
            bool isValid = true;
            Regex regnumber = new Regex(@"^([-+]?[0-9]+)?([\.][0-9]+)?([eE][-+]?[0-9]+)?$");
            if (!regnumber.IsMatch(Lex) == true)
            {
                isValid = false;
            }
            return isValid;
        }
        bool iscomment(string lex)
        {
            var regString = new Regex("^/\\*.*\\*/$");
            bool isValid = true;
            // Check if the lex is a string or not.
            if (!regString.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }

    }
}