using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    public enum TokenType
    {
        End_of_input, Op_multiply, Op_divide, Op_mod, Op_add, Op_subtract,
        Op_negate, Op_not, Op_less, Op_lessequal, Op_greater, Op_greaterequal,
        Op_equal, Op_notequal, Op_assign, Op_and, Op_or, TwoSlashcomments, Keyword_if, Keyword_then,
        Keyword_else, Keyword_for, Keyword_class, Integer, Float, String, Keyword_get, Keyword_put,
        Keyword_retun, Keyword_program, LeftParen, RightParen, OpenComments, CloseComments,
        LeftBrace, RightBrace, Semicolon, Comma, Identifier, Literals, None, InvalidId, InvalidNumber,
        DataType_int, DataType_float, ConditionOperator, FullStop, Op_qualifier, LeftSquareBracket, RightSquareBracket,
        Error_Keyword_if, Error_Keyword_then, Error_Keyword_else, Error_Keyword_for, Error_Keyword_class, Error_DataType_int,
        Error_DataType_float, Error_Keyword_get, Error_Keyword_put, Error_Keyword_retun, Error_Keyword_program
    }
    public class Tokens
    {
        public TokenType Type { get; set; }
        public int Line { get; set; }
        public int Position { get; set; }
        public string Value { get; set; }

        public string Error { get; set; }
        public override string ToString()
        {
            if (Type == TokenType.Integer || Type == TokenType.Identifier || Type == TokenType.Literals || Type == TokenType.InvalidId || Type == TokenType.Float || Type == TokenType.InvalidNumber)
            {
                return String.Format("{0,-5}  {1,-5}   {2,-14}     {3}   {4}", Line, Position, Type.ToString(), Value, Error);
            }
            else if (Type == TokenType.String)
            {
                //return String.Format("{0,-5}  {1,-5}   {2,-14}     \"{3}\"", Line, Position, Type.ToString(), Value.Replace("\n", "\\n"));
                return String.Format("{0,-5}  {1,-5}   {2,-14}     {3}   {4}", Line, Position, Type.ToString(), Value, Error);
            }
            return String.Format("{0,-5}  {1,-5}   {2,-14}", Line, Position, Type.ToString(), Error);
        }
    }
}
