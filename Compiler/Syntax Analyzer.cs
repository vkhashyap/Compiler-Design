using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Compiler
{

    public class Syntax_Analyzer
    {
    }
    public class Parser
    {
        private static Dictionary<string, string> _mainMaster;
        private static Dictionary<string, string> _specialCharacterMaster;
        private static Dictionary<string, List<string>> _productionMaster;
        private static Dictionary<string, List<string>> _firstMaster;
        private static Dictionary<string, List<string>> _followMaster;
        public static Dictionary<string, string> _dataTypeAndVariablePairToCheck;
        private static Dictionary<string, string> classAndVariablesPair = new Dictionary<string, string>();
        private static Dictionary<string, string> operators = new Dictionary<string, string>();
        private static Dictionary<string, string> funReturnType = new Dictionary<string, string>();


        private static Dictionary<string, List<string>> _nodes;

        private static List<string> _nonTerminalMaster;
        private static List<string> _terminalMaster;
        private static List<string> _checkNodeKeyValue;
        private static List<Tokens> _allTokensParsing;
        private static List<string> _semanticOutput;
        private static List<string> _classDeclChildList;
        private static List<string> _declChildList;
        private static List<string> _assignStatChildList;
        private static List<string> declaredVariablesListiwithClass;
        private static List<int> _arrayListAddToSym;
        private static List<string> _paramListAddToSym;
        private static List<string> _funcDefListAddToSym;
        private static List<string> assignlistToFormAssemble;

        //private static Stack<string> _tokenStack;
        private static Stack<string> _grammarStack;
        //private static Stack<string> _valueStack;
        private static Stack<string> _contextStack;
        private static Stack<TokenstoParse> tokenStackToParse;

        private static TokenstoParse _tokenStackPeek;
        private static string _grammarStackPeek;
        private static string _currentNonTerminal;
        private static string operatorToAdd;

        private static int i;

        public static Composite rootDecl = new Composite("Prog");
        public static Composite classList = new Composite("classList");
        public static Composite funcDefList = new Composite("funcDefList");
        public static Composite statBlock = new Composite("program");
        public static Composite currentVarDecl;
        public static Composite currentClassDecl;
        public static Composite currentFuncDecl;
        public static Composite currentFuncDef;
        public static Composite currentParamDecl;
        public static Composite currentFuncBody;
        public static Composite currentAssignStat;
        public static Composite currentMemberDeclList;
        public static Composite currentInheritanceList;
        public static Composite currentArraySizeList;
        public static Composite currentStatBlock;
        public static Composite currentReturnBlock;
        public static Composite currentstatBlockProg;
        public static Composite currentProgram;
        public static Composite currentClassObjDecl;
        public static SymbolTable symbolTable = new SymbolTable();
        public static SymbolTable varSymbolTable = new SymbolTable();
        public static SymbolTable varFuncSymbolTable = new SymbolTable();
        public static SymbolTable varFuncParamSymbolTable = new SymbolTable();
        public static SymbolTable varFuncDefSymbolTable = new SymbolTable();
        public static SymbolTable varFuncBodySymbolTable = new SymbolTable();
        public static SymbolTable varAssignStatTable = new SymbolTable();
        public static Symbol currentSymbol;
        public static Symbol currentVarSymbol;
        public static Symbol currentFunSymbol;
        public static Symbol currentFuncParamSymbol;
        public static Symbol currentFuncDefSymbol;
        public static Symbol currentFuncBodySymbol;
        public static Symbol currentAssignStatSymbol;
        public static Symbol currentProgramSymbol;
        public static int countFuncDecl = 0;
        public static int countFuncParamDecl = 0;
        public static int countFuncDef = 0;
        public static int countFuncBody = 0;
        public static int countAssignStat = 0;
        public static Boolean checkProgram = true;
        public static string currentDataTypeOnLeft = null;
        public static string currentDataTypeOnRight = null;
        public static string currentVariableScopePair = null;
        public static string combinedValue = null;
        public static Boolean scopeCheck = false;
        public static Boolean flagForFuncDecl = false;
        public static Boolean flagForArraySize = false;
        public static Boolean flagForVarFuncDef = false;
        public static Boolean flagForAssignRight = false;
        public static Boolean flagForError = false;
        public static Boolean flagForParamVisit = false;
        public static string currentClassName = null;
        public static string currentFuncName = null;
        public static string errorFunc = "program";
        public static string errorVar = null;
        public static int classSize = 0;
        public static int varSize = 0;
        public static int funcSize = 0;
        public static int programFuncSize = 0;
        public static int arraySize = 0;
        public static int paramSize = 0;
        public static int countdebug = 0;
        public static Boolean visitEqualto = false;
        public static string progAST = null;
        public static int maxRegister;
        public static int intSize;
        public static string leftOperand;
        public static string rightOperand;
        public static StringBuilder sbToAssembler = new StringBuilder();
        public static StringBuilder sbFunc = new StringBuilder();
        public static string strFunc = null;
        public static Boolean funcVisit = false;
        public static Boolean errorInDecl = false;
        public static int valueinFunc=0;
        public static string strPutValue = " % link buffer to stack \r\n" +
"		  addi r2,r0, buf \r\n" +
"		  sw -12(r14),r2 \r\n" +
"		  % convert int to string for output \r\n" +
"		  jl r15, intstr \r\n" +
"		  sw -8(r14),r13 \r\n" +
"		  % output to console \r\n" +
"		  jl r15, putstr \r\n" +
"		  addi   r1, r0, m2       % CR\r\n" +
"		  sw  -8(r14), r1\r\n" +
"		  jl     r15, putstr\r\n" +
"		  subi r14,r14,-4 \r\n" +
"		  hlt\r\n";
        public static string strEnd = "getstr    lw    r1,-8(r14)    % i := r1\r\n" +
"getstr1   getc  r2            % get ch\r\n" +
"          ceqi  r3,r2,10\r\n" +
"          bnz   r3,getstr2    % branch if ch = CR\r\n" +
"          sb    0(r1),r2      % B[i] := ch\r\n" +
"          addi  r1,r1,1       % i++\r\n" +
"          j     getstr1\r\n" +
"getstr2   sb    0(r1),r0      % B[i] := '\\0'\r\n" +
"          jr    r15\r\n" +
"strint    addi  r13,r0,0      % R := 0 (result)\r\n" +
"          addi  r4,r0,0       % S := 0 (sign)\r\n" +
"          lw    r1,-8(r14)    % i := r1\r\n" +
"          addi  r2,r0,0\r\n" +
"strint1   lb    r2,0(r1)      % ch := B[i]\r\n" +
"          cnei  r3,r2,32\r\n" +
"          bnz   r3,strint2    % branch if ch != blank\r\n" +
"          addi  r1,r1,1\r\n" +
"          j     strint1\r\n" +
"strint2   cnei  r3,r2,43\r\n" +
"          bnz   r3,strint3    % branch if ch != \"+\"\r\n" +
"          j     strint4\r\n" +
"strint3   cnei  r3,r2,45\r\n" +
"          bnz   r3,strint5    % branch if ch != \"-\"\r\n" +
"          addi  r4,r4,1       % S := 1\r\n" +
"strint4   addi  r1,r1,1       % i++\r\n" +
"          lb    r2,0(r1)      % ch := B[i]\r\n" +
"strint5   clti  r3,r2,48\r\n" +
"          bnz   r3,strint6    % branch if ch < \"0\"\r\n" +
"          cgti  r3,r2,57\r\n" +
"          bnz   r3,strint6    % branch if ch > \"9\"\r\n" +
"          subi  r2,r2,48      % ch -= \"0\"\r\n" +
"          muli  r13,r13,10    % R *= 10\r\n" +
"          add   r13,r13,r2    % R += ch\r\n" +
"          j     strint4\r\n" +
"strint6   ceqi  r3,r4,0\r\n" +
"          bnz   r3,strint7    % branch if S = 0\r\n" +
"          sub   r13,r0,r13    % R := -R\r\n" +
"strint7   jr    r15\r\n" +
"putstr   lw  r1,-8(r14)    % i := r1 \r\n" +
"          addi  r2,r0,0\r\n" +
"putstr1   lb    r2,0(r1)      % ch := B[i] \r\n" +
"          ceqi  r3,r2,0 \r\n" +
"          bnz   r3,putstr2    % branch if ch = 0 \r\n" +
"          putc  r2 \r\n" +
"          addi  r1,r1,1       % i++ \r\n" +
"          j     putstr1 \r\n" +
"putstr2   jr    r15 \r\n" +
"intstr    lw    r13,-12(r14)\r\n" +
"          addi  r13,r13,11    % r13 points to end of buffer\r\n" +
"          sb    0(r13),r0     % store terminator\r\n" +
"          lw    r1,-8(r14)    % r1 := N (to be converted)\r\n" +
"          addi  r2,r0,0       % S := 0 (sign)\r\n" +
"          cgei  r3,r1,0\r\n" +
"          bnz   r3,intstr1    % branch if N >= 0\r\n" +
"          addi  r2,r2,1       % S := 1\r\n" +
"          sub   r1,r0,r1      % N := -N\r\n" +
"intstr1   addi  r3,r1,0       % D := N (next digit)\r\n" +
"          modi  r3,r3,10      % D mod= 10\r\n" +
"          addi  r3,r3,48      % D += \"0\"\r\n" +
"          subi  r13,r13,1     % i--\r\n" +
"          sb    0(r13),r3     % B[i] := D\r\n" +
"          divi  r1,r1,10      % N div= 10\r\n" +
"          cnei  r3,r1,0\r\n" +
"          bnz   r3,intstr1    % branch if N != 0 \r\n" +
"          ceqi  r3,r2,0\r\n" +
"          bnz   r3,intstr2    % branch if S = 0\r\n" +
"          subi  r13,r13,1     % i--\r\n" +
"          addi  r3,r0,45\r\n" +
"          sb    0(r13),r3     % B[i] := \"-\"\r\n" +
"intstr2   jr    r15\r\n" +
"buf    res 20\r\n" +
"arg    dw  0\r\n" +
"x      dw    0\r\n" +
"m2     db  13, 10, 0\r\n";


        //Checking if same variable is decalred again
        public static bool IsVariableExist(string name)
        {
            var isExist = false;

            foreach (var symbol in varSymbolTable.GetSymbols())
            {
                if (symbol.Name == name)
                {
                    isExist = true;
                    break;
                }
            }

            return isExist;
        }
        public static Composite CreateNodeByName(string name)
        {
            return new Composite(name);
        }
        public Parser(List<Tokens> allTokens)
        {

            _mainMaster = new Dictionary<string, string>();
            _dataTypeAndVariablePairToCheck = new Dictionary<string, string>();
            _productionMaster = new Dictionary<string, List<string>>();
            _specialCharacterMaster = new Dictionary<string, string>();

            _nodes = new Dictionary<string, List<string>>();
            funReturnType = new Dictionary<string, string>();

            _firstMaster = new Dictionary<string, List<string>>();
            _followMaster = new Dictionary<string, List<string>>();


            //_tokenStack = new Stack<string>();
            _grammarStack = new Stack<string>();
            //_valueStack = new Stack<string>();
            _contextStack = new Stack<string>();
            tokenStackToParse = new Stack<TokenstoParse>();

            _checkNodeKeyValue = new List<string>();
            _semanticOutput = new List<string>();
            _classDeclChildList = new List<string>();
            _declChildList = new List<string>();
            _assignStatChildList = new List<string>();
            declaredVariablesListiwithClass = new List<string>();
            _arrayListAddToSym = new List<int>();
            _paramListAddToSym = new List<string>();
            _funcDefListAddToSym = new List<string>();
            assignlistToFormAssemble = new List<string>();
            maxRegister = 13;
            intSize = 8;
            leftOperand = "";
            rightOperand = "";
            operators.Add("*", "mul");
            operators.Add("+", "add");

            i = 1;

            _allTokensParsing = allTokens;

            StartParsing();

        }
        public static void StartParsing()
        {
            //Master
            PopulateMainMaster();
            PopulateFirstMaster();
            PopulateSpecialCharacterMaster();
            PopulateFollowMaster();
            PopulateTerminalMaster();
            PopulateNonTerminalMaster();
            PopulateProductionMaster();

            //get tokens
            PopulateTokenStackFromTokens(_allTokensParsing);
            _tokenStackPeek = tokenStackToParse.Peek();

            //grammar stack
            var productionList = GetProductionByKey(MainMasterKeys.prog_class);

            //Add Initial Nodes
            _grammarStack.Push("$");
            _grammarStack.PushRange(productionList);
            _grammarStackPeek = _grammarStack.Peek();

            //Creating root nodes and adding child nodes
            rootDecl.Add(classList);
            rootDecl.Add(funcDefList);
            rootDecl.Add(statBlock);

            //if Check tokenStackPeek is $ goto step6 else step4            
            if (_tokenStackPeek.tokenType == Terminals.dollar)
            {
                Console.WriteLine("No Input provided");
            }
            else
            {
                CheckSyntax();
                Console.WriteLine("**End**");
                Console.ReadLine();
            }
        }
        public static void CheckSyntax()
        {
            try
            {
                //Token Stack is empty
                if (_tokenStackPeek.tokenType == Terminals.dollar && _contextStack.Count == 0)
                {
                    Console.WriteLine("\nInput Stack reached end");
                    Console.WriteLine("\nParsing Completed");
                    Console.WriteLine("\nAbstract Syntax Tree\n");
                    rootDecl.Display(1, new GrammarValue());

                    Console.WriteLine("\n");
                    Console.WriteLine("Symbol Table\n");
                    foreach (Symbol symbl in symbolTable.GetSymbols())
                    {

                        Console.WriteLine(String.Format("Name:{0} Kind:{1} type:{2} link:{3} Symbol Table Name:{4} Size:{5}", symbl.Name, symbl.Kind, symbl.Type, symbl.link, symbl.st_Name, symbl.size));

                        if (symbl.link != null)
                        {
                            foreach (Symbol linkSymbol in symbl.link.GetSymbols())
                            {
                                Console.WriteLine(String.Format("Name:{0} Kind:{1} type:{2} link:{3} Symbol Table Name:{4} Size:{5}", linkSymbol.Name, linkSymbol.Kind, linkSymbol.Type, linkSymbol.link, linkSymbol.st_Name, linkSymbol.size));

                                if (linkSymbol.link != null)
                                {
                                    foreach (Symbol nestedLinkSymbol in linkSymbol.link.GetSymbols())
                                    {
                                        Console.WriteLine(String.Format("Name:{0} Kind:{1} type:{2} link:{3} Symbol Table Name:{4} Size:{5}", nestedLinkSymbol.Name, nestedLinkSymbol.Kind, nestedLinkSymbol.Type, nestedLinkSymbol.link, nestedLinkSymbol.st_Name, nestedLinkSymbol.size));

                                    }
                                }

                            }
                        }

                    }
                    Console.WriteLine("\n");
                    Console.WriteLine("Semantic Representation of Input:\n");

                    AssemblyCode();
                    foreach (String str in _semanticOutput)
                    {
                        Console.Write(str + " ");
                    }
                    Console.WriteLine("\n");
                    // Console.WriteLine("Errors:" + "\n");
                    //currentDataTypeOnLeft = "int";

                    var list = Lexical_Analyzer.errorDict.Keys.ToList();
                    list.Sort();
                    Console.WriteLine("Errors");
                    Console.WriteLine(String.Format("Line   Position   Message to User "));
                    foreach (KeyValuePair<string, List<string>> kp in Lexical_Analyzer.errorDict)
                    {
                        string dictkey = kp.Key;
                        string[] dictValue = kp.Value.ToArray();
                        string[] array = new string[100];
                        array = dictkey.Split(',');
                        Console.WriteLine(String.Format("Line:{0} Position:{1} Type&Error:{2}", array[0], array[1], dictValue[0]));
                    }

                }

                else if (flagForError == true && _grammarStackPeek.Substring(0, 1) == Terminals.hash)
                {
                    PopGrammarStack();
                    _contextStack.Pop();
                    CheckSyntax();
                }

                //Handling Semantic actions
                else if (_grammarStackPeek.Substring(0, 1) == Terminals.hash)
                {
                    switch (_grammarStackPeek)
                    {
                        //Start Class Declaration - Pushes Non Terminal to context Stack and pops semantic production from Grammar Stack
                        case Terminals.startClassDecl:
                            currentClassDecl = CreateNodeByName("ClassDecl");
                            currentMemberDeclList = CreateNodeByName("MembDeclList");
                            currentSymbol = new Symbol { Kind = "class", st_Name = "Global" };
                            _contextStack.Push("ClassDecl");
                            PopGrammarStack();
                            break;

                        //End Class Decl - Adds all child nodes of ClassDecl and other root nodes created and clears list
                        case Terminals.endClassDecl:
                            //classSize = varSize;
                            currentClassDecl.Add(currentMemberDeclList);
                            classList.Add(currentClassDecl);
                            _contextStack.Pop();
                            PopGrammarStack();
                            currentSymbol.link = varSymbolTable;
                            currentSymbol.size = classSize;
                            symbolTable.AddSymbol(currentSymbol);
                            varSymbolTable = new SymbolTable();

                            arraySize = 0;
                            varSize = 0;
                            classSize = 0;
                            break;

                        case Terminals.startDeclaration:
                            {
                                _contextStack.Push("StartDeclaration");
                                PopGrammarStack();
                                break;
                            }

                        case Terminals.endDeclaration:
                            {
                                _contextStack.Pop();
                                PopGrammarStack();
                                break;

                            }

                        case Terminals.startInheritanceList:
                            {
                                currentInheritanceList = CreateNodeByName("Inheritance");
                                Leaf leaf = new Leaf(_tokenStackPeek.tokenValue + "," +  _tokenStackPeek.tokenLine + "," + _tokenStackPeek.tokenPosition);
                                //Leaf leaf = new Leaf(_tokenStackPeek.tokenValue);
                                currentInheritanceList.Add(leaf);
                                currentClassDecl.Add(currentInheritanceList);
                                PopGrammarStack();
                                break;
                            }

                        case Terminals.startArraySize:
                            {
                                currentArraySizeList = CreateNodeByName("Dimensionlist");
                                _contextStack.Push("ArraySizeDecl");
                                PopGrammarStack();
                                break;

                            }

                        case Terminals.endArraySize:
                            {
                                _contextStack.Pop();
                                PopGrammarStack();
                                break;
                            }

                        //Start Variable Declaration - Pushes Non Terminal to context Stack and pops semantic production from Grammar Stack
                        case Terminals.startVarDecl:
                            {
                                currentVarDecl = CreateNodeByName("VarDecl");
                                PopGrammarStack();
                                i++;
                                currentVarSymbol = new Symbol { Kind = "variable" };
                                break;
                            }

                        //End Variable Declaration - Adds all child nodes to VarDecl root node and clears list
                        case Terminals.endVarDecl:
                            PopGrammarStack();
                            if (errorInDecl)
                            {
                                _declChildList.Insert(1, "#error");
                                errorInDecl = false;

                            }
                            if (flagForFuncDecl == false)
                            {
                                foreach (String str in _declChildList)
                                {
                                    Leaf leaf = new Leaf(str);
                                    currentVarDecl.Add(leaf);

                                }
                                String classNameAndVariableName = null;
                                if (currentFuncDef == null && checkProgram)
                                {
                                    classNameAndVariableName = currentClassName + "," + _declChildList.ElementAt(1);
                                }

                                else
                                {
                                    classNameAndVariableName = currentFuncName + "," + _declChildList.ElementAt(1);
                                }
                                if (_dataTypeAndVariablePairToCheck.ContainsKey(classNameAndVariableName))
                                {
                                    String dictkey = (_tokenStackPeek.tokenLine + "," + _tokenStackPeek.tokenPosition);
                                    List<String> dictValue = new List<string>();
                                    dictValue.Add("Sematic Error" + "," + "Multiple variable declaration in same scope:" + currentFuncName + currentClassName);
                                    Lexical_Analyzer.errorDict.Add(dictkey, dictValue);
                                }
                                else
                                {
                                    _dataTypeAndVariablePairToCheck.Add(classNameAndVariableName, _declChildList.ElementAt(0));
                                }


                                if (!_declChildList.ElementAt(1).Contains("#")) varSize = varSize + 4;

                                if (flagForArraySize)
                                {
                                    currentVarDecl.Add(currentArraySizeList);
                                    varSize = 4 * (arraySize + 1);
                                    flagForArraySize = false;
                                }

                                StringBuilder builder = new StringBuilder();
                                builder.Append(_declChildList.ElementAt(0));

                                if (_arrayListAddToSym.Count > 0)
                                {
                                    foreach (int i in _arrayListAddToSym)
                                    {
                                        string concat = "[" + i + "]";
                                        builder.Append(concat);
                                    }
                                }

                                currentVarSymbol.Name = _declChildList.ElementAt(1);
                                currentVarSymbol.Type = builder.ToString();
                                currentVarSymbol.st_Name = currentClassName;
                                currentVarSymbol.size = varSize;
                                currentDataTypeOnLeft = _declChildList.ElementAt(0);

                                _arrayListAddToSym.Clear();

                                if (currentVarSymbol != null && flagForVarFuncDef == false)
                                {
                                    currentMemberDeclList.Add(currentVarDecl);
                                    varSymbolTable.AddSymbol(currentVarSymbol);
                                }
                                if (currentFuncBody != null)
                                {
                                    currentFuncBody.Add(currentVarDecl);

                                }
                                _declChildList.Clear();
                                if (currentFuncDef != null)
                                {
                                    varSymbolTable.AddSymbol(currentVarSymbol);
                                    currentVarSymbol.st_Name = currentFuncName;
                                    _funcDefListAddToSym.Clear();
                                }
                                //Size for Program
                                if (!checkProgram)
                                {
                                    programFuncSize = programFuncSize + varSize;
                                    currentProgramSymbol.size = programFuncSize;
                                }

                            }
                            flagForFuncDecl = false;
                            classSize = classSize + varSize;
                            if (currentFuncDefSymbol != null && checkProgram)
                            {
                                funcSize = funcSize + varSize;
                            }
                            varSize = 0;
                            break;

                        //Start Function Declaration - Pushes Non Terminal to context Stack and pops semantic production from Grammar Stack
                        case Terminals.startFuncDecl:
                            {
                                flagForFuncDecl = true;
                                currentFuncDecl = CreateNodeByName("FuncDecl");
                                currentFunSymbol = new Symbol { Kind = "function" };
                                _contextStack.Push("FuncDecl");
                                PopGrammarStack();
                                break;
                            }

                        case Terminals.endFuncDecl:
                            {
                                _contextStack.Pop();
                                PopGrammarStack();
                                foreach (String str in _declChildList)
                                {
                                    Leaf leaf = new Leaf(str);
                                    currentFuncDecl.Add(leaf);
                                }
                                StringBuilder strBuilderFuncDecl = new StringBuilder();
                                strBuilderFuncDecl.Append(_declChildList.ElementAt(0) + ":");
                                currentFunSymbol.Name = _declChildList.ElementAt(1);
                                strBuilderFuncDecl.Append(_paramListAddToSym.ElementAt(0));
                                foreach (int i in _arrayListAddToSym)
                                {
                                    paramSize = paramSize + 4 * i;
                                    strBuilderFuncDecl.Append("[" + i + "]");
                                }
                                currentFuncParamSymbol.size = paramSize;
                                currentFuncParamSymbol.Type = _paramListAddToSym.ElementAt(0);
                                currentFunSymbol.size = paramSize;
                                currentFunSymbol.Type = strBuilderFuncDecl.ToString();
                                currentFunSymbol.st_Name = currentClassName;
                                currentFuncParamSymbol.st_Name = _declChildList.ElementAt(1);
                                currentFuncDecl.Add(currentParamDecl);
                                currentMemberDeclList.Add(currentFuncDecl);
                                currentFuncParamSymbol.Name = _paramListAddToSym.ElementAt(1);
                                varFuncParamSymbolTable.AddSymbol(currentFuncParamSymbol);
                                currentFunSymbol.link = varFuncParamSymbolTable;
                                varSymbolTable.AddSymbol(currentFunSymbol);
                                countFuncDecl = 0;
                                countFuncParamDecl = 0;
                                flagForVarFuncDef = false;
                                _declChildList.Clear();
                                paramSize = 0;
                                _paramListAddToSym.Clear();
                                _arrayListAddToSym.Clear();
                                break;
                            }

                        case Terminals.startParamDecl:
                            {
                                flagForParamVisit = true;
                                currentParamDecl = CreateNodeByName("ParamList");
                                _contextStack.Push("ParamDecl");
                                PopGrammarStack();
                                currentFuncParamSymbol = new Symbol { Kind = "parameter" };
                                break;
                            }

                        case Terminals.endParamDecl:
                            {
                                _contextStack.Pop();
                                PopGrammarStack();

                                currentParamDecl.Add(currentArraySizeList);
                                paramSize = 4 * arraySize;
                                if (flagForVarFuncDef == true && _paramListAddToSym.Count > 0)
                                {
                                    currentFuncParamSymbol.Type = _paramListAddToSym.ElementAt(0);
                                    currentFuncParamSymbol.Name = _paramListAddToSym.ElementAt(1);
                                    currentFuncParamSymbol.st_Name = currentFuncName;

                                    //if (_dataTypeAndVariablePairToCheck.ContainsKey(currentFuncName + "," + _paramListAddToSym.ElementAt(1)))
                                    //{
                                    //    Console.WriteLine("Error");
                                    //}
                                    //else
                                    //{
                                    //    _dataTypeAndVariablePairToCheck.Add(currentFuncName + "," + _paramListAddToSym.ElementAt(1), _paramListAddToSym.ElementAt(0));
                                    //}
                                }
                                paramSize = 0;
                                flagForParamVisit = false;
                                break;

                            }

                        case Terminals.startFuncDef:
                            {
                                flagForVarFuncDef = true;
                                currentFuncDef = CreateNodeByName("FuncDef");
                                _contextStack.Push("FuncDef");
                                currentFuncDefSymbol = new Symbol { Kind = "function", st_Name = "Global" };
                                PopGrammarStack();
                                break;
                            }

                        case Terminals.endFuncDef:
                            {
                                _contextStack.Pop();
                                PopGrammarStack();
                                currentFuncDef.Add(currentParamDecl);
                                funcDefList.Add(currentFuncDef);
                                currentFuncDefSymbol.Type = _funcDefListAddToSym.ElementAt(0);
                                currentFuncDefSymbol.Name = _funcDefListAddToSym.ElementAt(1);
                                currentFuncDefSymbol.link = varSymbolTable;
                                if (currentFuncParamSymbol.Name != null) varSymbolTable.AddSymbol(currentFuncParamSymbol);
                                symbolTable.AddSymbol(currentFuncDefSymbol);
                                varSymbolTable = new SymbolTable();
                                break;
                            }

                        case Terminals.startProgram:
                            {

                                currentFuncName = "program";
                                currentstatBlockProg = CreateNodeByName("stat Block Prog");
                                checkProgram = false;
                                PopGrammarStack();
                                currentProgramSymbol = new Symbol { Name = "program", Kind = "function", st_Name = "Global" };
                                break;
                            }

                        case Terminals.endProgram:
                            {

                                PopGrammarStack();
                                statBlock.Add(currentFuncBody);
                                //varFuncParamSymbolTable.AddSymbol(currentFuncParamSymbol);
                                //currentVarSymbol.link = varFuncParamSymbolTable;
                                currentProgramSymbol.link = varSymbolTable;
                                symbolTable.AddSymbol(currentProgramSymbol);
                                checkProgram = true;
                                varSize = 0;
                                break;
                            }

                        case Terminals.startFuncBody:
                            {

                                currentFuncBody = CreateNodeByName("stat Block");
                                PopGrammarStack();

                                _contextStack.Push("FuncBody");
                                break;
                            }

                        case Terminals.endFuncBody:
                            {
                                if (currentFuncDefSymbol != null && checkProgram)
                                {
                                    currentFuncDefSymbol.size = funcSize;

                                }
                                if (currentFuncDef != null && checkProgram == true)
                                    currentFuncDef.Add(currentFuncBody);
                                PopGrammarStack();
                                _contextStack.Pop();
                                currentAssignStat = null;
                                varSize = 0;
                                funcSize = 0;
                                //currentAssignStat = null;
                                break;
                            }

                        case Terminals.startAssignStat:
                            {
                                currentAssignStat = CreateNodeByName("AssignStat");
                                _contextStack.Push("AssignStat");
                                PopGrammarStack();
                                break;
                            }

                        case Terminals.endAssignStat:
                            {
                                _contextStack.Pop();
                                PopGrammarStack();
                                if (currentAssignStat != null && currentFuncBody != null)
                                    currentFuncBody.Add(currentAssignStat);
                                visitEqualto = false;
                                break;
                            }

                        case Terminals.startStatBlock:
                            {
                                currentStatBlock = CreateNodeByName("StatBlock");
                                PopGrammarStack();
                                _contextStack.Push("StatBlock");
                                break;
                            }
                        case Terminals.endStatBlock:
                            {
                                PopGrammarStack();
                                _contextStack.Pop();
                                break;
                            }

                        case Terminals.startReturn:
                            {
                                currentReturnBlock = CreateNodeByName("ReturnBlock");
                                _contextStack.Push("ReturnBlock");
                                PopGrammarStack();
                                break;
                            }
                        case Terminals.endReturn:
                            {
                                PopGrammarStack();
                                _contextStack.Pop();
                                if (checkProgram == true && currentFuncBody != null)
                                    currentFuncBody.Add(currentReturnBlock);
                                break;
                            }


                        case Terminals.startClassObjDecl:
                            {
                                currentClassObjDecl = CreateNodeByName("ClassObjDecl");
                                // _contextStack.Push("ReturnBlock");
                                PopGrammarStack();
                                break;
                            }


                        case Terminals.endClassObjDecl:
                            {
                                PopGrammarStack();
                                // _contextStack.Pop();
                                break;
                            }
                        //Makes node for peeks of Value Stack(Semantic values)
                        case Terminals.makeNode:
                            {
                                if (_contextStack.Peek().Equals("ReturnBlock"))
                                {
                                    Leaf leaf = new Leaf(_tokenStackPeek.tokenValue);
                                    currentReturnBlock.Add(leaf);
                                }



                                if (_contextStack.Peek().Equals("ClassDecl"))
                                {
                                    Leaf leaf = new Leaf(_tokenStackPeek.tokenValue);
                                    currentClassDecl.Add(leaf);
                                    currentSymbol.Name = _tokenStackPeek.tokenValue;
                                    currentClassName = _tokenStackPeek.tokenValue;
                                }
                                if (_contextStack.Peek().Equals("ArraySizeDecl"))
                                {
                                    flagForArraySize = true;
                                    _arrayListAddToSym.Add(Convert.ToInt32(_tokenStackPeek.tokenValue));
                                    Leaf leaf = new Leaf(_tokenStackPeek.tokenValue);
                                    currentArraySizeList.Add(leaf);
                                    arraySize = Convert.ToInt32(_tokenStackPeek.tokenValue);
                                }

                                if (_contextStack.Peek().Equals("StartDeclaration"))
                                {
                                    _declChildList.Add(_tokenStackPeek.tokenValue);
                                }


                                if (_contextStack.Peek().Equals("FuncDecl"))
                                {
                                    countFuncDecl++;
                                    Leaf leaf = new Leaf(_tokenStackPeek.tokenValue);
                                    currentFuncDecl.Add(leaf);
                                    if (countFuncDecl == 1)
                                    {
                                        currentFunSymbol.Type = _tokenStackPeek.tokenValue;
                                    }
                                    else if (countFuncDecl == 2)
                                    {
                                        currentFunSymbol.Name = _tokenStackPeek.tokenValue;
                                    }
                                }

                                if (_contextStack.Peek().Equals("ParamDecl"))
                                {
                                    countFuncParamDecl++;
                                    Leaf leaf = new Leaf(_tokenStackPeek.tokenValue);
                                    currentParamDecl.Add(leaf);
                                    _paramListAddToSym.Add(_tokenStackPeek.tokenValue);

                                }

                                if (_contextStack.Peek().Equals("FuncDef"))
                                {
                                    countFuncDef++;
                                    Leaf leaf = new Leaf(_tokenStackPeek.tokenValue);
                                    currentFuncDef.Add(leaf);
                                    _funcDefListAddToSym.Add(_tokenStackPeek.tokenValue);
                                    if (_funcDefListAddToSym.Count > 1 && _funcDefListAddToSym.Count <= 2)
                                        currentFuncName = _funcDefListAddToSym.ElementAt(1);
                                    else if (_funcDefListAddToSym.Count > 2)
                                        currentFuncName = _funcDefListAddToSym.ElementAt(2);
                                }

                                if (_contextStack.Peek().Equals("AssignStat"))
                                {

                                    if (visitEqualto == true)
                                    {
                                        if (operators.ContainsKey(_tokenStackPeek.tokenValue))
                                        {
                                            operatorToAdd = _tokenStackPeek.tokenValue;
                                        }
                                        else
                                        {
                                            if (checkProgram == false) assignlistToFormAssemble.Add(_tokenStackPeek.tokenValue);
                                        }
                                    }

                                    if (_tokenStackPeek.tokenValue == "=")
                                    {
                                        Leaf leaf = new Leaf("AssignOp");
                                        currentAssignStat.Add(leaf);
                                        flagForAssignRight = true;
                                        visitEqualto = true;
                                    }
                                    else if (_tokenStackPeek.tokenType == "intNum")
                                    {
                                        if (checkProgram == true)
                                        {
                                            valueinFunc = Convert.ToInt32(_tokenStackPeek.tokenValue);
                                        }
                                        Leaf leaf = new Leaf(_tokenStackPeek.tokenType);
                                        currentAssignStat.Add(leaf);
                                    }
                                    else if (_tokenStackPeek.tokenType == "floatNum")
                                    {
                                        Leaf leaf = new Leaf(_tokenStackPeek.tokenType);
                                        currentAssignStat.Add(leaf);
                                        if (flagForAssignRight == true)
                                        {
                                            currentDataTypeOnRight = "float";
                                            flagForAssignRight = false;
                                        }
                                    }
                                    else
                                    {
                                        Leaf leaf = new Leaf(_tokenStackPeek.tokenValue);
                                        currentAssignStat.Add(leaf);
                                    }
                                    countAssignStat++;

                                }
                                PopGrammarStack();
                                break;

                            }
                        default:
                            break;
                    }
                    CheckSyntax();
                }

                //Pop from Grammar and Token Stack if both are terminals
                else if (_terminalMaster.Contains(_grammarStackPeek) && _grammarStackPeek.Substring(0, 1) != Terminals.hash)
                {
                    if (_tokenStackPeek.tokenType == _grammarStackPeek || _tokenStackPeek.tokenType.Contains("#error"))
                    {
                        countdebug++;
                        if (countdebug == 168)
                        {
                            Console.WriteLine("d");
                        }
                        if (flagForError == true) flagForError = false;
                        PopTokenStack();
                        AddToSemanticList();
                        PopGrammarStack();
                        CheckSyntax();
                    }
                    else
                    {
                        flagForError = true;
                        errorInDecl = true;
                        String dictkey = (_tokenStackPeek.tokenLine + "," + _tokenStackPeek.tokenPosition).ToString();
                        List<String> dictValue = new List<string>();
                        dictValue.Add("Syntactical Error" + "," + "Invalid Syntax in:" + _currentNonTerminal + "declaration");
                        Lexical_Analyzer.errorDict.Add(dictkey, dictValue);
                        HandleError();

                    }
                }

                //If grammar Stack peek is non terminal
                else if (_nonTerminalMaster.Contains(_grammarStackPeek))
                {
                    IterateGrammarStackPeekUntilTerminal();
                    CheckSyntax();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception:" + ex.ToString());
            }
      
            }//Build Token Stack
       
        public static void PopulateTokenStackFromTokens(List<Tokens> listOfObjects)
        {
            List<TokenstoParse> tokensToParseList = new List<TokenstoParse>();
            foreach (Tokens c in listOfObjects)
            {
                TokenstoParse objtokenToCheck = new TokenstoParse();
                if (c.Value == null)
                {
                    objtokenToCheck.tokenType = "$";
                    objtokenToCheck.tokenValue = "$";
                    tokensToParseList.Add(objtokenToCheck);
                    break;
                }
                if ((c.Type.ToString() == "Identifier" && c.Value.ToString() != null) || c.Type.ToString() == "InvalidId")
                {
                    objtokenToCheck.tokenType = "id";
                }
                else if (c.Type.ToString() == "Integer" || c.Type.ToString() == "InvalidNumber")
                {
                    objtokenToCheck.tokenType = "intNum";
                }
                else if (c.Type.ToString() == "Float")
                {
                    objtokenToCheck.tokenType = "floatNum";
                }
                else if (c.Type.ToString() == "Op_equal")
                {
                    objtokenToCheck.tokenType = "+";
                }
                
                else
                {
                    objtokenToCheck.tokenType = c.Value;
                }

                objtokenToCheck.tokenValue = c.Value;
                objtokenToCheck.tokenLine = c.Line;
                objtokenToCheck.tokenPosition = c.Position;
                tokensToParseList.Add(objtokenToCheck);

            }
            //Put Tokens in token_Stack 
            tokensToParseList.Reverse();
            foreach (TokenstoParse tok in tokensToParseList)
            {
                tokenStackToParse.Push(tok);
            }
        }

        //Get Productions and iterate intil top of Grammar Stack is terminal
        public static void IterateGrammarStackPeekUntilTerminal()
        {
            _grammarStackPeek = _grammarStack.Peek();
            //iterate until _grammarStackPeek becomes Terminal

            if (_grammarStackPeek == "") //Epsilon
            {
                //RemoveNode(_currentNonTerminal);
                //need to remove non terminal
                PopGrammarStack();
            }

            else if (!_terminalMaster.Contains(_grammarStackPeek))
            {
                //to form key
                String mainMasterKey = null;
                _currentNonTerminal = _grammarStackPeek;

                if (_specialCharacterMaster.ContainsKey(_tokenStackPeek.tokenType))
                {
                    mainMasterKey = string.Format("{0}_{1}", _grammarStackPeek, _specialCharacterMaster[_tokenStackPeek.tokenType]);
                    PopGrammarStack(); //Remove the peek Non-Terminal from Grammar, replacing its actual productions

                    var productions = GetProductionByKey(mainMasterKey);

                    _grammarStack.PushRange(productions);
                    //Add Next Nodes - If condition - For handling Epsilon (Not replacing already exist nodes)
                    //if (!(productions.First() is "")) PutNode(_currentNonTerminal, productions);

                    IterateGrammarStackPeekUntilTerminal();
                }
                else
                {
                    //Error Handling - Table driven process error recovery method
                    HandleError();
                }

            }
        }
        public static List<string> GetProductionByKey(string mainMasterKey)
        {
            var productionKey = _mainMaster[mainMasterKey];

            return _productionMaster[productionKey];
        }
        public static void PopTokenStack()
        {
            if (tokenStackToParse.Count > 0) tokenStackToParse.Pop();
            _tokenStackPeek = tokenStackToParse.Peek();
            // PrintTokenStack();
        }
        public static void AddToSemanticList()
        {
            if (tokenStackToParse.Count > 0)
            {
                _semanticOutput.Add(_tokenStackPeek.tokenValue);
            }
        }
        //Error Handling
        public static void HandleError()
        {
            Console.WriteLine("*****Error in " + _currentNonTerminal + " syntax*****");
            foreach (String str in _followMaster[_currentNonTerminal])
            {
                if (_tokenStackPeek.tokenValue.Equals(str))
                {
                    while (_terminalMaster.Contains(_grammarStackPeek)) { PopGrammarStack(); } //Grammar Stack completely need to be removed
                    CheckSyntax();
                }
                else
                {
                    PopTokenStack();
                }
            }
        }
        public static void PopGrammarStack()
        {
            if (_grammarStack.Count > 1) _grammarStack.Pop();
            _grammarStackPeek = _grammarStack.Peek();
            // PrintGrammarStack();
        }
        //private methods
        private static void PopulateSpecialCharacterMaster()
        {
            _specialCharacterMaster.Add(Terminals.curlieOpen, "curlieOpen");
            _specialCharacterMaster.Add(Terminals.className, "class");
            _specialCharacterMaster.Add(Terminals.intDT, "int");
            _specialCharacterMaster.Add(Terminals.semicolon, "semicolon");
            _specialCharacterMaster.Add(Terminals.curlieClose, "curlieClose");
            _specialCharacterMaster.Add(Terminals.paranthesisOpen, "paranthesisOpen");
            _specialCharacterMaster.Add(Terminals.paranthesisClose, "paranthesisClose");
            _specialCharacterMaster.Add(Terminals.floatDT, "float");
            _specialCharacterMaster.Add(Terminals.id, "id");
            _specialCharacterMaster.Add(Terminals.colon, "colon");
            _specialCharacterMaster.Add(Terminals.bracketOpen, "bracketOpen");
            _specialCharacterMaster.Add(Terminals.equalTo, "equalTo");
            _specialCharacterMaster.Add(Terminals.program, "program");
            _specialCharacterMaster.Add(Terminals.returnValue, "return");
            _specialCharacterMaster.Add(Terminals.ifValue, "if");
            _specialCharacterMaster.Add(Terminals.forValue, "for");
            _specialCharacterMaster.Add(Terminals.getValue, "get");
            _specialCharacterMaster.Add(Terminals.putValue, "put");
            _specialCharacterMaster.Add(Terminals.sr, "sr");
            _specialCharacterMaster.Add(Terminals.comma, "comma");
            _specialCharacterMaster.Add(Terminals.dot, "dot");
            _specialCharacterMaster.Add(Terminals.greaterThan, "greaterThan");
            _specialCharacterMaster.Add(Terminals.lessThan, "lessThan");
            _specialCharacterMaster.Add(Terminals.intNum, "intNum");
            _specialCharacterMaster.Add(Terminals.floatNum, "floatNum");
            _specialCharacterMaster.Add(Terminals.bracketClose, "bracketClose");
            _specialCharacterMaster.Add(Terminals.plus, "plus");
            _specialCharacterMaster.Add(Terminals.minus, "minus");
            _specialCharacterMaster.Add(Terminals.lessThanEqualTo, "lessThanEqualTo");
            _specialCharacterMaster.Add(Terminals.greaterThanEqualTo, "greaterThanEqualTo");
            _specialCharacterMaster.Add(Terminals.product, "product");
        }
        private static void PopulateNonTerminalMaster()
        {
            _nonTerminalMaster = new List<string>
            {
                NonTerminals.prog,
                NonTerminals.classDecl,
                NonTerminals.classDecloptional,
                NonTerminals.classDeclRep,
                NonTerminals.funcDeclRep,
                NonTerminals.funcBody,
                NonTerminals.varDeclRep,
                NonTerminals.funcDefRep,
                NonTerminals.varDeclRep,
                NonTerminals.varDecl,
                NonTerminals.type,
                NonTerminals.funcDecl,
                NonTerminals.fParams,
                NonTerminals.classDeclid,
                NonTerminals.classDeclidRep,
                NonTerminals.assignOp,
                NonTerminals.sign,
                NonTerminals.relOp,
                NonTerminals.addOp,
                NonTerminals.multOp,
                NonTerminals.arraySize,
                NonTerminals.fParamsTail,
                NonTerminals.arraySizeRep,
                NonTerminals.fParamsTailRep,
                NonTerminals.variable,
                NonTerminals.idnestrep,
                NonTerminals.indicerep,
                NonTerminals.functionCall,
                NonTerminals.idnest,
                NonTerminals.idnestTail,
                NonTerminals.indice,
                NonTerminals.arithExpr,
                NonTerminals.arithExpr2,
                NonTerminals.term,
                NonTerminals.term2,
                NonTerminals.factor,
                NonTerminals.variabletail,
                NonTerminals.aParamsTail,
                NonTerminals.expr,
                NonTerminals.expr1,
                NonTerminals.relExpr,
                NonTerminals.aParams,
                NonTerminals.aParamsTailRep,
                NonTerminals.assignStat,
                NonTerminals.statBlock,
                NonTerminals.statementRep,
                NonTerminals.statement,
                NonTerminals.funcDef,
                NonTerminals.funcHead,
                NonTerminals.funcHeadTail1,
                NonTerminals.funcHeadTail2,
                NonTerminals.returnStat,
                NonTerminals.freeFuncDef,
                NonTerminals.funcBodyNew,
                NonTerminals.classobjDecl,
                NonTerminals.classobjDeclRep,
                NonTerminals.relExpr1,
                NonTerminals.exprDT,
                NonTerminals.term3
    };
        }
        private static void PopulateTerminalMaster()
        {
            _terminalMaster = new List<string>
            {
                Terminals.className,
                Terminals.id,
                Terminals.colon,
                Terminals.curlieOpen,
                Terminals.curlieClose,
                Terminals.program,
                Terminals.semicolon,
                Terminals.intDT,
                Terminals.floatDT,
                Terminals.epsilon,
                Terminals.dollar,
                Terminals.paranthesisOpen,
                Terminals.paranthesisClose,
                Terminals.comma,
                Terminals.equalTo,
                Terminals.plus,
                Terminals.minus,
                Terminals.equalCheck,
                Terminals.notEqualCheck,
                Terminals.lessThan,
                Terminals.greaterThan,
                Terminals.lessThanEqualTo,
                Terminals.greaterThanEqualTo,
                Terminals.or,
                Terminals.product,
                Terminals.divide,
                Terminals.and,
                Terminals.bracketOpen,
                Terminals.bracketClose,
                Terminals.sr,
                Terminals.intNum,
                Terminals.floatNum,
                Terminals.startClassDecl,
                Terminals.endClassDecl,
                Terminals.startVarDecl,
                Terminals.endVarDecl,
                Terminals.makeNode,
                Terminals.startFuncDecl,
                Terminals.endFuncDecl,
                Terminals.startParamDecl,
                Terminals.endParamDecl,
                Terminals.startAssignStat,
                Terminals.startFuncDef,
                Terminals.endAssignStat,
                Terminals.endFuncDecl,
                Terminals.returnValue,
                Terminals.startFuncBody,
                Terminals.endFuncBody,
                Terminals.startProgram,
                Terminals.ifValue,
                Terminals.forValue,
                Terminals.getValue,
                Terminals.putValue,
                Terminals.then,
                Terminals.elseValue,
                Terminals.startInheritanceList,
                Terminals.endInheritanceList,
                Terminals.startArraySize,
                Terminals.endArraySize,
                Terminals.startDeclaration,
                Terminals.endDeclaration,
                Terminals.dot,
                Terminals.startStatBlock,
                Terminals.endStatBlock,
                Terminals.startIf,
                Terminals.endIf,
                Terminals.startFor,
                Terminals.endFor,
                Terminals.startExpr,
                Terminals.endExpr,
                Terminals.endProgram,
                Terminals.startClassObjDecl,
                Terminals.endClassObjDecl,
                Terminals.startReturn,
                Terminals.endReturn
    };
        }
        private static void PopulateMainMaster()
        {
            _mainMaster.Add(MainMasterKeys.prog_class, ProductionMasterKeys.Number57);
            _mainMaster.Add(MainMasterKeys.classDeclRep_class, ProductionMasterKeys.Number58);
            _mainMaster.Add(MainMasterKeys.classDeclRep_program, ProductionMasterKeys.Number60);
            _mainMaster.Add(MainMasterKeys.classDeclRep_int, ProductionMasterKeys.Number60);
            _mainMaster.Add(MainMasterKeys.classDeclRep_float, ProductionMasterKeys.Number60);
            _mainMaster.Add(MainMasterKeys.classDeclRep_id, ProductionMasterKeys.Number60);
            _mainMaster.Add(MainMasterKeys.classDecl_class, ProductionMasterKeys.Number1);
            _mainMaster.Add(MainMasterKeys.classDecloptional_curlieOpen, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.varDeclRep_int, ProductionMasterKeys.Number50);
            _mainMaster.Add(MainMasterKeys.varDeclRep_float, ProductionMasterKeys.Number50);
            _mainMaster.Add(MainMasterKeys.varDeclRep_id, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.varDecl_int, ProductionMasterKeys.Number43);
            _mainMaster.Add(MainMasterKeys.varDecl_float, ProductionMasterKeys.Number43);
            _mainMaster.Add(MainMasterKeys.varDecl_id, ProductionMasterKeys.Number81);
            _mainMaster.Add(MainMasterKeys.type_int, ProductionMasterKeys.Number12);
            _mainMaster.Add(MainMasterKeys.type_float, ProductionMasterKeys.Number70);
            _mainMaster.Add(MainMasterKeys.varDeclRep_semicolon, ProductionMasterKeys.Number69);
            _mainMaster.Add(MainMasterKeys.varDeclRep_curlieClose, ProductionMasterKeys.Number69);
            _mainMaster.Add(MainMasterKeys.funcDeclRep_curlieClose, ProductionMasterKeys.Number63);
            _mainMaster.Add(MainMasterKeys.funcDeclRep_int, ProductionMasterKeys.Number6);
            _mainMaster.Add(MainMasterKeys.funcDeclRep_float, ProductionMasterKeys.Number6);
            _mainMaster.Add(MainMasterKeys.funcDeclRep_id, ProductionMasterKeys.Number6);
            _mainMaster.Add(MainMasterKeys.funcDecl_int, ProductionMasterKeys.Number52);
            _mainMaster.Add(MainMasterKeys.funcDecl_float, ProductionMasterKeys.Number52);
            _mainMaster.Add(MainMasterKeys.funcDecl_id, ProductionMasterKeys.Number52);
            _mainMaster.Add(MainMasterKeys.fParams_int, ProductionMasterKeys.Number16);
            _mainMaster.Add(MainMasterKeys.fParams_float, ProductionMasterKeys.Number16);
            _mainMaster.Add(MainMasterKeys.fParams_id, ProductionMasterKeys.Number16);
            _mainMaster.Add(MainMasterKeys.funcDefRep_int, ProductionMasterKeys.Number59);
            _mainMaster.Add(MainMasterKeys.funcDefRep_float, ProductionMasterKeys.Number59);
            _mainMaster.Add(MainMasterKeys.funcHead_int, ProductionMasterKeys.Number53);
            _mainMaster.Add(MainMasterKeys.funcHead_float, ProductionMasterKeys.Number53);
            _mainMaster.Add(MainMasterKeys.funcHead_id, ProductionMasterKeys.Number53);
            _mainMaster.Add(MainMasterKeys.funcHeadTail1_paranthesisOpen, ProductionMasterKeys.Number54);//epsilon
            _mainMaster.Add(MainMasterKeys.funcHeadTail1_sr, ProductionMasterKeys.Number55);//sr
            _mainMaster.Add(MainMasterKeys.funcHeadTail2_paranthesisOpen, ProductionMasterKeys.Number56);
            _mainMaster.Add(MainMasterKeys.funcBody_curlieOpen, ProductionMasterKeys.Number49);
            _mainMaster.Add(MainMasterKeys.funcBody_semicolon, ProductionMasterKeys.Number71);
            _mainMaster.Add(MainMasterKeys.classDecloptional_colon, ProductionMasterKeys.Number3);
            _mainMaster.Add(MainMasterKeys.classDeclidRep_curlieOpen, ProductionMasterKeys.Number72);
            _mainMaster.Add(MainMasterKeys.arraySizeRep_bracketOpen, ProductionMasterKeys.Number15);
            _mainMaster.Add(MainMasterKeys.arraySize_bracketOpen, ProductionMasterKeys.Number13);
            _mainMaster.Add(MainMasterKeys.arraySize_bracketClose, ProductionMasterKeys.Number73);
            _mainMaster.Add(MainMasterKeys.arraySizeRep_semicolon, ProductionMasterKeys.Number74);
            _mainMaster.Add(MainMasterKeys.statementRep_id, ProductionMasterKeys.Number47);
            _mainMaster.Add(MainMasterKeys.statementRep_if, ProductionMasterKeys.Number47);
            _mainMaster.Add(MainMasterKeys.statementRep_for, ProductionMasterKeys.Number47);
            _mainMaster.Add(MainMasterKeys.statementRep_get, ProductionMasterKeys.Number47);
            _mainMaster.Add(MainMasterKeys.statementRep_put, ProductionMasterKeys.Number47);
            _mainMaster.Add(MainMasterKeys.statementRep_return, ProductionMasterKeys.Number47);
            _mainMaster.Add(MainMasterKeys.statementRep_paranthesisOpen, ProductionMasterKeys.Number47);
            _mainMaster.Add(MainMasterKeys.statement_id, ProductionMasterKeys.Number48);
            _mainMaster.Add(MainMasterKeys.assignStat_id, ProductionMasterKeys.Number44);
            _mainMaster.Add(MainMasterKeys.expr_paranthesisOpen, ProductionMasterKeys.Number119);
            _mainMaster.Add(MainMasterKeys.assignStat_semicolon, ProductionMasterKeys.Number77);
            _mainMaster.Add(MainMasterKeys.statementRep_curlieClose, ProductionMasterKeys.Number75);
            _mainMaster.Add(MainMasterKeys.statement_semicolon, ProductionMasterKeys.Number76);
            _mainMaster.Add(MainMasterKeys.type_id, ProductionMasterKeys.Number78);
            _mainMaster.Add(MainMasterKeys.funcDefRep_program, ProductionMasterKeys.Number80);
            //_mainMaster.Add(MainMasterKeys.assignStat_int, ProductionMasterKeys.Number44);
            _mainMaster.Add(MainMasterKeys.returnStat_return, ProductionMasterKeys.Number101);
            _mainMaster.Add(MainMasterKeys.returnStat_curlieClose, ProductionMasterKeys.Number102);
            _mainMaster.Add(MainMasterKeys.arraySizeRep_paranthesisOpen, ProductionMasterKeys.Number103);
            _mainMaster.Add(MainMasterKeys.freeFuncDef_float, ProductionMasterKeys.Number206);
            _mainMaster.Add(MainMasterKeys.funcBodyNew_curlieOpen, ProductionMasterKeys.Number300);
            _mainMaster.Add(MainMasterKeys.classobjDeclRep_id, ProductionMasterKeys.Number301);
            _mainMaster.Add(MainMasterKeys.classobjDecl_id, ProductionMasterKeys.Number302);
            _mainMaster.Add(MainMasterKeys.classobjDeclRep_for, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.funcDef_int, ProductionMasterKeys.Number51);
            _mainMaster.Add(MainMasterKeys.funcDef_float, ProductionMasterKeys.Number51);
            _mainMaster.Add(MainMasterKeys.funcDef_id, ProductionMasterKeys.Number51);
            _mainMaster.Add(MainMasterKeys.funcHead_curlieOpen, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.fParamsTailRep_comma, ProductionMasterKeys.Number18);
            _mainMaster.Add(MainMasterKeys.fParamsTail_comma, ProductionMasterKeys.Number14);
            _mainMaster.Add(MainMasterKeys.arraySizeRep_comma, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.fParamsTailRep_paranthesisClose, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.arraySizeRep_paranthesisClose, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.variable_id, ProductionMasterKeys.Number20);
            _mainMaster.Add(MainMasterKeys.variable_equalTo, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.idnest_equalTo, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.idnest_dot, ProductionMasterKeys.Number82);
            _mainMaster.Add(MainMasterKeys.idnest_bracketOpen, ProductionMasterKeys.Number84);
            _mainMaster.Add(MainMasterKeys.indicerep_equalTo, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.idnestrep_lessThan, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.indicerep_bracketOpen, ProductionMasterKeys.Number83);
            _mainMaster.Add(MainMasterKeys.indice_bracketOpen, ProductionMasterKeys.Number85);
            _mainMaster.Add(MainMasterKeys.idnest_paranthesisOpen, ProductionMasterKeys.Number86);
            _mainMaster.Add(MainMasterKeys.idnestrep_dot, ProductionMasterKeys.Number87);
            _mainMaster.Add(MainMasterKeys.idnestrep_equalTo, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.expr_id, ProductionMasterKeys.Number36);
            _mainMaster.Add(MainMasterKeys.expr1_intNum, ProductionMasterKeys.Number37);
            _mainMaster.Add(MainMasterKeys.expr1_equalTo, ProductionMasterKeys.Number37);
            _mainMaster.Add(MainMasterKeys.expr1_greaterThan, ProductionMasterKeys.Number37);
            _mainMaster.Add(MainMasterKeys.expr1_greaterThanEqualTo, ProductionMasterKeys.Number37);
            _mainMaster.Add(MainMasterKeys.expr1_lessThan, ProductionMasterKeys.Number37);
            _mainMaster.Add(MainMasterKeys.expr1_lessThanEqualTo, ProductionMasterKeys.Number37);
            _mainMaster.Add(MainMasterKeys.expr1_paranthesisClose, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.expr1_semicolon, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.expr1_comma, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.relExpr_id, ProductionMasterKeys.Number39);
            _mainMaster.Add(MainMasterKeys.aParams_id, ProductionMasterKeys.Number40);
            _mainMaster.Add(MainMasterKeys.aParams_paranthesisClose, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.statBlock_else, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.statBlock_for, ProductionMasterKeys.Number45);
            _mainMaster.Add(MainMasterKeys.statBlock_get, ProductionMasterKeys.Number45);
            _mainMaster.Add(MainMasterKeys.statBlock_id, ProductionMasterKeys.Number45);
            _mainMaster.Add(MainMasterKeys.statBlock_if, ProductionMasterKeys.Number45);
            _mainMaster.Add(MainMasterKeys.statBlock_put, ProductionMasterKeys.Number45);
            _mainMaster.Add(MainMasterKeys.statBlock_return, ProductionMasterKeys.Number45);
            _mainMaster.Add(MainMasterKeys.statBlock_semicolon, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.statement_if, ProductionMasterKeys.Number90);
            _mainMaster.Add(MainMasterKeys.statement_for, ProductionMasterKeys.Number91);
            _mainMaster.Add(MainMasterKeys.statement_get, ProductionMasterKeys.Number92);
            _mainMaster.Add(MainMasterKeys.statement_put, ProductionMasterKeys.Number93);
            _mainMaster.Add(MainMasterKeys.statement_return, ProductionMasterKeys.Number94);
            _mainMaster.Add(MainMasterKeys.variableTail_id, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.expr_intNum, ProductionMasterKeys.Number38);
            _mainMaster.Add(MainMasterKeys.expr_floatNum, ProductionMasterKeys.Number122);
            _mainMaster.Add(MainMasterKeys.relExpr1_lessThan, ProductionMasterKeys.Number111);
            _mainMaster.Add(MainMasterKeys.relExpr1_lessThanEqualTo, ProductionMasterKeys.Number112);
            _mainMaster.Add(MainMasterKeys.relExpr1_greaterThan, ProductionMasterKeys.Number113);
            _mainMaster.Add(MainMasterKeys.relExpr1_greaterThanEqualTo, ProductionMasterKeys.Number114);
            _mainMaster.Add(MainMasterKeys.relExpr1_intNum, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.addOp_plus, ProductionMasterKeys.Number115);
            _mainMaster.Add(MainMasterKeys.addOp_minus, ProductionMasterKeys.Number116);
            _mainMaster.Add(MainMasterKeys.addOp_id, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.statBlock_curlieOpen, ProductionMasterKeys.Number117);
            _mainMaster.Add(MainMasterKeys.expr1_id, ProductionMasterKeys.Number118);
            _mainMaster.Add(MainMasterKeys.fParams_paranthesisClose, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.exprDT_semicolon, ProductionMasterKeys.Number2);
            //_mainMaster.Add(MainMasterKeys.exprDT_plus, ProductionMasterKeys.Number120);
            //_mainMaster.Add(MainMasterKeys.exprDT_product, ProductionMasterKeys.Number121);
            _mainMaster.Add(MainMasterKeys.arithExpr_id, ProductionMasterKeys.Number123);
            _mainMaster.Add(MainMasterKeys.term_id, ProductionMasterKeys.Number124);
            _mainMaster.Add(MainMasterKeys.term2_paranthesisOpen, ProductionMasterKeys.Number125);
            _mainMaster.Add(MainMasterKeys.term2_bracketOpen, ProductionMasterKeys.Number126);
            _mainMaster.Add(MainMasterKeys.arithExpr_semicolon, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.arithExpr_plus, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.arithExpr_product, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.term2_semicolon, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.expr1_plus, ProductionMasterKeys.Number127);
            _mainMaster.Add(MainMasterKeys.expr1_product, ProductionMasterKeys.Number128);
            _mainMaster.Add(MainMasterKeys.term_intNum, ProductionMasterKeys.Number129);
            _mainMaster.Add(MainMasterKeys.term_floatNum, ProductionMasterKeys.Number129);
            _mainMaster.Add(MainMasterKeys.term2_plus, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.term2_product, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.term_plus, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.term_product, ProductionMasterKeys.Number2);
            _mainMaster.Add(MainMasterKeys.term3_id, ProductionMasterKeys.Number151);
            _mainMaster.Add(MainMasterKeys.term3_intNum, ProductionMasterKeys.Number152);
            _mainMaster.Add(MainMasterKeys.term3_bracketClose, ProductionMasterKeys.Number2);
        }
        private static void PopulateProductionMaster()
        {

            _productionMaster.Add(ProductionMasterKeys.Number57, new List<string> { NonTerminals.classDeclRep, NonTerminals.funcDefRep, Terminals.startProgram, Terminals.program, NonTerminals.funcBody, Terminals.endProgram, Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number58, new List<string> { Terminals.startClassDecl, NonTerminals.classDecl, Terminals.endClassDecl, NonTerminals.classDeclRep });
            _productionMaster.Add(ProductionMasterKeys.Number1, new List<string> { Terminals.className, Terminals.makeNode, Terminals.id, NonTerminals.classDecloptional, Terminals.curlieOpen, NonTerminals.varDeclRep, Terminals.curlieClose, Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number2, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number60, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number50, new List<string> { Terminals.startVarDecl, Terminals.startDeclaration, NonTerminals.varDecl, Terminals.endVarDecl, NonTerminals.varDeclRep });
            _productionMaster.Add(ProductionMasterKeys.Number43, new List<string> { NonTerminals.type, Terminals.makeNode, Terminals.id, Terminals.endDeclaration, Terminals.startArraySize, NonTerminals.arraySizeRep, Terminals.endArraySize, Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number12, new List<string> { Terminals.makeNode, Terminals.intDT });
            _productionMaster.Add(ProductionMasterKeys.Number69, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number6, new List<string> { Terminals.startFuncDecl, NonTerminals.funcDecl, Terminals.endFuncDecl, NonTerminals.funcDeclRep });
            _productionMaster.Add(ProductionMasterKeys.Number52, new List<string> { NonTerminals.type, Terminals.makeNode, Terminals.id, Terminals.paranthesisOpen, Terminals.startParamDecl, NonTerminals.fParams, Terminals.endParamDecl, Terminals.paranthesisClose, Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number16, new List<string> { NonTerminals.type, Terminals.makeNode, Terminals.id, Terminals.startArraySize, NonTerminals.arraySizeRep, Terminals.endArraySize, NonTerminals.fParamsTailRep });
            _productionMaster.Add(ProductionMasterKeys.Number70, new List<string> { Terminals.makeNode, Terminals.floatDT });
            _productionMaster.Add(ProductionMasterKeys.Number63, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number51, new List<string> { Terminals.startFuncDef, NonTerminals.funcHead, Terminals.endFuncDef, NonTerminals.funcBody, Terminals.semicolon, });
            _productionMaster.Add(ProductionMasterKeys.Number53, new List<string> { NonTerminals.type, Terminals.makeNode, Terminals.id, NonTerminals.funcHeadTail1, NonTerminals.funcHeadTail2 });
            _productionMaster.Add(ProductionMasterKeys.Number54, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number55, new List<string> { Terminals.sr, Terminals.makeNode, Terminals.id });
            _productionMaster.Add(ProductionMasterKeys.Number56, new List<string> { Terminals.paranthesisOpen, Terminals.startParamDecl, NonTerminals.fParams, Terminals.endParamDecl, Terminals.paranthesisClose });
            _productionMaster.Add(ProductionMasterKeys.Number49, new List<string> { Terminals.startFuncBody, Terminals.curlieOpen, NonTerminals.varDeclRep, /*Terminals.id, Terminals.id, Terminals.semicolon, /*NonTerminals.arraySizeRep/*Terminals.bracketOpen,Terminals.intNum,Terminals.bracketClose, Terminals.semicolon,*/ /*Terminals.startStatBlock,*/ NonTerminals.statementRep, /*Terminals.endStatBlock,*/ Terminals.curlieClose, Terminals.endFuncBody });
            _productionMaster.Add(ProductionMasterKeys.Number71, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number3, new List<string> { Terminals.colon, Terminals.startInheritanceList, Terminals.id, NonTerminals.classDeclidRep });
            _productionMaster.Add(ProductionMasterKeys.Number72, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number15, new List<string> { NonTerminals.arraySize, NonTerminals.arraySizeRep });
            _productionMaster.Add(ProductionMasterKeys.Number13, new List<string> { Terminals.bracketOpen, Terminals.makeNode, Terminals.intNum, Terminals.bracketClose });
            _productionMaster.Add(ProductionMasterKeys.Number73, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number74, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number47, new List<string> { NonTerminals.statement, NonTerminals.statementRep });
            _productionMaster.Add(ProductionMasterKeys.Number48, new List<string> { Terminals.startAssignStat, NonTerminals.assignStat, Terminals.endAssignStat });
            _productionMaster.Add(ProductionMasterKeys.Number44, new List<string> { NonTerminals.variable, Terminals.makeNode, Terminals.equalTo, NonTerminals.expr, Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number77, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number75, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number76, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number78, new List<string> { Terminals.id });
            _productionMaster.Add(ProductionMasterKeys.Number79, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number80, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number100, new List<string> { Terminals.makeNode, Terminals.id, Terminals.makeNode, Terminals.equalTo, Terminals.makeNode, Terminals.id, Terminals.bracketOpen, Terminals.intNum, Terminals.bracketClose });
            _productionMaster.Add(ProductionMasterKeys.Number101, new List<string> { Terminals.returnValue, Terminals.paranthesisOpen, Terminals.id, Terminals.paranthesisClose, Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number102, new List<string> { Terminals.epsilon });
            _productionMaster.Add(ProductionMasterKeys.Number103, new List<string> { Terminals.startFuncDecl, Terminals.paranthesisOpen, Terminals.startParamDecl, NonTerminals.fParams, Terminals.endParamDecl, Terminals.paranthesisClose, Terminals.endFuncDecl });
           _productionMaster.Add(ProductionMasterKeys.Number204, new List<string> { Terminals.returnValue, Terminals.paranthesisOpen, Terminals.id, Terminals.paranthesisClose, Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number206, new List<string> { Terminals.floatDT, Terminals.id, Terminals.paranthesisOpen, Terminals.paranthesisClose, Terminals.curlieOpen, NonTerminals.varDeclRep, Terminals.id, Terminals.equalTo, Terminals.intNum, Terminals.product, Terminals.intNum, Terminals.semicolon, Terminals.returnValue, Terminals.paranthesisOpen, Terminals.id, Terminals.paranthesisClose, Terminals.semicolon, Terminals.curlieClose, Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number300, new List<string> { Terminals.curlieOpen,NonTerminals.varDeclRep, Terminals.id, Terminals.id, Terminals.semicolon, Terminals.id, Terminals.id,Terminals.bracketOpen, Terminals.intNum, Terminals.bracketClose, Terminals.semicolon, Terminals.forValue, Terminals.paranthesisOpen, Terminals.intDT,Terminals.id,Terminals.equalTo,Terminals.intNum,Terminals.semicolon,Terminals.id, Terminals.lessThanEqualTo,Terminals.intNum,Terminals.semicolon,Terminals.id,Terminals.equalTo,
                                                                                     Terminals.id,Terminals.plus,Terminals.intNum,Terminals.paranthesisClose,Terminals.curlieOpen,Terminals.getValue,Terminals.paranthesisOpen,Terminals.id,Terminals.bracketOpen,Terminals.id,Terminals.bracketClose,Terminals.paranthesisClose,Terminals.semicolon,Terminals.curlieClose,Terminals.semicolon,Terminals.id, Terminals.equalTo,Terminals.id,Terminals.dot,Terminals.id,Terminals.paranthesisOpen,
                                                                                     Terminals.id, Terminals.paranthesisClose,Terminals.semicolon, Terminals.id,Terminals.dot,Terminals.id,Terminals.bracketOpen,Terminals.id,Terminals.plus,Terminals.id,Terminals.bracketClose,Terminals.equalTo,Terminals.floatNum,Terminals.semicolon, Terminals.putValue, Terminals.paranthesisOpen, Terminals.id,Terminals.paranthesisClose,Terminals.semicolon,Terminals.curlieClose });

            _productionMaster.Add(ProductionMasterKeys.Number59, new List<string> { NonTerminals.funcDef, NonTerminals.funcDefRep });
            _productionMaster.Add(ProductionMasterKeys.Number18, new List<string> { NonTerminals.fParamsTail, NonTerminals.fParamsTailRep });
            _productionMaster.Add(ProductionMasterKeys.Number14, new List<string> { Terminals.comma, NonTerminals.type, Terminals.id, NonTerminals.arraySizeRep });
            _productionMaster.Add(ProductionMasterKeys.Number20, new List<string> { Terminals.makeNode, Terminals.id, NonTerminals.idnest });
            _productionMaster.Add(ProductionMasterKeys.Number81, new List<string> { NonTerminals.type, NonTerminals.idnest });
            _productionMaster.Add(ProductionMasterKeys.Number82, new List<string> { Terminals.dot, Terminals.id, NonTerminals.indicerep });
            _productionMaster.Add(ProductionMasterKeys.Number83, new List<string> { NonTerminals.indice, NonTerminals.indicerep });
            _productionMaster.Add(ProductionMasterKeys.Number84, new List<string> { Terminals.bracketOpen,/**/Terminals.bracketClose, NonTerminals.idnestrep });
            _productionMaster.Add(ProductionMasterKeys.Number87, new List<string> { Terminals.dot, Terminals.id, NonTerminals.indicerep });
            _productionMaster.Add(ProductionMasterKeys.Number85, new List<string> { Terminals.bracketOpen,/**/Terminals.bracketClose });
            _productionMaster.Add(ProductionMasterKeys.Number86, new List<string> { Terminals.paranthesisOpen,/**/Terminals.paranthesisClose, Terminals.dot, Terminals.id, NonTerminals.indicerep });
            _productionMaster.Add(ProductionMasterKeys.Number92, new List<string> { Terminals.getValue, Terminals.paranthesisOpen, Terminals.id, Terminals.bracketOpen, Terminals.id, Terminals.bracketClose, Terminals.paranthesisClose, Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number93, new List<string> { Terminals.putValue, Terminals.paranthesisOpen, Terminals.id, Terminals.paranthesisClose, Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number94, new List<string> { Terminals.startReturn,Terminals.returnValue, Terminals.paranthesisOpen, Terminals.makeNode, Terminals.id, Terminals.paranthesisClose, Terminals.semicolon, Terminals.endReturn });
            _productionMaster.Add(ProductionMasterKeys.Number91, new List<string> { Terminals.forValue, Terminals.paranthesisOpen, NonTerminals.type, Terminals.id, Terminals.equalTo, NonTerminals.expr, Terminals.semicolon, NonTerminals.relExpr, Terminals.semicolon, Terminals.id, Terminals.equalTo, Terminals.id, NonTerminals.addOp, Terminals.intNum, Terminals.paranthesisClose, NonTerminals.statBlock,/**/ Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number90, new List<string> { Terminals.ifValue, Terminals.paranthesisOpen, Terminals.id,Terminals.bracketOpen,Terminals.id,Terminals.bracketClose, Terminals.greaterThan,Terminals.id, Terminals.paranthesisClose, Terminals.then, Terminals.curlieOpen, Terminals.id, Terminals.equalTo, Terminals.id, Terminals.bracketOpen, Terminals.id, Terminals.bracketClose, Terminals.semicolon, Terminals.curlieClose, Terminals.elseValue, Terminals.curlieOpen, Terminals.curlieClose, Terminals.semicolon });
            _productionMaster.Add(ProductionMasterKeys.Number36, new List<string> { NonTerminals.arithExpr, NonTerminals.expr1 });
            _productionMaster.Add(ProductionMasterKeys.Number37, new List<string> { Terminals.intNum, Terminals.bracketClose });
            _productionMaster.Add(ProductionMasterKeys.Number38, new List<string> { Terminals.makeNode, Terminals.intNum, NonTerminals.expr1 });
            _productionMaster.Add(ProductionMasterKeys.Number39, new List<string> { Terminals.id, NonTerminals.relExpr1, Terminals.intNum });
            _productionMaster.Add(ProductionMasterKeys.Number111, new List<string> { Terminals.lessThan });
            _productionMaster.Add(ProductionMasterKeys.Number112, new List<string> { Terminals.lessThanEqualTo });
            _productionMaster.Add(ProductionMasterKeys.Number113, new List<string> { Terminals.greaterThan });
            _productionMaster.Add(ProductionMasterKeys.Number114, new List<string> { Terminals.greaterThanEqualTo });
            _productionMaster.Add(ProductionMasterKeys.Number115, new List<string> { Terminals.plus });
            _productionMaster.Add(ProductionMasterKeys.Number116, new List<string> { Terminals.minus });
            _productionMaster.Add(ProductionMasterKeys.Number117, new List<string> { Terminals.curlieOpen, NonTerminals.statementRep, Terminals.curlieClose });
            _productionMaster.Add(ProductionMasterKeys.Number118, new List<string> { Terminals.id, Terminals.bracketClose, NonTerminals.relExpr1, Terminals.id });
            _productionMaster.Add(ProductionMasterKeys.Number119, new List<string> { Terminals.paranthesisOpen, Terminals.id, Terminals.bracketOpen, Terminals.id, Terminals.bracketClose, Terminals.product, Terminals.id, Terminals.paranthesisOpen, Terminals.paranthesisClose, Terminals.paranthesisClose });
            _productionMaster.Add(ProductionMasterKeys.Number120, new List<string> { Terminals.plus, Terminals.intNum });
            _productionMaster.Add(ProductionMasterKeys.Number121, new List<string> { Terminals.product, Terminals.floatNum });
            _productionMaster.Add(ProductionMasterKeys.Number122, new List<string> { Terminals.makeNode, Terminals.floatNum, NonTerminals.exprDT });
            _productionMaster.Add(ProductionMasterKeys.Number123, new List<string> { NonTerminals.term });
            _productionMaster.Add(ProductionMasterKeys.Number124, new List<string> { Terminals.makeNode,Terminals.id, NonTerminals.term2 });
            _productionMaster.Add(ProductionMasterKeys.Number125, new List<string> { Terminals.paranthesisOpen,  Terminals.paranthesisClose });
            _productionMaster.Add(ProductionMasterKeys.Number126, new List<string> { Terminals.bracketOpen, NonTerminals.term3, Terminals.bracketClose });
            _productionMaster.Add(ProductionMasterKeys.Number127, new List<string> { Terminals.makeNode, Terminals.plus, NonTerminals.term });
            _productionMaster.Add(ProductionMasterKeys.Number128, new List<string> { Terminals.makeNode, Terminals.product, NonTerminals.term });
            _productionMaster.Add(ProductionMasterKeys.Number129, new List<string> { Terminals.makeNode, Terminals.intNum });
            _productionMaster.Add(ProductionMasterKeys.Number130, new List<string> { Terminals.makeNode, Terminals.intNum });
            _productionMaster.Add(ProductionMasterKeys.Number151, new List<string> { Terminals.makeNode, Terminals.id });
            _productionMaster.Add(ProductionMasterKeys.Number152, new List<string> { Terminals.makeNode, Terminals.intNum });
        }
        private static void PopulateFirstMaster()
        {
            _firstMaster.Add(NonTerminals.classDecl, new List<string> { Terminals.className });
        }
        private static void PopulateFollowMaster()
        {
            _followMaster.Add(NonTerminals.classDecl, new List<string> { Terminals.className, Terminals.program, Terminals.intDT, Terminals.floatDT, Terminals.id });
            _followMaster.Add(NonTerminals.classDecloptional, new List<string> { Terminals.curlieClose });
            _followMaster.Add(NonTerminals.varDeclRep, new List<string> { Terminals.intDT, Terminals.floatDT, Terminals.id });
            _followMaster.Add(NonTerminals.arraySize, new List<string> { Terminals.bracketOpen, Terminals.semicolon });
            _followMaster.Add(NonTerminals.type, new List<string> { Terminals.semicolon });
        }
        private static void PrintTokenStack()
        {
            Console.WriteLine("Token Stack in atocc format:\n");

            foreach (var item in tokenStackToParse)
            {
                Console.Write(item + " ");
            }

        }
        private static void PrintValueStack()
        {
            Console.WriteLine("Semantic Stack:");

            foreach (var item in tokenStackToParse)
            {
                Console.WriteLine(item.tokenValue);
            }

        }
        private static void PrintGrammarStack()
        {
            Console.WriteLine("Grammer Stack:");

            foreach (var item in _grammarStack)
            {
                Console.WriteLine(item);
            }
        }
        private static void AssemblyCode()
        {
            sbToAssembler.Append("entry");
            sbToAssembler.Append(Environment.NewLine);
            sbToAssembler.Append("addi r14,r0,topaddr");
            sbToAssembler.Append(Environment.NewLine);
            //List<string> inputs = new List<string>() { "9","random", "2", "+" };
            if(valueinFunc!=0)
            {
                assignlistToFormAssemble.Add(valueinFunc.ToString());
            }
            assignlistToFormAssemble.Add(operatorToAdd);
            var currentRegister = 1;

            foreach (var item in assignlistToFormAssemble)
            {
                if (currentRegister > maxRegister) currentRegister = 1; //reset the register

                //to check if current item is number
                int n;
                bool isNumeric = int.TryParse(item, out n);
                string str = item as string;

                if (isNumeric)
                {

                    if (funcVisit)
                    {
                        sbFunc.AppendFormat("addi r{0}, r0, {1}", currentRegister - 1, item);
                        sbFunc.Append(Environment.NewLine);
                        sbFunc.AppendFormat("lw r15, -4(r14)");
                        sbFunc.Append(Environment.NewLine);
                        sbFunc.AppendFormat("jr r15");
                        sbFunc.Append(Environment.NewLine);
                        sbToAssembler.AppendFormat("add r{0}, r{1}, r{2}", currentRegister, currentRegister - 2, currentRegister - 1);
                        sbToAssembler.Append(Environment.NewLine);
                        sbToAssembler.AppendFormat("sw -{0}(r14) , r{1}", intSize - 4, currentRegister);
                        sbToAssembler.Append(Environment.NewLine);
                        funcVisit = false;
                        break;
                    }
                    else
                    {
                        sbToAssembler.AppendFormat("addi r{0}, r0, {1}", currentRegister, item);
                        sbToAssembler.Append(Environment.NewLine);
                        sbToAssembler.AppendFormat("sw -{0}(r14) , r{1}", intSize, currentRegister);
                        sbToAssembler.Append(Environment.NewLine);

                        if (string.IsNullOrEmpty(leftOperand))
                        {
                            leftOperand = "r" + currentRegister;
                        }
                        else
                        {
                            rightOperand = "r" + currentRegister;
                        }
                    }
                }

                else if (operators.ContainsKey(item))
                {

                    sbToAssembler.AppendFormat("{0} r{1}, {2}, {3}", operators[item], currentRegister, leftOperand, rightOperand);
                    sbToAssembler.Append(Environment.NewLine);
                    sbToAssembler.AppendFormat("sw -{0}(r14) , r{1}", intSize, currentRegister);
                    sbToAssembler.Append(Environment.NewLine);

                    //lastOperationValue = "r" + currentRegister;
                    leftOperand = "";
                    rightOperand = "r" + currentRegister;
                }

                else if (item is String)
                {
                    sbToAssembler.AppendFormat("jl r15," + item);
                    sbToAssembler.Append(Environment.NewLine);
                    sbFunc.AppendFormat(item + " sw -4(r14), r15");
                    sbFunc.Append(Environment.NewLine);
                    funcVisit = true;
                }
                else
                {
                }

                currentRegister++;
                intSize += 4;
            }

            sbToAssembler.AppendFormat("lw r{0},-{1}(r14)", currentRegister, intSize - 4);
            sbToAssembler.Append(Environment.NewLine);
            sbToAssembler.AppendFormat("addi r14,r14,-4");
            sbToAssembler.Append(Environment.NewLine);
            sbToAssembler.AppendFormat("sw -8(r14),r{0}", currentRegister);
            sbToAssembler.Append(Environment.NewLine);
            sbToAssembler.Append(strPutValue);
            if(sbFunc.Length > 0)
            {
                sbToAssembler.Append(sbFunc);
            }
            sbToAssembler.Append(strEnd);

            using (StreamWriter writetoassemblyfile = new StreamWriter("C:\\Users\\Khashyap\\Downloads\\Moon\\moon\\AssemblyCode.m"))
            {

                writetoassemblyfile.WriteLine(sbToAssembler.ToString());

            }

            //  StreamWriter file = new StreamWriter(@"C:\Users\Khashyap\Downloads\Moon\moon\AssemblyCode.m",true);
            // file.WriteLine(); 
        }

    }

    public class TokenstoParse
    {
        public string tokenType { get; set; }
        public string tokenValue { get; set; }
        public int tokenLine { get; set; }
        public int tokenPosition { get; set; }
    }
    //Extension  to push Stack Values
    public static class Extensions
    {
        public static void PushRange<T>(this Stack<T> source, IEnumerable<T> collection)
        {
            foreach (var item in collection.Reverse<T>())
                source.Push(item);
        }
    }
    //List Of Non Terminals
    public static class NonTerminals
    {
        public const string term3 = "term3";
        public const string prog = "prog";
        public const string classDecl = "classDecl";
        public const string classDecloptional = "classDecloptional";
        public const string classDeclRep = "classDeclRep";
        public const string funcDeclRep = "funcDeclRep";
        public const string funcBody = "funcBody";
        public const string funcDefRep = "funcDefRep";
        public const string varDeclRep = "varDeclRep";
        public const string varDecl = "varDecl";
        public const string type = "type";
        public const string funcDecl = "funcDecl";
        public const string fParams = "fParams";
        public const string classDeclid = "classDeclid";
        public const string classDeclidRep = "classDeclidRep";
        public const string assignOp = "assignOp";
        public const string sign = "sign";
        public const string relOp = "relOp";
        public const string addOp = "addOp";
        public const string multOp = "multOp";
        public const string arraySize = "arraySize";
        public const string fParamsTail = "fParamsTail";
        public const string arraySizeRep = "arraySizeRep";
        public const string fParamsTailRep = "fParamsTailRep";
        public const string variable = "variable";
        public const string idnestrep = "idnestrep";
        public const string indicerep = "indicerep";
        public const string functionCall = "functionCall";
        public const string idnest = "idnest";
        public const string idnestTail = "idnestTail";
        public const string indice = "indice";
        public const string arithExpr = "arithExpr";
        public const string arithExpr2 = "arithExpr2";
        public const string term = "term";
        public const string term2 = "term2";
        public const string factor = "factor";
        public const string variabletail = "variabletail";
        public const string aParamsTail = "aParamsTail";
        public const string expr = "expr";
        public const string expr1 = "expr1";
        public const string relExpr = "relExpr";
        public const string aParams = "aParams";
        public const string aParamsTailRep = "aParamsTailRep";
        public const string assignStat = "assignStat";
        public const string statBlock = "statBlock";
        public const string statementRep = "statementRep";
        public const string statement = "statement";
        public const string funcDef = "funcDef";
        public const string funcHead = "funcHead";
        public const string funcHeadTail1 = "funcHeadTail1";
        public const string funcHeadTail2 = "funcHeadTail2";
        public const string returnStat = "returnStat";
        public const string freeFuncDef = "freeFuncDef";
        public const string funcBodyNew = "funcBodyNew";
        public const string classobjDecl = "classobjDecl";
        public const string classobjDeclRep = "classobjDeclRep";
        public const string relExpr1 = "relExpr1";
        public const string exprDT = "exprDT";
    }
    //List Of Terminals
    public static class Terminals
    {
        public const string className = "class";
        public const string id = "id";
        public const string colon = ":";
        public const string curlieOpen = "{";
        public const string curlieClose = "}";
        public const string program = "program";
        public const string semicolon = ";";
        public const string intDT = "int";
        public const string floatDT = "float";
        public const string epsilon = "";
        public const string dollar = "$";
        public const string paranthesisOpen = "(";
        public const string paranthesisClose = ")";
        public const string comma = ",";
        public const string equalTo = "=";
        public const string plus = "+";
        public const string minus = "-";
        public const string equalCheck = "==";
        public const string notEqualCheck = "!=";
        public const string lessThan = "<";
        public const string greaterThan = ">";
        public const string lessThanEqualTo = "<=";
        public const string greaterThanEqualTo = ">=";
        public const string or = "||";
        public const string product = "*";
        public const string divide = "/";
        public const string and = "&&";
        public const string bracketOpen = "[";
        public const string bracketClose = "]";
        public const string sr = "::";
        public const string intNum = "intNum";
        public const string floatNum = "floatNum";
        public const string startClassDecl = "#startClassDecl";
        public const string endClassDecl = "#endClassDecl";
        public const string startVarDecl = "#startVarDecl";
        public const string endVarDecl = "#endVarDecl";
        public const string startFuncDecl = "#startFuncDecl";
        public const string endFuncDecl = "#endFuncDecl";
        public const string startParamDecl = "#startParamDecl";
        public const string endParamDecl = "#endParamDecl";
        public const string startAssignStat = "#startAssignStat";
        public const string endAssignStat = "#endAssignStat";
        public const string startFuncDef = "#startFuncDef";
        public const string endFuncDef = "#endFuncDef";
        public const string makeNode = "#makeNode";
        public const string hash = "#";
        public const string returnValue = "return";
        public const string startFuncBody = "#startFuncBody";
        public const string endFuncBody = "#endFuncBody";
        public const string startProgram = "#startProgram";
        public const string endProgram = "#endProgram";
        public const string ifValue = "if";
        public const string elseValue = "else";
        public const string forValue = "for";
        public const string getValue = "get";
        public const string putValue = "put";
        public const string then = "then";
        public const string startInheritanceList = "#startInheritanceList";
        public const string endInheritanceList = "#endInheritanceList";
        public const string startArraySize = "#startArraySize";
        public const string endArraySize = "#endArraySize";
        public const string startDeclaration = "#startDeclaration";
        public const string endDeclaration = "#endDeclaration";
        public const string dot = ".";
        public const string startStatBlock = "#startStatBlock";
        public const string endStatBlock = "#endStatBlock";
        public const string startIf = "#startIf";
        public const string endIf = "#endIf";
        public const string startFor = "#startFor";
        public const string endFor = "#endFor";
        public const string startExpr = "#startExpr";
        public const string endExpr = "#endExpr";
        public const string startReturn = "#startReturn";
        public const string endReturn = "#endReturn";
        public const string startClassObjDecl = "#startClassObjDecl";
        public const string endClassObjDecl = "#endClassObjDecl";
    }
    //mainMaster Dictionary Valid Keys  - NonTerminal_Terminal
    public static class MainMasterKeys
    {
        public const string prog_class = "prog_class";
        public const string classDeclRep_class = "classDeclRep_class";
        public const string classDecl_class = "classDecl_class";
        public const string classDecloptional_curlieOpen = "classDecloptional_curlieOpen";
        public const string classDeclRep_program = "classDeclRep_program";
        public const string classDeclRep_int = "classDeclRep_int";
        public const string classDeclRep_float = "classDeclRep_float";
        public const string classDeclRep_id = "classDeclRep_id";
        public const string varDeclRep_int = "varDeclRep_int";
        public const string varDeclRep_float = "varDeclRep_float";
        public const string varDeclRep_id = "varDeclRep_id";
        public const string varDecl_int = "varDecl_int";
        public const string varDecl_float = "varDecl_float";
        public const string varDecl_id = "varDecl_id";
        public const string type_int = "type_int";
        public const string varDeclRep_curlieClose = "varDeclRep_curlieClose";
        public const string funcDeclRep_int = "funcDeclRep_int";
        public const string funcDeclRep_float = "funcDeclRep_float";
        public const string funcDeclRep_id = "funcDeclRep_id";
        public const string funcDecl_int = "funcDecl_int";
        public const string funcDecl_float = "funcDecl_float";
        public const string funcDecl_id = "funcDecl_id";
        public const string fParams_int = "fParams_int";
        public const string fParams_float = "fParams_float";
        public const string fParams_id = "fParams_id";
        public const string fParams_paranthesisClose = "fParams_paranthesisClose";
        public const string type_float = "type_float";
        public const string funcDeclRep_curlieClose = "funcDeclRep_curlieClose";
        public const string funcDefRep_int = "funcDefRep_int";
        public const string funcHead_int = "funcHead_int";
        public const string funcHead_curlieOpen = "funcHead_curlieOpen";
        public const string funcHead_id = "funcHead_id";
        public const string funcHeadTail1_sr = "funcHeadTail1_sr";
        public const string funcHeadTail1_id = "funcHeadTail1_id";
        public const string funcHeadTail2_paranthesisOpen = "funcHeadTail2_paranthesisOpen";
        public const string funcBody_curlieOpen = "funcBody_curlieOpen";
        public const string funcBody_semicolon = "funcBody_semicolon";
        public const string classDecloptional_colon = "classDecloptional_colon";
        public const string classDeclidRep_curlieOpen = "classDeclidRep_curlieOpen";
        public const string arraySizeRep_bracketOpen = "arraySizeRep_bracketOpen";
        public const string arraySize_bracketOpen = "arraySize_bracketOpen";
        public const string arraySizeRep_bracket = "arraySizeRep_bracketOpen";
        public const string arraySize_bracketClose = "arraySize_bracketClose";
        public const string arraySizeRep_semicolon = "arraySizeRep_semicolon";
        public const string arraySizeRep_comma = "arraySizeRep_comma";
        public const string arraySizeRep_id = "arraySizeRep_id";
        public const string statementRep_id = "statementRep_id";
        public const string statementRep_if = "statementRep_if";
        public const string statementRep_for = "statementRep_for";
        public const string statementRep_get = "statementRep_get";
        public const string statementRep_put = "statementRep_put";
        public const string statementRep_return = "statementRep_return";
        public const string statementRep_paranthesisOpen = "statementRep_paranthesisOpen";
        public const string statement_if = "statement_if";
        public const string statement_for = "statement_for";
        public const string statement_get = "statement_get";
        public const string statement_put = "statement_put";
        public const string statement_return = "statement_return";
        public const string statement_id = "statement_id";
        public const string assignStat_id = "assignStat_id";
        public const string assignStat_paranthesisOpen = "assignStat_paranthesisOpen";
        public const string assignStat_semicolon = "assignStat_semicolon";
        public const string statement_semicolon = "statement_semicolon";
        public const string statementRep_curlieClose = "statementRep_curlieClose";
        public const string type_id = "type_id";
        public const string funcDefRep_program = "funcDefRep_program";
        public const string assignStat_int = "assignStat_int";
        public const string returnStat_return = "returnStat_return";
        public const string returnStat_curlieClose = "returnStat_curlieClose";
        public const string funcDefRep_float = "funcDefRep_float";
        public const string funcHead_float = "funcHead_float";
        public const string varDeclRep_semicolon = "varDeclRep_semicolon";
        public const string arraySizeRep_paranthesisOpen = "arraySizeRep_paranthesisOpen";
        public const string arraySizeRep_paranthesisClose = "arraySizeRep_paranthesisClose";
        public const string freeFuncDef_float = "freeFuncDef_float";
        public const string funcBodyNew_curlieOpen = "funcBodyNew_curlieOpen";
        public const string classobjDeclRep_id = "classobjDeclRep_id";
        public const string classobjDecl_id = "classobjDecl_id";
        public const string classobjDeclRep_for = "classobjDeclRep_for";
        public const string funcDefRep_id = "funcDefRep_id";
        public const string funcDef_int = "funcDef_int";
        public const string funcDef_float = "funcDef_float";
        public const string funcDef_id = "funcDef_id";
        public const string funcHeadTail2_id = "funcHeadTail2_id";
        public const string funcHeadTail1_paranthesisOpen = "funcHeadTail1_paranthesisOpen";
        public const string fParamsTailRep_comma = "fParamsTailRep_comma";
        public const string fParamsTail_comma = "fParamsTail_comma";
        public const string fParamsTailRep_paranthesisClose = "fParamsTailRep_paranthesisClose";
        public const string variable_id = "variable_id";
        public const string variable_equalTo = "variable_equalTo";
        public const string idnestrep_id = "idnestrep_id";
        public const string idnest_bracketOpen = "idnest_bracketOpen";
        public const string idnest_dot = "idnest_dot";
        public const string idnest_equalTo = "idnest_equalTo";
        public const string indicerep_equalTo = "indicerep_equalTo";
        public const string indicerep_bracketOpen = "indicerep_bracketOpen";
        public const string indice_bracketOpen = "indice_bracketOpen";
        public const string idnest_paranthesisOpen = "idnest_paranthesisOpen";

        public const string expr_id = "expr_id";
        public const string expr_paranthesisOpen = "expr_paranthesisOpen";
        public const string expr1_id = "expr1_id";
        public const string expr1_intNum = "expr1_intNum";
        public const string expr_floatNum = "expr_floatNum";
        public const string expr_intNum = "expr_intNum";
        public const string expr_greaterThan = "expr_greaterThan";
        public const string expr1_plus = "expr1_plus";
        public const string expr1_product = "expr1_product";




        public const string expr1_equalTo = "expr1_equalTo";
        public const string expr1_semicolon = "expr1_semicolon";
        public const string expr1_lessThan = "expr1_lessThan";
        public const string expr1_greaterThan = "expr1_greaterThan";
        public const string expr1_lessThanEqualTo = "expr1_lessThanEqualTo";
        public const string expr1_greaterThanEqualTo = "expr1_greaterThanEqualTo";
        public const string expr1_paranthesisClose = "expr1_paranthesisClose";
        public const string expr1_comma = "expr1_comma";

        public const string exprDT_product = "exprDT_product";
        public const string exprDT_plus = "exprDT_plus";
        public const string exprDT_semicolon = "exprDT_semicolon";

        public const string term3_intNum = "term3_intNum";
        public const string term3_id = "term3_id";
        public const string term3_bracketClose = "term3_bracketClose";



        public const string relExpr_id = "relExpr_id";
        public const string relExpr1_lessThan = "relExpr1_lessThan";
        public const string relExpr1_greaterThan = "relExpr1_greaterThan";
        public const string relExpr1_lessThanEqualTo = "relExpr1_lessThanEqualTo";
        public const string relExpr1_greaterThanEqualTo = "relExpr1_greaterThanEqualTo";
        public const string relExpr1_intNum = "relExpr1_intNum";
        public const string aParams_id = "aParams_id";
        public const string aParams_paranthesisClose = "aParams_paranthesisClose";

        public const string statBlock_id = "statBlock_id";
        public const string statBlock_if = "statBlock_if";
        public const string statBlock_for = "statBlock_for";
        public const string statBlock_get = "statBlock_get";
        public const string statBlock_put = "statBlock_put";
        public const string statBlock_return = "statBlock_return";
        public const string statBlock_curlieOpen = "statBlock_curlieOpen";
        public const string statBlock_else = "statBlock_else";
        public const string statBlock_semicolon = "statBlock_semicolon";

        public const string arithExpr_id = "arithExpr_id";
        public const string arithExpr_plus = "arithExpr_plus";
        public const string arithExpr_product = "arithExpr_product";
        public const string arithExpr_semicolon = "arithExpr_semicolon";
        public const string arithExpr2_plus = "arithExpr2_plus";
        public const string arithExpr2_minus = "arithExpr2_minus";
        public const string arithExpr2_or = "arithExpr2_or";
        public const string arithExpr2_equalTo = "arithExpr2_equalTo";
        public const string arithExpr2_lessThan = "arithExpr2_lessThan";
        public const string arithExpr2_greaterThan = "arithExpr2_greaterThan";
        public const string arithExpr2_lessThanEqualTo = "arithExpr2_lessThanEqualTo";
        public const string arithExpr2_greaterThanEqualTo = "arithExpr2_greaterThanEqualTo";
        public const string arithExpr2_semicolon = "arithExpr2_semicolon";
        public const string arithExpr2_comma = "arithExpr2_comma";
        public const string arithExpr2_paranthesisClose = "arithExpr2_paranthesisClose";
        public const string arithExpr2_curlieClose = "arithExpr2_curlieClose";

        public const string term_id = "term_id";
        public const string term_plus = "term_plus";
        public const string term_minus = "term_minus";
        public const string term_or = "term_or";
        public const string term_equalTo = "term_equalTo";
        public const string term_lessThan = "term_lessThan";
        public const string term_greaterThan = "term_greaterThan";
        public const string term_lessThanEqualTo = "term_lessThanEqualTo";
        public const string term_greaterThanEqualTo = "term_greaterThanEqualTo";
        public const string term_semicolon = "term_semicolon";
        public const string term_comma = "term_comma";
        public const string term_paranthesisClose = "term_paranthesisClose";
        public const string term_curlieClose = "term_curlieClose";
        public const string term_intNum = "term_intNum";
        public const string term_floatNum = "term_floatNum";
        public const string term_product = "term_product";


        public const string term2_id = "term2_id";
        public const string term2_plus = "term2_plus";
        public const string term2_minus = "term2_minus";
        public const string term2_or = "term2_or";
        public const string term2_equalTo = "term2_equalTo";
        public const string term2_lessThan = "term2_lessThan";
        public const string term2_greaterThan = "term2_greaterThan";
        public const string term2_lessThanEqualTo = "term2_lessThanEqualTo";
        public const string term2_greaterThanEqualTo = "term2_greaterThanEqualTo";
        public const string term2_semicolon = "term2_semicolon";
        public const string term2_comma = "term2_comma";
        public const string term2_paranthesisClose = "term2_paranthesisClose";
        public const string term2_curlieClose = "term2_curlieClose";
        public const string term2_product = "term2_product";
        public const string term2_divide = "term2_divide";
        public const string term2_and = "term2_and";
        public const string term2_paranthesisOpen = "term2_paranthesisOpen";
        public const string term2_bracketOpen = "term2_bracketOpen";

        public const string factor_id = "factor_id";
        public const string factor_plus = "factor_plus";
        public const string factor_minus = "factor_minus";
        public const string factor_or = "factor_or";
        public const string factor_equalTo = "factor_equalTo";
        public const string factor_lessThan = "factor_lessThan";
        public const string factor_greaterThan = "factor_greaterThan";
        public const string factor_lessThanEqualTo = "factor_lessThanEqualTo";
        public const string factor_greaterThanEqualTo = "factor_greaterThanEqualTo";
        public const string factor_semicolon = "factor_semicolon";
        public const string factor_comma = "factor_comma";
        public const string factor_paranthesisClose = "factor_paranthesisClose";
        public const string factor_curlieClose = "factor_curlieClose";
        public const string factor_product = "factor_product";
        public const string factor_divide = "factor_divide";
        public const string factor_and = "factor_and";

        public const string variabletail_intNum = "variabletail_intNum";
        public const string variabletail_floatNum = "variabletail_floatNum";
        public const string variableTail_plus = "variableTail_plus";
        public const string variableTail_minus = "variableTail_minus";
        public const string variableTail_id = "variableTail_id";
        public const string variableTail_not = "variableTail_not";
        public const string variableTail_paranthesisOpen = "variableTail_paranthesisOpen";

        public const string sign_plus = "sign_plus";
        public const string sign_minus = "sign_minus";


        public const string relOp_equalTo = "relOp_equalTo";
        public const string relOp_lessThan = "relOp_lessThan";
        public const string relOp_greaterThan = "relOp_greaterThan";
        public const string relOp_lessThanEqualTo = "relOp_lessThanEqualTo";
        public const string relOp_greaterThanEqualTo = "relOp_greaterThanEqualTo";
        public const string relOp_notEqualCheck = "relOp_notEqualCheck";


        public const string addOp_plus = "addOp_plus";
        public const string addOp_minus = "addOp_minus";
        public const string addOp_or = "addOp_or";
        public const string addOp_id = "addOp_id";

        public const string multOp_product = "multOp_product";
        public const string multOp_divide = "multOp_divide";
        public const string multOp_and = "multOp_and";


        public const string idnestrep_paranthesisOpen = "idnestrep_paranthesisOpen";
        public const string idnestrep_bracketOpen = "idnestrep_bracketOpen";
        public const string idnestrep_dot = "idnestrep_dot";
        public const string idnestrep_lessThan = "idnestrep_lessThan";
        public const string idnestrep_greaterThan = "idnestrep_greaterThan";
        public const string idnestrep_equalTo = "idnestrep_equalTo";
        public const string idnestTail_dot = "idnestTail_dot";


        //public const string idnestrep_paranthesisOpen = "idnestrep_paranthesisOpen";
        //public const string fParamsTail_comma = "fParamsTail_comma";
    }
    public static class ProductionMasterKeys
    {
        public const string Number1 = "1";
        public const string Number2 = "2";
        public const string Number3 = "3";
        public const string Number4 = "4";
        public const string Number5 = "5";
        public const string Number6 = "6";
        public const string Number7 = "7";
        public const string Number8 = "8";
        public const string Number9 = "9";
        public const string Number10 = "10";
        public const string Number11 = "11";
        public const string Number12 = "12";
        public const string Number13 = "13";
        public const string Number14 = "14";
        public const string Number15 = "15";
        public const string Number16 = "16";
        public const string Number17 = "17";
        public const string Number18 = "18";
        public const string Number19 = "19";
        public const string Number20 = "20";
        public const string Number21 = "21";
        public const string Number22 = "22";
        public const string Number23 = "23";
        public const string Number24 = "24";
        public const string Number25 = "25";
        public const string Number26 = "26";
        public const string Number27 = "27";
        public const string Number28 = "28";
        public const string Number29 = "29";
        public const string Number30 = "30";
        public const string Number31 = "31";
        public const string Number32 = "32";
        public const string Number33 = "33";
        public const string Number34 = "34";
        public const string Number35 = "35";
        public const string Number36 = "36";
        public const string Number37 = "37";
        public const string Number38 = "38";
        public const string Number39 = "39";
        public const string Number40 = "40";
        public const string Number41 = "41";
        public const string Number42 = "42";
        public const string Number43 = "43";
        public const string Number44 = "44";
        public const string Number45 = "45";
        public const string Number46 = "46";
        public const string Number47 = "47";
        public const string Number48 = "48";
        public const string Number49 = "49";
        public const string Number50 = "50";
        public const string Number51 = "51";
        public const string Number52 = "52";
        public const string Number53 = "53";
        public const string Number54 = "54";
        public const string Number55 = "55";
        public const string Number56 = "56";
        public const string Number57 = "57";
        public const string Number58 = "58";
        public const string Number59 = "59";
        public const string Number60 = "60";
        public const string Number61 = "61";
        public const string Number62 = "62";
        public const string Number63 = "63";
        public const string Number64 = "64";
        public const string Number65 = "65";
        public const string Number66 = "66";
        public const string Number67 = "67";
        public const string Number68 = "68";
        public const string Number69 = "69";
        public const string Number70 = "70";
        public const string Number71 = "71";
        public const string Number72 = "72";
        public const string Number73 = "73";
        public const string Number74 = "74";
        public const string Number75 = "75";
        public const string Number76 = "76";
        public const string Number77 = "77";
        public const string Number78 = "78";
        public const string Number79 = "79";
        public const string Number80 = "80";
        public const string Number81 = "81";
        public const string Number82 = "82";
        public const string Number83 = "83";
        public const string Number84 = "84";
        public const string Number85 = "85";
        public const string Number86 = "86";
        public const string Number87 = "87";
        public const string Number88 = "88";
        public const string Number89 = "89";
        public const string Number90 = "90";
        public const string Number91 = "91";
        public const string Number92 = "92";
        public const string Number93 = "93";
        public const string Number94 = "94";
        public const string Number95 = "95";
        public const string Number96 = "96";
        public const string Number97 = "97";
        public const string Number98 = "98";
        public const string Number99 = "99";
        public const string Number100 = "100";
        public const string Number101 = "101";
        public const string Number102 = "102";
        public const string Number103 = "103";


        public const string Number111 = "111";
        public const string Number112 = "112";
        public const string Number113 = "113";
        public const string Number114 = "114";
        public const string Number115 = "115";
        public const string Number116 = "116";
        public const string Number117 = "117";
        public const string Number118 = "118";
        public const string Number119 = "119";
        public const string Number120 = "120";
        public const string Number121 = "121";
        public const string Number122 = "122";
        public const string Number123 = "123";
        public const string Number124 = "124";
        public const string Number125 = "125";
        public const string Number126 = "126";
        public const string Number127 = "127";
        public const string Number128 = "128";
        public const string Number129 = "129";
        public const string Number130 = "130";
        public const string Number151 = "151";
        public const string Number152 = "152";


        public const string Number200 = "200";
        public const string Number201 = "201";
        public const string Number202 = "202";
        public const string Number203 = "203";
        public const string Number204 = "204";
        public const string Number205 = "205";
        public const string Number206 = "206";
        public const string Number300 = "300";
        public const string Number301 = "301";
        public const string Number302 = "302";

    }
    public class GrammarValue
    {
        public string ParentClass { get; set; }
        public string CurrentCheck { get; set; }
        public string CurrentFunc { get; set; }
        public string CurrentProg { get; set; }
        public string astFuncName { get; set; }
        public string assignmentScope { get; set; }
        public bool AssignStateVarCheck { get; set; }
        public string AssignStateVarType { get; set; }
    }
    //Abstract Syntax tree using Composite Design Pattern
    public abstract class Component

    {
        protected string name;

        //public Component _compositeSibling { get; set; }
        // Constructor
        public Component(string name)
        {
            this.name = name;
        }

        public virtual string GetName() { return this.name; }
        public abstract void Add(Component c);
        public abstract void Remove(Component c);
        //public abstract void Display(int depth);

        public abstract void Display(int depth, GrammarValue grammarValue);
    }
    public class Session
    {
        private static Dictionary<string, string> users = new Dictionary<string, string>();

        public static void Set(string key, string value)
        {
            if (users.ContainsKey(key))
            {
                users[key] = value;
            }
            else
            {
                users.Add(key, value);
            }
        }

        public static string Get(string key)
        {
            string result = null;

            if (users.ContainsKey(key))
            {
                result = users[key];
            }

            return result;
        }
    }
    //Component _compositeSibling = new Composite("dummy");
    // The 'Composite' class
    public class Composite : Component
    {

        private List<Component> _children = new List<Component>();
        // Constructor
        public Composite(string name)
          : base(name)
        {
        }
        public override void Add(Component component)
        {
            _children.Add(component);
        }
        public override void Remove(Component component)
        {
            _children.Remove(component);
        }

        
        public override void Display(int depth, GrammarValue grammarValue)
        {

            Console.WriteLine(new String('-', depth) + name);
            Component previousItem = new Composite("Initial");

            //var assignmentScope = "";
            // Recursively display child nodes and handle semantic errors
            foreach (Component component in _children)
            {

                if (component.GetName() == "Inheritance")
                {
                    grammarValue = new GrammarValue { CurrentCheck = component.GetName(), ParentClass = previousItem.GetName() };
                }

                if (component.GetName() == "FuncDef")
                {
                    Session.Set("AssignmentKey", component.GetName());
                    
                    grammarValue = new GrammarValue { CurrentFunc = component.GetName() };
                }

                if (component.GetName() == "program")
                {
                    Session.Set("AssignmentKey", component.GetName());
                }

                if (component.GetName() == "AssignStat")
                {
                    grammarValue = new GrammarValue { CurrentCheck = component.GetName(), assignmentScope = Session.Get("AssignmentKey")};
                }

                if (component.GetName() == "ReturnBlock")
                {
                    grammarValue = new GrammarValue { CurrentCheck = component.GetName() };
                }

                previousItem = component;
                component.Display(depth + 1, grammarValue);
            }
        }
    }
    // The 'Leaf' class
    public class Leaf : Component
    {
        InheritanceValidator objvalidatorInheritance = new InheritanceValidator();
        DeclarationValidator objvalidatorDeclaration = new DeclarationValidator();
        GetDataType objDT = new GetDataType();
        public static string astFuncName = null;
        public static Boolean visitFuncDef = false;
        public static int countVisitAssignStatFunc = 0;
        public static int countVisitAssignStatProg = 0;
        // Constructor
        public Leaf(string name)
          : base(name)
        {
        }
        public override void Add(Component c)
        {
            Console.WriteLine("Cannot add to a leaf");
        }
        public override void Remove(Component c)
        {
            Console.WriteLine("Cannot remove from a leaf");
        }

        public override void Display(int depth, GrammarValue grammarValue)
        {
           
            if (grammarValue.CurrentCheck == "Inheritance")
            {
                String[] nameArray = name.Split(','); 
                var inheritanceKey = string.Format("{0}_{1}", nameArray[0], grammarValue.ParentClass);
                if (objvalidatorInheritance.IsCyclicInheritance(inheritanceKey, nameArray[0]))
                {
                    //Console.WriteLine("Cyclic Inheritance");
                    String dictkey = (nameArray[1] + "," + nameArray[2]);
                    List<String> dictValue = new List<string>();
                    dictValue.Add("Semantic Error" + "," + "Circular Inheritance on classes:+" + name[0] + "," +grammarValue.ParentClass);
                    Lexical_Analyzer.errorDict.Add(dictkey, dictValue);
                }
                Console.WriteLine(new String('-', depth) + nameArray[0]);
            }

           else if (grammarValue.CurrentCheck == "AssignStat" && grammarValue.assignmentScope == "FuncDef")
            //if (grammarValue.CurrentCheck == "AssignStat")
            {
                if (!grammarValue.AssignStateVarCheck)
                {
                    //check if <<name>> is int then assignmentVariableType = "
                    var scopeKey = string.Format("{0},{1}", astFuncName, name);
                    if (!objvalidatorDeclaration.IsScope(scopeKey))
                    {
                        Console.WriteLine("Validation error");
                    }
                    //grammarValue.AssignStateVarType = "intNum";
                    grammarValue.AssignStateVarType =  objDT.getDataType(name);
                    grammarValue.AssignStateVarCheck = true;
                    
                }
                //skip if assignop skip
                else if(name == "AssignOp") { }
                else if (name == "+") { }
                else
                {
                    if (name != grammarValue.AssignStateVarType)
                    {
                        String dictkey = (name + "," + grammarValue.ParentClass);
                        List<String> dictValue = new List<string>();
                        dictValue.Add("Semantic Error" + "," + "Circular Inheritance on classes:+" + name + "," + grammarValue.ParentClass);
                        Lexical_Analyzer.errorDict.Add(dictkey, dictValue);

                    }
                }

                Console.WriteLine(new String('-', depth) + name);
            }

           else if (grammarValue.CurrentCheck == "AssignStat" && grammarValue.assignmentScope == "program")
            //if (grammarValue.CurrentCheck == "AssignStat")
            {
                if (!grammarValue.AssignStateVarCheck)
                {
                    //check if <<name>> is int then assignmentVariableType = "
                    var scopeKey = string.Format("{0},{1}", "program", name);
                    if (!objvalidatorDeclaration.IsScope(scopeKey))
                    {
                        Console.WriteLine("Validation error");
                    }
                    //grammarValue.AssignStateVarType = "intNum";
                    grammarValue.AssignStateVarType = objDT.getDataType(name);
                    grammarValue.AssignStateVarCheck = true;

                }
                //skip if assignop skip
                else if (name == "AssignOp") { }
                else if (name == "+") { }
                else
                {
                    if (name != grammarValue.AssignStateVarType)
                    {
                        String dictkey = (name + "," + grammarValue.ParentClass);
                        List<String> dictValue = new List<string>();
                        dictValue.Add("Semantic Error" + "," + "Circular Inheritance on classes:+" + name + "," + grammarValue.ParentClass);
                        Lexical_Analyzer.errorDict.Add(dictkey, dictValue);

                    }
                }

                Console.WriteLine(new String('-', depth) + name);
            }

            else if (grammarValue.CurrentFunc == "FuncDef")
            {
                countVisitAssignStatFunc++;
                    if(countVisitAssignStatFunc == 2)
                { astFuncName = name;  }
                Console.WriteLine(new String('-', depth) + name);

            }
            else
            {
                Console.WriteLine(new String('-', depth) + name);
            }
        }

        
    }
    public class Symbol
    {
        public string Name
        {
            get; set;
        }
        public string Kind
        {
            get; set;
        }
        public string Type
        {
            get; set;
        }

        public int size
        {
            get; set;
        }
        public SymbolTable link
        {
            get; set;
        }

        public string st_Name
        {
            get; set;
        }
    }
    public class SymbolTable
    {
        public List<Symbol> _symbols;
        public SymbolTable()
        {
            _symbols = new List<Symbol>();
        }
        public void AddSymbol(Symbol sym)
        {
            _symbols.Add(sym);
        }

        public List<Symbol> GetSymbols() { return _symbols; }


    }
    public class InheritanceValidator
    {
        static Dictionary<String, String> inheritedClass = new Dictionary<String, String>();
        public bool IsCyclicInheritance(string key, string val)
        {
            char[] charArray = key.ToCharArray();
            Array.Reverse(charArray);
            var reversedKey = new string(charArray);

            if (inheritedClass.ContainsKey(reversedKey))
            {
                //Console.WriteLine("Cyclic Inheritance");

                return true;
            }

            else
            {
                if (!inheritedClass.ContainsKey(key))
                {
                    inheritedClass.Add(key, val);
                }
                return false;
            }

            //A_C => C_A ->  A_C (Add)
            //C_A => A_C -> Already Exist
        }

        //add for other checks
    }
    public class DeclarationValidator
    {
        public bool IsScope(string key)
        {
            foreach (KeyValuePair<string, string> dickKey in Parser._dataTypeAndVariablePairToCheck)
            {
                if (dickKey.Key.Equals(key))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class GetDataType
    {
       public string getDataType(string value)
        {

            foreach (KeyValuePair<string, string> dickKey in Parser._dataTypeAndVariablePairToCheck)
            {
                string key = dickKey.Key;
                string[] split = key.Split(',');
                if (split[1].Equals(value))
                {
                    if(dickKey.Value.Equals("int"))
                    return ("intNum");
                    else if (dickKey.Value.Equals("float"))
                        return ("floatNum");
                }
            }
            return ("");
        }
    }
}


