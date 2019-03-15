using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    class Scanner
    {

        private const string lettersToCheck = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
        private const string numbersToCheck = "0123456789";
        private const string identifierToCheck = lettersToCheck + numbersToCheck + "_";
        private const string whitespaceToCheck = " \t\n\r";
        int countofopenbraces = 0;
        int countofclosedbraces = 0;
        int[] arrayofopenbraces = new int[100];
        int[] arrayofclosedbraces = new int[100];
        int iforopenbraces = 0;
        int iforclosedbraces = 0;
        int countofopenparan = 0;
        int countofclosedparan = 0;
        int[] arrayofopenparan = new int[100];
        int[] arrayofclosedparan = new int[100];
        int countofopenbrackets = 0;
        int countofclosedbrackets = 0;
        int[] arrayofopenbrackets = new int[100];
        int[] arrayofclosedbrackets = new int[100];
        int iforopenbrackets = 0;
        int iforclosedbrackets = 0;
        int countofopencomments = 0;
        int countofclosedcomments = 0;
        int[] arrayofopencomments = new int[100];
        int[] arrayofclosedcomments = new int[100];
        int iforopencomments = 0;
        int iforclosedcomments = 0;
        int iforopenparan = 0;
        int iforclosedparan = 0;
        int checkfordatatype = 0;
        int checkforinitialnum = 0;
        Boolean strangecase = true;
        int slashcomments = 1000;
        int starcomments = 2000;

        private Dictionary<string, TokenType> dictKeywords = new Dictionary<string, TokenType>() {
            { "if", TokenType.Keyword_if },
            { "then", TokenType.Keyword_then },
            { "else", TokenType.Keyword_else },
            { "for", TokenType.Keyword_for },
            { "class", TokenType.Keyword_class },
            { "int", TokenType.DataType_int},
            { "float", TokenType.DataType_float},
            { "get", TokenType.Keyword_get},
            { "put", TokenType.Keyword_put},
            { "return", TokenType.Keyword_retun },
            { "program", TokenType.Keyword_program },
            {"InvalidId",TokenType.InvalidId },
            { "InvalidNumber", TokenType.InvalidNumber }

        };


        private Dictionary<string, TokenType> dictKeywordsCaps = new Dictionary<string, TokenType>() {
            { "IF", TokenType.Error_Keyword_if },
            { "THEN", TokenType.Error_Keyword_then },
            { "ELSE", TokenType.Error_Keyword_else },
            { "FOR", TokenType.Error_Keyword_for },
            { "CLASS", TokenType.Error_Keyword_class },
            { "INT", TokenType.Error_DataType_int},
            { "FLOAT", TokenType.Error_DataType_float},
            { "GET", TokenType.Error_Keyword_get},
            { "PUT", TokenType.Error_Keyword_put},
            { "RETURN", TokenType.Error_Keyword_retun },
            { "PROGRAM", TokenType.Error_Keyword_program }


        };


        private Dictionary<string, TokenType> dictOperators = new Dictionary<string, TokenType>() {
            { "+", TokenType.Op_add },
            { "-", TokenType.Op_subtract },
            { "*", TokenType.Op_multiply },
            { "/", TokenType.Op_divide },
            { "%", TokenType.Op_mod },
            { "=", TokenType.Op_assign },
            { "<", TokenType.Op_less },
            { ">", TokenType.Op_greater },
            { "!", TokenType.Op_not },

        };

        private List<string> listOfKeywords;
        private List<string> listOfKeywordsCaps;
        private string operatorsToCheck = "+-*/%=<>!%";
        private string codeToCheck;
        private List<Tokens> listTokens = new List<Tokens>();
        private int lineCount = 1;
        private int posCount = 1;

        public string characterNow
        {
            get
            {
                try
                {
                    return codeToCheck.Substring(0, 1);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return "";
                }
            }
        }

        public Scanner(string code)
        {
            codeToCheck = code;
            listOfKeywords = dictKeywords.Keys.ToList();
            listOfKeywordsCaps = dictKeywordsCaps.Keys.ToList();
        }

        private void advance(int characters = 1)
        {
            try
            {
                if (characters == slashcomments)
                {
                    do
                    {
                        codeToCheck = codeToCheck.Substring(1, codeToCheck.Length - 1);
                        posCount += characters;
                    } while (characterNow != "\n");
                }


                //else if (characters == starcomments)
                //{
                //    do
                //    {
                //        codeToCheck = codeToCheck.Substring(1, codeToCheck.Length - 1);
                //        posCount += characters;
                //    } while (characterNow != "*");
                //}
                else
                {

                    // reset position when there is a newline
                    if (characterNow == "\n")
                    {
                        posCount = 0;
                        lineCount++;
                    }

                    codeToCheck = codeToCheck.Substring(characters, codeToCheck.Length - characters);
                    posCount += characters;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                codeToCheck = "";
            }
        }
        private void advancereset(int currentline, int currentposition, string currentcode)
        {
            try
            {
                codeToCheck = currentcode;
                posCount = currentline;
                lineCount = currentposition;

            }
            catch (ArgumentOutOfRangeException)
            {
                codeToCheck = "";
            }
        }
        public void error(string message, int line, int position)
        {

            Console.WriteLine(String.Format("{0} @ {1}:{2}", message, line, position));
            Lexical_Analyzer.errorlog(message, line, position);

        }


        public bool check(string originalClass, string toCheckClass, TokenType tokenType,
                          string differentClass = null, int maxLength = Int32.MaxValue, bool same = false,
                          bool discard = false, int offset = 0)
        {
            try
            {
                //++ cases
                if (characterNow == "")
                    return false;

                int line = lineCount;
                int position = posCount;

                if (same)
                {
                    String tokenfloat = null;
                    if (codeToCheck.StartsWith(originalClass))
                    {
                        if (originalClass == "//")
                        {
                            advance(slashcomments);
                            advance(1);
                            strangecase = false;
                        }
                        //else if (originalClass == "/*")
                        //{
                        //    advance(starcomments);
                        //    advance(1);
                        //    strangecase = false;
                        //}
                        else if (!discard)
                        {
                            listTokens.Add(new Tokens() { Type = tokenType, Value = originalClass, Line = line, Position = position - offset });
                            strangecase = false;
                            advance(originalClass.Length);
                        }
                        return true;

                    }
                    //Check float values
                    else if (originalClass.Contains(characterNow) && codeToCheck.Contains("."))
                    {
                        string nextchar = codeToCheck.Substring(1, codeToCheck.Length - 1);
                        string nextcharnow = nextchar.Substring(0, 1);
                        int value;
                        int countwhile = 0;





                        if (((int.TryParse(nextcharnow, out value)) || (nextcharnow == ".")) && (int.TryParse(characterNow, out value)))
                        {
                            if ((!(discard)))
                            {
                                while (!(characterNow == ";" || characterNow == ","))
                                {
                                    countwhile++;
                                    tokenfloat += characterNow;
                                    advance();
                                    if (countwhile > 20)
                                        break;
                                }
                                if (tokenfloat.StartsWith("0"))
                                {
                                    Tokens token = new Tokens() { Type = dictKeywords["InvalidNumber"], Value = tokenfloat, Line = line, Position = position - offset, Error = "Lexical Analysis_Initial 0 is not allowed" };
                                    String dictkey = (line + "," + (position - offset)).ToString();
                                    List<String> dictValue = new List<string>();
                                    dictValue.Add("Lexical Error" + "," + "Invalid Number Declaration");                                    
                                    Lexical_Analyzer.errorDict.Add(dictkey, dictValue);
                                    listTokens.Add(token);
                                    strangecase = false;
                                }
                                else
                                {
                                    //if (tokenfloat.Contains("e") || (tokenfloat.Contains("+") || tokenfloat.Contains("-")))
                                    if (tokenfloat.Contains("e") || tokenfloat.Contains("-"))
                                    {
                                        int index = tokenfloat.IndexOf("e");
                                        if ((tokenfloat.Substring(index + 1, 1) == "+") || tokenfloat.Substring(index + 1, 1) == "-")
                                        {
                                            listTokens.Add(new Tokens() { Type = tokenType, Value = tokenfloat, Line = line, Position = position - offset });
                                            strangecase = false;
                                        }
                                        else
                                        {
                                            listTokens.Add(new Tokens() { Type = tokenType, Value = tokenfloat, Line = line, Position = position - offset });
                                            strangecase = false;
                                        }
                                    }
                                    else
                                    {
                                        listTokens.Add(new Tokens() { Type = tokenType, Value = tokenfloat, Line = line, Position = position - offset });
                                        strangecase = false;
                                    }

                                }
                                return true;
                            }
                        }
                    }
                    return false;
                }

                if (!originalClass.Contains(characterNow))
                    return false;

                string tokenValue = characterNow;
                advance();
                strangecase = false;

                while ((toCheckClass ?? "").Contains(characterNow) && tokenValue.Length <= maxLength && characterNow != "")
                {
                    tokenValue += characterNow;
                    advance();
                }


                //Handle identifiers - 1abc
                if (differentClass != null && differentClass.Contains(characterNow))
                {

                    checkforinitialnum++;
                    int countforloop = 0;
                    while (!(characterNow == "=" || characterNow == ";"))
                    {
                        countforloop++;
                        tokenValue += characterNow;
                        advance();
                        if (countforloop > 20)
                            break;
                    }
                }


                if (!discard)
                {
                    if (checkforinitialnum > 0)
                    {

                        Tokens token = new Tokens() { Type = dictKeywords["InvalidId"], Value = tokenValue, Line = line, Position = position - offset, Error = "Lexical Analysis_Identifier shouldnt start with number" };
                        listTokens.Add(token);
                        checkforinitialnum = 0;
                        strangecase = false;
                    }
                    else if (tokenValue.StartsWith("0"))
                    {

                        Tokens token = new Tokens() { Type = dictKeywords["InvalidNumber"], Value = tokenValue, Line = line, Position = position - offset, Error = "Lexical Analysis_Initial 0 is not allowed" };
                        String dictkey = (line + "," + (position - offset)).ToString();
                        List<String> dictValue = new List<string>();
                        dictValue.Add("Lexical Error" + "," + "Invalid Number Declaration");
                        Lexical_Analyzer.errorDict.Add(dictkey, dictValue);
                        listTokens.Add(token);
                        strangecase = false;
                    }
                    else
                    {
                        Tokens token = new Tokens() { Type = tokenType, Value = tokenValue, Line = line, Position = position - offset };
                        listTokens.Add(token);
                        strangecase = false;
                    }
                }
            }
            catch (Exception ex)
            {
                error(ex.ToString(), lineCount, posCount);
            }
            return true;


        }



        public List<Tokens> scan()
        {
            try
            {
                while (characterNow != "")
                {
                    strangecase = true;
                    // Check for Whitespaces
                    check(whitespaceToCheck, whitespaceToCheck, TokenType.None, discard: true);

                    // Check for float values(1.2) 
                    check(numbersToCheck, null, TokenType.Float, same: true);

                    check("+", null, TokenType.Op_equal, same: true);

                    // Check for integers
                    check(numbersToCheck, numbersToCheck, TokenType.Integer, differentClass: lettersToCheck);

                    // if(check(lettersToCheck,)

                    // Check for identifiers and keywords
                    if (check(lettersToCheck, identifierToCheck, TokenType.Identifier))
                    {
                        Tokens match = listTokens.Last();
                        String checkdt = match.Value.ToString();
                        if (checkfordatatype != 0)
                        {

                            //do code for int and float
                            if (checkdt.StartsWith("_"))
                            {
                                match.Type = dictKeywords["InvalidId"];
                            }
                        }
                        checkfordatatype = 0;
                        if ((checkdt == "int") || (checkdt == "float"))
                        {
                            checkfordatatype++;
                        }

                        if (listOfKeywordsCaps.Contains(match.Value))
                            match.Type = dictKeywordsCaps[match.Value];


                        if (listOfKeywords.Contains(match.Value))
                            match.Type = dictKeywords[match.Value];

                    }

                    if (check("/*", null, TokenType.OpenComments, same: true))
                    {
                        countofopencomments++;
                        arrayofopencomments[iforopencomments] = lineCount;
                        iforopencomments++;
                    }
                    if (check("*/", null, TokenType.CloseComments, same: true))
                    {
                        countofclosedcomments++;
                        arrayofclosedcomments[iforclosedcomments] = lineCount;
                        iforclosedcomments++;
                    }

                    check("<=", null, TokenType.Op_lessequal, same: true);
                    check(">=", null, TokenType.Op_greaterequal, same: true);
                    check("==", null, TokenType.Op_equal, same: true);
                    check("<>", null, TokenType.Op_notequal, same: true);
                    check("&&", null, TokenType.Op_and, same: true);
                    check("||", null, TokenType.Op_or, same: true);
                    check("::", null, TokenType.Op_qualifier, same: true);
                    check("//", null, TokenType.TwoSlashcomments, same: true);
                    check("/*", null, TokenType.TwoSlashcomments, same: true);
                    check("+=", null, TokenType.TwoSlashcomments, same: true);

                    if (check(operatorsToCheck, null, TokenType.None, maxLength: 1))
                    {
                        Tokens match = listTokens.Last();
                        match.Type = dictOperators[match.Value];
                    }

                    // brackets, braces and separators
                    if (check("(", null, TokenType.LeftParen, same: true))
                    {
                        countofopenparan++;
                        arrayofopenparan[iforopenparan] = lineCount;
                        iforopenparan++;
                    }
                    if (check(")", null, TokenType.RightParen, same: true))
                    {
                        countofclosedparan++;
                        arrayofclosedparan[iforclosedparan] = lineCount;
                        iforclosedparan++;
                    }

                    if (check("{", null, TokenType.LeftBrace, same: true))
                    {

                        arrayofopenbraces[iforopenbraces] = lineCount;
                        countofopenbraces++;
                        iforopenbraces++;

                    }
                    if (check("}", null, TokenType.RightBrace, same: true))
                    {
                        countofclosedbraces++;
                        arrayofclosedbraces[iforclosedbraces] = lineCount;
                        iforclosedbraces++;
                    }

                    check(";", null, TokenType.Semicolon, same: true);
                    check(",", null, TokenType.Comma, same: true);
                    check(".", null, TokenType.FullStop, same: true);
                    check(":", null, TokenType.ConditionOperator, same: true);

                    if (check("[", null, TokenType.LeftSquareBracket, same: true))
                    {
                        countofopenbrackets++;
                        arrayofopenbrackets[iforopenbrackets] = lineCount;
                        iforopenbrackets++;
                    }
                    if (check("]", null, TokenType.RightSquareBracket, same: true))
                    {
                        countofclosedbrackets++;
                        arrayofclosedbrackets[iforclosedbrackets] = lineCount;
                        iforclosedbrackets++;
                    }
                    if (strangecase == true)
                    {

                        error("Unidentified Token: " + characterNow, lineCount, posCount);
                        advance();

                    }
                }

                // end of file token
                listTokens.Add(new Compiler.Tokens() { Type = TokenType.End_of_input, Line = lineCount, Position = posCount });

                if (countofclosedbraces != countofopenbraces)
                {
                    Console.Write("Curly Brace error:");
                    Console.Write("\tOpen Curly Braces @lines:");
                    for (int i1 = 0; i1 < countofopenbraces; i1++)
                    {
                        Console.Write(arrayofopenbraces[i1] + ",");
                        Lexical_Analyzer.errorlog("Lexical Analysis_Curly brace error - Open braces@", arrayofopenbraces[i1], 1);

                    }
                    Console.Write("\tClosed Curly Braces @lines:");
                    for (int i2 = 0; i2 < countofclosedbraces; i2++)
                    {
                        Console.Write(arrayofclosedbraces[i2] + ",");
                        Lexical_Analyzer.errorlog("Lexical Analysis_Curly brace error - Close braces@", arrayofclosedbraces[i2], 1);
                    }

                }

                if (countofopenparan != countofclosedparan)
                {
                    Console.WriteLine("\n");
                    Console.Write("Paranthesis error:");
                    Console.Write("Open Paran @lines:");
                    for (int i3 = 0; i3 < countofopenparan; i3++)
                    {
                        Console.Write(arrayofopenparan[i3] + ",");
                        Lexical_Analyzer.errorlog("Lexical Analysis_Paranthesis error - Open braces@", arrayofopenparan[i3], 1);

                    }
                    Console.Write("\tClosed Paran @lines:");
                    for (int i4 = 0; i4 < countofclosedparan; i4++)
                    {
                        Console.Write(arrayofclosedparan[i4] + ",");
                        Lexical_Analyzer.errorlog("Lexical Analysis_Paranthesis error - Close braces@", arrayofclosedparan[i4], 1);
                    }

                }


                if (countofopenbrackets != countofclosedbrackets)
                {
                    Console.WriteLine("\n");
                    Console.Write("Brackets error:");
                    Console.Write("\tOpen Bracket @lines:");
                    for (int i5 = 0; i5 < countofopenbrackets; i5++)
                    {
                        Console.Write(arrayofopenbrackets[i5] + ",");
                        Lexical_Analyzer.errorlog("Lexical Analysis_Bracket error - Open Bracket@", arrayofopenbrackets[i5], 1);

                    }
                    Console.Write("\tClosed Bracket @lines:");
                    for (int i6 = 0; i6 < countofclosedbrackets; i6++)
                    {
                        Console.Write(arrayofclosedbrackets[i6] + ",");
                        Lexical_Analyzer.errorlog("Lexical Analysis_Bracket error - Close Bracket@", arrayofclosedbrackets[i6], 1);
                    }

                }

                if (countofopencomments != countofclosedcomments)
                {
                    Console.WriteLine("\n");
                    Console.Write("Comments error:");
                    Console.Write("\tOpen comments @lines:");
                    for (int i7 = 0; i7 < countofopencomments; i7++)
                    {
                        Console.Write(arrayofopencomments[i7] + ",");
                        Lexical_Analyzer.errorlog("Lexical Analysis_Comments error - Open Comment@", arrayofopencomments[i7], 1);
                    }
                    Console.Write("\tClosed comments @lines:");
                    for (int i8 = 0; i8 < countofclosedcomments; i8++)
                    {
                        Console.Write(arrayofclosedcomments[i8] + ",");
                        Lexical_Analyzer.errorlog("Lexical Analysis_Comments error - Close Comment@", arrayofclosedcomments[i8], 1);

                    }
                }
            }
            catch (Exception ex)
            {
                error(ex.ToString(), lineCount, posCount);
            }
            return listTokens;
        }
    }
}
